// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace ShadowScientrace {
public enum fresnelLensFactoryMethod {
		EqualHeightRings = 1,
		EqualWidthRings = 2,
		EqualAngleRings = 3,
	/*	FocalLength_and_EqualHeightRings = 4,
		FocalLength_and_EqualWidthRings = 5,
		FocalLength_and_EqualAngleRings = 6,*/
		}}



namespace Scientrace {
public class FresnelLens : Object3dCollection {

	public FresnelLens(ShadowScientrace.ShadowObject3d lensShadowObject): base(lensShadowObject) {
	
		ShadowScientrace.ShadowObject3d ringTemplateSO3D = new ShadowScientrace.ShadowObject3d(lensShadowObject);
		ringTemplateSO3D.materialprops = (Scientrace.MaterialProperties)ringTemplateSO3D.getObject("lens_material");

		//Setting Sphere Radius (if not provided) from Focal Length
		if (ringTemplateSO3D.hasArgument("focal_length") && !ringTemplateSO3D.hasArgument("lens_sphere_radius")) {
			double focal_length = (double)ringTemplateSO3D.getObject("focal_length");
			double sphere_radius = FresnelLens.getRadius(focal_length, ringTemplateSO3D.materialprops.refractiveindex(600E-9), 
										ringTemplateSO3D.getNBool("double_convex"));
			ringTemplateSO3D.arguments["lens_sphere_radius"] = sphere_radius;
			}

		//Setting Sphere Radius (if not provided) from Focal Length
		if (ringTemplateSO3D.hasArgument("lens_radius") && ringTemplateSO3D.hasArgument("lens_sphere_radius") &&!ringTemplateSO3D.hasArgument("lens_sphere_radians_max")) {
			double lens_radius = (double)ringTemplateSO3D.getObject("lens_radius");
			double sphere_radius  = (double)ringTemplateSO3D.getObject("lens_sphere_radius");

			if (sphere_radius < lens_radius)
				throw new ArgumentOutOfRangeException("Cannot create a lens with radius "+lens_radius+" from sphere with radius: "+sphere_radius+
					" possibly constructed from focal length: "+ringTemplateSO3D.printArgument("focal_length"));
			
			double lens_sphere_radians_max = Math.Asin(lens_radius/sphere_radius);
			//Console.WriteLine("sphere radius: "+sphere_radius+"  lens radius: "+lens_radius);
			ringTemplateSO3D.arguments["lens_sphere_radians_min"] = 0.0;
			ringTemplateSO3D.arguments["lens_sphere_radians_max"] = lens_sphere_radians_max;
			}

		switch (ringTemplateSO3D.factory_method) {
			case (int)ShadowScientrace.fresnelLensFactoryMethod.EqualHeightRings: this.shadowFac_SphereRadius_and_EqualHeightRings(ringTemplateSO3D);
				break;
			case (int)ShadowScientrace.fresnelLensFactoryMethod.EqualWidthRings: this.shadowFac_SphereRadius_and_EqualWidthRings(ringTemplateSO3D);
				break;
			case (int)ShadowScientrace.fresnelLensFactoryMethod.EqualAngleRings: this.shadowFac_SphereRadius_and_EqualAngleRings(ringTemplateSO3D);
				break;
/*			case (int)ShadowScientrace.fresnelLensFactoryMethod.FocalLength_and_EqualHeightRings: this.shadowFac_FocalLength_and_EqualHeightRings(shadowObject);
				break;
			case (int)ShadowScientrace.fresnelLensFactoryMethod.FocalLength_and_EqualWidthRings: this.shadowFac_FocalLength_and_EqualWidthRings(shadowObject);
				break;
			case (int)ShadowScientrace.fresnelLensFactoryMethod.FocalLength_and_EqualAngleRings: this.shadowFac_FocalLength_and_EqualAngleRings(shadowObject);
				break;*/

			default:
				throw new ArgumentOutOfRangeException("Factory method "+ringTemplateSO3D.factory_method+" not found for "+ringTemplateSO3D.typeString());
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

		double height_rings = (double)shadowObject.getObject("height_rings");
		double ringcount = height_rings;
		//construct by creating FresnelLensRing Object3D instances and adding them to collection
		for (double iring = 0; iring <= ringcount - 1; iring += 1) {
			ShadowScientrace.ShadowObject3d so3d = new ShadowScientrace.ShadowObject3d(shadowObject);
			so3d.factory_method = (int?)ShadowScientrace.fresnelLensRingFactoryMethod.PlanoCenterAndRadians;
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

		double width_rings = (double)shadowObject.getObject("width_rings");
		double ringcount = width_rings;
		//construct by creating FresnelLensRing Object3D instances and adding them to collection
		for (double iring = 0; iring <= ringcount - 1; iring += 1) {
			ShadowScientrace.ShadowObject3d so3d = new ShadowScientrace.ShadowObject3d(shadowObject);
			so3d.factory_method = (int?)ShadowScientrace.fresnelLensRingFactoryMethod.PlanoCenterAndRadians;
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

		double angle_rings = (double)shadowObject.getObject("angle_rings");
		double ringcount = angle_rings;


		//construct by creating FresnelLensRing Object3D instances and adding them to collection
		for (double iring = 0; iring <= ringcount - 1; iring += 1) {
			ShadowScientrace.ShadowObject3d so3d = new ShadowScientrace.ShadowObject3d(shadowObject);
			so3d.factory_method = (int?)ShadowScientrace.fresnelLensRingFactoryMethod.PlanoCenterAndRadians;
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

