// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections.Generic;
using System.Text;

namespace Scientrace {


/// <summary>
/// In order to keep things properly organised, these methods generating image data have been grouped in this separate file.
/// </summary>
public partial class TraceJournal {


		
	public string colourLegend(PDPSource pdpSource) {
		switch (pdpSource) {
			case PDPSource.Wavelength: return this.wavelengthLegend();
			case PDPSource.AngleWheel: return this.angleWheelLegend();
			}
		throw new Exception("Unknown PDPSource: "+pdpSource);
		}


	public string wavelengthLegend() {
		return "<!-- inline wavelengthlegend not yet implemented -->";
		}

	public string angleWheelLegend() {
		double wheelRadius = 0.1;
		double wheelcx = 1.145;
		double wheelcy = 1.145;
		StringBuilder retstrb = new StringBuilder("<!-- AngleWheel legend -->");
		int zstepcount = 10;		
		double zstep = Math.PI/(2*zstepcount);
		int astepcount = 24;
		double astep = 2*Math.PI/astepcount;
		for (double zenith = zstep*0.5; zenith <= Math.PI/2; zenith = zenith+zstep) {
		//double zenith = Math.PI/2;
			for (double azimuth = -(astep*0.5); azimuth < Math.PI*2-(astep*0.5); azimuth = azimuth+astep) {
				//Arc start coordinates
				double sx = wheelRadius*Math.Cos(azimuth);
				double sy = wheelRadius*-Math.Sin(azimuth);
				//Arc end coordinates
				double ex = wheelRadius*Math.Cos(azimuth+astep);
				double ey = wheelRadius*-Math.Sin(azimuth+astep);
				//Relatieve coordinates
				double dx = ex-sx;
				double dy = ey-sy;
				double cfraction = zenith*2/Math.PI;
//				Console.WriteLine(" dx:"+dx+" dy:"+dy+" sx:"+sx+" sy:"+sy+" ex:"+ex+" ey:"+ey+ " cf:"+cfraction);
				string id = "z"+cfraction+"a"+(azimuth/(2*Math.PI));
//				retstrb.Append("<path d=\"m "+(wheelcx+(sx*cfraction))+","+(wheelcy+(sy*cfraction))+" a "+(cfraction*wheelRadius)+","+(cfraction*wheelRadius)+" 0 0 0 "+(cfraction*dx)+","+(cfraction*dy)+"\" id=\""+id+"\" style=\"stroke: "+this.angleToHTML(azimuth+(astep*0.5), cfraction)+"; stroke-width:"+(wheelRadius/(zstepcount))+"; fill:none;\" />\n");
				retstrb.Append("<path d=\"m "+String.Format("{0:0.######}",wheelcx+(sx*cfraction))+","+String.Format("{0:0.######}",wheelcy+(sy*cfraction))+" a "+String.Format("{0:0.######}",cfraction*wheelRadius)+","+String.Format("{0:0.######}",cfraction*wheelRadius)+" 0 0 0 "+String.Format("{0:0.######}",cfraction*dx)+","+String.Format("{0:0.######}",cfraction*dy)+"\" id=\""+id+"\" style=\"stroke: "+this.angleToHTML(azimuth+(astep*0.5), cfraction)+"; stroke-width:"+String.Format("{0:0.#####}",wheelRadius/(zstepcount))+"; fill:none;\" />\n");
				}
			}
		return retstrb.ToString();
		}


	public string getPhotonColourForPDPSource(Scientrace.Spot casualtySpot, PDPSource pdpSource, PhysicalObject3d physObj) {
		switch (pdpSource) {
			case PDPSource.Wavelength: return this.wavelengthToRGB(casualtySpot.trace.wavelenght);
			case PDPSource.AngleWheel: return this.angleToRGB(casualtySpot, physObj);
			}
		throw new Exception("Unknown PDPSource: "+pdpSource);
		}
		
	public string angleToRGB(Scientrace.Spot aCasualty, PhysicalObject3d physObj) {
		UnitVector inAngle = aCasualty.trace.traceline.direction;
		Scientrace.Parallelogram pg = physObj.getDistributionSurface();
		Scientrace.NonzeroVector norm = physObj.getSurfaceNormal();
		
		UnitVector y = pg.plane.v.toUnitVector();
		UnitVector x = null;
		try {
			x = norm.crossProduct(y).tryToUnitVector();
			} catch { throw new ZeroNonzeroVectorException("The vertical vector (plane.v) is parallel to the unit normal.");
			}
		double dx = x.dotProduct(inAngle);
		double dy = y.dotProduct(inAngle);
		
		double lengthFraction = Math.Sqrt((dx*dx)+(dy*dy));
		//Console.WriteLine("Length: "+lengthFraction+ " vs: "+((x*dx)+(y*dy)).length);
		
		double angle = Math.Atan2(dy, dx);
		
		return this.angleToHTML(angle, 2*Math.Asin(lengthFraction)/Math.PI);
		
		//physObj.getSurfaceNormal()
		//aCasualty.object3d.
//		throw new Exception("Not yet implemented");
		}


	public string angleToHTML(double angle, double colourfraction) {
		double wheelFraction = (1+(angle / (Math.PI*2)))%1;
		double red, green, blue;
		int wheelSegment = Convert.ToInt32(Math.Floor(wheelFraction*3));
		switch (wheelSegment) {
			case 0:
				green = (wheelFraction*3)-0;
				red = 1 - green;
				blue = 0;
				break;
			case 1:
				blue = (wheelFraction*3)-1;
				green = 1 - blue;
				red = 0;
				break;
			case 2:
				red = (wheelFraction*3)-2;
				blue = 1 - red;
				green = 0;
				break;
			default:
				throw new ArgumentOutOfRangeException("wheelFraction ("+wheelFraction+") should be in between 0 and 1.");
			}
		double normalized_cfrac = Math.Cos(Math.PI*(1-colourfraction)/2);
		return RGBColor.rgbToHtml(normalized_cfrac*red, normalized_cfrac*green, normalized_cfrac*blue);
		}


	/// <summary>
	/// Returns the RED fraction of the RGB colour for a photon with the given wavelenght.
	/// Note that the wavelength is given in METERS, so 600nm will be 600E-9m.
	/// </summary>
	/// <returns>
	/// A fraction (0 - 1.0), where 1.0 is fully red of course.
	/// </returns>
	/// <param name='wlmeters'>
	/// The wavelength (double) in meters (NOT nanometers)
	/// </param>
	public double getRedRGB(double wlmeters) {
		if(wlmeters >= 350E-9 && wlmeters < 440E-9)
			return (440E-9 - wlmeters) / (440E-9 - 350E-9);
		if (wlmeters >= 580E-9 && wlmeters < 780E-9)
			return 1.0;
		if (wlmeters >780E-9)
			return Math.Min((2000E-9-wlmeters)/1220E-9*TraceJournal.MAX_GRAY_FRACTION,1);
		return 0.0;
		}
		
	public double getGreenRGB(double wlmeters) {
		if (wlmeters >= 440E-9 && wlmeters <= 490E-9)
			return (wlmeters - 440E-9) / (490E-9 - 440E-9);
		if(wlmeters >= 490E-9 && wlmeters <= 580E-9)
			return 1.0;	
		if(wlmeters >= 580E-9 && wlmeters < 645E-9)
			return (645E-9 - wlmeters) / (645E-9 - 580E-9);			
		if (wlmeters >780E-9)
			return Math.Min((2000E-9-wlmeters)/1220E-9*TraceJournal.MAX_GRAY_FRACTION,1);
			//return Math.Min((wlmeters - 780E-9)*1000E9*TraceJournal.MAX_GRAY_FRACTION,1);
		return 0.0;
		}
		
	public double getBlueRGB(double wlmeters) {
		if (wlmeters < 490E-9)
			return 1.0;
		if(wlmeters >= 490E-9 && wlmeters < 510E-9)
			return (510E-9 - wlmeters) / (510E-9 - 490E-9);
		if (wlmeters >780E-9)
			return Math.Min((2000E-9-wlmeters)/1220E-9*TraceJournal.MAX_GRAY_FRACTION,1);
			//return Math.Min((wlmeters - 780E-9)*1000E9*TraceJournal.MAX_GRAY_FRACTION,1);
		return 0.0;
		}		


	public string wavelengthToRGB(double wlmeters) {
		//Constructs an RGB hex colour string based on a wavelength in meters (range 350E-9 - ~1800E-9 [IR] )
		return RGBColor.rgbToHtml(this.getRedRGB(wlmeters), this.getGreenRGB(wlmeters), this.getBlueRGB(wlmeters));
		/*return "#"
			+this.twoDigitHex(this.getRedRGB(wlmeters))
			+this.twoDigitHex(this.getGreenRGB(wlmeters))
			+this.twoDigitHex(this.getBlueRGB(wlmeters));*/
		}

	public void writeX3DTraces(StringBuilder retsb) {
		string fromColour = "1 0 0";
		string toColour = "0 1 1";
		foreach (Scientrace.Trace trace in this.traces) {

			if (this.x3dWavelengthColourLines) {
				fromColour = this.getRedRGB(trace.wavelenght)*0.3+" "+
							this.getGreenRGB(trace.wavelenght)*0.3+" "+
							this.getBlueRGB(trace.wavelenght)*0.3;
				toColour = this.getRedRGB(trace.wavelenght)+" "+
							this.getGreenRGB(trace.wavelenght)+" "+
							this.getBlueRGB(trace.wavelenght);
				}
			//Console.WriteLine(trace.intensityFraction()+" squared becomes "+Math.Sqrt(trace.intensityFraction()));
			//Console.WriteLine("Writing trace "+trace.ToString()+" to X3D");
			retsb.Append(@"	<Shape>
		<LineSet vertexCount='2'>
			<Coordinate point='
"+trace.traceline.startingpoint.tricon()+trace.endloc.tricon()+@"
			'/>
		<ColorRGBA color='"+fromColour+" "+/*use Sqrt functions to see "anything" at all */Math.Sqrt(trace.intensityFraction())+@" "+toColour+" "+Math.Sqrt(trace.intensityFraction())+@"' />	
		</LineSet>
	</Shape>");			
		}
	}

	public void writeCasualties(Scientrace.Object3dEnvironment env, StringBuilder retsb) {
		//Console.WriteLine("iterating casualties");
		foreach (Scientrace.Spot casualty in this.casualties) {
			retsb.Append("<!-- start of casualty-->"+X3DGridPoint.x3D_Sphere(casualty.loc,env.radius/1250, "0.5 0 0", casualty.intensity)+"<!-- end of casualty-->");
		}
	}
		
		
	public void writeX3DSpots(Scientrace.Object3dEnvironment env, StringBuilder retsb) {
		//Console.WriteLine("iterating spots");
		foreach (Scientrace.Spot spot in this.spots) {
				//Console.WriteLine("Writing SPOT "+spot.ToString()+" to X3D");
				//Scientrace.X3DGridPoint x3dgp = new Scientrace.X3DGridPoint(spot.loc, null, null);
				retsb.Append(X3DGridPoint.x3D_Transparant_Sphere(spot.loc,((double)env.radius)/1250, spot.intensity)+"<!--spot-->");
/*			retsb.Append(@"	<Shape>
		<LineSet vertexCount='2'>
			<Coordinate point='
"+trace.traceline.startingpoint.tricon()+trace.endloc.tricon()+@"
			'/>
			<Color color='1 0 0 0 1 1' />
		</LineSet>
	</Shape>");			*/
		}
	}

	public void writeX3DAngles(Scientrace.Object3dEnvironment env, StringBuilder retsb) {
		Scientrace.Location tloc;
		foreach (Scientrace.Angle angle in this.angles) {
			tloc = angle.getLocation();
			Scientrace.X3DGridPoint x3dgp = new Scientrace.X3DGridPoint(env, tloc,
												tloc+angle.intersection.enter.flatshape.plane.u.toUnitVector().toLocation()*(env.radius/100),
												tloc+angle.intersection.enter.flatshape.plane.v.toUnitVector().toLocation()*(env.radius/100));
			//Console.WriteLine("Writing spot "+angle.ToString()+" to X3D");
			if (this.drawInteractionPlanes) {
				retsb.Append(x3dgp.exportX3DnosphereRGB("1 1 0"));
				}
			if (this.drawInteractionNormals) {
				retsb.Append(X3DGridPoint.get_RGBA_Line_XML(tloc, tloc+angle.intersection.enter.flatshape.plane.getNorm().negative()*0.125, "1 0 1 0.4"));
				}
			retsb.Append(@"
<Transform scale='"+(env.radius/1250).ToString()+" "+(env.radius/1250).ToString()+" "+(env.radius/1250).ToString()+@"' translation='"+angle.getLocation().trico()+@"'>
<Shape>
<Sphere /> 
<Appearance>
<Material emissiveColor='0 1 0' transparency='"+(1-angle.fromTrace.intensity)+@"' /><!--angle-->
</Appearance> 
</Shape>
</Transform> ");
		}
	}



	public string spotDescriptor(Scientrace.Spot aSpot, Scientrace.PhysicalObject3d anObject, bool bracket) {
		string retstr = "intensity:"+String.Format("{0:#.##E+00}",aSpot.intensity)+ //display absolute intensity
				"("+(String.Format("{0:0.00}",aSpot.intensityFraction*100))+"%)"+ //display intensity as a percentage
					", wavelength:"+String.Format("{0:00.0}",aSpot.trace.wavelenght*1E9)+"nm"+
					", "+aSpot.trace.ToCompactString()+
					//", #"+aSpot.GetHashCode()+ //hash code for spot
					"\nroot: "+aSpot.trace.getParentRoot().ToCompactString()
					;
		//Console.WriteLine("wl = "+aSpot.trace.wavelenght.ToString());

		return bracket ? "( "+retstr+" )" : retstr;
		}


	public string exportSVGOpening(Scientrace.PhysicalObject3d anObject) {
		double[] vb = anObject.getViewBorders();
		//double ToTot = ((vb[2]-vb[0])+(vb[3]-vb[1]))/100;
		double ToX = anObject.svgxsize/(vb[2]-vb[0]);
		double ToY = anObject.svgysize/(vb[3]-vb[1]);
		double ToTot = Math.Sqrt(Math.Pow(ToX,2)+Math.Pow(ToY,2));
		string strokewidth = (ToTot*((vb[2]-vb[0])+(vb[3]-vb[1]))/800.0).ToString("#.##############"); //0.25% linewidth	
		StringBuilder retstr = new StringBuilder();
		retstr.Append(@"<?xml version='1.0' standalone='no'?>
<!DOCTYPE svg PUBLIC '-//W3C//DTD SVG 1.1//EN'
'http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd'>


<svg width='1600' height='1200' version='1.1' viewBox='"+anObject.getRelMarginViewBox(0.1,0.1,0.25,0.25)+@"'
xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink'>
<rect width='100%' height='100%' fill='white' />
<desc>Drawing center crosshair</desc>");
		if (this.svggrid) { // GRID
			retstr.Append(@"<g stroke='lightgray'>
<line x1='"+vb[0]+"' y1='"+(vb[1]+((vb[3]-vb[1])/4.0))+"' x2='"+vb[2]+"' y2='"+(vb[1]+((vb[3]-vb[1])/4.0))+"' stroke-width='"+strokewidth+@"'  />
<line x1='"+vb[0]+"' y1='"+(vb[1]+((vb[3]-vb[1])*3.0/4.0))+"' x2='"+vb[2]+"' y2='"+(vb[1]+((vb[3]-vb[1])*3.0/4.0))+"' stroke-width='"+strokewidth+@"'  />
<line x1='"+(vb[0]+((vb[2]-vb[0])/4.0))+"' y1='"+vb[1]+"' x2='"+(vb[0]+((vb[2]-vb[0])/4.0))+"' y2='"+vb[3]+"' stroke-width='"+strokewidth+@"'  />
<line x1='"+(vb[0]+((vb[2]-vb[0])*3.0/4.0))+"' y1='"+vb[1]+"' x2='"+(vb[0]+((vb[2]-vb[0])*3.0/4.0))+"' y2='"+vb[3]+"' stroke-width='"+strokewidth+@"'  />
</g>
<g stroke='gray'>
<line x1='"+vb[0]+"' y1='"+(vb[1]+((vb[3]-vb[1])/2.0))+"' x2='"+vb[2]+"' y2='"+(vb[1]+((vb[3]-vb[1])/2.0))+"' stroke-width='"+strokewidth+@"'  />
<line x1='"+(vb[0]+((vb[2]-vb[0])/2.0)+"' y1='"+vb[1])+"' x2='"+(vb[0]+((vb[2]-vb[0])/2.0))+"' y2='"+vb[3]+"' stroke-width='"+strokewidth+@"'  />
</g>"); } // END GRID CODE

		retstr.Append(@"<desc>Drawing borders</desc>
<rect x='"+vb[0]+"' y='"+vb[1]+"' width='"+(vb[2]-vb[0])+"' height='"+(vb[3]-vb[1])+@"'
        fill='none' stroke='black' stroke-width='"+strokewidth+@"'/>

<desc>Spots on the surface:</desc>");
		return retstr.ToString();
		}

	public string exportSVG(Scientrace.PhysicalObject3d anObject, Scientrace.SurfaceSide side, Scientrace.PDPSource pdpSource) {
		
		//Scientrace.Parallelogram surface = anObject.getDistributionSurface();
		//this.spotsize = Math.Sqrt(Math.Pow(surface.u.length,2)+Math.Pow(surface.v.length,2))*this.spotdiagonalfraction;
		this.spotsize = Math.Sqrt(Math.Pow(anObject.svgxsize,2)+Math.Pow(anObject.svgysize,2))*this.spotdiagonalfraction;
		StringBuilder retstr = new StringBuilder();
		retstr.Append(this.exportSVGOpening(anObject));
		
		Scientrace.Location loc2d;

		string centerSpots = "<!-- END OF SPOTS CENTERS -->";
		string pol_lines = "<!-- END OF POLARISATION LINES -->";
		//Console.WriteLine("Now writing "+this.spots.Count+" spots");
		//int icount = 0;
		foreach(Scientrace.Spot casualty in this.spots) {

				if ((casualty.object3d == anObject) 
					&& (anObject.directedTowardsObjectside(side, casualty.trace))) {
				loc2d = anObject.getSVG2DLoc(casualty.loc);


//				double pol_fac_1 = Math.Pow(casualty.pol_vec_1.length,2)*1.0;//*casualty.pol_vec_1.length * casualty.intensity*3;
//				double pol_fac_2 = Math.Pow(casualty.pol_vec_2.length,2)*1.0;//*casualty.pol_vec_2.length * casualty.intensity*3;

				double pol_fac_1 = Math.Pow(casualty.pol_vec_1.length,1)*1.00*Math.Sqrt(casualty.intensity);//*casualty.pol_vec_1.length * casualty.intensity*3;
				double pol_fac_2 = Math.Pow(casualty.pol_vec_2.length,1)*1.00*Math.Sqrt(casualty.intensity);//*casualty.pol_vec_2.length * casualty.intensity*3;

				Scientrace.Vector pol_vec_1_2d = (anObject.get2DLoc(
						casualty.pol_vec_1.projectOnPlaneWithNormal(anObject.getSurfaceNormal()).toLocation()
								)-anObject.get2DLoc(Location.ZeroLoc())).normaliseIfNotZero() * pol_fac_1;
				Scientrace.Vector pol_vec_2_2d = (anObject.get2DLoc(
						casualty.pol_vec_2.projectOnPlaneWithNormal(anObject.getSurfaceNormal()).toLocation()
								)-anObject.get2DLoc(Location.ZeroLoc())).normaliseIfNotZero() * pol_fac_2;

				if (this.svg_export_photoncloud) {
					retstr.Append(@"<g>
<circle cx='"+loc2d.x+"' cy='"+(anObject.svgysize-loc2d.y)+"' r='"+(casualty.intensity*this.spotsize)+"' style='"+
"fill:"+this.getPhotonColourForPDPSource(casualty, pdpSource, anObject)
+@";fill-opacity:0.3;stroke:none'>
<title><!-- Tooltip -->"+this.spotDescriptor(casualty, anObject,true)+@"</title>
</circle>
</g>");
				} // end if draw photoncloud (to create the frogg-egg kinda image)
				
				centerSpots = (@"<g>
<circle cx='"+loc2d.x+"' cy='"+(anObject.svgysize-loc2d.y)+"' r='"+(casualty.intensity*this.spotsize/(this.svg_export_photoncloud?4:1))+"' style='"+
"fill:"+this.getPhotonColourForPDPSource(casualty, pdpSource, anObject)
+@";fill-opacity:8;stroke:none'>
<title><!-- Tooltip -->"+this.spotDescriptor(casualty, anObject,false)+@"</title>
</circle>
</g>") + centerSpots;
				pol_lines = (@"
 <line x1='"+(loc2d.x-(pol_vec_1_2d.x*this.spotsize))+"' y1='"+(anObject.svgysize-(loc2d.y-(pol_vec_1_2d.y*this.spotsize)))+"' x2='"+(loc2d.x+(pol_vec_1_2d.x*this.spotsize))+"' y2='"+(anObject.svgysize-(loc2d.y+(pol_vec_1_2d.y*this.spotsize)))+"' style='stroke:rgb(0,96,192);stroke-width:"+(casualty.intensity*this.spotsize/5)+@"' />
 <line x1='"+(loc2d.x-(pol_vec_2_2d.x*this.spotsize))+"' y1='"+(anObject.svgysize-(loc2d.y-(pol_vec_2_2d.y*this.spotsize)))+"' x2='"+(loc2d.x+(pol_vec_2_2d.x*this.spotsize))+"' y2='"+(anObject.svgysize-(loc2d.y+(pol_vec_2_2d.y*this.spotsize)))+"' style='stroke:rgb(192,96,0);stroke-width:"+(casualty.intensity*this.spotsize/5)+@"' />
 <line x1='"+(loc2d.x-(pol_vec_1_2d.x*this.spotsize))+"' y1='"+(anObject.svgysize-(loc2d.y-(pol_vec_1_2d.y*this.spotsize)))+"' x2='"+(loc2d.x+(pol_vec_1_2d.x*this.spotsize))+"' y2='"+(anObject.svgysize-(loc2d.y+(pol_vec_1_2d.y*this.spotsize)))+"' style='stroke:rgb(256,256,256);stroke-width:"+(casualty.intensity*this.spotsize/8)+@"' />
 <line x1='"+(loc2d.x-(pol_vec_2_2d.x*this.spotsize))+"' y1='"+(anObject.svgysize-(loc2d.y-(pol_vec_2_2d.y*this.spotsize)))+"' x2='"+(loc2d.x+(pol_vec_2_2d.x*this.spotsize))+"' y2='"+(anObject.svgysize-(loc2d.y+(pol_vec_2_2d.y*this.spotsize)))+"' style='stroke:rgb(256,256,256);stroke-width:"+(casualty.intensity*this.spotsize/8)+@"' />
")+pol_lines;
				} // end condition for spot to be on the correct object and from the proper direction
			}
			centerSpots = "<!-- START OF CENTERSPOTS -->"+centerSpots;
			pol_lines = "<!-- START OF POLARISATION LINES -->"+pol_lines;
			retstr.Append(centerSpots+pol_lines);
			foreach(Scientrace.SurfaceMarker marker in anObject.markers) {
				retstr.Append(marker.exportSVG());
				}	
		if (this.inline_legend)
			retstr.Append(this.colourLegend(pdpSource));
		retstr.Append("</svg> ");
		return retstr.ToString();
		}		
		
	

} }