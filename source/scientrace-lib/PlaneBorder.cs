// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public class PlaneBorder : AbstractGridBorder, IInterSectableBorder3d { // AbstractGridBorder {

	public Scientrace.Location loc;
	Scientrace.NonzeroVector allowedDirNormal;
	Scientrace.NonzeroVector planeBaseVec1, planeBaseVec2;
	/// <summary>
	/// The radius is added for drawing purposes, it is perhaps not the best name for the value. It is used e.g. to define the grid width. Open for discussion.
	/// </summary>
	public double radius;
	private Scientrace.Plane tPlane;

	public PlaneBorder(Scientrace.Location loc, Scientrace.NonzeroVector allowedDirectionSurfaceNormal) {
		this.loc = loc;
		this.allowedDirNormal = allowedDirectionSurfaceNormal;
		this.radius = allowedDirectionSurfaceNormal.length;
		this.allowedDirNormal.fillOrtogonalVectors(ref this.planeBaseVec1, ref this.planeBaseVec2);
		this.trf = this.createNewTransform();
		this.tPlane = new Scientrace.Plane(this.loc, this.planeBaseVec1, this.planeBaseVec2);
		}

	public Scientrace.Line getIntersectionLineWith(Scientrace.PlaneBorder anotherPlane) {
		return this.tPlane.getIntersectionLineWith(anotherPlane.tPlane);
		}

	public Scientrace.IntersectionPoint filterOutsideBorder(Scientrace.IntersectionPoint anIP) {
		if (anIP == null) { return null; }
		return 
			this.contains(anIP.loc)
			? anIP
			: null;
		}
		
				
	public bool isParallelTo(PlaneBorder anotherPlaneBorder) {
		Scientrace.UnitVector n1, n2;
		n1 = this.allowedDirNormal.toUnitVector();
		n2 = anotherPlaneBorder.allowedDirNormal.toUnitVector();
		//Console.WriteLine("DEBUG: Crossproduct "+n1.trico()+"/"+n2.trico()+": "+n1.crossProduct(n2).length);
		return (n1.crossProduct(n2).length == 0);		
		}
	

	public override VectorTransform createNewTransform() {
		return new VectorTransform(this.planeBaseVec1, this.planeBaseVec2, this.allowedDirNormal);
		}

	public bool containsExclude(Scientrace.Location aLocation, double excludedMargin) {			
		if (aLocation == null) { return false; }
		Scientrace.Location sLoc = (aLocation-this.loc); //shifted location based on plane origin
		Scientrace.Location tLoc = this.trf.transform(sLoc); //transformed location based on the plane basevectors and its normal
		return (tLoc.z >= excludedMargin);
		}
	
	public bool contains(Scientrace.Location aLocation, double includeMargin) {			
		return this.containsExclude(aLocation, -includeMargin);
		}
									
	public override bool contains(Scientrace.Location aLocation) {
		return this.contains(aLocation, 0);
		}
	
	/// <summary>
	/// Returns true if two Plane-instances cover the same area
	/// </summary>
	/// <returns><c>true</c> if planes overlap, <c>false</c> otherwise.</returns>
	/// <param name="anotherPlane">the other plane.</param>
	public bool overlapsWithPlane(Scientrace.PlaneBorder anotherPlane) {
		if ((this.allowedDirNormal.normalized().toVector() - anotherPlane.allowedDirNormal.normalized().toVector()).length > MainClass.SIGNIFICANTLY_SMALL)
			return false; //Normals differ significantly. Nuff said.

		// Normals are considered equal. Are the planes locations in space?
		Scientrace.Location loc1 = (this.lineThroughPlane(new Line(Location.ZeroLoc(), this.getNormal())));
		Scientrace.Location loc2 = (anotherPlane.lineThroughPlane(new Line(Location.ZeroLoc(), this.getNormal())));
		// if these locations are considered the same, the planes overlap.
		return ((loc1 - loc2).length < MainClass.SIGNIFICANTLY_SMALL);
		}

	public Scientrace.Location lineThroughPlane(Scientrace.Line aLine) {
		if (aLine.direction.dotProduct(this.allowedDirNormal)==0) {
			throw new Exception("Line is parallel to planeborder! Cannot calculate intersection!");
			}
		return this.tPlane.lineThroughPlane(aLine).copy();
		}
	
	public FlatShape2d planeToFlatShape2d() {
		return new FlatShape2d(this.tPlane);
		}
	
	public override Location getCenterLoc() {
		return this.loc;
		}


	public Intersection intersectsObject(Trace trace, Object3d o3dWithThisBorder) {
		PlaneBorder aBorder = this;
		if (trace.traceline.direction.dotProduct(aBorder.getNormal()) == 0)
			return Intersection.notIntersect(o3dWithThisBorder); //trace is parallel to plane, will not intersect.

		Scientrace.Location intersectLoc = aBorder.intersectsAt(trace.traceline);
		
		if (intersectLoc == null)
			return Intersection.notIntersect(o3dWithThisBorder); //trace is parallel to plane, will not intersect.
		
		return new Intersection(true, o3dWithThisBorder, intersectLoc, new Scientrace.FlatShape2d(this.tPlane), null, this.contains(trace.traceline.startingpoint));

		}
				
		
	public Scientrace.Location intersectsAt(Scientrace.Trace aTrace) {
		return this.intersectsAt(aTrace.traceline);
		}
		
	public Scientrace.Location intersectsAt(Scientrace.Line aLine) {
		if (aLine == null) {
			Console.WriteLine("WARNING: aLine @ PlaneBorder.intersects == null");
			return null;
			}
		return this.tPlane.lineThroughPlane(aLine);
		}
		
	public Scientrace.UnitVector getNormal() {
		return this.allowedDirNormal.toUnitVector();
		}	

	public override UnitVector getOrthoDirection() {
		return this.getNormal();
		}
	public override Location getOrthoStartCenterLoc() {
		return this.loc;
		}
	public override double getRadius() {
		return this.radius;
		}
		
		
	public static PlaneBorder createBetween3LocationsPointingTo(Scientrace.Location baseLoc, Scientrace.Location loc2, Scientrace.Location loc3, Scientrace.Location pointingToLoc) {
		Vector v1 = (loc2-baseLoc).tryToUnitVector();
		Vector v2 = (loc3-baseLoc).tryToUnitVector();
		NonzeroVector tryNormal = v1.crossProduct(v2).tryToNonzeroVector();
		//Console.WriteLine("V1: "+v1.trico()+" v2:"+v2.trico()+" trynormal:"+tryNormal.trico());
		PlaneBorder tryBorder = new PlaneBorder(baseLoc, tryNormal);
		if (tryBorder.contains(pointingToLoc))
			return tryBorder;
		tryBorder = new PlaneBorder(baseLoc, tryNormal.negative());
		if (tryBorder.contains(pointingToLoc))
			return tryBorder;
		throw new Exception("Couldn't create PlaneBorder containing "+pointingToLoc.trico());
		} //end createBetween3LocationsPoitningTo(...)
		
	public override string ToString() {
		return "[BORDER]\n"+
			"-Loc: "+this.loc.trico()+"\n"+
		//	"-V1: "+this.planeBaseVec1.trico()+"\n"+
		//	"-V1: "+this.planeBaseVec2.trico()+"\n"+
			"-Normal: "+this.getNormal().trico();
		} //end ToString()
	
	}
}

