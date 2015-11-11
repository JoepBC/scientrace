// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public class QuadraticEquation {

	public double a,b,c;
	public int answerCount;
	public bool hasAnswers;
	
	public double plusVal;
	public double minVal;
	
	public double discriminant;

	public QuadraticEquation(double aQuadratic, double bLinear, double cConstant) {
		this.a=aQuadratic;
		this.b=bLinear;
		this.c=cConstant;
		
		this.calcValues();
		}

	/// <summary>
	/// Gets one of the answers for the Quadratic Equation.
	/// </summary>
	/// <returns>
	/// An answer to the Quadratic Equation.
	/// </returns>
	/// <param name='answerNumber1or2'>
	/// 1 will return the + value in the Quadratic Equation, 2 will return the - value. All other values throw an ArgumentOutOfRangeException.
	/// </param>
	public double getAnswer(int answerNumber1or2) {
		switch (answerNumber1or2) {
			case 1: return this.plusVal;
			case 2: return this.minVal;
			default: throw new ArgumentOutOfRangeException("Cannot return answer "+answerNumber1or2+" for QuadraticEquation instance. Must be 1 (plus) or 2 (min).");
			}}

	public bool calcValues() {
		this.setDiscriminant();
		
		switch (this.answerCount) {
			case 0: return false;  // return false if quadratic formula has no answers
			case 1: 
				this.plusVal = -b/(2*a);
				return true;
			case 2:
				double sqrtDisc = Math.Sqrt(discriminant);
				this.plusVal = (-b + sqrtDisc) / (2*a);
				this.minVal = (-b - sqrtDisc) / (2*a);
				return true;
			default:
				throw new ArgumentOutOfRangeException("Quadratic Equation answerCount ("+this.answerCount+") != 0, 1 or 2");
				}
		}
		
	/// <summary>
	/// Sets the discriminant and the answerCount fields
	/// </summary>
	/// <returns>
	/// Whether the quadratic formula has answers or not.
	/// </returns>
	public void setDiscriminant() {
		this.discriminant = (b*b) - (4*a*c);
		if (this.discriminant < 0) {
			this.answerCount = 0;
			this.hasAnswers = false;
			} else {
			this.hasAnswers = true;
			if (this.discriminant == 0) {
				this.answerCount = 1;
				} else {
				this.answerCount = 2;
				}}
		} //end void setDiscriminant
						
		
	}
}

