// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using OpenTK;
//using System.Linq; //used for commented LINQ method below
using System.Collections.Generic;
using System.Threading;

namespace Scientrace {


public class VectorTransform {
	
	//private static List<VectorTransform> inverseVectorCache = new List<VectorTransform>();
	private static Dictionary<string,VectorTransform> inverseVectorDictionary = new Dictionary<string, VectorTransform>();
	private static Semaphore InverseVectorCacheSema = new Semaphore(1, 1, "InverseVectorCacheSema");
	public static TimeSpan total_inverse_calculation_time = new TimeSpan(0);

	public static bool CACHE_TRANSFORMS = true;

	public string idstring;
	public Scientrace.Vector u, v, w;
	//these cannot be unitvectors
	public Scientrace.Vector ui, vi, wi;
		
	public const int SWAP_U_WITH_W = 1;
	public const int SWAP_V_WITH_W = 2;

	public VectorTransform(Scientrace.NonzeroVector u, Scientrace.NonzeroVector v) {
		//third base coordinate orthogonal on both u and v.
		this.inituvw(u,v,u.crossProduct(v).tryToUnitVector());
	}

	//testing function below
	public VectorTransform(Scientrace.NonzeroVector u, Scientrace.NonzeroVector v, Scientrace.NonzeroVector w) {
		this.inituvw(u,v,w);
	}

	public void inituvw(Scientrace.Vector u, Scientrace.Vector v, Scientrace.Vector w) {
		this.u = u;
		this.v = v;
		this.w = w;
		this.idstring = this.getIdString();
		if (VectorTransform.CACHE_TRANSFORMS) {
			this.cachedInverseVectors(); // Use this method for chached (better performance) transform retrieval
			} else {
			this.notCachedInverseVectors(); // Use this method for chached (less memory usage) transform retrieval
			}
		}

	public string getIdString() {
		return this.u.ToString()+"-"+this.v.ToString()+"-"+this.w.ToString();
		}


	public bool cachedInverseVectors() {
		//return this.LINQListCachedInverseVectors(); //method commented out due to unnecessary use of resources
		return this.dictionaryCachedInverseVectors();
		}

	public bool dictionaryCachedInverseVectors() {
		if (VectorTransform.inverseVectorDictionary.ContainsKey(this.idstring)) {
			//Console.WriteLine("Transform u="+this.u+", v="+this.v+", u="+this.w+"; has been FOUND in cache.");
			this.ui = VectorTransform.inverseVectorDictionary[idstring].ui;
			this.vi = VectorTransform.inverseVectorDictionary[idstring].vi;
			this.wi = VectorTransform.inverseVectorDictionary[idstring].wi;
			return true;
			}

		DateTime startTime = DateTime.Now;

		//Console.WriteLine("Transform u="+this.u+", v="+this.v+", u="+this.w+"; not found in cache.");
		this.initInverseVectors();
		DateTime stopTime = DateTime.Now; 
		VectorTransform.total_inverse_calculation_time += (stopTime-startTime);
		
		//LOCK CACHE SEMAPHORE
		VectorTransform.InverseVectorCacheSema.WaitOne();
		//adding calculated item to cache
		if (VectorTransform.inverseVectorDictionary.ContainsKey(this.idstring)) {
			//do not add again, duplicate entry created at same time...
			Console.WriteLine("Duplicate entry...");
			//RELEASE CACHE SEMAPHORE
			VectorTransform.InverseVectorCacheSema.Release();
			return false;
			}
		//Add new entry.
		VectorTransform.inverseVectorDictionary.Add(this.idstring, this);
		//RELEASE CACHE SEMAPHORE
		VectorTransform.InverseVectorCacheSema.Release();
		//Console.WriteLine("Element (u="+this.u+", v="+this.v+", w="+this.w+") added to cache. Current cache size: "+VectorTransform.inverseVectorDictionary.Count);

		return false;
		}

	/*
	/// <summary>
	/// Calculates transformation vectors using inverse matrix but uses (LINQ) caching for calculated data.
	/// </summary>
	/// <returns>
	/// True if inverse vectors retrieved from cache, return false if vectors were calculated
	/// </returns>
	public bool LINQListCachedInverseVectors() {
		IEnumerable<VectorTransform> cachedTransform =
		from Inverse in VectorTransform.inverseVectorCache 
		where (Inverse.idstring == this.idstring)
				// old (working) code TOO SLOW SO REPLACED BY idstring above:
  				//(Inverse.u == this.u) &&
				//(Inverse.v == this.v) &&
				//(Inverse.w == this.w))
		select Inverse;
		if (cachedTransform.Count() > 0) {
			//Console.WriteLine("Transform u="+this.u+", v="+this.v+", u="+this.w+"; has been FOUND in cache.");
			//Console.WriteLine("CACHED VALUE: "+cachedTransform.SingleOrDefault().ToCompactString());
			this.ui = cachedTransform.SingleOrDefault().ui;
			this.vi = cachedTransform.SingleOrDefault().vi;
			this.wi = cachedTransform.SingleOrDefault().wi;
			return true;
			}
		//Console.WriteLine("Transform u="+this.u+", v="+this.v+", u="+this.w+"; not found in cache.");
		this.initInverseVectors();
		
		//adding calculated item to cache
		VectorTransform.inverseVectorCache.Add(this);
		//Console.WriteLine("Element (u="+this.u+", v="+this.v+", w="+this.w+") added to cache. Current cache size: "+VectorTransform.inverseVectorCache.Count);
		return false;
		} */

	public void notCachedInverseVectors() {
		this.initInverseVectors();
		}

	public VectorTransform(Scientrace.NonzeroVector u, Scientrace.NonzeroVector v, int swapz) {
		//third base coordinate orthogonal on both u and v.
		Scientrace.Vector w = u.crossProduct(v).tryToUnitVector();
		switch (swapz) {
			case VectorTransform.SWAP_U_WITH_W:
				//for some reasons it might be usefull to change u and w
				this.inituvw(w,v,u);
				break;
			case VectorTransform.SWAP_V_WITH_W:
				//or v and w
				this.inituvw(u,w,v);
				break;
			default:
				//normal init
				this.inituvw(u,v,w);
				break;
			}
		}


	public void initInverseVectors() {
		this.initInverseVectorsOPENTK();
		// XNA Matrix implementation does not support double precision matrix
		//this.initInverseVectorsXNA();
		// dnAnalytics (old version of Math.Net Numerics) Matrix implementation does not support double precision matrix
		//this.initInverseVectorsDNANALYTICS();
		}

	public void initInverseVectorsOPENTK() {
		Scientrace.Vector x = this.u;
		Scientrace.Vector y = this.v;
		Scientrace.Vector z = this.w;

		OpenTK.Matrix4d m = new OpenTK.Matrix4d(
				x.x, x.y, x.z, 0,
				y.x, y.y, y.z, 0,
				z.x, z.y, z.z, 0,
				0, 0, 0, 1);
		
		m.Invert();
		this.ui = new Scientrace.Vector(m.M11, m.M12, m.M13);
		this.vi = new Scientrace.Vector(m.M21, m.M22, m.M23);
		this.wi = new Scientrace.Vector(m.M31, m.M32, m.M33);

		}

	public void initInverseVectorsXNA() {
		//add to headers: using Microsoft.Xna.Framework;
		/* Scientrace.Vector x = this.u;
		Scientrace.Vector y = this.v;
		Scientrace.Vector z = this.w;

		Microsoft.Xna.Framework.Matrix m = new Microsoft.Xna.Framework.Matrix(
				(float)x.x, (float)x.y, (float)x.z, 0,
				(float)y.x, (float)y.y, (float)y.z, 0,
				(float)z.x, (float)z.y, (float)z.z, 0,
				0, 0, 0, 1);

		Microsoft.Xna.Framework.Matrix im = Microsoft.Xna.Framework.Matrix.Invert(m);
		this.ui = new Scientrace.Vector(im.M11, im.M12, im.M13);
		this.vi = new Scientrace.Vector(im.M21, im.M22, im.M23);
		this.wi = new Scientrace.Vector(im.M31, im.M32, im.M33); /**/
		}

	public void initInverseVectorsDNANALYTICS() {
		//add to headers: using dnAnalytics.LinearAlgebra;
		/*
		//abuse the MathNet lib for its inverse matrix function.
		double[,] mat = this.u.to3x3MatrixWith(this.v, this.w);
		//MathNet.Numerics.LinearAlgebra.Double.DenseMatrix m = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(mat);
		dnAnalytics.LinearAlgebra.DenseMatrix m = new dnAnalytics.LinearAlgebra.DenseMatrix(mat);

		//double[][] am = m.Inverse().CopyToJaggedArray();
		double[,] am = m.Inverse().ToArray();
		// column vectors
		this.ui = new Scientrace.Vector(am[0,0], am[1,0], am[2,0]);
		this.vi = new Scientrace.Vector(am[0,1], am[1,1], am[2,1]);
		this.wi = new Scientrace.Vector(am[0,2], am[1,2], am[2,2]);/**/
		}	
	
	public static string matrixToString(double[,] aMatrix) {
		return "["+aMatrix[0,0].ToString()+", "+aMatrix[0,1].ToString()+", "+aMatrix[0,2].ToString()+";\n"+aMatrix[1,0].ToString()+", "+aMatrix[1,1].ToString()+", "+aMatrix[1,2].ToString()+";\n"+aMatrix[2,0].ToString()+", "+aMatrix[2,1].ToString()+", "+aMatrix[2,2].ToString()+"];";
		}

	public static string compactMatrixToString(double[,] aMatrix) {
		string retstr = "";
		retstr += "[";
		for (int ir = 0; ir < 3; ir++) {
			for (int ic = 0; ic < 3; ic++) {
				retstr += String.Format("{0:0.00}", aMatrix[ir,ic]);
				if (ic < 2) {
					retstr += ", ";
				} else {
					if (ir < 2) {
						retstr += "; \n";
					}
				}
			}
		}
		return retstr+"]; ";
	}

	public static string oldfcompactMatrixToString(double[][] aMatrix) {
		return "["+String.Format("{0:0.00}", aMatrix[0][0])+", "+String.Format("{0:0.00}", aMatrix[0][1])+", "+String.Format("{0:0.00}", aMatrix[0][2])+";\n"+String.Format("{0:0.00}", aMatrix[1][0])+", "+String.Format("{0:0.00}", aMatrix[1][1])+", "+String.Format("{0:0.00}", aMatrix[1][2])+";\n"+String.Format("{0:0.00}", aMatrix[2][0])+", "+String.Format("{0:0.00}", aMatrix[2][1])+", "+String.Format("{0:0.00}", aMatrix[2][2])+"];";
	}


	public string ToCompactString() {
		return "tm = \n"+Scientrace.VectorTransform.compactMatrixToString(this.u.to3x3MatrixWith(v, w))+"\nitm = \n"+Scientrace.VectorTransform.compactMatrixToString(this.ui.to3x3MatrixWith(vi, wi));
	}

	public override string ToString() {
		//exports a matrix presentation of transformation and inverse transformation
		//which can be easily be imported in matlab or scilab
		return "u = "+this.u.ToString()+"\nv = "+this.v.ToString()+"\nw = "+this.w.ToString()+"\ntm = "+Scientrace.VectorTransform.matrixToString(this.u.to3x3MatrixWith(v, w))+"\nitm = "+Scientrace.VectorTransform.matrixToString(this.ui.to3x3MatrixWith(vi, wi));
	}

	public Scientrace.Vector dotProduct(Scientrace.Vector aVector) {
		return this.dotProductMatrixWithVector(this.u, this.v, this.w, aVector);
	}

	public Scientrace.Vector dotDivide(Scientrace.Vector aVector) {
		return this.dotProductMatrixWithVector(this.ui, this.vi, this.wi, aVector);
	}
	public Scientrace.Vector dotProductMatrixWithVector(Scientrace.Vector a, Scientrace.Vector b, Scientrace.Vector c, Scientrace.Vector aVector) {
		Scientrace.Vector retvec = new Scientrace.Vector(0, 0, 0);
		Scientrace.Vector tvec = null;
		for (int irow = 0; irow <= 2; irow++) {
			tvec = new Vector(a.readIndex(irow), b.readIndex(irow), c.readIndex(irow));
			retvec.onIndexWrite(irow, tvec.dotProduct(aVector));
		}
		return retvec;
	}

	public Scientrace.Vector transform(Scientrace.Vector aVector) {
		return this.dotDivide(aVector);
	}

	public Scientrace.Vector transformback(Scientrace.Vector aVector) {
		return this.dotProduct(aVector);
	}


	public Scientrace.NonzeroVector transform(Scientrace.NonzeroVector aNZVector) {
		return (this.dotDivide(aNZVector)).tryToNonzeroVector();
	}

	public Scientrace.NonzeroVector transformback(Scientrace.NonzeroVector aNZVector) {
		return (this.dotProduct(aNZVector)).tryToNonzeroVector();
	}


	public Scientrace.Location transform(Scientrace.Location loc) {
		return this.transform(loc.toVector()).toLocation();
		}

	public Scientrace.Location transformback(Scientrace.Location loc) {
		return this.transformback(loc.toVector()).toLocation();
		}

	public Scientrace.Line transform(Scientrace.Line aLine) {
		return new Scientrace.Line(this.transform(aLine.startingpoint).toLocation(),
			                           this.transform(aLine.direction).tryToUnitVector());
	}

	public Scientrace.Line transformback(Scientrace.Line aLine) {
		return new Scientrace.Line(this.transformback(aLine.startingpoint).toLocation(),
			                           this.transformback(aLine.direction).tryToUnitVector());
	}

	public Scientrace.Plane transform(Scientrace.Plane plane) {
		return new Scientrace.Plane(this.transform(plane.loc), this.transform(plane.u).tryToNonzeroVector(), this.transform(plane.v).tryToNonzeroVector());
	}

	public Scientrace.Plane transformback(Scientrace.Plane plane) {
		return new Scientrace.Plane(this.transformback(plane.loc), this.transformback(plane.u).tryToNonzeroVector(), this.transformback(plane.v).tryToNonzeroVector());
	}

/*	public Scientrace.IntersectionPoint transform(Scientrace.IntersectionPoint ip) {
		return new Scientrace.IntersectionPoint(this.transform(ip.loc), this.transform(ip.flatshape.plane));	
	}
*/ //THIS BECAME DIFFICULT WHILE UPDATING FROM PLANE TO SHAPE2D
/*
	public Scientrace.IntersectionPoint transformback(Scientrace.IntersectionPoint ip) {
		return new Scientrace.IntersectionPoint(this.transformback(ip.loc), this.transformback(ip.flatshape.plane));	
	}
*/
	public Scientrace.Trace transform(Scientrace.Trace trace) {
		Scientrace.Trace tc = trace.clone();
		tc.traceline = this.transform(trace.traceline);
		return tc;
	}

	public Scientrace.Trace transformback(Scientrace.Trace trace) {
		Scientrace.Trace tc = trace.clone();
		tc.traceline = this.transformback(trace.traceline);
		return tc;
	}


}
}
