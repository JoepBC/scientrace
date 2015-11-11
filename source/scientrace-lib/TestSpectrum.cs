// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class TestSpectrum : LightSpectrum {

/*	public override string identifier() {
		return "testspectrum";
		}*/
		
	public TestSpectrum(int mod_multip) : base (mod_multip) {
		/*this.addWavelength(300E-9, 0.25);
		this.addWavelength(500E-9, 0.5);*/
		this.addWavelength(700E-9, 0.25);
	}
}
}
