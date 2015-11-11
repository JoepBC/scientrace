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
public class LocationTest {

	[Test()]
	public void TestDistance() {
		Scientrace.Location loc1 = new Scientrace.Location(1, 1, 1);
		Scientrace.Location loc2 = new Scientrace.Location(4, 5, 6);
		Assert.AreEqual(loc1.distanceTo(loc2), Math.Sqrt(50));
	}
		
	[Test()]
	public void TestRotateAboutCenter() {
		Scientrace.Location loc1 = new Scientrace.Location(1, 1, 1);
		Assert.AreEqual(loc1.rotateAboutVector(new Scientrace.NonzeroVector(0,1,0), Math.PI/2).trico(), 
			new Scientrace.Location(1,1,-1).trico());
		Assert.AreEqual(
			loc1.rotateAboutVector(new Scientrace.NonzeroVector(0,1,0), new Scientrace.Location(0,1,0), Math.PI/2).trico(), 
			new Scientrace.Location(1,1,-1).trico());
		Assert.AreEqual(
			loc1.rotateAboutVector(new Scientrace.NonzeroVector(0,1,0), new Scientrace.Location(0,0,1), Math.PI/2).ToCompactString(), 
			new Scientrace.Location(0,1,0).ToCompactString());
		Assert.AreEqual(
			loc1.rotateAboutVector(new Scientrace.NonzeroVector(0,1,0), new Scientrace.Location(1,0,0), Math.PI/2).ToCompactString(), 
			new Scientrace.Location(2,1,0).ToCompactString());
		}
		
}
}
