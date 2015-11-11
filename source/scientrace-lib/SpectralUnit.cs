// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public class SpectralUnit {

	/// <summary>
	/// The wavelength in meters (so 500nm becomes 500E-9m)
	/// </summary>
	public double wavelength;
	public double intensity;
	public string tag;

		public SpectralUnit(double aWaveLength, double anIntensity) {
			this.wavelength = aWaveLength;
			this.intensity = anIntensity;
			this.tag = "wl"+aWaveLength.ToString()+"i"+anIntensity.ToString();
			}
	}}