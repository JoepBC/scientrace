// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class Angle {

	public Scientrace.Trace fromTrace;
	public Scientrace.Trace toTrace;
	public Scientrace.Intersection intersection;
	public double radians;
		
	public Angle(Scientrace.Trace fromTrace, Scientrace.Trace toTrace, Scientrace.Intersection intersection) {
		this.fromTrace = fromTrace;
		this.toTrace = toTrace;
		this.radians = fromTrace.traceline.direction.angleWith(toTrace.traceline.direction);
			this.intersection = intersection;
	}

	public Scientrace.Location getLocation() {
			return this.toTrace.traceline.startingpoint;
		}
}
}
