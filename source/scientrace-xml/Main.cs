using System;
using System.Xml.Linq;
using System.Collections.Generic;
using Scientrace;


using System.Linq; 
// "A generic 'IsIn' method from http://www.thecodepage.com/post/Quick-Tip-A-generic-In-method.aspx
public static class ExtensionMethods { 
    public static bool IsIn<T>(this T source, params T[] values) {
        if (values == null) { return false; } 
        return values.Contains(source); 
    } 
}


namespace ScientraceXMLParser {
	

class MainClass {

	public static Dictionary<string, string> getArgsDictionary(string[] arguments) {
		Dictionary<string, string> retDict = new Dictionary<string, string>();
		int nonamecount = 0;
		for (int i = 0; i < arguments.Length; i++) {
			if (arguments[i].Substring(0, 2) == "--") {
				retDict.Add(arguments[i].Substring(2, arguments[i].Length-2), arguments[i+1]);
				i++;
				continue;
				}
			if (arguments[i].Substring(0, 1) == "-") {
				retDict.Add(arguments[i].Substring(1, arguments[i].Length-1), arguments[i+1]);
				i++;
				continue;
				}
			retDict.Add(nonamecount.ToString(), arguments[i]);
			nonamecount++;
			}
		return retDict;
		}

	public static void Main (string[] args)	{
	
		ScientraceEnvironmentSetup setup;
		
		Dictionary<string,string> argumentDictionary = MainClass.getArgsDictionary(args);
		if (!argumentDictionary.ContainsKey("0")) {
			Console.WriteLine("\nERROR: Please specify a Scientrace XML (SCX) file to parse.");
			Console.WriteLine("Consult https://wiki.scientrace.org/Scientrace_Configuration_XML for more information.");
			Console.WriteLine("----\n\n");
			Console.WriteLine("Terminating Scientrace XML parser process.\n");
			Environment.Exit(0xA0);
		
			throw new ArgumentException("No arguments were given! Must have at least a filename for a Scientrace XML Configfile to parse! \n Consult https://wiki.scientrace.org/Scientrace_Configuration_XML for more information.");
			}
		string configfilename = argumentDictionary["0"];
		setup = new ScientraceEnvironmentSetup(configfilename); // build environment based on configfile contents.
		
		setup.simulate(argumentDictionary); //run simulation on environment

		Console.WriteLine("[Done] .");
		Environment.Exit(0);
	}




}
}
