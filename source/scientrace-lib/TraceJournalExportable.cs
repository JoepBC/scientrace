// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

using System.IO;

using System.Collections.Generic;

namespace Scientrace {
public abstract class TraceJournalExportable {

	protected Scientrace.TraceJournal tj;
	public bool export = true;

	public Dictionary<string, string> extra_exportable_config_vars = new Dictionary<string, string>();

	public string tag = "an_exportable";

	public TraceJournalExportable(TraceJournal aTraceJournal) {
		this.tj = aTraceJournal;
		}

	public Dictionary<string, string> addConfigVariablesToDictionary(Dictionary<string, string> aDictionary) {
		this.tj.addConfigVariablesToDictionary(aDictionary);
		foreach (Scientrace.LightSource lightsource in this.tj.registeredLightSources) {
			aDictionary.Add(lightsource.tag, lightsource.total_lightsource_intensity.ToString());
			aDictionary.Add("1_source_total_"+lightsource.tag, lightsource.weighted_intensity.ToString());
			}
		foreach (KeyValuePair<string,string> kv in this.extra_exportable_config_vars) {
			aDictionary.Add(kv.Key.ToString(), kv.Value.ToString());
			}
		return aDictionary;
		}


 

	public abstract void write(Scientrace.PhysicalObject3d anObject);

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

}}

