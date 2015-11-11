// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class Plane {
	public Scientrace.NonzeroVector u, v;
	protected Scientrace.NonzeroVector tu, tv;
	protected Scientrace.VectorTransform trf = null;

	public Scientrace.Location loc;
	//the this.loc transformed by this.trf . For efficiency reasons "just" a Vector
	protected Scientrace.Vector eloc;


	public Plane(Scientrace.Location loc, Scientrace.NonzeroVector u, Scientrace.NonzeroVector v) {
		this.loc = loc;
		this.u = u;
		this.v = v;
		}

	/// <summary>
	/// Copy Constructor Initializes a new instance of the <see cref="Scientrace.Plane"/> class.
	/// </summary>
	/// <param name='aPlane'>
	/// A plane instance with the same (copied) values (yet different instances) as parameters.
	/// </param>
	public Plane(Scientrace.Plane aPlane) {
		this.loc = aPlane.loc.copy();
		this.u = aPlane.u.copy();
		this.v = aPlane.v.copy();
		}

	public Scientrace.Location getOrigin() {
		return this.loc;
	}


	public Scientrace.Vector geteloc() {
		//construction on request
		if (this.eloc == null) {
			this.eloc = this.getTransform().transform(this.loc);
		}
		return this.eloc;
	}

	/// <summary>
	/// Factory method. Creates a plane through the origin (location 0,0,0) perpendicular to vector 'vec'
	/// </summary>
	/// <param name="vec">
	/// A <see cref="Scientrace.NonzeroVector"/> which is a normal to the created surface in the origin.
	/// </param>
	/// <returns>
	/// A <see cref="Scientrace.Plane"/> perpendicular to vec (or it's reciprocal value in some definitions)
	/// </returns>
	public static Scientrace.Plane newPlaneOrthogonalTo(Scientrace.Location loc, Scientrace.NonzeroVector vec) {
		//Console.WriteLine("short newPlaneOrthogonalTo METHOD START");
		return Scientrace.Plane.newPlaneOrthogonalTo(loc, vec, 
			Scientrace.NonzeroVector.x1vector(),
			Scientrace.NonzeroVector.y1vector());
		/* OBSOLETE CODE:
		Scientrace.UnitVector unitvec = vec.tryToUnitVector();
		Scientrace.UnitVector refvec = 
			(
			(vec.dotProduct(Scientrace.Vector.x1vector()) < 0.1) ?
			Scientrace.Vector.x1vector() :
			Scientrace.Vector.y1vector() 
			).tryToUnitVector();
		Scientrace.NonzeroVector u = unitvec.crossProduct(refvec).tryToUnitVector();
		Scientrace.NonzeroVector v = unitvec.crossProduct(u).tryToUnitVector();
		return new Scientrace.Plane(new Scientrace.Location(0,0,0), u,v); */
		}
	
	public static Scientrace.Plane newPlaneOrthogonalTo(Scientrace.Location loc, Scientrace.NonzeroVector normalVec, 
		Scientrace.NonzeroVector helpVec1, Scientrace.NonzeroVector helpVec2) {
		Scientrace.UnitVector unitvec = normalVec.toUnitVector();
		Scientrace.UnitVector h1 = helpVec1.toUnitVector();
		Scientrace.UnitVector h2 = helpVec2.toUnitVector();
		if (h1.dotProduct(h2) >= 1) {
			throw new Scientrace.ParallelVectorException(h1,h2, " in newPlaneOrthogonalTo(3) factory method.");
			}
		Scientrace.UnitVector refvec = 
			(
			(Math.Abs(unitvec.dotProduct(h1)) < 0.8) ?
			h1 :
			h2
			);
		Scientrace.NonzeroVector u = unitvec.crossProduct(refvec).tryToUnitVector();
		Scientrace.NonzeroVector v = unitvec.crossProduct(u).tryToUnitVector();
		return new Scientrace.Plane(loc, u,v);
		}		
		
		
/* OBSOLETE CODE

	private static Scientrace.Plane zequals0 = null;
	private static Scientrace.Plane zequals1 = null;

	public static Scientrace.Plane getzequals0plane() {
		if (Plane.zequals0 == null) {
			Plane.zequals0 = new Scientrace.Plane(new Location(0,0,0), new UnitVector(1,0,0), new UnitVector(0,1,0));
		}
		return Plane.zequals0;
	}


	public static Scientrace.Plane getzequals1plane() {
		if (Plane.zequals1 == null) {
			Plane.zequals1 = new Scientrace.Plane(new Location(0,0,1), new UnitVector(1,0,0), new UnitVector(0,1,0));
		}
		return Plane.zequals1;
	} */

	public string x3dRotationTransform() {
		return this.x3dRotationTransform(new Scientrace.Vector(0, 0, 1));
	}

		
	public string x3dRotationTransform(Scientrace.Vector orivec) {
		//the Circle2D objects will normally be drawn in the x,y plane (perpendicular to 0,0,1) so it has to
		//rotated out of that plane as follows.
		Scientrace.UnitVector oriunivec = orivec.tryToUnitVector();
		Scientrace.UnitVector uxv = (this.u.crossProduct(this.v).tryToUnitVector());
		if (uxv == oriunivec) {
			return "<Transform rotation='0 0 0 0'>";
		//	throw new ArithmeticException("Plane vectors uxv ("+uxv.trico()+") & oriunivec ("+oriunivec.trico()+") are equal");
			}
		try {
		Scientrace.UnitVector rotaxis = (oriunivec).crossProduct(uxv).tryToUnitVector();
		return "<Transform rotation='"+rotaxis.trico()+" "+(oriunivec.angleWith(rotaxis)).ToString()+"'>";
		} catch // possible error: zerovector after cross product (rounding error at first comparison), return zero rotation tag.
		{ return "<Transform rotation='0 0 0 0'>"; }
	}



	public Scientrace.UnitVector getNorm() {
		//the VectorTransform this.trf object already produced a cross product over this.u and this.v,
		//resulting in normalized (unit-vector) norm on the surface.
		Scientrace.VectorTransform trf = this.getTransform();
		return trf.w.tryToUnitVector();
	}

	public Scientrace.VectorTransform getTransform() {
		//construction on request
		if (this.trf == null) {
			this.trf = new Scientrace.VectorTransform(this.u, this.v);
		}
		return this.trf;
	}

	public Scientrace.Location lineThroughPlane(Scientrace.Line line) {
		/*if (line.direction.dotProduct(this.getNorm())==0) {
						throw new Exception("Line ("+line.ToString()+") is parallel to plane ("+this.getNorm().ToString()+")! Cannot calculate intersection!");
			}
		if (line.direction.dotProduct(this.getNorm()) < MainClass.SIGNIFICANTLY_SMALL) {
						throw new Exception("Line ("+line.ToString()+") is parallel to plane ("+this.getNorm().ToString()+")! Cannot calculate intersection!");
			} */
		Scientrace.Vector transformedLoc = this.lineThroughPlaneTLoc(line);
		if (transformedLoc == null) {
			return null;
			//throw new Exception("Transformed location is null, line is parallel to plane!"); 
			}
		return this.lineThroughPlaneOLoc(transformedLoc);
		}

	// Transform transformed location vector back to original base
	public Scientrace.Location lineThroughPlaneOLoc(Scientrace.Vector tloc) {
		if (tloc == null) {
			throw new Exception("Cannot transform a null vector (location).");
			}
		return this.getTransform().transformback(tloc).toLocation();
		}

	public override string ToString () {
		return "[plane "+this.u.trico()+"; "+this.v.trico()+"@+"+this.loc.trico()+"]";
	}


	public Scientrace.Vector lineThroughPlaneTLoc(Scientrace.Line line) {
		Scientrace.VectorTransform trf = this.getTransform();
		//coordinates after coordinate-transformation. u->e1, v->e2, loc->eloc
		//the parallelogram is in the (a*e1, b*e2, 0) plane
		Scientrace.Vector eloc;
		eloc = this.geteloc();
		//coordinates after coordinate-transformation. line.direction->eld, line.loc->ell
		Scientrace.Vector eld, ell;
		eld = trf.transform(line.direction);
		ell = trf.transform(line.startingpoint);
		//nbase is the new base when the parallelogram "location" has been substracted from the line-location
		Scientrace.Vector nbase = ell-eloc;
		
		//catch direction eld.z = 0; line is parallel to plane!
		if (eld.z == 0) {
			return null;
		}
		
		double nx, ny, nz;
		double dfac = (nbase.z/eld.z);
		nx = nbase.x-(dfac*eld.x);
		ny = nbase.y-(dfac*eld.y);
		nz = nbase.z-(dfac*eld.z);
		
		//shift plane back to eloc location and return
		return (new Scientrace.Vector(nx, ny, nz))+eloc;
	}
	
	public bool isParallelTo(Plane anotherPlane) {
		Scientrace.UnitVector n1, n2;
		n1 = this.getNorm();
		n2 = anotherPlane.getNorm();
		return (n1.crossProduct(n2).length != 0);		
		}
	
	/// <summary>
	/// Creates a line starting at the projection of this.loc on the intersection between this and another Plane. 
	/// The direction of the line is that of the crossproduct of this.normal and anotherPlane.normal.
	/// </summary>
	/// <returns>A line along the intersection of this and anotherPlane, returns "null" if two planes are significantly parallel</returns>
	/// <param name="anotherPlane">A plane to calculate the intersection with</param>
	public Scientrace.Line getIntersectionLineWith(Scientrace.Plane anotherPlane) {
		Scientrace.UnitVector n1, n2;
		n1 = this.getNorm();
		n2 = anotherPlane.getNorm();
		Vector tw = n1.crossProduct(n2);
		if (tw.length < MainClass.SIGNIFICANTLY_SMALL) {
			//Console.WriteLine("WARNING: two planes do not intersect since they are parallel.");
			return null;
			}
		UnitVector intersection_direction = tw.tryToUnitVector();	

		Vector tl1 = n1.crossProduct(intersection_direction);
		if (tl1.length == 0)
			throw new Exception("Plane.getIntersectionLineWith/l1.length can never be zero. If this happens, notify the jbos@scientrace.org about this..."+this.ToString()+anotherPlane.ToString());
		// l1 lies in "this" plane, and is orthogonal to the intersectionline-direction
		UnitVector l1 = tl1.tryToUnitVector();

		// where l1, starting at this.loc, meets anotherPlane we found a location on the intersectionline.
		Scientrace.Location retLoc = anotherPlane.lineThroughPlane(new Line(this.loc, l1));
		if (retLoc == null) {
			Console.WriteLine("WARNING: retLoc == null; l1: "+l1.ToString()+" ; anotherPlane:"+anotherPlane.getNorm().ToString());
			return null;
			}
		return new Line(retLoc, intersection_direction);
		}
	
}
}
