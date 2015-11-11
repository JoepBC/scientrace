// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class GaAsSurface : MaterialProperties {

		
	//Singleton instance "holder"
	private static GaAsSurface instance;
	
	public static GaAsSurface Instance {
		get {
			//lazy init
			if (GaAsSurface.instance == null) {
				GaAsSurface.instance = new GaAsSurface();
			}
			return GaAsSurface.instance;
		}
	}
		
	public override string identifier() {
		return "GaAs";
		}
		
	private GaAsSurface() {
	}

	public override double refractiveindex (double wavelength) {
		return 3.4;
	}

	public override double enterAbsorption (Trace trace, Scientrace.UnitVector norm, Scientrace.MaterialProperties previousMaterial)	{
		return 1;
		}

}
}
