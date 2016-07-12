using System;
using System.Xml.Linq;
using Extensions;
using ScientraceXMLParser;

namespace BatchCreator {

	
public class KeyExplodeArray : ConfigArray {
		
	public double valuefrom, valueto, valuestepsize, currentvalue, filenameValueAdd;
	public string mask = "{0}";
		
	public KeyExplodeArray (string name, double vfrom, double vto, double vstep) {
		this.name = name;
		this.valuefrom = vfrom;
		this.reset();
		this.valueto = vto;
		this.valuestepsize = vstep;
		}

	public KeyExplodeArray (XElement xe) {
		CustomXMLDocumentOperations X = new CustomXMLDocumentOperations();
		this.name = xe.Attribute("Key").Value;
		try {
			this.valuefrom = X.getXDouble(xe, "From");
			} catch(Exception e) { Console.WriteLine("Parser error for key "+this.name+": ");
				throw(e);
			}
		//this.valuefrom = Convert.ToDouble(xe.Attribute("From").Value);
		this.reset();

		try {
			this.valueto = X.getXDouble(xe, "To");
			} catch(Exception e) { Console.WriteLine("Parser error for key "+this.name+": ");
				throw(e);
			}
			
		//this.valueto = Convert.ToDouble(xe.Attribute("To").Value);
		try {
			this.valuestepsize = X.getXDouble(xe, "Step");
			} catch(Exception e) { Console.WriteLine("Parser error for key "+this.name+": ");
				throw(e);
			}
		//this.valuestepsize = Convert.ToDouble(xe.Attribute("Step").Value);

		if (xe.Attribute("AddToFilenameValue") != null) {
			this.filenameValueAdd = X.getXDouble(xe, "AddToFilenameValue");
			}
			
		if (xe.Attribute("Decimals") != null) {
			int decimals = Convert.ToInt32(xe.Attribute("Decimals").Value);
			if (xe.Attribute("PreDecimals") != null) {
				int predecimals = Convert.ToInt32(xe.Attribute("PreDecimals").Value);
				this.mask = "{0:"+"0".Multiply(predecimals)+"."+"0".Multiply(decimals)+"}";
				} else {
				this.mask = "{0:0."+"0".Multiply(decimals)+"}";
				}
			}
		}
		
	public string getCurrent() {
		return String.Format(this.mask, this.currentvalue);
		}

	public string getCurrentFilenameValue() {
		return String.Format(this.mask, this.currentvalue+this.filenameValueAdd);
		}		
		
	public override void reset() {
		this.currentvalue = this.valuefrom;
		}
		
	public override void inc() {
		this.currentvalue = this.currentvalue + this.valuestepsize;	
		}
	
	public override bool EOF() {
		return (this.currentvalue*Math.Sign(this.valuestepsize) > this.valueto*Math.Sign(this.valuestepsize));
		}
		
	public override string replaceForCurrentValues(string aString) {
		return BatchCreator.replaceKeyValues(aString, this.name, this.getCurrent());
		//return aString.Replace("$"+this.name, this.getCurrent());
		}

	public override string replaceForCurrentFilenameValues(string aString) {
		return BatchCreator.replaceKeyValues(aString, this.name, this.getCurrentFilenameValue());
		//return aString.Replace("$"+this.name, this.getCurrentFilenameValue());
		}
		
		
	}}
