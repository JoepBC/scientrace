// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {

public class Parallelogram : Scientrace.FlatShape2d {


	public Parallelogram(Scientrace.Location loc, Scientrace.NonzeroVector u, Scientrace.NonzeroVector v) : base(loc,u,v){
	}

	public override double getSurfaceSize() {
		return this.plane.u.crossProduct(this.plane.v).length;
	}

	public double angleOnSurface(Scientrace.UnitVector aUnitVector) {
		double projection = aUnitVector.dotProduct(this.plane.getNorm());
		if (Math.Abs(projection) > 1) {
			if ((Math.Abs(projection)-1) < 1E-14) {
				//this 1.00[...]00x value is probably an error due to rounding. Cos 0 = 1, so return 0.
				return 0;
			}
		}
		return Math.Acos(projection);
	}

	public double angleOnSurface(Scientrace.Vector aVector) {
		return this.angleOnSurface(aVector.tryToUnitVector());
	}

	public Scientrace.Plane getPlane() {
		return this.plane;
		//return new Scientrace.Plane(this.loc, this.u, this.v);
		}
		
	//<plane crossing and surface intersecting methods>
	public override Scientrace.Location atPath(Scientrace.Line line) {
		Scientrace.Vector tloc = this.plane.lineThroughPlaneTLoc(line);
		if (tloc==null) { //parallel to plane: not at path!
			return null;
			}
		Scientrace.Vector trevcrossloc = tloc-this.plane.geteloc();
		//check x value between (0,0,0)-(1,0,0) and y betw. (0,0,0)-(0,1,0)
		if (((trevcrossloc.x >= 0) && (trevcrossloc.x <= 1)) && ((trevcrossloc.y >= 0) && (trevcrossloc.y <= 1))) {
			return this.plane.lineThroughPlaneOLoc(tloc);
		} else {
			return null;
		}
	}
	

	public string exportX3D (Scientrace.Object3dEnvironment env)	{
		Scientrace.Vector a, b, c, d;
		//square
		a = Vector.ZeroVector();
		b = this.plane.u;
		c = this.plane.u+this.plane.v;
		d = this.plane.v;
	
			string colstr = "0.5 0.5 0.5 0.2";
		return @"<!-- Start Parallelogram instance -->\n <Transform translation='"+this.plane.loc.trico()+@"'>
<Shape>
<!-- Coordinate prism sequence (matches coordIndex actually) 0 1 2 3 0 -->
<LineSet vertexCount='5'>
<Coordinate point='
"+a.tricon()+b.tricon()+c.tricon()+d.tricon()+a.tricon()+"'/>" +

	/*@"<ColorRGBA color='0 1 0 1 0 1 0 1
0 1 0 1 0 1 0 1
0 1 0 1 0 1 0 1
0 1 0 1 0 1 0 1' />	
</LineSet>" + */
	@"<ColorRGBA color='"+colstr +" "+colstr +" "+colstr +" "+colstr +" "+colstr +" "+colstr +" "+colstr +" "+colstr +@"' />	
</LineSet>" +
	@"      </Shape>" +
	@"    </Transform>\n<!-- End Parallelogram instance -->";
	
	}

		
	//</plane crossing and surface intersecting methods>
	
}
}
