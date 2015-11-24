// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class RandomRangeSpectrum : LightSpectrum {

/*	public override string identifier() {
		return "randomnmrange";
		}*/
		
	public int min, max, points;

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
		public RandomRangeSpectrum(int min_nm, int max_nm, int points, int? random_seed) : base (1) {
		this.min = min_nm;
		this.max = max_nm;
		this.points = points;

		Random rnd;
		if (random_seed == null || random_seed== -1)
			rnd = new Random();
			else
			rnd = new Random((int)random_seed);

		for (int i = 0; i < this.points; i++) {
			//translate nanometers to meters... *1E-9
			this.addWavelength(rnd.Next(this.min, this.max)*1E-9, 1);
			}
	}
}
}
