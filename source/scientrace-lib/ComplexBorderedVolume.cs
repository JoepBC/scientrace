// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {

public partial class ComplexBorderedVolume : EnclosedVolume {

	public List<PlaneBorderEnclosedVolume> subVolumes;
	
	public Random rnd = new Random();

	public ComplexBorderedVolume(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops, List<List<PlaneBorder>> enclosing_borders) : base(parent, mprops) {
		/*foreach (PlaneBorder aBorder in enclosing_borders) {
			Console.WriteLine("A border added to PlaneBorderEnclosedVolume: "+aBorder.getNormal().trico()+" @ "+aBorder.getCenterLoc());
			}*/
		this.subVolumes = new List<PlaneBorderEnclosedVolume>();
		foreach (List<PlaneBorder> pblist in enclosing_borders) {
			PlaneBorderEnclosedVolume pbvol = new PlaneBorderEnclosedVolume(null, mprops, pblist);
			this.subVolumes.Add(pbvol);
			//this.borders = enclosing_borders;
			}
		//Console.WriteLine("Number of subvolumes for {"+this.tag+"}: "+this.subVolumes.Count);
		}

	public bool contains(Scientrace.Location aLocation, double excludedMargin) {
		bool retbool = false;
		foreach (PlaneBorderEnclosedVolume aSubVolume in subVolumes) {
			retbool = retbool || aSubVolume.contains(aLocation, null, excludedMargin);
			}
		return retbool;
		}
		
				
	public bool contains(Scientrace.Location aLocation) {
		bool retbool = false;
		foreach (PlaneBorderEnclosedVolume aSubVolume in subVolumes) {
			retbool = retbool || aSubVolume.contains(aLocation);
			}
		return retbool;
		}		
		
	// Add IntersectionPoints for entering and leaving to the SortedList when they exist and the distance from the startingpoint is not yet set
	public void conditionalIPList(SortedList<double,IntersectionPoint> ips, Intersection anIntersection, Location startingPoint) {
		if (anIntersection.enter != null) {
			if (!ips.Keys.Contains(anIntersection.enter.loc.distanceTo(startingPoint)))
				ips.Add(anIntersection.enter.loc.distanceTo(startingPoint), anIntersection.enter);
			/*else
				Console.WriteLine("WARNING: location "+anIntersection.enter.ToString()+" is entered (at least) twice at "+this.tag);*/
			}
		if (anIntersection.exit != null) {
			if (!ips.Keys.Contains(anIntersection.exit.loc.distanceTo(startingPoint)))
				ips.Add(anIntersection.exit.loc.distanceTo(startingPoint), anIntersection.exit);
			/*else
				Console.WriteLine("WARNING: location "+anIntersection.enter.ToString()+" is entered (at least) twice at "+this.tag);*/
			}
		}
		
		
	public override Intersection intersects(Trace aTrace) {
		/* OK, this is how we are going to do this:
		 * First (1), we find all intersectionpoints for all sub-borders and order them
		 * as a function of the distance from the startingpoint of the trace. Then (2), we
		 * take the average location in between two consecutive intersectionpoints. Let's
		 * ask all SubVolumes if anybody has this average value in it's body (3). We will
		 * repeat this procedure until we find an average value that is *NOT* included (4). The
		 * very first intersectionpoint, and the last intersectionpoint before that latest
		 * assessed average will be the two intersectionpoints construction the Intersection (5)
		 * value to return.
		 * A final remark must be made: when a trace is leaving from the current object and the
		 * average location between the start of the trace and the first intersection is also
		 * within the object, the first intersectionpoint is actually virtual and should be 
		 * removed. (6) */
		
		// (1)
		// create an orderedlist with as keys the distances from the last intersection to this ip
		SortedList<double,IntersectionPoint> all_ips = new SortedList<double,IntersectionPoint>();
		Scientrace.Location subTraceStart = aTrace.traceline.startingpoint;
		foreach (PlaneBorderEnclosedVolume aSubVolume in subVolumes) {
			Intersection tIntersection = aSubVolume.intersects(aTrace);
			if (tIntersection.intersects) {
				this.conditionalIPList(all_ips, tIntersection, subTraceStart);
				}
			}
		IntersectionPoint previous_ip = null;
		IntersectionPoint first_ip = null;
		IntersectionPoint last_ip = null;
		
		for (int iKey = 0; iKey < all_ips.Keys.Count; iKey++) {
			double currentKey = all_ips.Keys[iKey];
			IntersectionPoint currentIP = all_ips[currentKey];
			
			if (previous_ip == null) {
				// is this the last IP in the list?
				if (iKey != all_ips.Count-1) {
					// If both the area between the start of the trace and that between this and the next trace is contained, skip to the next (by continue)
					IntersectionPoint nextIP = all_ips[all_ips.Keys[iKey+1]];
					if (this.contains(subTraceStart.avgWith(currentIP.loc)) 
					  && this.contains(currentIP.loc.avgWith(nextIP.loc))) {
					  	continue;
						}
					}			
				// (6)
				if (this.contains(subTraceStart.avgWith(currentIP.loc)) && (iKey+1!=all_ips.Keys.Count)) {
					first_ip = currentIP;
					break; } else {
					// the first ip in the list, no average to take with previous value
					first_ip = currentIP;
					}
				} else {
				// (2)
				Scientrace.Location average_loc = currentIP.loc.avgWith(previous_ip.loc);
				// (3)
				if (!this.contains(average_loc)) {
					//Console.WriteLine("BREAKOUT!!!!");
					last_ip = previous_ip;
					// (4) Break out of foreach loop, we've found the last point in this intersection (the previous one in the list)
					break;
					}
				}
			// last statement in foreach loop, defining the current IP als the previous for the next cycle.
			previous_ip = currentIP;
			}

		// there has to be at least ONE intersection to succeed...
		if (first_ip == null) {
			return Intersection.notIntersect(this);
			}
		/* below must be the most obscure line in this routine, sry about that.
		 * it does this: if last_ip is still unassigned, assign previous_ip as long as it differs from first_ip... */
		// last_ip = last_ip ?? (previous_ip != first_ip ? previous_ip : null);
		 if (last_ip == null)
			last_ip = (previous_ip != first_ip ? previous_ip : null);
		// (5)
		Scientrace.IntersectionPoint[] real_ips = new Scientrace.IntersectionPoint[2];
		real_ips[0] = first_ip;
		real_ips[1] = last_ip;
		Intersection retIntersect = new Intersection(aTrace, real_ips, this);
		return retIntersect;
		//end func. intersect	
		}
	}
}

