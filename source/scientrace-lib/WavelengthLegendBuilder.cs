// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.IO;

namespace Scientrace {
public class WavelengthLegendBuilder : Scientrace.LegendBuilder {

	public WavelengthLegendBuilder() {
	}
		
	/// <summary>
	/// The minimal wavelength included in the wavelength legend, value in meters, i.e. 600nm equals 600E-9.
	/// </summary>
	public double min_wavelength = 350E-9;
		
	/// <summary>
	/// The maximal wavelength included in the wavelength legend, value in meters, i.e. 600nm equals 600E-9.
	/// </summary>
	public double max_wavelength = 1850E-9;
	
	/// <summary>
	/// The number of points/spots the legend bar is built with.
	/// </summary>
	public int resolution = 100;
		
	/// <summary>
	/// A margin added to the canvas (and thus the total image) size, based on the height of the used font. Particularly useful
	/// when adding large numbers with the axes.
	/// </summary>
	public double fontSizeBasedMarginLeft = 1;
	/// <summary>
	/// A margin added to the canvas (and thus the total image) size, based on the height of the used font. Particularly useful
	/// when adding large numbers with the axes.
	/// </summary>
	public double fontSizeBasedMarginRight = 3;
	/// <summary>
	/// A margin added to the canvas (and thus the total image) size, based on the height of the used font. Particularly useful
	/// when adding large numbers with the axes.
	/// </summary>
	public double fontSizeBasedMarginTop = 1;
	/// <summary>
	/// A margin added to the canvas (and thus the total image) size, based on the height of the used font. Particularly useful
	/// when adding large numbers with the axes.
	/// </summary>
	public double fontSizeBasedMarginBottom = 5;
		
		
	public Scientrace.NonzeroVector directionBase() {
		return	( this.isHorizontal()	?
									Scientrace.NonzeroVector.x1vector()	:
									Scientrace.NonzeroVector.y1vector() 
				);
		}
		
	public double legendSize() {
		return	( this.isHorizontal()	?
									this.figure_size_width	:
									this.figure_size_height
				);
		}			
		
	public Scientrace.Location relLocationVector(double waveLength) {
		return (this.directionBase().toVector() * 
			((waveLength-this.min_wavelength)/(this.max_wavelength-this.min_wavelength)) * this.legendSize()).toLocation();
		}
		
	public string getSVGHeader() {
		return @"<?xml version='1.0' standalone='no'?>
<!DOCTYPE svg PUBLIC '-//W3C//DTD SVG 1.1//EN' 'http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd'>
";		}
		
	public string getSVGTagOpen(double imagewidth, double imageheight, string viewbox) {
		return @"<svg width='"+imagewidth+"px' height='"+imageheight+"px' version='1.1' viewBox='"+viewbox+
		"' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink'>";
/*		return @"<svg width='"+imagewidth+"px' height='"+imageheight+"px' version='1.1' viewBox='"+
				                 legendSquare.getAbsMarginViewBox(fontSizeBasedMarginLeft,fontSizeBasedMarginTop,
				                                                  fontSizeBasedMarginRight, fontSizeBasedMarginBottom,textheight)+
		"' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink'>";*/
		}
		
	public void writeSVGFile(System.IO.DirectoryInfo path) {
		double min_wl = this.min_wavelength;
		double max_wl = this.max_wavelength;
		int resolution = this.resolution;
		int canvaswidth = this.figure_size_width;
		int canvasheight = this.figure_size_height;
			
		/* base square positioning on the direction. Since spots have to be placed in a line, 
		 * it makes sense to start the axis along which it lies at zero towards the length or height
		 * of the axis. The spots will be placed along other axis, which is therefore centered about
		 * that axis (-0.5*width/length; 0.5*width/length). */
		Scientrace.Location squareloc = ( this.isHorizontal() ?
										(Scientrace.Vector.y1vector()*canvasheight*-0.5).toLocation() :
										(Scientrace.Vector.x1vector()*canvaswidth*-0.5).toLocation() );
			
		Scientrace.Rectangle legendSquare = new Scientrace.Rectangle(null, null, 
				squareloc, 
				(Scientrace.Vector.x1vector()*canvaswidth).tryToNonzeroVector(),
				(Scientrace.Vector.y1vector()*canvasheight).tryToNonzeroVector());
			
		Scientrace.HorizontalGridSurfaceMarker hormarker = new Scientrace.HorizontalGridSurfaceMarker(legendSquare);
			hormarker.minval = min_wl*1E9;
			hormarker.maxval = max_wl*1E9;
			hormarker.xunits = "nm";
		double textheight = hormarker.textheight();
		double imagewidth = legendSquare.viewBoxWidth()+((fontSizeBasedMarginLeft+fontSizeBasedMarginRight)*textheight);
		double imageheight = legendSquare.viewBoxHeight()+((fontSizeBasedMarginTop+fontSizeBasedMarginBottom)*textheight);
			
		//Scientrace.Parallelogram surface = legendSquare.getDistributionSurface();
		//double spotsize = Math.Sqrt(Math.Pow(surface.u.length,2)+Math.Pow(surface.v.length,2))*(0.005);

		double[] vb = legendSquare.getViewBorders();
		//		Console.WriteLine("canvaswidth: "+canvaswidth+ " vb[2]: "+vb[3]);

		string strokewidth = (((vb[2]-vb[0])+(vb[3]-vb[1]))/800.0).ToString("#.##############"); //0.25% linewidth
		string fullfilename = path.FullName+System.IO.Path.DirectorySeparatorChar+"wl_legend.svg";
		//Console.WriteLine("Writing legend to "+fullfilename);
		using (StreamWriter retstr = new StreamWriter(fullfilename)) {
			retstr.WriteLine(this.getSVGHeader());
			retstr.WriteLine(getSVGTagOpen(imagewidth, imageheight, 
				              legendSquare.getAbsMarginViewBox(fontSizeBasedMarginLeft,fontSizeBasedMarginTop,
				                                                  fontSizeBasedMarginRight, fontSizeBasedMarginBottom,textheight)
				              ));

			retstr.WriteLine(@"<desc>Drawing borders</desc>
<rect x='"+vb[0]+"' y='"+vb[1]+"' width='"+vb[2]+"' height='"+vb[3]+@"'
        fill='none' stroke='black' stroke-width='"+strokewidth+"' />");

			retstr.WriteLine(hormarker.exportSVG());
			
			Scientrace.Location loc2d, tloc2d;
			int order = 5;
			string centerSpots = "<!-- END OF SPOTS CENTERS -->";
			double unitfraction = (Math.Pow(2,order)-1);
			double unitheight  = Convert.ToDouble(canvasheight)/unitfraction;
				
			//added "+(stepsize*0.001)" to condition to avoid float-boundary-problems
			for (double wavelength = min_wl; wavelength<=max_wl+(((max_wl-min_wl)/resolution)*0.001); wavelength=wavelength+((max_wl-min_wl)/resolution)) {
				Scientrace.Location loc =  this.relLocationVector(wavelength);
				Scientrace.Spot aSpot = new Scientrace.Spot(loc, legendSquare, 1, 1, null);
				loc2d = legendSquare.get2DLoc(aSpot.loc);
	
				if (loc2d==null) {throw new Exception("ERROR: loc2d created by "+legendSquare.tag+" == null");}
	
				double centerspotsize = (Convert.ToDouble(canvaswidth)/Convert.ToDouble(resolution))*0.5;
				loc2d.y = -centerspotsize+canvasheight;
							//unitheight * (unitfraction-0.5);
/*								(Math.Pow(2.0,order-1) * 0.5) +
								(Math.Pow(2.0,order-1) - 1)
								);*/

				centerSpots = this.spotSVGForWavelength(loc2d, centerspotsize, wavelength, 1)+centerSpots;
						
				for (int iorder = 1; iorder <= order; iorder++) {
					tloc2d = loc2d;
					tloc2d.y = unitheight * (
								(Math.Pow(2.0,iorder-1) * 0.5) +
								(Math.Pow(2.0,iorder-1) - 1)
								);
					retstr.WriteLine(this.spotSVGForWavelength(tloc2d, unitheight*(Math.Pow(2,(iorder-2))), wavelength, 0.25));
					}
				}
			centerSpots = "<!-- START OF CENTERSPOTS-->"+centerSpots;
			retstr.WriteLine(centerSpots);
			retstr.WriteLine("</svg> ");

				//retstr.Close();
				}
			}

		
		public string spotSVGForWavelength(Scientrace.Vector location, double radius, double wavelength, double opacity) {
			Scientrace.TraceJournal tj = TraceJournal.Instance;
			return @"<g>
<circle cx='"+location.x+"' cy='"+location.y+"' r='"+radius+"' style='"+
"fill:"+tj.wavelengthToRGB(wavelength)+";fill-opacity:"+opacity+@";stroke:none'>
<title>"+wavelength+@"nm</title>
</circle>
</g>";
			}
		
}
}

