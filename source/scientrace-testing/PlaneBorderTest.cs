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
public class PlaneBorderTest {

	[Test()]
	public void testPlaneBorderSide() {
		Scientrace.PlaneBorder pb = new Scientrace.PlaneBorder(
				new Scientrace.Location(0,0,1),
				new Scientrace.NonzeroVector(0,0,-1));
		Assert.IsTrue(pb.contains(new Scientrace.Location(0,0,0)));
		Assert.IsTrue(pb.contains(new Scientrace.Location(0,0,0.5)));
		Assert.IsFalse(pb.contains(new Scientrace.Location(0,0,1.5)));
		Assert.IsTrue(pb.contains(new Scientrace.Location(0,0,1)));
		Assert.IsTrue(pb.contains(new Scientrace.Location(1,0,0)));
		Assert.IsTrue(pb.contains(new Scientrace.Location(0,2,0.5)));
		Assert.IsFalse(pb.contains(new Scientrace.Location(3,0,1.5)));
		Assert.IsTrue(pb.contains(new Scientrace.Location(5,6,1)));
				
		}

	[Test()]
	public void testPlaneBorderSide2() {
		Scientrace.PlaneBorder pb = new Scientrace.PlaneBorder(
				new Scientrace.Location(1,1,1),
				new Scientrace.NonzeroVector(1,1,1));
		Assert.IsTrue(pb.contains(new Scientrace.Location(0,0,3.1)));
		Assert.IsTrue(pb.contains(new Scientrace.Location(3.1,0,0)));
		Assert.IsTrue(pb.contains(new Scientrace.Location(0,3.1,0)));
		Assert.IsFalse(pb.contains(new Scientrace.Location(0,0,0)));

		Assert.IsTrue(pb.contains(new Scientrace.Location(1,1,1)));
		Assert.IsTrue(pb.contains(new Scientrace.Location(1.1,1.1,1.1)));
		Assert.IsFalse(pb.contains(new Scientrace.Location(1,0,0)));
		Assert.IsTrue(pb.contains(new Scientrace.Location(0,0,3.1)));
		Assert.IsFalse(pb.contains(new Scientrace.Location(0,0,1.9)));
		}


	}
}

