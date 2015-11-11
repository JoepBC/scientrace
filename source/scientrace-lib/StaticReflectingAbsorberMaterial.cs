// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

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


	}} //end class+namespace

