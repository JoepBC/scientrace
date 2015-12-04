using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace BatchExplode {
	
public class SubBatchArray : ConfigArray {

	List<Dictionary<string, string>> valueSets = new List<Dictionary<string, string>>();
	
	int iListEntry = 0;
	
	public SubBatchArray (XElement SubBatch) {
		if (SubBatch.Attribute("Tag")!=null) {
			this.name = SubBatch.Attribute("Tag").Value;
			}
		foreach (XElement vs in SubBatch.Elements("ValueSet"))  {
			string batch_id = "unknown";
			if (vs.Attribute("Tag")!=null)
				batch_id = vs.Attribute("Tag").Value;
			this.createNewSet();
			this.addEntry(this.name, batch_id);
			foreach(XElement rxe in vs.Elements("Replace")) {
				if (rxe.Attribute("Value")!=null) {
					//value assignment via attribute
					this.addEntry(rxe.Attribute("Key").Value, rxe.Attribute("Value").Value);
					} else {
					//value assignment via element
					System.Xml.XmlReader xr = rxe.Element("Value").CreateReader();
					xr.MoveToContent();
					// PERFORMING OUTPUT MOVES READER POINTER Console.WriteLine("Assignment via element "+rxe.Attribute("Key")+" ="+xr.ReadInnerXml());
					this.addEntry(rxe.Attribute("Key").Value, xr.ReadInnerXml());
					}
				}
			}
		}

	public void createNewSet() {
		this.valueSets.Add(new Dictionary<string, string>());
		this.inc();
		}

	public void addEntry(string aKey, string aValue) {
		this.currentSet().Add(aKey, aValue);
		}
				
	public override void reset() {
			this.iListEntry = 1;
		}
		
	public override void inc() {
		this.iListEntry++;
		}
	
	public override bool EOF() {
		//Console.WriteLine(this.iListEntry + ">" + this.valueSets.Count);
		return (this.iListEntry > this.valueSets.Count);	
		}

	public Dictionary<string, string> currentSet() {
		if (this.EOF()) {
			return null;	
			}
		return this.valueSets[this.iListEntry-1];
		}

	public override string replaceForCurrentValues(string aString) {
		foreach (KeyValuePair<string, string> vp in this.currentSet()) {
			//REPLACED BY EXPLODER BELOW AT 20150325: aString = aString.Replace("$"+vp.Key, vp.Value);	
			aString = Exploder.replaceKeyValues(aString, vp.Key, vp.Value);
			//Console.WriteLine("REPLACE"+"$"+vp.Key +"TO "+ vp.Value);
			
			//aString = aString.Replace("\""+vp.Key+"\"", "\"_REM:q_"+vp.Key+"\"");	
			}
		return aString;
		}


	}}

