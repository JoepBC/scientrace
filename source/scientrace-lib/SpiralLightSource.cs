// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class SpiralLightSource : ParallelLightSource {
		
	double loops, radius;
	Scientrace.Location center;
	Scientrace.Plane spiral_plane;
	//added later (20131017)
	public int linecount;
	public Scientrace.UnitVector direction;	
	
	public SpiralLightSource(ShadowScientrace.ShadowLightSource shadowObject): base(shadowObject) {
		int ray_count = (int)shadowObject.getObject("ray_count");
		double loops = shadowObject.getDouble("loops", -1);
		if (loops == -1) {
			loops = 1.0154 * Math.Pow(Math.PI*2*(1-Math.Sqrt(((double)ray_count - 1) / (double)ray_count)), -0.5);
			Console.WriteLine("Number of loops for "+ray_count+" beams set to: {"+loops.ToString("#,0.000")+"}.");
			}

		this.paramInit(
			(Scientrace.Location)shadowObject.getObject("location"), //center
			(Scientrace.UnitVector)shadowObject.getObject("direction"),//direction,
			(Scientrace.Plane)shadowObject.getObject("spiral_plane"),//plane,
			ray_count,//linecount,
			(double)shadowObject.getObject("radius"),//radius,
			loops,//loops, 
			(double)shadowObject.getObject("distance")//distance,
			//(Scientrace.LightSpectrum)shadowObject.getObject("spectrum")//spectrum
			);
		}
	
	public SpiralLightSource(Scientrace.Object3dEnvironment env, Scientrace.Location center, 
		                         Scientrace.UnitVector direction, Scientrace.Plane plane, int linecount,
		                         double radius, double loops, double distance, LightSpectrum spectrum) : base(env) {

		/*Scientrace.UniformTraceModifier utm = 
			//new UniformTraceModifier(DistributionPattern.PeriodicSingleCircle, SpatialDistribution.UniformAngles);
			new UniformTraceModifier(DistributionPattern.SquareGrid, SpatialDistribution.UniformProjections);
		utm.add_self = false;
		utm.setMaxAngleDeg(0.5);
		utm.modify_traces_count = 16;
		utm.randomSeed = 1;
		this.modifiers.Add(utm); */
		//this.init_findmultip(env, center, direction, plane, linecount, radius, loops, spectrum);
		
		this.paramInit(center,direction,plane,linecount,radius,loops, distance);
		}

	public void paramInit(Scientrace.Location center, 
		                         Scientrace.UnitVector direction, Scientrace.Plane plane, int linecount,
		                         double radius, double loops, double distance) {
		                        
		this.loops = loops;
		this.radius = radius;
		this.center = center;
		//direction had been commented out. WHY? Re-enabled.
		this.direction = direction;
		this.spiral_plane = plane;
		this.tag = String.Format("{0:0.###}",loops)+"LoopSpiral";
		this.distance = distance;
		//added later:
		this.linecount = linecount;
		//and removed again:
		//this.spectrum = spectrum;
		}
		
	//public DoubleConvexLens(ShadowScientrace.ShadowObject3d shadowObject): base(shadowObject) {				
								
		
	public override void createStartTraces() {
		//Console.WriteLine("CENTER AT: "+center.trico());

		Scientrace.UnitVector urange, vrange;
		urange = this.spiral_plane.u.toUnitVector();
		vrange = this.spiral_plane.v.toUnitVector();
		if (urange.dotProduct(vrange)>0.01) { Console.WriteLine("**WARNING: plane vectors far from orthogonal"); }
		Scientrace.Line line;
		Scientrace.Location loc;
		for (int n = 1; n <= linecount; n++) {
			double phi = Math.Sqrt(((double)n-0.5)/linecount);
			
			loc = this.center + 
					urange.toLocation()*(this.radius*phi*
						Math.Sin(2*Math.PI*this.loops*phi)) +
					vrange.toLocation()*(this.radius*phi*
						Math.Cos(2*Math.PI*this.loops*phi));

			line = new Scientrace.Line(loc,this.direction);
			// DISTANCE NOW MOVED TO ADDTRACE METHOD. line = new Scientrace.Line(loc+(this.direction.negative().toVector()*this.distance),this.direction);
			//Console.WriteLine("New line: "+line.ToString());
		if (spectrum.it(n)==0) {
				continue;
				}
		Scientrace.Trace ttrace = new Scientrace.Trace(spectrum.wl(n), 
											this, line, env, spectrum.it(n),  spectrum.it(n));
											
		ttrace.traceid = "spiral_"+n.ToString();
		//this.traces.Add(ttrace);
		this.addTrace(ttrace);
		}
	}
	
/* Function moved to LightSpectrum
	public static bool sharePrimes(int a, int b) {
		Console.WriteLine("CHECK: Do "+a+" and "+b+" share primes?");
		int ta = Math.Max(a,b);
		int tb = Math.Min(a,b);
		if (tb == 1 ) { // 1 isn't prime so no common primes possible.
			Console.WriteLine("NO: tp==1");
			return false;
			}
		int remainder;

		do {
			remainder = (ta%tb);
			if (remainder == 0) {
				Console.WriteLine("YES: remainder==0");
				return true;
				}
			ta = tb;
			tb = remainder;
			} while (remainder != 1);
		Console.WriteLine("just return false");
		return false;
		}*/

	public override string ToString() {
		return "A SpiralLightSource instance with loops: "+
				this.loops+", radius: "+
				this.radius+ ", tracecount(): "+
				this.traceCount() +
				" center: "+this.center.trico()+ ", direction: "+this.direction.trico();
	}
		
}
}
