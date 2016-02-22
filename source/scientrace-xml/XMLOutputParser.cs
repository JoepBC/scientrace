using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ScientraceXMLParser {

	
public class XMLOutputParser : ScientraceXMLAbstractParser	{
		
//	public XMLOutputParser(XElement xel, CustomXMLDocumentOperations X): base(xel, X) {
	public XMLOutputParser():base() {
		}

		
	public void parseOutput(XElement rootXEl) {
		/* parsing output data
		 * writing properties to the (Singleton) TraceJournal
		 */
		XElement xoutput = rootXEl.Element("Output");
		if (xoutput == null) 
			xoutput = new XElement("Output");
		if (xoutput != null) {
			this.parseXOutput(xoutput);
			} else {
			Console.WriteLine("WARNING: No output elements found");
			}
		}

	/// <summary>
	/// Parsing Scientrace output. 
	/// </summary>
	/// <param name="xoutput">
	/// A <see cref="XElement"/>
	/// </param>
	public void parseXOutput(XElement xoutput) {
		//Console.WriteLine("PARSING XOUTPUT");
		Scientrace.TraceJournal tj = Scientrace.TraceJournal.Instance;
		tj.setPath(this.X.getXString(xoutput.Attribute("Path"),  "./out_"+tj.getSourceFilenameNoExt()));
		if (!tj.checkPath()) {
			Console.WriteLine("PARSER MESSAGE: output dir not valid. Unable to write output.");
			return;
			} else {
			//Console.WriteLine("OUTPUT DIR OK");
			}
		XElement xx3d = xoutput.Element("X3D");
		//if (xx3d != null) 
			this.configX3DOutput(xx3d);
		XElement xhist = xoutput.Element("Histogram");
			this.configHistogramOutput(xhist);
		XElement xdata = xoutput.Element("YieldData");
		//if (xdata != null) 
			this.configDataOutput(xdata);
		XElement xsvg = xoutput.Element("SVG");
		//	if (xsvg != null)
			this.configSVGOutput(xsvg); 
		XElement xxml = xoutput.Element("XML");
		//if (xxml != null) 
			this.configXMLOutput(xxml);
		XElement xphotondump = xoutput.Element("PhotonDump");
		//if (xphotondump != null)
			this.configPhotonDumpOutput(xphotondump);
		}

	public void configX3DOutput(XElement xx3d) { 
		Scientrace.TraceJournal tj = Scientrace.TraceJournal.Instance;
		tj.exportx3d = this.X.getXBool(xx3d, "Export", true);
		//Console.WriteLine("WRITING DATA IS " + tj.exportdata.ToString());
		tj.x3dfilename = this.X.getXStringByName(xx3d, "Filename", "3dview_"+tj.getSourceFilenameNoExt()+".x3d");
		}

	public void configXMLOutput(XElement xxml) {
		Scientrace.TraceJournal tj = Scientrace.TraceJournal.Instance;
		tj.xml_display_lightsource = this.X.getXBool(xxml, "DisplayCustomTraces", false);
		tj.xml_display_preprocessed = this.X.getXBool(xxml, "DisplayPreProcessed", false);
		tj.xml_export_lightsources = this.X.getXBool(xxml, "ExportCustomTraces", true);
		tj.xml_export_preprocessed = this.X.getXBool(xxml, "ExportPreProcessed", true);
		tj.xml_export_filename = this.X.getXStringByName(xxml, "Filename", "xml_%o_"+tj.getSourceFilenameNoExt()+".scx");
		}


	public void configPhotonDumpOutput(XElement xphotondump) {
		Scientrace.TraceJournal tj = Scientrace.TraceJournal.Instance;
		tj.exportphotondump = this.X.getXBool(xphotondump, "Export", true);
		tj.photondumpfilename = this.X.getXStringByName(xphotondump, "Filename","dump_%o_"+tj.getSourceFilenameNoExt()+".csv");
		}
		
		
	public void configSVGOutput(XElement xsvg) {
		Scientrace.TraceJournal tj = Scientrace.TraceJournal.Instance;
		tj.exportPDPSources = new HashSet<Scientrace.PDPSource>{};
		if (this.X.getXBool(xsvg, "ExportAngles", true))
				tj.exportPDPSources.Add(Scientrace.PDPSource.AngleWheel);
		if (this.X.getXBool(xsvg, "ExportWavelengths", true))
				tj.exportPDPSources.Add(Scientrace.PDPSource.Wavelength);
		//Console.WriteLine("PDPSOURCESCOUNT:"+tj.exportPDPSources.Count);
		tj.svg_export_photoncloud = this.X.getXBool(xsvg, "PhotonCloud", false);
		tj.exportsvg = this.X.getXBool(xsvg, "Export", true);
		tj.svg_export_legends = this.X.getXBool(xsvg, "Legends", tj.exportsvg);
		tj.svgfilename = this.X.getXStringByName(xsvg,"Filename", tj.getSourceFilenameNoExt()+"_%o.svg");
		tj.svggrid = this.X.getXBool(xsvg, "Grid", true);
		tj.spotdiagonalfraction = this.X.getXDouble(xsvg,"SpotSizeFraction", 1.0/200.0);
		}

	public void configHistogramOutput(XElement xhistogram) {
		Scientrace.TraceJournal tj = Scientrace.TraceJournal.Instance;
		tj.angle_histogram_export = this.X.getXBool(xhistogram, "Export", tj.angle_histogram_export);
		tj.angle_histogram_resolution = this.X.getXDouble(xhistogram, "Resolution", tj.angle_histogram_resolution);
		tj.angle_histogram_from = this.X.getXDouble(xhistogram, "FromAngle", tj.angle_histogram_from);
		tj.angle_histogram_to = this.X.getXDouble(xhistogram, "ToAngle", tj.angle_histogram_to);
		tj.angle_histogram_csv_filename = this.X.getXStringByName(xhistogram, "Filename", tj.angle_histogram_csv_filename);
		}
		
	public void configDataOutput(XElement xdata) {
		Scientrace.TraceJournal tj = Scientrace.TraceJournal.Instance;
		tj.exportyieldtablehtml = this.X.getXBool(xdata, "ExportHTML", true);
		tj.exportyieldtablecsv = this.X.getXBool(xdata, "ExportCSV", true);
		tj.yieldtablefilename = this.X.getXStringByName(xdata, "Filename", tj.getSourceFilenameNoExt()+"_%o");
		string htmlwritemode = this.X.getXStringByName(xdata, "HTMLWriteMode", "append");
		string csvwritemode = this.X.getXStringByName(xdata, "CSVWriteMode", "append");
		switch (htmlwritemode) {
			case "append": {
				tj.htmldatawritemode = Scientrace.TraceJournal.APPEND;
				break;
				}
			case "write": {
				tj.htmldatawritemode = Scientrace.TraceJournal.WRITE;
				break;
				}
			default: {
				throw new XMLException("Unknown data output WriteMode");
				//break; //unreachable
				}
			} //end switch
		switch (csvwritemode) {
			case "append": {
				tj.csvdatawritemode = Scientrace.TraceJournal.APPEND;
				break;
				}
			case "write": {
				tj.csvdatawritemode = Scientrace.TraceJournal.WRITE;
				break;
				}
			default: {
				throw new XMLException("Unknown data output WriteMode");
				//break; //unreachable
				}				
			} //end switch csv
		}			
		
		
	}
		
		
		
}
