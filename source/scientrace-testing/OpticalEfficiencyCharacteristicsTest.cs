// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using NUnit.Framework;
using System;

namespace ScientraceTesting {
[TestFixture()]
public class OpticalEfficiencyCharacteristicsTest {

		[Test()]
		public void TestWavelengthInterpolation() {
			Scientrace.OpticalEfficiencyCharacteristics oec = new Scientrace.OpticalEfficiencyCharacteristics();
			oec.quantum_efficiency.Add(600E-9, 0.4);
			oec.quantum_efficiency.Add(1000E-9, 0.8);
			oec.quantum_efficiency.Add(1200E-9, 0.6);
			Assert.AreEqual(oec.getEffForWavelength(900E-9), 0.7, 0.00000000001);
			Assert.AreEqual(oec.getEffForWavelength(100E-9), 0.4);
			Assert.AreEqual(oec.getEffForWavelength(100E-6), 0.6); //no accidental error, 100E-6 does not exist, largest item should be used.
			}
}
}

