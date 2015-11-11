// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {
public class OpticalEfficiencyCharacteristics {

	public SortedList<double, double> angular_efficiency = new SortedList<double, double>();
	public SortedList<double, double> quantum_efficiency = new SortedList<double, double>();

	public OpticalEfficiencyCharacteristics() {
		}

	public void addAngle(double angle_in_radians, double fraction) {
		this.angular_efficiency.Add(angle_in_radians, fraction);
		}

	public void addWavelength(double wavelengt_in_meters, double fraction) {
		this.angular_efficiency.Add(wavelengt_in_meters, fraction);
		}

	public void setDefaultTables() {
		this.setDefaultAngleList();
		this.setDefaultSpectralList();
		}

	public void setDefaultAngleList() {
		this.angular_efficiency.Add(0, 1);
		this.angular_efficiency.Add(Math.PI, 1);
		}

	public void setDefaultSpectralList() {
		this.quantum_efficiency.Add(400E-9, 1);
		this.quantum_efficiency.Add(1800E-9, 1);
		}

	public double getLowestValue(SortedList<double,double> aSortedList) {
		return aSortedList.Values[0];
		}
	public double getLowestKey(SortedList<double,double> aSortedList) {
		return aSortedList.Keys[0];
		}
	public double getHighestValue(SortedList<double,double> aSortedList) {
		return aSortedList.Values[aSortedList.Count-1];
		}
	public double getHighestKey(SortedList<double,double> aSortedList) {
		return aSortedList.Keys[aSortedList.Count-1];
		}

	public void getBeforeAndAfterKeys(SortedList<double, double> aSortedList, double pseudoKey, ref double before, ref double after) {
		double prevkey = 0;
		bool prevset = false;
		foreach (double key in aSortedList.Keys) {
			if (key > pseudoKey) {
				if (!prevset) // is the "before key" set?
					throw new ArgumentOutOfRangeException("Pseudokey {"+pseudoKey+"} is smaller than the first key {"+key+"}, therefor no surrounding keys exist.");
				before = prevkey;
				after = key;
				return;
				}
			prevkey = key;
			prevset = true;
			}
		throw new ArgumentOutOfRangeException("Pseudokey {"+pseudoKey+"} is larger than the last key {"+prevkey+"}, therefor no surrounding keys exist.");
		}

	public List<double> getBeforeAndAfterKeys(SortedList<double, double> aSortedList, double pseudoKey) {
		double before = 0;
		double after = 0;
		this.getBeforeAndAfterKeys(aSortedList, pseudoKey, ref before, ref after);
		return new List<double>{before, after};
		}

	public double interpolatePseudovalue(SortedList<double,double> aSortedList, double pseudoKey) {
		if (pseudoKey < this.getLowestKey(aSortedList))
			return this.getLowestValue(aSortedList);
		if (pseudoKey > this.getHighestKey(aSortedList))
			return this.getHighestValue(aSortedList);
		double before = 0;
		double after = 0;
		this.getBeforeAndAfterKeys(aSortedList, pseudoKey, ref before, ref after);
		double diff = after-before;
		double fraction = (pseudoKey - before) / diff;
		return aSortedList[before] + ((aSortedList[after]-aSortedList[before])*fraction);
		}

	public double getEffForAngle(double anAngle) {
		if (this.angular_efficiency.Count<1) {
			Console.WriteLine("WARNING: no angular efficiency table set, creating 100% dummy instead.");
			this.setDefaultAngleList();
			}
		return this.interpolatePseudovalue(this.angular_efficiency, anAngle);
		}

	public double getEffForWavelength(double wavelength) {
		if (wavelength > 10E-6)
			Console.WriteLine("WARNING, wavelength of "+wavelength+" is not in the default nanometers regime. Might there be an E-9 missing somewhere?");
			if (this.quantum_efficiency.Count<1) {
			Console.WriteLine("WARNING: no spectral efficiency table set, creating 100% dummy instead.");
			this.setDefaultSpectralList();
			}
			return this.interpolatePseudovalue(this.quantum_efficiency, wavelength);
		}

	}} //end of class + workspace

