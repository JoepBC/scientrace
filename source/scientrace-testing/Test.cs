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
public class DefaultBogusTests {

	[Test()]
	public void mathOrder () {
		Assert.AreEqual (1+2*3+4*5+6, 33);
	}
	
	[Test()]
	public void divfloatintint() {
		Assert.AreEqual(0.5, (double)1/2);
	}

	[Test()]
	public void divintint() {
		Assert.AreEqual(0, 1/2);
	}
		
}
}
