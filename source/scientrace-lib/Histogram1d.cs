// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {
public class Histogram1d : TraceJournalExportableHistogram {


	public Histogram1d(TraceJournal aTJ):base(aTJ) {
		this.tag="histogram";
		}



	public override void write(Scientrace.PhysicalObject3d anObject) {
		Dictionary<string, double> angle_histogram = this.getHistogramTemplate();

		foreach(Scientrace.Spot casualty in this.tj.spots) {
			//only count casualties for current object.
			if (casualty.object3d != anObject) continue;

			if (casualty == null) {
				Console.WriteLine("Error: Casualty is null when writing angle histogram for {"+anObject.ToString()+"}...");
				continue;
				}

			//The angle on which the histogram bin is based.
			double angle_rad = anObject.getSurfaceNormal().negative().angleWith(casualty.trace.traceline.direction);
		
			//Moved to new method: radiansToRoundedDegString
			/*double angle_deg = angle_rad*180/Math.PI;
			double angle_deg_mod =((180+angle_deg)%360)-180;
			string bin = this.toResString(angle_deg_mod);*/
			string bin = this.radiansToRoundedDegString(angle_rad);

			double reweigh_factor = (this.lightsource_weigh_intensity) 
				? (casualty.trace.lightsource.weighted_intensity/casualty.trace.lightsource.total_lightsource_intensity)
				: 1;
			this.addToBin(angle_histogram, bin, casualty.intensity*reweigh_factor, true);
			/*
			//Console.WriteLine("bin: "+bin+", angledegmod:"+angle_deg_mod+", hist_res:"+this.angle_histogram_resolution);
			if (angle_histogram.ContainsKey(bin)) //{
				angle_histogram[bin] = angle_histogram[bin]+casualty.intensity;
				//Console.WriteLine("bin {"+bin+"} increased."); }
				else {
				this.addToOtherBin(casualty.intensity, angle_histogram);
				//Console.WriteLine("WARNING: BIN {"+bin+"} NOT FOUND FOR HISTOGRAM."+angle_deg_mod+"/"+angle_deg);\
				} */
			}

		string angle_histogram_csv_filename = this.tj.exportpath+this.angle_histogram_csv_filename.Replace("%o",anObject.tag);
		this.appendWriteWithConfigVariables(angle_histogram_csv_filename, angle_histogram);
		}



}}

