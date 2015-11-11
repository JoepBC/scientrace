// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {

[Flags]
/// <summary>
/// The DistributionPattern describes the method via which points are distributed over
/// 2D surface.
/// </summary>
public enum DistributionPattern {
	// Not set
	Undefined = 0,
	// Divide the modifications via a square grid 
	SquareGrid = 1,
	// Divide modifications (seeded) randomly, but uniform on average, on a circle surface
	RandomUniformDensityCircle = 2,
	// Divide modifications (seeded) randomly on a circle surface, but with a distribution
	// density which is linear with the radius instead of the surface.
	RandomAngleRadiusCircle = 3,
	// Divide modifications along a "spiral distribution" for a 2D surface
	ProjectedSpiralGrid = 4,
	/*// Divide modifications along a "spiral distribution" for a 3D spheric surface
	//SphericalSpiralGrid = 5, <-- currently disabled */
	// Divide modifications periodically on (NOT within!) a circle (-> ring) with a fixed number of nodes
	PeriodicRing = 6,
	// Divide modifications randomly on (NOT within!) a circle with a fixed ratio
	RandomRing = 7
	}
	
[Flags]
/// <summary>
/// A distribution on a 2D surface (creaded by a DistributionPattern) can be translated 
/// to spatial vectors using different methods described by the SpatialDistribution flags.
/// </summary>
public enum SpatialDistribution {
	Undefined = 0,
	// Uniform angles (which means that equal distances between points translate to
	// equal distances between angles) distribute the 2D pattern on a spherical surface 
	// range (-1:1, -1:1) using vectors with equal radii. 
	// A rough example is drawn below:
	//     _
	//    /|\
	//   (\|/)
	UniformAngles = 1,
	// Uniform projections simply translates the 2D pattern to a pattern described by two
	// (3D base) vectors on a given distance from the origin.
	//   _____
	//   \ | /
	//    \|/ 
	UniformProjections = 2
	}


/* On LIGHTSOURCES TraceModifiers will work PARALLEL. On OBJECT3D-INSTANCES they work LINEAR. 
 * here's some documentation/explanation:
 * 
 * == Linear modifiers (on objects) ==
 * When a lightbeam passes an object's surface it interacts with the surface plane.
 * Physical objects alway differ (slightlY) from their theoretic mathematical shapes,
 * and TraceModifiers can be used to describe these abberations. In order to produce
 * a single altered plane, a specific alteration has to be chosen out of all possible
 * alterations. If the trace would have been split up (and divided by intensity) at each
 * Object3D surface, the performance of the simulation would slow down as the product
 * of all modification-traces (exponentially as a function of the number of passed objects)
 * which is unacceptable.
 *
 * == Parallel ==
 * A LightSource instance on the other hand *can* split up its original traces, as this happens
 * only once. There might not even be the need to divide the intensity of all split up 
 * beams, as they all add up to the total of "emitted lightsource traces".
 */		

/// <summary>
/// The TraceModifyAngularAperture class creates a distribution of traces about 
/// a given vector.
/// </summary>
public class UniformTraceModifier : TraceModifier {

	/// <summary>
	/// The mean vector about which all modified vectors are created.
	/// </summary>
	public NonzeroVector meanVec = null;
	
	public NonzeroVector baseVec1 = null;
	public NonzeroVector baseVec2 = null;
	
	
	//randomSeed = null for really random data.
	public int? randomSeed = null;
	public Random utm_rnd = null;

	public DistributionPattern distrPattern = DistributionPattern.Undefined;
	public SpatialDistribution spatialDistribution = SpatialDistribution.Undefined;

	public bool add_self = true;
	public int modify_traces_count = 1;

	/// <summary>
	/// The maxangle for the spatial distribution.
	/// </summary>
	public double maxangle = Math.PI*0.25;
	/// <summary>
	/// The extent of the sphere that correlates with a 2D distance "1" (default: Math.PI*0.5, be carefull to change this value)
	/// </summary>
	//private double sphereExtent = Math.PI*0.5;

	
	/* LINEAR MODIFIER ATTRIBUTES >>> */

	/// <summary>
	/// A modifier will return a number of traces (modifier.modify_traces_count). The iterator will
	/// tell the simulation which one to take. This parameter is only used then random_modification_select
	/// is set false.
	/// </summary>
	public static uint modifier_iterator = 0;

    /// <summary>
    /// The "random_modification_select" bool defines if the modified orientation is picked based on the 
    /// modifier_iterator value or at random.
    /// </summary>
	public bool random_modification_select = true;

	/* <<< LINEAR MODIFIERS ATTRIBUTES */


	/// <summary>
	/// plain argumentless constructor: preferentially use argumented constructors for this class
	/// </summary>
	public UniformTraceModifier() {
		//set some default values
		this.distrPattern = DistributionPattern.ProjectedSpiralGrid;
		this.spatialDistribution = SpatialDistribution.UniformProjections;
		}

	public UniformTraceModifier(DistributionPattern distributionpattern, SpatialDistribution spatialdistribution) {
		this.distrPattern = distributionpattern;
		this.spatialDistribution = spatialdistribution;
		}

	public UniformTraceModifier(DistributionPattern distributionpattern, SpatialDistribution spatialdistribution,
				NonzeroVector meanvector, NonzeroVector basevector1, NonzeroVector basevector2) {
		this.distrPattern = distributionpattern;
		this.spatialDistribution = spatialdistribution;
		this.meanVec = meanvector;
		this.baseVec1 = basevector1;
		this.baseVec2 = basevector2;
		}


	public UniformTraceModifier(DistributionPattern distributionpattern, SpatialDistribution spatialdistribution,
				NonzeroVector meanvector) {
		this.distrPattern = distributionpattern;
		this.spatialDistribution = spatialdistribution;
		this.meanVec = meanvector;
		meanvector.fillOrtogonalVectors(ref this.baseVec1, ref this.baseVec2);
		}
	
	
	public Scientrace.Trace modify(Scientrace.Trace aTrace, double nodenumber, double nodetotal) {
		Scientrace.Trace retTrace = aTrace.clone();
		retTrace.traceline = this.modify(aTrace.traceline, nodenumber, nodetotal);
		return retTrace;
		}

	public Scientrace.Line modify(Scientrace.Line aLine, double nodenumber, double nodetotal) {
		Scientrace.Line retLine = aLine.copy();
		retLine.direction = this.modify(retLine.direction, nodenumber, nodetotal).toUnitVector();
		return retLine;
		}


	public Scientrace.NonzeroVector modify(Scientrace.NonzeroVector meanvector, double nodenumber, double nodetotal) {
		this.meanVec = meanvector;
		this.meanVec.fillOrtogonalVectors(ref this.baseVec1, ref this.baseVec2);
		return this.modify(nodenumber, nodetotal);
		}


	/* LINEAR MODIFIER METHODS >>> */
	
	public Scientrace.Plane linearModify(Scientrace.Plane aPlane) {
		Scientrace.NonzeroVector aSurfaceNormal = aPlane.getNorm();
		Scientrace.NonzeroVector newNormal = 
			this.modify(aSurfaceNormal, 
			this.random_modification_select ? this.getRandomNodeNumber() : this.getNextNodeNumber(), 
			this.modify_traces_count);
		return Scientrace.Plane.newPlaneOrthogonalTo(aPlane.loc,
			newNormal, aPlane.u, aPlane.v);
		}


	public int getRandomNodeNumber() {
		return Convert.ToInt32(this.getRnd().NextDouble()*this.modify_traces_count);
		}
		
	public int getNextNodeNumber() {
		return Convert.ToInt32(++UniformTraceModifier.modifier_iterator % this.modify_traces_count);
		}

	/// <summary>
	/// Create a "modified" vector, based on this.meanVec, node "nodenumber" out of a total "nodetotal"
	/// </summary>
	/// <param name='nodenumber'>
	/// Nodenumber. WARNING: the lowest possible value is 1 (not 0) for some implementations, largest is nodetotal.
	/// </param>
	/// <param name='nodetotal'>
	/// The total number of nodes (although the implementation may be periodic with period nodetotal).
	/// </param>
	public NonzeroVector modify(double nodenumber, double nodetotal) {
		Vec2d t2dVector;
		
		//let the distributionpattern define how the temporary 2d vector is to be assigned:
		switch (this.distrPattern) {
/* DISABLED			case DistributionPattern.SphericalSpiralGrid:
				t2dVector = this.getSphereSpiralNode(nodenumber, nodetotal);
				break;*/
			case DistributionPattern.ProjectedSpiralGrid:
				t2dVector = this.getProjectedSpiralNode(nodenumber, nodetotal);
				break;
			case DistributionPattern.SquareGrid:
				t2dVector = this.getSquareGridNode(nodenumber, nodetotal);
				break;
			case DistributionPattern.RandomAngleRadiusCircle:
				t2dVector = this.getRandomAngleRadiusCircleNode();
				break;
			case DistributionPattern.RandomUniformDensityCircle:
				t2dVector = this.getRandomUniformDensityCircleNode();
				break;
			case DistributionPattern.PeriodicRing:
				t2dVector = this.getPeriodicRingNode(nodenumber, nodetotal);
				break;
				case DistributionPattern.RandomRing:
				t2dVector = this.getRandomRingNode();
				break;
			default:
				throw new NotImplementedException("DistributionPattern "+this.distrPattern+
						" has not been implemented for "+this.GetType().Name);
			}
		
		switch (this.spatialDistribution) {
			case SpatialDistribution.UniformAngles:
				double cx = t2dVector.x;
				double cy = t2dVector.y;
				double r = Math.Sqrt((cx*cx) + (cy*cy));
				//double xyangle = Math.Atan2(cx, cy);
				double xyangle = Math.Atan2(cy, cx);
			
				double theta = Math.Acos(1-(r*r));
				return (
					(this.meanVec.normalizedVector()*Math.Cos(theta)) +
					(this.baseVec1.normalizedVector()*Math.Cos(xyangle)*Math.Sin(theta)) +
					(this.baseVec2.normalizedVector()*Math.Sin(xyangle)*Math.Sin(theta))
					).tryToNonzeroVector();
				
				//break;
			case SpatialDistribution.UniformProjections:
				return (this.meanVec+
						(this.baseVec1.toVector()*t2dVector.x)+
						(this.baseVec2.toVector()*t2dVector.y)
						).tryToNonzeroVector();
				//break;
			default:
				throw new NotImplementedException("SpatialDistribution "+this.spatialDistribution+
						" has not been implemented for "+this.GetType().Name);
			}
		}
		
		
	public void setMaxAngleDeg(double deg_maxangle) {
		this.setMaxAngle(deg_maxangle*Math.PI/180);
		}
		
	public void setMaxAngle(double max_angle_radians) {
		switch (this.spatialDistribution) {
			case SpatialDistribution.UniformProjections:
				if (Math.Pow(Math.Tan(max_angle_radians),2) > 2) { 
					throw new ArgumentOutOfRangeException("SetMaxAngle("+max_angle_radians+" | "+(max_angle_radians*180/Math.PI)+
						" deg) for a UniformProjection may not be larger than atan(sqrt(2)) = 0.955316618 radians 54.735610317 deg.");
					//throw new ArgumentOutOfRangeException("SetMaxAngle("+max_angle+" | "+(max_angle*180/Math.PI)+" deg) for a UniformProjection may not be larger than pi/2.");
					}
				this.maxangle = angle3dForRadius2d(Math.Tan(max_angle_radians));
				break;
			default: 
				this.maxangle = max_angle_radians;
				break;
			}
		//Console.WriteLine("max_angle: "+max_angle+" becomes: "+this.maxangle);
		}
		
		
/* CONVERSION METHODS 2D / 3D >>> */
	/// <summary>
	/// An angle (radians) for a radius (radius[r] 0->1 equals 0->pi/2 radians but not linearly)
	/// </summary>
	public double angle3dForRadius2d(double r) {
		return Math.Acos(1-(r*r));
		}
	/// <summary>
	/// A radius (radius 0->1 equals 0->pi/2 radians but not linearly) for an angle (theta[radians])
	/// </summary>
	public double radius2dForAngle3d(double theta) {
		return Math.Sqrt(1-Math.Cos(theta));
		}		
/* <<< CONVERSION METHODS 2D / 3D */
		
/* --- METHODS CONSIDERING THE SQUARE GRID DISTRIBUTION >>> */
	public Dictionary<double, List<Vec2d>> squareGrids = new Dictionary<double, List<Vec2d>>();


	public void fillSquareGrid(double nodetotal) {
		List<Vec2d> aList = new List<Vec2d>();
		double excessnodetotal = nodetotal * (4.0/Math.PI) / Math.Pow(this.radius2dForAngle3d(this.maxangle),2);
		double noderoot = Math.Sqrt(excessnodetotal);
		double inbetweendistance = 2.0/noderoot;
		for (double x = 0-(Math.Floor(noderoot/2.0)*inbetweendistance); x<1; x = x+inbetweendistance) {
			for (double y = 0-(Math.Floor(noderoot/2.0)*inbetweendistance); y<1; y = y+inbetweendistance) {
				if(Math.Sqrt((x*x)+(y*y)) <= this.radius2dForAngle3d(this.maxangle)) {
					aList.Add(new Vec2d(x,y));
					}
				//Console.WriteLine("x: "+x+" y:"+y+" ibd:"+inbetweendistance);
				}
			}
		this.squareGrids.Add(nodetotal, aList);
		Console.WriteLine("Added square grid: "+aList.Count+"/"+nodetotal);
		}

	//lazy init method
	public List<Vec2d> getSquareGrid(double nodetotal) {
		if (!this.squareGrids.ContainsKey(nodetotal)) {
			this.fillSquareGrid(nodetotal);
			}
		return this.squareGrids[nodetotal];
		}

	public Vec2d getPeriodicSquareGridNode(double nodenumber, double nodetotal) {
		List<Vec2d> sqgrid = this.getSquareGrid(nodetotal);
		return sqgrid[Convert.ToInt32(nodenumber)%sqgrid.Count];
		
		}

	public Vec2d getSquareGridNode(double nodenumber, double nodetotal) {
		return this.getPeriodicSquareGridNode(nodenumber, nodetotal);
		}
/* <<< METHODS CONSIDERING THE SQUARE GRID DISTRIBUTION --- */


	public Random getRnd() {
		return (this.utm_rnd == null) ?
			(this.utm_rnd = 
				(this.randomSeed == null) ? 
				new Random() 
				: new Random((int)this.randomSeed))
			: this.utm_rnd;
		}


	public Vec2d getRandomUniformDensityCircleNode() {
		Random rnd = this.getRnd();
		Vec2d retVec = new Vec2d(((2*rnd.NextDouble())-1)*this.radius2dForAngle3d(this.maxangle), 
			((2*rnd.NextDouble())-1)*this.radius2dForAngle3d(this.maxangle));
		// is the created 2d vector within the circle boundaries?
		if (retVec.length() > this.radius2dForAngle3d(this.maxangle)) {
			//wrong size? make new one and return it.
			return this.getRandomUniformDensityCircleNode();
			}
		return retVec;
		}

	public Vec2d getRandomAngleRadiusCircleNode() {
		Random rnd = this.getRnd();
		double theta = rnd.NextDouble()*Math.PI*2;
		double r = rnd.NextDouble()*this.radius2dForAngle3d(this.maxangle);
		return new Vec2d(r*Math.Cos(theta), r*Math.Sin(theta));
		}


/* --- METHODS AND ATTRIBUTES CONSIDERING THE SINGLECIRCLE DISTRIBUTIONS >>> */
	public Vec2d getRandomRingNode() {
		Random rnd = this.getRnd();
		return this.getRingNode(this.radius2dForAngle3d(this.maxangle), rnd.NextDouble()*Math.PI*2.0);
		}

	public int circleElements = 8;
	public int iCircleElement = 0;
	/// <summary>
	/// Make sure that circleElements and iCircleInc do not share any prime-divisors.
	/// </summary>
	public int iCircleInc = 5;
	public Vec2d getPeriodicRingNode() {
		this.iCircleElement = (this.iCircleElement + this.iCircleInc) % this.circleElements;
		return this.getPeriodicRingNode(this.iCircleElement, this.circleElements);
		/*double angle2d = Math.PI*2.0*((double)this.iCircleElement / this.circleElements);
		double r = this.radius2dForAngle3d(this.maxangle);
		return getSingleCircleNode(r, angle2d);*/
		}

	public Vec2d getPeriodicRingNode(double nodenumber, double nodetotal) {
		double angle2d = Math.PI*2.0*(nodenumber / nodetotal);
		double r = this.radius2dForAngle3d(this.maxangle);
		return getRingNode(r, angle2d);
		}
						
	public Vec2d getRingNode(double circleRadius, double nodeOrientation) {
		//Console.WriteLine("Radius: "+circleRadius+ " maxangle: "+this.maxangle);
		Vec2d retvec = new Vec2d(circleRadius*Math.Cos(nodeOrientation), circleRadius*Math.Sin(nodeOrientation));
		return retvec;
		}
/* <<< METHODS CONSIDERING THE SINGLECIRCLE DISTRIBUTION --- */


/* --- METHODS CONSIDERING THE SPHERE SphericalSpiralGrid SPIRAL LOOP DISTRIBUTION >>> */
	public Vec2d getSphereSpiralNode(double nodenumber, double nodetotal) {
		//WARNING: This function wasn't implemented properly, it just uses a larger number of loops (arbitrairy 4*pi times the projected #).
		return this.getProjectedSpiralNode(nodenumber, nodetotal, this.getEqualSpaceLooptotal(nodetotal)*Math.PI*4);
		}
/* <<< METHODS CONSIDERING THE SphericalSpiralGrid SPIRAL LOOP DISTRIBUTION --- */



/* --- METHODS CONSIDERING THE PROJECTED SPIRAL LOOP DISTRIBUTION >>> */

	/// <summary>
	/// Gets the total number of loops to be produced for a number of nodes
	/// in order to get an approximate equal average distance in between nodes
	/// on the same loop and the closest on neighbouring loops.
	/// </summary>
	/// <returns>
	/// The equal space looptotal.
	/// </returns>
	/// <param name='nodetotal'>
	/// Gets the total number of loops to be produced for a number of nodes
	/// in order to get an approximate equal average distance in between nodes
	/// on the same loop and the closest on neighbouring loops.
	/// </param>
	public double getEqualSpaceLooptotal(double nodetotal) {
		return 1.0154 * Math.Pow(
			Math.PI*2*(1-Math.Sqrt((nodetotal - 1) / nodetotal)), -0.5);
		}

	public Vec2d getProjectedSpiralNode(double nodenumber, double nodetotal) {
		return this.getProjectedSpiralNode(nodenumber, nodetotal, this.getEqualSpaceLooptotal(nodetotal));
		//return this.getSpiral(nodenumber, nodetotal, 3);
		//return this.getSpiral(nodenumber, nodetotal, this.getEqualSpaceLooptotal(nodetotal));
		}
		
	public Vec2d getProjectedSpiralNode(double nodenumber, double nodetotal, double looptotal) {
		double phi = Math.Sqrt(((double)nodenumber-0.5)/nodetotal);
		double orient = 2*Math.PI*looptotal*phi;
		return new Vec2d(phi*Math.Sin(orient), phi*Math.Cos(orient));
		}

/*	public double getSpiralComponent(double nodenumber, double nodetotal, bool component) {
		return component?	this.getProjectedSpiralNode(nodenumber,nodetotal).x:
							this.getProjectedSpiralNode(nodenumber,nodetotal).y;
		}
		
	public double getSpiralComponent(int nodecount, int nodetotal, bool component) {
		return this.getSpiralComponent(Convert.ToDouble(nodecount), Convert.ToDouble(nodetotal), component);
		}
			
	public double getSpiralComponent1(double nodenumber, double nodetotal) {
		return this.getSpiralComponent(nodenumber, nodetotal, true);
		}

	public double getSpiralComponent2(double nodenumber, double nodetotal) {
		return this.getSpiralComponent(nodenumber, nodetotal, false);
		}*/
		
/* <<< METHODS CONSIDERING THE PROJECTED SPIRAL LOOP DISTRIBUTION --- */


	public override string ToString() {
		return this.distrPattern.ToString()+"/"+this.spatialDistribution+"/"+this.maxangle;
		}

}
}

