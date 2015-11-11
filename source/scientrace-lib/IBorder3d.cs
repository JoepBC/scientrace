// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public interface IBorder3d {

	bool contains(Scientrace.Location loc);
	
	}
		
public interface IInterSectableBorder3d : IBorder3d {

	Intersection intersectsObject(Trace trace, Object3d o3dWithThisBorder);
	bool contains(Scientrace.Location loc, double margin);
	
	}
		
}

