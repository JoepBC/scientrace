// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;


namespace Scientrace {

/// <summary>
/// The Object3d class is the class which both ObjectCollection
/// and PhysicalObject inherit from.
/// </summary>
public abstract class Object3d {

	/// <summary>
	/// The parent attribute stores the collection of which
	/// the Object3d instance is a member.
	/// </summary>
	public Object3dCollection parent;

	/// <summary>
	/// Collections and Surfaces don't have volumes. EnclosedVolumes for example do.
	/// </summary>
	public bool hasVolume = false;	
		
	/// <summary>
	/// Materialproperties store information of the material of the object,
	/// for a collection mostly to be air or vacuum, a prism e.g. PMMA etc.
	/// </summary>
	public Scientrace.MaterialProperties materialproperties;

	/// <summary>
	/// A tag is the name an object in space can be recognized by. More like a caption or a label. The default
	/// value should be the GetType value (class name) of the object.
	/// </summary>
	public string tag;

	/// <summary>
	/// Some objects (e.g. collections) are expensive to parse and a rough (always larger) border 
	/// can be defined in which the object exists. This way this object is parsed first, compared
	/// whether "this" object is intersected "at all", and "if", whether its first possible inter-
	/// section is before the first current collision-point in the "intersectbefore" function before
	/// parsing this object thoroughly.
	/// </summary>
	public Object3d dummyborder;

	/// <summary>
	/// The "ParseOrder" attribute is included to give a priority to objects within a collection. The
	/// objects with the lowest value are parsed first in a collection. If no order is given, a random
	/// floating point value between "0" and "1" is addressed. Negative values will be parsed "before"
	/// not addressed values, and values > 1 *after*.
	/// </summary>
	public double parseOrder;

	/// <summary>
	/// The "surface_normal_modifiers" with an object describes how the "theoretic" surface normal of
	/// an object is modified. In most cases only a single modifier will be adressed to an object.
	/// The collection of modifiers can be overwritten for surfaces using the Surface class.
	/// </summary>
	public List<Scientrace.UniformTraceModifier> surface_normal_modifiers = new List<UniformTraceModifier>();
		
	/// <summary>
	/// The spotrevenue of a function is the total amount of light (intensity"loss") on an object
	/// </summary> 
	// TODO check change to default 0 from "not set" by jbos @ 20111209.
	private double spotRevenue = 0;
		
	private double totalFlushedRevenue = 0;

	/// <summary>
	/// All Object3d instances should be created with an assigned parent,
	/// an ObjectCollection instance. Except for Environment instances which
	/// are "top level" collections. Their parent is automatically set "null".
	/// </summary>
	/// <param name="parent">
	/// A <see cref="Object3d"/>
	/// </param>
	public Object3d(Object3dCollection parent, MaterialProperties mp) {
		this.setDefaults(parent, mp);
		}
		
	public Object3d(ShadowScientrace.ShadowObject3d aShadowObject) {
		//Console.WriteLine("adding shadowobject to parent: "+aShadowObject.parent.tag+" and material" +aShadowObject.materialprops);
		this.setDefaults(aShadowObject.parent, aShadowObject.materialprops);

		//if tag is set for this object, pass it on to Object3D instance
		if (aShadowObject.hasTag()) this.tag = aShadowObject.tag;
		//same for parseorder
		if (aShadowObject.hasParseOrder()) this.parseOrder = (double)aShadowObject.parseorder;
		}				
				
				
	private void setDefaults(Object3dCollection parent, MaterialProperties mp) {
		this.tag = this.GetType().ToString();
		this.parseOrder = TraceJournal.Instance.rnd.NextDouble();
		this.materialproperties = mp;
		this.parent = parent;
		if (this.hasParent())
			this.parent.addObject3d(this);
		}
	
											
	public Object3dCollection getFirstNonVirtualParent() {
		Object3d tobject1 = this;
		while (tobject1.parent != null) {
			if (tobject1.parent.virtualCollection == false) {
				//Console.WriteLine(this.tag+" returns "+tobject1.parent.tag);
				return tobject1.parent;
				} else {
				tobject1 = tobject1.parent;
				}
			}
		throw new Exception("The top parent (environment) should always be non-virtual");
		}

		
	public static bool operator ==(Scientrace.Object3d o3d1, Scientrace.Object3d o3d2) {
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(o3d1, o3d2)) {
			return true;
		}

		// If one is null, but not both, return false. NO in ALL other cases:
		//if (((object)v1 == null) || ((object)v2 == null)) {
			return false;
		//}
	}
		
	public static bool operator !=(Scientrace.Object3d o3d1, Scientrace.Object3d o3d2) {
		return !(o3d1 == o3d2);
		//This will call to operator == simple way to implement !=
	}

	public override bool Equals(object o) {
		return this == (Scientrace.Object3d)o;
	}

	public override int GetHashCode() { //prevent warnings
		return base.GetHashCode();
	}
		
	public bool hasParent() {
		return !(this.parent == null);
	}

	public void spotted(double intensity) {
		this.spotRevenue += intensity;
		}

	public double getRevenue() {
		return this.spotRevenue;
		}
		
	public double getTotalRevenue() {
		return this.getRevenue()+this.totalFlushedRevenue;		
		}
		
	public void flushRevenue() {
		this.totalFlushedRevenue += this.spotRevenue;
		this.spotRevenue = 0;
		}

	/// <summary>
	/// Returns the parent Object3D(Collection) or this/self if root.
	/// </summary>
	/// <returns>
	/// The parent Object3D(Collection) or this/self if root.
	/// </returns>
	public Scientrace.Object3d getParentOrRoot() {
			if (this.hasParent()) {
				return this.parent;
			} else {
				return this;
			}
		}

	public Object3d () {
		throw new NotImplementedException();
		}

	public abstract string exportX3D(Scientrace.Object3dEnvironment env);

	/*	public void reflect(Scientrace.Trace trace, Scientrace.Intersection intrs) {

		}*/

	public bool attachedTo(Object3d object3d) {
		if ((this.GetType() == typeof(AttachedObject3dCollection)) || object3d.GetType() == typeof(AttachedObject3dCollection)) {
			return false;
		}
		
		AttachedObject3dCollection aoc = null;
		Object3d tobject1 = this;
		while (tobject1.parent != null) {
			if (typeof(AttachedObject3dCollection).IsAssignableFrom(tobject1.parent.GetType())) {
				//Console.WriteLine(tobject1.parent.GetType()+" is attacheble");
			//if (tobject1.parent.GetType() == typeof(AttachedObject3dCollection)) {
				aoc = (AttachedObject3dCollection)tobject1.parent;
				//Console.WriteLine(tobject1.parent.GetType());
				break;
			} else {
				tobject1 = tobject1.parent;
			}
		}
		//end while
		if (aoc == null) {
				return false;
			}
		Object3d tobject2 = object3d;
		while (tobject2.parent != null) {
			if (typeof(AttachedObject3dCollection).IsAssignableFrom(tobject2.parent.GetType())) {
			//if (tobject2.parent.GetType() == typeof(AttachedObject3dCollection)) {
				//Console.WriteLine("********** ATTACHED OBJECTS "+this+" & "+object3d);
				return (tobject2.parent == aoc);
			} else {
				tobject2 = tobject2.parent;
			}
		}
		//end while
		return false;
	}

	public virtual void addSurfaceModifiers(List<Scientrace.UniformTraceModifier> modifiers_range) {
		surface_normal_modifiers.AddRange(modifiers_range);
		}

	public abstract Intersection intersects(Trace trace);
	
	public virtual Intersection intersectsBefore(Trace trace, Intersection intrs) {
		if (!this.hasDummyBorder()) { // no dummyborder? Do "normal" procedure
			return this.intersects(trace);
			}
		if (this.dummyBefore(trace, intrs)) {	//is dummy intersected before intrs?
			return this.intersects(trace); //parse this object
			} else {
			return new Intersection(false,this); //do not parse this object
			}
		}
		
	public bool hasDummyBorder() {
		return (this.dummyborder!=null);
		}
		
	public bool dummyBefore(Trace trace, Intersection previousintersection) {
		Intersection dummyintersect = this.dummyborder.intersects(trace);
		if (!dummyintersect.intersects) { //dummyborder is NOT intersected.
			//Console.WriteLine("dummy NOT @ "+this.tag+" trace:"+trace.traceid+"@"+trace.traceline.ToString());
			return false;
			}
		//dummyborder *is* intersected. Is it before the previous intersection?
		if (!previousintersection.intersects) {
			//the previous intersection does not exist, so dummy is always first.
			//Console.WriteLine("dummy IS @ "+this.tag+" trace:"+trace.traceid+"@"+trace.traceline.ToString());
			return true;
			}
		if (trace.traceline.startingpoint.distanceTo(previousintersection.enter.loc) 
			    <
			trace.traceline.startingpoint.distanceTo(dummyintersect.enter.loc)) {
			//Console.WriteLine("dummy AFTER @ "+this.tag+" trace:"+trace.traceid+"@"+trace.traceline.ToString());
			return false;
			} else {
			//Console.WriteLine("dummy BEFORE @ "+this.tag+" trace:"+trace.traceid+"@"+trace.traceline.ToString());
			return true;
			}
		}

	public List<ObjectLinePiece> toOLP(List<LinePiece> aList, string colour) {
		return this.toOLP(aList, colour, this);
		}
						
	public List<ObjectLinePiece> toOLP(List<LinePiece> aList, string colour, Object3d anO3D) {
		List<ObjectLinePiece> olplist = new List<ObjectLinePiece>();
		foreach (LinePiece anLP in aList) {
			ObjectLinePiece anOLP = new ObjectLinePiece();
			anOLP.o3d = anO3D;
			anOLP.lp = anLP;
			anOLP.col = colour;
			olplist.Add(anOLP);
			}
		return olplist;
		}		

	}}
