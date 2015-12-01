using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ScientraceXMLParser {
	
public class XMLSpectrumParser : ScientraceXMLAbstractParser{

		public XMLSpectrumParser():base(){
		} //end constructor
	
	
	
	public Scientrace.LightSpectrum parseLightSpectrum(XElement xspec) {
		if (xspec == null) {
			Console.WriteLine("- no spectrum defined for lightsource. Using random wavelengths of the visible spectrum.");
			return this.visibleSpectrum();
			}

		string spectrum_id = this.X.getXString(xspec.Attribute("Class"));
		//Chosen to replace line below to give error at erroneous values when given.
		//int mod_multip = this.X.getXInt(xspec, "ModuloMultiplier", 1);
		int mod_multip = 1;
		if (xspec.Attribute("ModuloMultiplier") != null) {
			mod_multip = this.X.getXInt(xspec, "ModuloMultiplier");
			Console.WriteLine("Setting new MOD: "+mod_multip);

		} //else { Console.WriteLine("NOT Setting new MOD for "+xspec.Name.ToString()); }
		//Console.WriteLine(mod_multip+"MOD");
		switch (spectrum_id) {
			case "am15":
				return new Scientrace.AM15Spectrum(mod_multip);
				//break; //no need to break in unreachable code after return
			case "user":
				return this.getUserLightSpectrum(xspec);
				//break;
			case "testspectrum":
				return new Scientrace.TestSpectrum(mod_multip);
				//break;
			case "randomrange":  //DEPRECATED IDENTIFIER, RENAMED TO nmrange FOR CLARITY REASONS.
			case "randomnmrange":
				int rnd_min_nm = this.X.getXInt(xspec, "From");
				int rnd_max_nm = this.X.getXInt(xspec, "To");
				int rnd_entrycount = this.X.getXInt(xspec, "EntryCount", rnd_max_nm - rnd_min_nm);
				int? rnd_seed = this.X.getXNullInt(xspec, "RandomSeed");
				return new Scientrace.RandomRangeSpectrum(rnd_min_nm, rnd_max_nm, rnd_entrycount, rnd_seed);
				//break;
			case "static":
				double wavelength = this.X.getXDouble(xspec, "Wavelength");
				return new Scientrace.SingleWavelengthSpectrum(mod_multip, wavelength);
				//break;
			case "range": //DEPRECATED IDENTIFIER, RENAMED TO nmrange FOR CLARITY REASONS.
			case "nmrange":
				int min_nm = this.X.getXInt(xspec, "From");
				int max_nm = this.X.getXInt(xspec, "To");
				int entrycount = this.X.getXInt(xspec, "EntryCount", this.X.getXInt(xspec, "Resolution", max_nm-min_nm));
				//Console.WriteLine("RANGESPEC: "+mod_multip+"/"+min_nm+"/"+ max_nm+"/"+ res);
				return new Scientrace.RangeSpectrum(mod_multip, min_nm, max_nm, entrycount);
				//break;
			case "purdyblue":
				return new Scientrace.PurdyBlue(mod_multip);
				//break;
			case "visible":
				return this.visibleSpectrum();
				//break;
			default:
				Console.WriteLine("Spectrum class "+spectrum_id+" unknown. Throwing default {visible} spectrum.");
				return this.visibleSpectrum();
			}
		}	//end parseLightSpectrum	

	public Scientrace.LightSpectrum defaultSpectrum() {
		return visibleSpectrum();
		}

	public Scientrace.RandomRangeSpectrum visibleSpectrum() {
		int min_nm = 380;
		int max_nm = 750;
		// Keep the default spectrum seeded for reproducability purposes.
		int? seed = 1;
		return new Scientrace.RandomRangeSpectrum(min_nm, max_nm, max_nm-min_nm, seed);
		}

	public Scientrace.UserLightSpectrum getUserLightSpectrum(XElement xspec) {
		string tag = this.X.getXString(xspec, "Tag");
		Scientrace.UserLightSpectrum retls = new Scientrace.UserLightSpectrum(1, tag);
		//Console.WriteLine("Creating user lightsource "+tag);
		if (xspec.Attribute("CSVData")!= null) {
			Console.WriteLine("Reading from comma seperated value files not yet possible");
			//TODO: parse CSV values from this filename.
			}

		if (xspec.Attribute("XMLData")!= null) {
			string xmldatafilename = this.X.getXString(xspec.Attribute("XMLData"));
			retls.tag = retls.tag+xmldatafilename;
			this.getSpectrumXMLFromXMLFile(retls, xmldatafilename);
			}
		this.addXMLWaveLengths(retls, xspec);
		return retls;
		}


	
	public void getSpectrumXMLFromXMLFile(Scientrace.UserLightSpectrum ls, string filename) {
		if (System.IO.File.Exists(filename)) {
			XDocument xd = XDocument.Load(filename);
			this.addXMLWaveLengths(ls, xd.Element("Spectrum"));
			} 
		else 
			throw new System.IO.FileNotFoundException("Spectrum filename ("+filename+") does not exist");
		}

	
	public void addXMLWaveLengths(Scientrace.UserLightSpectrum ls, XElement xlightspectrum) {
		if (xlightspectrum.Attribute("ModuloMultiplier") != null) {
			ls.modulo_multiplier = this.X.getXInt(xlightspectrum, "ModuloMultiplier");
			}
		//Console.WriteLine("Mod multiplier is now: "+ls.modulo_multiplier);
		if (xlightspectrum.Attribute("Tag") != null) {
			ls.tag = this.X.getXString(xlightspectrum.Attribute("Tag"));
			}
		foreach (XElement wlxel in xlightspectrum.Elements("Wavelength")) {
			double wlintens = this.X.getXDouble(wlxel, "Intensity", 1);
			if (wlintens > 1E-9) { //check for minimal intensity threshold of 1E-9
				if (wlxel.Attribute("nm") == null) 
					ls.addWavelength(this.X.getXDouble(wlxel, "m"), wlintens);
				else 
					ls.addNanometerWavelength(this.X.getXDouble(wlxel, "nm"), wlintens);
				} 
			else
				Console.WriteLine("NOTE: "+wlxel.ToString()+" has neglected intensity of "+wlintens);
			}
		ls.force_verify_mod_multip();
		}
	
	}} //end class + namespace

