// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class RangeSpectrum : LightSpectrum {

/*	public override string identifier() {
		return "nmrange";
		}*/
		

	/// <summary>
	/// Create a wavelengt-spectrum with a resolution of "points" within "min" and "max" nanometers.
	/// </summary>
	/// <param name="min">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="max">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="points"> the resolution/number of wavelengths within the range
	/// A <see cref="System.Int32"/>
	/// </param>
	public RangeSpectrum(int mod_multip, int min_nm, int max_nm, double resolution) : base (mod_multip) {
		//the +1 in the ToDouble clause is to include the max_nm value as well and respect the resolution
		for (double i = min_nm; i <= max_nm; i=i+(Convert.ToDouble(1+max_nm-min_nm)/(resolution+1))) {
			//translate nanometers to meters... *1E-9
			this.addWavelength(i*1E-9, 1);
			}
	}
}
}
