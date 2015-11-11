// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class ParallelSquareLightSource : Scientrace.ParallelLightSource {

	public ParallelSquareLightSource(Scientrace.Object3dEnvironment env, Scientrace.Location cloc, Scientrace.UnitVector direction, Scientrace.NonzeroVector u, Scientrace.NonzeroVector v, int ucount, int vcount, double wavelength) : base(env) {
		Scientrace.Location loc = (cloc - ((u*(ucount-1)*0.5)+(v*(vcount-1)*0.5))).toLocation();
		for (int iu = 0; iu < ucount; iu++) {
			for (int iv = 0; iv < vcount; iv++) {
					//Console.WriteLine("New line at "+(loc+((u.toVector()*iu)+(v.toVector()*iv)).toLocation()).trico());
					Scientrace.Line line = new Scientrace.Line(loc+((u.toVector()*iu)+(v.toVector()*iv)).toLocation(), direction);
					this.addTrace(new Scientrace.Trace(wavelength, this, line, env,1,1));
			}
		}
	}
}
}
