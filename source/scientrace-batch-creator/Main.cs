using System;
using System.Reflection;
//using System.Collections;
using System.Collections.Generic;

namespace BatchCreator {
	
class MainClass	{
		
	public static void Main (string[] args)	{
			Console.WriteLine("Now running Scientrace Batch Creator version: "+Assembly.GetExecutingAssembly().GetName().Version);
		/*List<Dictionary<string, string>> valuePairs = new List<Dictionary<string, string>>();
		valuePairs.Add(new Dictionary<string,string>());
		valuePairs[0].Add("Naam", "Waarde");
		valuePairs[0].Add("Naam2", "Waarde2");
		foreach (KeyValuePair<string, string> vp in valuePairs[0]) {
			Console.WriteLine(vp.Key + " has value "+vp.Value);
			}
		//Exploder parser = */
			new BatchCreator(new Arguments(args), args);
		}

	}
	}