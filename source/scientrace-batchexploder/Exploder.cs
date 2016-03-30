using System;
using System.Xml.Linq;
using System.IO;
using System.Collections;

namespace BatchExplode {
public class Exploder {

	public string source, configfile, outputdir, configkey;
	public ArrayList keys = new ArrayList();
	public string batchid = "ExplodedConfig";
	public XElement xconfig;
		
	public Exploder(Arguments clArgs, string[] args) {

		if (clArgs["config"]==null) {
			if (args.Length < 1) {
				Console.WriteLine("Invalid parameters given. Need 'config' parameter.");
				return;
				}
			this.configfile = args[0];
			} else {
			this.configfile = clArgs["config"];/**/
			}
		/*this.configfile = "/home/jbos/scientrace/suncycle0.44/extendedconfig/config.xml"; /**/

		XDocument xd = XDocument.Load(this.configfile);
		if (xd.Element("BatchConfig") == null) { 
			Console.WriteLine("No Config element found in "+this.configfile);
			return;
			}

		if (xd.Element("BatchConfig").Attribute("Key") == null) { 
			Console.WriteLine("No Key element found in "+this.configfile);
			return;
			}

		if (xd.Element("BatchConfig").Attribute("ID") == null) { 
			Console.WriteLine("No ID element found in "+this.configfile);
			return;
			}

		if (xd.Element("BatchConfig").Attribute("XMLSource") == null) {
			Console.WriteLine("No XMLSource attribute found in "+this.configfile);
			return; 
			} else {
			this.source = xd.Element("BatchConfig").Attribute("XMLSource").Value;
			}

		/*this.configfile = "/home/jbos/BatchExplode/BatchExplode/bin/Debug/config.xml";
		this.source = "/home/jbos/scientrace/suncycle0.3/suncycle_generic.xml";*/
		this.checksourcefiles();
		this.readconfig();
		if (!this.checkPath(this.outputdir)) {
			Console.WriteLine("ERROR: cannot create outputdir: "+this.outputdir);
			return;
			}
		this.run(0);
		}
		
	public void checksourcefiles() {
		if (!System.IO.File.Exists(this.source)) {
			throw new Exception("Sourcefile "+this.source + " does not exist! Shutting down....");
			}
		if (!System.IO.File.Exists(this.configfile)) {
			throw new Exception("Configfile "+this.configfile + " does not exist! Shutting down....");
			}
		}
	
	public void readconfig() {
		XDocument xd = XDocument.Load(this.configfile);
		if (xd.Element("BatchConfig") == null) { 
			Console.WriteLine("No config found in "+this.source);
			return;
			}
		this.xconfig = xd.Element("BatchConfig");
		if (this.xconfig.Attribute("Key") != null) {
			this.configkey = this.xconfig.Attribute("Key").Value;
			}
		if (this.xconfig.Attribute("ID") != null) {
			this.batchid = this.xconfig.Attribute("ID").Value;
			}
		if (this.xconfig.Attribute("OutputDir") != null) {
			this.outputdir = this.xconfig.Attribute("OutputDir").Value;
			} else { this.outputdir = "explodedir"; }
			
		foreach (XElement sbxe in this.xconfig.Elements("SubBatch")) {
			this.keys.Add(new SubBatchArray(sbxe));	
			}
		foreach (XElement exe in this.xconfig.Elements("Explode")) {
			this.keys.Add(new KeyExplodeArray(exe));
			/* this.keys.Add(new KeyExplodeArray(
							exe.Attribute("Key").Value,
							Convert.ToDouble(exe.Attribute("From").Value),
							Convert.ToDouble(exe.Attribute("To").Value),
							Convert.ToDouble(exe.Attribute("Step").Value)
				            )); */
			//Console.WriteLine("another explode element");
			}
		}
		
	public string removeFinalSlashes(string aString) {
		while (((aString[aString.Length-1]) == '/') || ((aString[aString.Length-1]) == '\\')) {
			aString = aString.Substring(0, aString.Length-1);
			}
		return aString;
		}		

	public bool checkPath(string dirname) {
		string slashlesspath = this.removeFinalSlashes(dirname);
		if (System.IO.Path.GetFileName(slashlesspath) == slashlesspath) {
			slashlesspath = System.IO.Directory.GetCurrentDirectory()+"/"+slashlesspath;
			}
		if (System.IO.Directory.Exists(slashlesspath)) {
			return true;
			}
		string parentpath = System.IO.Directory.GetParent(slashlesspath).FullName;
		if(!System.IO.Directory.Exists(parentpath)) {
			Console.WriteLine("Invalid directory ("+slashlesspath+"). Parent ("+parentpath+") doesn't exist either.");
			return false;
			}
		Console.Write("Output path \""+slashlesspath+"\" does not exist. Would you like Scientrace to create it? [y/N]");
		ConsoleKeyInfo readk = Console.ReadKey();
		Console.WriteLine();
		if ((readk.KeyChar == 'y') || (readk.KeyChar == 'Y')) {
			System.IO.Directory.CreateDirectory(slashlesspath);
			if (System.IO.Directory.Exists(slashlesspath)) {
				return true;
				} else {
				Console.WriteLine("ERROR Creating directory");
				return false;
				}
			}
		return false;
		}

	public string replaceKeys(XElement xsource, string xmlsource) {
		foreach (XElement xe in xsource.Elements("Replace")) {
			string varname = xe.Attribute("Key").Value;
			string tovalue;
			if (xe.Attribute("Value")!=null) {
				tovalue = xe.Attribute("Value").Value;
				} else {
				if (xe.Element("Value") == null) {
					throw new Exception("No {Value} attribute or element found for ReplaceKey: {\n"+xe.ToString()+"\n}");
					}
				System.Xml.XmlReader xr = xe.Element("Value").CreateReader();
				xr.MoveToContent();
				tovalue = xr.ReadInnerXml();
				}
			//Generalise key / value replacement (include $ and @@ notation)
			xmlsource = this.replaceVariables(xmlsource, varname, tovalue);
			//xmlsource = xmlsource.Replace("$"+varname, tovalue);
			}
		return xmlsource;
		}		
		
	public static string replaceKeyValues(string source, string key, string value) {
		return source.Replace("$"+key, value).Replace("@"+key+"@", value);
		}

	public string replaceVariables(string source, string key, string value) {
		return Exploder.replaceKeyValues(source, key, value);
		}

	public void run(int arraycounter) {
		if (this.keys.Count < 1) {
			Console.WriteLine("No variable settings found. Now writing single Scientrace setup.");
			this.writeAllValues();
			return;
			}
		ConfigArray key = (ConfigArray)this.keys[arraycounter];
		key.reset();
		Console.WriteLine("Parsing: "+(1+arraycounter)+"/"+this.keys.Count+": "+key.name);
		while (!key.EOF()) {
			if (arraycounter < this.keys.Count - 1	) {
				this.run(arraycounter+1);
				key.inc();
				} else {
				this.writeAllValues();
				key.inc();
				}
			}
		}
				
		
	public void writeAllValues() {
		string replacedkey = this.configkey;
			//Console.WriteLine("REPLACEDKEY:"+replacedkey);
		string xmlsource = new System.IO.StreamReader(this.source).ReadToEnd();
		foreach(ConfigArray key in this.keys) {
			//before introducing AddToFilenameValue functionality replacedkey = key.replaceForCurrentValues(replacedkey);
			replacedkey = key.replaceForCurrentFilenameValues(replacedkey);
			xmlsource = key.replaceForCurrentValues(xmlsource);
			}

		replacedkey = this.replaceKeys(this.xconfig, replacedkey);
		replacedkey = replacedkey.Replace("$", "+"); //replace remaining $ values with + values
		
		xmlsource = this.replaceVariables(xmlsource, "CONFIG_DESCRIPTION", this.configkey.Replace("$", "").Replace("@",""));
		xmlsource = this.replaceVariables(xmlsource, "CONFIGKEY", replacedkey);
		xmlsource = this.replaceVariables(xmlsource, "READABLE_CONFIGKEY", replacedkey.Replace('%', '_'));
		xmlsource = this.replaceVariables(xmlsource, "BATCH_ID", this.batchid);

		xmlsource = this.replaceKeys(this.xconfig, xmlsource);

			
		string sourcefn = System.IO.Path.GetFileName(this.source);
		System.IO.FileInfo	fi = new System.IO.FileInfo(sourcefn);
		string fnextension = fi.Extension;
		string fnbase = System.IO.Path.GetFileNameWithoutExtension(this.source);

//		string fulloutputfilename = this.removeFinalSlashes(this.outputdir)+"/"+replacedkey.Replace('%', '_')+
//				"_"+System.IO.Path.GetFileName(this.source);
		string fulloutputfilename = this.removeFinalSlashes(this.outputdir)+"/"+fnbase+
				"_"+replacedkey.Replace('%', '_')+fnextension;
		Console.WriteLine("TO: "+fulloutputfilename);
			

		StreamWriter writestream;
		using (writestream = 
			new StreamWriter(fulloutputfilename)) {
			writestream.Write(xmlsource);
			}
		}
		
	}
}

