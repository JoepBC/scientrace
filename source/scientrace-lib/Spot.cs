// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class Spot {

	public Object3d object3d;
	public Scientrace.Location loc;
	public double intensity, intensityFraction;
	public Trace trace;
	public Vector pol_vec_1 = null;
	public Vector pol_vec_2 = null;

	public Spot(Scientrace.Location loc, Object3d object3d, double intensity, double intensityFraction, Scientrace.Trace trace) {
		this.loc = loc;
		this.object3d = object3d;
		this.object3d.spotted(intensity);
		this.intensity = intensity;
		this.intensityFraction = intensityFraction;
		this.trace = trace;
		this.fillPolVecs();
		}

	private void fillPolVecs() {
		if (this.trace != null) {
			this.pol_vec_1 = trace.getPolarisationVec1();
			this.pol_vec_2 = trace.getPolarisationVec2();
			}
		}

	public Spot(Scientrace.Spot copyFromSpot) {
		this.loc = new Scientrace.Location(copyFromSpot.loc);
		this.object3d = copyFromSpot.object3d;
		// do not spot again, so don't recall spotted from object3d
		this.intensity = copyFromSpot.intensity;
		this.trace = copyFromSpot.trace;
		this.intensityFraction = copyFromSpot.intensityFraction;
		this.fillPolVecs();
		}

	}}//end class Spot & namespace Scientrace
