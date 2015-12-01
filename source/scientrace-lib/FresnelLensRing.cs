// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Text;


namespace Scientrace {
public class FresnelLensRing : EnclosedVolume, IBorder3d {

	public Location sphereLoc;
	public double sphereRadius;
	public double radiansMin;
	public double radiansMax;
	public UnitVector orientation;
	
	// The three "IBorder3d" instances below describe the space within ring
	public InfiniteCylinderBorder innerVoid;
	public IInterSectableBorder3d flatBottomBorder;
	public Sphere lensSphere;

	public Sphere oppositeSideSphere = null;
	public bool doubleConvexRing;
	
	/// <summary>
	/// When exporting the ring, the curved surface can be "portrayed" by drawing several rings upohn this surface.
	/// When x3dCurvedSegments == 1 (min value) just the top and the bottom is drawn. When x3dCurvedSegments == 2, 
	/// there is one line dividing the ring in two smaller rings, etc. etc.
	/// </summary>
	public int x3dCurvedSegments = 2;

/*
	public FresnelLensRing(Object3dCollection parent, MaterialProperties mprops,
			Scientrace.Location lens_sphere_location, double lens_sphere_radius, 
			double lens_sphere_radians_min, double lens_sphere_radians_max,
			Scientrace.UnitVector orientation_from_sphere_center) : base (parent, mprops) {
		this.paramInit(lens_sphere_location, lens_sphere_radius, lens_sphere_radians_min, lens_sphere_radians_max, orientation_from_sphere_center);
		}

	/// <summary>
	/// Factory method that creates a new FresnelLensRing based on the properties of another FresnelLensRing
	/// but mirrored about the flat plane (flatBottomBorder).
	/// </summary>
	/// <returns>
	/// The new FresnelLensRing
	/// </returns>
	/// <param name='aFresnelLensRing'>
	/// A FresnelLensRing to base the (copied) properties upon.
	/// </param>
	public static FresnelLensRing newOppositeDirectionRing(FresnelLensRing aFresnelLensRing) {
		Object3dCollection parent = aFresnelLensRing.parent;
		MaterialProperties mprops = aFresnelLensRing.materialproperties;
		Scientrace.Location lens_sphere_location = aFresnelLensRing.sphereLoc + (aFresnelLensRing.orientation*aFresnelLensRing.getDistanceToPlanoCenter()*2);
		double lens_sphere_radius = aFresnelLensRing.sphereRadius;
		double lens_sphere_radians_min = aFresnelLensRing.radiansMin;
		double lens_sphere_radians_max = aFresnelLensRing.radiansMax;
		Scientrace.UnitVector orientation_from_sphere_center = aFresnelLensRing.orientation.negative();
		return new FresnelLensRing(parent, mprops, lens_sphere_location, lens_sphere_radius, lens_sphere_radians_min, lens_sphere_radians_max, orientation_from_sphere_center);
		} */
		
	public FresnelLensRing(ShadowScientrace.ShadowObject3d shadowObject): base(shadowObject) {
		switch (shadowObject.factory_id) {
			case "SphereCenterAndRadians": this.shadowFac_SphereCenter_And_Radians(shadowObject);
				break;
			case "PlanoCenterAndRadians": this.shadowFac_PlanoCenter_And_Radians(shadowObject);
				break;
			default:
				throw new ArgumentOutOfRangeException("Factory method {"+shadowObject.factory_id+"} not found for "+shadowObject.typeString());
			}

		//General stuff:
		this.x3dCurvedSegments = shadowObject.getInt("draw_3d_segment_linecount", this.x3dCurvedSegments);
		}				
		
	public void paramInit(
			Scientrace.Location lens_sphere_location, double lens_sphere_radius, 
			double lens_sphere_radians_min, double lens_sphere_radians_max,
			Scientrace.UnitVector orientation_from_sphere_center, bool double_convex_ring) {
			
		this.sphereLoc = lens_sphere_location;
		this.sphereRadius = lens_sphere_radius;
		//Console.WriteLine("Sphere radius: "+sphereRadius);
		this.radiansMin = lens_sphere_radians_min;
		this.radiansMax = lens_sphere_radians_max;
		this.orientation = orientation_from_sphere_center;
		this.doubleConvexRing = double_convex_ring;
		
		if (lens_sphere_radians_min > 0) {
			this.innerVoid = new InfiniteCylinderBorder(null, null, 
							lens_sphere_location, orientation_from_sphere_center, 
							lens_sphere_radius*Math.Sin(lens_sphere_radians_min)
							);
			this.innerVoid.enclosesInside = false;
			}
		
		this.lensSphere = new Sphere(null, null, lens_sphere_location, lens_sphere_radius);
		this.initBottom();
		}

	public IInterSectableBorder3d initBottom() {

		double planeDistanceFromSphereCenter = getDistanceFromSphereCenterToAngle(this.radiansMax);
		Location planoLoc = this.sphereLoc+(this.orientation*planeDistanceFromSphereCenter);
		
		if (this.doubleConvexRing) {
			this.oppositeSideSphere = new Sphere(null, null, this.sphereLoc+(this.orientation*planeDistanceFromSphereCenter*2), this.sphereRadius);
			return this.oppositeSideSphere;
			} else {
			this.flatBottomBorder = new PlaneBorder(planoLoc, this.orientation);
			return this.flatBottomBorder;
			}
		}
		
	public IInterSectableBorder3d getBottomBorder() {
		return this.doubleConvexRing?this.oppositeSideSphere:this.flatBottomBorder;
		}

	/// <summary>
	/// Required parameters:
	/// (Location) lens_plano_center, 
	/// (double) lens_sphere_radius
	/// (double) lens_sphere_radians_min
	/// (double) lens_sphere_radians_max
	/// (UnitVector) orientation_from_sphere_center	
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>
	protected void shadowFac_PlanoCenter_And_Radians(ShadowScientrace.ShadowObject3d shadowObject) {
		//User values(s)
		Scientrace.Location lens_plano_center = (Scientrace.Location)shadowObject.getObject("lens_plano_center");
		double lens_sphere_radius = (double)shadowObject.getObject("lens_sphere_radius");
		double lens_sphere_radians_min = (double)shadowObject.getObject("lens_sphere_radians_min");
		double lens_sphere_radians_max = (double)shadowObject.getObject("lens_sphere_radians_max");
		Scientrace.UnitVector orientation_from_sphere_center = (Scientrace.UnitVector)shadowObject.getObject("orientation_from_sphere_center");
		bool double_convex_ring = shadowObject.getNBool("double_convex")==true;

		//Derived value(s)
		Scientrace.Location lens_sphere_location = lens_plano_center -
					(orientation_from_sphere_center*lens_sphere_radius*Math.Cos(lens_sphere_radians_max));

		/*double rmin = Math.Sin(lens_sphere_radians_min)*lens_sphere_radius;
		double rmax = Math.Sin(lens_sphere_radians_max)*lens_sphere_radius;
		Console.WriteLine("Ring got radius "+lens_sphere_radius+" at"+lens_sphere_location+" radmin/max:{"+lens_sphere_radians_min+"}/{"+lens_sphere_radians_max+"}.");
		Console.WriteLine("Ring got rmin/rmax "+rmin+" / "+rmax); */

						
		//Console.WriteLine("Lens sphere: "+lens_sphere_location.tricon()+" Plano center: "+lens_plano_center.trico());
		//construct!
		this.paramInit(lens_sphere_location, lens_sphere_radius, lens_sphere_radians_min, lens_sphere_radians_max, 
						orientation_from_sphere_center, double_convex_ring);
		}

	/// <summary>
	/// Required parameters:
	/// (Location) lens_sphere_location, 
	/// (double) lens_sphere_radius
	/// (double) lens_sphere_radians_min
	/// (double) lens_sphere_radians_max
	/// (UnitVector) orientation_from_sphere_center	
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>
	protected void shadowFac_SphereCenter_And_Radians(ShadowScientrace.ShadowObject3d shadowObject) {
		//User values(s)
		Scientrace.Location lens_sphere_location = (Scientrace.Location)shadowObject.getObject("lens_sphere_location");
		double lens_sphere_radius = (double)shadowObject.getObject("lens_sphere_radius");
		double lens_sphere_radians_min = (double)shadowObject.getObject("lens_sphere_radians_min");
		double lens_sphere_radians_max = (double)shadowObject.getObject("lens_sphere_radians_max");
		Scientrace.UnitVector orientation_from_sphere_center = (Scientrace.UnitVector)shadowObject.getObject("orientation_from_sphere_center");		
		bool double_convex_ring = shadowObject.getNBool("double_convex")==true;
		//construct!
		this.paramInit(lens_sphere_location, lens_sphere_radius, lens_sphere_radians_min, 
						lens_sphere_radians_max, orientation_from_sphere_center, double_convex_ring);
		}
				

	public static double RadiansForRadii(double sphereRadius, double ringRadius) {
		return Math.Asin(ringRadius/sphereRadius);
		}
		
	public double getDistanceFromSphereCenterToAngle(double radians) {
		return this.sphereRadius * (Math.Cos(radians));
		}

	public Location getCenterAtAngleLoc(double radians) {
		return this.sphereLoc+(this.orientation.toVector()*this.getDistanceFromSphereCenterToAngle(radians));
		}

	public Location getOppositeCenterAtAngleLoc(double radians) {
		return this.oppositeSideSphere.loc+(this.orientation.negative().toVector()*this.getDistanceFromSphereCenterToAngle(radians));
		}


	public double getRadiusAtAngle(double radians) {
		return this.sphereRadius * (Math.Sin(radians));
		}
				
	public double getDistanceToPlanoCenter() {
		return this.getDistanceFromSphereCenterToAngle(this.radiansMax);
		//return this.sphereRadius * (1-Math.Cos(this.radiansMax));
		}	
	public double getDistanceToConvexTopCenter() {
		return this.getDistanceFromSphereCenterToAngle(this.radiansMin);
		//return this.sphereRadius * (1-Math.Cos(this.radiansMin));
		}	
	
	public Location getRingPlanoCenter() {
		return this.sphereLoc+(this.orientation*this.getDistanceToPlanoCenter());
		}
			
	public Location getRingConvexTopCenter() {
		return this.sphereLoc+(this.orientation*this.getDistanceToConvexTopCenter());
		}
	
	public bool bordersContain(Scientrace.Location aLocation) {
		if (!this.getBottomBorder().contains(aLocation, 1E-14))
			return false;
		if (this.innerVoid == null)
			return true;
		return (this.innerVoid.contains(aLocation));
		}
	
	/* interface IBorder3D implementation */
	/// <summary>
	/// Return true when the specified aLocation is contained by the Sphere AND the infinite cylinder.
	/// </summary>
	/// <param name='aLocation'>
	/// The location to check to be within the borders
	/// </param>
	public bool contains(Scientrace.Location aLocation) {
		return this.lensSphere.contains(aLocation, 1E-14)&&
			this.bordersContain(aLocation);
		}
	/* end of interface IBorder3D implementation */
		
	public override string exportX3D(Object3dEnvironment env) {
		StringBuilder retsb = new StringBuilder(1024);
		X3DShapeDrawer x3dsd = new X3DShapeDrawer();
		for (double angle = this.radiansMin; angle <= this.radiansMax; angle += (this.radiansMax-this.radiansMin)/this.x3dCurvedSegments) {
			double red = (((angle-this.radiansMin)/(this.radiansMax-this.radiansMin))/2)+0.3;
			x3dsd.primaryRGB = red.ToString()+" 0.2 0.2";
			//Console.WriteLine("radmin: "+this.radiansMin+ "radmax:"+this.radiansMax+ " iangle:"+angle);
			//if (angle > 0) {
			retsb.AppendLine(
					x3dsd.drawCircle(this.getCenterAtAngleLoc(angle), this.getRadiusAtAngle(angle), this.orientation));
			if (this.doubleConvexRing)
				retsb.AppendLine(
					x3dsd.drawCircle(this.getOppositeCenterAtAngleLoc(angle), this.getRadiusAtAngle(angle), this.orientation));
			//	}
			}
		if (this.radiansMin > 0 && this.x3dCurvedSegments > 0) 
			retsb.AppendLine(x3dsd.drawCircle(this.getCenterAtAngleLoc(this.radiansMax), this.getRadiusAtAngle(this.radiansMin), this.orientation));
		return retsb.ToString();
		}
		
	public override string ToString() {
		return "FresnelLensRing properties: \n:sphereLoc{"+sphereLoc.ToString()+"}, sphereRadius{"+sphereRadius.ToString()+"}, radiansMin{"+radiansMin.ToString()+"}, radiansMax{"+radiansMax.ToString()+"}, orientation{"+orientation.ToString()+"}";
		}

	public override Intersection intersects(Trace trace) {
		//Console.WriteLine("lenssphere:"+this.getBottomBorder());
		Intersection flatBottomAndSphereIntersection = 
			 this.lensSphere.intersects(trace).mergeToNewIntersectionWithinBorder(
					this.getBottomBorder().intersectsObject(trace, this),
					this, trace, this);
		
		if (this.innerVoid == null) 
			return flatBottomAndSphereIntersection;
		return flatBottomAndSphereIntersection.mergeToNewIntersectionWithinBorder(
				this.innerVoid.intersects(trace),
				this, trace, this);
				
	/*	return (this.lensSphere.intersects(trace)
				&&
				this.innerVoid.intersects(trace)
				);*/
		}
	}
}

