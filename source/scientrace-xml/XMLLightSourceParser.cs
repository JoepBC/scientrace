using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ScientraceXMLParser {
	
	
public class XMLLightSourceParser : ScientraceXMLAbstractParser {
		
//	public XMLLightSourceParser(XElement xel, CustomXMLDocumentOperations X, Scientrace.Object3dEnvironment env): base(xel, X) {
	public XMLLightSourceParser(Scientrace.Object3dEnvironment env): base() {
		this.parentcollection = env;
		}

	public void parseLightsources(XElement xenv, Scientrace.Object3dEnvironment env) {
		foreach (XElement xlight in xenv.Elements("LightSource")) {
			this.parseXLight(xlight, env);
			}
		if (env.lightsources.Count < 1) {
			Console.WriteLine("WARNING: No lightsource has been defined");
			}
		}
	
	public Scientrace.LightSource parseXLight(XElement xlight, Scientrace.Object3dEnvironment env) {
		Scientrace.LightSource retlight = null;
		string lightclass;
		try {
			lightclass = xlight.Attribute("Class").Value;
			} catch {
			throw new XMLException("Light class type not specified");
			}
		
//		XMLTraceModifierParser xtmp = new XMLTraceModifierParser(xlight, this.X);
		XMLTraceModifierParser xtmp = new XMLTraceModifierParser();
		ShadowLightSourceConstruct slsc = new ShadowLightSourceConstruct();

		switch (lightclass) {
			case "SingleRay":
				List<Scientrace.UniformTraceModifier> utms = xtmp.getModifiers(xlight);
				retlight = this.setSingleRayFromXData(xlight, utms, env);
				break;
			//case "CustomTraces":
				//retlight = this.setCustomTracesLightFromXData(xlight, env);
				//break;
		    case "TestLight":
		        //Console.WriteLine("TESTLightsource");
		        retlight = this.setTestLightFromXData(xlight, env);
		        break;
		    case "RandomCylinder":
		        Console.WriteLine("Random Cylinder Lightsource");
				retlight = this.setRandomCylinderLightFromXData(xlight, env);
		        break;
//		    case "Spiral": 
//			case "RandomSquare":
		    default:
				// No old style LightSource parsing class found? Good, let's hope there's a prettier ShadowClass constructor to do this.
				if (slsc.getClass(lightclass)!=null)  {
					retlight = slsc.constructLightSource(xlight, env);
		        	break;
					} 
				else //damnit, there wasn't a ShadowClass constructor either.
					throw new XMLException("unknown light class type");
			}
		this.addLightSourceTag(retlight, xlight);
		/* Generic (not lightsource-subclass specific) behaviour */
		retlight.max_interactions = this.X.getXInt(xlight.Attribute("MaxInteractions"), 15);
		//this.lightsource.minimum_intensity = this.X.getXDouble(xlight.Attribute("MinIntensity"), 0.10);
			//line above fails for sources with < 1 intensities.
		return retlight;
		}


		public Scientrace.LightSource setTestLightFromXData(XElement xlight,
												Scientrace.Object3dEnvironment env) {
			Scientrace.LightSource retLight = new Scientrace.TestLight(env, Scientrace.DistributionPattern.RandomUniformDensityCircle, //ProjectedSpiralGrid, 
				Scientrace.SpatialDistribution.UniformProjections, new Scientrace.NonzeroVector(1,0,0),
				2*Math.PI/180, 3);
			retLight.minimum_intensity_fraction = 0.2;
			return retLight;
			
			}

		public void addLightSourceTag(Scientrace.LightSource anObject, XElement xel) {
			XAttribute tag = xel.Attribute("Tag");
			if (tag != null) {
				anObject.tag = tag.Value;
				}
			}

		public Scientrace.LightSource setCustomTracesLightFromXData(XElement xlight,
												Scientrace.Object3dEnvironment env) {
			List <Scientrace.Trace> customtraces = new List<Scientrace.Trace>();
			string tag = "";
			double minintensity = this.X.getXDouble(xlight.Attribute("MinIntensity"), 0.01); //default minimum intensity for tracing set to 1%
			foreach (XElement xtrace in xlight.Elements("Trace")) {
				double wavelength = this.X.getXDouble(xtrace, "Wavelength", 
										(this.X.getXDouble(xtrace, "m",
				                            ((this.X.getXDouble(xtrace, "nm", -1))*1E-9))
											)
										);
				if (wavelength <= 0) {
					throw new XMLException("Error parsing wavelength from CustomTraces Lightsource. XML Source: \n"+
						xtrace.ToString());
					}
				double intensity = this.X.getXDouble(xtrace, "Intensity", 1);

				tag = this.X.getXStringByAttribute(xtrace, "Tag");
				Scientrace.Location loc = this.X.getXLocation(xtrace, "Location");
				Scientrace.UnitVector dir = this.X.getXNzVectorByName(xtrace, "Direction").toUnitVector();
				customtraces.Add(
				                 new Scientrace.Trace(wavelength, null, new Scientrace.Line(loc, dir), 
				                                      env, intensity, intensity)
				                 );
				}
			Scientrace.LightSource retlight = new Scientrace.CustomTracesLightSource(env, customtraces);
			if (tag != "") {
				retlight.tag = tag;
				}
			retlight.minimum_intensity_fraction = minintensity;			
				
			return retlight;
			}

	/* Classes supporthing the creation of the lightsource */	
		public Scientrace.LightSource setRandomCylinderLightFromXData(XElement xlight, 
												Scientrace.Object3dEnvironment env) {
			//Reading LightSource parameters from XML-element
			
			/* "RandomCylinder"
			 * d class.radius, d distance, i beamcount, i maxinteractions, d minintensity
			 * str spectrum, vec location, vec direction */
			double radius = this.X.getXDouble(xlight.Attribute("Radius"));
				if (radius <= 0) {throw new ArgumentOutOfRangeException("Radius "+radius+" out of range");}
			double distance = this.X.getXDouble(xlight.Attribute("Distance"), 0);
			int beamcount = this.X.getXInt(xlight.Attribute("RayCount"), this.X.getXInt(xlight.Attribute("BeamCount")));
			//int maxinteractions = this.X.getXInt(xlight.Attribute("MaxInteractions"), 8); //default value max_interactions -> 8
			double minintensity = this.X.getXDouble(xlight.Attribute("MinIntensity"), 0.01); //default minimum intensity for tracing set to 1%
			
			Scientrace.Vector light_direction = this.X.getXVector(xlight.Element("Direction"));
			Scientrace.Location centerloc = this.X.getXVector(xlight.Element("Location")).toLocation();
			Scientrace.Location locoffset = (light_direction.negative()*distance).toLocation(); //distance cm above surface
			Scientrace.Location loc = locoffset + centerloc;
				XMLSpectrumParser xsp = new XMLSpectrumParser();
			Scientrace.LightSpectrum spectrum = xsp.parseLightSpectrum(xlight.Element("Spectrum"));
				
			Scientrace.LightSource retlight = new Scientrace.RandomCircleLightSource(env, 
				loc, light_direction.tryToUnitVector(),
				new Scientrace.Plane(new Scientrace.Location(0,0,0),new Scientrace.NonzeroVector(1, 0, 0), new Scientrace.NonzeroVector(0,0,1)), 
				radius, beamcount, spectrum);
			//retlight.max_interactions = maxinteractions;
			retlight.minimum_intensity_fraction = minintensity;
			return retlight;
			}
		
		
		public Scientrace.SingleRaySource setSingleRayFromXData(XElement xlight, List<Scientrace.UniformTraceModifier> utms,
				Scientrace.Object3dEnvironment env) {
			double distance = this.X.getXDouble(xlight.Attribute("Distance"), 0);
			int beamcount = this.X.getXInt(xlight.Attribute("RayCount"), this.X.getXInt(xlight.Attribute("BeamCount")));
			double minintensity = this.X.getXDouble(xlight, "MinIntensity", 0.01); //default minimum intensity for tracing set to 1%
			
			Scientrace.NonzeroVector light_direction = this.X.getXNzVector(xlight.Element("Direction"));
			Scientrace.Location centerloc = this.X.getXVector(xlight.Element("Location")).toLocation();
			Scientrace.Location locoffset = (light_direction.toVector().negative()*distance).toLocation(); //distance cm above surface
			Scientrace.Location loc = locoffset + centerloc;
				XMLSpectrumParser xsp = new XMLSpectrumParser();
			Scientrace.LightSpectrum spectrum = xsp.parseLightSpectrum(xlight.Element("Spectrum"));
			if (spectrum==null) { throw new Exception("LightSpectrum "+xlight.Element("Spectrum").ToString()+" unknown"); }

			Scientrace.SingleRaySource retlight = new Scientrace.SingleRaySource(new Scientrace.Line(loc, light_direction.toUnitVector()), 
				beamcount, spectrum, env); 

			retlight.minimum_intensity_fraction = minintensity;
			retlight.lightsource_modifiers.AddRange(utms);
			retlight.init();
			return retlight; 
			}
		
		
		
		
		public Scientrace.SpiralLightSource setSpiralLightFromXData(XElement xlight, List<Scientrace.UniformTraceModifier> utms,
				Scientrace.Object3dEnvironment env) {
			/* "Spiral"
			 * d class.radius, d class.loops, d distance, i beamcount, i maxinteractions, d minintensity
			 * i modulomultiplier, str spectrum, vec location, vec direction */
			double radius = this.X.getXDouble(xlight.Attribute("Radius"));
				if (radius < 0) {throw new ArgumentOutOfRangeException("Radius "+radius+" out of range");}
			double distance = this.X.getXDouble(xlight.Attribute("Distance"), 0);
			int beamcount = this.X.getXInt(xlight.Attribute("RayCount"), this.X.getXInt(xlight.Attribute("BeamCount")));
			//int maxinteractions = this.X.getXInt(xlight, "MaxInteractions", 8); //default value max_interactions -> 8
			double minintensity = this.X.getXDouble(xlight, "MinIntensity", 0.2); //default minimum intensity for tracing set to 1%
			//Spiralspecific
			
			double loops = this.X.getXDouble(xlight, "Loops", -1);
			if (loops == -1) {
				loops = 1.0154 * Math.Pow(Math.PI*2*(1-Math.Sqrt(((double)beamcount - 1) / (double)beamcount)), -0.5);
				Console.WriteLine("Number of loops for "+beamcount+" beams set to: {"+loops.ToString("#,0.000")+"}.");
				}
			
			Scientrace.NonzeroVector light_direction = this.X.getXNzVector(xlight.Element("Direction"));
			Scientrace.NonzeroVector spiral_axis = this.X.getXNzVector(xlight.Element("SpiralAxis"));			
			Scientrace.Location centerloc = this.X.getXVector(xlight.Element("Location")).toLocation();
			/*Scientrace.Location locoffset = (light_direction.toVector().negative()*distance).toLocation(); //distance cm above surface
			Scientrace.Location loc = locoffset + centerloc; */
				XMLSpectrumParser xsp = new XMLSpectrumParser();
			Scientrace.LightSpectrum spectrum = xsp.parseLightSpectrum(xlight.Element("Spectrum"));
			if (spectrum==null) { throw new Exception("LightSpectrum "+xlight.Element("Spectrum").ToString()+" unknown"); }

			double divangle = 0;
			double divdistance = 0;
			Scientrace.NonzeroVector divpinvec = null; //=  new Scientrace.NonzeroVector(0,1,0);
			int divsteps = 1;
			bool divincludebase = true;
			this.getDivergence(xlight, ref divangle, ref divpinvec, ref divdistance, ref divincludebase, ref divsteps);
			if (divangle > 0) {
				if (divpinvec.crossProduct(light_direction).length == 0) {
					throw new ArgumentOutOfRangeException("divpinvec", "Divergence Pinvector ("+divpinvec.trico()+") is parallel to light directon ("+
						light_direction.trico()+")");
					}			
				}
			
			// INSERT OPERATION THAT MAKES BOTH SPIRAL PLANE VECTORS ORTHOGONAL TO DIRECTION
			Scientrace.Plane spiralplane = Scientrace.Plane.newPlaneOrthogonalTo(centerloc, spiral_axis);
			//Scientrace.NonzeroVector direction = light_direction.tryToUnitVector();


			Scientrace.SpiralLightSource retlight = new Scientrace.SpiralLightSource(env, centerloc, light_direction.toUnitVector(),
				spiralplane, beamcount, radius, loops, distance, spectrum); 

						
/* BEFORE REMOVAL OF DISPERSION_STEPS			Scientrace.LightSource retlight = new Scientrace.SpiralLightSource(env, loc, light_direction.toUnitVector(),
				spiralplane, beamcount, radius, loops, spectrum, 
				divangle, divpinvec, divdistance, divincludebase, divsteps);  */
			//retlight.max_interactions = maxinteractions;
			retlight.minimum_intensity_fraction = minintensity;
			retlight.lightsource_modifiers.AddRange(utms);
			//retlight.createStartTraces();
			return retlight; 
			}


		public void getDivergence(XElement xlight
		, ref double divangle, ref Scientrace.NonzeroVector divpinvec, ref double divdistance
		, ref bool divIncludeBase, ref int divsteps                          ) {
			if (xlight.Element("Divergence") == null) {
				return;
				}
			XElement xdivr = xlight.Element("Divergence");
			divangle = this.X.getXAngleByName(xdivr, "Angle"); //getXAngle(xdivr.Element("Angle"));
			divpinvec = this.X.getXNzVector(xdivr.Element("PinVector"));
			divdistance = this.X.getXDouble(xdivr, "Distance", 0);
			divIncludeBase = this.X.getXBool(xdivr.Attribute("IncludeBase"), true);
			divsteps = this.X.getXInt(xdivr, "Steps", 1);
			//Console.WriteLine("DIVERGENCE STEPS: "+divsteps);
			}



		
	}}