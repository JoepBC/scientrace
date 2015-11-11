// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using NUnit.Framework;
namespace ScientraceTesting {
[TestFixture()]
public class TraceModifierTest {
		
		
	[Test()]
	public void TestSelector() {
		Assert.IsFalse(Scientrace.TraceModifier.hasSelector(6,1));
		Assert.IsTrue(Scientrace.TraceModifier.hasSelector(6,2));
		Assert.IsTrue(Scientrace.TraceModifier.hasSelector(6,4));
		Assert.IsFalse(Scientrace.TraceModifier.hasSelector(6,8));
		Assert.IsFalse(Scientrace.TraceModifier.hasSelector(6,16));
		Assert.IsFalse(Scientrace.TraceModifier.hasSelector(6,32));
	}
}
}

