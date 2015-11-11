// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;




namespace Scientrace {

[Flags]
/// <summary>
/// A surface, e.g. a solar cell, may have two sides from which only one is relevant to export. Nevertheless
/// it could be interesting to study the other side (the back?) as well "while where at it".
/// </summary>
public enum SurfaceSide {
	Both = 0,
	Front = 1,
	Back = 2,
	Inside = 2,
	}
	

public abstract class PhysicalObject3d : Object3d {

	public double svgxsize = 800;
	public double svgysize = 800;		

		public HashSet<Scientrace.SurfaceSide> surface_sides = new HashSet<SurfaceSide>() {Scientrace.SurfaceSide.Front, Scientrace.SurfaceSide.Back};

	public Scientrace.Location loc;
	public List<SurfaceMarker> markers = new List<SurfaceMarker>();

	public OpticalEfficiencyCharacteristics optical_efficiency = new OpticalEfficiencyCharacteristics();

	public PhysicalObject3d(ShadowScientrace.ShadowObject3d aShadowObject):base(aShadowObject) {
		this.surface_sides = new HashSet<SurfaceSide>() {Scientrace.SurfaceSide.Both};
		}

	public PhysicalObject3d(Object3dCollection parent, Scientrace.MaterialProperties materialproperties) : base(parent, materialproperties) {
		this.surface_sides = new HashSet<SurfaceSide>() {Scientrace.SurfaceSide.Both};
		//this.surface_sides.Add(Scientrace.SurfaceSide.Both);
		//this.materialproperties = materialproperties;		
		}

	public virtual bool directedTowardsObjectside(Scientrace.SurfaceSide side, Scientrace.Trace aTrace) {
		switch (side) {
			case Scientrace.SurfaceSide.Back: // == SurfaceSide.Inside
				throw new NotImplementedException("Side "+side.ToString()+" not implemented for class"+this.GetType().ToString());
				//break;
			case Scientrace.SurfaceSide.Front:
				throw new NotImplementedException("Side "+side.ToString()+" not implemented for class"+this.GetType().ToString());
				//break;
			case Scientrace.SurfaceSide.Both:
				return true;
				//break;
			default:
				throw new NotImplementedException("Side "+side.ToString()+" not implemented");
				//break;
			}
		}

	public virtual Parallelogram getDistributionSurface() {
		throw new NotImplementedException("getDistributionSurface not implemented for "+this.tag);
		}

	public virtual Scientrace.UnitVector getSurfaceNormal() {
		throw new NotImplementedException("getSurfaceNormal not implemented for "+this.tag);
		}
		
	public virtual Scientrace.Location getSVG2DLoc(Scientrace.Location loc) {
		throw new NotImplementedException("getSVG2DLoc not implemented for "+this.tag);
		}
		
		
	public virtual Scientrace.Location get2DLoc(Scientrace.Location loc) {
		throw new NotImplementedException("get2DLoc not implemented for "+this.tag);
		}

/*	public virtual string getViewBox() {
		throw new NotImplementedException("No viewbox defined for "+this.tag);
		}*/

/*	public string getViewBox() {
		double[] vb = this.getViewBorders();
		return (""+vb[0]+" "+vb[1]+" "+vb[2]+" "+vb[3]);
		//return ("0 0 "+this.surface.u.length+" "+this.surface.v.length);
		} */

	public virtual double[] getViewBorders() {
		throw new NotImplementedException("No viewborders defined for "+this.tag);
		}		
		
	public string getRelMarginViewBox(double allSidesFractions, double xfactor, double yfactor) {
		return this.getRelMarginViewBox(allSidesFractions, allSidesFractions, allSidesFractions, allSidesFractions);
		}
		
	public double viewBoxLeft() {
		double[] vb = this.getViewBorders();
		return vb[0];
		}
		
	public double viewBoxTop() {
		double[] vb = this.getViewBorders();
		return vb[1];
		}

	public double viewBoxWidth() {
		return (this.viewBoxRight() - this.viewBoxLeft());
		}
		
	public double viewBoxRight() {
		double[] vb = this.getViewBorders();
		return vb[2];
		}

	public double viewBoxBottom() {
		double[] vb = this.getViewBorders();
		return vb[3];
		}
		
	public double viewBoxHeight() {
		return (this.viewBoxBottom() - this.viewBoxTop());
		}

	/// <summary>
	/// The viewbox for the svg document "viewbox" attribute.
	/// </summary>
	/// <param name="leftMarginFraction">
	/// A <see cref="System.Double"/> to specify the added border at the left of the image as a fraction of the physicalobject surface.
	/// </param>
	/// <param name="topMarginFraction">
	/// A <see cref="System.Double"/> to specify the added border at the top of the image as a fraction of the physicalobject surface.
	/// </param>
	/// <param name="rightMarginFraction">
	/// A <see cref="System.Double"/> to specify the added border at the right of the image as a fraction of the physicalobject surface.
	/// </param>
	/// <param name="bottomMarginFraction">
	/// A <see cref="System.Double"/> to specify the added border at the bottom of the image as a fraction of the physicalobject surface.
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/> containing space-separated doubles, the input for the SVG "viewbox" based on the "getviewborders"
	/// parameters for the specific physicalobject and the margin-parameters with this method.
	/// </returns>
	public string getRelMarginViewBox(double leftMarginFraction, 
		                         double topMarginFraction, double rightMarginFraction, double bottomMarginFraction) {
		double left, right, top, bottom, width, height;
		left = this.viewBoxLeft();
		right = this.viewBoxRight();
		width = right-left;
		top = this.viewBoxTop();
		bottom = this.viewBoxBottom();
		height = bottom-top;
/*		return (""+(vb[0]-((vb[2]-vb[0])*leftMarginFraction))+" "+(vb[1]-((vb[3]-vb[1])*topMarginFraction))+" "
			      +(vb[2]+((vb[2]-vb[0])*rightMarginFraction))+" "+(vb[3]+((vb[3]-vb[1])*bottomMarginFraction))); */

		return (""+(left-(width*leftMarginFraction))+" "+(top-(height*topMarginFraction))+" "+
					(width+(width*leftMarginFraction)+(rightMarginFraction))+" "+
					(height+(height*topMarginFraction)+(bottomMarginFraction)));
		}

	public string getAbsMarginViewBox(double leftAbsoluteMargin, 
		                         double topAbsoluteMargin, double rightAbsoluteMargin, double bottomAbsoluteMargin, double unitsize) {
		return this.getAbsMarginViewBox(leftAbsoluteMargin*unitsize, 
										topAbsoluteMargin*unitsize,
			                            rightAbsoluteMargin*unitsize,
			                            bottomAbsoluteMargin*unitsize);
		}
		
	public string getAbsMarginViewBox(double leftAbsoluteMargin, 
		                         double topAbsoluteMargin, double rightAbsoluteMargin, double bottomAbsoluteMargin) {
		double left, right, top, bottom, width, height;
		left = this.viewBoxLeft();
		right = this.viewBoxRight();
		width = right-left;
		top = this.viewBoxTop();
		bottom = this.viewBoxBottom();
		height = bottom-top;
/*		return (""+(vb[0]-((vb[2]-vb[0])*leftMarginFraction))+" "+(vb[1]-((vb[3]-vb[1])*topMarginFraction))+" "
			      +(vb[2]+((vb[2]-vb[0])*rightMarginFraction))+" "+(vb[3]+((vb[3]-vb[1])*bottomMarginFraction))); */

		return (""+(left-leftAbsoluteMargin)+" "+(top-topAbsoluteMargin)+" "+
					(width+leftAbsoluteMargin+rightAbsoluteMargin)+" "+
					(height+topAbsoluteMargin+bottomAbsoluteMargin));
		}
		
		
}
}
