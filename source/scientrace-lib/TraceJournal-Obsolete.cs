// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Scientrace {


/// <summary>
/// These methods are no longer in use.
/// </summary>
public class TraceJournal_Obsolete : TraceJournal {




	/********* CONSIDERING DistributionSquares ******************/

	/* An array of object3d instances resulting in distributions, e.g. solarcells and other surfaces */
	public ArrayList registeredDistributionObjects = new ArrayList();
		
		
	public DistributionSquare getDistributionSquare(Scientrace.PhysicalObject3d anObject, int order) {
		Scientrace.Parallelogram surface = anObject.getDistributionSurface();
		Scientrace.DistributionSquare ds = new Scientrace.DistributionSquare(0, order, surface.plane.loc, surface.plane.u, surface.plane.v);
		foreach(Scientrace.Spot aSpot in this.spots) {
			ds.addSpot(aSpot.loc);
			}
		return ds;
		}

	public void formerPartOfExportAll(Scientrace.Object3dEnvironment env) {
			//Calculate data for surface distribution
			foreach (Scientrace.PhysicalObject3d anObject in this.registeredDistributionObjects) {
				for (int iorder = 1; iorder <= 3; iorder++) {
					Scientrace.DistributionSquare ds = this.getDistributionSquare(anObject, iorder);
					double ratio = ds.totalDistribution();
					//Console.WriteLine("Ratio "+iorder+"th order "+anObject.tag+":"+ratio);
					TraceJournal.Instance.addExportField("dratio"+anObject.tag+":n="+iorder.ToString(), ratio.ToString());
					}
/*				using (writestream = new StreamWriter(path+anObject.tag+".xml")) {
					Console.WriteLine("MAKE XML HERE");
					//writestream.Write(ds.exportXML());
				}*/
				}
		}

	public void registerDistributionObject(Scientrace.Object3d anObject) {
		this.registeredDistributionObjects.Add(anObject);
		}

	/********* CONSIDERING DistributionSquares ******************/

		//TODO: remove methode below
		public void old_writeYieldCSV() {
			List<string> header = new List<string>();
			List<string> body = new List<string>();
			
			//CONFIG ID CONTAINS SEVERAL DATAFIELDS:
			string[] cids = this.config_id.Split('%');
			string[] cdescs = this.config_description.Split('%');
			for (int icids = 0; icids < cids.Length; icids++) {
				//writing headers
				header.Add(this.stringToCsv(cdescs[icids]));
				body.Add(this.stringToCsv(cids[icids]));
				/*xheader.Add(new XElement("td", cdescs[icids]));
				xrow.Add(new XElement("td", cids[icids]));*/
				}
			header.Add("TimeStamp");
			body.Add(this.stringToCsv(this.timestamp));

			double total_intensity = 0;
			double total_shine_duration = 0;
			foreach (Scientrace.LightSource lightsource in this.registeredLightSources) {
				header.Add(this.stringToCsv("lightsource"));
				body.Add(this.stringToCsv(lightsource.tag));
				/* header.Add(this.stringToCsv("direction X"));
				body.Add(this.doubleToCsv(lightsource.direction.x));
				header.Add(this.stringToCsv("Y"));
				body.Add(this.doubleToCsv(lightsource.direction.y));
				header.Add(this.stringToCsv("Z"));
				body.Add(this.doubleToCsv(lightsource.direction.z)); */
				header.Add(this.stringToCsv("calculation time (s)"));
				body.Add(this.doubleToCsv(lightsource.shine_duration.TotalSeconds));
				header.Add(this.stringToCsv("number of traces"));
				body.Add(this.intToCsv(lightsource.traceCount()));
				header.Add(this.stringToCsv("intensity"));
				body.Add(this.doubleToCsv(lightsource.total_lightsource_intensity));
				header.Add(this.stringToCsv("weighted_intensity"));
				body.Add(this.doubleToCsv(lightsource.weighted_intensity));

				total_intensity = total_intensity + lightsource.total_lightsource_intensity;
				total_shine_duration = total_shine_duration + lightsource.shine_duration.TotalSeconds;
				}

			double total_revenue = 0;
			foreach (Scientrace.PhysicalObject3d solarcell in this.registeredPerformanceObjects) {
				header.Add(this.stringToCsv("solarcell"));
				body.Add(this.stringToCsv(solarcell.tag));
				
				foreach (Scientrace.LightSource lightsource in this.registeredLightSources) {
					header.Add(this.stringToCsv(lightsource.tag+"-yield"));
					body.Add(this.doubleToCsv(lightsource.revenueForObject(solarcell)/lightsource.total_lightsource_intensity));
					header.Add(this.stringToCsv(lightsource.tag+"-weighted_sum"));
					body.Add(this.doubleToCsv((lightsource.revenueForObject(solarcell)/lightsource.total_lightsource_intensity)*lightsource.weighted_intensity));
					}

				header.Add(this.stringToCsv("total_revenue"));
				body.Add(this.doubleToCsv(solarcell.getTotalRevenue()));
				header.Add(this.stringToCsv(solarcell.tag+"-yield"));
				body.Add(this.doubleToCsv(solarcell.getTotalRevenue()/total_intensity));
				total_revenue = total_revenue + solarcell.getTotalRevenue();
				}
			header.Add(this.stringToCsv("total light intensity"));
			body.Add(this.doubleToCsv(total_intensity));
			header.Add(this.stringToCsv("total revenue"));
			body.Add(this.doubleToCsv(total_revenue));
			header.Add(this.stringToCsv("total yield"));
			body.Add(this.doubleToCsv(total_revenue/total_intensity));
			/*header.Add(this.stringToCsv("total yield (%)"));
			body.Add(this.stringToCsv((100*total_revenue/total_intensity)+"%"));*/
			/*header.Add(this.stringToCsv("total time(s)"));
			body.Add(this.doubleToCsv(total_shine_duration));*/
			IDictionaryEnumerator e = this.exportFields.GetEnumerator();
			while (e.MoveNext()) {
				header.Add(this.stringToCsv(e.Key.ToString())); //both key and value are already Strings... but anyway
				body.Add(this.stringToCsv(e.Value.ToString()));
			}

			//WRITING DATA TO FILES:
			int lightsourcescount = this.registeredLightSources.Count;
			int solarcellcount = this.registeredPerformanceObjects.Count;
			int exportFieldsCount = this.exportFields.Count;
			string fullcsvheader = this.exportpath+this.yieldtablefilename.Replace("%o", "l"+lightsourcescount+"s"+solarcellcount+"e"+exportFieldsCount)+"_header.csv";
			string fullcsvbody = this.exportpath+this.yieldtablefilename.Replace("%o", "l"+lightsourcescount+"s"+solarcellcount+"e"+exportFieldsCount)+"_body.csv";
			string fullcsvfilename = this.exportpath+this.yieldtablefilename.Replace("%o", "l"+lightsourcescount+"s"+solarcellcount+"e"+exportFieldsCount)+".csv";
			/* If the data should be appended and the file already exists */
			bool appendcsv = (this.csvdatawritemode==APPEND);
			if (appendcsv) {
				if (!File.Exists(fullcsvfilename)) { 
					Console.WriteLine("Creating file to append CSV data to: " +fullcsvfilename);
					appendcsv = false;
					} else {
					Console.WriteLine("Appending CSV data to: " +fullcsvfilename);
					}
				} else {
				Console.WriteLine("(over)Writing CSV data to: " +fullcsvfilename);
				}


			using (StreamWriter bodycsvwritestream = new StreamWriter(fullcsvbody, appendcsv)) {
				bodycsvwritestream.WriteLine(this.csvLine(body));
				}
			using (StreamWriter headercsvwritestream = new StreamWriter(fullcsvheader, false)) {
				headercsvwritestream.WriteLine(this.csvLine(header));
				}
			using (StreamWriter fullcsvwritestream = new StreamWriter(fullcsvfilename, this.csvdatawritemode==APPEND)) {
				if (!appendcsv) {
					fullcsvwritestream.WriteLine(this.csvLine(header));
					}
				fullcsvwritestream.WriteLine(this.csvLine(body));
				}
			}// end old_writeYieldCSV
		


}}