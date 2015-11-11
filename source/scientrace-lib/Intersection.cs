// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections.Generic;

namespace Scientrace {


public class Intersection {

	public bool intersects;
	public Scientrace.IntersectionPoint enter = null;
	public Scientrace.IntersectionPoint exit = null;
	public Scientrace.Object3d object3d;
	public bool leaving = false;


	public Intersection(bool intersects, Scientrace.Object3d object3d) {
		this.intersects = intersects;
		this.object3d = object3d;
		
		this.checkObject();//<- DEBUGGING:
	}


	public Intersection(bool intersects, Scientrace.Object3d object3d, Scientrace.Location enterloc, 
						Scientrace.FlatShape2d enterplane, Scientrace.Location exitloc, bool leaving) {
		this.intersects = intersects;
		this.object3d = object3d;
		this.enter = new IntersectionPoint(enterloc, enterplane);
		//plane of exitloc irrelevant
		if (exitloc == null) {
			this.exit = null; 
			} else {
			this.exit = new IntersectionPoint(exitloc, null);
			}
		this.leaving = leaving;
		this.checkObject();//<- DEBUGGING:
	}
		
	public Intersection(bool intersects, Scientrace.Object3d object3d, Scientrace.Location enterloc, 
						Scientrace.FlatShape2d enterplane, Scientrace.Location exitloc) {
		//same as constructor above but with leaving = false.
		this.intersects = intersects;
		this.object3d = object3d;
		this.enter = new IntersectionPoint(enterloc, enterplane);
		if (exitloc == null) {
			this.exit = null; 
			} else {
			this.exit = new IntersectionPoint(exitloc, null);
			}
		this.leaving = false;
		this.checkObject();//<- DEBUGGING:
	}

	public Intersection(Scientrace.Trace trace, Scientrace.IntersectionPoint[] ips, Scientrace.Object3d object3d) {
		this.initNewIntersection(trace.traceline, ips, object3d, false);
		this.checkObject();//<- DEBUGGING:
		}
			
	public Intersection(Scientrace.Line traceline, Scientrace.IntersectionPoint[] ips, Scientrace.Object3d object3d) {
		this.initNewIntersection(traceline, ips, object3d, false);
		this.checkObject();//<- DEBUGGING:
		}

	public Intersection(Scientrace.Line traceline, Scientrace.IntersectionPoint[] ips, Scientrace.Object3d object3d, bool bothdirections) {
		this.initNewIntersection(traceline, ips, object3d, bothdirections);
		this.checkObject();//<- DEBUGGING:
		}
		
	public Intersection mergeToNewIntersectionWithinBorder(Intersection anotherIntersection, IBorder3d aBorder, Trace aTrace, Object3d intersectedObject3d) {
		//List<IntersectionPoint> allips = new List<IntersectionPoint>(new IntersectionPoint[]{this.enter, this.exit, anotherIntersection.enter, anotherIntersection.exit} );
		List<IntersectionPoint> allips = new List<IntersectionPoint>();
		allips.Add(this.enter);
		allips.Add(this.exit);
		allips.Add(anotherIntersection.enter);
		allips.Add(anotherIntersection.exit);
		List<IntersectionPoint> validips = new List<IntersectionPoint>();
	
		foreach (IntersectionPoint ip in allips)
			if ((ip != null) && (aBorder.contains(ip.loc)))
				validips.Add(ip);
	
		return new Intersection(aTrace, validips.ToArray(), intersectedObject3d);		
		}
		
	// Factory method for false intersections...
	public static Scientrace.Intersection notIntersect(Scientrace.Object3d aNotIntersectedObject) {
		return new Scientrace.Intersection(false, aNotIntersectedObject);
		}
		
		
	public Scientrace.SurfaceProperties getSurfaceProperties() { 
		return this.enter.flatshape.surfaceproperties;
		}
		
	public void initNewIntersection(Scientrace.Line traceline, Scientrace.IntersectionPoint[] ips, Scientrace.Object3d object3d, bool bothdirections) {
		this.object3d = object3d;
		//Console.WriteLine("New Intersection with "+ips.Length+" IP's - "+object3d.hasParent());
		this.intersects = false;
			//Console.WriteLine("FALSE");		
		foreach (Scientrace.IntersectionPoint ip in ips) {
			if (ip == null) {
				//Console.WriteLine("<NULL>");			
				continue;
			}
			//if bothdirections == true the Intersection does not nessicarily have to be away (in direction "direction") 
			// from the startingpoint.
			if (!bothdirections) {
				//check if ip is in the line of the trace from its startingpoint
				if (traceline.direction.dotProduct(ip.loc-traceline.startingpoint) <= 1E-9) {
					//Console.WriteLine("WARNING: Intersectionpoint "+ip.loc.ToString()+" lies in the past @ "+traceline.startingpoint);
					continue;
					}
				}
			if (this.intersects == false) {
				//Console.WriteLine("TRUE");
				this.intersects = true;
				this.enter = ip;
				this.exit = null; // new IntersectionPoint(null, null); CHANGED (BUGFIX?) BY JBC @ 20131205
				continue;
				}
			if (this.enter.loc.distanceTo(traceline.startingpoint) > ip.loc.distanceTo(traceline.startingpoint)) {
				//this intersectionpoint is closer than the "enterpoint so far"
				// this condition is wrong right? if (this.exit==null) {
					this.exit = this.enter;
				// this is wrong right? 	}
				this.enter = ip;
				continue;
				} else {
				if (this.exit == null) {
					this.exit = ip;
					continue;
					} else {
					//this intersectionpoint is further from beam than enterpoint. New exitpoint defined.
					if (this.exit.loc.distanceTo(traceline.startingpoint) > ip.loc.distanceTo(traceline.startingpoint)) {
						//intersectionpoint is closer than old exitpoint
						this.exit = ip;
						continue;
						}}
				continue; // <-- existing exitpoint is closer than new IP: do nothing.
				}
			}
		}

	/// <summary>
	/// Surface errors can be modelled using linear modifiers. They change the surface
	/// normal accordingly. The "linear" means that it doesn't matter in what order
	/// the errors are applied.
	/// </summary>
	public void applyLinearModifiers() {
		Scientrace.Object3d o3d = this.object3d;
		Scientrace.SurfaceProperties surfaceprops = this.getSurfaceProperties();
		// fill the list with modifiers. A surface may have specific (dominant) properties set, but not necessarily. 
		// If not, use the properties for the interacting object.
		List<Scientrace.UniformTraceModifier> modifiers = 
			((surfaceprops == null) ?
			o3d.surface_normal_modifiers
			: surfaceprops.surface_normal_modifiers);
		//foreach (Scientrace.UniformTraceModifier utm in o3d.surface_normal_modifiers) {
		foreach (Scientrace.UniformTraceModifier utm in modifiers) {
			/*TODO: due to the history of the Plane class, it's vectors length contain crucial 
			 * information considering the sizes of Parallelograms etc. 
			 * Altering the plane of a flatshape, which is also a property of e.g. a parallelogram
			 * may malform this parallelograms properties. A copy is therefore required. */
			this.enter.flatshape = this.enter.flatshape.copy();
			this.enter.flatshape.plane = utm.linearModify(this.enter.flatshape.plane);
			
			}
		}


	/// <summary>
	/// Reduces the "object3d" element from the intersection to its parent when leaving the current object.
	/// </summary>
	/// <param name='anIntersection'>
	/// The intersection that could be leaving the object.
	/// </param>
	public void resetObjectToParentWhenLeaving() {
		if (this.leaving) {	//Leaving current object, returning to parent...
			if (this.object3d.hasParent()) {
				this.object3d = this.object3d.getFirstNonVirtualParent();
				} else { //before the "virtual" collection improvement default behaviour.
				this.object3d = this.object3d.getParentOrRoot();
				}
			}
		}

	public void removeExit() {
		this.exit = new IntersectionPoint(null,null);
		this.leaving = true;
		}

	public void checkObject() {
		if (this.intersects) {
				if (this.object3d==null) {
					throw new NullReferenceException();
				}
			}
		}

	public string enterexitspecs() {
		return "enter: "+(this.enter==null?"null":this.enter.loc.tricoshort())+" / exit: "+(this.exit==null?"null":this.exit.loc.tricoshort())+
			(this.leaving?" / leaving ":" / entering ");
		}

	public override string ToString() {
		if (this.intersects) {
			if (this.exit == null) {
				return "en: "+this.enter.loc.trico()+", ex: end of the world in a:"+this.object3d.GetType();
			} else {
				return "en: "+this.enter.loc.trico()+", ex: "+this.exit.loc.trico()+" in a:"+this.object3d.GetType();
			}
		} else {
			return "This object ("+this.GetType()+") does not intersect";
		}
		
	}

	
	
}
}
