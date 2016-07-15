// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class PMMAProperties : Scientrace.MaterialProperties {


	//Singleton instance "holder"
	private static PMMAProperties instance;

	private PMMAProperties() {
		this.reflects = true;
		this.dielectric = true;
		}

	public override string identifier() {
		return "pmma";
		}

	public static PMMAProperties Instance {
		get {
			//lazy init
			if (PMMAProperties.instance == null) {
				PMMAProperties.instance = new PMMAProperties();
			}
			return PMMAProperties.instance;
		}
	}


	public override double refractiveindex(double wavelength) {
		wavelength = wavelength*1E6; //turn meters into micrometers
		//source www.refractiveindex.info, function of wavelength in micrometers
		return Math.Sqrt(2.399964+-0.08308636*Math.Pow(wavelength, 2)+-0.1919569*Math.Pow(wavelength, -2)+0.08720608*Math.Pow(wavelength, -4)+-0.01666411*Math.Pow(wavelength, -6)+0.001169519*Math.Pow(wavelength, -8));
		}
	}	//end of class PMMAProperties


public class BlackBodySurface : MaterialProperties {
		
	//Singleton instance "holder"
	private static BlackBodySurface instance;
	
	public static BlackBodySurface Instance {
		get {
			//lazy init
			if (BlackBodySurface.instance == null) {
				BlackBodySurface.instance = new BlackBodySurface();
				}
			return BlackBodySurface.instance;
			}
		}
		
	public override string identifier() {
		return "black";
		}
	private BlackBodySurface() {}
	public override double refractiveindex (double wavelength) {
		return 10;
		}
	public override double enterAbsorption (Trace trace, Scientrace.UnitVector norm, Scientrace.MaterialProperties previousMaterial)	{
		return 1;
		}
	} // end of class BlackBodySurface



public class SodaLimeGlassProperties : Scientrace.MaterialProperties {
	//Singleton instance "holder"
	private static SodaLimeGlassProperties instance;

	private SodaLimeGlassProperties() {
		this.reflects = true;
		this.dielectric = true;
		}

	public override string identifier() {
		return "glass";
		}

	public static SodaLimeGlassProperties Instance {
		get {
			//lazy init
			if (SodaLimeGlassProperties.instance == null) {
				SodaLimeGlassProperties.instance = new SodaLimeGlassProperties();
			}
			return SodaLimeGlassProperties.instance;
		}
	}


	public override double refractiveindex(double wavelength) {
		double l = wavelength*1E6; //turn meters into micrometers
		/* source: http://refractiveindex.info/?shelf=glass&book=soda-lime&page=Rubin-clear
		 * valid from wavelengths ranging 310nm to 4100nm
		 */
		return (1.5130-(0.003169*Math.Pow(l,2))+(0.003962*Math.Pow(l,-2)));
		}
	}	//end of class SodaLimeGlassProperties



public class SchottBK7Properties : Scientrace.MaterialProperties {
	//Singleton instance "holder"
	private static SchottBK7Properties instance;

	private SchottBK7Properties() {
		this.reflects = true;
		this.dielectric = true;
		}

	public override string identifier() {
		return "bk7";
		}

	public static SchottBK7Properties Instance {
		get {
			//lazy init
			if (SchottBK7Properties.instance == null) {
				SchottBK7Properties.instance = new SchottBK7Properties();
			}
			return SchottBK7Properties.instance;
		}
	}


	public override double refractiveindex(double wavelength) {

		double l = wavelength*1E6; //turn meters into micrometers
		/* source: //http://refractiveindex.info/?shelf=glass&book=BK7&page=SCHOTT
		 * valid from wavelengths ranging 300nm to 2500nm
		 */
		double l2 = l*l;
		double n2m1 = (1.03961212*l2 / (l2 - 0.00600069867)) + (0.231792344*l2 / (l2 - 0.0200179144)) + (1.01046945*l2 / (l2 - 103.560653));
		return Math.Sqrt(n2m1+1);
		}
	}	//end of class SchottBK7Properties




}
