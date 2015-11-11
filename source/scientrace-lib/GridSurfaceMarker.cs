// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {
public class GridSurfaceMarker : SurfaceMarker {

	public GridSurfaceMarker(Scientrace.PhysicalObject3d parentObject):base(parentObject) {}

	public double markerSizeFraction = 20;
	public double gridSteps = 5;
	public double? minval = null;
	public double? maxval = null;

	public double widthStep() {
		//return this.marksObject.viewBoxWidth()/this.gridSteps;
		return this.marksObject.svgxsize/this.gridSteps;
		}

	public double heightStep() {
		//return this.marksObject.viewBoxHeight()/this.gridSteps;
		return this.marksObject.svgysize/this.gridSteps;
		}
		
	public double markerheight() {
		return this.marksObject.viewBoxWidth()/this.markerSizeFraction;	
		}
		
	public double textheight() {
		return this.marksObject.viewBoxWidth()/this.fontSizeFraction;	
		}

}
}

