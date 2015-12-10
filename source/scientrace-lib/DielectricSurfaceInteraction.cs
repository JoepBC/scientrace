// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {


public class DielectricSurfaceInteraction {

		public Trace trace_in; // set at construction
		public double intensity_in; // set at construction
		public double intensity_after_absorption; // what is the intensity after absorption at the surface has been subtracted?
		public Object3d object_to; // set at construction

		public Location interaction_loc; //set at construction
		public UnitVector surface_normal; //set at construction

		/// <summary>
		/// The direction of the incoming beam.
		/// </summary>
		public UnitVector dir_i; //set at construction

		/// <summary>
		/// The incoming angle (to the surface of interaction normal)
		/// </summary>
		public double angle_i; //set by applySnellius()

		/// <summary>
		/// The transmitted/refracted angle (to the surface of interaction normal)
		/// </summary>
		public double angle_t; //set by applySnellius() if not total_internal_reflection
		public bool total_internal_reflection; //set by applySnellius()
		public bool total_absorption; //set by constructor

		// Some definitions: below are the FRACTIONS of the amplitudes after reflection (amp_r?) and refraction (amp_t?) for the vectors p and s.
		// amp_r? is described by eqn (25a) in Jenkins & White's "Fundamentals of Optics" [1]. amp_t? by eqn (25b).
		// E' in [1] is substituted by "E0t" (transmittance), E in [1] is substituted by "E0i" (incoming). Reflection R simply becomes "E0r".
		// The previous was incidentally at first WRONGFULLY confused with the transmission and reflection amplitude COEFFICIENTS (resp. "t" and "r").

		/// <summary>
		/// The amplitude of the electric vector decomposition of the incoming beam
		/// set @ calcIncomingAmplitudes() which is called by constructor
		/// </summary>
		public double amp_is, amp_ip; // set at calcIncomingAmplitudes
		/// <summary>
		/// The amplitude of the electric vector decomposition of the reflected beam
		/// set @ calcResultingAmplitudes() which is called by constructor
		/// </summary>
		public double amp_rs, amp_rp; // set at calcResultingAmplitudes
		/// <summary>
		/// The amplitude of the electric vector decomposition of the refracted/transmitted beam
		/// set @ calcResultingAmplitudes() which is called by constructor
		/// </summary>
		public double amp_ts, amp_tp; // set at calcResultingAmplitudes

		/// <summary>
		/// The refractive index for the previous medium (n_i) and the medium of interaction (n_t)
		/// </summary>
		public double n_i, n_t; // set at construction

		/// <summary>
		/// The projection of the incoming beam on the plane of interaction.
		/// </summary>
		public Vector in_projection; // set at calcResultingDirections

		// The direction orthogonal to the plane of refraction. From the German word "senkrecht" which means perpendicular.
		public UnitVector dir_s; // set at construction

		/// <summary>
		/// The direction "within the plane, orthogonal to dir_s and the direction of the trace" inherently 
		/// depends on the direction of the trace. Be default the directions are defined (by lack of phase)
		/// as p = s*z (with crossproduct *) and s = z*p, although the phase does not have significant relevance
		/// in the current Scientrace implementation.
		/// </summary>
		public UnitVector dir_ip, dir_tp, dir_rp; //dir_ip is set at construction, dir_rp and dir_tp are set at calcResultingDirections()

		/// <summary>
		/// The directions of the reflecting beam and (if not full internal reflection) the transmitted/refracted beam.
		/// </summary>
		public UnitVector dir_t, dir_r; // set by calcResultingDirections()

		// The intensity coefficients for the resp. fractions
		public double intc_rs, intc_rp; // set by calcIntensities()
		public double intc_ts, intc_tp; // set by calcIntensities()

		/// <summary>
		/// If absorption at a surface occurs, this fraction is larger than 0.0. A black surface has full=1.0 absorption.
		/// The transmission is 1-static_surface_absorption. As the amplitude is the squareroot of the intensity, the
		/// amplitudes of the trace are being multiplied by the squareroot of this transmission. This is performed by
		/// the method "performStaticAbsorption()" which is called by the constructor.
		/// </summary>
		public double static_surface_absorption;

		/// <summary>
		/// Initializes a new instance of the <see cref="Scientrace.DielectricSurfaceInteraction"/> class.
		/// </summary>
		/// <param name="incoming_trace">Incoming trace.</param>
		/// <param name="interaction_loc">Interaction location.</param>
		/// <param name="surface_normal">Surface normal.</param>
		/// <param name="refindex_from">Refractive index for the current volume</param>
		/// <param name="refindex_to">Refractive index on the other side of the surface</param>
		/// <param name="static_absorption_fraction">If absorption at a surface occurs, this fraction is larger than 0.0. A black surface has full=1.0 absorption.</param>
		public DielectricSurfaceInteraction(Trace aTrace, Location interaction_loc, UnitVector	surface_normal, 
											double refindex_from, double refindex_to, Object3d object_to) {
			// Store all starting conditions
			this.trace_in = aTrace;
			this.intensity_in = aTrace.intensity;
			this.object_to = object_to;
			this.n_i = refindex_from;
			this.n_t = refindex_to;
			this.dir_i = aTrace.traceline.direction;
			// Make sure the surface normal is oriented in the right direction
			this.surface_normal = surface_normal.orientedAgainst(this.dir_i).tryToUnitVector();
			this.interaction_loc = interaction_loc;

			// Define the "orthogonal to the plane - s" polarisation vector direction
			// Replaced "safeNormalised" method commented out below
			// At normal incidence, you want to retain the polarisation vectors of the trace for the best results
			Scientrace.Vector out_of_plane_vec = this.surface_normal.crossProduct(this.dir_i);
			if (out_of_plane_vec.length < 1E-9)
				out_of_plane_vec = new Vector(this.trace_in.getPolarisationDir1());
			this.dir_s = out_of_plane_vec.tryToUnitVector();
			//this.dir_s = this.surface_normal.safeNormalisedCrossProduct(this.dir_i);

			// Define the "in the plane - p" polarisation vector direction
			this.dir_ip = this.dir_s.crossProduct(this.dir_i).tryToUnitVector();
			this.in_projection = this.dir_i.projectOnPlaneWithNormal(this.surface_normal);

			//First, calculate all properties concerning the incoming trace
			this.applySnellius();
			this.calcIncomingAmplitudes();

			//Before doing anything else, ABSORP the static surface absorption.
			this.subtractStaticAbsorption();

			// If the trace has ended due to total absorption it ends here.
			if (this.total_absorption) return;

			//Now calculate all that other stuff.
			this.calcResultingAmplitudes();
			this.calcResultingIntensities();
			if (Math.Abs(1 - (this.intc_rp + this.intc_tp)) > 1E-9)
					throw new Exception("p-polarisation fractions don't add op to 1 (rp: "+this.intc_rp+" tp:"+this.intc_tp+")");
			if (Math.Abs(1 - (this.intc_rs + this.intc_ts)) > 1E-9)
					throw new Exception("s-polarisation fractions don't add op to 1 (rs: "+this.intc_rs+" ts:"+this.intc_ts+")");

			this.calcResultingDirections();

			//this.debugOutput();
			} //end of constructor

		public void debugOutput() {
			Console.WriteLine(this.ToString());			
			}

		public override string ToString() {
				return "amp_is: "+(this.amp_is)+" amp_ip: "+(this.amp_ip)+
						" i^2: "+(Math.Pow(this.amp_is,2)+Math.Pow(this.amp_ip,2))+
					" amp_ts: "+this.amp_ts+ " amp_tp: "+this.amp_tp
					+"\n intc_tp: "+this.intc_tp+" intc_ts: "+this.intc_ts+
					" intensity_in: "+this.intensity_in
						+"\ns: "+this.dir_s+" tp: "+this.dir_tp
					+" intensity_fraction: "+this.trace_in.intensityFraction()
					;
			}

		/// <summary>
		/// Application of Snell's law of refraction.
		/// </summary>
		public void applySnellius() {
			this.angle_i = this.dir_i.angleWith(this.surface_normal.orientedAlong(this.dir_i));
			double sin_transmission = Math.Sin(this.angle_i)*this.n_i/this.n_t;
			this.total_internal_reflection = (Math.Abs(sin_transmission) > 1);
			if (this.total_internal_reflection)
				return; // can't set (real) refraction angle on total internal reflection.
			this.angle_t = Math.Asin(sin_transmission);
			}

		/// <summary>
		/// A trace may have its polarisation components with their relative amplitudes, but
		/// interaction with a surface is likely to decompose these vectors into two new vectors (s and p).
		/// This method will perform this decomposition for vec1 and vec2 into "amp_is" and "amp_ip".
		/// </summary>
		public void calcIncomingAmplitudes() {
			UnitVector s = this.dir_s;
			UnitVector p = this.dir_ip;
			Vector v1 = this.trace_in.getPolarisationVec1();
			Vector v2 = this.trace_in.getPolarisationVec2();

			double squared_vec_sum_is = 0;
			double squared_vec_sum_ip = 0; 
			double oldval_is = 0;
			double oldval_ip = 0;	
			foreach (Vector aVec in new List<Vector>{v1, v2}) {
				squared_vec_sum_is += Math.Pow(aVec.dotProduct(s),2);
				squared_vec_sum_ip += Math.Pow(aVec.dotProduct(p),2);
				oldval_is += aVec.dotProduct(s);
				oldval_ip += aVec.dotProduct(p);
				}
			this.amp_is = Math.Pow(squared_vec_sum_is,0.5);
			this.amp_ip = Math.Pow(squared_vec_sum_ip,0.5);
			/*
			double l1 =	v1.length*v1.length + v2.length*v2.length;
			double l2 = this.amp_is*this.amp_is + this.amp_ip * this.amp_ip;
			Console.WriteLine((l1-l2).ToString()+" =("+l1+"/"+l2+")      from v1/v2:"+v1.length.ToString()+"/"+v2.length.ToString()+" becomes: "+this.amp_is+"/"+this.amp_ip+" was:"+oldval_is+"/"+oldval_ip); 
			/* */
			}


		/// <summary>
		/// Before calculating the amplitudes for the reflected and/or refracted fractions, static absorption has to be substracted.
		/// </summary>
		public void subtractStaticAbsorption() {
			this.static_surface_absorption = this.object_to.materialproperties.absorption(this.trace_in, this.surface_normal, this.trace_in.currentObject.materialproperties);
			double transmission = 1 - this.static_surface_absorption;
			double sqrt_transmission = Math.Sqrt(transmission);

			this.amp_ip = this.amp_ip*sqrt_transmission;
			this.amp_ip = this.amp_ip*sqrt_transmission;
			this.total_absorption = (transmission == 0);

			double absolute_absorption_density = this.static_surface_absorption*this.intensity_in;

			this.intensity_after_absorption = this.intensity_in * transmission;

			double relative_absorption_density = this.static_surface_absorption*this.trace_in.intensityFraction();
			if (this.static_surface_absorption > 0.000001) { //don't write a spot at minimal absorption.
				TraceJournal.Instance.recordSpot(new Scientrace.Spot(this.interaction_loc, 
					                                                  object_to,
					                                                  absolute_absorption_density, relative_absorption_density,
					                                                  this.trace_in));
					}
			}


		/// <summary>
		/// Calculate the polarisation amplitudes after reflaction and refraction according to chapter 25.1 from [1].
		/// </summary>
		public void calcResultingAmplitudes() {

			//The special cases of total_internal_reflection
			if (this.total_internal_reflection) {
				this.amp_rs = this.amp_is;
				this.amp_rp = this.amp_ip;
				this.amp_ts = 0;
				this.amp_tp = 0;
				return;
				}
			
			double sums = this.angle_i + this.angle_t;
			double imint = this.angle_i - this.angle_t;
			//double rmint = this.angle_refracted - this.angle_in;
			double sinofsums = Math.Sin(sums);

			/* A "division by zero" error must be prevented. But how is the
			 * amplitude calculated based on the angles if both are zero?
			 **** derivation of normal angle calculation ****
			 * According to Jenkins 25a:
			 * Rs/Es = -sin(a1 - a2) / sin(a1 + a2)
			 * And at normal incidence (disregarding phase): |Rs/Es| = |Rp/Ep|
			 * In the case of incident angles, both a1 and a2 are 0, Rs/Es can not be calculated.
			 * Let's take the limit towards 0. Snellius states that:
			 * n1 sin a1 = n2 sin a2
			 * When a1 and a2 are very very small, the approximation of the small sine can be used:
			 * -> sin x = x (when x is very small)
			 * so: n1 a1 = n2 a2
			 * a2 = a1 * n1 / n2
			 * That makes Jenkins 25a can be reduced to:
			 * Rs/Es = -sin(a1 - a2) / sin(a1 + a2)
			 * Rs/Es = -sin(a1 - (a1 * n1 / n2)) / sin(a1 + (a1 * n1 / n2))
			 * Small sine aproximation again (as a1 is very small and so is the product of the refractive indices with a1):
			 * Rs/Es = -(a1 * (1 - (n1 / n2))) / (a1 * (1 + (n1 / n2)))
			 * Get rid of a1:
			 * Rs/Es = -(1 - (n1 / n2)) / (1 + (n1 / n2))
			 */
			if (sinofsums == 0) {
				double nit = (this.n_i/this.n_t);
				this.amp_rs = amp_is * -((1.0 - nit) / (1.0 + nit));
				this.amp_rp = amp_ip * ((1.0 - nit) / (1.0 + nit));

				//this line made no sense whereas normal incident light (sinofsums==0) can never fully internally reflect.
				//if (this.total_internal_reflection) return;

				/*
				 * According to Jenkins (25f) and Hecht (4.49) 
				 * note that Hecht (4.50) would have prescribed this.amp_tp = (this.amp_ip - this.amp_rp)
				 * which should result in the same answer.
				 * THIS METHOD WORKED WHEN GOING INTO THE MEDIUM, BUT FAILS WHEN LEAVING.
				 *
				this.amp_ts = this.amp_is + this.amp_rs;
				this.amp_tp = (this.amp_ip + this.amp_rp) / (this.n_t/this.n_i);
				 */

				this.amp_ts = this.amp_is * 2.0 * nit / (1.0+nit);
				this.amp_tp = this.amp_ip * 2.0 * nit / (1.0+nit);

				return;
				}
			
			/* The formulae as stated in Hecht (at least the fourth edition) 
			 * lack the note of E, the amplitude of the incoming vector.
			 * This IS stated in Jenkins, and also applied here due to the
			 * principle of photon splitting/intensities. 
			 * The incoming intensity is seldomly normalised. */

			// (25A) Jenkins / (4.42) Hecht * this.amp_ix
			this.amp_rs = this.amp_is * - (Math.Sin(imint) / sinofsums);
			// (25A) Jenkins / (4.43) Hecht * this.amp_ix
			this.amp_rp = this.amp_ip * (Math.Tan(imint) / Math.Tan(sums));

			if (this.total_internal_reflection) {
				throw new InvalidOperationException();
				// used to be: return;
				}

			//(25B) Jenkins / (4.44) Hecht * this.amp_ix
			this.amp_ts = this.amp_is * 2.0 * Math.Sin(this.angle_t) * Math.Cos(this.angle_i) / sinofsums;
			//(25B) Jenkins / (4.45) Hecht * this.amp_ix
			this.amp_tp = this.amp_ip * 2.0 * Math.Sin(this.angle_t) * Math.Cos(this.angle_i) / (sinofsums*Math.Cos(imint));
			}


		public void calcResultingIntensities() {
			//(4.56 [4.61 is for coefficients]) Hecht, (25C) Jenkins
			this.intc_rs = this.amp_rs*this.amp_rs / (this.amp_is*this.amp_is);
			//(4.56 [4.62 is for coefficients]) Hecht, (25C) Jenkins
			this.intc_rp = this.amp_rp*this.amp_rp / (this.amp_ip*this.amp_ip);
			
			if (this.total_internal_reflection) {
				this.intc_ts = 0;
				this.intc_tp = 0;

				return;
				}

			double ncosfrac = (n_t * Math.Cos(angle_t)) / (n_i * Math.Cos(angle_i));

			//(4.57 [4.63 is for coeffs]) Hecht, not stated explicitly in Jenkins
			this.intc_ts = ncosfrac * amp_ts*amp_ts / (this.amp_is*this.amp_is);
			//(4.57 [4.64  is for coeffs]) Hecht
			this.intc_tp = ncosfrac * amp_tp*amp_tp / (this.amp_ip*this.amp_ip);

	/*		
			Console.WriteLine(" rs: "+this.intc_rs+" rp:"+this.intc_rp+" ts: "+this.intc_ts+" tp:"+this.intc_tp+  " e:"+ncosfrac);


			//Console.WriteLine("-- Going from "+n_i+" to "+n_t+", resp. "+(angle_i*180/Math.PI)+"/"+(angle_t*180/Math.PI)+" introduces an amplitude to energy conversion factor of: "+ncosfrac);
			Console.WriteLine(
				"Start intensity: "+this.intensity_in+"\n"+
				" is^2:"+amp_is*amp_is+
				" ip^2:"+amp_ip*amp_ip+"\n"+
				" ts^2:"+amp_ts*amp_ts+
				" tp^2:"+amp_tp*amp_tp+"\n"+
				" Tcs:"+intc_ts+
				" Tcp:"+intc_tp+"\n"+
				" rs^2:"+amp_rs*amp_rs+
				" rp^2:"+amp_rp*amp_rp+"\n"+
				" Rcs:"+intc_rs+
				" Rcp:"+intc_rp);
/**/
			

			//And if everything is correct, as a result int_tx + int_rx = int_ix.
			}

		public void calcResultingDirections() {
			//Calculate the direction of the reflecting trace:
			UnitVector norm = this.surface_normal.orientedAlong(this.dir_i);
			this.dir_r = this.dir_i.reflectOnSurface(norm);
			this.dir_rp = this.dir_s.crossProduct(this.dir_r).tryToUnitVector();
			Vector l1 = this.in_projection;
			Vector l2 = l1 * (this.n_i / this.n_t);
			// Check for full internal reflection:
			if (l2.length > 1) {
				if (!total_internal_reflection)
					throw new ArgumentOutOfRangeException("Method {calcResultingDirections} finds that full internal reflection occurs, whereas this was not the case for the {applySnellius} method.");
				return;
				}
			this.dir_t = l2.constructHypothenuse(norm);
			this.dir_tp = this.dir_s.crossProduct(this.dir_t).tryToUnitVector();
			}

		public Trace getReflectTrace() {
			return this.getReflectTrace(0);
			}

		public Trace getReflectTrace(double offset) {
			// If the trace has ended due to total absorption, there's no reflection.
			if (this.total_absorption) return null;

			// Construct the new trace line
			Scientrace.Line reflect_line = new Scientrace.Line(this.interaction_loc + (this.dir_r.toVector()*offset), this.dir_r);

			double new_intensity = (this.intensity_after_absorption/this.intensity_in) *
						(Math.Pow(this.amp_rs,2) + Math.Pow(this.amp_rp,2));

			// Sometimes, this new intensity is to small to care about. Return null.	
			if (new_intensity <= MainClass.SIGNIFICANTLY_SMALL) {
				//Console.WriteLine("New intensity is: "+new_intensity+" is:"+this.amp_is+" ip:"+this.amp_ip+" ts:"+this.amp_ts+" tp:"+this.amp_tp);
				return null;
				}

			Scientrace.Trace reflect_trace = 
					trace_in.fork(reflect_line, (this.dir_s.toVector()*this.amp_rs), (this.dir_rp.toVector()*this.amp_rp), 
									new_intensity, (this.total_internal_reflection?"_fr":"_r"));

			// Remain within the same object / space
			reflect_trace.currentObject = this.trace_in.currentObject;

			// Don't bother with empty traces.
			if (reflect_trace.isEmpty()) return null;

			return reflect_trace;
			}

		public Trace getRefractTrace() {
			return this.getRefractTrace(0);
			}
		public Trace getRefractTrace(double offset) {
			// If the trace has ended due to total absorption, there's no either.
			if (this.total_absorption) return null;

			// At total internel reflection, there's no refraction either (statements split up for readability purposes).
			if (this.total_internal_reflection) return null;

			Scientrace.Line refract_line = new Scientrace.Line(this.interaction_loc + (this.dir_t.toVector()*offset), this.dir_t);

			double refr_intensity = (this.intensity_after_absorption/this.intensity_in) *
						((Math.Pow(this.amp_is,2) + Math.Pow(this.amp_ip,2))-(Math.Pow(this.amp_rs,2) + Math.Pow(this.amp_rp,2)) );

			Scientrace.Trace refract_trace = 
					trace_in.fork(refract_line, (this.dir_s.toVector()*this.amp_ts), (this.dir_tp.toVector()*this.amp_tp), 
									refr_intensity, "_t");
			refract_trace.currentObject = this.object_to;

			// Don't bother with empty traces.
			if (refract_trace.isEmpty()) return null;

			return refract_trace;
			}


	} //end class DielectricSurfaceInteraction


}//end namespace