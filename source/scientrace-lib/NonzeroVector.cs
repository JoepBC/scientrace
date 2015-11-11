// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class NonzeroVector : Vector {

	public double xn, yn, zn;

	public NonzeroVector(double x, double y, double z) : base(x, y, z) {
		this.init();
		}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="copyVec">source data to be copied</param>
	public NonzeroVector(NonzeroVector copyVec) : base(copyVec.x, copyVec.y, copyVec.z) {
		this.init();
		}

	private void init() {
		if (this.length == 0) {
			//a nonzerovector can only be created for a vector with a direction
			throw new ZeroNonzeroVectorException("length = 0 for new NonzeroVector with("+this.x.ToString()+", "+this.y.ToString()+", "+this.z.ToString()+") and length "+this.length.ToString());
		}
		this.xn = x/this.length;
		this.yn = y/this.length;
		this.zn = z/this.length;
		}

	public void normalize() {
		this.x = this.xn;
		this.y = this.yn;
		this.z = this.zn;
		this.length = 1;
		}

	public new static NonzeroVector x1vector() {
		return new NonzeroVector(1,0,0);
		}
	public new static NonzeroVector y1vector() {
		return new NonzeroVector(0,1,0);
		}
	public new static NonzeroVector z1vector() {
		return new NonzeroVector(0,0,1);
		}

	public new NonzeroVector copy() {
		return new NonzeroVector(this.x, this.y, this.z);
		}

	public new NonzeroVector reflectOnSurface(NonzeroVector norm) {
		return base.reflectOnSurface(norm).tryToNonzeroVector();
		}

	public new NonzeroVector orientedAlong(Scientrace.Vector aVector) {
		if (aVector.dotProduct(this) < 0) {
			return this.negative();
			}
		return this;
		}

	public new NonzeroVector orientedAgainst(Scientrace.Vector aVector) {
		if (aVector.dotProduct(this) > 0) {
			return this.negative();
			}
		return this;
		}

	public Vector normalizedVector() {
		//copy this
		return new Vector(this.xn, this.yn, this.zn);
		}

	public NonzeroVector normalized() {
		//copy this
		return new NonzeroVector(this.xn, this.yn, this.zn);
		}

	/*
	 * Overloaded operators
	 */
	public static NonzeroVector operator -(Scientrace.NonzeroVector v1, Scientrace.NonzeroVector v2) {
		return new NonzeroVector(v1.x-v2.x, v1.y-v2.y, v1.z-v2.z);
	}

	public static NonzeroVector operator *(Scientrace.NonzeroVector v1, double f) {
		if (f==0) {
			throw new ZeroNonzeroVectorException("multiplication "+v1+" with 0 cannot create a nonzerovector");
			}
		return new NonzeroVector(v1.x*f, v1.y*f, v1.z*f);
	}

	public static NonzeroVector operator +(Scientrace.NonzeroVector v1, Scientrace.NonzeroVector v2) {
		return new NonzeroVector(v1.x+v2.x, v1.y+v2.y, v1.z+v2.z);
	}

	public new NonzeroVector negative() {
		return new NonzeroVector(-this.x, -this.y, -this.z);
	}

	//exact same function as "tryToUnitVector" but has to have a result unlike a normal vector which could be the 0-vector.
	public Scientrace.UnitVector toUnitVector() {
		return this.tryToUnitVector();
	}
		
	public void fillOrtogonalVectors(ref Scientrace.NonzeroVector refVec1, ref Scientrace.NonzeroVector refVec2) {
		Scientrace.UnitVector tRefvec = Scientrace.UnitVector.x1vector();
		if (Math.Abs(this.toUnitVector().dotProduct(tRefvec))>0.8) {
			tRefvec = Scientrace.UnitVector.y1vector();
		}		
		refVec1 = tRefvec.crossProduct(this).tryToUnitVector();
		refVec2 = refVec1.crossProduct(this).tryToUnitVector();			
		}
		
	//The sign of the sin functions could be changed following the example on http://en.wikipedia.org/wiki/Rotation_matrix as for juli 15th 2010
	public new NonzeroVector rotateAboutX(double radians) {
		return new NonzeroVector(
				this.x,
				this.dotProduct(new Vector(0, Math.Cos(radians), Math.Sin(radians))),
				this.dotProduct(new Vector(0, -Math.Sin(radians), Math.Cos(radians)))
				);
		}

	public new NonzeroVector rotateAboutY(double radians) {
		return new NonzeroVector(
				this.dotProduct(new Vector(Math.Cos(radians), 0, -Math.Sin(radians))),
				this.y,
				this.dotProduct(new Vector(Math.Sin(radians), 0, Math.Cos(radians)))
				);
		}

	public new NonzeroVector rotateAboutZ(double radians) {
		return new NonzeroVector(this.dotProduct(new Vector(Math.Cos(radians), -Math.Sin(radians), 0)),
				this.dotProduct(new Vector(Math.Sin(radians), Math.Cos(radians), 0)),
				this.z);
		}



}
}