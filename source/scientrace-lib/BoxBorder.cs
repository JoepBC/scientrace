// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections;

namespace Scientrace {


public class BoxBorder : AbstractGridBorder {
	public Scientrace.NonzeroVector width, height, length;
	//length inherited from Prism
	public Scientrace.Location loc;

	private ArrayList pgrams = new ArrayList();
/*	public Scientrace.Object3d parentObject;*/
		
	public BoxBorder(Scientrace.Location loc, Scientrace.NonzeroVector width, Scientrace.NonzeroVector height, Scientrace.NonzeroVector length /*, Scientrace.Object3d parentObject*/) {
		this.loc = loc;
		this.width = width;
		this.length = length;
		this.height = height;
/*		this.parentObject = parentObject; */
		
		//defining the sides of the box
		this.pgrams.Add(new Parallelogram(loc, width, height));
		this.pgrams.Add(new Parallelogram(loc+length, width, height));
		this.pgrams.Add(new Parallelogram(loc, height, length));
		this.pgrams.Add(new Parallelogram(loc+width, height, length));
		this.pgrams.Add(new Parallelogram(loc, width, length));
		this.pgrams.Add(new Parallelogram(loc+height, width, length));
	}

	public void rotateAboutY(double angle, Scientrace.Location center) {
		this.loc = ((this.loc - center).rotateAboutY(angle)).toLocation()+center;
		this.width = this.width.rotateAboutY(angle);
		this.length = this.length.rotateAboutY(angle);
		this.height = this.height.rotateAboutY(angle);
		this.trf = null;
		}

	public override UnitVector getOrthoDirection () {
		return length.toUnitVector();
	}


	public override Scientrace.Location getOrthoStartCenterLoc() {
		//length property is removed as it equals the orthoDirection
		return (this.loc + (this.width*0.5) + (this.height*0.5));
		}

	public override Scientrace.Location getCenterLoc() {
		//return this.loc;
		return this.loc + (this.width*0.5) + (this.length*0.5) + (this.height*0.5);
		}
		
		
	public override Scientrace.VectorTransform getGridTransform(UnitVector nz) {
		NonzeroVector u, v, w;
		u = this.width;
		v = this.height;
		w = this.length;
		while (Math.Abs(nz.dotProduct(u.normalized())) > 0.99) {
			u = v;
			v = w;
			w = w.vectorDance().tryToUnitVector();
			}
		while (Math.Abs(nz.dotProduct(v.normalized())) > 0.99) {
			v = w;
			w = w.vectorDance().tryToUnitVector();
			}
		/*Console.WriteLine("NEW MAT:"+
			                  u.tricon()+
			                  v.tricon()+
			                  nz.tricon()); */
		return new Scientrace.VectorTransform(u,v,nz);
		}
		
	public bool valuewithin(double val, double a, double b) {
		double min = Math.Min(a,b);
		double max = Math.Max(a,b);
		return ((val >= min) && (val<=max));
	}

	public override double getRadius() {
		//calculate length of "width and height square surface"-center towards corner
		return Math.Sqrt(Math.Pow(this.width.length, 2)+Math.Pow(this.height.length, 2))/2;
		}
		
	public override double getURadius() {
		//calculate length of "width and height square surface"-center towards corner
		return this.width.length*0.5;
		}

	public override double getVRadius() {
		//calculate length of "width and height square surface"-center towards corner
		return this.height.length*0.5;
		}
		
		
	public override Scientrace.VectorTransform createNewTransform() {
		// the height is the direction in which grids etc. are projected parallel towards
		return new VectorTransform(this.width, this.length, this.height);
		}

	public override bool contains (Location tryloc) {
		Scientrace.VectorTransform trf = this.getTransform();
		Scientrace.Vector trfdtryloc = trf.transform(tryloc-this.loc);
		if (!this.valuewithin(trfdtryloc.x, 0,1)) {
			return false;}
		if (!this.valuewithin(trfdtryloc.y, 0,1)) {
			return false;}
		if (!this.valuewithin(trfdtryloc.z, 0,1)) {
			return false;}
		return true;
		}

	public override string exportX3D (Scientrace.Object3dEnvironment env)	{
			string retstr = "<!-- EXPORTING BOXBORDER SQUARES -->";
			foreach (Scientrace.Parallelogram pgram in this.pgrams) {
				retstr = retstr + pgram.exportX3D(env);
				}
			retstr = retstr + "<!-- /// END OF EXPORTING BOXBORDER SQUARES -->";
			return retstr;
			}

}
}
