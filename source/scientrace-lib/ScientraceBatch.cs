// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections;

namespace Scientrace {


public class ScientraceBatch {

	public Scientrace.Object3dEnvironment env;
	public string batchdir;
	public int numberOfCycles;
		
	public ScientraceBatch(string batchdir, Scientrace.Object3dEnvironment env, int numberOfCycles) {
		this.env = env;
		this.batchdir = batchdir;
		this.numberOfCycles = numberOfCycles;
	}

	public void cycle(ArrayList envState) {
		this.cycle(0, envState);
		}

	public void explore(ArrayList envState) {
		
		}

	public void cycle(int iCount, ArrayList envState) {
		if (iCount>=this.numberOfCycles) {
			return;
			}
		// keep on cycling:
		this.explore(envState);
		}
}
}
