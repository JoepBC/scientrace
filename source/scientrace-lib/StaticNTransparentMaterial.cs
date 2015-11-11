// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class StaticNTransparentMaterial : Scientrace.MaterialProperties {

	public override string identifier() {
		return "staticn";
		}
		
		
	private double refindex;

	public StaticNTransparentMaterial(double refindex) {
		this.reflects = true;
		this.refindex = refindex;
		}

	public void setRefractiveIndex(double ref_index) {
		this.refindex = ref_index;
		}

	public override double refractiveindex(double wavelength) {
		return this.refindex;
		}

	}}//end class+namespace
