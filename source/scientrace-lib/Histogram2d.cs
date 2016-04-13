// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {
public class Histogram2d : TraceJournalExportableHistogram {

	public Scientrace.NonzeroVector referenceVector;
	public Dictionary<Scientrace.UnitVector, Scientrace.UnitVector> plane_a_vecs = new Dictionary<UnitVector, UnitVector>();
	public Dictionary<Scientrace.UnitVector, Scientrace.UnitVector> plane_b_vecs = new Dictionary<UnitVector, UnitVector>();

	public Histogram2d(TraceJournal aTJ):base(aTJ) {
		this.tag="hist2d";
		}

	public void fillPlaneVectors(Scientrace.UnitVector surface_normal) {
		//"A = ref x norm"
		if (!this.plane_a_vecs.ContainsKey(surface_normal))
			this.plane_a_vecs.Add(surface_normal, this.referenceVector.safeNormalisedCrossProduct(surface_normal));
		//"B = norm x A"
		if (!this.plane_b_vecs.ContainsKey(surface_normal))
			this.plane_b_vecs.Add(surface_normal, surface_normal.safeNormalisedCrossProduct(this.plane_a_vecs[surface_normal]));
		}

	private void addHist2dConfigVars(Scientrace.UnitVector surface_normal, string plane_description) {
		this.extra_exportable_config_vars = new Dictionary<string, string>();
		this.extra_exportable_config_vars.Add("norm x", surface_normal.x.ToString());
		this.extra_exportable_config_vars.Add("norm y", surface_normal.y.ToString());
		this.extra_exportable_config_vars.Add("norm z", surface_normal.z.ToString());

		this.extra_exportable_config_vars.Add("ref x", this.referenceVector.x.ToString());
		this.extra_exportable_config_vars.Add("ref y", this.referenceVector.y.ToString());
		this.extra_exportable_config_vars.Add("ref z", this.referenceVector.z.ToString());

		this.fillPlaneVectors(surface_normal);
		this.extra_exportable_config_vars.Add("A = ref x norm", this.plane_a_vecs[surface_normal].trico());
		this.extra_exportable_config_vars.Add("B = norm x A", this.plane_b_vecs[surface_normal].trico());

		this.extra_exportable_config_vars.Add("Plane", plane_description);
		}

	public override void write(Scientrace.PhysicalObject3d anObject) {
		Scientrace.UnitVector normvec = anObject.getSurfaceNormal();
		this.fillPlaneVectors(normvec);
		
		this.write2dHistogramForPlane(anObject, "A = ref x norm", this.plane_a_vecs[normvec]);
		this.write2dHistogramForPlane(anObject, "B = norm x A", this.plane_b_vecs[normvec]);
		this.write2dHistogramForPlane(anObject, "1D histogram", null);
		}

	public void write2dHistogramForPlane(Scientrace.PhysicalObject3d anObject, string plane_descr, Scientrace.UnitVector plane_vector) {
		Scientrace.UnitVector normvec = anObject.getSurfaceNormal();

		Dictionary<string, double> angle_histogram = this.getHistogramTemplate();

		foreach(Scientrace.Spot casualty in this.tj.spots) {
			//only count casualties for current object.
			if (casualty.object3d != anObject) continue;

			if (casualty == null) {
				Console.WriteLine("Error: Casualty is null when writing angle histogram2d for {"+anObject.ToString()+"} plane {"+plane_descr+"}...");
				continue;
				}
			
			double angle_rad;
			if (plane_vector == null) {
				angle_rad = anObject.getSurfaceNormal().negative().angleWith(casualty.trace.traceline.direction);
				} 
			else {
				//2d unraveling
				Scientrace.Vector vecOnPlane = casualty.trace.traceline.direction.projectOnPlaneWithNormal(plane_vector);
				//The angle on which the histogram bin is based.
				angle_rad = normvec.negative().angleWith(vecOnPlane);
				}

			string bin = this.radiansToRoundedDegString(angle_rad);
			//Console.WriteLine("Line: "+casualty.trace.traceline.direction.trico()+" becomes: "+vecOnPlane.trico()+" angle:"+angle_rad+" / "+bin);
			this.addToBin(angle_histogram, bin, casualty.intensity, true);
			}

		string angle_histogram_csv_filename = this.tj.exportpath+this.angle_histogram_csv_filename.Replace("%o",anObject.tag).Replace("%t",this.tag).Replace("%s",plane_descr.Replace(" ", "_"));

		this.addHist2dConfigVars(normvec, plane_descr);
		//this.extra_exportable_config_vars.Add(plane_description, plane_vector);
		this.appendWriteWithConfigVariables(angle_histogram_csv_filename, angle_histogram);
		}


}}

