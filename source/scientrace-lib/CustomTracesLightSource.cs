// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {
public class CustomTracesLightSource : Scientrace.LightSource {


	public CustomTracesLightSource(ShadowScientrace.ShadowLightSource shadowObject): base(shadowObject) {
		foreach (Scientrace.Trace aTrace in (List<Scientrace.Trace>)shadowObject.getObject("traces_list")) {
			aTrace.lightsource = this;
			this.addTrace(aTrace);
			}
		}
	


	public CustomTracesLightSource(Scientrace.Object3dEnvironment env):base(env) {
		}
		
	public CustomTracesLightSource(Scientrace.Object3dEnvironment env, List<Scientrace.Trace> traces):base(env) {
		foreach (Scientrace.Trace aTrace in traces) {
			aTrace.lightsource = this;
			this.addTrace(aTrace);
			}
			//Console.WriteLine("FINISHED creating CustomTracesLightSource with instant count: "+this.traces.Count);
		}

}}