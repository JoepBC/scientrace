// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Threading;

namespace Scientrace {

[Flags]
public enum PDPSource {
	Wavelength = 0,
	AngleWheel = 1,
	}
	


public partial class TraceJournal {

	//Singleton instance "holder"
	private static TraceJournal instance;

	private Semaphore spot_sema = new Semaphore(1, 1, "add_spot_semaphore");
	private Semaphore angle_sema = new Semaphore(1, 1, "add_angle_semaphore");
	private Semaphore casualty_sema = new Semaphore(1, 1, "add_casualty_semaphore");
	private Semaphore trace_sema = new Semaphore(1, 1, "add_trace_semaphore");

	public System.Random rnd = new System.Random(1);
		
	public string source_filename = "";

	public string config_id = "unknown";
	public string config_description = "unknown";
	public string timestamp = "r_"+String.Format("{0:s}",DateTime.Now);

	public string exportpath = "./"; //default path is current dir

	public bool exportx3d = false;
	public string x3dfilename;

	/// <summary>
	/// These options are specific for the Scientrace-XML-Parser. 
	/// The XML Export options allow the user to export both the PreProcessed code 
	/// after replacing all variables and export of the LightSource in terms of all
	/// single traces that were produced by the lightsource.
	/// </summary>
	public bool xml_display_lightsource = false;
	public bool xml_display_preprocessed = false;
	public string xml_export_filename = "xml_%o_@READABLE_CONFIGKEY@.scx";
	public bool xml_export_preprocessed = true;
	public string xml_export_preprocessed_string = "";
	public bool xml_export_lightsources = true;


	/// <summary>
	/// Infrared light is printed as gray values in the SVG PDP's. The "colour" is expressed as a gradient between black at 780nm
	/// to a gray of all MAX_GRAY_FRACTION RGB values at 1780nm. If this constant is set to 1, 1780nm is printed "white". Lower
	/// values will have darker gray values.
	/// </summary>
	public const double MAX_GRAY_FRACTION = 0.6;

	/* Attributes concerning SVG export */
	public bool svg_export_legends = false;
	public bool exportsvg = false;
	public bool svggrid = true;
	public bool inline_legend = true;
	public HashSet<Scientrace.PDPSource> exportPDPSources = new HashSet<PDPSource>{Scientrace.PDPSource.Wavelength, Scientrace.PDPSource.AngleWheel};
	public string svgfilename;
	public bool svg_export_photoncloud = false;
		
	/* Attributes concerning X3D export */
	public bool drawInteractionPlanes = false;
	public bool drawInteractionNormals = false;
	public bool drawAngles = false;
	public bool drawSpots = false;
	public bool drawCasualties = false;
	public bool drawTraces = true;
	public bool x3dWavelengthColourLines = true;

	public ArrayList traces = new ArrayList(100000);
	public ArrayList spots = new ArrayList(100000);
	public ArrayList angles = new ArrayList(100000);
	public ArrayList casualties = new ArrayList(100000);
	public double spotsize;// = null; //0.1E-3; //radius of a spot in a 2D export set to 0.2 mm.
	public double spotdiagonalfraction = 0.005;


	/* A list of SolarCells to calculate their yields and export to datafiles */
	public List<Object3d> registeredPerformanceObjects = new List<Object3d>();

	/* An array of LightSources to make the calculation of the solarcell yields possible */
	public ArrayList registeredLightSources = new ArrayList();

	protected TraceJournal() {
	}

	public static void reset() {
		TraceJournal.instance = null;
		}

	public static TraceJournal Instance {
		get {
			//lazy init
			if (TraceJournal.instance == null) {
				TraceJournal.instance = new TraceJournal();
			}
			return TraceJournal.instance;
		}
	}

	public string getSourceFilenameNoExt() {
		return Path.GetFileNameWithoutExtension(this.source_filename);
		}

	public void recordTrace(Scientrace.Trace trace) {
		this.trace_sema.WaitOne();
			this.traces.Add(trace);
		this.trace_sema.Release();
		}


	public void recordAngle(Scientrace.Angle angle) {
		this.angle_sema.WaitOne();
			this.angles.Add(angle);
		this.angle_sema.Release();
		}

	public void recordCasualty(Scientrace.Spot casualty) {
		this.casualty_sema.WaitOne();
			if (!casualty.loc.isValid()) {
				throw new ArgumentOutOfRangeException("Casualty has an invalid location.");
				}
			this.casualties.Add(casualty);
		this.casualty_sema.Release();
		}


	public void recordSpot(Scientrace.Spot spot) {
		this.spot_sema.WaitOne();
			if (spot == null)
				throw new NullReferenceException("ERROR: why would you record a null value?");
			this.spots.Add(spot);
		this.spot_sema.Release();
		}

	public void checkNullSpots(string routine_id) {
		this.spot_sema.WaitOne();
		foreach (Spot aspot in this.spots)
						if (aspot == null) Console.WriteLine("Null spot found at "+routine_id);
		this.spot_sema.Release();
		}

	public string exportX3D(Scientrace.Object3dEnvironment env) {
		//Console.WriteLine("Writing journal to X3D");
		StringBuilder retsb = new StringBuilder(500000); //let's just preserve 0.5MB's for starters. Much faster than continuously increasing.
		if (this.drawTraces)
			this.writeX3DTraces(retsb);
		if (this.drawAngles)
			this.writeX3DAngles(env, retsb);
		if (this.drawSpots)
			this.writeX3DSpots(env, retsb);
		if (this.drawCasualties) 
			this.writeCasualties(env, retsb);
		return retsb.ToString();
	}

	public void setPath(string path) {
		if (((path[path.Length-1]) != '/') && ((path[path.Length-1]) != '\\')) {
			this.exportpath = path+"/";
			} else {
			this.exportpath = path;
			}
		}

	public string removeFinalSlashes(string aString) {
		while (((aString[aString.Length-1]) == '/') || ((aString[aString.Length-1]) == '\\')) {
			aString = aString.Substring(0, aString.Length-1);
			}
		return aString;
		}

	public System.IO.DirectoryInfo checkOrCreateSubdir(string parentpath, string subdirname) {
		string endSeparatorlessParentPath = this.removeFinalSlashes(parentpath);
		System.IO.DirectoryInfo fullpath = 
				new System.IO.DirectoryInfo(endSeparatorlessParentPath+System.IO.Path.DirectorySeparatorChar+subdirname);
		if (fullpath.Exists) { //subdir already exists, do nothing
			return fullpath;
			}
		System.IO.DirectoryInfo parentdir = fullpath.Parent;
		if (!parentdir.Exists) { //if parent does not exist, do not attempt to create subdir at all
			Console.WriteLine("Cannot locate/create directory ("+fullpath.FullName+
				                  "). Parent ("+parentdir.FullName+") doesn't exist either.");
			return null;
			}
		Console.Write("Output path \""+fullpath.FullName+"\" does not exist. Would you like Scientrace to create it? [y/N]");
//		try {
			ConsoleKeyInfo readk = Console.ReadKey();
			Console.WriteLine();
			if ((readk.KeyChar == 'y') || (readk.KeyChar == 'Y')) {
				System.IO.DirectoryInfo subdir= parentdir.CreateSubdirectory(subdirname);
				if (subdir.Exists) {
					Console.Write("Subdirectory \""+subdir.FullName+"\" was succesfully created.");
					return subdir;
					} else {
					Console.WriteLine("ERROR Creating directory");
					return null;
					}
				}
			//} catch {
			//throw new Exception("Cannot \"Console.ReadKey()\", perhaps Windows doesn't properly support consoles?");
			//}
		Console.WriteLine("WARNING: "+fullpath.FullName+" was NOT created.");
		return null;
		}		
		
	public bool checkPath() {
		string stringlesspath = this.removeFinalSlashes(this.exportpath);
		if (System.IO.Directory.Exists(stringlesspath)) {
			return true;
			}
		string parentpath = System.IO.Directory.GetParent(stringlesspath).FullName;
		if(!System.IO.Directory.Exists(parentpath)) {
			Console.WriteLine("Invalid directory ("+stringlesspath+"). Parent ("+parentpath+") doesn't exist either.");
			return false;
			}
		Console.Write("Output path \""+stringlesspath+"\" does not exist. Would you like Scientrace to create it? [y/N]");
		try {
			ConsoleKeyInfo readk = Console.ReadKey();
			Console.WriteLine();
			if ((readk.KeyChar == 'y') || (readk.KeyChar == 'Y')) {
				System.IO.Directory.CreateDirectory(stringlesspath);
				if (System.IO.Directory.Exists(stringlesspath)) {
					return true;
					} else {
					Console.WriteLine("ERROR Creating directory");
					return false;
					}
				}

			} catch {
			throw new Exception("Cannot \"Console.ReadKey()\", perhaps Windows doesn't properly support consoles?");
			}
		return false;
		}

	public void exportAll(Scientrace.Object3dEnvironment env) {
			if (this.exportx3d)
				this.writeX3D(env);
			
			// The method probes "if (this.export_lightsources_xml)" inside because it also has a "display" flag.
			foreach (Scientrace.LightSource aLightSource in this.registeredLightSources)
				this.writeLightsourceDumpXML(aLightSource);

			// PreProcessed XML code display and/or export to file based on export flags.
			this.exportPreProcessed();

			//Write SVG dispesrion files for SolarCells
			foreach (Scientrace.PhysicalObject3d anObject in this.registeredPerformanceObjects) {
				if (this.exportphotondump)
					this.writePhotondumpCSV(anObject);
				this.writeExportHistograms(anObject);
				if (this.exportsvg) {
					this.writeSVG(anObject);
					}
				}

			if (this.svg_export_legends)
				this.writeSVGLegends();

			if (this.exportyieldtablehtml) {
				this.writeYieldData();
			}

		}
		
		/// <summary>
		/// Image or numerical data from a string can be exported to a file with given filename.
		/// </summary>
		/// <param name="export_data">The file contents</param>
		/// <param name="filename">The filename</param>
		/// <param name="data_description">A description for display purposes. Pass "" to avoid display.</param>
		public void writeExportDataToFile(string export_data, string filename, string data_description, DateTime start_time) {
			StreamWriter writestream;
			string filename_fullpath = this.exportpath+filename;
			if (data_description.Length > 0) //only display when data_description is given.
				Console.Write("Writing "+data_description+" to: " +filename_fullpath);
			using (writestream = new StreamWriter(filename_fullpath))
				writestream.Write(export_data);
			TimeSpan duration = DateTime.Now-start_time;
			Console.WriteLine(" [done in "+duration.TotalSeconds+"s]");
			}




		public void writeLightsourceDumpXML(Scientrace.LightSource aLightsource) {
			DateTime start_time = DateTime.Now;
			XElement xlight = aLightsource.exportCustomTracesXML();

			string filename = this.xml_export_filename.Replace("%o", aLightsource.tag);
			if (this.xml_export_lightsources) 
				this.writeExportDataToFile(xlight.ToString(), filename, aLightsource.tag+"-custromtraces-XML", start_time);
			if (this.xml_display_lightsource)
				Console.WriteLine(xlight.ToString());
			}
			
			
			/*string csvphotondumpfilename = this.exportpath+this.photondumpfilename.Replace("%o",anObject.tag);
			using (StreamWriter xmlLightSourceDumpWritestream = new StreamWriter(csvphotondumpfilename)) {
				xmlLightSourceDumpWritestream.WriteLine("FOO");
				}*/
			//}
			
		
		public void exportPreProcessed() {
			DateTime start_time = DateTime.Now;
			if (this.xml_display_preprocessed) {
				Console.WriteLine("<!-- *** PreProcessed XML *** -->");
				Console.WriteLine(this.xml_export_preprocessed_string);
				Console.WriteLine("<!-- *** END OF PreProcessed XML *** -->");
				}
			if (this.xml_export_preprocessed) {
				string filename = this.xml_export_filename.Replace("%o",this.config_id);
				this.writeExportDataToFile(this.xml_export_preprocessed_string, filename, "PreProcessed SCX", start_time);
				}
			}

		/* help-methods for exportAll function */
		public void writeX3D(Scientrace.Object3dEnvironment env) {
			DateTime start_time = DateTime.Now;
			string filename = this.x3dfilename.Replace("%o", env.tag);
			this.writeExportDataToFile(env.exportX3D(), filename, "X3D", start_time);
			}

		public void writeSVGLegends() {
			//EXPORTING SVG legends (building dummy legends, TODO: read from legends-array and implement functionality to build them)
			Scientrace.WavelengthLegendBuilder legend = new Scientrace.WavelengthLegendBuilder();

			//Previous functionality: write legens in subdir. Now all in output dir.
			//System.IO.DirectoryInfo outdir = this.checkOrCreateSubdir(this.exportpath, this.config_id);
			System.IO.DirectoryInfo outdir = new System.IO.DirectoryInfo(this.exportpath);
			if (outdir != null) {
				legend.writeSVGFile(outdir);
				}
			}


		public void writeSVG(Scientrace.PhysicalObject3d anObject) {

			foreach (Scientrace.PDPSource pdpSource in this.exportPDPSources)
				foreach (Scientrace.SurfaceSide side in anObject.surface_sides) {
					DateTime start_time = DateTime.Now;
					//Write down SVG trace-pattern of an object for side "side"
					string filename = this.svgfilename.Replace("%o",anObject.tag+"-"+side.ToString()+"-"+pdpSource.ToString());
								this.writeExportDataToFile(this.exportSVG(anObject, side, pdpSource), filename, side.ToString()+"@"+anObject.tag+"-SVG", start_time);
					}

			/*
			StreamWriter writestream;

			foreach (Scientrace.PDPSource pdpSource in this.exportPDPSources)
			foreach (Scientrace.SurfaceSide side in anObject.surface_sides) {

				//Write down SVG trace-pattern of an object for side "side"
				string filename = this.exportpath+this.svgfilename.Replace("%o",anObject.tag+"-"+side.ToString()+"-"+pdpSource.ToString());
				Console.WriteLine("Writing SVG to: " +filename);
				using (writestream = new StreamWriter(filename)) {
					writestream.Write(this.exportSVG(anObject, side, pdpSource));
					}
				}
*/
			}

		public void writeYieldData() {
			//Write down data/statistics considering yields etc.
			this.writeYieldHTML();
			this.writeYieldCSV();
			}




	public void registerObjectPerformance(Scientrace.Object3d aCell) {
		this.registeredPerformanceObjects.Add(aCell);
		}

	public void registerLightSource(Scientrace.LightSource lightsource) {
		this.registeredLightSources.Add(lightsource);
		}
			
}
}
