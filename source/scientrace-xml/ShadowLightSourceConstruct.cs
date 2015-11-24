using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using ShadowScientrace;

namespace ScientraceXMLParser {
	
public class ShadowLightSourceConstruct : ScientraceXMLAbstractParser{
	
	public ShadowLightSourceConstruct ():base() {
		}


/* TO IMPLEMENT: Class="RandomToSurface"
<LightSource BeamCount="N" Class="RandomToSurface" Seed="1">
	<Surface Class="circle" Radius="r" />
		<Center xyz="LENSXYZ" />
		<Normal xyz="DIRXYZ" />
	</Surface>
</LightSource>

*/

	public Scientrace.LightSource constructLightSource(XElement xel, Scientrace.Object3dEnvironment env) {

		/* //Special property removed at 20151021 
		XElement xspec = xel.Element("Spectrum");
		Scientrace.LightSpectrum aSpectrum = null;
		if (xspec == null) {
			//throw new XMLException("No Spectrum element found for: "+xel.ToString());
			} else {
			XMLSpectrumParser xsp = new XMLSpectrumParser();			
			aSpectrum = xsp.parseLightSpectrum(xspec);
			}
		*/

		//modified at 20150707
		//ShadowScientrace.ShadowLightSource shadowLS = 
		//		new ShadowScientrace.ShadowLightSource(typeof(Scientrace.SpiralLightSource), env, aSpectrum);
			ShadowScientrace.ShadowLightSource shadowLS = 
				new ShadowScientrace.ShadowLightSource(null, env);
		
		this.setClassInfo(xel, shadowLS);
		
		this.setLightSourceConstructorParams(shadowLS, xel);
		this.setLightSourceSettings(shadowLS, xel);

		return shadowLS.factory(0);
		}
		
	
	public void setClassInfo(XElement xel, ShadowScientrace.ShadowLightSource shadowLS) {
		shadowLS.class_name = this.X.getXStringByName(xel, "Class");
		shadowLS.class_type = this.getClass(shadowLS.class_name);
		if (shadowLS.class_type == null)
			throw new XMLException("LightSource Class {"+shadowLS.class_name+"} is not recognized.");
		}
		




	public Type getClass(string class_name) {
		switch (class_name) {
			case "CustomTraces":
				return typeof(Scientrace.CustomTracesLightSource);
				//unreachable break;
			case "Spiral":
				return typeof(Scientrace.SpiralLightSource);
				//unreachable break;
			case "RandomSquare": //Fallthrough
			case "RandomRectangle":
				return typeof(Scientrace.ParallelRandomSquareLightSource);
				//unreachable break;
			case "RandomCircle":
				return typeof(Scientrace.RandomCircleLightSource);
				//unreachable break;
			default:
				return null;
			}	
		}
		
	public void setLightSourceSettings(ShadowScientrace.ShadowLightSource shadowLS, XElement xel) {
		XMLTraceModifierParser xtmp = new XMLTraceModifierParser();
		List<Scientrace.UniformTraceModifier> utms = xtmp.getModifiers(xel);
		shadowLS.arguments.Add("trace_modifiers", utms);
		double minintensity = this.X.getXDouble(xel, "MinIntensity", 0.2); //default minimum intensity for tracing set to 1%
		shadowLS.arguments.Add("minimum_intensity_fraction", minintensity);
		}
		
		
	public void setLightSourceConstructorParams(ShadowScientrace.ShadowLightSource shadowLS, XElement xel) {
		// STRUCTURE
		/* "dictionary name of the parameter"
		 * Description of this parameters
		 * used at: list of light source classes that use this parameter
		 * nullable: yes if this field may be empty
		 */

		
		/* "location"
		 * The location of the light source
		 * used at: all
		 * nullable: no
		 */
		Scientrace.Location location = this.X.getXLocation(xel, "Location", null);
		shadowLS.arguments.Add("location", location);

		/* "width" and "height"
		 * The width of the light source
		 * used at: random square light source
		 * nullable: yes
		 */
		Scientrace.Vector width = this.X.getXLocation(xel, "Width", null);
		Scientrace.Vector height = this.X.getXLocation(xel, "Height", null);
		shadowLS.arguments.Add("width", width);
		shadowLS.arguments.Add("height", height);

		/* "ray count"
		 * The number of traces that will be initiated by the light source, 
		 		some of the rays may be split or absorbed during simulation
		 * Previously called: BeamCount, but renamed to RayCount.
		 * used at: SpiralLightSource
		 * nullable: no
		 */
		int ray_count = this.X.getXInt(xel, "RayCount", this.X.getXInt(xel, "BeamCount", 256));
		shadowLS.arguments.Add("ray_count", ray_count);
	
		/* "loops"
		 * The number of loops the spiral makes
		 * used at: SpiralLightSource
		 * nullable: yes
		 */
		if (this.X.hasAttribute(xel, "Loops")) {
		double? loops = this.X.getXNullDouble(xel, "Loops");
		/* MOVED TO SPIRALLIGHTSOURCE
		if (loops == -1) {
			loops = 1.0154 * Math.Pow(Math.PI*2*(1-Math.Sqrt(((double)ray_count - 1) / (double)ray_count)), -0.5);
			Console.WriteLine("Number of loops for "+ray_count+" beams set to: {"+loops.ToString("#,0.000")+"}.");
			} */
		shadowLS.arguments.Add("loops", loops);
		}
		
		/* "radius"
		 * The radius for any circle shaped irradiating surface
		 * used at: SpiralLightSource
		 * nullable: no
		 */
		double? radius = this.X.getXNullDouble(xel, "Radius");
		if (radius == null) 
			radius = this.X.getXNullDouble(xel, "Diameter")/2;
		if (radius != null && radius < 0) {throw new ArgumentOutOfRangeException("Radius "+radius+" out of range");}
		shadowLS.arguments.Add("radius", radius);

		/* "distance"
		 * In some cases the user might want to describe a surface/orientation where the
		   rays to trace pass, but should they actually start a given distance up front.
		   The distance parameter describes the distance to start "before" the given surface.
		 * used at: SpiralLightSource
		 * nullable: default = 0
		 */
		shadowLS.arguments.Add("distance", this.X.getXDouble(xel, "Distance", 0));

		/* "weighted_intensity"
		 * The intensity of the entire lightsource may be weighted/redefined by setting its intensity.
		 * nullable: yes
		 * used at: all
		 */
		shadowLS.arguments.Add("weighted_intensity", this.X.getXNullDouble(xel, "Intensity"));

		/* "random_seed"
		 * In order to make repreducable results, a randomized may be seeded
		 * used at: RandomSquare
		 * nullable: default = null
		 */
		shadowLS.arguments.Add("random_seed", this.X.getXNullInt(xel, "RandomSeed"));


		/* "direction"
		 * The direction in which the rays leave the (parallel) source
		 * used at: SpiralLightSource, RandomSquare
		 * nullable: no
		 */
		shadowLS.arguments.Add("direction", this.X.getXUnitVectorByName(xel, "Direction", null));
		

		/* "orthogonal to plane in which the "circle of light" is created"
		 * used at: RandomCircleLightSource
		 * nullable: yes
		 */
		shadowLS.arguments.Add("normal", this.X.getXUnitVectorByName(xel, "Normal", null));

			
		/* "orthogonal to plane about which spiral is created"
		 * sued at: SpiralLightSource
		 * nullable: yes
		 */
		Scientrace.NonzeroVector spiral_axis = this.X.getXNzVectorByName(xel, "SpiralAxis", null);
		if (spiral_axis != null) {
			Scientrace.Plane spiralplane = Scientrace.Plane.newPlaneOrthogonalTo(location, spiral_axis);
			shadowLS.arguments.Add("spiral_plane", spiralplane);
			}

		/* "spectrum"
		 * The wavelengths that are irradiated by the source with their relative intensities
		 * used at: all except CustomTraces
		 * 
		 */
		XMLSpectrumParser xsp = new XMLSpectrumParser();
		Scientrace.LightSpectrum spectrum = xsp.parseLightSpectrum(xel.Element("Spectrum"));
		//if (spectrum==null) { throw new Exception("LightSpectrum "+xel.Element("Spectrum").ToString()+" unknown"); }
		shadowLS.arguments.Add("spectrum", spectrum);


		/* "efficiency_characteristics"
		 * If the performance of a cell depends on the angle of incidence or the wavelength of light, it can be defined
		 * in efficiency_characteristics. The default fractions are "1" for both.
		 * used at: all
		 */
		XMLEfficiencyCharacteristicsParser xecp = new XMLEfficiencyCharacteristicsParser();
		Scientrace.OpticalEfficiencyCharacteristics oec = xecp.parseEfficiencyForLightSource(xel);
		shadowLS.arguments.Add("efficiency_characteristics", oec);

		/* "traces_list"
		 * A generic List<Scientrace.Trace> with traces that are manually added to a lightsource.
		 * used at: CustomTraces
		 */
		shadowLS.arguments.Add("traces_list", this.getCustomTracesListFromXData(xel, shadowLS.env));
		}


		public List <Scientrace.Trace> getCustomTracesListFromXData(XElement xlight,
												Scientrace.Object3dEnvironment env) {
			if (!xlight.Elements("Trace").Any())
						return null;
			List <Scientrace.Trace> customtraces = new List<Scientrace.Trace>();
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
				Scientrace.Location loc = this.X.getXLocation(xtrace, "Location");
				Scientrace.UnitVector dir = this.X.getXNzVectorByName(xtrace, "Direction").toUnitVector();
				customtraces.Add(
				                 new Scientrace.Trace(wavelength, null, new Scientrace.Line(loc, dir), 
				                                      env, intensity, intensity)
				                 );
				}
			return customtraces;
			}
		
	}} //end class + namespace