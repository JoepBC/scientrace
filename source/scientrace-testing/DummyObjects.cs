// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace ScientraceTesting {
public class DummyObjects {
		public DummyObjects() {
		}

		public static Scientrace.Object3dEnvironment DummyEnv() {
			return new Scientrace.Object3dEnvironment(Scientrace.AirProperties.Instance, 1);
			}

		public static Scientrace.Line DummyLine() {
			return new Scientrace.Line(0,0,0, 1,0,0);
			}

		public static Scientrace.SingleRaySource DummyLight(double wavelength) {
			return new Scientrace.SingleRaySource(DummyObjects.DummyLine(), 1, 
				new Scientrace.SingleWavelengthSpectrum(1, wavelength), DummyObjects.DummyEnv());
			}

		public static Scientrace.Trace DummyTrace(double wavelength) {
			return new Scientrace.Trace(wavelength, DummyObjects.DummyLight(wavelength), DummyObjects.DummyLine(), null, 1,1);
			}
}
}

