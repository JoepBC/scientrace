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
}
}
