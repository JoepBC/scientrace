// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections.Generic;

namespace ShadowScientrace {
public enum triangularPrismFactoryMethod {
		WidthHeightAndLength = 1,
		//B = 2,
		}}

namespace Scientrace {


public class TriangularPrism : Scientrace.Prism {

	public Scientrace.NonzeroVector width, height;

	// In the production process, the top surface of a prism is often very flat, whereas 
	// the "active" hypotenuse surface has some flaws. The quality of the sides is seldomly relevant.
	// The booleans below allows the setting of the hypotenuse surface only.
	public bool perfectTop = true;
	public bool perfectSides = true;			


	// nomenclature schematiccaly:
	//
	//         top
	//       _____
	//       |   /
	// side  |  /
	//       | /  hypotenuse
	//       |/

	List<Scientrace.UniformTraceModifier> bottom_surface_modifiers = new List<Scientrace.UniformTraceModifier>();
	List<Scientrace.UniformTraceModifier> side_surface_modifiers = new List<Scientrace.UniformTraceModifier>();
	List<Scientrace.UniformTraceModifier> hypotenuse_surface_modifiers = new List<Scientrace.UniformTraceModifier>();
	List<Scientrace.UniformTraceModifier> front_surface_modifiers = new List<Scientrace.UniformTraceModifier>();
	List<Scientrace.UniformTraceModifier> back_surface_modifiers = new List<Scientrace.UniformTraceModifier>();

	private Scientrace.FlatShape2d bottom_flatshape = null;
	private Scientrace.FlatShape2d side_flatshape = null;
	private Scientrace.FlatShape2d hypothenuse_flatshape = null;
	private Scientrace.FlatShape2d front_flatshape = null;
	private Scientrace.FlatShape2d back_flatshape = null;

	//length inherited from Prism
	public TriangularPrism(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops, 
			Scientrace.Location loc, Scientrace.NonzeroVector width, Scientrace.NonzeroVector height, 
			Scientrace.NonzeroVector length) : base(parent, mprops) {
		this.paramInit(loc, width, height, length);
		}

	protected void paramInit(Scientrace.Location loc, Scientrace.NonzeroVector width, Scientrace.NonzeroVector height, 
		Scientrace.NonzeroVector length) {
		if ((loc.x == Double.NaN) || (loc.y == Double.NaN) || (loc.z == Double.NaN) ){
			throw new ArgumentNullException("Location must be numerical!");
			}
		this.loc = loc;
		this.width = width;
		this.length = length;
		this.height = height;
		}


	/* ShadowClass constructor */
	public TriangularPrism(ShadowScientrace.ShadowObject3d shadowObject): base(shadowObject) {
		switch (shadowObject.factory_method??(int)ShadowScientrace.triangularPrismFactoryMethod.WidthHeightAndLength) {
			case (int)ShadowScientrace.triangularPrismFactoryMethod.WidthHeightAndLength: this.shadowFac_Width_Height_And_Length(shadowObject);
				break;
			default:
				throw new ArgumentOutOfRangeException("Factory method "+shadowObject.factory_method+" not found for "+shadowObject.typeString());
			}
		}

	/// <summary>
	/// Required parameters:
	/// (Location) corner_location, 
	/// (NonzeroVector) length
	/// (NonzeroVector) width
	/// (NonzeroVector) height
	/// (boolean) perfect_front (default true)
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>
	protected void shadowFac_Width_Height_And_Length(ShadowScientrace.ShadowObject3d shadowObject) {
		//User values(s)
				Scientrace.Location corner_location = shadowObject.getLocation("corner_location");
				Scientrace.NonzeroVector length = shadowObject.getNzVector("length");
				Scientrace.NonzeroVector width = shadowObject.getNzVector("width");
				Scientrace.NonzeroVector height = shadowObject.getNzVector("height");
		bool perfect_top = shadowObject.getBool("perfect_top")==true;

		//Derived value(s)
		//none

		//initialize!
		this.paramInit(corner_location, width, height, length);

		//add some additional parameters
		this.perfectTop = perfect_top;
		}



	/* LAZY-INITIALIZERS FOR SURFACES */
	public Scientrace.FlatShape2d setBottom() {
		this.bottom_flatshape = new Scientrace.Parallelogram(this.loc, this.width, this.length);
		this.bottom_flatshape.surfaceproperties = SurfaceProperties.NewSurfaceModifiedObject(this, this.bottom_surface_modifiers);
		return this.bottom_flatshape;
		}
	public Scientrace.FlatShape2d getBottom() {
		return this.bottom_flatshape ?? this.setBottom();
		}

	public Scientrace.FlatShape2d setSide() {
		this.side_flatshape = new Scientrace.Parallelogram(this.loc, this.height, this.length);
		this.side_flatshape.surfaceproperties = SurfaceProperties.NewSurfaceModifiedObject(this, this.side_surface_modifiers);
		return this.side_flatshape;
		}
	public Scientrace.FlatShape2d getSide() {
		return this.side_flatshape ?? this.setSide();
		}

	
	public Scientrace.FlatShape2d setHypothenuse() {
		this.hypothenuse_flatshape = new Scientrace.Parallelogram(this.loc+this.width.toLocation(), (this.height-this.width), this.length);
		this.hypothenuse_flatshape.surfaceproperties = SurfaceProperties.NewSurfaceModifiedObject(this, this.hypotenuse_surface_modifiers);
		return this.hypothenuse_flatshape;
		}
	public Scientrace.FlatShape2d getHypothenuse() {
		return this.hypothenuse_flatshape ?? this.setHypothenuse();
		}

	public Scientrace.FlatShape2d setFront() {
		this.front_flatshape = new Scientrace.Triangle(this.loc, this.height, this.width);
		this.front_flatshape.surfaceproperties = SurfaceProperties.NewSurfaceModifiedObject(this, this.front_surface_modifiers);
		return this.front_flatshape;
		}
	public Scientrace.FlatShape2d getFront() {
		return this.front_flatshape ?? this.setFront();
		}

	public Scientrace.FlatShape2d setBack() {
		this.back_flatshape = new Scientrace.Triangle(this.loc+this.length.toLocation(), this.height, this.width);
		this.back_flatshape.surfaceproperties = SurfaceProperties.NewSurfaceModifiedObject(this, this.back_surface_modifiers);
		return this.back_flatshape;
		}
	public Scientrace.FlatShape2d getBack() {
		return this.back_flatshape ?? this.setBack();
		}



	public override Scientrace.Intersection intersects(Trace trace) {
		//Console.WriteLine("PROBING TRIANGULARPRISM "+this.tag+" FROM "+trace.currentObject.tag);
		List<Scientrace.FlatShape2d> pgrams = new List<Scientrace.FlatShape2d>();
		//first: add parallelogram surfaces
			//bottom
		pgrams.Add(this.getBottom());
			//"up"
		pgrams.Add(this.getSide());
			//hypothenusa
		pgrams.Add(this.getHypothenuse());
		//second: add triangular surfaces
			//front
		pgrams.Add(this.getFront());
			//back
		pgrams.Add(this.getBack());
		return this.intersectFlatShapes(trace, pgrams);
		
	}


	public override void addSurfaceModifiers(List<Scientrace.UniformTraceModifier> modifiers_range) {
		this.side_surface_modifiers.AddRange(modifiers_range);
		this.hypotenuse_surface_modifiers.AddRange(modifiers_range);
		if (!this.perfectTop) {
			this.bottom_surface_modifiers.AddRange(modifiers_range);
			}
		if (!this.perfectSides) {
			this.front_surface_modifiers.AddRange(modifiers_range);
			this.back_surface_modifiers.AddRange(modifiers_range);
			}
		// default behaviour for Object3d's below has been disabled:
		//surface_normal_modifiers.AddRange(modifiers_range);
		}

	public override string exportX3D(Scientrace.Object3dEnvironment env) {
// prev after Transform      <Viewpoint description='LineSet cube close up' position='0 0 5'/>
		//TODO: try to transform to indexedlineset
		Scientrace.Vector a, b, c, d, e, f;
		a = Vector.ZeroVector();
		//front triangle
		b = this.width;
		c = this.height;
		d = this.length;
		//back triangle
		e = this.length+this.width;
		f = this.length+this.height;
		//front triangle
		
		//back triangle
		
		//connecting squares
		
		//Console.WriteLine("Doublecheck: "+this.loc.trico());
		
		return "<!-- Start TriangularPrism instance -->\n<Transform translation='"+this.loc.trico()+"'>" +
		      	"<Shape>" +
		      	"<!-- Coordinate triangprism sequence (matches coordIndex actually) 0 1 2 3 0, 4 5 6 7 4, 0 4, 1 5, 2 6, 3 7 -->" +
		      	@"        <LineSet vertexCount='4 4 2 2 2'>" +
		      	          "<Coordinate point='"
				+a.tricon()+b.tricon()+c.tricon()+a.tricon()+d.tricon()+e.tricon()+f.tricon()+d.tricon()+a.tricon()+d.tricon()+b.tricon()+e.tricon()+c.tricon()+f.tricon()+"'/>" +
@"<ColorRGBA color='0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1
0.8 0.5 0 1' />"+
		      	"</LineSet>" +
		      	"</Shape>" +
		      	"</Transform>\n<!-- End TriangularPrism instance -->";
//          <Color color='1 0 0 1 0.5 0 1 1 0 0 1 0 tri 0 0 0 0 1 0.8 0 0.8 0.3 0 0.3 1 1 1 0 0 1 1 0 0 0 0 1 1 0.5 0 0.8 0 0.8 1 1 0 0.3 0 0.3 0 1 0 1 1 1'/>
		
	}
}
}
