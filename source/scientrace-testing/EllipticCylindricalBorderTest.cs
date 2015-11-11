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
public class EllipticCylindricalBorderTesting {

	[Test()]
	public void TestWithinCylinder() {
		Scientrace.CylindricalBorder cylb = new Scientrace.CylindricalBorder(
			new Scientrace.Location(-10,10,-10),
			new Scientrace.NonzeroVector(-1,-1,0),5);
			
		Assert.IsTrue(cylb.contains(new Scientrace.Location(-11, 9, -10))); //top
		Assert.IsTrue(cylb.contains(new Scientrace.Location(-10, 10, -10))); //bottom
		Assert.IsTrue(cylb.contains(new Scientrace.Location(-10.5, 9.5, -10))); //middle
		Assert.IsTrue(cylb.contains(new Scientrace.Location(-10.5, 9.5, -14.9))); //middle just within
		Assert.IsFalse(cylb.contains(new Scientrace.Location(-10.5, 9.5, -15.1))); //middle just outside
		Assert.IsTrue(cylb.contains(new Scientrace.Location(-10.5, 9.5, -5.1))); //middle just within
		Assert.IsFalse(cylb.contains(new Scientrace.Location(-10.5, 9.5, -4.9))); //middle just outside
		Assert.IsTrue(cylb.contains(new Scientrace.Location(-11, 9, -14.9))); //top just within
		Assert.IsFalse(cylb.contains(new Scientrace.Location(-11, 9, -15.1))); //top just outside
		Assert.IsTrue(cylb.contains(new Scientrace.Location(-11, 9, -5.1))); //top just within
		Assert.IsFalse(cylb.contains(new Scientrace.Location(-11, 9, -4.9))); //top just outside
//		Assert.IsTrue(cylb.contains(new Scientrace.Location(-10.5,9.2,14)));
	}
}
}
