using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ScientraceXMLParser {


public class ScientraceXMLParser {

	public XDocument xd;
	public XElement xsctconf;
	private CustomXMLDocumentOperations X;
	private XMLLightSourceParser lp;
	private XMLOutputParser op;

	public ScientraceXMLParser (XDocument xd) {

		//TODO: read from XML and put in appropriate method
		Scientrace.VectorTransform.CACHE_TRANSFORMS = true;

		this.xd = xd;
		this.X = new CustomXMLDocumentOperations();
		this.xsctconf = this.xd.Element("ScientraceConfig");
		this.setTraceJournalProperties(xsctconf);
		if (this.xsctconf == null) { throw new NotSupportedException("No <ScientraceConfig> root-node in XML");}
		
		}
		
	public void setTraceJournalProperties(XElement xconfig) {
		Scientrace.TraceJournal tj = Scientrace.TraceJournal.Instance;
		long ticks = (DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks)/10000000;
		string timestamp = ticks.ToString();	
		tj.config_id = this.X.getXString(xconfig.Attribute("ConfigID"), "g"+timestamp);
		Scientrace.Trace.support_polarisation = this.X.getXBool(xconfig, "PolarisationSupport", //default language UK English, but also check US English for def. val.
			this.X.getXBool(xconfig, "PolarizationSupport", true));

		if (this.X.hasAttribute(xconfig, "ConfigDescription")) {
				//Console.WriteLine("Config_ID: "+this.X.getXString(xconfig.Attribute("ConfigID")));
				tj.config_description = this.X.getXString(xconfig.Attribute("ConfigDescription"));
			}
		if (this.X.hasAttribute(xconfig, "TimeStamp")) {
				tj.timestamp = this.X.getXString(xconfig.Attribute("TimeStamp"));
			}
		}

	public Scientrace.Object3dEnvironment parseEnvironment() {
		XElement xenv = this.xsctconf.Element("ObjectEnvironment");
		if (xenv == null) {
			Console.WriteLine("WARNING: no Object3d Environment defined in configuration file");
			}
		return this.parseXEnv(xenv);
		}
		
	public void parseOutput() {
		this.op = new XMLOutputParser();
		this.op.parseOutput(this.xsctconf);
		}
		
	public Scientrace.Object3dEnvironment parseXEnv(XElement xenv) {
		//Create the collection itself with its properties first.
		Scientrace.Object3dEnvironment retenv;
		
		// Creating "the entire object-space"
		double env_radius = this.X.getXDouble(xenv.Attribute("Radius"));
		bool drawaxes = this.X.getXBool(xenv.Attribute("DrawAxes"), true);
		string environment_material_id = this.X.getXString(xenv.Attribute("Environment"), "air");
		Scientrace.Vector cameraviewpoint = this.X.getXVector(xenv.Element("CameraViewpoint"), new Scientrace.Vector(0,0,1));
		Scientrace.Vector camrotationvec = null;
		double camrotationangle = 0;
		XElement camrot = xenv.Element("CameraRotation");
		if (camrot != null) {
			camrotationvec = this.X.getXVectorByName(camrot, "Vector");
			camrotationangle = this.X.getXAngleByName(camrot, "Angle");
			}
//			Scientrace.Vector camorientation = this.X.getXVector(xenv.Element("Orientation"), new Scientrace.Vector(0,0,1));
		Scientrace.MaterialProperties env_material = Scientrace.MaterialProperties.FromIdentifier(environment_material_id);
		retenv = new Scientrace.Object3dEnvironment(env_material, env_radius, cameraviewpoint);
			retenv.perishAtBorder = true;
			retenv.labelaxes = drawaxes;
			retenv.camrotationangle = camrotationangle;
		if (camrotationvec != null)
			retenv.camrotationvector = camrotationvec;
			retenv.tag = this.X.getXString(xenv.Attribute("Tag"), "ScientraceXML_Setup");
		
		//Parsing lightsources
		this.lp = new XMLLightSourceParser(retenv);
		this.lp.parseLightsources(xenv, retenv);
			
		//ADDING UNDERLYING BODIES/OBJECTS
		this.parseXObject3dCollectionContent(xenv, retenv);
		
		//return environment
		return retenv;
		}

	public void parseXObject3dCollectionContent(XElement xcol, 
					Scientrace.Object3dCollection objectcollection) {

		//XMLObject3dParser o3dp = new XMLObject3dParser(xcol, this.X, objectcollection);
		XMLObject3dParser o3dp = new XMLObject3dParser(objectcollection);
			
		ShadowClassConstruct shadowConstructor = new ShadowClassConstruct(objectcollection);
			
		//PARSE OBJECTS WITHIN COLLECTION HERE
			foreach (XElement xel in xcol.Elements()) {
				this.parseXObject3d(xel, objectcollection, o3dp, shadowConstructor);
				
				}
		}


	public void parseXObject3d(XElement xel, Scientrace.Object3dCollection col, XMLObject3dParser o3dp, ShadowClassConstruct shadowConstructor) {
		Scientrace.Object3d createdObject3d = null;
		switch (xel.Name.ToString()) {
			case "CircularFresnelPrism":
				createdObject3d = o3dp.parseXFresnelPrism(xel);
				break;
			case "ParabolicMirror":
				createdObject3d = o3dp.parseXParabolicMirror(xel);
				//PARSE PARAB MIRROR
				break;
			case "Prism": //fall through
			case "TriangularPrism":
				createdObject3d = shadowConstructor.constructTriangularPrism(xel);
				//createdObject3d = o3dp.parseXTriangularPrism(xel);
				break;
			case "RectangularPrism":
				createdObject3d = o3dp.parseXRectangularPrism(xel);
				break;
			case "SquareCell":
			case "Rectangle":
				//square cell can be used for any square or rectangular surface-object
				//createdObject3d = o3dp.parseXRectangle(xel);
				createdObject3d = shadowConstructor.constructRectangle(xel);
				break;
			case "Sphere":
				createdObject3d = o3dp.parseXSphere(xel);
				break;
			case "PlanoConvexLens":
				createdObject3d = o3dp.parseXPlanoConvexLens(xel);
				break;
			case "FresnelLens":
				createdObject3d = shadowConstructor.constructFresnelLens(xel);
				break;		
			case "FresnelLensRing":
				createdObject3d = shadowConstructor.constructFresnelLensRing(xel);
				break;		
			case "DoubleConvexLens":
				//createdObject3d = o3dp.parseXDoubleConvexLens(xel);
				createdObject3d = shadowConstructor.constructDoubleConvexLens(xel);
				break;
			case "BorderedVolume":
				createdObject3d = o3dp.parsXBorderedVolume(xel);
				break;
			case "ToppedPyramid":
				createdObject3d = o3dp.parseXToppedPyramid(xel);
				break;
			case "CameraViewpoint":
				//set cam settings
				break;
			case "CameraRotation":
				//set cam settings
				break;
			case "LightSource":
				//do nothing, already parsed previously
				break;
			case "Ignore":
			case "IGNORE":
				//do nothing, IGNORE!
				break;
			default:
				if (!(xel.Name.ToString().Substring(0,3)=="REM")) {
					Console.WriteLine("WARNING: UNKNOWN OBJECT: "+xel.Name+" \n[XML code]\n"+xel.ToString()+"\n[/XML code]\n");
					}
				break;
			}
		if (createdObject3d != null) {
			XMLTraceModifierParser xtmp = new XMLTraceModifierParser();
			List<Scientrace.UniformTraceModifier> utms = xtmp.getModifiers(xel);
			createdObject3d.addSurfaceModifiers(utms);
			}
		} // end parseXObject3d
		
		
	}


}