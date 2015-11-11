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
public class VectorTransformTest {

	[Test()]
	public void CreateOrtho3rdVector() {
		Scientrace.UnitVector u, v;
		u = new Scientrace.UnitVector(2, 3, 4);
		v = new Scientrace.UnitVector(1, 2, 3);
		Scientrace.VectorTransform foo = new Scientrace.VectorTransform(u, v);
		Assert.AreNotEqual(0, u.dotProduct(v));
		//, 1E-15);
		Assert.AreEqual(0, u.dotProduct(foo.w), 1E-15);
		Assert.AreEqual(0, v.dotProduct(foo.w), 1E-15);
	}

	[Test()]
	public void TransformMatrixTest() {
		Scientrace.NonzeroVector u, v;
		u = new Scientrace.NonzeroVector(2, 3, 4);
		v = new Scientrace.NonzeroVector(1, 2, 3);
		Scientrace.VectorTransform foo = new Scientrace.VectorTransform(u, v);
		Scientrace.Vector tvec = foo.transform(new Scientrace.Vector(1, 1.5, 2));
		Assert.AreEqual(tvec.x, 0.5);
		Assert.AreEqual(tvec.y, 0, 1E-15);
		Assert.AreEqual(tvec.z, 0, 1E-15);
		
	}


	[Test()]
	public void TransformMatrixAndBackTest1() {
		Scientrace.NonzeroVector u, v;
		u = new Scientrace.NonzeroVector(2, 3, 4);
		v = new Scientrace.NonzeroVector(3, 2, 3);
		Scientrace.VectorTransform foo = new Scientrace.VectorTransform(u, v);
		Scientrace.Vector tvec = foo.transform(new Scientrace.Vector(6, 3, 8));
		Scientrace.Vector tbackvec = foo.transformback(tvec);
		Assert.AreEqual(tbackvec.x, 6, 1E-14);
		Assert.AreEqual(tbackvec.y, 3, 1E-14);
		Assert.AreEqual(tbackvec.z, 8, 1E-14);
	}

	[Test()]
	public void TransformMatrixAndBackTest2() {
		Scientrace.NonzeroVector u, v;
		u = new Scientrace.NonzeroVector(2.3, 3.2, 4.23);
		v = new Scientrace.NonzeroVector(3.14, 2.12, 3);
		Scientrace.VectorTransform foo = new Scientrace.VectorTransform(u, v);
		Scientrace.Vector tvec = foo.transform(new Scientrace.Vector(6, 3, 8));
		Scientrace.Vector tbackvec = foo.transformback(tvec);
		Assert.AreEqual(tbackvec.x, 6, 1E-14);
		Assert.AreEqual(tbackvec.y, 3, 1E-14);
		Assert.AreEqual(tbackvec.z, 8, 1E-14);
	}
		
		
	[Test()]
	public void TransformMatrixAndBackToStringTest() {
		Scientrace.NonzeroVector u, v;
		u = new Scientrace.NonzeroVector(5, 2, 4);
		v = new Scientrace.NonzeroVector(3, 2, 3);
		Scientrace.VectorTransform foo = new Scientrace.VectorTransform(u, v);
		Scientrace.Vector orivec = new Scientrace.Vector(5, 1, 2);
		Scientrace.Vector tvec = foo.transform(orivec);
		Scientrace.Vector tbackvec = foo.transformback(tvec);
		Assert.AreEqual(orivec.ToCompactString(), tbackvec.ToCompactString());
	}
	
	
}
}
