// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections.Generic;




namespace Scientrace {
public abstract class EnclosedVolume : PhysicalObject3d {

	public EnclosedVolume(ShadowScientrace.ShadowObject3d aShadowObject):base(aShadowObject) {
		this.hasVolume = true;
		}

	public EnclosedVolume(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops) : base(parent, mprops) {
		this.hasVolume = true;
		}

	public float getRefractiveIndex(Scientrace.Trace trace) {
		throw new SubclassResponsibilityException("Implementation required of getRefractiveIndex(trace) function required at"+this.GetType().ToString()+trace.ToString());
		}

	public Scientrace.Intersection intersectFlatShapes(Scientrace.Trace trace, List<Scientrace.FlatShape2d> pgrams) {
		return trace.intersectplanesforobject(this, pgrams);
		}
	
}
}
