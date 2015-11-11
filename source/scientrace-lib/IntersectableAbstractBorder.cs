// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public abstract class IntersectableAbstractBorder : AbstractBorder {
	public IntersectableAbstractBorder() {
		}

	public abstract IntersectionPoint[] intersectionPoints(Trace trace);
	
	}}