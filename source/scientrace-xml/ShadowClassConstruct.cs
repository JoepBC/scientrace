using System;

using System.Xml.Linq;
using System.Collections.Generic;
using ShadowScientrace;



namespace ScientraceXMLParser {

	public class ShadowClassConstruct : ScientraceXMLAbstractParser{
	
	public ShadowClassConstruct (Scientrace.Object3dCollection parent):base() {
		this.parentcollection = parent;
		}

	public ShadowScientrace.ShadowObject3d getShadowObject3dBase(XElement xel, Type classType) {
		//create ShadowObject core object
		ShadowScientrace.ShadowObject3d shadowO3D = 
				new ShadowScientrace.ShadowObject3d(classType, this.parentcollection, //ClassName
				this.getXMaterial(xel));

		//add some optional parameters
		shadowO3D.tag = this.X.getXNullString(xel, "Tag");
		shadowO3D.parseorder = this.X.getXNullDouble(xel, "ParseOrder");
		shadowO3D.debug_data = xel.ToString();

		this.getX3DBaseProperties(shadowO3D, xel);

		XElement xer = xel.Element("Register");
		if (xer != null) {
			if (this.X.getXBool(xer,"SolarCell", (this.X.getXBool(xer, "Performance", false))))
							shadowO3D.register_performance = true;
			if (this.X.getXBool(xer,"Distribution", (this.X.getXBool(xer, "DistributionObject", false))))
				shadowO3D.register_distribution = true;
			} // end Register element parse

		return shadowO3D;
		}

	private void getX3DBaseProperties(ShadowObject3d so3d, XElement xel) {
		so3d.arguments.Add("x3d_fill_emissive_color_html", this.X.getXNullString(xel, "X3DEmissiveColor"));
		so3d.arguments.Add("x3d_fill_diffuse_color_html", this.X.getXNullString(xel, "X3DDiffuseColor"));
		so3d.arguments.Add("x3d_fill_bitmap", this.X.getXNullString(xel, "X3DFillBitmap"));
		so3d.arguments.Add("x3d_fill_both_sides", this.X.getXNullBool(xel, "X3DFillBothSides", null));
		so3d.arguments.Add("x3d_fill_normal", this.X.getXVectorByName(xel, "X3DFillNormal", null));
		//OBSOLETE properties, replaced by introducing two colours to be set (or not for gridlines)
//		so3d.arguments.Add("x3d_fill", this.X.getXNullBool(xel, "X3DFill", null));
//		so3d.arguments.Add("x3d_fill_diffuse", this.X.getXNullBool(xel, "X3DFillDiffuse", null));
		}

/***************************** TEMPLATE **************************************
	public Scientrace.Object3d constructClassName(XElement xel) { //replace ClassName by actuall class name for readability purposes
		ShadowScientrace.ShadowObject3d shadowO3D = 
				new ShadowScientrace.ShadowObject3d(typeof(Scientrace.ClassName), this.parentcollection, //ClassName
				this.getXMaterial(xel));
		
		shadowO3D.arguments.Add("X", this.X.getXLocation(XElement, "X", null));
		//use the NullDouble so the constructor can decide whether this value is mandatory
		shadowO3D.arguments.Add("Y", this.X.getXNullDouble(XElement, "Y", null)); 
		shadowO3D.arguments.Add("Z", this.X.getXNzVectorByName(XElement, "Z", null));

		//base the constructor method on the parameters submitted by the user
		if (shadowO3D.hasArgument("X"))
			return shadowO3D.factory((int)ShadowScientrace.classNameFactoryMethod.ConstructorMethod1);
		if (shadowO3D.hasArgument("Z"))
			return shadowO3D.factory((int)ShadowScientrace.classNameFactoryMethod.ConstructorMethod2);
		throw new Exception("Factory method not set/known");
		} //end constructClassName
**************************** TEMPLATE END *************************************/		

	public Scientrace.Object3d constructRectangle(XElement xel) {
		ShadowScientrace.ShadowObject3d shRectangle = this.getShadowObject3dBase(xel, typeof(Scientrace.Rectangle));
				//new ShadowScientrace.ShadowObject3d(typeof(Scientrace.Rectangle), this.parentcollection, this.getXMaterial(xel));
		shRectangle.arguments.Add("center_location", this.X.getXLocation(xel, "Center", null));
		shRectangle.arguments.Add("corner_location", this.X.getXLocation(xel, "Location", null));

		shRectangle.arguments.Add("width", this.X.getXNzVectorSuggestions(xel, "Width", Scientrace.Vector.ZeroVector(), null));
		shRectangle.arguments.Add("height", this.X.getXNzVectorSuggestions(xel, "Height", Scientrace.Vector.ZeroVector(), null));

//		shRectangle.arguments.Add("width", this.X.getXNzVectorByName(xel, "Width", null));
//		shRectangle.arguments.Add("height", this.X.getXNzVectorByName(xel, "Height", null));
		Scientrace.Vector normal = this.X.getXNzVectorByName(xel, "Normal", null);
		Scientrace.Location pointing_towards = null;
		if (normal != null) {
			if (shRectangle.hasArgument("center_location"))
				pointing_towards = shRectangle.getLocation("center_location")+normal;
			else 
				throw new XMLException("Normal is defined for Rectangle, but cannot be used without a Center. \n"+xel.ToString());
			}
		shRectangle.arguments.Add("pointing_towards", this.X.getXLocation(xel, "PointingTowards", pointing_towards));
		shRectangle.arguments.Add("orthogonal_direction", this.X.getXUnitVectorByName(xel, "OrthogonalDirection", null));
		shRectangle.arguments.Add("side_length", this.X.getXNullDouble(xel, "SideLength"));
		if (shRectangle.hasArgument("center_location"))
			return shRectangle.factory("Center_and_Sidelength");
		if (shRectangle.hasArgument("corner_location")) 
			return shRectangle.factory("Loc_width_height");
		throw new XMLException("Cannot create Rectangle for XML:\n"+xel.ToString());
		}


	public Scientrace.Object3d constructFresnelLens(XElement xel) {
		/* due to the complex structure of the fresnelLens (a collection) 
		 * the generic getShadowObject3dBase method cannot be used */
		ShadowScientrace.ShadowObject3d shadowO3D = 
				new ShadowScientrace.ShadowObject3d(typeof(Scientrace.FresnelLens), this.parentcollection,
				this.parentcollection.materialproperties);

		//base parameters
		shadowO3D.arguments.Add("lens_plano_center", this.X.getXLocation(xel, "LensPlanoCenter", null));
		shadowO3D.arguments.Add("lens_sphere_location", this.X.getXLocation(xel, "LensSphereCenter", null));
		shadowO3D.arguments.Add("width_rings_count", this.X.getXNullDouble(xel, "WidthRings")); 
		shadowO3D.arguments.Add("height_rings", this.X.getXNullDouble(xel, "HeightRings")); 
		shadowO3D.arguments.Add("angle_rings", this.X.getXNullDouble(xel, "AngleRings")); 
		shadowO3D.arguments.Add("orientation_from_sphere_center", this.X.getXNzVectorByName(xel, "OpticalAxis", null).toUnitVector());
		shadowO3D.arguments.Add("double_convex", this.X.getXBool(xel, "DoubleConvex", false));
		shadowO3D.arguments.Add("lens_material", this.getXMaterial(xel));

		shadowO3D.arguments.Add("draw_3d_segment_linecount", this.X.getXNullInt(xel, "Draw3DLineCount"));

		//possible options:
			//or
			shadowO3D.arguments.Add("lens_sphere_radians_min", this.X.getXNullAngleByName(xel, "MinAngle")); 
			shadowO3D.arguments.Add("lens_sphere_radians_max", this.X.getXNullAngleByName(xel, "MaxAngle")); 
			//or
			shadowO3D.arguments.Add("lens_radius", this.X.getXNullDouble(xel, "Radius")); 

		//possible options:
			//or		
			shadowO3D.arguments.Add("lens_sphere_radius", this.X.getXNullDouble(xel, "LensSphereRadius")); 
			//or
			shadowO3D.arguments.Add("focal_length", this.X.getXNullDouble(xel, "FocalLength"));
		
		//base the constructor method on the parameters submitted by the user
		if (shadowO3D.hasArgument("angle_rings"))
			return shadowO3D.factory("EqualHeightRings");
		if (shadowO3D.hasArgument("width_rings_count"))
			return shadowO3D.factory("EqualHeightRings");
		if (shadowO3D.hasArgument("height_rings"))
			return shadowO3D.factory("EqualHeightRings");
		throw new Exception("Factory method not set/known");
		} //end constructClassName		
		
		
	public Scientrace.Object3d constructFresnelLensRing(XElement xel) { //replace ClassName by actuall class name for readability purposes
		ShadowScientrace.ShadowObject3d shadowO3D = this.getShadowObject3dBase(xel, typeof(Scientrace.FresnelLensRing));
		/* replaced by above:
			ShadowScientrace.ShadowObject3d shadowO3D = 
				new ShadowScientrace.ShadowObject3d(typeof(Scientrace.FresnelLensRing), this.parentcollection, //ClassName
				this.getXMaterial(xel));
		*/

		shadowO3D.arguments.Add("lens_plano_center", this.X.getXLocation(xel, "LensPlanoCenter", null));
		shadowO3D.arguments.Add("lens_sphere_location", this.X.getXLocation(xel, "LensSphereCenter", null));
		shadowO3D.arguments.Add("lens_sphere_radius", this.X.getXNullDouble(xel, "LensSphereRadius")); 
		shadowO3D.arguments.Add("lens_sphere_radians_min", this.X.getXAngleByName(xel, "MinAngle")); 
		shadowO3D.arguments.Add("lens_sphere_radians_max", this.X.getXAngleByName(xel, "MaxAngle")); 
		shadowO3D.arguments.Add("orientation_from_sphere_center", this.X.getXNzVectorByName(xel, "OpticalAxis", null).toUnitVector());

		//base the constructor method on the parameters submitted by the user
		if (shadowO3D.hasArgument("lens_plano_center"))
			return shadowO3D.factory("PlanoCenterAndRadians");
		if (shadowO3D.hasArgument("lens_sphere_location"))
			return shadowO3D.factory("SphereCenterAndRadians");
		/*if (shadowO3D.hasArgument("Z"))
			return shadowO3D.factory((int)ShadowScientrace.classNameFactoryMethod.ConstructorMethod2);*/
		throw new Exception("Factory method not set/known");
		} //end constructClassName


	public Scientrace.Object3d constructDoubleConvexLens(XElement xel) {
		ShadowScientrace.ShadowObject3d shDCLens =  this.getShadowObject3dBase(xel, typeof(Scientrace.DoubleConvexLens));

		shDCLens.arguments.Add("focal_length", this.X.getXNullDouble(xel, "FocalLength"));
		shDCLens.arguments.Add("lens_diameter", this.X.getXNullDouble(xel, "LensDiameter"));
		shDCLens.arguments.Add("sphere1_radius", this.X.getXNullDouble(xel, "Radius1"));
		shDCLens.arguments.Add("sphere2_radius", this.X.getXNullDouble(xel, "Radius2"));
		
		shDCLens.arguments.Add("lens_center", this.X.getXLocation(xel, "Center", null));
		shDCLens.arguments.Add("optical_axis", this.X.getXNzVectorByName(xel, "OpticalAxis", //Still try depricated name: LensPlaneNormal
									this.X.getXNzVectorByName(xel, "LensPlaneNormal", null)));
		shDCLens.arguments.Add("sphere1_center_loc", this.X.getXLocation(xel, "Sphere1Center", null));
		shDCLens.arguments.Add("sphere2_center_loc", this.X.getXLocation(xel, "Sphere2Center", null));
		
		if (shDCLens.hasArgument("focal_length"))
			return shDCLens.factory("FocalLength_and_Diameter");
		if (shDCLens.hasArgument("lens_diameter")) 
			return shDCLens.factory("TwoRadii_and_Diameter");
		if (shDCLens.hasArgument("sphere2_center_loc")) 
			return shDCLens.factory("TwoRadii_and_Locations");
		throw new Exception("Factory method not set/known");
		}

	public Scientrace.MaterialProperties getXMaterial(XElement xel) {
		string materialid;
		XAttribute material_attribute = xel.Attribute("Material");
		XElement xmaterial = null;

		if (material_attribute == null) {
			xmaterial = xel.Element("Material");
			if (xmaterial == null) {
				throw new XMLException("No material defined in XML Element: \n---\n"+xel.ToString()+"\n---");
				}
			materialid = this.X.getXStringByAttribute(xmaterial,"Class");
			} else {
			materialid = material_attribute.Value;
			}
		
		if (Scientrace.MaterialProperties.listedIdentifier(materialid))
			return Scientrace.MaterialProperties.FromIdentifier(materialid);

			
		switch (materialid) {
			case "StaticNTransparant": //old type I once made... :S fall through to non-typo version
			case "StaticNTransparent":
				double refindex = this.X.getXDouble(xmaterial, "RefractiveIndex");
				Scientrace.StaticNTransparentMaterial matprops = new Scientrace.StaticNTransparentMaterial(refindex);
				matprops.reflects = this.X.getXBool(xmaterial, "Reflects", true);

				return matprops;
				//break;
			case "StaticReflectingAbsorber":
				Scientrace.StaticReflectingAbsorberMaterial statRefAbsorber = new Scientrace.StaticReflectingAbsorberMaterial();
				statRefAbsorber.setRefractiveIndex(this.X.getXDouble(xmaterial, "RefractiveIndex", 1));
				statRefAbsorber.setReflectionFraction(this.X.getXDouble(xmaterial, "Reflection", 1));
				statRefAbsorber.setAbsorptionFraction(this.X.getXDouble(xmaterial, "Absorption", 0));
				return statRefAbsorber;
				//break;
			default:
				throw new XMLException("Material Class ["+materialid+"] unknown");
				//break;
			}
		}	


/* TRIANGULARPRISM ***************************/

	//supportive methods on TriangularPrism creation
	private bool triangularPrismHasVolume(ShadowObject3d sho3d) {
		Scientrace.Vector length = sho3d.getVector("length");
		Scientrace.Vector width = sho3d.getVector("width");
		Scientrace.Vector height = sho3d.getVector("height");

		//as angle doesn't have to be provided when height is provided directly use angle default 1.
		double angle = sho3d.getDouble("angle", 1);

		if (angle*length.length*width.length*height.length == 0) {
			Console.WriteLine("WARNING: TriangularPrism "+sho3d.tag+
				" has no volume and will " +
				"therefor not be created.");
			return false;
			}
		return true;
		}
	private Scientrace.NonzeroVector calcTriangleHeightVector(ShadowObject3d sho3d) {
		Scientrace.Vector length = sho3d.getVector("length");
		Scientrace.Vector width = sho3d.getVector("width");
		Scientrace.Vector heightdir = sho3d.getVector("heightdir");

		double angle = sho3d.getDouble("angle");

		//create a vector orthogonal to length en width in the same binary direction as heightdir.
		Scientrace.UnitVector owl = (width.crossProduct(length) * 
					Math.Sign(width.crossProduct(length).dotProduct(heightdir))).tryToUnitVector();

		Scientrace.NonzeroVector bdir = ( //calculate the direction of the short side of the prism
										owl*Math.Sin(angle) + 
										width.tryToUnitVector()*Math.Cos(angle)
											).tryToNonzeroVector();
		if ((bdir.length < 0.99999) || (bdir.length > 1.00001)) {
				throw new ArgumentOutOfRangeException("bdir.length", bdir.length, "!= 1");
				}
				
		Scientrace.VectorTransform trf = new Scientrace.VectorTransform(
			width.tryToNonzeroVector(), owl.tryToNonzeroVector(), length.tryToNonzeroVector());
		Scientrace.NonzeroVector hdirtrf = trf.transform(heightdir).tryToNonzeroVector();
		Scientrace.Vector hprimetrf = new Scientrace.Vector(hdirtrf.x, hdirtrf.y, 0); //eliminate "length" component of heightdir in hprime
		//Console.WriteLine("HPRIMTRF:"+hprimetrf.trico());
		Scientrace.NonzeroVector hprimedir = trf.transformback(hprimetrf).tryToNonzeroVector().normalized();
		/*       ^
		 *      /C\
		 *     /   \  
		 *  h'/     \ b
		 *   /       \
		 *  /B_______A\
		 *     width
		 * angle = A; beta = B; gamma = C.
		 */
		//sine rule: hprimelen / sin A = width.length() / sin C = blen / sin B
		double beta, gamma;
		beta = Math.Acos(hprimedir.normalized().dotProduct(width.tryToNonzeroVector().normalized()));
		gamma = Math.PI - (angle + beta);
		double hprimelen;
		double sinruleconstant = width.length / Math.Sin(gamma);
		hprimelen = sinruleconstant*Math.Sin(angle);
		Scientrace.NonzeroVector hprime = hprimedir*hprimelen;

		// check: (trf.transform(hprime).x / hdirtrf.x) == (trf.transform(hprime).y / hdirtrf.y)
		double xycoeff = ((trf.transform(hprime).x / hdirtrf.x) / (trf.transform(hprime).y / hdirtrf.y));
		if (Math.Abs(1-xycoeff)>0.00001) { //doesn't do anything if .x/.x = NaN, but that's OK for now.
				throw new ArgumentOutOfRangeException("xycoeff", xycoeff, "!=1");
				}

		try {
		Scientrace.NonzeroVector h = ((Math.Abs(hdirtrf.x)>Math.Abs(hdirtrf.y)) ? // Preventing .x or .y denominator == 0 errors.
				trf.transformback(hdirtrf*(trf.transform(hprime).x / hdirtrf.x)) :
				trf.transformback(hdirtrf*(trf.transform(hprime).y / hdirtrf.y))
				).tryToNonzeroVector();
		
		return h;
			} catch (Scientrace.ZeroNonzeroVectorException zne)	{
			Console.WriteLine("ERROR: calculated height for triangularprism has length zero!");
			throw (zne); 
			}
		} //end calcTriangleHeightVector


	public Scientrace.Object3d constructTriangularPrism(XElement xel) {
		if (!xel.Name.ToString().IsIn("TriangularPrism", "Prism")) { // != "TriangularPrism") {
			throw new XMLException("TriangularPrism does not match its name: "+xel.Name.ToString());
			}

		ShadowScientrace.ShadowObject3d shadowO3D = 
				new ShadowScientrace.ShadowObject3d(typeof(Scientrace.TriangularPrism), this.parentcollection, //ClassName
					this.getXMaterial(xel));

		// Base parameters:
		shadowO3D.arguments.Add("corner_location", this.X.getXLocation(xel, "Location", null));
		shadowO3D.arguments.Add("perfect_top", this.X.getXBool(xel, "PerfectFront", true));
		shadowO3D.arguments.Add("length", this.X.getXNzVectorByName(xel, "Length"));
		shadowO3D.arguments.Add("width", this.X.getXNzVectorByName(xel, "Width"));

		// Optional parameters
			// TRY:
			shadowO3D.arguments.Add("height",  this.X.getXNzVectorByName(xel, "Height", null));
			// ELSE:
			if (!shadowO3D.hasArgument("height")) {
				shadowO3D.arguments.Add("heightdir", this.X.getXNzVectorByName(xel, "HeightDirection"));
				shadowO3D.arguments.Add("angle", this.X.getXAngleByName(xel, "Angle"));
				shadowO3D.arguments.Add("height", this.calcTriangleHeightVector(shadowO3D));
				}

		//if (!
		//just throw a warning if prism has no volume.
			this.triangularPrismHasVolume(shadowO3D);
		//	) throw new XMLException("Prism has no volume...");

		Scientrace.TriangularPrism tretprism = new Scientrace.TriangularPrism(shadowO3D);

		return tretprism;
		}



/* END TRIANGULARPRISM */






}
}
