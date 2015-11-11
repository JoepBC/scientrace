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
public class BoxBorderTest {

	[Test()]
	public void TestWithinOrthogonalBorders() {
		Scientrace.BoxBorder bb = new Scientrace.BoxBorder(new Scientrace.Location(1,1,1),
			                                                   new Scientrace.NonzeroVector(3,0,0),
			                                                   new Scientrace.NonzeroVector(0,4,0),
			                                                   new Scientrace.NonzeroVector(0,0,5)/*,
			                                                   Scientrace.Object3dEnvironment.dummy()*/
			                                                   );
		Assert.IsFalse(bb.contains(new Scientrace.Location(0,0,0)));
		Assert.IsTrue(bb.contains(new Scientrace.Location(1,1,1)));
		Assert.IsTrue(bb.contains(new Scientrace.Location(1,2,3)));
		Assert.IsTrue(bb.contains(new Scientrace.Location(3,4,5)));
		Assert.IsTrue(bb.contains(new Scientrace.Location(4,5,6)));
		Assert.IsFalse(bb.contains(new Scientrace.Location(5,6,7)));
		}

	[Test()]
	public void TestWithinDiagonalBorders() {
		Scientrace.BoxBorder bb = new Scientrace.BoxBorder(new Scientrace.Location(1,1,1),
			                                                   new Scientrace.NonzeroVector(1,2,0),
			                                                   new Scientrace.NonzeroVector(0,2,0),
			                                                   new Scientrace.NonzeroVector(0,0,2)/*,
			                                                   Scientrace.Object3dEnvironment.dummy()*/
			                                                   );
		Assert.IsFalse(bb.contains(new Scientrace.Location(0,0,0)));
		Assert.IsTrue(bb.contains(new Scientrace.Location(1,1,1)));
		Assert.IsTrue(bb.contains(new Scientrace.Location(1,2,3)));
		Assert.IsTrue(bb.contains(new Scientrace.Location(1.49,2,2)));
		Assert.IsFalse(bb.contains(new Scientrace.Location(1.51,2,2)));
		}

}
}
