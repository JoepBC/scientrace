// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class ZeroNonzeroVectorException : Exception {

	public ZeroNonzeroVectorException (string str) : base ("A unitvector with length 0 has been assigned and cannot be normalized. "+str) {
		//Console.WriteLine ("A unitvector with length 0 has been assigned and cannot be normalized. "+str);
	}
}
}
