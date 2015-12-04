// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public class FresnelLens : Object3dCollection {

	public FresnelLens(ShadowScientrace.ShadowObject3d lensShadowObject): base(lensShadowObject) {
	
		ShadowScientrace.ShadowObject3d ringTemplateSO3D = new ShadowScientrace.ShadowObject3d(lensShadowObject);
		ringTemplateSO3D.materialprops = (Scientrace.MaterialProperties)ringTemplateSO3D.getObject("lens_material");


		if (ringTemplateSO3D.hasArgument("FocalVector")) {
			
			}


		//Setting Sphere Radius (if not provided) from Focal Length
		if (ringTemplateSO3D.hasArgument("focal_length") && !ringTemplateSO3D.hasArgument("lens_sphere_radius")) {
			double focal_length = (double)ringTemplateSO3D.getObject("focal_length");

			double sphere_radius = FresnelLens.getRadius(focal_length, ringTemplateSO3D.materialprops.refractiveindex(600E-9), 
										ringTemplateSO3D.getNBool("double_convex"));
			ringTemplateSO3D.arguments["lens_sphere_radius"] = sphere_radius;
			}

		//Setting Sphere Radius (if not provided) from Focal Length
		if (ringTemplateSO3D.hasArgument("lens_radius") 
				&& ringTemplateSO3D.hasArgument("lens_sphere_radius") 
				&&!ringTemplateSO3D.hasArgument("lens_sphere_radians_max")) {
			double lens_radius = (double)ringTemplateSO3D.getObject("lens_radius");
			double sphere_radius  = (double)ringTemplateSO3D.getObject("lens_sphere_radius");

			// VarRings can have larger radii. Will throw an error upon calculating the ring sphere radius later on if necessary.
			if ((ringTemplateSO3D.factory_id != "VarRings") && (sphere_radius < lens_radius))
				throw new ArgumentOutOfRangeException("Cannot create a lens with radius "+lens_radius+" from sphere with radius: "+sphere_radius+
					" possibly constructed from focal length: "+ringTemplateSO3D.printArgument("focal_length")+"{"+ringTemplateSO3D.factory_id);
			
			double lens_sphere_radians_max = Math.Asin(lens_radius/sphere_radius);
			//Console.WriteLine("sphere radius: "+sphere_radius+"  lens radius: "+lens_radius);
			ringTemplateSO3D.arguments["lens_sphere_radians_min"] = 0.0;
			ringTemplateSO3D.arguments["lens_sphere_radians_max"] = lens_sphere_radians_max;
			}

		switch (ringTemplateSO3D.factory_id) {
			case "EqualHeightRings": 
				this.shadowFac_SphereRadius_and_EqualHeightRings(ringTemplateSO3D);
				break;
			case "EqualWidthRings": 
				this.shadowFac_SphereRadius_and_EqualWidthRings(ringTemplateSO3D);
				break;
			case "VarRings": 
				this.shadowFac_VariusRadii_and_EqualWidthRings(ringTemplateSO3D);
				break;
			case "EqualAngleRings": 
				this.shadowFac_SphereRadius_and_EqualAngleRings(ringTemplateSO3D);
				break;
			default:
				throw new ArgumentOutOfRangeException("Factory id {"+ringTemplateSO3D.factory_id+"} not found for "+ringTemplateSO3D.typeString());
			}

		}

/*
	//Focal length conversion shadow factory methods:
	protected void shadowFac_FocalLength_and_EqualHeightRings(ShadowScientrace.ShadowObject3d shadowObject) {
		double focal_length = (double)shadowObject.getObject("focal_length");
		double sphere_radius = FresnelLens.getRadius(focal_length, shadowObject.materialprops.refractiveindex(600E-9));
		shadowObject.arguments.Add("lens_sphere_radius", sphere_radius);
		this.shadowFac_SphereRadius_and_EqualHeightRings(shadowObject);
		}
	protected void shadowFac_FocalLength_and_EqualWidthRings(ShadowScientrace.ShadowObject3d shadowObject) {
		double focal_length = (double)shadowObject.getObject("focal_length");
		double sphere_radius = FresnelLens.getRadius(focal_length, shadowObject.materialprops.refractiveindex(600E-9));
		shadowObject.arguments.Add("lens_sphere_radius", sphere_radius);
		this.shadowFac_SphereRadius_and_EqualWidthRings(shadowObject);
		}
	protected void shadowFac_FocalLength_and_EqualAngleRings(ShadowScientrace.ShadowObject3d shadowObject) {
		double focal_length = (double)shadowObject.getObject("focal_length");
		double sphere_radius = FresnelLens.getRadius(focal_length, shadowObject.materialprops.refractiveindex(600E-9));
		shadowObject.arguments.Add("lens_sphere_radius", sphere_radius);
		this.shadowFac_SphereRadius_and_EqualAngleRings(shadowObject);
		}
*/


	/// <summary>
	/// Required parameters:
	/// (Location) lens_plano_center, 
	/// (double) lens_sphere_radius
	/// (double) lens_sphere_radians_min
	/// (double) lens_sphere_radians_max
	/// (double) height_rings   double value instead of int, so the ring may end in a ring-fraction
	/// (UnitVector) orientation_from_sphere_center	
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>
	protected void shadowFac_SphereRadius_and_EqualHeightRings(ShadowScientrace.ShadowObject3d shadowObject) {
		//Console.WriteLine("CREATING EQUAL HEIGHT RINGS");
		//User values(s)
		//NOT USED: Scientrace.Location lens_plano_center = (Scientrace.Location)shadowObject.getObject("lens_plano_center");
		//NOT USED: double lens_sphere_radius = (double)shadowObject.getObject("lens_sphere_radius");
		double lens_sphere_radians_min = (double)shadowObject.getObject("lens_sphere_radians_min");
		double lens_sphere_radians_max = (double)shadowObject.getObject("lens_sphere_radians_max");
		//NOT USED: Scientrace.UnitVector orientation_from_sphere_center = (Scientrace.UnitVector)shadowObject.getObject("orientation_from_sphere_center");

		double height_rings = (double)shadowObject.getObject("height_rings_count");
		double ringcount = height_rings;
		//construct by creating FresnelLensRing Object3D instances and adding them to collection
		for (double iring = 0; iring <= ringcount - 1; iring += 1) {
			ShadowScientrace.ShadowObject3d so3d = new ShadowScientrace.ShadowObject3d(shadowObject);
			so3d.factory_id = "PlanoCenterAndRadians";
			double dcos = Math.Cos(lens_sphere_radians_max)-Math.Cos(lens_sphere_radians_min);
			so3d.arguments["lens_sphere_radians_min"] = Math.Acos((dcos*(iring/ringcount))+Math.Cos(lens_sphere_radians_min));
			so3d.arguments["lens_sphere_radians_max"] = Math.Acos((dcos*Math.Min((iring+1)/ringcount,1))+Math.Cos(lens_sphere_radians_min));

			so3d.parent = this;
			new FresnelLensRing(so3d);
			}
		}







	/// <summary>
	/// Required parameters:
	/// (Location) lens_plano_center, 
	/// (double) lens_radius
	/// (double) var_rings_count   double value instead of int, so the ring may end in a ring-fraction
	/// (NonzeroVector) focal_vector
	/// (double) refractive_index derived from SO3D at a given wavelength (600E-9 by default)
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>
	protected void shadowFac_VariusRadii_and_EqualWidthRings(ShadowScientrace.ShadowObject3d ringTemplateSO3D) {
		//Console.WriteLine("CREATING EQUAL WIDTH RINGS");
		//User values(s)
		//NOT USED: Scientrace.Location lens_plano_center = (Scientrace.Location)shadowObject.getObject("lens_plano_center");

		//NOT USED: double lens_sphere_radius = (double)shadowObject.getObject("lens_sphere_radius");

		//NOT USED: Scientrace.UnitVector orientation_from_sphere_center = (Scientrace.UnitVector)shadowObject.getObject("orientation_from_sphere_center");

		double radius = ringTemplateSO3D.getDouble("lens_radius");

		double ring_count = ringTemplateSO3D.getDouble("var_rings_count");
		//construct by creating FresnelLensRing Object3D instances and adding them to collection

		double ring_width = radius/ring_count;

		double focal_length = ringTemplateSO3D.getDouble("focal_length");

		double focus_wavelength = ringTemplateSO3D.getDouble("focus_wavelength", 600E-9);
		double refindex = ringTemplateSO3D.materialprops.refractiveindex(focus_wavelength);

		for (double iring = 0; iring <= ring_count - 1; iring += 1) {
			double rmin = iring*ring_width;
			double rmax = Math.Min(iring+1, ring_count)*ring_width;
			double ravg = (rmin+rmax) / 2;

						double ring_sphere_radius = FresnelLens.GetOptimalRingSphereRadius(focal_length, ravg, refindex, rmax);
			//Console.WriteLine("Radius "+iring+ ": {"+ring_sphere_radius+"}, rmin: {"+rmin+"}, rmax: {"+rmax+"}, xy_ratio: {"+xy_ratio+"}");

			ShadowScientrace.ShadowObject3d so3d = new ShadowScientrace.ShadowObject3d(ringTemplateSO3D);
			so3d.factory_id = "PlanoCenterAndRadians";
			so3d.arguments["lens_sphere_radians_min"] = FresnelLensRing.RadiansForRadii(ring_sphere_radius, rmin);
			so3d.arguments["lens_sphere_radians_max"] = FresnelLensRing.RadiansForRadii(ring_sphere_radius, rmax);
			so3d.arguments["lens_sphere_radius"] = ring_sphere_radius;
			so3d.parent = this;
			new FresnelLensRing(so3d);
			}
		}		


		public static double GetOptimalRingSphereRadius(double focal_length, double ring_radius, double refractive_index, double ring_max_radius) {
			double l = Math.Sqrt((ring_radius*ring_radius)+(focal_length*focal_length));

			double xy_ratio = ((refractive_index*l)-focal_length)/ring_radius;
			/* double sx =   xy_ratio 	/ Math.Sqrt(Math.Pow(xy_ratio,2) + 1);
			 * double sy =		  1 	/ Math.Sqrt(Math.Pow(xy_ratio,2) + 1); */

			double sphere_cos = ring_radius*xy_ratio;
			double ring_sphere_radius = Math.Sqrt((sphere_cos*sphere_cos)+(ring_radius*ring_radius));

			

			/*Console.WriteLine("Attempting to create a lens(ring) up to radius {"+ring_max_radius+"}. Calculated ring sphere has radius: {"+ring_sphere_radius+
					"} At ring radius:{"+ring_radius+"} and focal distance: {"+focal_length+"}");*/

			if (ring_max_radius > focal_length * xy_ratio)
				throw new ArgumentOutOfRangeException("Cannot create a lens(ring) up to radius {"+ring_max_radius+"}. Max radius is: {"+(focal_length * xy_ratio)+
					"} At ring radius:{"+ring_radius+"} and focal distance: {"+focal_length+"}\n\nSUGGESTED SOLUTION: modify your FresnelLens such that it has a SMALLER RADIUS, or a LARGER FOCAL DISTANCE. [good luck]\n\n");
			return ring_sphere_radius;
			}










	/// <summary>
	/// Required parameters:
	/// (Location) lens_plano_center, 
	/// (double) lens_sphere_radius
	/// (double) lens_sphere_radians_min
	/// (double) lens_sphere_radians_max
	/// (double) width_rings   double value instead of int, so the ring may end in a ring-fraction
	/// (UnitVector) orientation_from_sphere_center	
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>
	protected void shadowFac_SphereRadius_and_EqualWidthRings(ShadowScientrace.ShadowObject3d shadowObject) {
		//Console.WriteLine("CREATING EQUAL WIDTH RINGS");
		//User values(s)
		//NOT USED: Scientrace.Location lens_plano_center = (Scientrace.Location)shadowObject.getObject("lens_plano_center");
		//NOT USED: double lens_sphere_radius = (double)shadowObject.getObject("lens_sphere_radius");
		double lens_sphere_radians_min = (double)shadowObject.getObject("lens_sphere_radians_min");
		double lens_sphere_radians_max = (double)shadowObject.getObject("lens_sphere_radians_max");
		//NOT USED: Scientrace.UnitVector orientation_from_sphere_center = (Scientrace.UnitVector)shadowObject.getObject("orientation_from_sphere_center");

		double width_rings = (double)shadowObject.getObject("width_rings_count");
		double ringcount = width_rings;
		//construct by creating FresnelLensRing Object3D instances and adding them to collection
		for (double iring = 0; iring <= ringcount - 1; iring += 1) {

			ShadowScientrace.ShadowObject3d so3d = new ShadowScientrace.ShadowObject3d(shadowObject);

			//Console.WriteLine("Radius "+iring+ ": {"+so3d.arguments["lens_sphere_radius"]+"}");

			so3d.factory_id = "PlanoCenterAndRadians";
			double dsin = Math.Sin(lens_sphere_radians_max)-Math.Sin(lens_sphere_radians_min);
			so3d.arguments["lens_sphere_radians_min"] = Math.Asin((dsin*(iring/ringcount))+Math.Sin(lens_sphere_radians_min));
			so3d.arguments["lens_sphere_radians_max"] = Math.Asin((dsin*Math.Min((iring+1)/ringcount,1))+Math.Sin(lens_sphere_radians_min));
			so3d.parent = this;
			new FresnelLensRing(so3d);
			}
		}	


	/// <summary>
	/// Required parameters:
	/// (Location) lens_plano_center, 
	/// (double) lens_sphere_radius
	/// (double) lens_sphere_radians_min
	/// (double) lens_sphere_radians_max
	/// (double) angle_rings   double value instead of int, so the ring may end in a ring-fraction
	/// (UnitVector) orientation_from_sphere_center	
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>
	protected void shadowFac_SphereRadius_and_EqualAngleRings(ShadowScientrace.ShadowObject3d shadowObject) {
		//User values(s)
		//NOT USED: Scientrace.Location lens_plano_center = (Scientrace.Location)shadowObject.getObject("lens_plano_center");
		//NOT USED: double lens_sphere_radius = (double)shadowObject.getObject("lens_sphere_radius");
		double lens_sphere_radians_min = (double)shadowObject.getObject("lens_sphere_radians_min");
		double lens_sphere_radians_max = (double)shadowObject.getObject("lens_sphere_radians_max");
		//NOT USED: Scientrace.UnitVector orientation_from_sphere_center = (Scientrace.UnitVector)shadowObject.getObject("orientation_from_sphere_center");

		double angle_rings = (double)shadowObject.getObject("angle_rings_count");
		double ringcount = angle_rings;


		//construct by creating FresnelLensRing Object3D instances and adding them to collection
		for (double iring = 0; iring <= ringcount - 1; iring += 1) {
			ShadowScientrace.ShadowObject3d so3d = new ShadowScientrace.ShadowObject3d(shadowObject);
			so3d.factory_id = "PlanoCenterAndRadians";
			double dradians = lens_sphere_radians_max-lens_sphere_radians_min;

			so3d.arguments["lens_sphere_radians_min"] = (dradians*(iring/ringcount))+lens_sphere_radians_min;
			so3d.arguments["lens_sphere_radians_max"] = (dradians*(Math.Min((iring+1)/ringcount,1)))+lens_sphere_radians_min;
			
			so3d.parent = this;
			new FresnelLensRing(so3d);
			}
		}
	
	public static double getRadius(double focal_length, double refractive_index, bool? isDoubleConvex) {
		double retval = 0;
		//Derived value from 1/f = (n-1)(1/r1+1/r2)
		if (isDoubleConvex == true) {
			retval = 2*focal_length*(refractive_index-1);
			//Console.WriteLine("Sphere (double) radius is: "+retval);
			//double convex lens, two curved surfaces, r1=r2
			return retval;
			}
	
		// 1/f = (n - 1) / r
		// r = f*n - f
		//plano convex lens, a single curved surface, r1 = r, r2 = infinite (a flat surface)
		retval = ((refractive_index - 1) * focal_length);
		//Console.WriteLine("Sphere (single) radius is: "+retval);
		return retval;
		}
		
	}
}

