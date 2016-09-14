// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace ScientraceTesting {


public class DummySources {

	public DummySources() {
	}

	public static Scientrace.Object3dEnvironment dEnv() {
		return new Scientrace.Object3dEnvironment(Scientrace.AirProperties.Instance, 100);
		}

	public static Scientrace.Line dLine() {
		return new Scientrace.Line(1, 0, 0, 1, 1, 0);
		}

	/*
	public static Scientrace.Trace dTrace() {
		return new Scientrace.Trace(500, DummySources.dLightSource(), DummySources.dLine(), DummySources.dEnv(), 1,1);
		} */

	/*
	public static Scientrace.ParallelRandomSquareLightSource dLightSource() {
		return DummySources.dLightSource(DummySources.dEnv());
		}*/

	/*
	public static Scientrace.ParallelRandomSquareLightSource dLightSource(Scientrace.Object3dEnvironment env) {
		return new Scientrace.ParallelRandomSquareLightSource(env, new Scientrace.Location(0,0,0)
			                                                      , new Scientrace.UnitVector(1,0,0)
			                                                      , new Scientrace.NonzeroVector(0,1,0)
			                                                      , new Scientrace.NonzeroVector(0,0,1)
			                                                      , 10, 500);

		}*/
}
}
