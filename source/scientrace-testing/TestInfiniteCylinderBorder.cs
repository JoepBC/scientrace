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
public class TestInfiniteCylinderBorder {

	[Test()]
	public void TestWithinCylinderBorder() {
	
		Scientrace.InfiniteCylinderBorder icb = new Scientrace.InfiniteCylinderBorder(null, null, 
						new Scientrace.Location(0,0,0), 
						new Scientrace.UnitVector(0,1,0), 
						2);
		Assert.IsTrue(icb.contains(new Scientrace.Location(0,0,0)));
		Assert.IsTrue(icb.contains(new Scientrace.Location(1,1,1)));
		Assert.IsTrue(icb.contains(new Scientrace.Location(1.2,-5,1)));
		Assert.IsTrue(icb.contains(new Scientrace.Location(1,5,1)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(1,2,3)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(3,4,5)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(4,5,6)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(5,6,7)));
		}

	[Test()]
	public void TestWithinDiagonalCylinder() {
		Scientrace.InfiniteCylinderBorder icb = new Scientrace.InfiniteCylinderBorder(null, null, 
						new Scientrace.Location(0,0,10), 
						new Scientrace.UnitVector(1,1,0), 
						2);
		Assert.IsFalse(icb.contains(new Scientrace.Location(0,0,0)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(1,1,1)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(1.2,-5,1)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(1,5,1)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(1,2,3)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(3,4,5)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(4,5,6)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(5,6,7)));
		Assert.IsTrue(icb.contains(new Scientrace.Location(0,0,9)));
		Assert.IsTrue(icb.contains(new Scientrace.Location(0,-1.5,9)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(0,-1.5,8.1)));
		Assert.IsTrue(icb.contains(new Scientrace.Location(0,-0.5,8.1)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(0,-0.5,8.0)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(0,-0.1,8.0)));
		}

	[Test()]
	public void TestCylinder2() {
		Scientrace.InfiniteCylinderBorder icb = new Scientrace.InfiniteCylinderBorder(null, null, 
						new Scientrace.Location(0,-14.5,0), 
						new Scientrace.UnitVector(0,0,1), 
						0.2);
		Assert.IsFalse(icb.contains(new Scientrace.Location(0,0,0)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(1,1,1)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(1.2,-5,1)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(1,5,1)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(1,2,3)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(3,4,5)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(4,5,6)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(5,6,7)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(0,0,9)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(0,-1.5,9)));
		
		Assert.IsTrue(icb.contains(new Scientrace.Location(0, -14.4, 7)));
		Assert.IsTrue(icb.contains(new Scientrace.Location(0, -14.4, -7)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(0.2, -14.4, -7)));
		Assert.IsTrue(icb.contains(new Scientrace.Location(0.2, -14.5, -7)));
		Assert.IsTrue(icb.contains(new Scientrace.Location(-0.2, -14.5, -27)));
		Assert.IsFalse(icb.contains(new Scientrace.Location(0.1, -14.7, -7)));
		Assert.IsTrue(icb.contains(new Scientrace.Location(0, -14.7, -127)));
		}


	[Test()]
	public void TestCylinderIntersects() {
		Scientrace.InfiniteCylinderBorder icb = new Scientrace.InfiniteCylinderBorder(null, null, 
						new Scientrace.Location(0,-14.5,0), 
						new Scientrace.UnitVector(0,0,1), 
						0.2);
		Scientrace.Line testLine = new Scientrace.Line(0,0,0,  0,-1,0);
		Scientrace.Intersection Ints = icb.intersects(new Scientrace.Trace(600, null, testLine, null, 1, 1));
		Assert.AreEqual(Ints.enter.loc.ToCompactString(), new Scientrace.Location(0, -14.3, 0).ToCompactString());
		Assert.AreEqual(Ints.exit.loc.ToCompactString(), new Scientrace.Location(0, -14.7, 0).ToCompactString());

		testLine = new Scientrace.Line(0,0,0.1,  0,-1,0);
		Ints = icb.intersects(new Scientrace.Trace(600, null, testLine, null, 1, 1));
		Assert.AreEqual(Ints.enter.loc.ToCompactString(), new Scientrace.Location(0, -14.3, 0.1).ToCompactString());
		Assert.AreEqual(Ints.exit.loc.ToCompactString(), new Scientrace.Location(0, -14.7, 0.1).ToCompactString());

		testLine = new Scientrace.Line(0.1,0,0,  0,-1,0);
		Ints = icb.intersects(new Scientrace.Trace(600, null, testLine, null, 1, 1));
		Assert.AreEqual(Ints.enter.loc.ToCompactString(), new Scientrace.Location(0.1, -14.5+(Math.Sqrt(0.03)), 0).ToCompactString());
		Assert.AreEqual(Ints.exit.loc.ToCompactString(), new Scientrace.Location(0.1, -14.5-(Math.Sqrt(0.03)), 0).ToCompactString());
		
		}
		
}
}
