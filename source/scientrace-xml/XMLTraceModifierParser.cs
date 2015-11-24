using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ScientraceXMLParser {

	public class XMLTraceModifierParser : ScientraceXMLAbstractParser {
		
//	public XMLTraceModifierParser(XElement lightsourceXElement, CustomXMLDocumentOperations X): base(lightsourceXElement, X) {
	public XMLTraceModifierParser(): base() {
		}

	public List<Scientrace.UniformTraceModifier> getModifiers(XElement xel) {
		List<Scientrace.UniformTraceModifier> utms = new List<Scientrace.UniformTraceModifier>();
		foreach (XElement xutm in xel.Elements("TraceModifier")) {
			//Console.WriteLine("ADDING TRACE MODIFIER "+xutm.ToString());
			utms.Add(this.getUtm(xutm));
			}
		return utms;
		}


	public Scientrace.UniformTraceModifier getUtm(XElement xutm) {
		string sDispat = this.X.getXStringByAttribute(xutm, "DistributionPattern");
		Scientrace.DistributionPattern dispat = (Scientrace.DistributionPattern)Enum.Parse(typeof(Scientrace.DistributionPattern), sDispat);
		string sSpadis = this.X.getXStringByAttribute(xutm, "SpatialDistribution");
		Scientrace.SpatialDistribution spadis = (Scientrace.SpatialDistribution)Enum.Parse(typeof(Scientrace.SpatialDistribution), sSpadis);
		bool addself = this.X.getXBool(xutm, "AddSelf");
		int modifycount = this.X.getXInt(xutm, "ModifyCount", 1);
		int? randomseed = this.X.getXInt(xutm, "RandomSeed", -1);
		//cannot set default value "null" for double so use -1 workaround (which can also be used as an integer value in the XML)
		if (randomseed == -1) { randomseed = null; }
		double maxangle = this.X.getXAngleByName(xutm, "MaxAngle", Math.PI*0.25);
		
		
		Scientrace.UniformTraceModifier retutm = new Scientrace.UniformTraceModifier(dispat, spadis);

		retutm.add_self = addself;
		retutm.setMaxAngle(maxangle);
		Console.WriteLine("Maxangle read: "+maxangle+" result: "+retutm.maxangle);
		retutm.modify_traces_count = modifycount;
		retutm.randomSeed = randomseed;
		
		return retutm;
		}
				
		
	}}

