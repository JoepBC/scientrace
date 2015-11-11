// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public class SingleWavelengthSpectrum : LightSpectrum {
		public SingleWavelengthSpectrum(int mod_multip, double wavelength) : base (mod_multip) {
			if (wavelength > 1)
				throw new ArgumentOutOfRangeException("Wavelength ('"+wavelength+"') in SingleWavelengthSpectrum > 1. Forgot to present in in nanometers? (E-9).");
			this.addWavelength(wavelength, 1);
			}
		}
	}
