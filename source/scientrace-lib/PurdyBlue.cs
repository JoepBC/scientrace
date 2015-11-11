// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {
public class PurdyBlue : LightSpectrum {
		
/*	public override string identifier() {
		return "purdyblue";
		}*/

	public PurdyBlue(int mod_multip) : base (mod_multip) {
		for (int iwl = 450; iwl <= 490; iwl = iwl+2) {
			this.addWavelength(iwl * 1E-9,1);
		}
		for (int iwl = 489; iwl >= 450; iwl = iwl-2) {
			this.addWavelength(iwl * 1E-9,1);
		}
	}
}
}