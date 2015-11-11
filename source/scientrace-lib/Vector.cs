// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Diagnostics;
using System.Reflection;

namespace Scientrace {

public class Vector {

	/*
	 * Object state
	 */
	public double x, y, z;
	public double length;

	/*
	 * Constructor and initializing methods
	 */
	public Vector(double x, double y, double z) {
		this.init(x, y, z);
		if (!this.isValid()) {
						throw new ArgumentNullException("NaN values assigned to Vector.");
			/*StackTrace stackTrace = new StackTrace();
			MethodBase methodBase = stackTrace.GetFrame(2).GetMethod();
			//string filename = stackTrace.GetFrame(1).GetFileName();
			string filename = stackTrace.GetFrame(2).ToString();
			Console.WriteLine("WARNING: NaN values assigned by: "+methodBase.Name+methodBase+" from "+filename); */
			}
		}

	/// <summary>
	/// Copy Constructor, Initializes a new instance of the <see cref="Scientrace.Vector"/> class.
	/// </summary>
	/// <param name="copySource">The source vector to be copied.</param>
	public Vector(Vector copySource) {
		this.init(copySource.x, copySource.y, copySource.z);
		}

	public void init(double x, double y, double z) {
		this.x = x;
		this.y = y;
		this.z = z;			
		this.update();
		}

	public void update() {
		// recalculate length after (re)setting x,y and/or z
		this.length = Math.Sqrt((this.x*this.x)+(this.y*this.y)+(this.z*this.z));
		}

	/*	*** removed to minimise the number of dependencies ***
	public Vector(MathNet.Numerics.LinearAlgebra.Vector v) {
		double x, y, z;
		x = v.CopyToArray()[0];
		y = v.CopyToArray()[0];
		z = v.CopyToArray()[0];
		this.init(x, y, z);
		}		 */

	public static Vector ZeroVector() {
		return new Vector(0, 0, 0);
		}

	public static Vector x1vector() {
		return new Vector(1,0,0);
		}
	public static Vector y1vector() {
		return new Vector(0,1,0);
		}
	public static Vector z1vector() {
		return new Vector(0,0,1);
		}

	/// <summary>
	/// If a vector 1,0,0 would be transformed back by vectortransform trf this would be its length
	/// </summary>
	/// <param name="trf">
	/// A <see cref="Scientrace.VectorTransform"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Double"/> the length of the backtransform of 1,0,0
	/// </returns>
	public static double x1trbl(Scientrace.VectorTransform trf) {
		return trf.transformback(Scientrace.Vector.x1vector()).length;	
		}
	/// <summary>
	/// If a vector 0,1,0 would be transformed back by vectortransform trf this would be its length
	/// </summary>
	/// <param name="trf">
	/// A <see cref="Scientrace.VectorTransform"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Double"/> the length of the backtransform of 0,1,0
	/// </returns>
	public static double y1trbl(Scientrace.VectorTransform trf) {
		return trf.transformback(Scientrace.Vector.y1vector()).length;	
		}
	/// <summary>
	/// If a vector 0,0,1 would be transformed back by vectortransform trf this would be its length
	/// </summary>
	/// <param name="trf">
	/// A <see cref="Scientrace.VectorTransform"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Double"/> the length of the backtransform of 0,0,1
	/// </returns>
	public static double z1trbl(Scientrace.VectorTransform trf) {
		return trf.transformback(Scientrace.Vector.z1vector()).length;	
		}
			
			
	/*
	 * Overloaded operators
	 */
	public static Vector operator -(Scientrace.Vector v1, Scientrace.Vector v2) {
		return new Vector(v1.x-v2.x, v1.y-v2.y, v1.z-v2.z);
		}

	public static Vector operator *(Scientrace.Vector v1, double scalar) {
		return new Vector(v1.x*scalar, v1.y*scalar, v1.z*scalar);
		}

	public static Vector operator /(Scientrace.Vector v1, double scalar) {
		return new Vector(v1.x/scalar, v1.y/scalar, v1.z/scalar);
		}

	public static Vector operator +(Scientrace.Vector v1, Scientrace.Vector v2) {
		return new Vector(v1.x+v2.x, v1.y+v2.y, v1.z+v2.z);
		}

	public static bool operator ==(Scientrace.Vector v1, Scientrace.Vector v2) {
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(v1, v2)) {
			return true;
		}
		// If one is null, but not both, return false.
		if (((object)v1 == null) || ((object)v2 == null)) {
			return false;
		}
		
		//poking whether state v1 equals state v2
		return ((v1.x == v2.x) && (v1.y == v2.y) && (v1.z == v2.z) && (v1.length == v2.length));
	}

	public static bool operator !=(Scientrace.Vector v1, Scientrace.Vector v2) {
		return !(v1 == v2);
		//This will call to operator == simple way to implement !=
	}

	public override bool Equals(object o) {
		return this == (Scientrace.Vector)o;
	}

	public override int GetHashCode() {
		return base.GetHashCode();
	}


	/*
	 * Overriden methods
	 */
	public override string ToString() {
		//exporting matlab/scilab compatible string
		return "["+x.ToString()+", "+y.ToString()+", "+z.ToString()+"]"; //\n%length: "+this.length;
	}
		
	public string pragmaticFloatToString(double aFloat) {
		string eformatting = "{0:0.####E+0}";
		string aformatting = "{0:0.####}";
		return (((Math.Abs(aFloat) < 0.01) && (aFloat!=0)) ? String.Format(eformatting, aFloat) : String.Format(aformatting, aFloat));
		}	
			
	public string tricoshort() {
		string glue = ";";
		string start = "(";
		string end = ")";
		
		return start+this.pragmaticFloatToString(this.x)+
				glue+this.pragmaticFloatToString(this.y)+
				glue+this.pragmaticFloatToString(this.z)+
				end;
		}
		
		
	public string exportWithGlue(string glue) {
		if (!this.isValid())
			throw new NullReferenceException("Cannot export Vector with NaN values.");
		return this.x.ToString()+glue+this.y.ToString()+glue+this.z.ToString();
	}

	public string trico() {
		//export as "1 2 3" for x=1, y=2, z=3. For export purposes.
		return this.exportWithGlue(" ");
	}
	
	public bool isValid() {
		if (Double.IsNaN(this.x)) 
			return false;
		if (Double.IsNaN(this.y)) 
			return false;
		if (Double.IsNaN(this.z)) 
			return false;
		return true;
		}
		
	/// <summary>
	/// In some situations, a vector should explicitly not lie parallel to another vector. 
	/// The vectordance function will make sure a vector will make an angle of at least 45 degrees with
	/// the original vector, but always less than 135 degrees, by rotating twice, first 90 degrees about
	/// the X axis, secondly 90 degrees about the Y axis.
	/// </summary>
	/// <returns>
	/// A rotated <see cref="Vector"/>
	/// </returns>
	public Vector vectorDance() {
		return (this.rotateAboutX(Math.PI/2).rotateAboutY(Math.PI/2));
		}

	
	public string tricon() {
		//export as "1 2 3" for x=1, y=2, z=3 ending with a \n (newline). For export purposes.
		return this.trico()+"\n";
	}

	/*
	 * Accessor methods
	 */
	public double readIndex(int index) {
		switch (index) {
			case 0:
				return this.x;
			case 1:
				return this.y;
			case 2:
				return this.z;
			default:
				throw new IndexOutOfRangeException();
			// unreachable code -> break;
		}
	}

	public void onIndexWrite(int index, double val) {
		switch (index) {
			case 0:
				this.x = val;
				this.update();
				break;
			case 1:
				this.y = val;
				this.update();
				break;
			case 2:
				this.z = val;
				this.update();
				break;
			default:
				throw new IndexOutOfRangeException();
			// unreachable code -> break;
		}
	}


	/*
	 * Additional methods
	 */
	public string ToCompactString() {
		//exporting matlab/scilab compatible string on 4 decimals to prevent occurences of 0,9999999999999 instead of 1 etc.
		return "["+String.Format("{0:0.0000}", x)+", "+String.Format("{0:0.0000}", y)+", "+String.Format("{0:0.0000}", z)+"]\n%length: "+String.Format("{0:0.0000}", this.length);
	}

	//The sign of the sin functions could be changed following the example on http://en.wikipedia.org/wiki/Rotation_matrix as for juli 15th 2010
	public Vector rotateAboutX(double radians) {
		return new Vector(
				this.x,
				this.dotProduct(new Vector(0, Math.Cos(radians), -Math.Sin(radians))),
				this.dotProduct(new Vector(0, Math.Sin(radians), Math.Cos(radians)))
				);
		}

	public Vector projectOnDirection(UnitVector aDirection) {
		return aDirection.toVector()*this.dotProduct(aDirection);
		}

	/// <summary>
	/// Projects a Vector on a plane with a given normal. 
	/// This may also be interpreted as the distance from a vector to its projection on a direction.
	/// </summary>
	/// <returns>The vector component orthogonal to the surface normal.</returns>
	/// <param name="normal">Normal.</param>
	public Vector projectOnPlaneWithNormal(UnitVector normal) {
		Vector normal_component = this.projectOnDirection(normal);
		return this-normal_component;
		}

	public Vector rotateAboutY(double radians) {
		return new Vector(
				this.dotProduct(new Vector(Math.Cos(radians), 0, Math.Sin(radians))),
				this.y,
				this.dotProduct(new Vector(-Math.Sin(radians), 0, Math.Cos(radians)))
				);
		}

	public Vector rotateAboutZ(double radians) {
		return new Vector(
				this.dotProduct(new Vector(Math.Cos(radians), -Math.Sin(radians), 0)),
				this.dotProduct(new Vector(Math.Sin(radians), Math.Cos(radians), 0)),
				this.z);
		}
	

	/// <summary>
	/// Use function below to transform a vector over any angle v. Keep in mind that this
	/// function uses significantly more processingpower than the aboutX,Y,Z functions.
	/// </summary>
	/// <param name="v">
	/// A <see cref="Scientrace.NonzeroVector"/> about which the vector should be rotated
	/// </param>
	/// <param name="radians">
	/// A <see cref="System.Double"/> an angle in radians about which the vetor should be rotated
	/// anticlockwise with the vector V rising towards the viewer.
	/// </param>
	/// <returns>
	/// A <see cref="Vector"/> which is the result of the rotating operation. The original (this) 
	/// vector is not modified during this operation.
	/// </returns>
	public Vector rotateAboutVector(Scientrace.NonzeroVector v, double radians) {
		NonzeroVector tv1;
		if (Math.Abs(new NonzeroVector(1, 0, 0).normalized().dotProduct(v.normalized())) < 0.9) {
			//throw new Exception("A: "+v.trico()+" cross: "+new NonzeroVector(1,0,0).crossProduct(v));
			tv1 = new NonzeroVector(1,0,0).crossProduct(v).tryToNonzeroVector().normalized();
			} else {
			tv1 = new NonzeroVector(0,1,0).crossProduct(v).tryToNonzeroVector().normalized();
			}
		NonzeroVector tv2 = v.crossProduct(tv1).tryToNonzeroVector().normalized();
		VectorTransformOrthonormal vtrf = new VectorTransformOrthonormal(tv1, tv2, v);
		Vector tvec = vtrf.transform(this);
		tvec = tvec.rotateAboutZ(radians);
		return vtrf.transformback(tvec);
		}

	public Vector normaliseIfNotZero() {
		if (this.length == 0) return this;
		return this/this.length;
		}

	public Vector crossProduct(Scientrace.Vector v) {
		double a1, a2, a3, b1, b2, b3;
		//renaming variables for readability purposes. Has very little/no influence on performance.
		a1 = this.x;
		a2 = this.y;
		a3 = this.z;
		b1 = v.x;
		b2 = v.y;
		b3 = v.z;
		return new Scientrace.Vector(a2*b3-a3*b2, a3*b1-a1*b3, a1*b2-a2*b1);
	}
	
	/// <summary>
	/// When two vectors are parallel, the cross product cannot be normalised. This method hacks around that problem.
	/// </summary>
	/// <returns>A UnitVector</returns>
	/// <param name="aVector">A second vector to which the result will be orthogonal.</param>
	public UnitVector safeNormalisedCrossProduct(Scientrace.Vector aVector) {
		if (this.length*aVector.length == 0) return UnitVector.x1vector();
		Vector crossVector = this.crossProduct(aVector);
		if (crossVector.length == 0) {
			return this.safeNormalisedCrossProduct(aVector.vectorDance());
			}
		return crossVector.tryToUnitVector();
		}

	public Vector elementWiseProduct(Scientrace.Vector v) {
		return new Vector(this.x*v.x, this.y*v.y, this.z*v.z);
		}

	public Scientrace.Location toLocation() {
		return new Scientrace.Location(this.x, this.y, this.z);
	}

	public Scientrace.UnitVector tryToUnitVector(string error_message) {
		if (this.length == 0) {
			throw new ZeroNonzeroVectorException(error_message);
			}
		return this.tryToUnitVector();
		}

	public Scientrace.UnitVector tryToUnitVector() {
		return new Scientrace.UnitVector(this.x, this.y, this.z);
	}

	public Scientrace.NonzeroVector tryToNonzeroVector(string error_message) {
		if (this.length == 0) {
			throw new ZeroNonzeroVectorException(error_message);
			}
		return this.tryToNonzeroVector();
		}

	public Scientrace.NonzeroVector tryToNonzeroVector() {
		return new Scientrace.NonzeroVector(this.x, this.y, this.z);
		}

	public double dotProduct(Scientrace.Vector v) {
		return (this.x)*(v.x)+(this.y)*(v.y)+(this.z)*(v.z);
	}
		
	public Scientrace.Vector toVector() {
		return new Scientrace.Vector(this.x, this.y, this.z);
		}

	public virtual Scientrace.Vector copy() {
		return this.toVector();
		}

	public double roundForInverseAngleFunction(double d) {
		//this function makes sure an inverse Sine or Cosine of 1.000000000000000000001 and dito -1.000..1 
		//due to rounding can be obtained as if it were resp. 1 and -1.
		if (Math.Abs(d) <= 1)
			return d;
		//ugly spaghetti below, but quick and dirty fix has no relevant consequences.
		if ((d > 1) && (d < 1.0000000000001)) {
			return 1;
		} else {
			if ((d < -1) && (d > -1.0000000000001)) {
				return -1;
			} else {
				throw new DivideByZeroException("No InverseAngleFunction possible over "+d.ToString());
			}
		}
	}

	public double angleWith(Scientrace.Vector v) {
		return Math.Acos(this.roundForInverseAngleFunction(this.tryToUnitVector().dotProduct(v.tryToUnitVector())));
	}

	public Vector negative() {
		return new Vector(-this.x, -this.y, -this.z);
	}

	/// <summary>
	/// Constructs the hypothenuse from the triangle which has this vector as opposite (perpendicular) side,
	/// and an adjacent (base) side in the direction of "adjacentDirection"
	/// </summary>
	/// <returns>The hypothenuse (unitvector) for a right triangle where hypothenuse lenght = 1. </returns>
	/// <param name="adjacentDirection">The direction of the Adjacent side of the triangle.</param>
	public UnitVector constructHypothenuse(UnitVector adjacentDirection) {
		double ls = this.length*this.length;
		double adjacent_length = Math.Sqrt(1.0-ls);
		return (adjacentDirection*adjacent_length + this).tryToUnitVector();
		}
		
	public double[,] to3x3MatrixWith(Vector v2, Vector v3) {
		//produce a matrix with this, v2 and v3 as column (vertical) vectors. 
		//matlab notation [v1,v2,v3] instead of [v1;v2;v3]
		Vector[] v = {
			this,
			v2,
			v3
		};
		double[,] dm = new double[3, 3];
		
		/*	for (int i = 0; i < 3; i++) {
		// creating a 3*3 jagged array
		dm[i] = new double[3];
		}*/
		//entering the data
		for (int i = 0; i < 3; i++) {
			//code for column vectors matrix:
			dm[0, i] = v[i].x;
			dm[1, i] = v[i].y;
			dm[2, i] = v[i].z;
		}
		/*	//code for row vectors matrix -NOT USED@2010/1/7-
			dm[i][0] = v[i].x;
			dm[i][1] = v[i].y;
			dm[i][2] = v[i].z;*/		
		
		return dm;
	}

	public Vector reflectOnSurface(Scientrace.NonzeroVector norm) {
		Scientrace.Vector negdir = this.negative();

		// Relative mirrorpoint: disregard the location of the line, just the orientations.
		Scientrace.Vector rel_mirrorpoint = (norm.toVector()*negdir.dotProduct(norm));

		//the distance from the surface normal to the direction vector (in the opposite direction) of the incoming line
		Scientrace.Vector distance_to_normal = (negdir - rel_mirrorpoint);

		//the direction of the reflected line
		return (rel_mirrorpoint-distance_to_normal); 
		}
//.tryToUnitVector()

	public Vector orientedAlong(Scientrace.Vector aVector) {
		if (aVector.dotProduct(this) < 0) {
			return this.negative();
			}
		return this;
		}

	public Vector orientedAgainst(Scientrace.Vector aVector) {
		if (aVector.dotProduct(this) > 0) {
			return this.negative();
			//this.negated();
			}
		return this;
		}


	/*	public MathNet.Numerics.LinearAlgebra.Vector toMNVector() {
		//MathNet.Numerics.LinearAlgebra.Vector v = 
		return new MathNet.Numerics.LinearAlgebra.Vector(new double[] {
			this.x,
			this.y,
			this.z
		});
	}		 */	
}
	
}
