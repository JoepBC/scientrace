// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {
	

public class AirProperties : MaterialProperties {

	private static AirProperties instance;

	private AirProperties() {
	}

	public override string identifier() {
		return "air";
		}

	public static AirProperties Instance {
		get {
			//lazy init
			if (AirProperties.instance == null) {
				AirProperties.instance = new AirProperties();
			}
			return AirProperties.instance;
		}
	}

	public override double refractiveindex(double wavelength) {
		return 1;
	}
}
}

