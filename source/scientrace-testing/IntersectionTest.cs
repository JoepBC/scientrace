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
public class IntersectionTest {

	[Test()]
	public void TestIntersectionByVectorArray() {
			Scientrace.Object3dEnvironment env = new Scientrace.Object3dEnvironment(Scientrace.AirProperties.Instance,100);
			Scientrace.Line beam = new Scientrace.Line(10, 10, 10, -1, -1, 0);
			Scientrace.LightSource light = new Scientrace.SingleRaySource(beam,
			                                                             10, new Scientrace.AM15Spectrum(1), env);
			Scientrace.Trace trace = new Scientrace.Trace(500.0E-9, light, beam, env,1,1);
			Console.WriteLine(trace.ToString());
			Scientrace.Vector v = new Scientrace.Vector(10,10,10);
			Scientrace.IntersectionPoint[] ips = new Scientrace.IntersectionPoint[2];
			ips[0] = new Scientrace.IntersectionPoint(new Scientrace.Location(10,10,10), 
			                                 new Scientrace.FlatShape2d(v.toLocation(),
			                                 new Scientrace.NonzeroVector(1,0,0),
			                                 new Scientrace.NonzeroVector(0,1,0)));
			ips[1] = new Scientrace.IntersectionPoint(new Scientrace.Location(1,1,1), 
			                                 new Scientrace.FlatShape2d(v.toLocation(),
			                                 new Scientrace.NonzeroVector(1,0,0),
			                                 new Scientrace.NonzeroVector(0,1,0)));
			//Scientrace.Intersection intr = new Scientrace.Intersection(trace, ips);
			//Assert.IsTrue(intr.intersects);
			
	}

	[Test()]
	public void TestIntersectionByVectorArray2() {
			Scientrace.Object3dEnvironment env = new Scientrace.Object3dEnvironment(Scientrace.AirProperties.Instance,100);
			Scientrace.Line beam = new Scientrace.Line(10, 10, 10, -1, -1, 0);
			Scientrace.LightSource light = new Scientrace.SingleRaySource(beam,
			                                                             10, new Scientrace.AM15Spectrum(1), env);
			Scientrace.Trace trace = new Scientrace.Trace(500.0E-9, light, beam, env,1,1);
			Console.WriteLine(trace.ToString());
			Scientrace.Vector v = new Scientrace.Vector(10,10,10);
			Scientrace.IntersectionPoint[] ips = new Scientrace.IntersectionPoint[2];
			ips[0] = null;
			ips[1] = new Scientrace.IntersectionPoint(new Scientrace.Location(1,1,1), 
			                                 new Scientrace.FlatShape2d(v.toLocation(),
			                                 new Scientrace.NonzeroVector(1,0,0),
			                                 new Scientrace.NonzeroVector(0,1,0)));
			//Scientrace.Intersection intr = new Scientrace.Intersection(trace, ips);
			//Assert.IsTrue(intr.intersects);
	}

	[Test()]
	public void TestIntersectionByVectorArray3() {
			Scientrace.Object3dEnvironment env = new Scientrace.Object3dEnvironment(Scientrace.AirProperties.Instance, 100);
			Scientrace.Line beam = new Scientrace.Line(10, 10, 10, -1, -1, 0);
			Scientrace.LightSource light = new Scientrace.SingleRaySource(beam,
			                                                             10, new Scientrace.AM15Spectrum(1), env);
			Scientrace.Trace trace = new Scientrace.Trace(500.0E-9, light, beam, env,1,1);
			Console.WriteLine(trace.ToString());
			//Scientrace.Vector v = new Scientrace.Vector(10,10,10);
			Scientrace.IntersectionPoint[] ips = new Scientrace.IntersectionPoint[2];
			ips[0] = null;
			ips[1] = null;
			//Scientrace.Intersection intr = new Scientrace.Intersection(trace, ips);
			//Assert.IsFalse(intr.intersects);
	}
		

}
}
