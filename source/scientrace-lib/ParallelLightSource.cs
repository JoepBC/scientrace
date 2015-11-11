// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public abstract class ParallelLightSource : Scientrace.LightSource {

	/* Parameters for diverging source */
	public double divergence_angle = 0;
	public Scientrace.NonzeroVector divergence_relvec;
	public double divergence_distance;
	public int divergence_steps = 1;
	public bool include_base_when_diverge = true;
		
		
	public ParallelLightSource(Scientrace.Object3dEnvironment env):base(env) {
		}
	
	public ParallelLightSource(ShadowScientrace.ShadowLightSource shadowObject): base(shadowObject) {
		}
	
	}}//end class + namespace 