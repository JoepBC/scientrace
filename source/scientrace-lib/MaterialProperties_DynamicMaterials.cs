// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;
using org.mariuszgromada.math.mxparser;

namespace Scientrace {
public class StaticReflectingAbsorberMaterial : StaticNTransparentMaterial {

	//default = perfect mirror properties
	public double reflection_fraction = 1;
	public double absorption_fraction = 0;

	public StaticReflectingAbsorberMaterial() : base(1) {
		this.dominantOnLeave = true;
		this.dielectric = false;
		}

		
	public StaticReflectingAbsorberMaterial(double reflectionFraction, double absorptionFraction) : base(1) {
		this.init(reflectionFraction, absorptionFraction);
		}

	public override string identifier() {
		return "reflectingabsorber";
		}

	public void init(double reflectionFraction, double absorptionFraction) {
		// is this possible at all?
		this.setReflectionFraction(reflectionFraction);
		this.setAbsorptionFraction(absorptionFraction);
		}


	public override double enterAbsorption (Trace trace, Scientrace.UnitVector norm, Scientrace.MaterialProperties previousMaterial)	{
		return this.absorption_fraction;
		}

	public override double enterReflection(Trace trace, Scientrace.UnitVector norm, Scientrace.MaterialProperties previousMaterial)	{
		//Console.WriteLine("New: "+this.reflection_fraction);
		return this.reflection_fraction;
		}


	public void setReflectionFraction(double reflectionFraction) {
		if (reflectionFraction > 1 || reflectionFraction < 0)
			throw new ArgumentOutOfRangeException("The reflection {"+reflectionFraction+"} must be in the range of [0, 1].");
		this.reflection_fraction = reflectionFraction;
		}

	public void setAbsorptionFraction(double absorptionFraction) {
		if (absorptionFraction > 1 || absorptionFraction < 0)
			throw new ArgumentOutOfRangeException("The absorption {"+absorptionFraction+"} must be in the range of [0, 1].");
		this.absorption_fraction = absorptionFraction;
		}


	} //end StaticReflectingAbsorberMaterial



public class DispersionFormulaDielectricProperties : Scientrace.MaterialProperties {
	//Singleton instance "holder"
	private static DispersionFormulaDielectricProperties instance;
	public Dictionary<double,double> refindices = new Dictionary<double,double>();

	public string user_formula="1.5";

	public bool was_warned_for_x = false;

	public DispersionFormulaDielectricProperties() {
		this.reflects = true;
		this.dielectric = true;
		}

	public void set_user_formula(string a_formula) {
		// The only operation with an "x" in it is called "exp", it is temp. renamed to "EXP" to be replaced back at the end of this method.
		this.user_formula = a_formula.Replace("exp", "EXP");
		if (!this.was_warned_for_x && this.user_formula.Contains("x")) {
			Console.WriteLine("WARNING: an 'x' character was found in the refractive index definition, assuming wavelength in micrometers.");
			this.was_warned_for_x = true;
			this.user_formula = this.user_formula.Replace("x", "um");
			}
		// Cleaning up the renaming operation
		this.user_formula = this.user_formula.Replace("EXP", "exp");
		}

	public override string identifier() {
		return "dispersionformula";
		}

	public static DispersionFormulaDielectricProperties Instance {
		get {
			//lazy init
			if (DispersionFormulaDielectricProperties.instance == null) {
				DispersionFormulaDielectricProperties.instance = new DispersionFormulaDielectricProperties();
			}
			return DispersionFormulaDielectricProperties.instance;
		}
	}


	public static string matlab_to_mxparser_string(string aMatlabString) {
		return aMatlabString
				.Replace(".^", "^")
				.Replace("./", "/")
				.Replace(".*", "*")
				;
		}

	public override double refractiveindex(double wavelength) {
		// Using cached results for increased performance
		if (this.refindices.ContainsKey(wavelength))
			return this.refindices[wavelength];

		string wlstring = wavelength.ToString("F99").TrimEnd("0".ToCharArray());

		string formula = matlab_to_mxparser_string(this.user_formula.Replace("nm", "(("+wlstring+")*1000000000)")
											.Replace("um", "(("+wlstring+")*1000000)")
											.Replace("mm", "(("+wlstring+")*1000)")
											.Replace("m", wlstring));
/*
		string formula = this.user_formula.Replace("nm", "(("+wavelength.ToString()+")*1E9)")
											.Replace("um", "(("+wavelength.ToString()+")*1E6)")
											.Replace("mm", "(("+wavelength.ToString()+")*1E3)")
											.Replace("m", wavelength.ToString());*/

		org.mariuszgromada.math.mxparser.Expression refindex = new org.mariuszgromada.math.mxparser.Expression(formula.ToString());
//		org.mariuszgromada.math.mxparser.Expression refindex = new org.mariuszgromada.math.mxparser.Expression(formula);

        double result = refindex.calculate();
		//Console.WriteLine("Evaluated refindex for "+this.user_formula+" after replace: "+formula+" wl: "+(wavelength*1e9)+" = "+result.ToString());
		this.refindices[wavelength] = result;
		return result;
		}
	}	//end of class DispersionFormulaDielectricProperties



public class StaticNTransparentMaterial : Scientrace.MaterialProperties {

	public override string identifier() {
		return "staticn";
		}
		
		
	private double refindex;

	public StaticNTransparentMaterial(double refindex) {
		this.reflects = true;
		this.refindex = refindex;
		}

	public void setRefractiveIndex(double ref_index) {
		this.refindex = ref_index;
		}

	public override double refractiveindex(double wavelength) {
		return this.refindex;
		}

	}//end StaticNTransparentMaterial

} //end of namespace
