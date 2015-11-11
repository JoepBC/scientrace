// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections.Generic;

namespace Scientrace {


public class TestLight: Scientrace.LightSource {

	public TestLight(Scientrace.Object3dEnvironment env, DistributionPattern dispat, SpatialDistribution spadis, Scientrace.NonzeroVector mean_direction, double maxangle, int raycount) :base(env){

		/*UniformTraceModifier utm =  new UniformTraceModifier(
			//DistributionPattern.ProjectedSpiralGrid,
			//DistributionPattern.SphericalSpiralGrid,
			DistributionPattern.SingleCircle,
			//DistributionPattern.SquareGrid,
			
			//SpatialDistribution.UniformProjections,
			SpatialDistribution.UniformAngles,
			new Scientrace.NonzeroVector(1,0,0),
			new Scientrace.NonzeroVector(0,1,0),
			new Scientrace.NonzeroVector(0,0,1));*/

		UniformTraceModifier utm =  new UniformTraceModifier(
			dispat, spadis,
			mean_direction);
		utm.setMaxAngle(maxangle);
				//Console.WriteLine("MAXANGLEJ: "+(maxangle*180/Math.PI));
				//Console.WriteLine("MAXANGLEK: "+(utm.maxangle*180/Math.PI));
		int arraysize = raycount;
		double[] wavelengths = new double[arraysize];
		for (int k=0; k<arraysize; k++) {
			wavelengths[k] = 400E-9;
			}
		
		/*double[] wavelengths = new double[100]{400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9}; */
											
/*		double[] wavelengths = new double[15]{400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9,
											400E-9, 400E-9, 400E-9, 400E-9, 400E-9};*/
																						
		List<Scientrace.Location> locs = this.getLocs();
		
		int i = 0;
		foreach (double lambda in wavelengths) {
			//Scientrace.Line ray = new Line(new Location(0, 0.0, 0.0), utm.modify(++i,wavelengths.Length).toUnitVector()); //start at node 1 -> ++i !
			//this.traces.Add(new Scientrace.Trace(lambda, this, ray, env, 1 ,1));
			foreach (Location aLoc in locs) {
				this.addTracesLoc(lambda, aLoc, utm, env, i++, wavelengths.Length);
				}
			}
	}
		
	public void addTracesLoc(double lambda, Location loc, UniformTraceModifier utm, Object3dEnvironment env, int node, int total) {
		Scientrace.Line ray = new Line(loc*0.001, utm.modify(node,total).toUnitVector()); //start at node 1 -> ++i !
		this.addTrace(new Scientrace.Trace(lambda, this, ray, env, 1 ,1));
		}
		
	public List<Location> getLocs() {
		List<Location> retLocs = new List<Location>();
		//CHAR: R
		retLocs.Add(new Location(0, -4,-3));
		retLocs.Add(new Location(0, -4,-2));
		retLocs.Add(new Location(0, -4,-1));
		retLocs.Add(new Location(0, -4,0));
		retLocs.Add(new Location(0, -4,1));
		retLocs.Add(new Location(0, -4,2));
		retLocs.Add(new Location(0, -4,3));
		retLocs.Add(new Location(0, -3,-3));
		retLocs.Add(new Location(0, -3,1));
		retLocs.Add(new Location(0, -2.5,0.5));
		retLocs.Add(new Location(0, -2.5,1.5));
		retLocs.Add(new Location(0, -2,-3));
		retLocs.Add(new Location(0, -2,0));
		retLocs.Add(new Location(0, -2,2));
		retLocs.Add(new Location(0, -1.5,-2.5));
		retLocs.Add(new Location(0, -1.5,-0.5));
		retLocs.Add(new Location(0, -1.5,2.5));
		retLocs.Add(new Location(0, -1,-2));
		retLocs.Add(new Location(0, -1,-1));
		retLocs.Add(new Location(0, -1,3));
		//CHAR: U
		retLocs.Add(new Location(0, 1,-3));
		retLocs.Add(new Location(0, 1,-2));
		retLocs.Add(new Location(0, 1,-1));
		retLocs.Add(new Location(0, 1,0));
		retLocs.Add(new Location(0, 1,1));
		retLocs.Add(new Location(0, 1,2));
		retLocs.Add(new Location(0, 1.5,2.5));
		retLocs.Add(new Location(0, 2,3));
		retLocs.Add(new Location(0, 3,3));
		retLocs.Add(new Location(0, 3.5,2.5));
		retLocs.Add(new Location(0, 4,-3));
		retLocs.Add(new Location(0, 4,-2));
		retLocs.Add(new Location(0, 4,-1));
		retLocs.Add(new Location(0, 4,0));
		retLocs.Add(new Location(0, 4,1));
		retLocs.Add(new Location(0, 4,2));
		retLocs.Add(new Location(0, 4,3));
		return retLocs;
		}
		
}
}
