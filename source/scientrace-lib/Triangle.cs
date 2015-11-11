// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class Triangle : Scientrace.FlatShape2d {

	public Triangle(Scientrace.Location loc, Scientrace.NonzeroVector u, Scientrace.NonzeroVector v) : base(loc, u, v) {
	}

	public override Scientrace.Location atPath(Scientrace.Line line) {
		Scientrace.Vector tloc = this.plane.lineThroughPlaneTLoc(line);
		if (tloc == null) {
			//Console.WriteLine("Line is parallel to plane surface.");
			return null;
		}
		Scientrace.Vector trevcrossloc = tloc-this.plane.geteloc();
		//check x value between (0,0,0)-(1,0,0) and y betw. (0,0,0)-(0,1,0) and x+y <= 1. Be in the bottom-left triangle of the 1*1 square
		if (((trevcrossloc.x >= 0) && (trevcrossloc.x <= 1)) && ((trevcrossloc.y >= 0) && (trevcrossloc.y <= 1)) && (trevcrossloc.x+trevcrossloc.y <= 1)) {
			return this.plane.lineThroughPlaneOLoc(tloc);
		} else {
			return null;
		}
	}


	public override double getSurfaceSize() {
		return this.plane.u.crossProduct(this.plane.v).length/2;
	}
	
}
}
