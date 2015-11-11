// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {
public class HorizontalGridSurfaceMarker : GridSurfaceMarker {

	public HorizontalGridSurfaceMarker(Scientrace.PhysicalObject3d parentObject):base(parentObject) {}		

		
	public override string exportSVG() {
		double ToX = this.marksObject.svgxsize/(this.marksObject.viewBoxWidth());
		double ToY = this.marksObject.svgysize/(this.marksObject.viewBoxHeight());
		//double ToTot = (ToX+ToY)/2;

		double left, right, bottom, width, y1, y2, strokewidth;
		left = this.marksObject.viewBoxLeft()*ToX;
		right = this.marksObject.viewBoxRight()*ToX;
		bottom = this.marksObject.viewBoxBottom()*ToY;
		width = this.marksObject.viewBoxWidth()*ToX;
		y1 = bottom-(this.markerheight()/2);
		y2 = bottom+(this.markerheight()/2);
		strokewidth = width/500;
		if (this.minval == null) { this.minval = left; }
		if (this.maxval == null) { this.maxval = right; }
		double gridlength = (double)this.maxval - (double)this.minval;
		double surfacelength = right-left;
		double gridfactor = gridlength/surfacelength;
			
		string retstr = "";
		//the *1.000000000001 is to avoid rounding errors which would leave the last grid-index out.
		for (double x = left; x*Math.Sign(this.widthStep())<=right*1.000000000001*Math.Sign(this.widthStep()); x=x+this.widthStep()) {
			double textx = (x-(0.75*this.textheight()));
			double texty = (y2+(this.textheight()*0.8));
			retstr = retstr +"<g stroke='green'><line x1='"+x.ToString()+"' y1='"+y1+"' x2='"+x+"' y2='"+y2+"' stroke-width='"+strokewidth+@"'  /></g>
  <text x='"+textx+"' y='"+texty+"' transform='rotate(30,"+textx+","+texty+")' id='"
					+"horizontalmarker"+x.ToString()+@"' style='font-size:"+this.textheight()+@"px'>
    <tspan>"+(((x-left)*gridfactor)+this.minval)+xunits+@"</tspan>
  </text>
";
				
/*	  <text x='0.16' y='0.207' transform='rotate(45,0.16,0.207)' id='horizontalmarker0.16' style='font-size:0.008px'>
    <tspan>0.16 IS DEZE</tspan>
  </text>*/
						 
			}
		return retstr;
		}
}
}

