// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class ParallelVectorException : Exception {

	public ParallelVectorException (Scientrace.Vector vec1, Scientrace.Vector vec2, string str) {
		Console.WriteLine("Two vectors ("+vec1.trico()+" / "+vec2.trico()+") are parallel which is not allowed. "+str);
		}
		
	}}
