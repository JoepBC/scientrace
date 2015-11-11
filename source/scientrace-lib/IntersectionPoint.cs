// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class IntersectionPoint {

	public Scientrace.Location loc;
	public Scientrace.FlatShape2d flatshape;

	public IntersectionPoint(Scientrace.Location loc, Scientrace.FlatShape2d flatshape) {
		this.loc = loc;
		this.flatshape = flatshape;
	}


	public static IntersectionPoint locAtSurfaceNormal(Scientrace.Location loc, Scientrace.NonzeroVector normalVec) {
		return new IntersectionPoint(loc, new FlatShape2d(Plane.newPlaneOrthogonalTo(loc, normalVec)));
		}

	public IntersectionPoint copy() {
		return new IntersectionPoint(this.loc.copy(), this.flatshape);
		}

	/*
	 * Overloaded operators
	 */
	public static Scientrace.IntersectionPoint operator -(Scientrace.IntersectionPoint ip, Scientrace.Location loc) {
		//the - operator for an IntersectionPoint with a location works on "loc" only, not on direction of the plane
		return new Scientrace.IntersectionPoint(ip.loc - loc, ip.flatshape);
	}
	public static Scientrace.IntersectionPoint operator +(Scientrace.IntersectionPoint ip, Scientrace.Location loc) {
		//the + operator for an IntersectionPoint with a location works on "loc" only, not on direction of the plane
		return new Scientrace.IntersectionPoint(ip.loc + loc, ip.flatshape);
	}

	public override string ToString ()	{
		return this.loc.ToString()+"@"+this.flatshape.plane.ToString();
	}

}
}
