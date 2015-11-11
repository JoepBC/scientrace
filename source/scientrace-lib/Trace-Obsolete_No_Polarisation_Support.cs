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
///  These are the methods that were previously used to calculate/evaluate the surface interaction.
/// </summary>
public partial class Trace {

		

	// return type changed from single Scientrace.Trace instance to List<Scientrace.Trace> newTraces in order to better facilitate the splitting of traces.
	public List<Scientrace.Trace> redirect_without_polarisation(Scientrace.Object3d toObject, Scientrace.Intersection intersection) {
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


		
		//Does full internal reflection occur?
		if (this.fullInternalReflects(normal, fromObject, toObject)) {
			//construct reflectedTrace
			Trace internalReflectedTrace = this.createFullInternalReflectedTraceFork(intersection);
			newTraces.Add(internalReflectedTrace);
			// in case of full internal reflection, no other "effects" occur, so return the newTraces collection here.
			return newTraces;
			}
			
		newTraces.AddRange(this.partialReflectRefract(fromObject, toObject, intersection, normal));
		
		// HERE USED TO BE CODE THAT CHECKED WHETHER (based on alphadirection) the direction of the incoming beam was altered at all.

		return newTraces;
		} // end func. redirect




	public List<Scientrace.Trace> partialReflectRefract(Scientrace.Object3d fromObject3d, Scientrace.Object3d toObject3d, 
														Scientrace.Intersection intersection, UnitVector surfaceNormal) {
		/*double oldrefindex = fromObject3d.materialproperties.refractiveindex(this);
		double newrefindex = toObject3d.materialproperties.refractiveindex(this);*/
		List<Scientrace.Trace> newTraces = new List<Trace>();
		Scientrace.Trace refractTrace = this.fork("_pr");
			
		refractTrace.currentObject = toObject3d;
		refractTrace.traceline.startingpoint = intersection.enter.loc;

		Scientrace.UnitVector normal = intersection.enter.flatshape.plane.getNorm()
										.orientedAgainst(this.traceline.direction)
										.tryToUnitVector();
		// Evaluate absorption by toObject3d	
		refractTrace.absorpByObject(toObject3d, normal, fromObject3d);
		if (refractTrace.isEmpty()) {
			newTraces.Add(refractTrace);
			return newTraces;
			}	

		// CHECK whether (partial) reflection occurs...
		if ((toObject3d.materialproperties.reflects) || ((intersection.leaving) && (fromObject3d.materialproperties.reflects))) {
			//double refcoef = toObject3d.materialproperties.reflection(refractTrace, surfaceNormal, this.currentObject);
			double refcoef = toObject3d.materialproperties.reflection(refractTrace, surfaceNormal, fromObject3d.materialproperties);
			
			Scientrace.Trace reflectTrace = refractTrace.fork("_R"); //.clone();	reflectrace.nodecount++;
			reflectTrace.intensity = refractTrace.intensity*refcoef;
			//intensity shouldn't spawn nor disappear here
			refractTrace.intensity = refractTrace.intensity-reflectTrace.intensity;
			//Scientrace.Spot normspot = new Scientrace.Spot(norm.toLocation()+intersection.enter.loc, null, 1);
			
			//CHANGED 20131030
			//reflectrace.traceline = newtrace.reflectAt(intersection.enter.flatshape.plane);
			reflectTrace.traceline = refractTrace.reflectLineAbout(reflectTrace.traceline, surfaceNormal);
	
			//ADDED @ 20130122: the parial internal reflected trace should have the current object as oldobject.
			reflectTrace.currentObject = this.currentObject; //CURRENT OBJECT DOES NOT CHANGE FOR (partial)INTERNAL REFLECTION!
			newTraces.Add(reflectTrace);
			}
			
		this.initCreatedRefractTrace(refractTrace, surfaceNormal, fromObject3d, toObject3d);
		newTraces.Add(refractTrace);
		return newTraces;
		} // end func partialReflectRefract
		



	public void initCreatedRefractTrace(Trace refractTrace, UnitVector surfaceNormal, Scientrace.Object3d fromObject3d, Scientrace.Object3d toObject3d)	{
		double oldrefindex = fromObject3d.materialproperties.refractiveindex(this);
		double newrefindex = toObject3d.materialproperties.refractiveindex(this);
		//for definitions on the parameters below, check fullInternalReflects function.

		UnitVector incoming_trace_direction = this.traceline.direction;
		if (incoming_trace_direction.dotProduct(surfaceNormal) > 0) {
			surfaceNormal = surfaceNormal.negative();
			}

		Scientrace.UnitVector nnorm = surfaceNormal.negative();
		Scientrace.Vector incoming_normal_projection = nnorm*(incoming_trace_direction.dotProduct(nnorm));
		Vector L1 = incoming_trace_direction-incoming_normal_projection;
		double L2 = (oldrefindex/newrefindex)*L1.length;

		if (incoming_trace_direction == incoming_normal_projection) {
			//in case of normal incident light: do not refract.
			refractTrace.traceline.direction = incoming_trace_direction;
			return;
			}

		try {
			refractTrace.traceline.direction = ((nnorm*Math.Sqrt(1 - Math.Pow(L2,2)))
					+(L1.tryToUnitVector()*L2)).tryToUnitVector();
			} catch (ZeroNonzeroVectorException) {
				Console.WriteLine("WARNING: cannot define direction for refraction trace. Using surface normal instead. (L1: "+incoming_trace_direction.trico()+", L2:"+incoming_normal_projection.trico()+").");
				refractTrace.traceline.direction = nnorm;
			} //end try/catch
		}
					



	/// <summary>
	/// Fulls the internal reflects.
	/// </summary>
	/// <returns>
	/// True if full internal reflection occurs. Returns false in case of refraction and partial reflection
	/// </returns>
	/// <param name='incoming_trace_direction'>
	/// incoming_trace_direction is the vector of the trace passing the surface 
	/// </param>
	public bool fullInternalReflects(UnitVector surfaceNormal, Scientrace.Object3d fromObject3d, Scientrace.Object3d toObject3d)	{
		double oldrefindex = fromObject3d.materialproperties.refractiveindex(this);
		double newrefindex = toObject3d.materialproperties.refractiveindex(this);
	
		UnitVector incoming_trace_direction = this.traceline.direction;
		// make sure the surfaceNormal is directed towards the incoming beam (so in the opposite direction)
		if (incoming_trace_direction.dotProduct(surfaceNormal) > 0) {
			surfaceNormal = surfaceNormal.negative();
			}
		// nnorm is surface normale directed into the *new* surface, so *along* the incoming beam	
		Scientrace.UnitVector nnorm = surfaceNormal.negative();
		// incoming_normal_projection is the projection of incoming_trace_direction at the (n)norm		
		Scientrace.Vector incoming_normal_projection = nnorm*(incoming_trace_direction.dotProduct(nnorm));
		// L1 and L2 are defined as on figure 15 / page 20 from the "Optical simulation of the SunCycle concentrator using Scientrace (J. Bos-Coenraad 2013)
		Vector L1 = incoming_trace_direction-incoming_normal_projection;
		double L2 = (oldrefindex/newrefindex)*L1.length;
		// If L2 is larger than 1, full internal reflection takes place.
		return (L2 > 1);
		}




	public Scientrace.Trace createFullInternalReflectedTraceFork(Scientrace.Intersection intersection) {
		Scientrace.Trace fullReflectTrace = this.fork("_fR");
		fullReflectTrace.traceline = fullReflectTrace.reflectAt(intersection.enter.flatshape.plane);
		fullReflectTrace.traceid = fullReflectTrace.traceid+"r";
		fullReflectTrace.traceline.startingpoint = intersection.enter.loc;
		fullReflectTrace.nodecount++;
		//Console.WriteLine("INTERNAL REFLECTION! direction = "+newtrace.traceline.direction.trico()+this.currentObject.GetType());
		return fullReflectTrace;				
		}



}} //end namespace Scientrace + partial class Trace
