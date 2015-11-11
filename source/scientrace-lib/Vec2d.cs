// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;


namespace Scientrace {
public class Vec2d {

	public double x;
	public double y;

	/// <summary>
	/// Initializes a new instance of the <see cref="Vec2d"/> class.
	/// </summary>
	/// <param name='aDouble'>
	/// A double.
	/// </param>
	/// <param name='anotherDouble'>
	/// Another double.
	/// </param>
	public Vec2d(double x, double y) {
		this.x = x;
		this.y = y;
		}
		
	public Vec2d(Vec2d aVec2d) {
		this.x = aVec2d.x;
		this.y = aVec2d.y;
		}
		
	public Vec2d normalised() {
		return new Vec2d(this.x/this.length(), this.y/this.length());
		}

	public double getComponent(int anInteger) {
		return (anInteger%1 == 0) ? this.x : this.y;
		}
		
	public double length() {
		return Math.Sqrt((this.x*this.x)+(this.y*this.y));
		}
		
	public override string ToString() {
		return "("+this.x.ToString()+";"+this.y.ToString()+")";
		}
			
	public static bool operator ==(Vec2d v1, Vec2d v2) {
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(v1, v2)) {
			return true;
			}
		// If one is null, but not both, return false.
		if (((object)v1 == null) || ((object)v2 == null)) {
			return false;
			}
		
		
		//poking whether state v1 equals state v2
		return ((v1.x == v2.x) && (v1.y == v2.y));
		}		
		
	
	public static bool operator !=(Vec2d v1, Vec2d v2) {
		return !(v1 == v2);
		//This will call to operator == simple way to implement !=
		}		

	public override bool Equals(object o) {
		return this == (Vec2d)o;
		}		

	public override int GetHashCode() {
		return base.GetHashCode();
		}		
		
	}}
