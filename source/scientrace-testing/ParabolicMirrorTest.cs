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
public class ParabolicMirrorTest {

	[Test()]
	public void Testbaseintersection1() {
		Scientrace.Object3dEnvironment env = new Scientrace.Object3dEnvironment(Scientrace.AirProperties.Instance, 100, new Scientrace.Vector(0, -10, 75));

/*		Scientrace.CylindricalBorder cbord = new Scientrace.CylindricalBorder(
											new Scientrace.Location(0,0,0),
											new Scientrace.NonzeroVector(0,0,10), 20);*/
			Scientrace.CylindricalBorder cborder = new Scientrace.CylindricalBorder(
			new Scientrace.Location(-10,10,-10),
			new Scientrace.NonzeroVector(-1,-1,0),5);
			
		Scientrace.ParabolicMirror pm = new Scientrace.ParabolicMirror(env, Scientrace.PerfectMirror.Instance, 
			                                                            new Scientrace.Location(0,1,0),
			                                                            new Scientrace.UnitVector(0,0,1),
			                                                            1, cborder);
			Scientrace.Line traceline = new Scientrace.Line(2,0,0,0,1,4);
		Scientrace.IntersectionPoint[] intr = pm.baseintersections(traceline);
		Assert.AreEqual(true, traceline.throughLocation(intr[0].loc, 0.0001));
		Assert.AreEqual(new Scientrace.Location(2,2,8) ,intr[0].loc);
	}
	/*
	[Test()]
	public void Testrealintersections1() {
		Scientrace.Object3dEnvironment env = new Scientrace.Object3dEnvironment(Scientrace.AirProperties.Instance, 100, new Scientrace.Vector(0, -10, 75));
		Scientrace.CylindricalBorder cbord = new Scientrace.CylindricalBorder(
											new Scientrace.Location(0,0,0),
											new Scientrace.NonzeroVector(0,0,10), 20);

		Scientrace.ParabolicMirror pm = new Scientrace.ParabolicMirror(env, Scientrace.PerfectMirror.Instance, 
			                                                            new Scientrace.Location(0,1,0),
			                                                            new Scientrace.UnitVector(0,0,1),
			                                                            1, cbord);
		Scientrace.Trace trace = new Scientrace.Trace(500E-9, ScientraceTesting.DummySources.dLightSource(env), 
			                                              new Scientrace.Line(0,1,1,0,0,-1), pm, 1,1);
		Scientrace.Intersection intr = pm.realIntersections(trace, false);
		Assert.AreEqual(new Scientrace.Location(0,1,0) ,intr.enter.loc);
	}
		
	[Test()]
	public void Testrealintersections2() {
		Scientrace.Object3dEnvironment env = new Scientrace.Object3dEnvironment(Scientrace.AirProperties.Instance, 100, new Scientrace.Vector(0, -10, 75));
		Scientrace.CylindricalBorder cbord = new Scientrace.CylindricalBorder(
											new Scientrace.Location(0,0,0),
											new Scientrace.NonzeroVector(0,0,10), 20);

		Scientrace.ParabolicMirror pm = new Scientrace.ParabolicMirror(env, Scientrace.PerfectMirror.Instance, 
			                                                            new Scientrace.Location(0,0,0),
			                                                            new Scientrace.UnitVector(0,1,0),
			                                                            1, cbord);
		Scientrace.Trace trace = new Scientrace.Trace(500E-9, ScientraceTesting.DummySources.dLightSource(env), 
			                                              new Scientrace.Line(1,0,0,0,1,0), pm,1,1);
		Scientrace.Intersection intr = pm.realIntersections(trace, false);
		Assert.AreEqual(new Scientrace.Location(1,1,0) ,intr.enter.loc);
	}

	[Test()]
	public void Testrealintersections3() {
		Scientrace.Object3dEnvironment env = new Scientrace.Object3dEnvironment(Scientrace.AirProperties.Instance, 100, new Scientrace.Vector(0, -10, 75));
		Scientrace.CylindricalBorder cbord = new Scientrace.CylindricalBorder(
											new Scientrace.Location(0,0,0),
											new Scientrace.NonzeroVector(0,0,10), 20);

		Scientrace.ParabolicMirror pm = new Scientrace.ParabolicMirror(env, Scientrace.PerfectMirror.Instance, 
			                                                            new Scientrace.Location(0,0,0),
			                                                            new Scientrace.UnitVector(0,1,0),
			                                                            1, cbord);
		Scientrace.Trace trace = new Scientrace.Trace(500E-9, ScientraceTesting.DummySources.dLightSource(env), 
			                                              				new Scientrace.Line(2,0,0,0,4,1), pm,1,1);
		Scientrace.Intersection intr = pm.realIntersections(trace, false);
		Assert.AreEqual(new Scientrace.Location(2,8,2) ,intr.enter.loc);
	}*/
		
	[Test()]
	public void TestFailingRealIntersectionsForloop() {
		Scientrace.Object3dEnvironment env = DummySources.dEnv();
		Scientrace.CylindricalBorder cb = new Scientrace.CylindricalBorder(new Scientrace.Location(0, -0.07, 0), new Scientrace.NonzeroVector(0, -0.57, 0), 0.5);
		Scientrace.ParabolicMirror pm = new Scientrace.ParabolicMirror(env
			, Scientrace.PerfectMirror.Instance, new Scientrace.Location(0.354861881099294, -0.57, 0.276718350209505)
			,  new Scientrace.UnitVector(-0.343710964996787, 0.900014703397015, -0.268022958363946), 2, cb);
		for (double a = -0.184140645666577; a >= -0.5; a = a-0.01) {
		Scientrace.Line tl = new Scientrace.Line(a, -0.0383229578843359, -0.3, 0.343710966184079, -0.900014705907235, 0.268022948412108);
//		Scientrace.Line tl = new Scientrace.Line(-0.284140645666577, -0.0383229578843359, -0.3, 0.343710966184079, -0.900014705907235, 0.268022948412108);
		try {
		pm.realIntersections(tl, false);
			} catch {
			throw new Exception(a + " is fucked");
			}
			}
		//Assert.AreEqual(new Scientrace.Location(0,0,0), tips[0].loc);
		}
		
	[Test()]
	public void TestFailingRealIntersections() {
		Scientrace.Object3dEnvironment env = DummySources.dEnv();
		Scientrace.CylindricalBorder cb = new Scientrace.CylindricalBorder(new Scientrace.Location(0, -0.07, 0), new Scientrace.NonzeroVector(0, -0.57, 0), 0.5);
		Scientrace.ParabolicMirror pm = new Scientrace.ParabolicMirror(env
			, Scientrace.PerfectMirror.Instance, new Scientrace.Location(0.354861881099294, -0.57, 0.276718350209505)
			,  new Scientrace.UnitVector(-0.343710964996787, 0.900014703397015, -0.268022958363946), 2, cb);
		Scientrace.Line tl = new Scientrace.Line(-0.184140645666577, -0.0383229578843359, -0.3, 0.343710966184079, -0.900014705907235, 0.268022948412108);

//		pm.trf.transform(tl); 
//		Scientrace.IntersectionPoint[] tips = pm.realIntersections(tl);
		//Assert.AreEqual(new Scientrace.Location(0,0,0), tips[0].loc);
				
		Scientrace.VectorTransform trf = pm.trf;
		Scientrace.Line trfline = trf.transform(tl-pm.loc);
					//transform location AND direction
			Scientrace.IntersectionPoint[] ips = pm.baseintersections(trfline);
			//Scientrace.IntersectionPoint tip;
			Scientrace.IntersectionPoint[] retips = new Scientrace.IntersectionPoint[2];
			for (int ipi = 0; ipi < ips.GetLength(0); ipi++) {
				if (ips[ipi] == null) {
					retips[ipi] = null;
					continue;
				}
				
				//check below removed for performance reasons
				if (!trfline.throughLocation(ips[ipi].loc, 0.00001)) {
					string warning =@"ERROR: GOING DOWN! \n baseintersections "+trfline+" FAILED!";
					throw new ArgumentOutOfRangeException(warning+ips[ipi].loc.trico() + " not in line " + trfline);
				}
			}		

		}
		

}
}
