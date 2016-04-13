// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {
public abstract class TraceJournalExportableHistogram : TraceJournalExportable {

	/// <summary>
	/// The size of the bin. The total number of bins is 90/angle_histogram_resolution
	/// </summary>
	public double angle_histogram_resolution = 1.0;
	public double angle_histogram_from = 0;
	public double angle_histogram_to = 180;
	public string angle_histogram_csv_filename = "%t_%o.csv";


	public TraceJournalExportableHistogram(TraceJournal aTJ):base(aTJ) {
		}

	public string radiansToRoundedDegString(double radians_angle) {
		double angle_deg = radians_angle*180/Math.PI;
		double angle_deg_mod =((180+angle_deg)%360)-180;
		return toResString(angle_deg_mod);
		}

	public string toResString(double aDouble) {
		// This *1.00000000000001 thing is a quick&dirty hack to avoid floating point errors that otherwise occur.
		return (this.angle_histogram_resolution*
				Math.Floor(aDouble*1.000000000001/this.angle_histogram_resolution)).ToString();
		}
		
	public void addToOtherBin(double value, Dictionary<string, double> dict) {
		if (!dict.ContainsKey("other")) {
			dict.Add("other", value);
			return;
			}
		dict["other"] = dict["other"] + value;
		}

	public Dictionary<string, double> getHistogramTemplate() {
		//public void writeAngleHistogramCSV(Scientrace.PhysicalObject3d anObject) {
		Dictionary<string, double> angle_histogram = new Dictionary<string, double>();

		//Creating empty bins
		for (double bin = this.angle_histogram_from; bin < this.angle_histogram_to; bin = bin+this.angle_histogram_resolution) {
			//Console.WriteLine("NEWBIN: "+this.toResString(bin)+ " from:"+bin);
			angle_histogram.Add(this.toResString(bin), 0);
			}
		return angle_histogram;
		}

	public bool addToBin(Dictionary<string,double> hist, string bin, double add_value, bool use_other_bin=true) {
			if (hist.ContainsKey(bin)) {
				hist[bin] = hist[bin]+add_value;
				return true;
				} else
					if (use_other_bin)
						this.addToOtherBin(add_value, hist);
			return false;
			}

}}

