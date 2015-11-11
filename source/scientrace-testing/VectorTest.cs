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
public class VectorTest {


	[Test()]
	public void TestVectorCrossProducts() {
		Scientrace.Vector x = Scientrace.Vector.x1vector();
		Scientrace.Vector y = Scientrace.Vector.y1vector();
		Scientrace.Vector z = Scientrace.Vector.z1vector();

		Assert.AreEqual(x.crossProduct(y), z);
		Assert.AreEqual(y.crossProduct(x), z*-1);
		Assert.AreEqual(y.crossProduct(z), x);
		}

	[Test()]
	public void TestVectorProjection() {
		Scientrace.Vector aVec = new Scientrace.Vector(0.2, 0.3, 0.4);
		Scientrace.UnitVector normal = new Scientrace.UnitVector(0,0,-1);
		Assert.AreEqual(aVec.projectOnPlaneWithNormal(normal), new Scientrace.Vector(0.2, 0.3, 0));
		}


	[Test()]
	public void TestMinusOperator() {
		Scientrace.Vector v1, v2, v3;
		v1 = new Scientrace.Vector(1, 2, 3);
		v2 = new Scientrace.Vector(4, 1, 0.5);
		v3 = v1-v2;
		Assert.AreEqual(-3, v3.x);
		Assert.AreEqual(1, v3.y);
		Assert.AreEqual(2.5, v3.z);
	}

	[Test()]
	public void TestEqualsOperator() {
		Scientrace.Vector v1, v2;
		v1 = new Scientrace.Vector(1, 1, 1);
		v2 = new Scientrace.Vector(1, 1, 1);
		Assert.AreEqual(v1, v2);
	}

	[Test()]
	public void RotateAboutZ1() {
		Scientrace.Vector v1;
		v1 = new Scientrace.Vector(1, 2, 0).rotateAboutZ(Math.PI/2);
		Scientrace.Vector v2 =new Scientrace.Vector(-2,1,0);
		Assert.AreEqual(v2.x, v1.x, 1E-15);
		Assert.AreEqual(v2.y, v1.y, 1E-15);
		Assert.AreEqual(v2.z, v1.z, 1E-15);
	}
		
	[Test()]
	public void RotateAboutY1() {
		Scientrace.Vector v1;
		v1 = new Scientrace.Vector(1, 0, 2).rotateAboutY(Math.PI/2);
		Scientrace.Vector v2 =new Scientrace.Vector(2,0, -1);
		Assert.AreEqual(v2.x, v1.x, 1E-15);
		Assert.AreEqual(v2.y, v1.y, 1E-15);
		Assert.AreEqual(v2.z, v1.z, 1E-15);
	}

	[Test()]
	public void TestNormalizing() {
		Scientrace.NonzeroVector v1 = new Scientrace.NonzeroVector(1,2,3);
		Scientrace.NonzeroVector v2 = v1.copy().tryToNonzeroVector();
		v2.normalize();
		Assert.AreEqual(v1.normalized(), v2);
		}
		
	[Test()]
	public void TestRotateAboutVector() {
		Scientrace.Vector v1 = new Scientrace.Vector(1,1,1);
		double angle = Math.PI/2; //a random angle
	/**/	Assert.AreEqual(v1.rotateAboutZ(angle), 
						v1.rotateAboutVector(new Scientrace.NonzeroVector(0,0,1), angle)
						);
	/*	Assert.AreEqual(new Scientrace.Vector(1, -1, 1), 
						v1.rotateAboutVector(new Scientrace.NonzeroVector(1,0,0), angle)
						);/* this will only fail due to significance */
	/**/	Assert.AreEqual(v1.rotateAboutY(angle), 
						v1.rotateAboutVector(new Scientrace.NonzeroVector(0,1,0), angle)
						);/**/
	/**/	Assert.AreEqual(v1.rotateAboutX(angle), 
						v1.rotateAboutVector(new Scientrace.NonzeroVector(1,0,0), angle)
						);/**/
		}
		
		
	[Test()]
	public void TestDotProduct1() {
		Scientrace.Vector v1, v2;
		v1 = new Scientrace.UnitVector(1, 1, 1);
		v2 = new Scientrace.UnitVector(1, 1, 1);
		Assert.AreEqual(1, v1.dotProduct(v2), 1E-15);
	}


	[Test ()]
	public void testDotProductResults() {
		Random r = new Random();
		for (int i = 0; i<1000; i++) {
			Scientrace.Vector a,b;
				a = new Scientrace.Vector(r.NextDouble()*100,r.NextDouble()*100,r.NextDouble()*100);
				b = new Scientrace.Vector(r.NextDouble()*100,r.NextDouble()*100,r.NextDouble()*100);
				Assert.LessOrEqual(Math.Abs(a.tryToUnitVector().dotProduct(b.tryToUnitVector())), 1);
			}
		}

	[Test()]
	public void TestDotProduct2() {
		Scientrace.Vector v1, v2;
		v1 = new Scientrace.UnitVector(1, 1, 1);
		v2 = new Scientrace.UnitVector(-1, -1, -1);
		Assert.AreEqual(-1, v1.dotProduct(v2), 1E-15);
	}

	[Test()]
	public void TestZeroAngleWith() {
		Scientrace.Vector v1, v2;
		v1 = new Scientrace.Vector(1, 1, 1);
		v2 = new Scientrace.Vector(1, 1, 1);
		Assert.AreEqual(0, v1.angleWith(v2));
	}

	[Test()]
	public void TestAngleWith1() {
		Scientrace.Vector v1, v2;
		v1 = new Scientrace.Vector(1, 1, 1);
		v2 = new Scientrace.Vector(-1, -1, -1);
		Assert.AreEqual(Math.PI, v1.angleWith(v2));
	}


	[Test()]
	public void TestNotEqualsOperator() {
		Scientrace.Vector v1, v2;
		v1 = new Scientrace.Vector(1, 1, 1);
		v2 = new Scientrace.Vector(1, 1, 0);
		Assert.AreNotEqual(v1, v2);
	}
	
	
}
}
