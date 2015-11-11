
using System;

namespace ScientraceXMLParser {


	public class AngleDerivation {
		public bool initialized = false;

		//Static Suncycle System parameters in radians
		public double par_mirr_z;
		public double fresnel_norm_z;
		public double refractive_index;

		//Light parameters
		public Scientrace.Vector light_before_refraction;
		public Scientrace.Vector light_after_1st_refraction;
 
		//output values
		public double nrad = 0;
		public double urad = 0;

		 
		//debug only variables, for math use _z variables declared above
		double debug_mirr_angle_deg, debug_fresnel_angle_deg;

		//constructor
		public AngleDerivation (double par_mirr_angle_with_z_degrees, double fresnel_normal_with_z_degrees, double refractive_index) {
			//debug storage:
			this.debug_mirr_angle_deg = par_mirr_angle_with_z_degrees; this.debug_fresnel_angle_deg = fresnel_normal_with_z_degrees;

			//storing relevant variables
			this.refractive_index = refractive_index;
			this.fresnel_norm_z = Math.Cos (AngleDerivation.getRadians (fresnel_normal_with_z_degrees));
			this.par_mirr_z = Math.Cos (AngleDerivation.getRadians (par_mirr_angle_with_z_degrees));
		}
		
		public void setIncomingLightVectorFromXmZY(Scientrace.Vector v) {
			this.setIncomingLightVector(new Scientrace.Vector(v.x, v.z, -v.y));
		}
	
		public void setIncomingLightAngle (double angle_with_z_axis, double angle_with_x_axis) {
			double opx, opy, opz, xangle, zangle;
			Scientrace.Vector op;
			zangle = AngleDerivation.getRadians (angle_with_z_axis);
			xangle = AngleDerivation.getRadians (angle_with_x_axis);
			opx = Math.Sin (zangle) * Math.Cos (xangle);
			opy = Math.Sin (zangle) * Math.Sin (xangle);
			opz = Math.Sqrt (1 - (opy * opy) - (opx * opx));
			op = new Scientrace.Vector (opx, opy, opz);
			this.setIncomingLightVector (op);
		}

		public void setIncomingLightVector(Scientrace.Vector op) {
			double opx, opy, px, py, pz, s;
			opx = op.x;
			opy = op.y;
			s = this.refractive_index;
			this.light_before_refraction = op;
			py = opy / s;
			px = opx / s;
			pz = Math.Sqrt (1 - (py * py) - (px * px));
			this.light_after_1st_refraction = new Scientrace.Vector (px, py, pz);
		}

		public void setRotationAngles () {

			double s, px, py, pz, nz, uz;
			//shorter name reference variables
			s = this.refractive_index;
			px = this.light_after_1st_refraction.x;
			py = this.light_after_1st_refraction.y;
			pz = this.light_after_1st_refraction.z;
			nz = this.fresnel_norm_z;
			uz = this.par_mirr_z;
			double A1, A2, B1, B2, ra, rb, c, chi, ABCa, ABCb,
			ABCc;
			//temp variables
			//B = ux, also the answer from the ABC aquation
			//A = nx
			//A relates to B via constants (B - s*px) / A = c (with c = (uz-s*pz)/nz )
			//ra is the distance from n to the Z axis
			ra = Math.Sqrt (1 - (nz * nz));
			//rb is the distance from u to the Z axis
			rb = Math.Sqrt (1 - (uz * uz));
			c = (uz - (s * pz)) / nz;
			//chi is a combination of known constants making equations shorter.
			chi = (rb * rb) + (s * s * ((px * px) + (py * py))) - (c * c * ra * ra);
			
			ABCa = 4 * s * s * ((py * py) + (px * px));
			ABCb = -4 * s * px * chi;
			ABCc = (chi * chi) - (4 * s * s * py * py * rb * rb);
			// get values B1 and B2 via ABC formula
			B1 = 0;
			B2 = 0;
			this.initialized = this.ABC(ref B1, ref B2, ABCa, ABCb, ABCc);
			if (!this.initialized) { return; }
			A1 = (B1 - (s * px)) / c;
			A2 = (B2 - (s * px)) / c;
			/*debug:
			Console.WriteLine(" chi: "+chi+"; c: "+c+ "; px:"+px);
			Console.WriteLine("A = "+ABCa+", B = "+ABCb+"; C = "+ABCc);
			Console.WriteLine("A1 = "+A1+", B1 = "+B1+"; A2 = "+A2+", B2 = "+B2); */			
			
			//PSEUDO code explaining what's happening below
			//ny = +/-Math.Sqrt((ra*ra)-(An*An)); (An = A1 or A2)
			//uy = +/-Math.Sqrt((rb*rb)-(Bn*Bn)); (Bn = B1 or B2)
			//all fine if:
			//(uy-(s*py))/(ny) = c
			double ny1, ny2, uy1, uy2;
			ny1 = 0;
			ny2 = 0;
			uy1 = 0;
			uy2 = 0;
			this.getnyuy (ref ny1, ref uy1, ra, rb, A1, B1, py, c);
			this.getnyuy (ref ny2, ref uy2, ra, rb, A2, B2, py, c);
			
			this.setOutputAngles (A2, ny2, B2, uy2);

			//this.printAngles(A1, ny1, B1, uy1);
			//this.printAngles(A2, ny2, B2, uy2);
		}

		public void setOutputAngles (double nxo, double nyo, double uxo, double uyo)
		{
			double nx, ny, ux, uy;
			double rn = Math.Sqrt ((nxo * nxo) + (nyo * nyo));
			double ru = Math.Sqrt ((uxo * uxo) + (uyo * uyo));
			nx = nxo / rn;
			ny = nyo / rn;
			ux = uxo / ru;
			uy = uyo / ru;
			this.nrad = AngleDerivation.getRadians(nx, ny);
			this.urad = AngleDerivation.getRadians(ux, uy);
			this.removeNans();
			//Console.WriteLine(this.nrad+"AFTER");
		}

		public void removeNans() {
			if (Double.IsNaN(this.nrad)) {
				this.nrad = 0;
				//this.nrad = (Double.IsNaN(this.urad) ? 0 : this.urad);
				}
			if (Double.IsNaN(this.urad)) {
				this.urad = 0;
				//this.urad = (Double.IsNaN(this.nrad) ? 0 : this.nrad);
				}
			}

		public void printAngles (double nxo, double nyo, double uxo, double uyo) {
			double nx, ny, ux, uy;
			double rn = Math.Sqrt ((nxo * nxo) + (nyo * nyo));
			double ru = Math.Sqrt ((uxo * uxo) + (uyo * uyo));
			nx = nxo / rn;
			ny = nyo / rn;
			ux = uxo / ru;
			uy = uyo / ru;
			//Console.WriteLine ("\n\n nrad = " + this.getRadians (nx, ny) + "rad, n = {" + nxo + ";" + nyo + "; " + Math.Sqrt (1.0 - rn * rn) + "}" + "\n urad = " + this.getRadians (ux, uy) + "rad, u = {" + uxo + ";" + uyo + "; " + Math.Sqrt (1.0 - ru * ru) + "}" + "\n diff (deg): " + ((this.getDAngle (nx, ny) - this.getDAngle (ux, uy))));
			Console.WriteLine ("\n\n nrad = " + this.nrad + "rad, n = {" + nxo + ";" + nyo + "; " + Math.Sqrt (1.0 - rn * rn) + "}" + "\n urad = " + this.urad + "rad, u = {" + uxo + ";" + uyo + "; " + Math.Sqrt (1.0 - ru * ru) + "}" + "\n diff (deg): " + ((AngleDerivation.getDAngle (nx, ny) - AngleDerivation.getDAngle (ux, uy))));
		}


		public void getnyuy (ref double ny, ref double uy, double ra, double rb, double A, double B, double py, double c)
		{
			double s = this.refractive_index;
			double nyp, nym;
			double uyp, uym;
			nyp = Math.Sqrt ((ra * ra) - (A * A));
			//positive squareroot
			nym = -Math.Sqrt ((ra * ra) - (A * A));
			//negative "
			uyp = Math.Sqrt ((rb * rb) - (B * B));
			uym = -Math.Sqrt ((rb * rb) - (B * B));
			if (AngleDerivation.closeEnough ((uyp - (s * py)) / (nyp), c)) {
				ny = nyp;
				uy = uyp;
				return;
			}
			if (AngleDerivation.closeEnough ((uyp - (s * py)) / (nym), c)) {
				ny = nym;
				uy = uyp;
				return;
			}
			if (AngleDerivation.closeEnough ((uym - (s * py)) / (nyp), c)) {
				ny = nyp;
				uy = uym;
				return;
			}
			if (AngleDerivation.closeEnough ((uym - (s * py)) / (nym), c)) {
				ny = nym;
				uy = uym;
				return;
			}
		}

		public bool ABC (ref double B1, ref double B2, double ABCa, double ABCb, double ABCc)
		{
			double discr = (ABCb * ABCb) - (4 * ABCa * ABCc);
			//if (discr < -1E-17) { discr = 0; }
			if (discr < 0) {
				Console.WriteLine ("* WARNING: DISCRIMINANT <= 0; -> discr = "+discr+
					" f^"+this.debug_fresnel_angle_deg+" m^"+this.debug_mirr_angle_deg+
					" l^"+this.light_before_refraction.trico());
				discr = 0;
				return false;
			} else {
				//Console.WriteLine ("CLEAN discr = "+discr);
			}
			B1 = (-ABCb + Math.Sqrt (discr)) / (2 * ABCa);
			B2 = (-ABCb - Math.Sqrt (discr)) / (2 * ABCa);
			return true;
		}

		public static double getRadians (double degrees) {
			return degrees * Math.PI / 180;
		}

		public static double getDegrees (double radians)
		{
			return radians * 180 / Math.PI;
		}

		public static double getDAngle (double x, double y)
		{
			return ((Math.Acos (x) * (y / Math.Abs (y)) * 180 / Math.PI));
		}

		public static double getRadians (double x, double y)
		{
			return ((Math.Acos (x) * (y / Math.Abs (y))));
		}

		public static bool closeEnough (double a, double b)
		{
			return (Math.Ceiling (a * 1000) == Math.Ceiling (b * 1000));
		}
		
	}
}

