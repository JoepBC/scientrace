// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class PerfectMirror : Scientrace.MaterialProperties {

	//Singleton instance "holder"
	private static PerfectMirror instance;

	public override string identifier() {
		return "mirror";
		}
		
	private PerfectMirror() {
		this.reflects = true;
		this.dielectric = false;
		}

	public override double enterReflection (Scientrace.Trace trace, Scientrace.UnitVector norm, Scientrace.MaterialProperties previousMaterial) {
		return 1;
		}
		
	public override double enterAbsorption (Trace trace, Scientrace.UnitVector norm, Scientrace.MaterialProperties previousMaterial)	{
		return 0;
		}

	public static PerfectMirror Instance {
		get {
			//lazy init
			if (PerfectMirror.instance == null) {
				PerfectMirror.instance = new PerfectMirror();
			}
			return PerfectMirror.instance;
		}
	}

	public override double refractiveindex(double wavelength) {
		return 1;
		}

}
}