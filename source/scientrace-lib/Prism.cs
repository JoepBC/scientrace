// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public abstract class Prism : Scientrace.EnclosedVolume {

	public Scientrace.NonzeroVector length;

	public Prism(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops):base(parent, mprops) {
		}

	public Prism(ShadowScientrace.ShadowObject3d aShadowObject):base(aShadowObject) {
		}

}} // end class Prism 