// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {
public class UserLightSpectrum : LightSpectrum {
		
	public UserLightSpectrum(int mod_multip, string tag) : base(mod_multip) {
		this.tag = tag;
		}

	}}