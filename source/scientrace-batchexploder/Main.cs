using System;
using System.Reflection;
//using System.Collections;
using System.Collections.Generic;

namespace BatchExplode {
	
class MainClass	{
		
	public static void Main (string[] args)	{
			Console.WriteLine("Now running BatchExploder version: "+Assembly.GetExecutingAssembly().GetName().Version);
		/*List<Dictionary<string, string>> valuePairs = new List<Dictionary<string, string>>();
		valuePairs.Add(new Dictionary<string,string>());
		valuePairs[0].Add("Naam", "Waarde");
		valuePairs[0].Add("Naam2", "Waarde2");
		foreach (KeyValuePair<string, string> vp in valuePairs[0]) {
			Console.WriteLine(vp.Key + " has value "+vp.Value);
			}
		//Exploder parser = */
			new Exploder(new Arguments(args), args);
		}

	}
	}