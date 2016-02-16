using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ScientraceXMLParser {
	
	
public class XMLObject3dParser : ScientraceXMLAbstractParser {

//	public XMLObject3dParser (XElement xel, CustomXMLDocumentOperations X, Scientrace.Object3dCollection env): base(xel, X) {
	public XMLObject3dParser (Scientrace.Object3dCollection env): base() {
		this.parentcollection = env;
		}

/* replaced by ShadowScientrace factory
	public Scientrace.DoubleConvexLens parseXDoubleConvexLens(XElement xLens) {

		if (xLens.Name.ToString() != "DoubleConvexLens") {
			throw new XMLException("DoubleConvexLens does not match its name: "+xLens.Name.ToString());
			}

		Scientrace.MaterialProperties materialprops = this.getXMaterial(xLens.Element("Material"));		
		Scientrace.Location lensPlaneCenterLoc = this.X.getXLocation(xLens, "LensPlaneCenter");
		Scientrace.NonzeroVector lensPlaneNormal = this.X.getXNzVectorByName(xLens, "LensPlaneNormal");
		double r1 = this.X.getXDouble(xLens, "Radius1");
		double r2 = this.X.getXDouble(xLens, "Radius2");
		double lensDiameter = this.X.getXDouble(xLens, "LensDiameter");

		Scientrace.DoubleConvexLens tRetLens = null;
		
		Scientrace.DoubleConvexLens.CreateWithLensDiameter(
			this.parentcollection, materialprops, lensPlaneCenterLoc, lensPlaneNormal,
			lensDiameter, r1, r2);
			
		return tRetLens;
		}
*/


	public Scientrace.PlanoConvexLens parseXPlanoConvexLens(XElement xLens) {

		if (xLens.Name.ToString() != "PlanoConvexLens") {
			throw new XMLException("PlanoConvexLens does not match its name: "+xLens.Name.ToString());
			}

		// Replaced with below: Scientrace.MaterialProperties materialprops = this.getXMaterial(xLens.Element("Material"));		
		Scientrace.MaterialProperties materialprops = this.getXMaterialForObject(xLens);		
		Scientrace.Location lensFlatCenterLoc = this.X.getXLocation(xLens, "PlanoCenter");;
		Scientrace.NonzeroVector sphereRadiusVec = this.X.getXNzVectorByName(xLens, "SphereRadius");
		double lensDiameter = this.X.getXDouble(xLens, "LensDiameter");;
		Scientrace.PlanoConvexLens tRetLens = null;
		
		Scientrace.PlanoConvexLens.CreateWithLensDiameter(
			this.parentcollection, materialprops, lensFlatCenterLoc, lensDiameter, sphereRadiusVec);
			
		return tRetLens;
		}

	public Scientrace.Sphere parseXSphere(XElement xSphere) {
		if (xSphere.Name.ToString() != "Sphere") {
			throw new XMLException("Sphere does not match its name: "+xSphere.Name.ToString());
			}
		double radius = this.X.getXDouble(xSphere, "Radius");
		Scientrace.Location loc = this.X.getXLocation(xSphere, "Location");
			// Replaced with below: Scientrace.MaterialProperties materialprops = this.getXMaterial(xSphere.Element("Material"));
		Scientrace.MaterialProperties materialprops = this.getXMaterialForObject(xSphere);
		Scientrace.Sphere tRetSphere  = new Scientrace.Sphere(this.parentcollection,
			materialprops, loc, radius);
			
		tRetSphere.setRefAxis(this.X.getXNzVectorByName(xSphere, "RefVec", null));
		return tRetSphere;
		}
		

	public Scientrace.ComplexBorderedVolume parsXBorderedVolume(XElement xBorVol) {
			// Replaced with below: Scientrace.MaterialProperties materialprops = this.getXMaterial(xBorVol.Element("Material"));
		Scientrace.MaterialProperties materialprops = this.getXMaterialForObject(xBorVol);
		if (xBorVol.Name.ToString() != "BorderedVolume") {
			throw new XMLException("BorderedVolume does not match its name: "+xBorVol.Name.ToString());
			}
		
		List<List<Scientrace.PlaneBorder>> nestedBorders = new List<List<Scientrace.PlaneBorder>>();
		foreach (XElement xSubVolume in xBorVol.Elements("SubVolume")) {
			List<Scientrace.PlaneBorder> tBorderList = new List<Scientrace.PlaneBorder>(); 
			foreach (XElement xBorder in xSubVolume.Elements("Plane")) {
				if (xBorder.Element("Loc1") == null) { // No LocSet? Defined by location and AllowedNormal
					Scientrace.Location borderLoc = this.X.getXLocation(xBorder, "Location");
					Scientrace.NonzeroVector borderAllowDir = this.X.getXNzVectorByName(xBorder, "AllowedNormal");
					tBorderList.Add(new Scientrace.PlaneBorder(borderLoc, borderAllowDir));
					} else {  // LocSet defined with 3 locations and an included-location to define the direction.
					Scientrace.Location loc1 = this.X.getXLocation(xBorder, "Loc1");
					Scientrace.Location loc2 = this.X.getXLocation(xBorder, "Loc2");
					Scientrace.Location loc3 = this.X.getXLocation(xBorder, "Loc3");
					Scientrace.Location includeLoc = this.X.getXLocation(xBorder, "IncludeLoc");
					tBorderList.Add(Scientrace.PlaneBorder.createBetween3LocationsPointingTo(loc1, loc2, loc3, includeLoc));
					}
				}
			nestedBorders.Add(tBorderList);
			}
		Scientrace.ComplexBorderedVolume tRetBoxVol = new Scientrace.ComplexBorderedVolume(this.parentcollection, materialprops, nestedBorders);
		return tRetBoxVol;
		}				

	public Scientrace.PlaneBorderEnclosedVolume parseXToppedPyramid(XElement xToppedPyramid) {

			// Replaced with below: Scientrace.MaterialProperties materialprops = this.getXMaterial(xToppedPyramid.Element("Material"));
		Scientrace.MaterialProperties materialprops = this.getXMaterialForObject(xToppedPyramid);
		
		if (xToppedPyramid.Name.ToString() != "ToppedPyramid") {
			throw new XMLException("ToppedPyramid does not match its name: "+xToppedPyramid.Name.ToString());
			}

		List<Scientrace.Location> front_corners = new List<Scientrace.Location>();
		foreach (XElement xFrontCorner in xToppedPyramid.Elements("Corner")) {
			Scientrace.Location loc = this.X.getXLocation(xFrontCorner);
			front_corners.Add(loc);
			}
		Scientrace.Location virtual_top = this.X.getXLocation(xToppedPyramid, "VirtualTop");
		// parsing topping plane data:
		XElement xTopPlane = xToppedPyramid.Element("TopPlane");
		if (xTopPlane==null) {
			throw new XMLException("TopPlane element not found... "+xToppedPyramid.ToString());
			}
		Scientrace.Location topPlaneLoc = this.X.getXLocation(xTopPlane, "Location");
		Scientrace.NonzeroVector topPlaneNormal = this.X.getXNzVectorByName(xTopPlane, "Normal");
		Scientrace.PlaneBorder topping_plane = new Scientrace.PlaneBorder(topPlaneLoc, topPlaneNormal);

		Scientrace.PlaneBorderEnclosedVolume tRetToppedPyramid  = Scientrace.PlaneBorderEnclosedVolume.createToppedPyramid(this.parentcollection, materialprops,
			front_corners, virtual_top, topping_plane);
		return tRetToppedPyramid;
		}				


	public Scientrace.MaterialProperties getXMaterialForObject(XElement xmaterial) {
		ShadowClassConstruct scc = new ShadowClassConstruct(this.parentcollection);
		Scientrace.MaterialProperties matprop = scc.getXMaterial(xmaterial);
		return matprop;
		/*
		string materialid = this.X.getXStringByAttribute(xmaterial,"Class");
		if (Scientrace.MaterialProperties.listedIdentifier(materialid)) {
			return Scientrace.MaterialProperties.FromIdentifier(materialid);
			}
			
		switch (materialid) {
			case "StaticNTransparant": //old type I once made... :S
			case "StaticNTransparent":
				double refindex = this.X.getXDouble(xmaterial, "RefractiveIndex");
				return new Scientrace.StaticNTransparentMaterial(refindex);
				//break;
			default:
				throw new XMLException("Material Class ["+materialid+"] unknown");
				//break;
			}*/
		}

		
	public Scientrace.RectangularPrism parseXRectangularPrism(XElement xrectprism) {
		//try {
		//Console.WriteLine("Adding Square Prism");
		//check basic prerequisites
		if (xrectprism.Name.ToString() != "RectangularPrism") {
			throw new XMLException("RectangularPrism does not match its name: "+xrectprism.Name.ToString());
			}
		Scientrace.Vector width = this.X.getXVectorByName(xrectprism, "Width");
		Scientrace.Vector length = this.X.getXVectorByName(xrectprism, "Length");
		Scientrace.Vector height = this.X.getXVectorByName(xrectprism,"Height");
		if (width.length*length.length*height.length == 0) {
			Console.WriteLine("WARNING: Rectangular Prism "+xrectprism.Attribute("Tag").Value+" has no volume and will " +
				"therefor not be created.");
			return null;
			}
		Scientrace.Location location = null;
		if (xrectprism.Element("Center") == null) {
			location = this.X.getXVectorByName(xrectprism,"Location").toLocation();
			} else {
			location = (this.X.getXVectorByName(xrectprism, "Center")+(width*-0.5)+(length*-0.5)+(height*-0.5))
					.toLocation();
			}

		Scientrace.MaterialProperties materialprops = this.getXMaterialForObject(xrectprism);
		/*
		string materialid = this.X.getXStringByAttribute(xrectprism,"Material");
		Scientrace.MaterialProperties material = Scientrace.MaterialProperties.FromIdentifier(materialid); */
		//tryToNonzeroVector operations below are permitted since the prism has a volume as was tested above.
		Scientrace.RectangularPrism tretprism = new Scientrace.RectangularPrism(this.parentcollection,
			materialprops, location, width.tryToNonzeroVector(), height.tryToNonzeroVector(),length.tryToNonzeroVector());
		this.addCommonObjectProperties(tretprism, xrectprism);
		return tretprism;
		/*	} catch(Exception e) {Console.WriteLine("Couldn't create RectangularPrism: "+this.X.getXString(xrectprism.Attribute("Tag")));
				throw(e);
			}*/
		}
		
		
	public Scientrace.CircularFresnelPrism parseXFresnelPrism(XElement xprism) {
		//check basic prerequisites
		if (xprism.Name.ToString() != "CircularFresnelPrism") {
			throw new XMLException("FresnelPrism does not match its name: "+xprism.Name.ToString());
			}
			Scientrace.Location location = this.X.getXVector(xprism.Element("Location")).toLocation();
			Scientrace.Vector tsurfv1 = this.X.getXVector(xprism.Element("SurfaceVector1"));
			Scientrace.Vector tsurfv2 = this.X.getXVector(xprism.Element("SurfaceVector2"));
			
			Scientrace.NonzeroVector surfv1, surfv2, zaxisheight;
			try {
				surfv1 = tsurfv1.tryToNonzeroVector();
				surfv2 = tsurfv2.tryToNonzeroVector();
				zaxisheight = surfv1.crossProduct(tsurfv2).tryToNonzeroVector();
				} catch {
				throw new XMLException("Parsing fresnel prism error: SurfaceVectors1 and 2 may not be zero.");
				}

			Scientrace.Vector tzaxis = this.X.getXVector(xprism.Element("Height"), zaxisheight);
			try {
				zaxisheight = tzaxis.tryToNonzeroVector();
				} catch {
				throw new XMLException("Parsing fresnel prism error: height vector may not be zero.");
				}

			double LongSideAngle = this.X.getXAngle(xprism.Element("LongSideAngle"));
			double ShortSideAngle = this.X.getXAngle(xprism.Element("ShortSideAngle"));
			int TeethCount = this.X.getXInt(xprism.Attribute("TeethCount"));
			string materialid = this.X.getXString(xprism.Attribute("Material"));
			Scientrace.MaterialProperties material = Scientrace.MaterialProperties.FromIdentifier(materialid);
			Scientrace.CircularFresnelPrism tretprism = 
				new Scientrace.CircularFresnelPrism(this.parentcollection,
				this.parentcollection.materialproperties, location, surfv1, surfv2, zaxisheight, 
				LongSideAngle, ShortSideAngle, TeethCount, material);
			this.addCommonObjectProperties(tretprism, xprism);
			return tretprism;
		}
	
/*		
		Scientrace.CircularFresnelPrism cprism = new Scientrace.CircularFresnelPrism(env, air,
			new Location(0, 0, 0), new NonzeroVector(prismradius, 0, 0).rotateAboutY(prismAngleRadians),
			new NonzeroVector(0,0, prismradius).rotateAboutY(prismAngleRadians), new NonzeroVector(0, -0.5E-2, 0),
			this.prism_angle*Math.PI/180, (Math.PI/2)-(this.prism_angle*Math.PI/180), dentalcount, pmma);
		 *?/

	/* Prism and Mirror data from configfile */
		public Scientrace.ParabolicMirror parseXParabolicMirror(XElement xmir) {
			Scientrace.UnitVector miraxis = this.X.getXNzVector(xmir.Element("MirrorAxis")).toUnitVector();

			string materialid = this.X.getXString(xmir.Attribute("Material"));
			Scientrace.MaterialProperties material = Scientrace.MaterialProperties.FromIdentifier(materialid);
			XMLBorderParser xbp = new XMLBorderParser();
			Scientrace.AbstractGridBorder border = xbp.getXBorder(xmir.Element("Border"));
			Scientrace.ParabolicMirror tretpmir;
			if (xmir.Attribute("DistanceToMirror") == null) {
				double parabolic_constant = this.X.getXDouble(xmir.Attribute("ParabolicConstant"));
				Scientrace.Location location = this.X.getXLocation(xmir.Element("Location"));
				tretpmir = new Scientrace.ParabolicMirror(
					this.parentcollection, material, location, miraxis, parabolic_constant, border);
				} else {
				double distance_to_mirror = this.X.getXDouble(xmir.Attribute("DistanceToMirror"));
				Scientrace.Location focuspoint = this.X.getXLocation(xmir.Element("FocusPoint"));
//				Console.WriteLine("-->"+focuspoint.tricoshort()+ " / "+ distance_to_mirror+miraxis.tricon()+ " / <---");
				tretpmir = Scientrace.ParabolicMirror.CreateAtFocus(this.parentcollection, material,
								focuspoint, distance_to_mirror, miraxis, border);
				tretpmir.exportX3Dgridsteps = 64;
				}
			return tretpmir;
		}
		
		public Scientrace.Rectangle parseXRectangle(XElement xsc) {
			if (xsc.Element("Center") != null) {
				return this.parseXRectangleCenterConstructor(xsc);
				}
			// else (location with width-height vectors construction)
			return this.parseXRectangleVectorBasedConstructor(xsc);
			//throw new NotImplementedException("Construction of rectangular cells is not yet implemented");
			}

		public Scientrace.Rectangle parseXRectangleVectorBasedConstructor(XElement xsc) {
			Scientrace.Location location = this.X.getXLocation(xsc.Element("Location"));
			/* OLD method that caused a crashed on zerovectors.
			Scientrace.NonzeroVector width = this.X.getXNzVector(xsc.Element("Width"));
			Scientrace.NonzeroVector height = this.X.getXNzVector(xsc.Element("Height")); */

			Scientrace.Vector vwidth = this.X.getXVector(xsc.Element("Width"));
			Scientrace.Vector vheight = this.X.getXVector(xsc.Element("Height"));
			if (vwidth.length*vheight.length == 0) {
				return null;
				}
			Scientrace.NonzeroVector width = vwidth.tryToNonzeroVector();
			Scientrace.NonzeroVector height = vheight.tryToNonzeroVector();
			string materialid = this.X
				.getXString(xsc.Attribute("Material"));
			Scientrace.MaterialProperties material = Scientrace.MaterialProperties.FromIdentifier(materialid);
			//Console.WriteLine("MAT CELL: "+material.GetType());
			Scientrace.Rectangle retcel = new Scientrace.Rectangle(this.parentcollection,
				material, location, width, height);
			this.addCommonObjectProperties(retcel, xsc);
			this.registerObject(xsc, retcel);
			return retcel;
			}
	
		public void registerObject(XElement xe, Scientrace.PhysicalObject3d o3d) {
			XElement xer = xe.Element("Register");
			if (xer == null) { return; }
			// This one has some history which is still backwards compatible. Early versions
			// of Scientrace demanded an element called "Register" and an attribute called
			// "SolarCell" (later replaced by "Performace") to be set. 
			// Nowadays, a simple "Register" attribute to the object main element will do.
			if (this.X.getXBool(xer,"SolarCell", 
					(this.X.getXBool(xer, "Performance", 
								(this.X.getXBool(xe, "Register", false))
					)))) {
				Scientrace.TraceJournal.Instance.registerObjectPerformance(o3d);
				}
			//20151021: The DistributionSquare functionality has been removed as it was no longer considered useful.
			/*if (this.X.getXBool(xer.Attribute("DistributionObject"), false)) {
				Scientrace.TraceJournal.Instance.registerDistributionObject(o3d);
				} */
			}
		
		
		public Scientrace.Rectangle parseXRectangleCenterConstructor(XElement xsc) {
			Scientrace.Location center = this.X.getXLocation(xsc.Element("Center"));
			//Scientrace.Location pointingtowards = this.X.getXLocation(xsc.Element("PointingTowards"));
			Scientrace.Location pointingtowards = this.X.getXLocation(xsc, "PointingTowards", (this.X.getXLocation(xsc, "Normal")+center));

			Scientrace.UnitVector orthogonaldir = this.X.getXVector(xsc.Element("OrthogonalDirection")).tryToUnitVector();
			//double sidelength = this.X.getXDouble(xsc.Attribute("SideLength"));
			double sidelength = this.X.getXDouble(xsc, "SideLength");
			string materialid = this.X.getXString(xsc.Attribute("Material"));
			Scientrace.MaterialProperties material = Scientrace.MaterialProperties.FromIdentifier(materialid);
			//Console.WriteLine("MAT CELL: "+material.GetType());
			Scientrace.Rectangle retcel = new Scientrace.Rectangle(this.parentcollection, material, center, 
				                          pointingtowards, orthogonaldir, sidelength);
			this.addCommonObjectProperties(retcel, xsc);
			this.registerObject(xsc, retcel);
			return retcel;
			}
	
		
		public void addCommonObjectProperties(Scientrace.Object3d anObject, XElement xel) {
			XAttribute tag = xel.Attribute("Tag");
			if (tag != null) {
				anObject.tag = tag.Value;
				//Console.WriteLine("TAG SET: "+anObject.tag);
				}
			XAttribute priority = xel.Attribute("ParseOrder");
			if (priority != null) {
				anObject.parseOrder = Convert.ToDouble(priority.Value);
				//Console.WriteLine("ParseOrder adjusted: "+anObject.parseOrder);
				}
			}
	}
}

