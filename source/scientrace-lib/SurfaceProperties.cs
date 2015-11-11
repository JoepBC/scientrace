// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {
public class SurfaceProperties {

	/// <summary>
	/// The "surface_normal_modifiers" with a surface describes how the "theoretic" surface normal at
	/// the surface of an object is modified. In most cases only a single modifier will be adressed.
	/// When instances of this class are appointed to an intersection the modifiers of the Object3d
	/// will be overwritten.
	/// </summary>
	public List<Scientrace.UniformTraceModifier> surface_normal_modifiers = new List<UniformTraceModifier>();

	/// <summary>
	/// Materialproperties store information of the material of the object,
	/// for a collection mostly to be air or vacuum, a prism e.g. PMMA etc.
	/// </summary>
	public Scientrace.MaterialProperties materialproperties;
		
	/// <summary>
	/// The Object3d instance the Surface is part of.
	/// </summary>
	public Scientrace.Object3d object3d;

	private SurfaceProperties() {
		}
		
	public static Scientrace.SurfaceProperties NoModifierSurfaceForObject(Scientrace.Object3d anObject) {
		Scientrace.SurfaceProperties retSurf = new Scientrace.SurfaceProperties();
		retSurf.materialproperties = anObject.materialproperties;
		return retSurf;
		}

		
	public static Scientrace.SurfaceProperties NewSurfaceModifiedObject(Scientrace.Object3d anObject,
			List<Scientrace.UniformTraceModifier> surface_normal_modifiers) {
		Scientrace.SurfaceProperties retSurf = new Scientrace.SurfaceProperties();
		retSurf.materialproperties = anObject.materialproperties;
		retSurf.addSurfaceModifiers(surface_normal_modifiers);
		return retSurf;
		}
				
								
	public virtual void addSurfaceModifiers(List<Scientrace.UniformTraceModifier> modifiers_range) {
		surface_normal_modifiers.AddRange(modifiers_range);
		}
																
		
	}}