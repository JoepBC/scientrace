// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class VectorTransformOrthonormal : VectorTransform {

		public VectorTransformOrthonormal(NonzeroVector u, NonzeroVector v, NonzeroVector w) : base(u,v,w) {
			if (Math.Abs(this.u.dotProduct(this.v)) + Math.Abs(this.v.dotProduct(this.w)) + Math.Abs(this.w.dotProduct(this.u)) > 0) {
				throw new Exception("Vectors not orthogonal");
			}
			this.normalize();
		}

		public VectorTransformOrthonormal(NonzeroVector u, NonzeroVector v) : base(u,v) {
			//if (this.u.dotProduct(this.v) != 0) {
			//please let's be a bit more tolerant because of rounding-errors
			if (Math.Abs(this.u.dotProduct(this.v)) > 0.000001) {
				throw new Exception("Vectors not orthogonal");
			}
			this.normalize();
		}

		public VectorTransformOrthonormal(Scientrace.NonzeroVector u, Scientrace.NonzeroVector v, int swapz) : base(u,v,swapz) {
			//built in margin of 1E-15 on orthogonal-dotproduct-mismatch for an orthonormal base
			if (Math.Abs(this.u.dotProduct(this.v)) + Math.Abs(this.v.dotProduct(this.w)) + Math.Abs(this.w.dotProduct(this.u)) > 1E-15) {
				Console.Write(this.ToString());
				throw new Exception("Vectors not orthogonal"+Math.Abs(this.u.dotProduct(this.v)) +"v"+ Math.Abs(this.v.dotProduct(this.w)) +"w"+ Math.Abs(this.w.dotProduct(this.u)));
			}
			this.normalize();
		}



		public void normalize() {
			//Console.WriteLine(this.ToCompactString());
			this.u = this.u.tryToUnitVector();
			this.v = this.v.tryToUnitVector();
			this.w = this.w.tryToUnitVector();
			//Console.WriteLine("Normalized: "+u.trico()+", "+v.trico()+w.trico());
			this.initInverseVectors();
			//Console.WriteLine("AFTER:"+this.ToCompactString());
		}

}
}
