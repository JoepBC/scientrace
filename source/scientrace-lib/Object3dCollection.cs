// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace Scientrace {

public class Object3dCollection : Scientrace.Object3d {

	/// <summary>
	/// Quite often it is desired to group objects within a collection, without actually 
	/// creating a new environment in between the parent collection and the objects within
	/// this collection. A specific example might be a collection within an AttachedO3dCollection,
	/// when leaving the physical objects, the tracer will pretend to be inside this subcollection,
	/// which is actually also a part of the attached-objects. Currently no refraction is calculated
	/// when leaving a physical object and entering a new object within the attached collection.
	/// When making a collection "Virtual" the tracer doesn't fall back to this collection, but to
	/// its own parent, preventing this problem to occur.
	/// </summary>
	public bool virtualCollection = false;

	/* 
	/// <summary>
	/// If a collection contains a lot of objects, it might be usefull to specify a large border
	/// whithin they all should exist. This way the raytracer can check whether they all should
	/// be parsed individually in the first place or not. Leaving this object "null" it doesn't
	/// perform this check. Any type of object will do, including collections. Of course always
	/// keep in mind the only purpose is to gain performance.
	/// </summary> 
	// public Object3d contentsWithinVirtualObject = null; MADE OBSOLETE BY DUMMYBORDER */

	/// <summary>
	/// A collection of objects which will be iterated one by one when the collection is addressed.
	/// </summary>
	/// <param name="parent">
	/// A <see cref="Object3dCollection"/>. There always has to be a parent collection in which
	/// an object (including objectcollections which are also objects as such) exists. The top 
	/// collection (root) is the Object3dEnvironment collection.
	/// </param>
	/// <param name="mp">
	/// A <see cref="MaterialProperties"/>. The surroundings in which the objects are situated?
	/// </param>
	public Object3dCollection(Object3dCollection parent, MaterialProperties mp) : base(parent, mp) {
	}
	public Object3dCollection(ShadowScientrace.ShadowObject3d aShadowObject):base(aShadowObject) {}	
	
	//Attributes
	public ArrayList objects = new ArrayList();
	//Methods
	public void addObject3d(Object3d anObject3d) {
		//Console.WriteLine("ADDING OBJECT: "+anObject3d.tag);
		this.objects.Add(anObject3d);
	}
	public int count() {
		return this.objects.Count;
	}

		
	public override void addSurfaceModifiers(List<Scientrace.UniformTraceModifier> modifiers_range) {
		foreach (Object3d anO3d in this.objects) {
			/*Console.WriteLine("ADDING TO CHILD OBJECT: "+anO3d.tag+modifiers_range.Count);
			foreach(UniformTraceModifier mo in modifiers_range) {
				Console.WriteLine(mo.ToString());
				}*/
			anO3d.addSurfaceModifiers(modifiers_range);
			}
		}

	
	public override Intersection intersects(Trace trace) {
		Scientrace.Intersection firstIntersection = new Scientrace.Intersection(false, null);
		Scientrace.Intersection currentintersection = new Scientrace.Intersection(false, null);
		double? firstdistance = null;
		double currentdistance;
		
		
		SortedList slobjects = new SortedList();
		//Console.WriteLine(this.tag+" HAS "+this.objects.Count+ " o3ds");
		//copying (by reference of course) all objects to a sortedlist.
		foreach (Object3d iobject3d in this.objects) {
			//Console.WriteLine("POKING "+ iobject3d.tag);
			slobjects.Add(iobject3d.parseOrder, iobject3d);
			}
		foreach (DictionaryEntry deObject3d in slobjects) {
			Object3d iObject = (Object3d)deObject3d.Value; 
			//validating currentintersection:
			currentintersection = iObject.intersectsBefore(trace, currentintersection);

			/*if (currentintersection==null) { continue; }
			if (currentintersection.enter==null) { continue; }
			if (currentintersection.exit==null) { continue; } */

			//currentintersection = iObject.intersects(trace);
			 
		/*foreach (Object3d iobject3d in this.objects) {
			currentintersection = iobject3dl.intersects(trace);*/
			REPARSE:
			if (!currentintersection.intersects) {
				continue; //no intersection with this object? Try next object in collection (continue)
			}

			if ((currentintersection.enter.loc-trace.traceline.startingpoint).dotProduct(trace.traceline.direction) < 0) {
				//Console.WriteLine("Found object lies in the past! Skipping "+iObject.tag);
				continue;
				}// else {Console.WriteLine("not in the past, but :"+(currentintersection.enter.loc-trace.traceline.startingpoint).dotProduct(trace.traceline.direction));}

			currentdistance = currentintersection.enter.loc.distanceTo(trace.traceline.startingpoint);
				
			if (currentintersection.object3d == trace.currentObject) {
				/* If you're inside an object and the beam meets the same object, 
				 * it should only have an "enterloc" which is where the beam "leaves" the object. */
				if (currentintersection.exit != null) { // .loc part after exit removed at 20131205
					/* perhaps the intersection has found the object at the same starting point */
					if (currentintersection.enter.loc.distanceTo(trace.traceline.startingpoint)
						< trace.getMinDistinctionLength()) {
						Console.WriteLine("New functionality: removing exit for "+currentintersection.object3d.tag);
						currentintersection.removeExit();
						goto REPARSE;
						}
					/* check this occuring warning out after internal reflections. Doesn't 
					 * seem to cause any problems though. */
					if (currentintersection.object3d != trace.currentObject) { // because if they are the same it seems like internal reflection
						Console.WriteLine("WARNING @"+this.tag+": Collision ("+currentintersection.object3d.tag+")"+
						" with current object ("+trace.currentObject.tag+")"+
						" without leaving first \n-----\n"+					
							trace.ToCompactString()+ "\nentering: "+currentintersection.enter.loc.trico()+
							                  "\nexit:"+currentintersection.exit.loc.tricon()+"-----");
						} //endif (currentintersection.object3d != trace.currentObject)
					
					continue;
					/* else: if no exit location is set, we must be leaving the current object at "enter" */
					} else { // Leaving the current object should return to the parent collection (environment?).
					currentintersection.leaving = true;
					}
				/* else: if the colliding object isn't the current object */
				} else {
				// analogue as above but inverted: you cannot leave an object you're not inside.
				if (currentintersection.exit == null) {  // .loc part after exit removed at 20131205
						//or can you?
						//continue; NO LONGER ABORT PARSING HERE, BECAUSE:
						//CONCLUSION:
						//YES YOU CAN, think about an object within an object or a reflective layer
						if (currentintersection.object3d.hasVolume) {
							//Console.WriteLine("Leaving object with volume("+currentintersection.object3d.tag+"), but don't refract");
							continue;
						}
				}
			}
			if (!firstIntersection.intersects) {
				firstIntersection = currentintersection;
				firstdistance = firstIntersection.enter.loc.distanceTo(trace.traceline.startingpoint);
				continue;
			}
			/* below a firstInteraction is already set */
			currentdistance = currentintersection.enter.loc.distanceTo(trace.traceline.startingpoint);

				if (firstIntersection.leaving) {
				/* in order to make "attached objects" change into one another the
				 * colliding object has to be found instead of the "leaving" border. */
				/* object which are seperated from each other with less than the lights wavelength
				 * will be treated as connected to each other. Not first leaving previous border */
				if (currentdistance - firstdistance < trace.getMinDistinctionLength()) {
					firstIntersection = currentintersection;
					firstdistance = currentdistance;
					continue;
					}
				continue;
				}
/*			if (firstdistance == null) {
				throw new NullReferenceException("firstdistance not set");
			} */ //this shouldn't occur and doesn't, so not checking anymore
			if (firstdistance > currentdistance) {
				firstdistance = currentdistance;
				firstIntersection = currentintersection;
				continue;
			}
			/* TODO for optimization purposes:
			 * find whether any "containing" object is intersected, find first and return an Intersection */
		}
		/*if (firstIntersection.leaving) {
			//Console.WriteLine("----------> leaving *** "+currentintersection.object3d);
			}*/
		return firstIntersection;
	}

	public override string exportX3D(Scientrace.Object3dEnvironment env) {
		StringBuilder retsb = new StringBuilder(1024);
		//reserve a memory size of 1024 chars "to start off with".
		foreach (Object3d object3d in this.objects) {
			retsb.Append(object3d.exportX3D(env));
		}
		
		return retsb.ToString();
	}
	
}
}
