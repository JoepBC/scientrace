// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class Cube : Scientrace.Prism {

//	public Scientrace.Location loc; <-- noew derived
	public Scientrace.Vector width, height;
	//length is inherited from Scientrace.Prism
/* although it has no physical significance w,l and h can be thought of as below.
	 ____
	/   /|
   /__ / |
   |   | |
  h|   | /
   |___|/ l
     w
   */	
	
		public Cube(Object3dCollection parent, Scientrace.MaterialProperties mprops) : base(parent, mprops) {
	}

	public override Intersection intersects(Trace trace) {
		throw new System.NotImplementedException();
	}

	public override string exportX3D() {
			throw new NotImplementedException("exportx3d not yet implemented for cube");}

	public Cube(Object3dCollection parent, Scientrace.MaterialProperties mprops, int x, int y, int z, int width, int length, int height) : base(parent, mprops) {
		//TODO create constructor and basic behaviour
	}
	
}
}
