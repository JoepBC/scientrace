// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {
public partial class PlaneBorderEnclosedVolume:EnclosedVolume {

	public List<PlaneBorder> borders; // = new List<PlaneBorder>();

	public PlaneBorderEnclosedVolume(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops, List<PlaneBorder> enclosing_borders) : base(parent, mprops) {
		List<PlaneBorder> effective_borders = new List<PlaneBorder>();
		for (int index1 =0; index1 < enclosing_borders.Count; index1++) {
			bool this_one_is_OK = true;
			for (int index2=index1+1; index2 < enclosing_borders.Count; index2++) {
				this_one_is_OK = this_one_is_OK && (!enclosing_borders[index1].overlapsWithPlane(enclosing_borders[index2]));
				} //end index2 loop
			if (this_one_is_OK) {
				effective_borders.Add(enclosing_borders[index1]);
				} else {
				//Console.WriteLine("WARNING: DISPOSED DUPLICATE BORDER:"+enclosing_borders[index1]);
				} //end this_one_is_OK
			} //end index1 loop
		if (effective_borders.Count != enclosing_borders.Count)
			Console.WriteLine(effective_borders.Count.ToString()+" out of "+enclosing_borders.Count+" non-duplicate borders preserved ("+(enclosing_borders.Count-effective_borders.Count)+" duplicates disposed)");
		this.borders = effective_borders;
		}

	public List<ObjectLinePiece> slice(List<ObjectLinePiece> lps, PlaneBorder aPlaneBorder) {
		List<ObjectLinePiece> retSlices = new List<ObjectLinePiece>();
		foreach (ObjectLinePiece anObjectLinePiece in lps) {
			Line sectLine = anObjectLinePiece.lp.toLine();
			//Console.WriteLine("FOO"+aPlaneBorder.getNormal().dotProduct(sectLine.direction));
			if (Math.Abs(aPlaneBorder.getNormal().dotProduct(sectLine.direction))<MainClass.SIGNIFICANTLY_SMALL) {
				//Console.WriteLine("BAR");
				retSlices.Add(anObjectLinePiece);
				continue; // line is within the plane, does not intersect. Continue for iLimitBorder loop
				}
			Scientrace.Location node = aPlaneBorder.lineThroughPlane(sectLine);
			if (node == null) {
				Console.WriteLine("WARNING, NODE=null: "+aPlaneBorder.ToString()+" vs. "+sectLine.ToString());
				continue; // somehow the line didn't go through the plane in the end... strange..
				}
			// fill return-list
			retSlices.AddRange(this.toOLP(anObjectLinePiece.lp.sliceIfBetween(node), "0 0.2 0.5 1", anObjectLinePiece.o3d));	
			}
		return retSlices;
		}

/*			if (this.borders[iBorder].getNormal().dotProduct(sectLine.direction)==0)
				continue; // line is within the plane, does not intersect. Continue for iLimitBorder loop
			Scientrace.Location node = this.borders[iBorder].lineThroughPlane(sectLine);
			double nodeDistance = sectLine.startingpoint.distanceTo(node);
			// slice location found before start of the line
			if (nodeDistance < 0)
				continue;
			// slice location found after end of the line
			if (nodeDistance > lpLen)
				continue;
			if (node == null) {
				Console.WriteLine("WARNING, NODE=null: "+this.borders[iBorder].ToString()+" vs. "+sectLine.ToString());
				continue; // somehow the line didn't go through the plane in the end...
				}*/

	public List<ObjectLinePiece> sliceObjectLinePiece(ObjectLinePiece anObjectLinePiece) {
		List<ObjectLinePiece> slices = new List<ObjectLinePiece>();
		slices.Add(anObjectLinePiece); //start with single OLP in list, make the list grow/be rewritten in iteration below
		// Iterate through all borders
		foreach (PlaneBorder aBorder in this.borders) {
			slices = this.slice(slices, aBorder);
			}
		return slices;
		}


	public bool contains(Scientrace.Location aLocation, PlaneBorder excludeBorder) {
		return this.contains(aLocation, excludeBorder, MainClass.SIGNIFICANTLY_SMALL);
		}

	public bool contains(Scientrace.Location aLocation, PlaneBorder excludeBorder, double excludedMargin) {
		foreach (PlaneBorder aBorder in this.borders) { // iterate all borders
			if (aBorder == excludeBorder) {
				continue; // if a location is created AT a border, it's safer to exclude that border from checking due to floating point errors
				}
			if (!aBorder.containsExclude(aLocation, excludedMargin)) {
				return false; // location not within border? FALSE
				}
			} // end foreach borders
		return true; // all good? TRUE
		}
		
	public bool contains(Scientrace.Location aLocation) {
		return this.contains(aLocation, null);
		}

	public override Intersection intersects(Trace trace) {
		List<IntersectionPoint> ips = new List<IntersectionPoint>();
		foreach (PlaneBorder aBorder in this.borders) {
			if (trace.traceline.direction.dotProduct(aBorder.getNormal()) == 0)
				continue; //trace is parallel to plane, will not intersect.
			Scientrace.Location intersectLoc = aBorder.intersectsAt(trace.traceline);
			if (intersectLoc != null) {
				if (this.contains(intersectLoc, aBorder)) {
					ips.Add(new Scientrace.IntersectionPoint(intersectLoc, aBorder.planeToFlatShape2d()));
					}
				}
			} // end foreach borders
		return new Intersection(trace, ips.ToArray(), this);
		}

	public static PlaneBorderEnclosedVolume createToppedPyramid(
						Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops,
						List<Scientrace.Location> front_corners, Scientrace.Location pyramid_top_location, PlaneBorder topping_plane) {
		/*// DEBUG INFO:
		Console.WriteLine("DEBUG: creating topped pyramid with front_corners: ");
		foreach (Scientrace.Location aLoc in front_corners) {
			Console.Write("Location: "+aLoc.trico());
			}
		Console.WriteLine("Top location: "+pyramid_top_location.trico());
		Console.WriteLine("topping_plane: "+topping_plane);
		// END DEBUG INFO */
		if (front_corners.Count<3) {
			throw new IndexOutOfRangeException("(topped) pyramid must have -at least- 3 front_corners in list (="+front_corners.Count+").");
			}
		List<Scientrace.PlaneBorder> toppedPyramidBorders = new List<Scientrace.PlaneBorder>();

		toppedPyramidBorders.Add( //adding large (opening) plane
			PlaneBorder.createBetween3LocationsPointingTo(front_corners[0], front_corners[1], front_corners[2], pyramid_top_location)
			);
			
		int fcc = front_corners.Count;
		for (int iBorder = 0; iBorder < fcc; iBorder++) { //adding side planes
			Console.WriteLine("PLANE:"+
			front_corners[iBorder].trico()+ front_corners[(iBorder+1)%fcc].trico()+ pyramid_top_location.trico()+" towards: "+front_corners[(iBorder+2)%fcc]);
			toppedPyramidBorders.Add(
			PlaneBorder.createBetween3LocationsPointingTo(
				front_corners[iBorder], front_corners[(iBorder+1)%fcc], pyramid_top_location, front_corners[(iBorder+2)%fcc])
				);
			}
		
		toppedPyramidBorders.Add(topping_plane); //adding small (closing) plane
		
		return new PlaneBorderEnclosedVolume(parent, mprops,
					toppedPyramidBorders);
		} // end createToppedPyramid Factory Method


	}}

