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
public class TriangleTest {

		
	[Test()]
	public void TrangleInPath1() {
		Scientrace.Location loc = new Scientrace.Location(1, 2, 3);
		Scientrace.NonzeroVector v1 = new Scientrace.NonzeroVector(5, 0, 0);
		Scientrace.NonzeroVector v2 = new Scientrace.NonzeroVector(0, 2, 0);
		Scientrace.Triangle pg = new Scientrace.Triangle(loc, v1, v2);
		Scientrace.Line cline = new Scientrace.Line(1.5, 3.5, 0, 0, 0, 5);
		Assert.IsTrue(pg.inPath(cline));
		}
}
}
