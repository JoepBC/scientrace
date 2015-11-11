using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ScientraceXMLParser {

	public class XMLEfficiencyCharacteristicsParser : ScientraceXMLAbstractParser {

		public XMLEfficiencyCharacteristicsParser():base(){
			}

		public Scientrace.OpticalEfficiencyCharacteristics parseEfficiencyForLightSource(XElement xLightSource) {
			Scientrace.OpticalEfficiencyCharacteristics oec = new Scientrace.OpticalEfficiencyCharacteristics();
			if (xLightSource.Element("IncidentEfficiency")!=null)
				this.parseIncidentEfficiency(oec, xLightSource.Element("IncidentEfficiency"));
			if (xLightSource.Element("QuantumEfficiency")!=null)
				this.parseQuantumEfficiency(oec, xLightSource.Element("QuantumEfficiency"));
			return oec;
			}

		public void parseQuantumEfficiency(Scientrace.OpticalEfficiencyCharacteristics oec, XElement xQuantumEfficiency) {
			foreach (XElement xwl in xQuantumEfficiency.Elements("Wavelength"))
				oec.addWavelength(this.X.getXDouble(xwl, "m", this.X.getXDouble(xwl, "nm")*1E-9), this.X.getXDouble(xwl, "Fraction"));
			}

		public void parseIncidentEfficiency(Scientrace.OpticalEfficiencyCharacteristics oec, XElement xIncidentEfficiency) {
			foreach (XElement xangle in xIncidentEfficiency.Elements("Angle"))
				oec.addAngle(this.X.getXAngle(xangle), this.X.getXDouble(xangle, "Fraction"));
			}

	}}
