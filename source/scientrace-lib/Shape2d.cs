// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public class Shape2d {

		public Scientrace.SurfaceProperties surfaceproperties = null;

		public Shape2d() {
		}
		
		
	public virtual double getSurfaceSize() {
		throw new SubclassResponsibilityException("getsurface not available @ Plane itself");
		}
		
		
	public bool inPath(Scientrace.Line line) {
		return (this.atPath(line) != null);
		}

		
	public virtual Scientrace.Location atPath(Scientrace.Line line) {
		throw new SubclassResponsibilityException("atpath not available @ Plane class");
		}		
		
		}
}