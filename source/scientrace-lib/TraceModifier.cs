// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {
public class TraceModifier {
	public TraceModifier() {
	}

	public static int getBinarySelector(int flagcollection, int SELECTOR) {
		return ((flagcollection-(flagcollection%SELECTOR))%(SELECTOR*2));
		//=MOD(B$1-MOD(B$1,B2),B3)/B2		
		}
		
		
	public static bool hasSelector(int flagcollection, int SELECTOR) {
		return (TraceModifier.getBinarySelector(flagcollection, SELECTOR) == SELECTOR);
		//return (((flagcollection-(flagcollection%SELECTOR))%(SELECTOR*2))>0);
		//=MOD(B$1-MOD(B$1,B2),B3)/B2		
		}
		
		

	/// <summary>
	/// DUMMY for static function hasSelector
	/// </summary>
	/// <param name="a">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="b">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public bool hasSel(int flagcollection, int SELECTOR) {
		return TraceModifier.hasSelector(flagcollection,SELECTOR);
		}
	
}
}

