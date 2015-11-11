// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {


public class UnitVector : NonzeroVector {

	public UnitVector(double x, double y, double z) : base(x, y, z) {
		this.init_unit();
		}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="copyVec">source data to be copied</param>
	public UnitVector(UnitVector copyVec) : base(copyVec.x, copyVec.y, copyVec.z) {
		this.init_unit();
		}

	public new static UnitVector x1vector() {
		return new UnitVector(1,0,0);
		}
	public new static UnitVector y1vector() {
		return new UnitVector(0,1,0);
		}
	public new static UnitVector z1vector() {
		return new UnitVector(0,0,1);
		}
		
	public void init_unit() {
		this.length = 1;
		this.x = this.xn;
		this.y = this.yn;
		this.z = this.zn;		
		}

	public new UnitVector negative() {
		return new UnitVector(-this.x, -this.y, -this.z);
		}

	public new UnitVector orientedAlong(Scientrace.Vector aVector) {
		if (aVector.dotProduct(this) < 0) {
			return this.negative();
			}
		return this;
		}

	public new UnitVector orientedAgainst(Scientrace.Vector aVector) {
		if (aVector.dotProduct(this) > 0) {
			return this.negative();
			}
		return this;
		}

	public new UnitVector reflectOnSurface(NonzeroVector norm) {
		return base.reflectOnSurface(norm).toUnitVector();
		}

	public void check() {
			if (((Math.Pow(this.x,2)+Math.Pow(this.y,2)+Math.Pow(this.z,2)) < 0.99999) || ((Math.Pow(this.x,2)+Math.Pow(this.y,2)+Math.Pow(this.z,2)) > 1.00001)) {
				throw new ArithmeticException((Math.Pow(this.x,2)+Math.Pow(this.y,2)+Math.Pow(this.z,2)).ToString()+" != 1 ");
			}
		}

	public new Scientrace.UnitVector copy() {
		return new Scientrace.UnitVector(this);
		//return new Scientrace.UnitVector(this.x, this.y, this.z);
		}

}
}
