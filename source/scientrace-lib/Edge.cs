// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public class Edge {

	public Scientrace.Vector[] vectors = new Scientrace.Vector[2];
	
	public Edge(Vector startVec, Scientrace.Vector endVec) {
		this.start = startVec;
		this.end = endVec;
		}
		
	public Scientrace.Vector start {
		get { return vectors[0]; }
		set { vectors[0] = value; }
		}

	public Scientrace.Vector end {
		get { return vectors[1]; }
		set { vectors[1] = value; }
		}
		
	}}

