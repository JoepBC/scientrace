// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Scientrace {


/// <summary>
/// In order to keep things properly organised, these methods generating textual/numerical data have been grouped in this separate file.
/// </summary>
public partial class TraceJournal {

	public Dictionary<string, string> exportFields = new Dictionary<string,string>();

	public List<Scientrace.TraceJournalExportable> exportables = new List<TraceJournalExportable>();

	public string yieldtablefilename;
	public bool exportyieldtablehtml = false;
	public int htmldatawritemode = 1001; //append by default
	public bool exportyieldtablecsv = false;
	public int csvdatawritemode = 1001; //append by default

	public const int APPEND = 1001;
	public const int WRITE = 1002;

	/// <summary>
	/// The PhotonDump exports detailed numerical information about individual photons.
	/// </summary>
	public bool exportphotondump = true;
	public string photondumpfilename = "photondump_%o.csv";


		public static string csvLine<T>(List<T> values) {
			return String.Join(",", values.ToArray());
			}

		public static string csvLine(List<double> values) {
			return String.Join(",", values.ToArray());
			}
		
		public static string csvLine(List<string> values) {
			return String.Join(",", values.ToArray());
			}
		
		public static string intToCsv(int anInteger) {
			return Convert.ToString(anInteger);
			}

		public static string doubleToCsv(double aDouble) {
			return Convert.ToString(aDouble);
			}
		
		public static string stringToCsv(string aString) {
			return aString.Replace("\"", "\"\"");
			}



	public void addExportField(string name, string value) {
		//Console.WriteLine("Adding tag name: "+name+" with value: "+value);
		try { this.exportFields.Add(name, value);
			} catch { throw new ArgumentException("Double tag name: "+name+" with value: "+value); }
		}



	public Dictionary<string, string> addConfigVariablesToDictionary(Dictionary<string, string> aDictionary) {
		//CONFIG ID CONTAINS SEVERAL DATAFIELDS:
		string[] cids = this.config_id.Split('%');
		string[] cdescs = this.config_description.Split('%');
		for (int icids = 0; icids < cids.Length; icids++) {
			string description = cdescs[icids];
			string value = cids[icids];
			aDictionary.Add(description, value);
			}
		return aDictionary;
		}

	public string ensureUniqueKey(string key, Dictionary<string, string> aDictionary) {
		if (!aDictionary.ContainsKey(key))
			return key;
		return this.ensureUniqueKey(key+"'", aDictionary);
		}

	/// <summary>
	/// Safely adds a value to a dictionary with a key. If the key already exists, it is altered such that it IS new. The return value is true if the key has been modified, and false if the original key was used.
	/// </summary>
	/// <returns><c>true</c> if the key has been modified, and false if the original key was used
	/// <param name="adict">A <string,string> Dictionary</param>
	/// <param name="key">A string</param>
	/// <param name="value">A string</param>
	public bool safeAddToDict(Dictionary<string, string> adict, string key, string value) {
		bool key_modified = false;
		if (adict.ContainsKey(key)) {
			Console.WriteLine("WARNING: cannot add key {"+key+"} to dictionary. Key already exists. Creating unique version.");
			key = this.ensureUniqueKey(key, adict);
			key_modified = true;
			}
		adict.Add(key, value);
		return key_modified;
		}

	public Dictionary<string, string> exportYieldDataDictionary() {
		Dictionary<string, string> retdict = new Dictionary<string, string>();
		this.addConfigVariablesToDictionary(retdict);
		this.safeAddToDict(retdict,"Timestamp", this.timestamp); // REPLACED LINE BELOW.
		//retdict.Add("Timestamp", this.timestamp);
		double total_intensity = 0;
		double total_shine_duration = 0;
		foreach (Scientrace.LightSource lightsource in this.registeredLightSources) {
			string tag_prefix = lightsource.tag+"-";
			//this.safeAddToDict(retdict,"light source", lightsource.tag); // REPLACED LINE BELOW.
			//retdict.Add("light source",lightsource.tag);
			this.safeAddToDict(retdict,tag_prefix+"cpu time (s)",lightsource.shine_duration.TotalSeconds.ToString()); // REPLACED LINE BELOW.
			//retdict.Add("cpu time (s)",lightsource.shine_duration.TotalSeconds.ToString());
			this.safeAddToDict(retdict,tag_prefix+"#traces", lightsource.traceCount().ToString()); // REPLACED LINE BELOW.
			//retdict.Add("#traces",lightsource.traceCount().ToString());
			this.safeAddToDict(retdict,tag_prefix+"intensity", lightsource.total_lightsource_intensity.ToString()); // REPLACED LINE BELOW.
			//retdict.Add("intensity",lightsource.total_lightsource_intensity.ToString());
			this.safeAddToDict(retdict,tag_prefix+"weighted_intensity", lightsource.weighted_intensity.ToString()); // REPLACED LINE BELOW.
			//retdict.Add("weighted_intensity",lightsource.weighted_intensity.ToString());
			total_intensity = total_intensity + lightsource.total_lightsource_intensity;
			total_shine_duration = total_shine_duration + lightsource.shine_duration.TotalSeconds;
			}
		double total_revenue = 0;
		int isolarcellcount = 1;
		foreach (Scientrace.PhysicalObject3d solarcell in this.registeredPerformanceObjects) {
			retdict.Add("solarcell-"+isolarcellcount,solarcell.tag);
			foreach (Scientrace.LightSource lightsource in this.registeredLightSources) {
				retdict.Add(this.ensureUniqueKey(lightsource.tag+"-yield", retdict),(lightsource.revenueForObject(solarcell)/lightsource.total_lightsource_intensity).ToString());
				retdict.Add(this.ensureUniqueKey(lightsource.tag+"-weighted_sum", retdict),((lightsource.revenueForObject(solarcell)/lightsource.total_lightsource_intensity)*lightsource.weighted_intensity).ToString());
				}
			//retdict.Add("total_revenue-"+isolarcellcount,solarcell.getTotalRevenue().ToString());
			retdict.Add(solarcell.tag+"-yield",(solarcell.getTotalRevenue()/total_intensity).ToString());
			total_revenue = total_revenue + solarcell.getTotalRevenue();
			isolarcellcount++;
			}
		retdict.Add("total light intensity",total_intensity.ToString());
		retdict.Add("total revenue",total_revenue.ToString());
		retdict.Add("total yield",(total_revenue/total_intensity).ToString());


		foreach (string aKey in this.exportFields.Keys) {
			retdict.Add(aKey, TraceJournal.stringToCsv(exportFields[aKey]));
			}

		return retdict;
		}





/* Histogram methods */
	protected void writeExportHistograms(Scientrace.PhysicalObject3d anObject) {
		foreach (TraceJournalExportable exportable in this.exportables) {
			if (exportable.export) {
				exportable.write(anObject);
				//exportable.writeAngleHistogramCSV(anObject);
				}
			}
		}
/* End of Histogram methods */



/* Start of Dictionary export methods */

	public Dictionary<string, string> appendWriteWithConfigVariables<T,U>(string csv_filename, Dictionary<T,U> aDictionary) {
		Dictionary<string, string> retdict = new Dictionary<string, string>();
		this.addConfigVariablesToDictionary(retdict);
		foreach (KeyValuePair<T,U> kv in aDictionary) {
			retdict.Add(kv.Key.ToString(), kv.Value.ToString());
			}
		this.appendWriteDictionary(csv_filename, retdict);
		return retdict;
		}

	public void appendWriteDictionary<T,U>(string csv_filename, Dictionary<T,U> aDictionary) {
		bool write_headers = !System.IO.File.Exists(csv_filename);

		using (StreamWriter histogram_csv_writestream = new StreamWriter(csv_filename, true)) {
			//Only write header (keys) for new file:
			if (write_headers) {
				Console.Write("Writing data to new file: "+csv_filename);
				histogram_csv_writestream.WriteLine(TraceJournal.csvLine(new List<T>(aDictionary.Keys)));
				} else
				Console.Write("Appending data to: "+csv_filename);
			//Always write data (values) - duh
			histogram_csv_writestream.WriteLine(TraceJournal.csvLine(new List<U>(aDictionary.Values)));
			}
		Console.WriteLine(" [done]");
		}

/* End of Dictionary Export methods */


	public void writePhotondumpCSV(Scientrace.PhysicalObject3d anObject) {
		Scientrace.Location loc2d;
		List<string> header = new List<string>();
		
		// FIELDS: IDENTIFIER DIRX DIRY DIRZ SURFACEANGLE LOCX LOCY LOCZ ...
		// ... 2DLOCX 2DLOCY 2DLOCZ WAVELENGTH RGBCOLOUR DISTANCE INTENSITY INTENSITYFRACTION

		header.Add("IDENTIFIER");
		header.Add("DIRX");
		header.Add("DIRY");
		header.Add("DIRZ");
		header.Add("SURFACEANGLE");
		header.Add("LOCX");
		header.Add("LOCY");
		header.Add("LOCZ");
		header.Add("2DLOCX");
		header.Add("2DLOCY"); 
		header.Add("2DLOCZ");
		header.Add("WAVELENGTH");
		header.Add("RGBCOLOR");
		header.Add("DISTANCE");
		header.Add("CASUALTYINTENSITY");
		header.Add("TRACESOURCEINTENSITY");
		header.Add("INTENSITYFRACTION");
		header.Add("Pol1DirX");
		header.Add("Pol1DirY");
		header.Add("Pol1DirZ");
		header.Add("Pol1Len");
		header.Add("Pol2DirX");
		header.Add("Pol2DirY");
		header.Add("Pol2DirZ");
		header.Add("Pol2Len");
		//string csvphotondumpfilename = this.exportpath+this.config_id.Replace("%","-")+this.yieldtablefilename.Replace("%o", anObject.tag)+".tracedump.csv";
		string csvphotondumpfilename = this.exportpath+this.photondumpfilename.Replace("%o",anObject.tag);

		using (StreamWriter csvphotondumpwritestream = new StreamWriter(csvphotondumpfilename)) {
			csvphotondumpwritestream.WriteLine(TraceJournal.csvLine(header));
			foreach(Scientrace.Spot casualty in this.spots) {
				if (casualty == null) {
					Console.WriteLine("WARNING: Casualty null value found. That's weird...");
					}
				List<string> body = new List<string>();
				if (casualty.object3d == anObject) {
					try {
					loc2d = anObject.get2DLoc(casualty.loc);
					body.Add(casualty.trace.traceid); // IDENTIFIER
					body.Add(casualty.trace.traceline.direction.x.ToString()); // DIRX
					body.Add(casualty.trace.traceline.direction.y.ToString()); // DIRY
					body.Add(casualty.trace.traceline.direction.z.ToString()); // DIRZ
					body.Add(anObject.getSurfaceNormal().angleWith(casualty.trace.traceline.direction).ToString());
					body.Add(casualty.loc.x.ToString()); //LOCX
					body.Add(casualty.loc.y.ToString()); //LOCY
					body.Add(casualty.loc.z.ToString()); //LOCZ
					body.Add(loc2d.x.ToString()); //2DLOCX
					body.Add(loc2d.y.ToString()); //2DLOCY
					body.Add(loc2d.z.ToString()); //2DLOCZ
					body.Add(casualty.trace.wavelenght.ToString());
					body.Add(this.wavelengthToRGB(casualty.trace.wavelenght));
					body.Add(casualty.trace.tracedistance.ToString());
					body.Add(casualty.intensity.ToString());
					body.Add(casualty.trace.original_intensity.ToString());
					body.Add(casualty.intensityFraction.ToString());
					body.Add(casualty.pol_vec_1.x.ToString());
					body.Add(casualty.pol_vec_1.y.ToString());
					body.Add(casualty.pol_vec_1.z.ToString());
					body.Add(casualty.pol_vec_1.length.ToString());
					body.Add(casualty.pol_vec_2.x.ToString());
					body.Add(casualty.pol_vec_2.y.ToString());
					body.Add(casualty.pol_vec_2.z.ToString());
					body.Add(casualty.pol_vec_2.length.ToString());
					csvphotondumpwritestream.WriteLine(TraceJournal.csvLine(body));	
										} catch { Console.WriteLine("Some of the attributes messed up..." ); }
				}
			}
		
		
			}
							         
		}
	




	public void writeYieldHTML() {
		XDocument xdoc = new XDocument();
		XElement xhtml = new XElement("html");
		XElement xbody = new XElement("body");
		XElement xtable = new XElement("table");
		xtable.Add(new XAttribute("border", "1"));
		XElement xrow = new XElement("tr");
		XElement xheader = new XElement("tr");

		string[] cids = this.config_id.Split('%');
		string[] cdescs = this.config_description.Split('%');
		for (int icids = 0; icids < cids.Length; icids++) {
			xheader.Add(new XElement("td", cdescs[icids]));
			xrow.Add(new XElement("td", cids[icids]));
			}
		xheader.Add(new XElement("td", "TimeStamp"));
		xrow.Add(new XElement("td", this.timestamp));
		double total_intensity = 0;
		double total_shine_duration = 0;
		int lightsourcescount = 0;
		foreach (Scientrace.LightSource lightsource in this.registeredLightSources) {
			xheader.Add(new XElement("td","lightsource"));
			xrow.Add(new XElement("td", lightsource.tag));
			/*xheader.Add(new XElement("td","direction X"));
			xrow.Add(new XElement("td", lightsource.direction.x));
			xheader.Add(new XElement("td","Y"));
			xrow.Add(new XElement("td", lightsource.direction.y));
			xheader.Add(new XElement("td","Z"));
			xrow.Add(new XElement("td", lightsource.direction.z));*/
			xheader.Add(new XElement("td","calc_seconds"));
			xrow.Add(new XElement("td", lightsource.shine_duration));
			xheader.Add(new XElement("td","number_of_traces"));
			xrow.Add(new XElement("td", lightsource.traceCount()));
			xheader.Add(new XElement("td","intensity"));
			xrow.Add(new XElement("td", lightsource.total_lightsource_intensity.ToString()));
			xheader.Add(new XElement("td","weighted_intensity"));
			xrow.Add(new XElement("td", lightsource.weighted_intensity));
			total_intensity = total_intensity + lightsource.total_lightsource_intensity;
			total_shine_duration = total_shine_duration + lightsource.shine_duration.TotalSeconds;
			lightsourcescount++;
			}
		double total_revenue = 0;
		int solarcellcount = 0;
		foreach (Scientrace.PhysicalObject3d solarcell in this.registeredPerformanceObjects) {
			xheader.Add(new XElement("td", "solarcell"));
			xrow.Add(new XElement("td", solarcell.tag));
			foreach (Scientrace.LightSource lightsource in this.registeredLightSources) {
				xheader.Add(new XElement("td", lightsource.tag+"-yield"));
				xrow.Add(new XElement("td", lightsource.revenueForObject(solarcell)/lightsource.total_lightsource_intensity));
				xheader.Add(new XElement("td", lightsource.tag+"-weighted_sum"));
				xrow.Add(new XElement("td", (lightsource.revenueForObject(solarcell)/lightsource.total_lightsource_intensity)*lightsource.weighted_intensity));
				}
			xheader.Add(new XElement("td", "total_revenue"));
			xrow.Add(new XElement("td", solarcell.getTotalRevenue()));
			xheader.Add(new XElement("td", solarcell.tag+"-yield"));
			xrow.Add(new XElement("td", solarcell.getTotalRevenue()/total_intensity));
			total_revenue = total_revenue + solarcell.getTotalRevenue();
			solarcellcount++;
			}
		xheader.Add(new XElement("td", "total_light_intensity"));
		xrow.Add(new XElement("td", total_intensity));
		xheader.Add(new XElement("td", "total_revenue"));
		xrow.Add(new XElement("td", total_revenue));
		xheader.Add(new XElement("td", "total_yield"));
		xrow.Add(new XElement("td", total_revenue/total_intensity));
		/*xheader.Add(new XElement("td", "total yield (%)"));
		xrow.Add(new XElement("td", (100*total_revenue/total_intensity)+"%"));*/
		/*xheader.Add(new XElement("td", "total time(s)"));
		xrow.Add(new XElement("td", total_shine_duration));*/


		int exportFieldsCount = 0;
		foreach (string aKey in this.exportFields.Keys) {
			exportFieldsCount++;
			xheader.Add(new XElement("td", aKey));
			xrow.Add(new XElement("td", this.exportFields[aKey]));
			}
		/*
		IDictionaryEnumerator e = this.exportFields.GetEnumerator();
		int exportFieldsCount = 0;
		while (e.MoveNext()) {
			exportFieldsCount++;
			xheader.Add(new XElement("td", e.Key.ToString())); //both key and value are already Strings... but anyway
			xrow.Add(new XElement("td", e.Value.ToString()));
		}*/
		
		string fullhtmlfilename = this.exportpath+this.yieldtablefilename.Replace("%o", "l"+lightsourcescount+"s"+solarcellcount+"e"+exportFieldsCount+".html");

		/* If the data should be appended and the file already exists, 
		 * the "header" data should be added first to the table */
		if (this.htmldatawritemode==APPEND) {
			Console.WriteLine("Appending HTML data to: " +fullhtmlfilename);
			if (!this.loadOriginalDatafileInTable(fullhtmlfilename, xtable)) {
				xtable.Add(xheader);
				}
			} else {
			Console.WriteLine("(over)Writing HTML data to: " +fullhtmlfilename);
			xtable.Add(xheader);
			}
		xtable.Add(xrow);
		xbody.Add(xtable);
		xhtml.Add(xbody);
		xdoc.Add(xhtml);
		xdoc.Save(fullhtmlfilename);
/*			using (writestream = 
			new StreamWriter(this.exportpath+this.svgfilename.Replace("%o",anObject.tag))) {
			writestream.Write(this.exportSVG(anObject));
			}*/
		}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="fulldatafilename">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="xtable">
	/// A <see cref="XElement"/>. The table into which currently existing data should be added.
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>. True if file existed and its tr data is appended, false if not.
	/// </returns>
	public bool loadOriginalDatafileInTable(string fulldatafilename, XElement xtable) {
		if (!File.Exists(fulldatafilename)) { return false;}
		XDocument xdoc = XDocument.Load(fulldatafilename);
		xtable.Add(xdoc.Element("html").Element("body").Element("table").Elements("tr"));
		return true;
		}



		public void writeYieldCSV() {
			List<string> header = new List<string>();
			List<string> body = new List<string>();

			Dictionary<string, string> exportData = this.exportYieldDataDictionary();

			foreach (string key in exportData.Keys) {
				header.Add(key);
				body.Add(exportData[key]);
				}

			//WRITING DATA TO FILES:
			int lightsourcescount = this.registeredLightSources.Count;
			int registered_surfaces_count = this.registeredPerformanceObjects.Count;
			int exportFieldsCount = this.exportFields.Count;
			string fullcsvheader = this.exportpath+this.yieldtablefilename.Replace("%o", "l"+lightsourcescount+"s"+registered_surfaces_count+"e"+exportFieldsCount)+"_header.csv";
			string fullcsvbody = this.exportpath+this.yieldtablefilename.Replace("%o", "l"+lightsourcescount+"s"+registered_surfaces_count+"e"+exportFieldsCount)+"_body.csv";
			string fullcsvfilename = this.exportpath+this.yieldtablefilename.Replace("%o", "l"+lightsourcescount+"s"+registered_surfaces_count+"e"+exportFieldsCount)+".csv";
			/* If the data should be appended and the file already exists */
			bool appendcsv = (this.csvdatawritemode==APPEND);
			if (appendcsv) {
				if (!File.Exists(fullcsvfilename)) { 
					Console.WriteLine("Creating file to append CSV data to: " +fullcsvfilename);
					appendcsv = false;
					} else {
					Console.WriteLine("Appending CSV data to: " +fullcsvfilename);
					}
				} else {
				Console.WriteLine("(over)Writing CSV data to: " +fullcsvfilename);
				}


			using (StreamWriter bodycsvwritestream = new StreamWriter(fullcsvbody, appendcsv)) {
				bodycsvwritestream.WriteLine(TraceJournal.csvLine(body));
				}
			using (StreamWriter headercsvwritestream = new StreamWriter(fullcsvheader, false)) {
				headercsvwritestream.WriteLine(TraceJournal.csvLine(header));
				}
			using (StreamWriter fullcsvwritestream = new StreamWriter(fullcsvfilename, this.csvdatawritemode==APPEND)) {
				if (!appendcsv) {
					fullcsvwritestream.WriteLine(TraceJournal.csvLine(header));
					}
				fullcsvwritestream.WriteLine(TraceJournal.csvLine(body));
				}
		}







}}