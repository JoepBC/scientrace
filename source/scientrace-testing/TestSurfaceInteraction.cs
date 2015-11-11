// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using NUnit.Framework;

namespace ScientraceTesting {

public class TestSurfaceInteraction {	

	/// <summary>
	/// The angle of a refracted beam can be calculated from the projection-vector. This test case checks whether
	/// the angle with the surface normal meets the demands from Snellius.
	/// </summary>
	/// <value>The test refraction angle from vectors.</value>
	[Test()]
	public void TestRefractionAngleFromVectors() {

		Scientrace.UnitVector direction_incoming_trace = new Scientrace.UnitVector(1,0,1);
		Scientrace.Location loc = new Scientrace.Location(0,0,0);
		Scientrace.UnitVector surface_normal = new Scientrace.UnitVector(0,0,1);

		Scientrace.Trace aTrace =  DummyObjects.DummyTrace(600E-9);
				aTrace.traceline.direction = direction_incoming_trace;

		double refindex_from = 1;
		double refindex_to = 1.5;
		Scientrace.DielectricSurfaceInteraction fsi = new Scientrace.DielectricSurfaceInteraction(
														aTrace, loc, 
														surface_normal, refindex_from, refindex_to, null);
		//Assert.AreEqual(fsi.dir_in.ToString(), fsi.surface_normal);
		Assert.AreEqual(fsi.dir_s.angleWith(fsi.dir_ip), Math.PI/2);
		Assert.AreEqual(Math.Abs(fsi.amp_ip), Math.Abs(fsi.amp_is), 1E-10);
		}


}
}

