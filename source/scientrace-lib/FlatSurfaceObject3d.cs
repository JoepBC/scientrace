// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;


namespace Scientrace {

public abstract class FlatSurfaceObject3d : PhysicalObject3d {

	/// <summary>
	/// Instead of just the outlining, some objects can be drawn as a solid/filled surface in their 3d export if x3d_fill is set true
	/// UPDATE: EARLY REPLACED BY SETTING RESP. Emmissive or Diffuse COLOURS 
	/// </summary>
	//public bool x3d_fill = false;

	/// <summary>
	/// If x3d_fill is set true, but no bitmap (x3d_fill_bitmap) is defined or the file cannot be found, this colour is used in the x3d export.
	/// </summary>
	public RGBColor x3d_fill_emissive_color = null;
	public RGBColor x3d_fill_diffuse_color = null;

	/// <summary>
	/// A bitmap/texture referred to in the X3D file. Relative paths are related to the "output dir" loction where the x3d file itself is written.
	/// </summary>
	public string x3d_fill_bitmap = null;
	/// <summary>
	/// By default (x3d_fill_diffuse = false) the X3D colour is applied by setting an "emissive colour". By setting this boolean true, 
	/// a colour will be presented which brightness will depend on the angle under which it is viewed (related to x3d_fill_normal)
	/// giving some "shading like" effects.
	/// UPDATE: EARLY REPLACED BY SETTING RESP. Emmissive or Diffuse COLOURS 
	/// </summary>
	//public bool x3d_fill_diffuse = false;


	/// <summary>
	/// The angle which shows the brightest x3d_fill_color when viewing. When set null the normal of the surface will be used.
	/// </summary>
	public Scientrace.Vector x3d_fill_normal = null;
	/// <summary>
	/// When x3d_fill_both_sides is set false the surface will be transparant when viewed from the back.
	/// </summary>
	public bool x3d_fill_both_sides = true;


	/// <summary>
	/// The front_normal_direction vector is ONLY to be used to retrieve whether a vector
	/// is pointing FROM the front or the back of the surface/cell. Its detailed orientation has 
	/// no additional relevance. This normal points OUT OF THE FRONT (NOT TOWARDS!)
	/// </summary>
	public Scientrace.NonzeroVector front_normal_direction = null;

	/* CONSTRUCTORS */
	public FlatSurfaceObject3d(ShadowScientrace.ShadowObject3d aShadowObject):base(aShadowObject) {
		//only front surface by default:
		//this.x3d_fill = aShadowObject.getBool("x3d_fill", this.x3d_fill);
		this.x3d_fill_emissive_color = aShadowObject.getColorFromHTML("x3d_fill_emissive_color_html", this.x3d_fill_emissive_color);
		this.x3d_fill_diffuse_color = aShadowObject.getColorFromHTML("x3d_fill_diffuse_color_html", this.x3d_fill_diffuse_color);
		this.x3d_fill_bitmap = aShadowObject.getString("x3d_fill_bitmap");
		this.x3d_fill_both_sides = aShadowObject.getBool("x3d_fill_both_sides", this.x3d_fill_both_sides);
		//this.x3d_fill_diffuse = aShadowObject.getBool("x3d_fill_diffuse", this.x3d_fill_diffuse);
		this.x3d_fill_normal = aShadowObject.getVector("x3d_fill_normal", this.x3d_fill_normal);
		}
	
	public FlatSurfaceObject3d(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops):base(parent, mprops) {          
		//this.surface_sides = new HashSet<SurfaceSide>() {Scientrace.SurfaceSide.Both, Scientrace.SurfaceSide.Front, Scientrace.SurfaceSide.Back};
		
		//only front surface by default:
		this.surface_sides = new HashSet<SurfaceSide>() {Scientrace.SurfaceSide.Front};
		}
	/* END CONSTRUCTORS */

	public override bool directedTowardsObjectside(Scientrace.SurfaceSide side, Trace aTrace) {
		Scientrace.NonzeroVector traceline_direction = aTrace.traceline.direction;
		switch (side) {
			case Scientrace.SurfaceSide.Back: // == SurfaceSide.Inside
				// Vector is directed TOWARDS (so in the contrary direction) of the surface normal
//				if (aVector.dotProduct(this.front_normal_direction)>0) 
					/*Console.WriteLine(aTrace.traceid+" analysing: "+traceline_direction.tricon()+" from: "+aTrace.traceline.startingpoint.tricon()+
						aTrace.parent.intensityFraction()+" = intensity \n"+
						" DISTANCE: "+(aTrace.endloc-aTrace.traceline.startingpoint).length +" direction along: "+this.front_normal_direction.trico()+ " @"+aTrace.currentObject.GetType()); */
				return (traceline_direction.dotProduct(this.front_normal_direction)>0);
				//break;
			case Scientrace.SurfaceSide.Front:
				// Vector is directed AWAY FROM (so along the direction) of the surface normal
				return (traceline_direction.dotProduct(this.front_normal_direction)<0);
				//break;
			case Scientrace.SurfaceSide.Both:
				return true;
				//break;
			default:
				throw new NotImplementedException("Side "+side.ToString()+" not implemented");
				//break;
			}
		}
		
		
		
	}}

