// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


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
		
	private BlackBodySurface() {
	}

	public override double refractiveindex (double wavelength) {
		return 10;
	}

	public override double enterAbsorption (Trace trace, Scientrace.UnitVector norm, Scientrace.MaterialProperties previousMaterial)	{
		return 1;
		}

}
}
