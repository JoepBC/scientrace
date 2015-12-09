// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public class PlanoConvexLens : Scientrace.EnclosedVolume {

		public Scientrace.Location sphereCenterLoc;
		public double sphereRadius;
		public PlaneBorder lensPlane;

		private Scientrace.Sphere dummySphere;

		// Constructor: plane, sphereCenterLoc, sphereRadius
		protected PlanoConvexLens(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops,
			Scientrace.PlaneBorder lensPlane, Scientrace.Location sphereCenterLoc, double sphereRadius)
			: base(parent, mprops) {	
			this.lensPlane = lensPlane;
			this.sphereCenterLoc = sphereCenterLoc;
			this.sphereRadius = sphereRadius;
			this.dummySphere = new Scientrace.Sphere(null, null, sphereCenterLoc, sphereRadius);
			}

		
		// Factory method: lensFlatCenterLoc, sphereRadius, lensDiameter
		static public Scientrace.PlanoConvexLens CreateWithLensDiameter(Scientrace.Object3dCollection parent, 
					Scientrace.MaterialProperties mprops,
					Scientrace.Location lensFlatCenterLoc, double lensDiameter,
					Scientrace.NonzeroVector sphereRadiusVec) {
			Scientrace.PlaneBorder aPlane = new PlaneBorder(lensFlatCenterLoc, sphereRadiusVec);
			double r /*sphere radius*/ = sphereRadiusVec.length;
			double x /*lens radius*/ = lensDiameter/2;
			double y /*distance lens center to sphere center*/ = Math.Sqrt((r*r)-(x*x));
			double t /*thickness*/ = r-y;
			
			//Console.WriteLine("sphereradius: "+r+" lens thickness: "+t+" y:"+y+" x:"+x);
			
			if (t<=0) {
				throw new Exception("This PlanoConvexLens cannot be created: "+sphereRadiusVec.trico()+"/"+lensDiameter);
				}
			Scientrace.Location sphereLoc = lensFlatCenterLoc - (sphereRadiusVec.normalizedVector()*y);
			//Console.WriteLine("sphereLoc: "+sphereLoc.trico()+" lensFlatCenterLoc: "+t+" lensFlatCenterLoc:"+lensFlatCenterLoc.trico());
			return new PlanoConvexLens(parent, mprops, aPlane, sphereLoc, r);
			}			
			
		public bool contains(Scientrace.Location aLoc) {
			return (this.dummySphere.contains(aLoc) && this.lensPlane.contains(aLoc));
			}
			
		public override Intersection intersects(Scientrace.Trace aTrace) {
			// Finding intersectionpoints at sphere
			Scientrace.Intersection sphereIntersections = this.dummySphere.intersects(aTrace);
			/* If the PlanoConvexLens doesn't even pass the surface of the sphere, the intersection 
			   of the lens does not exist. */
			if (!sphereIntersections.intersects) {
				//return sphereIntersections; 
				return Intersection.notIntersect(this);
				}
			
			Scientrace.Location planoLoc = this.lensPlane.lineThroughPlane(aTrace.traceline);

			Scientrace.IntersectionPoint planoIP;
			if ((!this.dummySphere.contains(planoLoc)) || (planoLoc == null)) {
				planoIP = null;
				} else {
				planoIP = new Scientrace.IntersectionPoint(planoLoc, this.lensPlane.planeToFlatShape2d());
				}
						
			Scientrace.IntersectionPoint[] ips = new Scientrace.IntersectionPoint[3];
			ips[0] = planoIP;
			ips[1] = this.lensPlane.filterOutsideBorder(sphereIntersections.enter);
			ips[2] = this.lensPlane.filterOutsideBorder(sphereIntersections.exit);
			Scientrace.Intersection lensIntersection = new Scientrace.Intersection(aTrace, ips, this);

			// when currently inside the lens, intersection from here must mean leaving.
			lensIntersection.leaving = this.contains(aTrace.traceline.startingpoint);
			//return created new intersection
			return lensIntersection;
			}

	public Scientrace.Location planoCenterLoc() {
		Scientrace.Line sphereLine = new Scientrace.Line(this.sphereCenterLoc, this.lensPlane.getNormal());
		return this.lensPlane.intersectsAt(sphereLine);	
		}
		
	public double distanceFromSphereCenterToPlano() {
		Scientrace.Location h1 = this.planoCenterLoc();
		return h1.distanceTo(this.sphereCenterLoc);
		}

	public double getLensRadius() {
		double r = this.sphereRadius;
		double y = this.distanceFromSphereCenterToPlano();
		double x = Math.Sqrt((r*r)-(y*y));
		return x;
		}

	public Scientrace.Location constructPlaneNodeLoc(Scientrace.Location planeCenter, double radius,
						double angle, Scientrace.NonzeroVector baseVec1, Scientrace.NonzeroVector baseVec2) {
		Scientrace.Vector ix = baseVec1.toVector()*radius*Math.Sin(angle);
		Scientrace.Vector iy = baseVec2.toVector()*radius*Math.Cos(angle);
		return planeCenter+ix+iy;
		}

	/// theta -> 0-pi, phi -> 0-2pi
	public Scientrace.Location constructSphereNodeLoc(double theta, double phi,
						Scientrace.NonzeroVector baseVec1, Scientrace.NonzeroVector baseVec2,
						Scientrace.NonzeroVector planeNormal) {
						
		return this.dummySphere.getSphericalLoc(baseVec1, baseVec2, planeNormal, theta, phi);
		}

	public override string exportX3D(Scientrace.Object3dEnvironment env) {
		Scientrace.Location h1 = this.planoCenterLoc();
		double r = this.sphereRadius;
		double x = this.getLensRadius();
		double lensRadians = Math.Asin(x/r);

		NonzeroVector baseVecZ = this.lensPlane.getNormal();
		NonzeroVector baseVec1 = null;
		NonzeroVector baseVec2 = null;
		baseVecZ.fillOrtogonalVectors(ref baseVec1, ref baseVec2);
						
		double lat_circles = 3;
		double meridians = 12;
		
		System.Text.StringBuilder retx3d = new System.Text.StringBuilder("<!-- PLANOCONVEXLENS GRID start -->", 1024);
		retx3d.Append("\t<!-- Plano part -->" );


		Scientrace.Location tNodeLoc;
		Scientrace.Location tMerConnectLoc;
		Scientrace.Location tLatConnectLoc;
		Scientrace.X3DGridPoint tGridPoint;				
		double pi2 = Math.PI*2;
		
		for (double iPlaneCircle = lat_circles; iPlaneCircle > 0; iPlaneCircle--) {		
			for (double iPlaneMerid = 0; iPlaneMerid < meridians; iPlaneMerid++) {
				tNodeLoc = this.constructPlaneNodeLoc(h1, 
							x*iPlaneCircle/lat_circles, pi2*iPlaneMerid/meridians, 
							baseVec1, baseVec2);
				tMerConnectLoc = this.constructPlaneNodeLoc(h1, 
							x*iPlaneCircle/lat_circles, pi2*(iPlaneMerid+1)/meridians, 
							baseVec1, baseVec2);
				tLatConnectLoc = this.constructPlaneNodeLoc(h1, 
							x*(iPlaneCircle-1)/lat_circles, pi2*iPlaneMerid/meridians, 
							baseVec1, baseVec2);
				tGridPoint = new Scientrace.X3DGridPoint(env, tNodeLoc, tMerConnectLoc, tLatConnectLoc);
				retx3d.AppendLine(tGridPoint.exportX3DnosphereRGBA("0.4 0 0.2 1"));
				}} //end for iPlaneCircles / iPlaneMerid

		retx3d.Append("\t<!-- End of plano part -->" );
		retx3d.Append("\t<!-- Convex part -->" );
				
		for (double iSphereCircle = 2*lat_circles; iSphereCircle > 0; iSphereCircle--) {		
			for (double iSphereMerid = 0.5; iSphereMerid < 2*meridians; iSphereMerid++) {
				//double lat_angle = lensRadians * (iSphereCircle / lat_circles); // theta
				//double mer_angle = pi2 * (iSphereMerid/meridians); // mer_angle = phi
				tNodeLoc = this.constructSphereNodeLoc(
						lensRadians * (iSphereCircle / (2*lat_circles)), // lat_angle = theta
						pi2 * (iSphereMerid/(2*meridians)), // mer_angle = phi
						baseVec1, baseVec2, baseVecZ);
				tMerConnectLoc = this.constructSphereNodeLoc(
						lensRadians * (iSphereCircle / (2*lat_circles)), // lat_angle = theta
						pi2 * ((iSphereMerid+1)/(2*meridians)), // mer_angle = phi
						baseVec1, baseVec2, baseVecZ);
				tLatConnectLoc = this.constructSphereNodeLoc(
						lensRadians * ((iSphereCircle-1) / (2*lat_circles)), // lat_angle = theta
						pi2 * (iSphereMerid/(2*meridians)), // mer_angle = phi
						baseVec1, baseVec2, baseVecZ);
				tGridPoint = new Scientrace.X3DGridPoint(env, tNodeLoc, tMerConnectLoc, tLatConnectLoc);
				retx3d.AppendLine(tGridPoint.exportX3DnosphereRGBA("0.2 0 0.4 1"));
				}} // end for iSphereCircle / iSphereMerid

																														retx3d.Append("\t<!-- Convex part -->" );
		retx3d.Append("\t<!-- End of Convex part -->" );																												
		retx3d.Append("<!-- End of PLANOCONVEXLENS GRID -->");
		return retx3d.ToString();
		

		} //end string exportX3D(env)
		
			
	}}
