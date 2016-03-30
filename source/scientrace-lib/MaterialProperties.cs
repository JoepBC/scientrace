// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections;

//namespacing used for retrieving calling method...
using System.Diagnostics;
using System.Reflection;

namespace Scientrace {


public abstract class MaterialProperties {

	public bool reflects = false;
	public bool dielectric = true;

	//a metarial that is "dominant on leave" will prescribe the reflection and absorption when the trace leaves the object.
	public bool dominantOnLeave = false;

	public MaterialProperties() {
	}

	public abstract double refractiveindex(double wavelength);

	public abstract string identifier();		


/************* STATIC PROPERTIES AND METHODS ***********************/

	private static Hashtable materials = new Hashtable();

	public static MaterialProperties FromIdentifier(string identifier) {
		if (MaterialProperties.materials.Count < 1) { MaterialProperties.buildMaterialList(); }
		Scientrace.MaterialProperties retmat = MaterialProperties.getMaterial(identifier);
		if (retmat==null) {
			throw new ArgumentException("MaterialProperties Identifier \""+identifier+"\" is unknown.");
			}
		return retmat;
		}

	public static bool listedIdentifier(string identifier) {
		if (MaterialProperties.materials.Count < 1) { MaterialProperties.buildMaterialList(); }
		return (MaterialProperties.materials.ContainsKey(identifier));
		}
		
	public static void buildMaterialList( ) {
		//For backwards compatibility:
		MaterialProperties.materials.Add("suncyclepmma", AltPMMAProperties.Instance);

		MaterialProperties.addMaterial(PMMAProperties.Instance);
		MaterialProperties.addMaterial(AltPMMAProperties.Instance);
		MaterialProperties.addMaterial(BlackBodySurface.Instance);
		MaterialProperties.addMaterial(AirProperties.Instance);
		MaterialProperties.addMaterial(PerfectMirror.Instance);
		MaterialProperties.addMaterial(GaAsSurface.Instance);
		MaterialProperties.addMaterial(SodaLimeGlassProperties.Instance);
		MaterialProperties.addMaterial(SchottBK7Properties.Instance);
		//MaterialProperties.materialstablebuilt = true;
		}
	
	public static void addMaterial(MaterialProperties mp) {
		MaterialProperties.materials.Add((mp.identifier()).ToLower(), mp);
		}

	/* call FromIdentifier function instead which includes lazy loading functionality */
	private static MaterialProperties getMaterial(string identifier) {
		return (MaterialProperties)MaterialProperties.materials[identifier.ToLower()];
		}


/******************** INSTANCE METHODS *****************/


	public virtual double absorption(Scientrace.Trace trace, Scientrace.UnitVector norm, MaterialProperties previousMaterial) {
		if (previousMaterial.dominantOnLeave)
			return (previousMaterial.enterAbsorption(trace, norm, this));
		return this.enterAbsorption(trace, norm, previousMaterial);		
		}

	//materials can transmit, reflect and absorp light. 
	public virtual double enterAbsorption(Scientrace.Trace trace, Scientrace.UnitVector norm, MaterialProperties previousMaterial) {
		return 0;
		}

	public virtual double getLeaveAbsorption(Scientrace.Trace trace, Scientrace.UnitVector norm, MaterialProperties nextMaterial) {
		throw new NotImplementedException("getLeaveAbsorption is not implemented for "+this.GetType().ToString());
		}





	public double reflection (Scientrace.Trace trace, Scientrace.UnitVector norm, MaterialProperties previousMaterial) {
		if (previousMaterial.dominantOnLeave)
			return (previousMaterial.enterReflection(trace, norm, this));
		return this.enterReflection(trace, norm, previousMaterial);
		}

	public virtual double enterReflection (Scientrace.Trace trace, Scientrace.UnitVector norm, MaterialProperties previousMaterial) {
		double ti = trace.traceline.direction.angleWith(norm.negative());
		double sti = Math.Sin(ti);
		double cti = Math.Cos(ti);
		/*debug		Console.WriteLine("reflecting prev.: "+ previousObject.GetType());	Console.WriteLine("reflecting prev. material: "+ previousObject.materialproperties.GetType());*/
		double n1 = previousMaterial.refractiveindex(trace);
		double n2 = this.refractiveindex(trace);
		//Fresnel Equations on a 50/50 distribution of parallel and perpendicular to the plane polarization
		double Rs = Math.Pow(
				    ( (n1*cti) - (n2*Math.Sqrt(1.0 - Math.Pow((n1/n2)*sti,2.0))) ) /
			        ( (n1*cti) + (n2*Math.Sqrt(1.0 - Math.Pow((n1/n2)*sti,2.0))) )
			        ,2.0);
		double Rp = Math.Pow(
					( (n1*Math.Sqrt(1.0-Math.Pow((n1/n2)*sti,2.0))) - (n2*cti) ) /
					( (n1*Math.Sqrt(1.0-Math.Pow((n1/n2)*sti,2.0))) + (n2*cti) )
					,2.0);
		double R = (Rs+Rp)/2.0;
		//		Console.WriteLine("Reflection (n1/n2) = ("+n1+"/"+n2+") for angle "+ti+" is ("+Rs+"+"+Rp+")/2 ="+R);
		//		Console.WriteLine("Reflection "+trace.traceline.direction.trico()+"->"+norm.trico()+" for angle "+ti+" is ("+Rs+"+"+Rp+")/2 ="+R);
		if (!((R>=0) && (R<1))) {
				Console.WriteLine("R==null("+R+", Rs:"+Rs+" Rp:"+Rp+" n1:"+n1+" n2:"+n2+" ti:"+ti+" rpnoem:"+
								((n1/n2)*sti)+") where "+this.ToString()+ " and "+previousMaterial.ToString());
			}


		/*StackTrace stackTrace = new StackTrace();
		MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
		Console.WriteLine(methodBase.Name); // e.g. */
		return R;
	}


	public double refractiveindex(Scientrace.Trace trace) {
		return this.refractiveindex(trace.wavelenght);
		}
}
}
