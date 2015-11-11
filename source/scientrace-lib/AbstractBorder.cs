// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public abstract class AbstractGridBorder : IBorder3d {

/*
 * Abstractborder will propagate some abstract methods implemented by BoxBorder and the possible
 * future created different types of borders.
 */
	public Scientrace.VectorTransform trf;
	
	public AbstractGridBorder() {
	}
	
	//public abstract Intersection intersectsObject(Trace trace, Object3d o3dWithThisBorder); // {
		/*throw new NotImplementedException();
		}*/
	//public abstract Scientrace.IntersectionPoint[] intersects(Scientrace.Trace trace);

	public abstract bool contains(Scientrace.Location loc);
		
	public virtual VectorTransform getTransform() {
		//construction on request
		if (this.trf == null) {
			this.trf = this.createNewTransform();
		}
		return this.trf;
		}

	public abstract VectorTransform createNewTransform();

	/// <summary>
	/// This method should be overloaded to get a VectorTransformation instance that for 
	/// example rotates a grid with the orientation of the object.
	/// </summary>
	/// <param name="nz">
	/// A <see cref="NonzeroVector"/>
	/// </param>
	/// <returns>
	/// A <see cref="Scientrace.VectorTransform"/>
	/// </returns>
	public virtual Scientrace.VectorTransform getGridTransform(UnitVector griddirection) {
		Console.WriteLine("getGridTransform not implemented, using static grid for "+this.GetType().ToString()+" instance.");
		return this.getTransform();
		}
		
	public virtual string exportX3D (Scientrace.Object3dEnvironment env) { 
			return "<!-- X3D export not implemented for type of border -->";
		}
		
		
	public abstract Scientrace.Location getCenterLoc();

	/// <summary>
	/// When probing the entire volume of the border along the "getOrthoDirection" direction,
	/// this would be a nice (centered) place to start, from which all points lie in the
	/// OrthoDirection direction of this point. This feature can be used when drawing a
	/// grid on a surface that is limited by a border.
	/// </summary>
	/// <returns>
	/// A <see cref="Scientrace.Location"/>
	/// </returns>
	public abstract Scientrace.Location getOrthoStartCenterLoc();
	public abstract Scientrace.UnitVector getOrthoDirection();
		
	public abstract double getRadius();

	public virtual double getURadius() { 
			return this.getRadius();
		}		

	public virtual double getVRadius() { 
			return this.getRadius();
		}		
		


}
	
}
