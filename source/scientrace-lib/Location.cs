// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {

public class Location : Vector {


	public Location(double x, double y, double z) : base(x, y, z) {
	}

	public Location(Location aLoc) : base(aLoc.x, aLoc.y, aLoc.z) {
		//copy constructor
		//this.init(aLoc.x, aLoc.y, aLoc.z);
		}

	/*
	 * Overloaded operators
	 */
	public static Location operator -(Scientrace.Location v1, Scientrace.Location v2) {
		return new Location(v1.x-v2.x, v1.y-v2.y, v1.z-v2.z);
	}

	public static Location operator -(Scientrace.Location v1, Scientrace.Vector v2) {
		return new Location(v1.x-v2.x, v1.y-v2.y, v1.z-v2.z);
	}

	public static Location operator *(Scientrace.Location v1, double f) {
		return new Location(v1.x*f, v1.y*f, v1.z*f);
	}

	public static Location operator +(Scientrace.Location v1, Scientrace.Location v2) {
		return new Location(v1.x+v2.x, v1.y+v2.y, v1.z+v2.z);
	}

	public static Location operator +(Scientrace.Location v1, Scientrace.Vector v2) {
		return new Location(v1.x+v2.x, v1.y+v2.y, v1.z+v2.z);
	}

	public new Location negative() {
		return new Location(-this.x, -this.y, -this.z);
	}

	/*
	 * Regular functions
	 */
	public double distanceTo(Location aLocation) {
		return Math.Sqrt(Math.Pow(this.x-aLocation.x, 2)+Math.Pow(this.y-aLocation.y, 2)+Math.Pow(this.z-aLocation.z, 2));
	}

	public new Scientrace.Location copy() {
		return new Scientrace.Location(this.x, this.y, this.z);
		}

	public static Location ZeroLoc() {
		return new Location(0,0,0);
		}

	public Location avgWith(Scientrace.Location aLoc) {
		return new Location(0.5*(this.x+aLoc.x), 0.5*(this.y+aLoc.y), 0.5*(this.z+aLoc.z));
		}
		
		
	public Scientrace.Location rotateAboutVector(Scientrace.NonzeroVector mirrorAxis, Scientrace.Location center, double radians) {
		return (this - center).rotateAboutVector(mirrorAxis, radians).toLocation()+center;
		}
		
	/// <summary>
	/// Check if a location in the list exists which is not significantly different from this.
	/// </summary>
	/// <param name="aList">A list with locations</param>
	public void addToListIfNew(List<Scientrace.Location> aList) {
		foreach (Location aLoc in aList) {
			if ((aLoc - this).length < (MainClass.SIGNIFICANTLY_SMALL*1E5)) //adjustment *100 necessary based on experience. TODO: get better error value
				return;
			}
		aList.Add(this);
		}


}
}
