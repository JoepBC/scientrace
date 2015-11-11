// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class SingleRaySource: Scientrace.LightSource {

	Scientrace.Line ray;
//	public LightSpectrum spectrum;
	public int linecount;	
	
	public SingleRaySource(Scientrace.Line rayline, int raycount, LightSpectrum spectrum, Scientrace.Object3dEnvironment env) :base(env){
		this.weighted_intensity = spectrum.total_intensity;
		this.spectrum = spectrum;
		this.linecount = raycount;
		this.ray = rayline;
		}

/*	public SingleRaySource(Scientrace.Line ray, double[] wavelengths, Scientrace.Object3dEnvironment env) :base(env){
		this.ray = ray;
		this.wls = wavelengths;
		this.init();
		}*/
		
						
	public void init() {
		for (int n = 1; n <= linecount; n++) {
			if (spectrum.it(n)==0) {
				continue;
				}
			Scientrace.Trace ttrace = new Scientrace.Trace(spectrum.wl(n), this, this.ray, env, spectrum.it(n),  spectrum.it(n));
			ttrace.traceid = "singleray_"+n.ToString();
			this.addTrace(ttrace);
			}
		}
		
}
}
