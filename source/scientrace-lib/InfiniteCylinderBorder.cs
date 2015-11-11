// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {

public class InfiniteCylinderBorder : EnclosedVolume, IBorder3d {

	public double radius;
	/// <summary>
	/// If "enclosesInside" is true, the border is surrounding a volume. If it is false, a hole is excluded from a volume by the border.
	/// </summary>
	public bool enclosesInside = true;
	//inherited from EnclosedVolume: public Scientrace.Location loc;
	public UnitVector direction;
	/// <summary>
	/// Gridtrf is just another vectortransformation, but specially for a rotating grid.
	/// </summary>
	public Scientrace.VectorTransform trf;

	public InfiniteCylinderBorder(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops,
									Scientrace.Location loc, Scientrace.UnitVector direction, double radius) : base(parent, mprops) {
		this.direction = direction;
		this.radius = radius;
		this.loc = loc;
		}
	
	public override Intersection intersects(Trace trace) {
		Intersection retIS = new Intersection(trace, this.intersectionPoints(trace), this);
		retIS.leaving = this.contains(trace.traceline.startingpoint);
		return retIS;
		}
	
	public override string exportX3D(Object3dEnvironment env) {
		//throw new System.NotImplementedException("exportX3D not implemented for InfiniteCylinderBorder class");
		Console.WriteLine("WARNING! exportX3D method not implemented for InfiniteCylinderBorder class");
		return "<!-- the InfiniteCylinderBorder has no X3D code (at this time) -->";
		}
		
	/* interface IBorder3D implementation */
	/// <summary>
	/// Return true when the specified aLocation is contained by the Sphere. If the 
	/// </summary>
	/// <param name='aLocation'>
	/// If set to <c>true</c> a location.
	/// </param>
	public bool contains(Scientrace.Location aLocation) {
		VectorTransform trf = this.getTransform();
		Location trLoc = trf.transform(aLocation-this.loc);
		return ((trLoc.x*trLoc.x) + (trLoc.y*trLoc.y) <= 1) == this.enclosesInside;
		}
	/* end of interface IBorder3D implementation */

	
	public IntersectionPoint[] intersectionPoints(Trace trace) {		
		VectorTransform trf = this.getTransform();
	
		// the points (any V) on the center line of this cylinder is described by "V = s + x l" for any x.
		Vector s = trf.transform(trace.traceline.startingpoint-this.loc);
		Vector l = trf.transform(trace.traceline.direction);	
		double r = 1; // not (this.radius;) as the transformation rendered it 1.
		
		double ABCa = Math.Pow(l.x,2) + Math.Pow(l.y,2);
		double ABCb = 2*((s.x*l.x)+(s.y*l.y));
		double ABCc = Math.Pow(s.x,2) + Math.Pow(s.y,2) - r*r;
		
		QuadraticEquation qe = new QuadraticEquation(ABCa, ABCb, ABCc);
		
		Scientrace.IntersectionPoint[] ips = new Scientrace.IntersectionPoint[2]{null, null};
		if (!qe.hasAnswers) { return ips; }
		for (int iAns = 1; iAns <= qe.answerCount; iAns++) {
			double x = qe.getAnswer(iAns);
			Vector tLoc = s + (l * x);
			Vector tNormal = new Vector(tLoc.x, tLoc.y, 0);
			Location oLoc = (trf.transformback(tLoc)+this.loc).toLocation();
			UnitVector oNormal = null;
			try {
				oNormal = trf.transformback(tNormal).tryToUnitVector();
				} catch { new ZeroNonzeroVectorException("oNormal is a zerovector which cannot be normalised for o3d ["+this.tag+"] and trace "+trace.ToCompactString()); }
			ips[iAns-1] = Scientrace.IntersectionPoint.locAtSurfaceNormal(oLoc, oNormal);
			}
/*		switch (qe.answerCount) {
			case 2:
				Scientrace.Location minLoc = ((l*qe.minVal) + o).toLocation();
				Scientrace.NonzeroVector minNorm = (minLoc - c).tryToNonzeroVector();
				ips[0] = Scientrace.IntersectionPoint.locAtSurfaceNormal(minLoc, minNorm);
				Scientrace.Location plusLoc = ((l*qe.plusVal) + o).toLocation();
				Scientrace.NonzeroVector plusNorm = (plusLoc - c).tryToNonzeroVector();
				ips[1] = Scientrace.IntersectionPoint.locAtSurfaceNormal(plusLoc, plusNorm);
				return new Intersection(aTrace, ips, this);
				//goto case 1;	//continue to case 1
			case 1:
				Scientrace.Location loc = ((l*qe.plusVal) + o).toLocation();
				Scientrace.NonzeroVector norm = (loc - c).tryToNonzeroVector();
				ips[0] = Scientrace.IntersectionPoint.locAtSurfaceNormal(loc, norm);
				ips[1] = null;
				return new Intersection(aTrace, ips, this);
			default:
				throw new IndexOutOfRangeException("eq.answerCount is not allowed to be "+qe.answerCount.ToString()+"in Shpere.intersects(..)");
			} //end switch(qe.answerCount)
		*/
		
		return ips;
		}
	
	public virtual VectorTransform getTransform() {
		//construction on request
		if (this.trf == null) {
			this.trf = this.createNewTransform();
		}
		return this.trf;
		}
		
	public Scientrace.VectorTransform createNewTransform() {
		Scientrace.Vector refvec;
		if (this.direction.dotProduct(new Vector(1,0,0))<0.8) {
			refvec = new Vector(1,0,0);
		} else {
			refvec = new Scientrace.Vector(0,1,0);
		}
		Scientrace.NonzeroVector u, v;
		//there used to be a *5 after lines below. TODO: check if OK like this
		u = refvec.crossProduct(this.direction).tryToUnitVector()*this.radius;
		v = u.crossProduct(this.direction).tryToUnitVector()*this.radius;
		return new Scientrace.VectorTransform(u,v,this.direction);
		}
		
		
/*		These methods were defined to inherit from AbstractBorder, but the IBorder3d interface doesn't require this.

	public Scientrace.VectorTransform getGridTransform(UnitVector nz) {
		//construction on request
		if (this.gridtrf == null) {
			Scientrace.Vector refvec;
			//vector has to differ at least 1% from the orthogonal orientation.
			if (Math.Abs(this.direction.dotProduct(nz))<0.9) { //previously: 0.99
				refvec = nz;
				} else {
				refvec = (nz.rotateAboutX(Math.PI/2).rotateAboutY(Math.PI/2)).tryToUnitVector();
				if (Math.Abs(this.direction.dotProduct(nz))<0.9) { //previously: 0.99
					throw new Exception("WARNING: CrossProducts cannot produce orthogonal lines over two parallel " +
					"vectors, two vectors that are almost parallel will give a large significance error. FIX DIDN'T WORK, please contact the Scientrace developer.");
					}
				}
			Scientrace.NonzeroVector u, v;
			u = refvec.crossProduct(this.direction).tryToUnitVector()*this.radius;
			v = u.crossProduct(this.direction).tryToUnitVector()*this.radius;
			this.gridtrf = new Scientrace.VectorTransform(u,v,this.direction);
		}
		return this.gridtrf;
		}

	public override bool contains(Location loc) {
		throw new System.NotImplementedException();
		}

	public override Scientrace.Location getCenterLoc() {
		return this.loc;
		}

	public override Scientrace.Location getOrthoStartCenterLoc() {
		return this.loc;
		}
	public override UnitVector getOrthoDirection (){
		return this.direction;
		}
	public override double getRadius() {
		return this.radius;
		}
	*/
	
	
	}} // end class/namespace

