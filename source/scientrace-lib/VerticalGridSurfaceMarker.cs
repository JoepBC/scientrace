// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {
public class VerticalGridSurfaceMarker : GridSurfaceMarker {
		
	public VerticalGridSurfaceMarker(Scientrace.PhysicalObject3d parentObject):base(parentObject) {}		
	
		
	public override string exportSVG() {
//		double ToX = this.marksObject.svgxsize/(this.marksObject.viewBoxWidth());
//		double ToY = this.marksObject.svgysize/(this.marksObject.viewBoxHeight());
		double ToX = 1;
		double ToY = 1;
		double right, top, bottom, height, x1, x2, strokewidth;
		//left = this.marksObject.viewBoxLeft()*ToX;
		right = this.marksObject.viewBoxRight()*ToX;
		//bottom = this.marksObject.viewBoxBottom()*ToY;
		//top = this.marksObject.viewBoxTop()*ToY;
		bottom = this.marksObject.svgysize;
		top = 0;
		//width = this.marksObject.viewBoxWidth()*ToX;
		height = this.marksObject.viewBoxHeight()*ToY;
		x1 = right-(this.markerheight()/2);
		x2 = right+(this.markerheight()/2);
		strokewidth = height/500;
		if (this.minval == null) { this.minval = top; }
		if (this.maxval == null) { this.maxval = bottom; }
		double gridlength = (double)this.maxval - (double)this.minval;
		double surfacelength = bottom-top;
		double gridfactor = gridlength/surfacelength;
			
		string retstr = "";
			//the *1.000000000001 is to avoid rounding errors which would leave the last grid-index out.
		for (double y = top; (y*Math.Sign(this.heightStep()))<=(bottom*1.000000000001*Math.Sign(this.heightStep())); y=y+this.heightStep()) {
			double textx = (x2+(this.textheight()*0.3));
			//double texty = (y-(0.75*this.textheight()));
			double texty = height-y;
			retstr = retstr +"<g stroke='green'><line x1='"+x1+"' y1='"+y+"' x2='"+x2+"' y2='"+y+"' stroke-width='"+strokewidth+@"'  /></g>
  <text x='"+textx+"' y='"+texty+"' transform='rotate(30,"+textx+","+texty+")' id='"
					+"horizontalmarker"+y.ToString()+@"' style='font-size:"+this.textheight()+@"px'>
    <tspan>"+(((y-top)*gridfactor)+this.minval)+yunits+@"</tspan>
  </text>
";
			}
		return retstr;
		}		
		
		
}
}





