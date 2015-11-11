// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class Circle2d : FlatShape2d {

	public double radius;

	public Circle2d(Scientrace.Location loc, Scientrace.NonzeroVector u, Scientrace.NonzeroVector v, double radius) : base(loc, u, v) {
		this.radius = radius;
	}


	public override Scientrace.Location atPath(Scientrace.Line line) {
		Scientrace.Vector cloc = this.plane.lineThroughPlane(line);
		if (cloc == null) {
			//Console.WriteLine("Line is parallel to plane surface.");
			return null;
		}
		Scientrace.Vector revcrossloc = cloc - this.plane.loc;
		if (Math.Sqrt(Math.Pow(revcrossloc.x,2)+Math.Pow(revcrossloc.y,2)+Math.Pow(revcrossloc.z,2)) <= this.radius) {
			return cloc.toLocation();
		} else {
			return null;
		}
	}


	public override double getSurfaceSize() {
			//pi*r^2
		return (Math.PI*radius*radius);
		}
	
}
}
