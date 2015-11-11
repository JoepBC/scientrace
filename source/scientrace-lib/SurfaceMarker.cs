// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {
	
public class SurfaceMarker {
	
	public Scientrace.PhysicalObject3d marksObject;
		
	public double fontSizeFraction = 25;
	public string xunits = "";
	public string yunits = "";
	
	public SurfaceMarker(Scientrace.PhysicalObject3d parentObject) {
		this.marksObject = parentObject;
		}
		
		
	public virtual string exportSVG() {
		return "";	
		}

}
}

