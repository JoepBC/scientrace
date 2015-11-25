// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {
public class DoubleConvexLens : Scientrace.EnclosedVolume {

	public Scientrace.Location sphere1CenterLoc, sphere2CenterLoc;
	public double sphere1Radius, sphere2Radius;
//	public PlaneBorder lensPlane;

	private Scientrace.Sphere dummySphere1, dummySphere2;
	
	// Constructor: plane, sphereCenterLoc, sphereRadius
	protected DoubleConvexLens(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops,
		//Scientrace.PlaneBorder lensPlane, 
		Scientrace.Location sphere1CenterLoc, double sphere1Radius,
		Scientrace.Location sphere2CenterLoc, double sphere2Radius)
		: base(parent, mprops) {
		this.paramInit(sphere1CenterLoc, sphere1Radius, sphere2CenterLoc, sphere2Radius);
	}
			
	protected void paramInit(Scientrace.Location sphere1CenterLoc, double sphere1Radius,
		Scientrace.Location sphere2CenterLoc, double sphere2Radius) {
		//this.lensPlane = lensPlane;
		this.sphere1CenterLoc = sphere1CenterLoc;
		this.sphere2CenterLoc = sphere2CenterLoc;
		this.sphere2Radius = sphere2Radius;
		this.sphere1Radius = sphere1Radius;
		this.dummySphere1 = new Scientrace.Sphere(null, null, sphere1CenterLoc, sphere1Radius);
		this.dummySphere2 = new Scientrace.Sphere(null, null, sphere2CenterLoc, sphere2Radius);
		}
	
	
	public DoubleConvexLens(ShadowScientrace.ShadowObject3d shadowObject): base(shadowObject) {
		switch (shadowObject.factory_id) {
			case "TwoRadii_and_Diameter": this.shadowFac_TwoRadii_and_Diameter(shadowObject);
				break;
			case "TwoRadii_and_Locations": this.shadowFac_TwoRadii_and_Locations(shadowObject);
				break;
			case "FocalLength_and_Diameter": this.shadowFac_FocalLength_and_Diameter(shadowObject);
				break;
			default:
				throw new ArgumentOutOfRangeException("Factory method {"+shadowObject.factory_id+"} not found for "+shadowObject.typeString());

			}
		}

		// Factory method: lensFlatCenterLoc, sphereRadius, lensDiameter
		public void initWithLensDiameter(Scientrace.Location lensPlaneCenterLoc, Scientrace.NonzeroVector lensPlaneNormal,
					double lensDiameter, double sphere1Radius, double sphere2Radius) {
			//Console.WriteLine("New lens: Loc: "+lensPlaneCenterLoc.trico()+" normal: "+lensPlaneNormal.trico()+" diameter:"+lensDiameter+" r1:"+sphere1Radius+" r2:"+sphere2Radius);
			//Scientrace.PlaneBorder aPlane = new PlaneBorder(lensPlaneCenterLoc, lensPlaneNormal);
			double r1 /*sphere radius*/ = sphere1Radius;
			double r2 /*sphere radius*/ = sphere2Radius;
			double x /*lens radius*/ = lensDiameter/2;
			if ((x > r1) || (x > r2)) {
				throw new Exception("DoubleConvexLens cannot be created, on of sphere radii is larger than lens radius: "+r1+"|"+r2+" > "+x);
				}
			/* y => distance lens center to sphere center*/
			double y1  = Math.Sqrt((r1*r1)-(x*x));
			double y2  = Math.Sqrt((r2*r2)-(x*x));
				
			Scientrace.Location sphere1Loc = lensPlaneCenterLoc - (lensPlaneNormal.normalizedVector()*y1);
			Scientrace.Location sphere2Loc = lensPlaneCenterLoc + (lensPlaneNormal.normalizedVector()*y2);
			//Console.WriteLine("sphereLoc: "+sphere1Loc.trico()+"/"+sphere2Loc.trico()); 
			//+	" lensFlatCenterLoc: "+t+" lensFlatCenterLoc:"+lensFlatCenterLoc.trico());
			
			this.paramInit(sphere1Loc, r1, sphere2Loc, r2);
			}			

		
	/// <summary>
	/// Required parameters:
	/// (double)focal_length
	/// (Location)lens_center
	/// (NonzeroVector)optical_axis
	/// (double)lens_diameter
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>
	protected void shadowFac_FocalLength_and_Diameter(ShadowScientrace.ShadowObject3d shadowObject) {
		//User values(s)
		double focalLength = (double)shadowObject.getObject("focal_length");
		Scientrace.Location lensCenter = (Scientrace.Location)shadowObject.getObject("lens_center");
		Scientrace.NonzeroVector lensPlaneNormal = (Scientrace.NonzeroVector)shadowObject.getObject("optical_axis");
		double lensDiameter = (double)shadowObject.getObject("lens_diameter");
		
		//Derived value from 1/f = (n-1)(1/r1+1/r2)
		double sphereRadii = 2*focalLength*(shadowObject.materialprops.refractiveindex(600E-9)-1);
		
		//construct!
		this.initWithLensDiameter(lensCenter, lensPlaneNormal, lensDiameter, sphereRadii, sphereRadii);
		}
		
	/// <summary>
	/// Required parameters:
	/// (Location)lens_center
	/// (NonzeroVector)optical_axis
	/// (double)lens_diameter
	/// (double)sphere1_radius
	/// (double)sphere2_radius
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>
	protected void shadowFac_TwoRadii_and_Diameter(ShadowScientrace.ShadowObject3d shadowObject) {
		//User values(s)
		Scientrace.Location lensCenter = (Scientrace.Location)shadowObject.getObject("lens_center");
		Scientrace.NonzeroVector opticalAxis = (Scientrace.NonzeroVector)shadowObject.getObject("optical_axis");
		
		double lensDiameter = (double)shadowObject.getObject("lens_diameter");
		double sphere1Radius = (double)shadowObject.getObject("sphere1_radius");
		double sphere2Radius = (double)shadowObject.getObject("sphere2_radius");

		this.initWithLensDiameter(lensCenter,opticalAxis, lensDiameter, sphere1Radius, sphere2Radius);
		}

	/// <summary>
	/// Required parameters:
	/// (Location)sphere1_center_loc
	/// (Location)sphere2_center_loc
	/// (double)sphere1_radius
	/// (double)sphere2_radius
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>			
	protected void shadowFac_TwoRadii_and_Locations(ShadowScientrace.ShadowObject3d shadowObject) {
		//User values(s)
		Scientrace.Location sphere1CenterLoc = (Scientrace.Location)shadowObject.getObject("sphere1_center_loc");
		Scientrace.Location sphere2CenterLoc = (Scientrace.Location)shadowObject.getObject("sphere2_center_loc");
		double sphere1Radius = (double)shadowObject.getObject("sphere1_radius");
		double sphere2Radius = (double)shadowObject.getObject("sphere2_radius");
		
		this.paramInit(sphere1CenterLoc, sphere1Radius, sphere2CenterLoc, sphere2Radius);
		}
		

	public bool contains(Scientrace.Location aLoc) {
		return (this.dummySphere1.contains(aLoc) && this.dummySphere2.contains(aLoc));
		}	
	
	public string oldexportX3D(Object3dEnvironment env) {
		//throw new System.NotImplementedException();
		return this.dummySphere1.exportX3D(env)+this.dummySphere2.exportX3D(env);
		}
	
	public double getLensRadius() {
		double r1 = this.sphere1Radius;
		double r2 = this.sphere2Radius;
		double l = this.dummySphere1.loc.distanceTo(this.dummySphere2.loc);
		/* the equation below is based on the "law of cosines", but r1 * sin acos X = r1*sqrt(1-X^2) */
		return r1*Math.Sqrt(1.0 - Math.Pow(
			((l*l)+(r1*r1)-(r2*r2)) / (2.0*l*r1), 2)
			// used to be: ((l*l)+(r1*r1)+(r2*r2)) / (2.0*l*r1), 2)
			);
		}
		
	public void setPlaneCenterRadians1Radians2(ref Scientrace.Location planeCenter, ref double radians1, ref double radians2) {
		// h1(s) and h2(s) are the (squared) radii of dummySphere1 and dummySphere2
		// the convention "h" is due to the triangle hypothenusa description in figure (REF)
		double h1 = this.sphere1Radius;
		double h2 = this.sphere2Radius;
		double h1s = h1*h1;
		double h2s = h2*h2;
		// d is the distance from the center of sphere1 to the center of sphere2
		double d = this.sphere1CenterLoc.distanceTo(this.sphere2CenterLoc);
		// p1 and p2 are the signs which describe whether the center of the sphere lies in between planecenter(H) and the other sphere (-1) or on the other side of H (+1).
		// definition using floating point doubles instead of integers to prevent typecasting
		double p1 = Math.Sign(d-this.sphere2Radius);
		double p2 = Math.Sign(d-this.sphere1Radius);
		// a1 is the distance from sphere1 to planeCenter H.
		double a1 = ((d*d)+h1s-h2s) / (2*d*p1);
		double a2 = (d - (a1*p1)) / p2;
		
		//vD12 is the direction from sphere1 towards sphere2
		Scientrace.UnitVector vD12;
		try {
			vD12 = (this.sphere2CenterLoc - this.sphere1CenterLoc).tryToUnitVector();
			} catch
			{ throw new ZeroNonzeroVectorException("Two spheres with equal locations ("+
					this.sphere1CenterLoc.trico()+"/"+this.sphere2CenterLoc.trico()+
					") where defined within a DoubleConvexLens instance.");
			}
		planeCenter = this.sphere1CenterLoc+vD12*a1*p1;
		radians1 = Math.Acos(a1*p1/h1);
		radians2 = Math.Acos(a2*p2/h2);
		}

	public Scientrace.Location getLensPlaneCenter() {
		Scientrace.Location planeCenter = null;
		double r1 = 0;
		double r2 = 0;
		this.setPlaneCenterRadians1Radians2(ref planeCenter, ref r1, ref r2);
		return planeCenter;
		}

	public double getRadiansSphere1() {
		Scientrace.Location planeCenter = null;
		double r1 = 0;
		double r2 = 0;
		this.setPlaneCenterRadians1Radians2(ref planeCenter, ref r1, ref r2);
		return r1;
		}
	public double getRadiansSphere2() {
		Scientrace.Location planeCenter = null;
		double r1 = 0;
		double r2 = 0;
		this.setPlaneCenterRadians1Radians2(ref planeCenter, ref r1, ref r2);
		return r2;
		}
		 		
	
	public override string exportX3D(Scientrace.Object3dEnvironment env) {
		double r1 = this.sphere1Radius;
		double r2 = this.sphere2Radius; //used to say sphere1Radius too?
		//The lensRadians is the angle made from the center of a sphere along the center of the lens to the side of the lens.
		double lens1Radians = this.getRadiansSphere1();
		double lens2Radians = this.getRadiansSphere2();
		
		Scientrace.NonzeroVector lens1to2Dir = (dummySphere2.loc - dummySphere1.loc).tryToUnitVector()*Math.Sign(r1)*Math.Sign(r2);
		
		Scientrace.NonzeroVector lens1Dir = lens1to2Dir*Math.Sign(r2);
		Scientrace.NonzeroVector lens2Dir = lens1to2Dir*-1*Math.Sign(r1);
		

		NonzeroVector baseVec1 = null;
		NonzeroVector baseVec2 = null;
		lens1to2Dir.fillOrtogonalVectors(ref baseVec1, ref baseVec2);

		double lat_circles = 3;
		double meridians = 12;

						
		System.Text.StringBuilder retx3d = new System.Text.StringBuilder(1024);
		retx3d.AppendLine("<!-- DOUBLECONVEXLENS GRID start -->");
		X3DShapeDrawer xsd = new X3DShapeDrawer();
		xsd.primaryRGB = "0.4 0 0.2";
		retx3d.Append(xsd.drawSphereSlice(this, lat_circles, meridians, this.dummySphere1, 0, lens1Radians, lens1Dir.toUnitVector()));
		retx3d.Append(xsd.drawSphereSlice(this, lat_circles, meridians, this.dummySphere2, 0, lens2Radians, lens2Dir.toUnitVector()));
		retx3d.AppendLine("<!-- DOUBLECONVEXLENS GRID end -->");

		return retx3d.ToString();


		/*
		Scientrace.Location tNodeLoc;
		Scientrace.Location tMerConnectLoc;
		Scientrace.Location tLatConnectLoc;
		Scientrace.X3DGridPoint tGridPoint;				
		double pi2 = Math.PI*2;

		retx3d.Append("\t<!-- Convex part -->" );
		
		for (double iSphereCircle = 2*lat_circles; iSphereCircle > 0; iSphereCircle--) {		
			for (double iSphereMerid = 0.5; iSphereMerid < 2*meridians; iSphereMerid++) {
				//double lat_angle = lensRadians * (iSphereCircle / lat_circles); // theta
				//double mer_angle = pi2 * (iSphereMerid/meridians); // mer_angle = phi

				tNodeLoc = this.dummySphere1.getSphericalLoc(
							baseVec1, baseVec2,
							lens1to2Dir,
							lens1Radians * (iSphereCircle / (2*lat_circles)), // lat_angle = theta
							pi2 * (iSphereMerid/(2*meridians)) // mer_angle = phi
							);
				if (!tNodeLoc.isValid())
					throw new NullReferenceException("Cannot calculate base gridpoint at @ "+this.tag);
				tMerConnectLoc = this.dummySphere1.getSphericalLoc(
							baseVec1, baseVec2,
							lens1to2Dir,
							lens1Radians * (iSphereCircle / (2*lat_circles)), // lat_angle = theta
							pi2 * ((iSphereMerid+1)/(2*meridians)) // mer_angle = phi
							);														
				if (!tMerConnectLoc.isValid())
					throw new NullReferenceException("Cannot calculate meridian gridpoint at @ "+this.tag);
							
				tLatConnectLoc = this.dummySphere1.getSphericalLoc(
							baseVec1, baseVec2,
							lens1to2Dir,
							lens1Radians * ((iSphereCircle-1) / (2*lat_circles)), // lat_angle = theta
							pi2 * ((iSphereMerid)/(2*meridians)) // mer_angle = phi
							);	
				if (!tLatConnectLoc.isValid())
					throw new NullReferenceException("Cannot calculate lateral gridpoint at @ "+this.tag);

				tGridPoint = new Scientrace.X3DGridPoint(env, tNodeLoc, tMerConnectLoc, tLatConnectLoc);
				retx3d.AppendLine(tGridPoint.exportX3Dnosphere("0.2 0 0.4 1"));
				}} // end for iSphereCircle / iSphereMerid
		retx3d.Append("\t<!-- End of Convex part -->" );
		retx3d.Append("<!-- End of DOUBLECONVEXLENS GRID -->"); */
		
		} //end string exportX3D(env)	
	
	
	
	public override Intersection intersects(Trace aTrace) {
	
		// Finding intersectionpoints at spheres
		Scientrace.Intersection sphere1Intersections = this.dummySphere1.intersects(aTrace);
		Scientrace.Intersection sphere2Intersections = this.dummySphere2.intersects(aTrace);

		/* No sphere intersections at all? */			
		if (!sphere1Intersections.intersects && !sphere2Intersections.intersects) { 
			return Intersection.notIntersect(this);
			}

		List<IntersectionPoint> iplist = new List<IntersectionPoint>();

		//conditional &&'s, locations are only probed if existing
		if ((sphere1Intersections.enter!=null) && this.dummySphere2.contains(sphere1Intersections.enter.loc)) {
			iplist.Add(sphere1Intersections.enter);
			}
		if ((sphere1Intersections.exit!=null) && this.dummySphere2.contains(sphere1Intersections.exit.loc)) {
			iplist.Add(sphere1Intersections.exit);
			}
		if ((sphere2Intersections.enter!=null) && this.dummySphere1.contains(sphere2Intersections.enter.loc)) {
			iplist.Add(sphere2Intersections.enter);
			}
		if ((sphere2Intersections.exit!=null) && this.dummySphere1.contains(sphere2Intersections.exit.loc)) {
			iplist.Add(sphere2Intersections.exit);
			}

		/* No valid intersections? */
		 if (iplist.Count < 1) {
			return Intersection.notIntersect(this);
			}

		Scientrace.IntersectionPoint[] ips = iplist.ToArray();
		Scientrace.Intersection lensIntersection = new Scientrace.Intersection(aTrace, ips, this);

		// when currently inside the lens, intersection from here must mean leaving.
		lensIntersection.leaving = this.contains(aTrace.traceline.startingpoint);
		//return created new intersection
		return lensIntersection;
		}

	}}

