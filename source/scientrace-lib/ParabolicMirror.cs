// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class ParabolicMirror : Scientrace.FlatSurfaceObject3d {

	//top of the parabole @ public Scientrace.Location loc;

	//Where does the norm above the minimum value (note the direction of the parabole) point towards?
	public Scientrace.UnitVector zaxis;

	/// <summary>
	/// The constant c in z = c*(x^2 + y^2)
	/// </summary>
	public double c;

	/// <summary>
	/// When an X3D image is exported the mirror is drawn as a grid. This value represents the number
	/// of gridpoints on each of both axis over which it is drawn. A low number will show a low-res
	/// mirror, a large number will make it both slow when viewing but also less transparent in display.
	/// </summary>
	public int exportX3Dgridsteps = 32;
	public bool x3dgridspheres = false;

	/// <summary>
	/// TRF will be used to "rotate" the environment in order to have the mirror-plane pointing at 0,1,0
	/// </summary>
	public Scientrace.VectorTransformOrthonormal trf;

	/// <summary>
	/// Only a specific range of the parabole is used which is defined as being within CylindricalBorder
	/// </summary>
	public Scientrace.AbstractGridBorder cborder;		
		
	/// <summary>
	/// The parabolic mirror is one of the key elements of the SunCycle concentrator
	/// When light is directed onto a parabolic mirror in the right direction the
	/// lightrays will reflect towards a single point, hence it can be used as a concentrator.
	/// </summary>
	/// <param name="parent">
	/// A <see cref="Scientrace.Object3dCollection"/> in which the physicalobject "parabolicmirror" is placed.
	/// </param>
	/// <param name="mprops">
	/// A <see cref="Scientrace.MaterialProperties"/> instance defines the properties of the mirror.
	/// </param>
	/// <param name="loc">
	/// A <see cref="Scientrace.Location"/> defines the location which has to be shifted to 0,0,0 to make z = x^2 + y^2 = 0
	/// </param>
	/// <param name="zaxis">
	/// A <see cref="Scientrace.UnitVector"/> pointing the direction of the norm at the "bottom" of the mirror pointing "up"
	/// </param>
	/// <param name="c">
	/// A <see cref="System.Double"/>, the value c for which z = c(x^2+y^2) after a location shift.
	/// </param>
	/// <param name="cborder">
	/// A <see cref="CylindricalBorder"/> the mirror only exists between these borders.
	/// </param>
	public ParabolicMirror(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops, 
		                       Scientrace.Location loc, Scientrace.UnitVector zaxis, double c, 
		                       AbstractGridBorder cborder) : base(parent, mprops) {
		/* Console.WriteLine("Mirror props: loc:"+
			                  loc.trico()+" zax:"+zaxis.trico()+ " c:"+c +" border: "+cborder.ToString()); */
		this.c = c;
		this.loc = loc;
		this.zaxis = zaxis;
		this.cborder = cborder;


		Scientrace.UnitVector basevec1 = new Scientrace.UnitVector(1,0,0);
		if (Math.Abs(zaxis.dotProduct(basevec1)) > 0.8) {
			basevec1 = new Scientrace.UnitVector(0,1,0);
			}
		Scientrace.UnitVector v = basevec1.crossProduct(zaxis).tryToUnitVector();
		trf = new VectorTransformOrthonormal(zaxis, v, VectorTransform.SWAP_U_WITH_W);
		
		//Console.WriteLine("V transform: "+trf.transform(v).trico()+ " z: "+zaxis+" transform(Z):"+trf.transform(zaxis).trico());
	}

	public static Scientrace.ParabolicMirror CreateAtFocus(Scientrace.Object3dCollection parent, 
						Scientrace.MaterialProperties mprops, Scientrace.Location focusloc,
						double distancetormirror, Scientrace.UnitVector zaxis, 
						AbstractGridBorder cborder) {
			double c = 1/(4*distancetormirror);
			Scientrace.Location loc = (focusloc - (zaxis.toVector()*distancetormirror)).toLocation();
			return new Scientrace.ParabolicMirror(parent, mprops, loc, zaxis, c, cborder);
		}

	public Scientrace.IntersectionPoint[] realIntersections(Scientrace.Line traceline, bool checkBorder) {
//			Scientrace.Line line = traceline-this.loc;	//substract loc value from traceline location, 
													//leave direction unchanged
			//Console.WriteLine("In untransformed world: traceline -> "+traceline.ToString());
			//default value should be checkBorder = true;
			Scientrace.VectorTransform trf = this.trf;
			Scientrace.Line trfline = trf.transform(traceline-this.loc);
					//transform location AND direction
			Scientrace.IntersectionPoint[] ips = this.baseintersections(trfline);
			Scientrace.IntersectionPoint tip;
			Scientrace.IntersectionPoint[] retips = new Scientrace.IntersectionPoint[2];
			for (int ipi = 0; ipi < ips.GetLength(0); ipi++) {
				if (ips[ipi] == null) {
					retips[ipi] = null;
					continue;
				}
				//Console.WriteLine("$$$$$$$$$$$$$$$$$"+ips[ipi].loc+"in?:"+trfline);
				//Console.WriteLine("___1");
				
				//check below removed for performance reasons
				if (!trfline.throughLocation(ips[ipi].loc, 0.00001)) {
					string warning =@"ERROR: GOING DOWN! \n baseintersections "+trfline+" FAILED!";
					throw new ArgumentOutOfRangeException(warning+ips[ipi].loc.trico() + " not in line " + trfline);
				}/* else {
					Scientrace.Line reflline = trfline.reflectAt(this.baseIntersectionPlane(ips[ipi].loc.x, ips[ipi].loc.y),0);
				//Console.WriteLine("___2");
					/*if (!reflline.throughLocation(this.getBaseConcentrationPoint(), 0.000001)) {
						//throw new ArgumentOutOfRangeException("CONCENTREER EENS!");
						}*/
				//	} end of removed check
				//throw new AccessViolationException("FOO");
				//Console.WriteLine("ips["+ipi.ToString()+"]:"+ips[ipi].ToString());
				
				//tip = trf.transformback(ips[ipi])+this.loc;
				
				tip =ips[ipi].copy();
				tip.loc = trf.transformback(ips[ipi].loc)+this.loc;
				tip.flatshape.plane = trf.transformback(tip.flatshape.plane);
				
				//Console.WriteLine("___3");
				/*if (!traceline.throughLocation(tip.loc, 0.0001)) {
					throw new ArgumentOutOfRangeException(tip.loc.trico() + " (transformed) not in line " + traceline);
				} else {
					}*/

			if (this.cborder.contains(tip.loc)|| !checkBorder) { //is this location within the borders between which the
													  //parabolic mirror exists?
					retips[ipi] = tip;
/*					//below for debug purposes only
					retips[ipi] = new IntersectionPoint(traceline.startingpoint+
					                                    traceline.direction.toLocation()*(traceline.direction.dotProduct(tip.loc-traceline.startingpoint))
					                                                                      , tip.plane);*/
				} else {
					//Console.WriteLine(tip.loc.ToString() + " is NOT within the cborder range-> "+this.cborder);
					retips[ipi] = null;
					continue;
				}

				//Console.WriteLine("trf: "+trf.ToString());
				//Console.WriteLine("retips["+ipi.ToString()+"]:"+retips[ipi].ToString());
			}	
			return retips;
		}

		
	public Scientrace.Intersection realIntersections(Scientrace.Trace trace, bool checkborder) {
			// check whether trace goes through cborder first for performance reasons
			Scientrace.Intersection intr;
			//if (this.cborder.crosses(trace)) {
				Scientrace.IntersectionPoint[] tips = this.realIntersections(trace.traceline, checkborder);
				intr = new Scientrace.Intersection(trace, tips, this);
			//}/ else {
			//	intr = new Intersection(false, this);
			//}
			return intr;
		}

	/// <summary>
	/// "line" has to be transformed so it encounters the 2nd order polynominal with 
	/// its base at 0,0,0 and orientation in 0,0,1 (z-direction)
	/// </summary>
	/// <param name="line">
	/// A <see cref="Scientrace.Line"/>
	/// </param>
	public IntersectionPoint[] baseintersections(Scientrace.Line line) {
		// location = line.loc * (d * line.dir);
		// keep formulae readable
		double d1, d2, dx, dy, dz, c, lx, ly, lz;
		double ABCa, ABCb, ABCc, discr;
		Scientrace.Vector v1, v2;
		Scientrace.IntersectionPoint[] ip;
		c = this.c;
		dx = line.direction.x;
		dy = line.direction.y;
		dz = line.direction.z;
		lx = line.startingpoint.x;
		ly = line.startingpoint.y;
		lz = line.startingpoint.z;
		
		ABCa = c*(Math.Pow(dy, 2)+Math.Pow(dx, 2));
		ABCb = (2*c*(dy*ly+dx*lx))-dz;
		ABCc = (c*(Math.Pow(ly, 2)+Math.Pow(lx, 2)))-lz;
			
		/*if ((dx*dy) == 0) {
			ip = new Scientrace.IntersectionPoint[1];
			v1 = new Vector(lx, ly, c*(Math.Pow(lx, 2)+Math.Pow(ly, 2)));
			ip[0] = new IntersectionPoint(v1.toLocation(), this.intersectionPlane(v1.x, v1.y));
			//ip[1] = null;
			return ip;
		}*/
//		if ((Math.Pow(ABCa,2)) < 1E-56) { // <-- used to be 
		if ((Math.Pow(ABCa,2)) < 1E-31) { // <-- now, so no (/less) significance errors do occur any longer in results
			//Console.WriteLine("ABCa is SMALL"+ABCa.ToString());
			ip = new Scientrace.IntersectionPoint[1];
			d1 = -ABCc / ABCb;
			v1 = (line.direction.toVector()*d1)+line.startingpoint.toVector();
			ip[0] = new IntersectionPoint(v1.toLocation(), this.baseIntersectionShape(v1.x, v1.y));
			return ip;
		}
		//Console.WriteLine("ABCa is large enough "+ABCa.ToString());

		//d1,2 => lambda 1,2 at http://amswiki.jbos.eu/wiki/index.php/Parabolic_Collision#Quadratic_formula
		//derived from the quadratic formula, note the +/- sign before the Sqrt.
		discr = Math.Pow(ABCb, 2)-(4*ABCa*ABCc);
		if (discr < 0) {
			//Console.WriteLine("Negative discriminant : "+discr);
			ip = new Scientrace.IntersectionPoint[0];
			//ip[0] = null;
			//ip[1] = null;
			return ip;
		}
		if (discr == 0) {
			ip = new Scientrace.IntersectionPoint[1];
			d1 = ((-ABCb)+Math.Sqrt(Math.Pow(ABCb, 2)-(4*ABCa*ABCc)))/(2*ABCa);
			v1 = (line.direction.toVector()*d1)+line.startingpoint.toVector();
			ip[0] = new IntersectionPoint(v1.toLocation(), this.baseIntersectionShape(v1.x, v1.y));
			return ip;
		}
		//still here? two results!
		/*d1 = ((-ABCb)+Math.Sqrt(Math.Pow(ABCb, 2)-(4*ABCa*ABCc)))/(2*ABCa);
		d2 = ((-ABCb)-Math.Sqrt(Math.Pow(ABCb, 2)-(4*ABCa*ABCc)))/(2*ABCa);*/
		d1 = ((-ABCb)+Math.Sqrt(discr))/(2*ABCa);
		d2 = ((-ABCb)-Math.Sqrt(discr))/(2*ABCa);
		v1 = (line.direction.toVector()*d1)+line.startingpoint.toVector();
		v2 = (line.direction.toVector()*d2)+line.startingpoint.toVector();
	//	Console.WriteLine("discr: "+discr+" ABCa: "+ABCa+" ABCb: "+ABCb+" ABCc: "+ABCc);
		//FOR BETTER PERFORMANCE REMOVE CHECKS BELOW
		if (!line.throughLocation(v1.toLocation(), 0.00001)) {
			throw new Exception("TRFd line 1 "+v1.trico()+" avoids "+line.ToString());
			}
		if (!line.throughLocation(v2.toLocation(), 0.00001)) {
			v2 = v1;
//			Console.WriteLine("discr: "+discr);
//			throw new Exception("TRFd line 2 "+v1.trico()+" avoids "+line.ToString()+" contrairy to "+v1.trico());
			}

		ip = new Scientrace.IntersectionPoint[2];
		ip[0] = new IntersectionPoint(v1.toLocation(), this.baseIntersectionShape(v1.x, v1.y));
		ip[1] = new IntersectionPoint(v2.toLocation(), this.baseIntersectionShape(v2.x, v2.y));
		return ip;
	}



	public Scientrace.FlatShape2d baseIntersectionShape(double x, double y) {
	
		//return new Scientrace.FlatShape2d(new Plane(new Location(0,0,0), NonzeroVector.x1vector().negative(), NonzeroVector.z1vector()));
		Scientrace.Plane refplane = new Scientrace.Plane(new Scientrace.Location(x,y,this.c*Math.Pow(x,2)*Math.Pow(y,2)),
			                            new Scientrace.UnitVector(1,0,2*c*x),
			                            new Scientrace.UnitVector(0,1,2*c*y));
		//Console.WriteLine("Mirror plane ("+x+", "+y+") equals: "+refplane.getNorm().trico());
		return new Scientrace.FlatShape2d(refplane);
		}

	/// <summary>
	/// Generates a new Location "where in space" the parabolic mirror concentrates parallel 
	/// light under the right angle towards.
	/// </summary>
	/// <returns>
	/// A <see cref="Location"/>
	/// </returns>
	public Location getConcentrationPoint() {
		return (this.zaxis*(1/(4*this.c))).toLocation()+this.loc;
		}

	public Location getBaseConcentrationPoint() {
		return (new Location(0,0,1/(4*this.c)));
		}


	public override string exportX3D(Scientrace.Object3dEnvironment env) {
		int steps = this.exportX3Dgridsteps;
		Scientrace.AbstractGridBorder cborder = this.cborder;
//		Scientrace.VectorTransform cbordertrf = cborder.getTransform();
		//Generate a rotating grid!
		Scientrace.VectorTransform cbordertrf = cborder.getGridTransform(this.zaxis);
		//Console.WriteLine("CBTRFcp: "+cbordertrf.ToCompactString());
			/* 
			 * stloc: starting location actually representing the center of the border,
			 * from there in the orthonormal direction of this border the collision points with 
			 * the parabolic mirror will be found (from -radius to +radius in both transform-directions
			 */
		Scientrace.Location stloc = cborder.getOrthoStartCenterLoc();// - cborder.getOthoDirection();
		Scientrace.IntersectionPoint[] iparr, iparre, iparrs;
		Scientrace.Intersection cintr,eintr, sintr;
			/*
			 * eloc is the location "east" of the current node, sloc "south" for drawing a grid
			 * of course are terms east and south symbolic
			 */
		Scientrace.Location eloc, sloc;
		Scientrace.Line cline, eline, sline;
		//double r = cborder.getRadius();
		double r1 = cborder.getURadius();
		double r2 = cborder.getVRadius();
			
		Scientrace.Vector v1 = cbordertrf.u.tryToUnitVector().toVector();
		Scientrace.Vector v2 = cbordertrf.v.tryToUnitVector().toVector();;
		string retstr = "";
		Scientrace.X3DGridPoint concentrationpoint = new Scientrace.X3DGridPoint(env, this.getConcentrationPoint(), null, null);
		retstr = concentrationpoint.x3DSphere(env.radius/1000, "1 0 1", 0.5);
		retstr += concentrationpoint.x3DLineTo(this.loc, "1 0 1 1");
		for (double ix = 0.5; ix < steps; ix++) {
			for (double iy = 0.5; iy <= steps; iy++) {
				/* Drawing a grid of lines in direction "border.orthodirection" to ParabolicMirror,
				 * at every intersection a gridpoint is located. "cline" are those ortho-dir lines */
				cline = new Scientrace.Line(((stloc-(v1*r1)-(v2*r2))+(v1*(r1*2*(ix/steps)))+(v2*(r2*2*(iy/steps)))).toLocation(),
											cborder.getOrthoDirection());
//											cborder.directionlength.toUnitVector());
				/* USE THE "checkborder = false" function below to always 
				 * show the grid-points, also outside borders
				 * iparr is the IntersectionPoint at the Parabolic Mirror for the current ix/iy iteration */
				iparr = this.realIntersections(cline, true);
/* DEBUG INFO	foreach (IntersectionPoint ip in iparr) {
					if (ip!=null) {
						Console.WriteLine("IP AT: "+ip.ToString());	
						} else {
						Console.WriteLine("NO IP FROM: "+((stloc-(v1*r)-(v2*r))+(v1*(r*2*(ix/steps)))+(v2*(r*2*(iy/steps))))
							                  +" AND "+cborder.getOrthoDirection());
						}
					}*/
				eline = new Scientrace.Line(((stloc-(v1*r1)-(v2*r2))+(v1*(r1*2*((ix+1)/steps)))+(v2*(r2*2*(iy/steps)))).toLocation(),
											cborder.getOrthoDirection());
//											cborder.directionlength.toUnitVector());
				iparre = this.realIntersections(eline, true);
				//defining "east" neighbour
				eintr = new Intersection(eline,iparre, this);
				if (eintr.intersects) {
						eloc = eintr.enter.loc;
					} else {
						eloc = null;
					}
				sline = new Scientrace.Line(((stloc-(v1*r1)-(v2*r2))+(v1*(r1*2*((ix)/steps)))+(v2*(r2*2*((iy+1)/steps)))).toLocation(),
											cborder.getOrthoDirection());
//											cborder.directionlength.toUnitVector());
				iparrs = this.realIntersections(sline, true);
				//defining "south" neighbour
				sintr = new Intersection(sline, iparrs, this);
					if (sintr.intersects) {
						sloc = sintr.enter.loc;
					} else {
						sloc = null;
					}
				/* "central" point
				 * where does line "cline" with intersections "iparr" intersect this object?
				 */
				cintr = new Intersection(cline, iparr, this, true);
				if (cintr.intersects) { //add existing gridpoints
					if (this.x3dgridspheres) {
						retstr = retstr + new X3DGridPoint(env, cintr.enter.loc, eloc, sloc).exportX3D();
						} else {
						retstr = retstr + new X3DGridPoint(env, cintr.enter.loc, eloc, sloc).exportX3DnosphereRGBA("0 0 1 1");
						}
					}
				}
			}
			//Console.WriteLine("!!!"+retstr);
		return retstr+cborder.exportX3D(env);
	}

	public override Intersection intersects(Trace trace) {
		return this.realIntersections(trace, true);
	}
	
}
}
