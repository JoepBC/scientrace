// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class ParallelRandomSquareLightSource : ParallelLightSource {

	public ParallelRandomSquareLightSource(ShadowScientrace.ShadowLightSource shadowObject): base(shadowObject) {
		int? random_seed = shadowObject.getNInt("random_seed");
		Scientrace.Location location = shadowObject.getLocation("location"); //corner
		Scientrace.UnitVector direction = shadowObject.getUnitVector("direction");
		int ray_count = shadowObject.getInt("ray_count"); //linecount,
		this.distance = shadowObject.getDouble("distance", 0);//distance,

		//This line has been moved to the LightSource parent shadowconstructor
		//Scientrace.LightSpectrum spectrum = (Scientrace.LightSpectrum)shadowObject.getObject("spectrum"); //spectrum

		Scientrace.Vector width = shadowObject.getVector("width");//urange,
		Scientrace.Vector height = shadowObject.getVector("height");//vrange,

		Random randNum;
		if (random_seed == null || random_seed== -1)
			randNum = new Random();
			else
			randNum = new Random((int)random_seed);

		for (int iline = 0; iline < ray_count; iline++) {
			Scientrace.Line line = new Scientrace.Line(location+(width*randNum.NextDouble()+(height*randNum.NextDouble())).toLocation(), direction);
			// THIS IS BEING DONE BY THE PARENT LIGHTSOURCE CLASS ALREADY: line.startingpoint = line.startingpoint - (direction.toLocation()*distance);
			Scientrace.Trace newtrace = new Scientrace.Trace(spectrum.wl(iline), this, line, env, spectrum.it(iline), spectrum.it(iline));
			newtrace.traceid = "RS"+(random_seed==null?"r":random_seed.ToString())+"-"+iline;
			this.addTrace(newtrace);
			} //end for
		} //end void oldParamInit


	/*
	//old deprecated constructor
	public ParallelRandomSquareLightSource(Scientrace.Object3dEnvironment env, Scientrace.Location loc, Scientrace.UnitVector direction, Scientrace.NonzeroVector urange, Scientrace.NonzeroVector vrange, int linecount, double wavelength) : base(env) {
		this.oldParamInit(loc, direction, urange, vrange, linecount, wavelength);
		}

	//old deprecated init
	public void oldParamInit(Scientrace.Location loc, Scientrace.UnitVector direction, Scientrace.Vector urange, Scientrace.Vector vrange, int linecount, double wavelength) {
		Random randNum = new Random();
		for (int iline = 0; iline < linecount; iline++) {
			Scientrace.Line line = new Scientrace.Line(loc+(urange*randNum.NextDouble()+(vrange*randNum.NextDouble())).toLocation(), direction);
			//this.traces.Add(new Scientrace.Trace(wavelength*0.6, this, line, env));
			this.traces.Add(new Scientrace.Trace(wavelength, this, line, env, 1, 1));
			//this.traces.Add(new Scientrace.Trace(wavelength*1.4, this, line, env));
			} //end for
		} //end void oldParamInit
	*/
}
}
