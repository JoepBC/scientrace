// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {
public class AM15Spectrum : LightSpectrum {
		
/*	public override string identifier() {
		return "am15";
		}*/

	public AM15Spectrum(int mod_multip) : base(mod_multip) {
		this.init();
		}
		
	public void init() {
//		this.addWavelength(300E-9,2.90E+000); //too small for refindex calculation for example.
		this.addWavelength(350E-9,3.25E+001);
		this.addWavelength(400E-9,4.82E+001);
		this.addWavelength(450E-9,5.84E+001);
		this.addWavelength(500E-9,6.67E+001);
		this.addWavelength(550E-9,6.75E+001);
		this.addWavelength(600E-9,6.60E+001);
		this.addWavelength(650E-9,6.36E+001);
		this.addWavelength(700E-9,5.73E+001);
		this.addWavelength(750E-9,4.97E+001);
		this.addWavelength(800E-9,4.87E+001);
		this.addWavelength(850E-9,4.47E+001);
		this.addWavelength(900E-9,3.67E+001);
		this.addWavelength(950E-9,2.01E+001);
		this.addWavelength(1000E-9,3.33E+001);
		this.addWavelength(1050E-9,3.08E+001);
		this.addWavelength(1100E-9,2.10E+001);
		this.addWavelength(1150E-9,1.22E+001);
		this.addWavelength(1200E-9,2.09E+001);
		this.addWavelength(1250E-9,2.08E+001);
		this.addWavelength(1300E-9,1.75E+001);
		this.addWavelength(1350E-9,3.95E+000);
		this.addWavelength(1400E-9,1.92E-001);
		this.addWavelength(1450E-9,2.97E+000);
		this.addWavelength(1500E-9,9.37E+000);
		this.addWavelength(1550E-9,1.29E+001);
		this.addWavelength(1600E-9,1.17E+001);
		this.addWavelength(1650E-9,1.08E+001);
		this.addWavelength(1700E-9,6.97E+000);
/*		this.addWavelength(1800E-9,1.21E+000);//irrelevant for triple junction, add E-9 for below later
		this.addWavelength(1900E-9,1.99E-002);
		this.addWavelength(2000E-9,1.10E+000);
		this.addWavelength(2100E-9,1.62E+000);
		this.addWavelength(2200E-9,1.53E+000);
		this.addWavelength(2300E-9,1.20E+000);
/*		this.addWavelength(2400E-9,7.30E-001);//irrelevant for significance
		this.addWavelength(2500E-9,1.60E-001);
		this.addWavelength(2600E-9,1.07E-009);
		this.addWavelength(2700E-9,7.57E-019);
		this.addWavelength(2800E-9,4.54E-005);
		this.addWavelength(2900E-9,2.88E-002);
		this.addWavelength(3000E-9,9.09E-002);
		this.addWavelength(3100E-9,7.26E-002);
		this.addWavelength(3200E-9,9.93E-002);
		this.addWavelength(3300E-9,8.20E-002);
		this.addWavelength(3400E-9,1.64E-001);
		this.addWavelength(3500E-9,2.23E-001);
		this.addWavelength(3600E-9,2.02E-001);
		this.addWavelength(3700E-9,1.92E-001);
		this.addWavelength(3800E-9,1.79E-001);
		this.addWavelength(3900E-9,1.49E-001);
		this.addWavelength(4000E-9,8.28E-002);/**/
	}
}
}