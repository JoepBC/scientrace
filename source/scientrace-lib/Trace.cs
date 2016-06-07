// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections.Generic;


public static class MyExtensions {
    public static void AddIfNotNull<TValue>(this IList<TValue> list, TValue value) {
        if ((object)value != null)
            list.Add(value);
    }}


namespace Scientrace {

public partial class Trace {

	public Scientrace.Trace parent = null;
		
	public double wavelenght;
	/// <summary>
	/// If a ray reflects or passes a surface it is shifted distance "interactionoffset" in its shining direction.
	/// This prevents double mirrorring and refracting.
	/// </summary>
	public double interactionoffset;
	public LightSource lightsource;
	public Line traceline;

	public Scientrace.Location endloc;
	public double tracedistance = 0;
	//total distance so far until loc of traceline
	public double wavedistance = 0;
	//total distance so far until loc of traceline but measured as integral of refractive-index over distance
	public int nodecount = 0;
	//number of "forks" so far
	public Object3d currentObject;
	//the intensity of the ray as a fraction. 1 is full intensity. 0 none.
	public double intensity = 1;
	public double original_intensity = 1;
	public bool alive = true;
	

	public string traceid = "not set";
	public int forkedcount = 0;
		
	public Trace(double wlengthmeters, LightSource lsource, Scientrace.Line traceline, 
		             Scientrace.Object3d currentObject, double intensity, double original_intensity) {
		this.init(wlengthmeters, lsource, traceline, currentObject, intensity, original_intensity);
	}
		
/* 
	!!!!!!!!!!!!!!THIS METHOD IS DEPRECATED AS THE FORK METHOD SHOULD BE USED TO PREVENT INFINITE LOOPS!!!!!!!!!!!
	/// <summary>
	/// This constructor method clones the original trace parameters to a new trace instance but changes the traceline
	/// value to the newLine values.
	/// </summary>
	/// <param name="aTrace">
	/// A <see cref="Scientrace.Trace"/>
	/// </param>
	/// <param name="newLine">
	/// A <see cref="Scientrace.Line"/>
	/// </param>
	public Trace(Scientrace.Trace aTrace, Scientrace.Line newLine) {
		this.init(aTrace.wavelenght, aTrace.lightsource, newLine, aTrace.currentObject, aTrace.intensity, aTrace.original_intensity);
		}
*/
		
/*	public Trace(double wlength, LightSource lsource, Scientrace.Line line, Scientrace.Object3d currentObject) {
		//default start-intensity of 1
		this.init(wlength, lsource, line, currentObject, 1);
	}		 */

	public void init(double wlength, LightSource lsource, Scientrace.Line line, Scientrace.Object3d currentObject,
		                 double intensity, double original_intensity) {
		this.intensity = intensity;
		this.original_intensity = original_intensity;
		if (original_intensity == 0) {
			throw new ArgumentOutOfRangeException("original_intensity", "wl: "+wlength.ToString()+", currentobj:"+
					currentObject.tag);
			}
		this.wavelenght = wlength;
		this.interactionoffset = this.getMinDistinctionLength(); //CHANGED OFFSET FROM WL*100 TO WL TODO CHECK
		this.traceline = line;
		this.currentObject = currentObject;
		this.lightsource = lsource;
		if (lsource != null) {
			if (this.isEmpty()) {
				throw new Exception("An invisible beam (I="+this.intensity+") has been constructed");
			}}
		}


	public double getMinDistinctionLength() {
		return this.wavelenght/100;
		}

	public void setLightSource(Scientrace.LightSource light) {
		this.lightsource = light;
		}

	public List<Scientrace.Trace> redirect(Scientrace.Object3d toObject, Scientrace.Intersection intersection) {
		return (toObject.materialproperties.dielectric && Trace.support_polarisation) ? 
						this.redirect_with_polarisation(toObject, intersection) // the new implementation
						: this.redirect_without_polarisation(toObject, intersection); // the old implementation
				
		}

	public Scientrace.Intersection intersectplanesforobject(Scientrace.Object3d anObject, 
													List<Scientrace.FlatShape2d> pgrams) {
		Scientrace.Trace trace = this;
		Scientrace.Location firstloc = null;
		Scientrace.FlatShape2d enterplane = null;
		Scientrace.Location lastloc = null;
		double? firstdistance = null;
		double? lastdistance = null;
		
		Scientrace.Location tloc;
		double tdistance;
		foreach (Scientrace.FlatShape2d pgram in pgrams) {
			tloc = pgram.atPath(trace.traceline);
			if (tloc == null) {	continue; }
			if (((tloc-trace.traceline.startingpoint).dotProduct(trace.traceline.direction)) < trace.getMinDistinctionLength()) {
				continue; //the location (even if increased with one wavelength) is "behind" the trace
				}
			tdistance = tloc.distanceTo(trace.traceline.startingpoint);
			if (firstdistance == null) {
				//assign first and last to first item in collection
				firstdistance = tdistance;
				firstloc = tloc;
				enterplane = pgram;
				lastdistance = tdistance;
				lastloc = tloc;
				}
			if (tdistance < firstdistance) {
				//Console.WriteLine(tdistance.ToString()+"<"+firstdistance.ToString());
				firstdistance = tdistance;
				firstloc = tloc;
				enterplane = pgram;
				}
			if (tdistance > lastdistance) {
				//Console.WriteLine("We have a large distance gentlemen");
				lastdistance = tdistance;
				lastloc = tloc;
				}
			} //end of pgrams-for-loop
		if (firstdistance == null) {
			//when no side of the paralellogram has been intersected
			return new Scientrace.Intersection(false, anObject);}
		if (firstdistance == lastdistance) {
			lastdistance = null; lastloc = null; }
		if (lastdistance == null) {
			//Console.WriteLine("I have no ending");
		} else {
			//Console.WriteLine("I from "+trace.currentObject.GetType()+" @ "+trace.traceline.startingpoint.trico()+" pass through an object");
		}
		Scientrace.Intersection intrs = new Scientrace.Intersection(true, anObject, firstloc, enterplane, lastloc);
		intrs.leaving = (lastloc == null);
		return intrs;
	}		
	/* end of intersectplanes function */

	public Line reflectAt(Scientrace.Plane plane) {
		return this.reflectLine(this.traceline, plane);
		}

	/// <summary>
	/// After an inventory of the first next intersection has been made, the trace has to "cross" it.
	/// This could mean that the trace is continued/prolonged, but it could also reflect or refract
	/// which is processed inside the "redirect" method.
	/// </summary>
	/// <param name='anIntersection'>
	/// An intersection.
	/// </param>
	public List<Scientrace.Trace> processIntersection(Scientrace.Intersection anIntersection) {
		// basic check on validity
		if (this.isEmpty()) { Console.WriteLine("invisible trace found. Should have died earlier. Now terminating."); throw new Exception("invisible trace");	/*return;*/ }	

		/* OBSOLETE: Done by "trace::cycle()" method already.
		// if the intersection states that the object is left set the parent object as intersection 
		//intersection.resetToParentObjectWhenLeaving();
		*/
		//Console.WriteLine("INTERSECTION REPORT: \n"+anIntersection.ToString());
		// another basic check if object3d is set
		if (anIntersection.object3d == null) {throw new ArgumentNullException("intersection.object3d","AN ERROR OCCURRED. UNKNOWN OBJECT "+anIntersection.intersects.ToString());}

		List<Scientrace.Trace> newTraces;
		
		// there any interaction at the intersection?
		if (anIntersection.object3d.attachedTo(this.currentObject)) {// do not interact between attached objects

			newTraces = this.prolong(anIntersection.object3d, anIntersection);
			//DO NOT ADD ANGLES TO JOURNAL FOR PROLOGING: // TraceJournal.Instance.addAngle(new Scientrace.Angle(this, newtrace, intersection));
			} else {
			// redirect on volume-change while intersecting
			//***newtrace = this.redirect(currentObj, intersection);
			newTraces = this.redirect(anIntersection.object3d, anIntersection);
			foreach (Scientrace.Trace newtrace in newTraces) {

				TraceJournal.Instance.recordAngle(new Scientrace.Angle(this, newtrace, anIntersection));
				}
			}
		//get rid of "this" old trace, traces in newTraces (if any) are the new traces
		this.perish(anIntersection.enter.loc);
		//let the new traces cycle instead
		foreach (Scientrace.Trace newtrace in newTraces) {
			newtrace.cycle();
			}
		return newTraces;
		}



	/// <summary>
	/// Prolong the trace to the specified Object3d instance (toObject3d) at location as specified in intersection.
	/// Prolonging does NOT increase the nodecounter. Attached objects count as 1 node alltogether.
	/// </summary>
	/// <param name='toObject3d'>
	/// The Object3d instance of interaction with the current Object3d at the surface.
	/// </param>
	/// <param name='intersection'>
	/// The intersection instance that describes the intersection that is calculated to occur.
	/// </param>
	public List<Scientrace.Trace> prolong(Scientrace.Object3d toObject3d, Scientrace.Intersection intersection) {
		Scientrace.Trace newtrace = this.clone();
		List<Scientrace.Trace> newTraces = new List<Scientrace.Trace>();
		newtrace.currentObject = toObject3d;
		newtrace.traceline.startingpoint = intersection.enter.loc;
		newtrace.traceline.direction = this.traceline.direction;
		newTraces.Add(newtrace);
		return newTraces;
		}
		
					

	public Line reflectLine(Scientrace.Line line, Scientrace.Plane plane) {
		Scientrace.UnitVector norm = plane.getNorm();
		/*ADDED and removed 20131030 make surface normal in direction of incoming beam
		if (line.direction.dotProduct(norm) > 0) {
			norm.negated();
			}*/
		return this.reflectLineAbout(line, norm);
		}

	public Line reflectLineAbout(Scientrace.Line line, Scientrace.NonzeroVector norm) {
		return line.reflectOnSurface(norm, this.interactionoffset);
		}


	public static Scientrace.Trace getParentRoot(Scientrace.Trace aTrace) {
		Scientrace.Trace tTrace = aTrace;
		while (tTrace.parent != null) {
			tTrace = tTrace.parent;
			}
		return tTrace;
		}
		
	public Scientrace.Trace getParentRoot() {
		return Scientrace.Trace.getParentRoot(this);
		}
	
	public Scientrace.Trace fork(Scientrace.Line newLine, Vector pol_vec_1, Vector pol_vec_2, double new_intensity, string fork_tag) {
		Scientrace.Trace retTrace = this.fork(fork_tag);
		retTrace.traceline = newLine;
		retTrace.intensity = new_intensity;
		retTrace.setCircularPolarisation(pol_vec_1, pol_vec_2);
		return retTrace;
		}

	/// <summary>
	/// For this trace, which means to clone it, to increase its parents fork-counter, modify the trace-id by adding "fork_tag" to it.
	/// </summary>
	/// <param name="fork_tag">A tag that is added to the trace-id of the created fork. The "#" char will be replaced by the this objects forkedcount value.</param>
	public Scientrace.Trace fork(string fork_tag) {
		Scientrace.Trace ttrace = this.clone();
		this.forkedcount++;
		//ttrace.traceid = ttrace.traceid+"_"+this.forkedcount.ToString();
		ttrace.traceid = ttrace.traceid+fork_tag.Replace("#",this.forkedcount.ToString());
		ttrace.nodecount++;
		return ttrace;
		}

	public Scientrace.Trace clone() {
		//use same functionality as with shift, but without shift ;)
		return this.shiftclone(new Location(0,0,0));
		}

	public Scientrace.Trace shiftclone(Scientrace.Location loc) {
		Scientrace.Trace rettrace = new Scientrace.Trace(this.wavelenght, this.lightsource, this.traceline+loc, this.currentObject, this.intensity, this.original_intensity);
		rettrace.nodecount = this.nodecount;
		rettrace.traceid = this.traceid;
		rettrace.parent = this;
		if (this.nodecount > this.lightsource.max_interactions) {
			Console.WriteLine("WARNING!: "+this.nodecount.ToString()+" has reached the maximum nodecount! Should die.");
			}
		return rettrace;
		}

	public override string ToString() {
		if (this.endloc == null) {
			return "Trace ("+this.nodecount+") "+this.traceid+" from: "+this.currentObject.GetType()+"@"+this.traceline.startingpoint.trico()+" To: "+this.traceline.direction.trico()+"(unknown)";
		}
		return "Trace ("+this.nodecount+") "+this.traceid+" from: "+this.currentObject.GetType()+"@"+this.traceline.startingpoint.trico()+" To: "+this.traceline.direction.trico()+"("+this.endloc.trico()+")";
	}

	public string ToCompactString() {
		string endlocstr = (this.endloc == null ? "{0}" : "†"+this.endloc.tricoshort()+"");
		return "@"+this.traceline.startingpoint.tricoshort()+"→"+this.traceline.direction.tricoshort()+endlocstr+"#"+this.GetHashCode();
		}
	
	
	/// <summary>
	/// End the trace by setting it no longer alive and thereby making the next cycle void.
	/// The trace is stored at the tracejournal as:
	/// * casualty
	/// To also store the trace as a recorded trace, use the "perish(perish_location)" method.
	/// </summary>
	public void perishQuietly() {
		this.alive = false;
		Scientrace.TraceJournal tj = TraceJournal.Instance;
		tj.recordCasualty(new Spot(this.endloc, this.currentObject, this.intensity, this.intensityFraction(), this));
		}
		
	/// <summary>
	/// End the trace by setting it no longer alive (and make the next cycle void) ending the trace at a given location. 
	/// The trace is stored at the tracejournal as:
	/// * trace
	/// * casualty (via the perishQuietly method)
	/// </summary>
	/// <param name="perishlocation">The location where the current trace is ended.</param>
	public void perish(Scientrace.Location perish_location) {
		this.endloc = perish_location;
		Scientrace.TraceJournal tj = TraceJournal.Instance;
		tj.recordTrace(this);
		this.perishQuietly();
		}
		
	public double intensityFraction() {
		double ret = this.intensity/this.original_intensity;
		return ret;
		}
		
	public bool isEmpty() {
		//Console.WriteLine(this.intensityFraction()+" < " + this.lightsource.minimum_intensity_fraction+" (min)"
		//+"("+this.intensity+")/("+this.original_intensity+")");
		if ((this.intensityFraction() <= this.lightsource.minimum_intensity_fraction)
			||
			(this.nodecount >= this.lightsource.max_interactions)) { 
			//Console.WriteLine("I am empty. Will die NOW@"+this.currentObject.tag+this.traceline.startingpoint.trico());
			this.alive = false;	
			return true;
		}
		return false;
	}

	public double absorpByObject(Scientrace.Object3d anObject3d, Scientrace.UnitVector norm, Object3d previousObject) {
		double oldintensity = this.intensity;
		double oldintensityfraction = this.intensityFraction();
		//EXTENDED 20150706
		double absorption_fraction = anObject3d.materialproperties.absorption(this, norm, previousObject.materialproperties);
		double absorption = absorption_fraction*oldintensity;
		double absorptionIntensityFraction = absorption_fraction*oldintensityfraction;
		this.intensity = oldintensity-absorption;

		if (absorption > MainClass.SIGNIFICANTLY_SMALL) { //don't write a spot at ~0 absorption.
		TraceJournal.Instance.recordSpot(new Scientrace.Spot(this.traceline.startingpoint, 
			                                                  anObject3d,
			                                                  absorption, absorptionIntensityFraction,
			                                                  this));
			}
		if (this.intensityFraction() <= this.lightsource.minimum_intensity_fraction) {
			//Console.WriteLine("INTENSITY TOO LOW"+anObject3d.ToString()+"/"+anObject3d.materialproperties.absorption(this));
			this.perish(this.traceline.startingpoint);
			}
		return absorption_fraction;
		}

	public double debug_AngleEnterSurfaceWithTraceNormal(Intersection anIntersection) {
		return this.traceline.direction.angleWith(anIntersection.enter.flatshape.plane.getNorm())*180/Math.PI;
		}
	public void debug_IntersectionToConsole(Intersection anIntersection, string tag) {
		Console.WriteLine(tag+": "+anIntersection.ToString()+
				"\n\t norm: "+anIntersection.enter.flatshape.plane.getNorm()+
				"\n\t deg: "+this.debug_AngleEnterSurfaceWithTraceNormal(anIntersection)+
				"\n\t dotproduct: "+this.traceline.direction.dotProduct(anIntersection.enter.flatshape.plane.getNorm()) );
		}
	public void debug_IntersectionValuesToConsole(Intersection in_before, Intersection in_after) {
		Console.WriteLine("### DEBUG ###");
		this.debug_IntersectionToConsole(in_before, "-  PRE");
		this.debug_IntersectionToConsole(in_after, "- POST");
		}

	public bool validFlatPlaneModification(Intersection in_before, Intersection in_after) { return this.validFlatPlaneModification(in_before, in_after, this);}
	public bool validFlatPlaneModification(Intersection in_before, Intersection in_after, Trace aTrace) {
		Vector in_dir = aTrace.traceline.direction;
		UnitVector old_norm = in_before.enter.flatshape.plane.getNorm();
		UnitVector new_norm = in_after.enter.flatshape.plane.getNorm();
		Vector refl_dir = in_dir.reflectOnSurface(new_norm);
		bool reflection_ok = in_dir.dotProduct(old_norm) * refl_dir.dotProduct(old_norm) < 0;
		bool incident_side_unchanged = in_dir.dotProduct(old_norm) * in_dir.dotProduct(new_norm) > 0;
/*		Console.WriteLine("reflection: "+reflection_ok+ ", side: "+incident_side_unchanged);
	Console.WriteLine("Incident beam dir: "+in_dir.ToString()+" dotp:"+in_dir.dotProduct(old_norm));
	Console.WriteLine("Refl beam dir: "+refl_dir.ToString()+" dotp:"+refl_dir.dotProduct(new_norm));*/
		return incident_side_unchanged && reflection_ok;
		}


	/// <summary>
	/// The Cycle is actually the main action for a trace. If you start somewhere in 
	/// space with components, with a trace with a direction, what components do you
	/// encounter? How does the trace interact? It starts new traces that cycles in
	/// turn, or it might just "end" due to absorption, a max. number of interactions,
	/// too low intensity, whatever.
	/// </summary>
	public void cycle() {
		// Does cycling still makes sense? If not: don't.
		if (!this.alive || this.isEmpty()) {
			return;
			}

		bool retry = false;
		// Find intersections 
		Scientrace.Intersection unmod_intersection = this.lightsource.env.intersects(this);
		// Is there an intersection? (an object with a surface in this trace's path?
		if (unmod_intersection.intersects) {
			/* Linear Modifiers are applied. This may cause the intersection to produce a surface with a normal that
			   has an orientation in the opposite direction of the incoming beam. This is not allowed. When this is
			   the case, a new modification is performed on the raw intersection until this is OK. */
			// TODO: create a "safe" applyLinearModifiers method which performs this operation on a ref vector attribute.
			Scientrace.Intersection active_intersection = new Scientrace.Intersection(unmod_intersection);
			if (active_intersection.hasSurfaceNormalModifiers()) do {
				if (retry) {
					active_intersection = new Scientrace.Intersection(unmod_intersection);
					//Console.WriteLine("\n+1\n");
					}
				retry = true;

				// Intoduce modelled surface alterations, e.g. to simulate imperfections.
				active_intersection.applyLinearModifiers();
				//this.debug_IntersectionValuesToConsole(unmod_intersection, active_intersection);
				}
			while (!this.validFlatPlaneModification(unmod_intersection, active_intersection));
	
			// Any new intersection should lie in the line of the direction of the traceline from the startingpoint. Doublecheck:
			if ((active_intersection.enter.loc - this.traceline.startingpoint).dotProduct(this.traceline.direction) < 0) {
				throw new Exception("location lies in the past!"); }

			// FUNCTIONAL PROCEDURES ON TRACE CYCLING
			
			// If the trace is leaving the current component...
			if (active_intersection.leaving) {
				active_intersection.resetObjectToParentWhenLeaving();
				}
			//List<Scientrace.Trace> newTraces = 
				this.processIntersection(active_intersection);
/*			if (active_intersection.hasSurfaceNormalModifiers()) {
				foreach (Trace atrace in newTraces) {
					if (atrace.intensityFraction() == 0)
						continue;
					Console.WriteLine("\n\nTrace: "+this.ToCompactString());
					Console.WriteLine("ToTrace ("+newTraces.Count+"): "+atrace.ToCompactString());
				//Console.WriteLine("Intensity/isalive:"+atrace.intensityFraction()+"/"+atrace.alive);
					Console.WriteLine("New angle with Old normal: "+(atrace.traceline.direction.angleWith(active_intersection.enter.flatshape.plane.getNorm())*180/Math.PI).ToString()+"@ "+active_intersection.object3d.tag);
					Console.WriteLine("New angle with New normal: "+(atrace.traceline.direction.angleWith(unmod_intersection.enter.flatshape.plane.getNorm())*180/Math.PI).ToString());
	
					Console.WriteLine("-Old angle with New normal: "+(this.traceline.direction.angleWith(active_intersection.enter.flatshape.plane.getNorm())*180/Math.PI).ToString()+"@ "+active_intersection.object3d.tag);
					Console.WriteLine("-Old angle with Old normal: "+(this.traceline.direction.angleWith(unmod_intersection.enter.flatshape.plane.getNorm())*180/Math.PI).ToString());
					}
				}*/

			} else {
			/*no intersection before border? End the trace at the border of the environment */
			if (this.lightsource.env.perishAtBorder) {
				//draws last line to the end of the environment, this function also perishes this trace.
				this.lightsource.env.traceLeavesEnvironment(this);
				} else {this.perishQuietly();}
			} // end "if not intersects"
		} // end Cycle method
}
}
