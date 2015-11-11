// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class ParallelRandomCylindricalLightSource : ParallelLightSource {

	public ParallelRandomCylindricalLightSource(Scientrace.Object3dEnvironment env, Scientrace.Location center, Scientrace.UnitVector direction, Scientrace.Plane plane, double radius, int linecount, LightSpectrum spectrum) : base(env) {
		//this.direction = direction;
		Random randNum = new Random();
		Scientrace.UnitVector urange, vrange;
		double r1, r2;
		urange = plane.u.toUnitVector();
		vrange = plane.v.toUnitVector();
		Scientrace.Line line;
		for (int iline = 0; iline < linecount; iline++) {
			//Scientrace.Line line = new Scientrace.Line(loc+(urange*randNum.NextDouble()+(vrange*randNum.NextDouble())).toLocation(), direction);
			r1 = 1-randNum.NextDouble()*2;
			r2 = 1-randNum.NextDouble()*2;
			if (Math.Pow(r1,2)+Math.Pow(r2,2)<1) {
				line = new Scientrace.Line(center+((urange*r1*radius)+(vrange*r2*radius)).toLocation(),direction);
				} else {
				iline--;
				continue;
				}
			//this.traces.Add(new Scientrace.Trace(wavelength*0.6, this, line, env));
			//this.traces.Add(new Scientrace.Trace(wavelength*0.8, this, line, env));
			//double wavelength = wlmin+(randNum.NextDouble()*(wlmax-wlmin));
			double wavelength = spectrum.wl(iline);
			double intensity = spectrum.it(iline);
			Console.WriteLine("Warning: ParallelRandomCylindricalLightSource does not use modulo yet");
			this.addTrace(new Scientrace.Trace(wavelength, this, line, env, intensity, intensity));
			//this.traces.Add(new Scientrace.Trace(wavelength*1.2, this, line, env));
			//this.traces.Add(new Scientrace.Trace(wavelength*1.4, this, line, env));
		}
	}
	
}
}
