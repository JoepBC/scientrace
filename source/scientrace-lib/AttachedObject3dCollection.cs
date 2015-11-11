// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class AttachedObject3dCollection : Scientrace.Object3dCollection {

	public AttachedObject3dCollection(Object3dCollection parent, MaterialProperties mp) : base(parent, mp)  {
		}

/*	public override string exportX3D(Scientrace.Object3dEnvironment env) {
		return base.exportX3D(env);
	}		 */
	
}
}
