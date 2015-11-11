// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {
public class NumDistribute {
		
	/// <summary>
	/// Constructor method
	/// </summary>
	public NumDistribute() {
	}
		
	/// <summary>
	/// method based on the Box Müller algorithm as described in Ronald Kleiss 
	/// reader on Monte Carlo techniques: http://www.hef.ru.nl/~kleiss/mcnotes.pdf
	/// "x" values are multiplied by sqrt(2) to get a stddev = 1 (to be multiplied
	/// by the given stddev.
	/// </summary>
	/// <param name="stddev">
	/// The standard deviation to base the variation upon.
	/// A <see cref="System.Double"/>
	/// </param>
	/// <param name="rnd">
	/// A <see cref="Random"/> to use for (e.g.) seeded randomized use.
	/// </param>
	/// <returns>
	/// A <see cref="System.Double"/> value that is varied about "0" with stddev as given.
	/// </returns>
	public static double getNormDistrVal(double stddev, Random rnd) {
		double p1 = rnd.NextDouble();
		double p2 = rnd.NextDouble();
		double t = -Math.Log(p1);
		double f = Math.PI*2*p2;
		return Math.Cos(f)*Math.Sqrt(t)*Math.Sqrt(2)*stddev;			
		}

	public static double getNormDistrAboutVal(double stddev, double mean, Random rnd) {
		return NumDistribute.getNormDistrVal(stddev,rnd)+mean;
		}

	public static double getNormallyDistributedValue(double stddev) {
		Random rnd = new Random();
		return NumDistribute.getNormDistrVal(stddev,rnd);
		}

	public static double getNormallyDistributedAboutValue(double stddev, double mean) {
		Random rnd = new Random();
		return NumDistribute.getNormDistrAboutVal(stddev,mean, rnd);
		}
		
}
}

