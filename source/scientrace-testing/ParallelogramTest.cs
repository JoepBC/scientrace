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
public class ParallelogramTest {

	[Test()]
	public void PGramSquareSurfaceTest() {
		Scientrace.Parallelogram a = new Scientrace.Parallelogram(new Scientrace.Location(10, 10, 10), new Scientrace.NonzeroVector(3, 4, 0), new Scientrace.NonzeroVector(0, 0, 5));
		Assert.AreEqual(25, a.getSurfaceSize());
	}

	[Test()]
	public void PGramDiagonalSurfaceTest1() {
		Scientrace.Parallelogram a = new Scientrace.Parallelogram(new Scientrace.Location(1, 2, 3), new Scientrace.NonzeroVector(3, 4, 5), new Scientrace.NonzeroVector(3, 4, 0));
		Assert.AreEqual(25, a.getSurfaceSize());
	}

	[Test()]
	public void PGramDiagonalSurfaceTest2() {
		double x1, x2, y1, y2, z1, z2;
		x1 = 4;
		x2 = -3;
		y1 = 56;
		y2 = 12;
		z1 = -2;
		z2 = 23;
		Scientrace.NonzeroVector nzv1 = new Scientrace.NonzeroVector(x1, y1, z1);
		Scientrace.NonzeroVector nzv2 = new Scientrace.NonzeroVector(x2, y2, z2);
		Scientrace.Parallelogram a = new Scientrace.Parallelogram(new Scientrace.Location(1, 2, 3), nzv1, nzv2);
		//comparing surface calculated by sine of the angle between vectors and multiplication with their lengths with the lenght of the outproduct
		Assert.AreEqual(
			Math.Sin(Math.Acos(nzv1.toUnitVector().dotProduct(nzv2.toUnitVector())))*nzv1.length*nzv2.length, 
			a.getSurfaceSize(),
			1E-12);
	}

	[Test()]
	public void PGramDiagonalSurfacePerformanceTest() {
		DateTime startTime = DateTime.Now;
		double x1, x2, y1, y2, z1, z2, foo = 1;
		for (x1 = 0; x1 < 10000; x1++) {
			x2 = -3;
			y1 = 56;
			y2 = 12;
			z1 = -2;
			z2 = 23;
			Scientrace.NonzeroVector nzv1 = new Scientrace.NonzeroVector(x1, y1, z1);
			Scientrace.NonzeroVector nzv2 = new Scientrace.NonzeroVector(x2, y2, z2);
			Scientrace.Parallelogram a = new Scientrace.Parallelogram(new Scientrace.Location(1, 2, 3), nzv1, nzv2);
			//comparing surface calculated by sine of the angle between vectors and multiplication with their lengths with the lenght of the outproduct
			if (Math.Sin(Math.Acos(nzv1.toUnitVector().dotProduct(nzv2.toUnitVector())))*nzv1.length*nzv2.length != a.getSurfaceSize()) {
				//Assert.AreEqual(Math.Sin(Math.Acos(nzv1.toUnitVector().dotProduct(nzv2.toUnitVector())))*nzv1.length*nzv2.length, a.getSurface(), 1e-10); 
				foo++;
			}
		}
		DateTime stopTime = DateTime.Now;
		TimeSpan duration = stopTime-startTime;
		Console.WriteLine(foo.ToString()+" siginificance failures");
		//making sure these 10 000 dotproducts and crossproducts comparisons can be executed within 60 milliseconds
		Assert.Less(duration.Milliseconds, 60);
	}


	[Test()]
	public void PGramAngleTest1() {
		Scientrace.Location loc = new Scientrace.Location(1, 2, 3);
		Scientrace.NonzeroVector v1 = new Scientrace.NonzeroVector(1, 0, 0);
		Scientrace.NonzeroVector v2 = new Scientrace.NonzeroVector(0, 1, 0);
		Scientrace.NonzeroVector v3 = new Scientrace.NonzeroVector(0, 1, 1);
		Scientrace.Parallelogram pg = new Scientrace.Parallelogram(loc, v1, v2);
		Assert.AreEqual((Math.PI)/4, pg.angleOnSurface(v3), 1E-15);
	}

	[Test()]
	public void PGramAngleTest2() {
		Scientrace.Location loc = new Scientrace.Location(1, 2, 3);
		Scientrace.NonzeroVector v1 = new Scientrace.NonzeroVector(-1, 1, 0);
		Scientrace.NonzeroVector v2 = new Scientrace.NonzeroVector(-1, 0, 1);
		Scientrace.NonzeroVector v3 = new Scientrace.NonzeroVector(1, 1, 1);
		Scientrace.Parallelogram pg = new Scientrace.Parallelogram(loc, v1, v2);
		Assert.AreEqual(pg.angleOnSurface(v3), 0, 1E-15);
	}

	[Test()]
	public void PGramLineIntersects() {
		Scientrace.Location loc = new Scientrace.Location(1, 2, 3);
		Scientrace.NonzeroVector v1 = new Scientrace.NonzeroVector(1, 0, 0);
		Scientrace.NonzeroVector v2 = new Scientrace.NonzeroVector(0, 1, 0);
		Scientrace.Parallelogram pg = new Scientrace.Parallelogram(loc, v1, v2);
		//this line intersects the defined plane at (5,5,3)
		Scientrace.Vector intersection = pg.plane.lineThroughPlane(new Scientrace.Line(1, 1, -1, 2, 2, 2));
		Assert.AreEqual(intersection.x, 5);
		Assert.AreEqual(intersection.y, 5);
		Assert.AreEqual(intersection.z, 3);
	}

	[Test()]
	public void PGramIntersectionAtBaseVectors1() {
		Scientrace.Location loc = new Scientrace.Location(1, 2, 3);
		Scientrace.NonzeroVector v1 = new Scientrace.NonzeroVector(1, 0, 0);
		Scientrace.NonzeroVector v2 = new Scientrace.NonzeroVector(0, 1, 0);
		Scientrace.Parallelogram pg = new Scientrace.Parallelogram(loc, v1, v2);
		//this line intersects the defined plane at (5,5,3)
		Scientrace.Vector intersection = pg.plane.lineThroughPlane(new Scientrace.Line(1, 1, -1, 2, 2, 2));
		Scientrace.VectorTransform trf = pg.plane.getTransform();
		Scientrace.Vector tintersection = trf.transform(intersection-loc);
		/* when the pgram-plane is shifted through 0,0,0, a transformation of any location in this plane to 
			 * the base vectors e1 and e2 (based on v1 and v2) should leave a 3rd coordinate set to zero. */		
		Assert.AreEqual(tintersection.z, 0);
	}

	[Test()]
	public void PGramIntersectionAtBaseVectors2() {
		Scientrace.Location loc = new Scientrace.Location(1, 2, 3);
		Scientrace.NonzeroVector v1 = new Scientrace.NonzeroVector(2, 5, 7);
		Scientrace.NonzeroVector v2 = new Scientrace.NonzeroVector(3, 2, 0);
		Scientrace.Parallelogram pg = new Scientrace.Parallelogram(loc, v1, v2);
		Scientrace.Vector intersection = pg.plane.lineThroughPlane(new Scientrace.Line(1, 1, -1, 2, 2, 2));
		Scientrace.VectorTransform trf = pg.plane.getTransform();
		Scientrace.Vector tintersection = trf.transform(intersection-loc);
		/* when the pgram-plane is shifted through 0,0,0, a transformation of any location in this plane to 
			 * the base vectors e1 and e2 (based on v1 and v2) should leave a 3rd coordinate set to zero. */		
		Assert.AreEqual(tintersection.z, 0, 1E-14);
	}

	[Test()]
	public void PGramInPath1() {
		Scientrace.Location loc = new Scientrace.Location(1, 2, 3);
		Scientrace.NonzeroVector v1 = new Scientrace.NonzeroVector(5, 0, 0);
		Scientrace.NonzeroVector v2 = new Scientrace.NonzeroVector(0, 2, 0);
		Scientrace.Parallelogram pg = new Scientrace.Parallelogram(loc, v1, v2);
		Scientrace.Line cline = new Scientrace.Line(5.5, 3.5, 0, 0, 0, 5);
		Assert.IsTrue(pg.inPath(cline));
	}
	
	
}
}
