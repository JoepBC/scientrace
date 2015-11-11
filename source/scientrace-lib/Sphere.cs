// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public class Sphere : Scientrace.EnclosedVolume, IInterSectableBorder3d {

	/// <summary>
	/// The radius of the Sphere about location this.loc .
	/// </summary>
	public double radius;

	/* fields that describe how the X3D grid should be exported */
	public int x3d_meridians = 24;
	public int x3d_circles_of_latitude = 12;
	
	/// <summary>
	/// The reference vectors can be used to base spherical coordinate locations on the sphere upon.
	/// </summary>
	public Scientrace.UnitVector refVecX = new Scientrace.UnitVector(1,0,0);
	public Scientrace.UnitVector refVecY = new Scientrace.UnitVector(0,1,0);
	public Scientrace.UnitVector refVecZ = new Scientrace.UnitVector(0,0,1);

	public Sphere(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops,
		Scientrace.Location aLocation, double sphereRadius
		):base(parent, mprops) {
		this.loc = aLocation;
		this.radius = sphereRadius;
		}
	
	/* interface IBorder3D implementation */
	/// <summary>
	/// Return true when the specified aLocation is contained by the Sphere. If the 
	/// </summary>
	/// <param name='aLocation'>
	/// If set to <c>true</c> a location.
	/// </param>
	public bool contains(Scientrace.Location aLocation) {
		return this.contains(aLocation, 0);
		}
	/* end of interface IBorder3D implementation */
		
	public bool contains(Scientrace.Location aLocation, double margin) {
		return (aLocation.distanceTo(this.loc)<Math.Abs(this.radius+margin))
				== this.isPositive();
		}
		
		
	/// <summary>
	/// A sphere is "negative" when it is defined as an "excluded sphere" with a negative
	/// radius. Although the negative property is not yet to be trusted for a regular
	/// "Object3d" instance, this feature may be used for IBorder3d properties.
	/// </summary>
	/// <returns>
	/// True if the sphere is positive. False if negative.
	/// </returns>
	public bool isPositive() {
		return (this.sign() == 1);
		}
		
	public int sign() {
		return Math.Sign(this.radius);
		}
		
	/// <summary>
	/// The negated version of isPositive()
	/// </summary>
	/// <returns>
	/// False if the sphere is positive. True if the sphere is negative.
	/// </returns>
	public bool isNegative() {
		return !this.isNegative();
		}
		
	public void setRefAxis(Scientrace.NonzeroVector refVec) {
		if (refVec==null) {return;}
		this.refVecZ = refVec.toUnitVector();
		Scientrace.Plane tPlane = Plane.newPlaneOrthogonalTo(new Scientrace.Location(0,0,0), refVec);
		this.refVecX = tPlane.u.toUnitVector();
		this.refVecY = tPlane.v.toUnitVector();
		}
	
	public Intersection intersectsObject(Trace trace, Object3d o3dWithThisBorder) {
		Intersection retIntersection = this.intersects(trace);
		retIntersection.object3d = o3dWithThisBorder;
		return retIntersection;
		}
	
	
	public override Intersection intersects(Scientrace.Trace aTrace) {
		Scientrace.Vector c = this.loc;
		Scientrace.Vector o = aTrace.traceline.startingpoint;
		Scientrace.Vector l = aTrace.traceline.direction;
		Scientrace.Vector y = o-c;
		double r = this.radius;
		double ABCa = l.dotProduct(l);
		double ABCb = l.dotProduct(y)*2;
		double ABCc = y.dotProduct(y)-(r*r);
		QuadraticEquation qe = new QuadraticEquation(ABCa,ABCb,ABCc);
		// if the trace doesn't hit the sphere, return a "false Intersection"
		if (!qe.hasAnswers) { return new Intersection(false, this); }
		Scientrace.IntersectionPoint[] ips = new Scientrace.IntersectionPoint[2];
		switch (qe.answerCount) {
			case 2:
				Scientrace.Location minLoc = ((l*qe.minVal) + o).toLocation();
				Scientrace.NonzeroVector minNorm = (minLoc - c).tryToNonzeroVector();
				ips[0] = Scientrace.IntersectionPoint.locAtSurfaceNormal(minLoc, minNorm);
				Scientrace.Location plusLoc = ((l*qe.plusVal) + o).toLocation();
				Scientrace.NonzeroVector plusNorm = (plusLoc - c).tryToNonzeroVector();
				ips[1] = Scientrace.IntersectionPoint.locAtSurfaceNormal(plusLoc, plusNorm);
				return new Intersection(aTrace, ips, this);
				//goto case 1;	//continue to case 1
			case 1:
				Scientrace.Location loc = ((l*qe.plusVal) + o).toLocation();
				Scientrace.NonzeroVector norm = (loc - c).tryToNonzeroVector();
				ips[0] = Scientrace.IntersectionPoint.locAtSurfaceNormal(loc, norm);
				ips[1] = null;
				return new Intersection(aTrace, ips, this);
			default:
				throw new IndexOutOfRangeException("eq.answerCount is not allowed to be "+qe.answerCount.ToString()+"in Shpere.intersects(..)");
			} //end switch(qe.answerCount)
			
		}
	
	public Location getSphericalLoc(Scientrace.NonzeroVector xvec, Scientrace.NonzeroVector yvec, 
									Scientrace.NonzeroVector zvec,
									double theta, double phi) {
		return this.getSphericalLoc(xvec, yvec, zvec, this.loc, this.radius, theta, phi);
		}
		
	public Location getSphericalLoc(Scientrace.NonzeroVector xvec, Scientrace.NonzeroVector yvec, 
									Scientrace.NonzeroVector zvec,
									Scientrace.Vector sphereCenter, double sphereRadius,
									double theta, double phi) {
		return this.getEllipsoidLoc(
				xvec.normalizedVector()*sphereRadius,
				yvec.normalizedVector()*sphereRadius,
				zvec.normalizedVector()*sphereRadius,
				sphereCenter, theta, phi);
		}

	/// <summary>
	/// Gets the cylindrical location.
	/// </summary>
	/// <returns>
	/// The cylindrical location based on a set of base vectors, 
	/// the center of a sphere and two spherical locations on the sphere.
	/// </returns>
	/// <param name='xvec'>
	/// basevector X
	/// </param>
	/// <param name='yvec'>
	/// basevector Y
	/// </param>
	/// <param name='zvec'>
	/// basevector Z is the line in between theta = 0 and theta = pi.
	/// </param>
	/// <param name='sphereCenter'>
	/// Sphere center location
	/// </param>
	/// <param name='theta'>
	/// Theta runs from 0 to pi
	/// </param>
	/// <param name='phi'>
	/// Phi runs from 0 to 2*pi
	/// </param>
	public Location getEllipsoidLoc(Scientrace.Vector xvec, Scientrace.Vector yvec, 
									Scientrace.Vector zvec, Scientrace.Vector sphereCenter,
									double theta, double phi) {
		return (
			(((xvec*Math.Cos(phi)) +
			(yvec*Math.Sin(phi)))*Math.Sin(theta)) +
			(zvec*Math.Cos(theta)) + 
			sphereCenter ).toLocation();
		}
		
	public Scientrace.Location getNodeLoc(int circle_of_magnitude, int meridian) {
		if (circle_of_magnitude > (this.x3d_circles_of_latitude) || meridian > this.x3d_meridians) {
			return null;
			}
		double theta; // height of the circle of latitude (0 - pi)
		double phi; // orientation on the circles of latitude (meridian-lines) (0 - 2pi)
		theta = circle_of_magnitude * Math.PI / this.x3d_circles_of_latitude;
		phi = meridian * Math.PI * 2 / this.x3d_meridians;
		return this.getSphericalLoc(this.refVecX, this.refVecY, this.refVecZ, 
													this.loc, this.radius, theta, phi);
		}
			
	public override string exportX3D(Scientrace.Object3dEnvironment env) {
		System.Text.StringBuilder retx3d = new System.Text.StringBuilder("<!-- SPHERE GRID start -->");
		
		Scientrace.Location tNodeLoc;
		Scientrace.Location tMerConnectLoc;
		Scientrace.Location tLatConnectLoc;
		Scientrace.X3DGridPoint tGridPoint;
		
		for (int ic = 0; ic <= this.x3d_circles_of_latitude; ic++) { //0-pi
			for (int im = 0; im < this.x3d_meridians; im++) { //0-2pi
				
				tNodeLoc = this.getNodeLoc(ic, im);					
				tMerConnectLoc = 
					(ic > 0 && ic < this.x3d_meridians) ? // do not connect "points" located at north and south pole
						this.getNodeLoc(ic, (im+1)%this.x3d_meridians)
						: null;
				tLatConnectLoc = this.getNodeLoc(ic+1, im);
				
				// a gridpoint has a location and two neighbour location to which to connect to
				tGridPoint = new Scientrace.X3DGridPoint(env, tNodeLoc,
					tMerConnectLoc, tLatConnectLoc);
				retx3d.AppendLine(tGridPoint.exportX3DnosphereRGBA("0.4 0.8 0 1"));
				}}
		retx3d.Append("<!-- Sphere grid end -->");
		return retx3d.ToString();
		} //end string exportX3D(env)
		
				
		
	}}

