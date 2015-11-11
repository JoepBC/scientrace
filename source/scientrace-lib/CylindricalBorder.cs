// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class CylindricalBorder : AbstractGridBorder {

	public double radius;
	public Scientrace.Location loc;
	public Scientrace.NonzeroVector directionlength;
	/// <summary>
	/// Gridtrf is just another vectortransformation, but specially for a rotating grid.
	/// </summary>
	public Scientrace.VectorTransform gridtrf;

	public CylindricalBorder(Scientrace.Location loc, Scientrace.NonzeroVector directionlength, double radius) {
		this.radius = radius;
		this.loc = loc;
		this.directionlength = directionlength;
	}

	public override UnitVector getOrthoDirection (){
		return this.directionlength.toUnitVector();
		}


	public override double getRadius() {
		return this.radius;
		}
		
	public override Scientrace.Location getOrthoStartCenterLoc() {
		return this.loc;
		}
		
	public override Scientrace.Location getCenterLoc() {
		return this.loc+(this.directionlength*0.5);
		}

	public override Scientrace.VectorTransform getGridTransform(UnitVector nz) {
		//construction on request
		if (this.gridtrf == null) {
			Scientrace.Vector refvec;
			//vector has to differ at least 1% from the orthogonal orientation.
			if (Math.Abs(directionlength.toUnitVector().dotProduct(nz))<0.99) {
				refvec = nz;
				} else {
				refvec = (nz.rotateAboutX(Math.PI/2).rotateAboutY(Math.PI/2)).tryToUnitVector();
				if (Math.Abs(directionlength.toUnitVector().dotProduct(nz))<0.99) {
					throw new Exception("WARNING: CrossProducts cannot produce orthogonal lines over two parallel " +
					"vectors, two vectors that are almost parallel will give a large significance error. FIX DIDN'T WORK, please contact the Scientrace developer.");
					}
				}
			Scientrace.NonzeroVector u, v;
			u = refvec.crossProduct(directionlength).tryToUnitVector()*this.radius;
			v = u.crossProduct(directionlength).tryToUnitVector()*this.radius;
			this.gridtrf = new Scientrace.VectorTransform(u,v,this.directionlength);
		}
		return this.gridtrf;
	}

	public override Scientrace.VectorTransform createNewTransform() {
		Scientrace.Vector refvec;
		if (directionlength.toUnitVector().dotProduct(new Vector(1,0,0))<0.8) {
			refvec = new Vector(1,0,0);
		} else {
			refvec = new Scientrace.Vector(0,1,0);
		}
		Scientrace.NonzeroVector u, v;
		u = refvec.crossProduct(directionlength).tryToUnitVector()*this.radius;
		v = u.crossProduct(directionlength).tryToUnitVector()*this.radius;
		return new Scientrace.VectorTransform(u,v,this.directionlength);
	}


	public override bool contains(Location aLocation) {
		Scientrace.VectorTransform trf = this.getTransform();
		Scientrace.Location tloc = trf.transform(aLocation-this.loc);
		double ufac, vfac;
		// ufac is the fraction of "a vector 1,0,0 transformedback.length" of the radius of the cylinder
		ufac = Vector.x1trbl(trf)/this.radius;
		vfac = Vector.y1trbl(trf)/this.radius;
		//checking for exceptions
		if ((tloc.z < 0) || (tloc.z > 1)) {
			return false; //location lies below or above cylinder != within
			}
		if (Math.Pow(tloc.x*ufac,2)+Math.Pow(tloc.y*vfac,2) > 1) {
			return false; //location lies outside the cylinder
			}
		return true;
		}

	public IntersectionPoint[] intersects11circleborder(Trace trace) {
		Scientrace.Line line = trace.traceline;
		double d1, d2, dx, dy, lx, ly;
		double ABCa, ABCb, ABCc, discr;
		Scientrace.Vector v1, v2;
		Scientrace.FlatShape2d plane1, plane2;
		Scientrace.IntersectionPoint[] ip;
		dx = line.direction.x;
		dy = line.direction.y;
		lx = line.startingpoint.x;
		ly = line.startingpoint.y;

		ABCa = Math.Pow(ly,2)+Math.Pow(lx,2);
		ABCb = 2*((ly*dy) + (lx*dx));
		ABCc = Math.Pow(dy,2)+Math.Pow(dx,2)-1;
		discr = Math.Pow(ABCb, 2)-(4*ABCa*ABCc);
		if (discr < 0) {
			ip = new Scientrace.IntersectionPoint[0];
			//ip[0] = null;
			//ip[1] = null;
			return ip;
		}
		if (discr == 0) {
			ip = new Scientrace.IntersectionPoint[1];
			d1 = ((-ABCb)+Math.Sqrt(Math.Pow(ABCb, 2)-(4*ABCa*ABCc)))/(2*ABCa);
			v1 = (line.direction.toVector()*d1)+line.startingpoint.toVector();
				//plane1 is based on v1 rotated 90 degrees which makes: x' = -y, y' = x, z = z
			plane1 = new Scientrace.FlatShape2d(v1.toLocation(), new Scientrace.UnitVector(0,0,1),
				                                               new Scientrace.UnitVector(-v1.y, v1.x, v1.z));
			ip[0] = new IntersectionPoint(v1.toLocation(), plane1);
			return ip;
		}
		d1 = ((-ABCb)+Math.Sqrt(Math.Pow(ABCb, 2)-(4*ABCa*ABCc)))/(2*ABCa);
		d2 = ((-ABCb)-Math.Sqrt(Math.Pow(ABCb, 2)-(4*ABCa*ABCc)))/(2*ABCa);
		v1 = (line.direction.toVector()*d1)+line.startingpoint.toVector();
		v2 = (line.direction.toVector()*d2)+line.startingpoint.toVector();
		ip = new Scientrace.IntersectionPoint[2];
		plane1 = new Scientrace.FlatShape2d(v1.toLocation(), new Scientrace.UnitVector(0,0,1),
				                                               new Scientrace.UnitVector(-v1.y, v1.x, v1.z));
		ip[0] = new IntersectionPoint(v1.toLocation(), plane1);
		plane2 = new Scientrace.FlatShape2d(v2.toLocation(), new Scientrace.UnitVector(0,0,1),
				                                               new Scientrace.UnitVector(-v2.y, v2.x, v2.z));
		ip[1] = new IntersectionPoint(v2.toLocation(), plane2);
		return ip;

	}
		
		public override string ToString () {
			return "cb @ "+this.loc+", r="+this.radius+", dir="+this.directionlength;
		}

	
}
}
