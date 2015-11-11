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
public class UnitvectorTest {

	[Test()]
	public void UnitvectorLength() {
		Scientrace.UnitVector u = new Scientrace.UnitVector(3, 4, 5);
		//The first 15 decimals (tolerance 1e-15) of the length of the unit-vector should be equal to 1.
		Assert.AreEqual(1, (u.x*u.x)+(u.y*u.y)+(u.z*u.z), 1E-15);
	}

	[Test]
	[ExpectedException(typeof(Scientrace.ZeroNonzeroVectorException))]
	public void ExpectAZeroUnitvectorException() {
		new Scientrace.UnitVector(0, 0, 0);
	}
	
}
}
