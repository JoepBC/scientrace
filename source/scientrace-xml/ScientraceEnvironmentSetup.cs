using System;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using Scientrace;

namespace ScientraceXMLParser{
	public class ScientraceEnvironmentSetup	{
	
	private ScientraceXMLParser sctxp;
	private CustomXMLDocumentOperations X;

	/* variables related to the total configurationfile */
	public string configfilename;

	
	/* environment, lightsource, solarcell, etc. */
//	public Scientrace.LightSource lightsource;
	public Scientrace.Rectangle[] solarcell = new Scientrace.Rectangle[2];
	public Scientrace.Object3dEnvironment env;

	public ScientraceEnvironmentSetup(string configfilename) {
		this.configfilename = configfilename;
		this.X = new CustomXMLDocumentOperations();
		Scientrace.TraceJournal.reset();
		this.parseConfiguration();
		}

	public void parseConfiguration() {
		Console.WriteLine(">>> loading configuration file: " + this.configfilename);

		Scientrace.TraceJournal tj = Scientrace.TraceJournal.Instance;
		tj.source_filename = this.configfilename;

		string xmlsource = this.preProcess(this.configfilename);

		tj.xml_export_preprocessed_string = xmlsource;

		this.sctxp = new ScientraceXMLParser(XDocument.Parse(xmlsource));
		this.env = this.sctxp.parseEnvironment();
		this.sctxp.parseOutput();
		}

	public string preProcess(string filename) {
		string xmlsource = new System.IO.StreamReader(filename).ReadToEnd();

		//If READABLE_CONFIGKEY still hasn't been set, use the ConfigID to do so.
		xmlsource = xmlsource.Replace("$READABLE_CONFIGKEY", TraceJournal.Instance.getSourceFilenameNoExt()).Replace("@READABLE_CONFIGKEY@", TraceJournal.Instance.getSourceFilenameNoExt());

		// double process to make variables within replacement values possible.
		//Console.WriteLine(this.preProcessString(this.preProcessString(this.preProcessFiles(filename, xmlsource))));
		return this.preProcessString(this.preProcessString(this.preProcessFiles(filename, xmlsource)));
		}

	public string getFileContents(string importfilename, string sourcefilename) {
		string retval = "";
		if (System.IO.File.Exists(importfilename)) {
			retval = new System.IO.StreamReader(importfilename).ReadToEnd();
			} else {
			string importfileindir = System.IO.Directory.GetParent(sourcefilename)+"/"+importfilename;
			if (!System.IO.File.Exists(importfileindir)) {
				throw new XMLException("ERROR: Both "+importfilename+" and "+importfileindir+" do not exist. Cannot import value");
				}
			retval = new System.IO.StreamReader(importfileindir).ReadToEnd();
			}
		return retval;
		}

	public string preProcessFiles(string sourcefilename, string xmlsource) {
		XDocument xd = XDocument.Parse(xmlsource);
		XElement xconf = xd.Element("ScientraceConfig");
		if (xconf == null) {
			throw new XMLException("No root element [ScientraceConfig] found in ["+sourcefilename+"]");
			}
		XElement xpreprocess = xconf.Element("PreProcess");
		if (xpreprocess == null) { return xmlsource; }

		foreach (XElement xe in xd.Element("ScientraceConfig").Element("PreProcess").Elements("Import")) {
			string varname = this.X.getXStringByName(xe, "Key"); //xe.Attribute("Key").Value;
			XAttribute importfileatt = xe.Attribute("File");
			string importvalue;
			if (importfileatt == null) {
				XElement importValueElements = xe.Element("Value");
				
				importvalue = String.Join("\n", importValueElements.Nodes());
				/*System.Xml.XmlReader xr = xe.Element("Value").CreateReader();
				xr.MoveToContent();
				importvalue = xr.ReadInnerXml(); */
				//Console.WriteLine(importvalue+"###!");
				} else {
				importvalue = this.getFileContents(importfileatt.Value, sourcefilename);
				}

			//This operation used to be executed before the replace loop below, but it encountered errors. Single value replacement preferred at the end?
			//importvalue = this.parseForFromToElements(xe, importvalue);
			for (int i = 0; i <64; i++) { //allow nested loops up to 64 runs (should be sufficient right?)
				importvalue = this.replaceTwoKeySources(xe, xpreprocess, importvalue);
				}
			//and the final ForFromTo parse:
			importvalue = this.parseForFromToElements(xe, xpreprocess, importvalue);
			//Console.WriteLine("IMPORT VALUE: "+importvalue);
			xmlsource = xmlsource.Replace("$"+varname, importvalue).Replace("@"+varname+"@", importvalue);
			}
		//Console.WriteLine("FINAL SOURCE: "+xmlsource);
		return xmlsource;
		}

	public string replaceTwoKeySources(XElement x1, XElement x2, string xmlsource) {
		return this.replaceKeys(x2, this.replaceKeys(x1, xmlsource));
		}

	public string replaceKeys(XElement xsource, string xmlsource) {
		foreach (XElement xe in xsource.Elements("Replace")) {
			string varname = this.X.getXStringByName(xe, "Key");
			string tovalue;
			if (xe.Element("Value") != null) {
				System.Xml.XmlReader xr = xe.Element("Value").CreateReader();
				xr.MoveToContent();
				tovalue = xr.ReadInnerXml();
				} 
			else
				tovalue = this.X.getXStringByName(xe, "Value");
			//xmlsource = xmlsource.Replace("$"+varname, tovalue).Replace("@"+varname+"@", tovalue);
			xmlsource = this.replaceKeyValue(xmlsource, varname, tovalue);
			}
		return xmlsource;
		}


	public string replaceKeyValue(string xml_source, string aKey, string aValue) {
		return xml_source.Replace("$"+aKey, aValue).Replace("@"+aKey+"@", aValue);
		}

	public string solveKeys(XElement xsource, string xmlsource) {
		foreach (XElement xe in xsource.Elements("Solve")) {
			string varname = this.X.getXStringByName(xe, "Key");
			string formula = this.X.getXStringByName(xe, "Formula");
			xmlsource = this.replaceKeyValue(xmlsource, varname, MathStrings.solveString(formula).ToString());
			}
		return xmlsource;
		}

	public string parseForFromToElements(XElement ximport, XElement xpreprocess, string importXMLSource) {
		string retxml = importXMLSource;
		foreach (XElement xfor in ximport.Elements("For")) {
			retxml = this.performForElementExplode(xfor, retxml, ximport, xpreprocess);
			}
		return retxml;
		}		

	public string performForElementExplode(XElement xfor, string xmlcopysource, XElement ximport, XElement xpreprocess) {
		XElement replacedXFor = XElement.Parse(this.replaceTwoKeySources(ximport, xpreprocess, xfor.ToString()));
		string retval = "";
		string forKey =  this.X.getXStringByAttribute(replacedXFor, "Key");
		double forFrom = this.X.getXDouble(replacedXFor, "From");
		double forTo = this.X.getXDouble(replacedXFor, "To");
		double forStep = this.X.getXDouble(replacedXFor, "Step");
		int sign = Math.Sign(forStep);
		for (double iDouble = forFrom*sign; iDouble <= forTo*sign; iDouble += forStep*sign) {
			string iVal = (iDouble*sign).ToString();
			string add_xml = xmlcopysource.Replace("$"+forKey, iVal).Replace("@"+forKey+"@", iVal);
			retval = retval + add_xml;
			}
		return retval;
		}
		
	public string preProcessString(string xmlsource) {
		XDocument xd;
		try {
			xd = XDocument.Parse(xmlsource);
			} catch (Exception e) {
			throw new XMLException("ERROR PARSING XMLSOURCE: \n"+xmlsource+"\n"+e.Message);
			}
		if (xd.Element("ScientraceConfig").Element("PreProcess") == null) { return xmlsource; }
		xmlsource = this.replaceKeys(xd.Element("ScientraceConfig").Element("PreProcess"), xmlsource);

		// Reload "xd" for solveKeys:
		XDocument newXd;
		try {
			newXd = XDocument.Parse(xmlsource);
			} catch (Exception e) {
			throw new XMLException("ERROR PARSING ALREADY PARSED XMLSOURCE: \n"+xmlsource+"\n"+e.Message);
			}
		xmlsource = this.solveKeys(newXd.Element("ScientraceConfig").Element("PreProcess"), xmlsource);
		//Console.WriteLine(xmlsource);
		return xmlsource;
		}
	

	public void simulate(Dictionary<string,string> argumentDictionary) {
//		this.setupObject3dEnvironment();
		DateTime startTime;
		if (this.env.lightsources.Count > 1) {
			Console.WriteLine("Now foreaching "+this.env.lightsources.Count+" lightsources");
			}
		foreach (Scientrace.LightSource light in this.env.lightsources) {
			if (argumentDictionary.ContainsKey("t"))
				light.lightsource_shine_threads = Convert.ToInt32(argumentDictionary["t"]);
				
			//double intensity =
			light.createStartTracesOnEmpty();
			Console.WriteLine("Shining "+light.traceCount()+" traces within "+light.lightsource_shine_threads+" threads, polarisation support is "+(Trace.support_polarisation?"on":"off")+".");
			startTime = DateTime.Now;
			light.shine();
				
			DateTime stopTime = DateTime.Now;
			TimeSpan duration = stopTime-startTime;
			Console.WriteLine("Shining "+light.traceCount()+" beams took "+light.shine_duration+"/"+duration.TotalSeconds+"s"); //, of which "+Scientrace.VectorTransform.total_inverse_calculation_time+"ms on calculating inverse matrices.");
//			Console.Write("OPTIC REVENUE LEFT CELL: "+(100*this.solarcell[0].getRevenue()/intensity)+"%, ");
//			Console.WriteLine("OPTIC REVENUE RIGHT CELL: "+(100*this.solarcell[1].getRevenue()/intensity)+"%");
			Scientrace.TraceJournal.Instance.registerLightSource(light);
			}
		Scientrace.TraceJournal.Instance.exportAll(this.env);
		}		



}}

