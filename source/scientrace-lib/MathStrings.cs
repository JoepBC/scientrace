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
public class MathStrings {

	public static string matlabToMxParserString(string aMatlabString) {
		return aMatlabString
				.Replace(".^", "^")
				.Replace("./", "/")
				.Replace(".*", "*")
				;
		}



	public static double solveString(string anMxParserString, Dictionary<string,object> vars) {
		string string_to_solve = anMxParserString;
		foreach (string key in vars.Keys) {
			string_to_solve = string_to_solve.Replace(key, vars[key].ToString());
			}
		return MathStrings.solveString(string_to_solve);
		}

	public static double solveString(string anMxParserString) {
		org.mariuszgromada.math.mxparser.Expression expr = new org.mariuszgromada.math.mxparser.Expression(anMxParserString);
        double result = expr.calculate();
		if (double.IsNaN(result)) {
			//throw new Exception(expr.getErrorMessage());
			
			throw new Exception("Couldn't calculate: {\n"+anMxParserString+"\n}, mxparser error: \n---\n"+expr.getErrorMessage()+"---\n" );
			}
		return result;
		}
}}

