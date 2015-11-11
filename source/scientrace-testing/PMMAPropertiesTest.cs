// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using NUnit.Framework;

namespace ScientraceTesting {


[TestFixture()]
public class PMMAPropertiesTest {

	[Test()]
	public void TestRefIndex1() {
			Scientrace.PMMAProperties pmma = Scientrace.PMMAProperties.Instance;
			double refindex = pmma.refractiveindex(500E-9);
			Assert.AreEqual(1.4965145886358746, refindex);

	}
}
}
