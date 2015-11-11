// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Scientrace {


public abstract class LightSpectrum {

	public List<SpectralUnit> spectralUnits = new List<SpectralUnit>();
	public double total_intensity = 0;
	public string tag;
		
	public int modulo_multiplier = 1;
	private bool mod_multip_verified = false;

	public LightSpectrum(int mod_multip) {
		this.tag = this.defaultTag();
		this.modulo_multiplier = mod_multip;
		}

	public virtual string defaultTag() {	
		return this.GetType().ToString();
		}

	public void force_verify_mod_multip() {
		this.mod_multip_verified = false;
		this.verify_mod_multip();
		}
		
	public void verify_mod_multip() {
		//I shall repeat this only once
		if (this.mod_multip_verified) { return; }
		
		if (this.modulo_multiplier == 1) {
			this.mod_multip_verified = true;
			return;
			}
		int wlcount = this.unitCount();
		if (wlcount == 0) { //the UserLightsource class has no intial wavelengths at creation.
			this.mod_multip_verified = false;
			return;
			}
		if (this.modulo_multiplier > 1) {
			if (!this.sharePrimes(this.modulo_multiplier, wlcount)) {
				this.mod_multip_verified = true;
				return; 
				} else {
				throw new Exception("The Modulo Multiplier ("+this.modulo_multiplier+") shares " +
					"at least a single prime with the specified number of wavelengths ("+wlcount+") for "+this.tag+".");
				}
			}
		//if modulo multiplier == 0 or negative, a multiplier is generated.
		this.modulo_multiplier = this.findRelPrimeMultiplier();
		this.mod_multip_verified = true;
		return;
		}

	public int findRelPrimeMultiplier() {
		/* Introducing "multip", a multiplier which is relatively prime to the number of wavelengths */
		int wlcount = this.unitCount();
		int multipl = (wlcount / 3)-1;
		if (this.sharePrimes(wlcount, multipl)) {
			multipl = 2;
			if (this.sharePrimes(wlcount, multipl)) {
				multipl = 1;
				if (this.sharePrimes(wlcount, multipl)) {
					throw new Exception("1 should never share a prime with "+wlcount);
					}
				}
			}
		return multipl;
		}
		
	public bool sharePrimes(int a, int b) {
		//Console.WriteLine("CHECK: Do "+a+" and "+b+" share primes?");			
		int ta = Math.Max(a,b);
		int tb = Math.Min(a,b);
		if (tb < 1) { throw new Exception("LightSpectrum instance.sharePrimes cannot be run for values "+a+" and "+b); }
		if (tb == 1 ) { // 1 isn't prime so no common primes possible.
			return false;
			}
		int remainder;

		do {
			remainder = (ta%tb);
			if (remainder == 0) {
				//Console.WriteLine("YES: remainder==0");
				return true;
				}
			ta = tb;
			tb = remainder;
			} while (remainder != 1);
		//Console.WriteLine("just return false");
		return false;
		}

/* Functionality had to be removed unfortunately due to too much variation in parameters for spectra
	//identifier behaviour
	public abstract string identifier();

	private static Hashtable spectra = new Hashtable();
	public static LightSpectrum FromIdentifier(string identifier) {
		if (LightSpectrum.spectra.Count < 1) { LightSpectrum.buildSpectraList(); }
		return LightSpectrum.getSpectrum(identifier);
		}

	public static void buildSpectraList() {
		LightSpectrum.addSpectrum(new PurdyBlue());
		LightSpectrum.addSpectrum(new AM15Spectrum());
		LightSpectrum.addSpectrum(new TestSpectrum());
		LightSpectrum.addSpectrum(new RangeSpectrum(300, 1700, 100));
		}
	
	public static void addSpectrum(LightSpectrum ls) {
		LightSpectrum.spectra.Add(ls.identifier(), ls);
		}

	public static LightSpectrum getSpectrum(string identifier) {
		return (LightSpectrum)LightSpectrum.spectra[identifier];	
		} */
		
	public int unitCount() {
		return this.spectralUnits.Count;
		}

	public double wl(int i) {
		this.verify_mod_multip();
		return this.spectralUnits[(i*this.modulo_multiplier)%this.unitCount()].wavelength;
		}

	public double it(int i) {
		this.verify_mod_multip();
		return this.spectralUnits[(i*this.modulo_multiplier)%this.unitCount()].intensity;
		}

	public void addNanometerWavelength(double nmwavelength, double intensity) {
		this.addWavelength(nmwavelength*1E-9, intensity);
		}

	public void addWavelength(double wavelengthmeters, double intensity) {
		//Console.WriteLine("wl:"+wavelengthmeters+" int:"+intensity+" added");
		this.total_intensity = this.total_intensity+intensity;
		SpectralUnit aSpectralUnit = new SpectralUnit(wavelengthmeters, intensity);
		this.spectralUnits.Add(aSpectralUnit);
	}


}
}
