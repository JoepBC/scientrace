// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class AltPMMAProperties : Scientrace.MaterialProperties {


	//Singleton instance "holder"
	private static AltPMMAProperties instance;

	private AltPMMAProperties() {
		this.reflects = true;
		this.dielectric = true;
	}

	public override string identifier() {
		return "altpmma";
		}

	public static AltPMMAProperties Instance {
		get {
			//lazy init
			if (AltPMMAProperties.instance == null) {
				AltPMMAProperties.instance = new AltPMMAProperties();
			}
			return AltPMMAProperties.instance;
		}
	}



	public override double refractiveindex(double wavelength) {
			wavelength = wavelength*1E6; //turn meters into micrometers
			return (1.478+0.00453/(Math.Pow(wavelength,2)));
			//source www.refractiveindex.info, function of wavelength in micrometers
		//return Math.Sqrt(2.399964+-0.08308636*Math.Pow(wavelength, 2)+-0.1919569*Math.Pow(wavelength, -2)+0.08720608*Math.Pow(wavelength, -4)+-0.01666411*Math.Pow(wavelength, -6)+0.001169519*Math.Pow(wavelength, -8));
	}
}
}
