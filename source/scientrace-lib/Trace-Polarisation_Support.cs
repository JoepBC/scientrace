// /*
//  * Scientrace by Joep Bos-Coenraad - jbos@scientrace.org
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {


/// <summary>
/// Polarisation Support was added in a later stage. Althoug it has been integrated in all regular
/// routines, for clearity purposes, it's functions have been grouped within this file.
/// </summary>
public partial class Trace {

	/// <summary>
	/// This method switches between the old "always 50/50 polarisation on interaction" functionality
	/// and the current functionality which supports polarisation (disregarding photon phase and therefore
	/// only a basic approach to polarisation, i.e. not discriminating between linear and circular polarisation).
	/// </summary>
	public static bool support_polarisation = true;

	/// <summary>
	/// The direction AND amplitude (its length) of the first polarization vector.
	/// All polarisations are split up/decomposed in two vectors. Their phase is neglected.
 	/// Upon interection they are re-arranged in an "s" and a "p" direction which
	/// will become the polarisation decomposation vectors. Their amplitudes are defined
	/// by splitting up these two vectors and adding the two resulting fractions for "s" and "p".
	/// NOTE THAT THE AMPLITUDE IS NOT THE ENERGY OF THE TRACE
	/// </summary>
	private Vector pol_vec_1 = null;
	/// <summary>
	/// The direction AND amplitude (its length) of the first polarization vector.
	/// All polarisations are split up/decomposed in two vectors. Their phase is neglected.
 	/// Upon interection they are re-arranged in an "s" and a "p" direction which
	/// will become the polarisation decomposation vectors. Their amplitudes are defined
	/// by splitting up these two vectors and adding the two resulting fractions for "s" and "p".
	/// NOTE THAT THE AMPLITUDE IS NOT THE ENERGY OF THE TRACE
	/// </summary>
	private Vector pol_vec_2 = null;


	public void setCircularPolarisation(Vector u, Vector v) {
		//The length of the polarisation vector squared should equal the intensity.
		double total_vector_size = Math.Sqrt((u.length*u.length) + (v.length*v.length));
		if (total_vector_size == 0) 
			throw new ZeroNonzeroVectorException("The polarisation vector of a trace may never be a zero vector.");
		this.pol_vec_1 = u/total_vector_size;
		this.pol_vec_2 = v/total_vector_size;
		this.pol_vec_1 = u*Math.Sqrt(this.intensity)/(total_vector_size);
		this.pol_vec_2 = v*Math.Sqrt(this.intensity)/(total_vector_size);
		}


	public void setUniformPolarisation() {
		Scientrace.Plane dummyPlane = Scientrace.Plane.newPlaneOrthogonalTo(Scientrace.Location.ZeroLoc(),
			this.traceline.direction);
		this.pol_vec_1 = dummyPlane.u.toVector()*Math.Sqrt(0.5*this.intensity);
		this.pol_vec_2 = dummyPlane.v.toVector()*Math.Sqrt(0.5*this.intensity);
		}

	public Vector getPolarisationVec1() {
		if (this.pol_vec_1 == null)
			this.setUniformPolarisation();
		return this.pol_vec_1;
		}

	public Vector getPolarisationVec2() {
		if (this.pol_vec_2 == null)
			this.setUniformPolarisation();
		return this.pol_vec_2;
		}

	public double getPolarisationAmplitude1() {
		return this.pol_vec_1.length;
		}
	public double getPolarisationAmplitude2() {
		return this.pol_vec_2.length;
		}

	/// <summary>
	/// Call this method to define a linearly polarised beam.
	/// </summary>
	/// <param name="aVector">The vector (orthogonal to the direction of the trace) of the polarisation.</param>
	public void setLinearPolarisation(Vector aVector) {
		if (aVector.length == 0) {
			throw new ZeroNonzeroVectorException("The polarisation vector of a trace may never be a zero vector.");
			}
		this.pol_vec_1 = aVector;
		this.pol_vec_2 = Scientrace.Vector.ZeroVector();
		}

	/// <summary>
	/// Call this method to define a linearly polarised beam.
	/// </summary>
	/// <param name="aVector">The vector (orthogonal to the direction of the trace) of the polarisation.</param>
	public void setLinearPolarisation(Vector aVector, Vector anotherVector) {
		if ((aVector.length == 0) && (anotherVector.length == 0)) {
			throw new ZeroNonzeroVectorException("The polarisation vector of a trace may never be a zero vector.");
			}
		this.pol_vec_1 = aVector;
		this.pol_vec_2 = anotherVector;
		}	

	/// <summary>
	/// Gets the direction of polarisation vector 1, even if its amplitude is 0, in that case it will
	/// be the direction orthogonal to vector 2 and the trace direction such that vec1 = vec2 * dir
	/// </summary>
	/// <returns>The direction of the first component of the polarisation decomposition.</returns>
	public Scientrace.UnitVector getPolarisationDir1() {
		if (this.getPolarisationVec1().length == 0)
			return this.getPolarisationVec2().crossProduct(this.traceline.direction).tryToUnitVector();
		return this.getPolarisationVec1().tryToUnitVector();
		}

	/// <summary>
	/// Gets the direction of polarisation vector 2, even if its amplitude is 0, in that case it will
	/// be the direction orthogonal to vector 1 and the trace direction such that vec2 = dir * vec1
	/// </summary>
	/// <returns>The direction of the second component of the polarisation decomposition.</returns>
	public Scientrace.UnitVector getPolarisationDir2() {
		if (this.getPolarisationVec2().length == 0)
			return this.traceline.direction.crossProduct(this.getPolarisationVec1()).tryToUnitVector();
		return this.getPolarisationVec2().tryToUnitVector();
		}


	// return type changed from single Scientrace.Trace instance to List<Scientrace.Trace> newTraces in order to better facilitate the splitting of traces.
	public List<Scientrace.Trace> redirect_with_polarisation(Scientrace.Object3d toObject, Scientrace.Intersection intersection) {
		List<Scientrace.Trace> newTraces = new List<Trace>();
		//what objects are acting here?
		Scientrace.Object3d fromObject = this.currentObject;

		/* The "normal" vector should always point towards the incoming beam when
		 * using the Snellius-approach as described by figure 15 in 
		 * "Optical simulation of the SunCycle concentrator using Scientrace"
		 * by Joep Bos-Coenraad (2013) */
		Scientrace.UnitVector normal = intersection.enter.flatshape.plane.getNorm()
										.orientedAgainst(this.traceline.direction)
										.tryToUnitVector();

		DielectricSurfaceInteraction fsi = new DielectricSurfaceInteraction(this, intersection.enter.loc, normal, 
						fromObject.materialproperties.refractiveindex(this), toObject.materialproperties.refractiveindex(this), 
						toObject);
		
		newTraces.AddIfNotNull(fsi.getReflectTrace(this.getMinDistinctionLength()));
		newTraces.AddIfNotNull(fsi.getRefractTrace(this.getMinDistinctionLength()));
		return newTraces;
		}


}


} //end namespace Scientrace