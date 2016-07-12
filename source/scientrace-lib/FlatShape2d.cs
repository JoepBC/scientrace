// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public class FlatShape2d : Scientrace.Shape2d {

	public Scientrace.Plane plane;
	
	public FlatShape2d(Scientrace.Location loc, Scientrace.NonzeroVector u, Scientrace.NonzeroVector v) {
		this.plane = new Plane(loc, u, v);
		}

	public FlatShape2d(FlatShape2d copyFlatShape2d) {
		this.plane = new Scientrace.Plane(copyFlatShape2d.plane);
		}

	public FlatShape2d(Scientrace.Plane aPlane) {
		this.plane = aPlane;
		}


	public FlatShape2d copy() {
		//return new FlatShape2d(new Scientrace.Plane(this.plane));
		return new FlatShape2d(this);
		}

	}
}

