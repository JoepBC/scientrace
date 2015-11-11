// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
namespace Scientrace {
	
public class CircularFresnelPrism : Object3dCollection {
	
	public Scientrace.Location loc;
	public Scientrace.NonzeroVector surfacev1, surfacev2, surfacev3;
	public Scientrace.MaterialProperties fresnelMaterial;
	public double largeAngle, relative_angle, total_prism_height;
	public int teethCount;
	public int divisioncount = 12;
	public Object3dCollection[] division; //firsthalf, secondhalf;
	//public Scientrace.RectangularPrism dummyRect;
		
	/// <summary>
	/// The CircularFresnelPrism is developed specifically for the SunCycle system but can be used
	/// for different purposes. It generates a filled Object3dCollection with 
	/// </summary>
	/// <param name="parent">
	/// A <see cref="Object3d"/>
	/// </param>
	/// <param name="surroundingsproperties">
	/// A <see cref="MaterialProperties"/>
	/// </param>
	/// <param name="loc">
	/// A <see cref="Location"/>
	/// </param>
	/// <param name="surfacev1">
	/// A <see cref="NonzeroVector"/> orthogonal to the prism-teeth "length"-direction.
	/// </param>
	/// <param name="surfacev2">
	/// A <see cref="NonzeroVector"/>
	/// </param>
	/// <param name="surfacev3">
	/// The Z axis </param>
	/// <param name="teethCount">
	/// A int representing the number of teeth over the fresnel surface.
	/// </param>
	/// <param name="fresnelMaterial">
	/// A <see cref="MaterialProperties"/>
	/// </param>
	
	public CircularFresnelPrism(Object3dCollection parent, MaterialProperties surroundingsproperties,
		Location loc, NonzeroVector surfacev1, NonzeroVector surfacev2, NonzeroVector zaxisheight,
		double largeAngle_rad, double shortAngle_rad, int teethCount,
		MaterialProperties fresnelMaterial) 
		: base(parent, surroundingsproperties) {

		//check vectors
		try{surfacev1.toUnitVector();}
			catch { throw new ZeroNonzeroVectorException("Fresnel Prism surface 1 has length 0"); }
		try{surfacev2.toUnitVector();}
			catch { throw new ZeroNonzeroVectorException("Fresnel Prism surface 2 has length 0"); }
		try{zaxisheight.toUnitVector();}
			catch { throw new ZeroNonzeroVectorException("Z-axis height has length 0"); }

		//set attributes
		this.loc = loc;
		this.surfacev1 = surfacev1;
		this.surfacev2 = surfacev2;
		this.surfacev3 = zaxisheight;
			
		this.largeAngle = largeAngle_rad;
		this.relative_angle = Math.PI - (shortAngle_rad + largeAngle_rad);
		this.teethCount = teethCount;
		this.fresnelMaterial = fresnelMaterial;

		//create "divisions" that organises the Fresnel-zones in segments in order to boost performance.
		this.division = new Object3dCollection[this.divisioncount];
		for (int idiv = 0; idiv<this.divisioncount; idiv++) {
			this.division[idiv] = new Object3dCollection(this, surroundingsproperties);	
			this.division[idiv].virtualCollection = true;
			this.division[idiv].tag = "fresnel_div_"+idiv;				
			}

		this.addElements();
		this.dummyborder = new RectangularPrism(null, this.materialproperties, loc-(surfacev1+surfacev2),
			surfacev1*2, surfacev2*2, surfacev3.normalized()*this.total_prism_height);
	}

	public void addElements() {
			
		/* see sine_rule_applied.svg */
		double alpha;
		double beta, gamma;
		alpha = this.largeAngle;
		gamma = this.relative_angle;
		beta = Math.PI - (alpha+gamma);
		/* vteethheight is the vector of the triangularprism that points to the top (see vector "h" below)
		 *       /\
		 *      /  \
		 *     /    \
		 *    /      \
		 *    \      /|
		 * l = \  h=/ |
		 *      \  /  |
		 *       \/___|
		 *      w = ^
		 * since uvLargeAngle // b... */

		Scientrace.UnitVector v1u = this.surfacev1.toUnitVector();
		Scientrace.UnitVector v3u = this.surfacev3.toUnitVector();
		UnitVector uvLargeAngle = null;
		try {
		uvLargeAngle = (v3u.toVector()*Math.Sin(alpha) + //"z" axis
											 v1u.toVector()*Math.Cos(alpha)).tryToUnitVector();  //"x" axis
			} catch (Exception e) {
				throw new ZeroNonzeroVectorException("Large angle for FresnelPrism has a calculated length 0 \n"+e.Message);
			}
		/* the short angle direction vector below is not used. Commented for possibel relevant future use.
		 Scientrace.UnitVector uvShortAngle = (v1u*Math.Sin(this.largeAngle+this.relative_angle) + //"x" axis
											 v3u*Math.Cos(this.largeAngle+this.relative_angle)).tryToUnitVector();  //"z" axis */

		double teethwidth = 2*this.surfacev1.length/Convert.ToDouble(this.teethCount); //2* because v1 represents a radius, not diameter.
		Scientrace.NonzeroVector vteethheight = null;
		try {
			vteethheight = uvLargeAngle * teethwidth * ((Math.Sin(beta))/(Math.Sin(gamma)));
			} catch (Exception e) {
			throw new ZeroNonzeroVectorException("Teeth height for FresnelPrism has a calculated length 0 \n"+e.Message);
			}

		Scientrace.Location v1base = (this.loc - this.surfacev1.toLocation());

		double vratio = this.surfacev2.length / this.surfacev1.length;
		Scientrace.NonzeroVector vteethwidth = this.surfacev1.toUnitVector()*teethwidth;
			
		/* Adding teeth to collection, pointing in v3 direction */
		for (int itooth = 0; itooth < this.teethCount; itooth++) {
			double distfromcenter = (Math.Abs((0.5*teethCount)-(itooth+0.5))-0.5)/(0.5*teethCount);
			//vteethlength is the equiv for vector c in sine_rule_applied.svg and w in ascii scetch above.
			Scientrace.NonzeroVector vteethlength = //*2 because real length is double the length from the center-line.
				(this.surfacev2*(Math.Sqrt(1-Math.Pow(distfromcenter, 2))*vratio))*2;
			
			//	Console.WriteLine(itooth+" @ "+((this.divisioncount*itooth)/this.teethCount));
			Scientrace.Object3d tri = new Scientrace.TriangularPrism(
					this.division[(this.divisioncount*itooth)/this.teethCount], this.fresnelMaterial,
					((v1base/*+this.surfacev3*/) + (v1u.toVector()*itooth*teethwidth) - (vteethlength*0.5)).toLocation(),
					vteethwidth, vteethheight, vteethlength);

			tri.tag = "prismdent_"+itooth;
			}
		/* Adding "top layer, circular prism, on which the teeth are attached. Height surfacev3 */
		/* since the top circularprism was moved, the shift (+this.surfacev3) at the adding of the teeth was
		 * also commented out */

		//Scientrace.CircularPrism topcircle = new Scientrace.CircularPrism(this, this.fresnelMaterial, loc, this.surfacev1, this.surfacev2, this.surfacev3);
		//adding to second half, true that's a bit redundant...
		//this.secondhalf.addObject3d(topcircle);

		/* Define the total height of the circle and the teeth in the surfacev3-direction for the environment
		 * to request for total size purposes */
			
		this.total_prism_height = //this.surfacev3.length + //OBSOLETE SINCE TOP CIRCULARPRISM HAS BEEN REMOVED
				Math.Abs(vteethheight.dotProduct(this.surfacev3.toUnitVector()));

		//Setting boundaries for first and second half collections:
		if (this.teethCount < this.divisioncount) {
			throw new ArgumentOutOfRangeException("Lees teeth("+this.teethCount+") for prism than divisions("+this.divisioncount+")");
			}
		for (int idiv = 0; idiv<this.divisioncount; idiv++) {

			double divstart = Math.Ceiling((double)(idiv*teethCount) /(double)this.divisioncount);
			double divend = Math.Ceiling((double)((1+idiv)*teethCount) /(double)this.divisioncount);
					//Math.Ceiling(double((idiv+1)*teethCount) / this.divisioncount);
			//Console.WriteLine("idiv: "+idiv+" divstart:"+divstart+" divend:" +divend + " tc:"+teethCount+" dc:"+divisioncount);
			double divstartfrac = (double)divstart / (double)teethCount;
			double divendfrac = (double)divend / (double)teethCount;
			this.division[idiv].dummyborder = new RectangularPrism(null, this.materialproperties, 
				loc-surfacev1-surfacev2+(surfacev1.toVector()*divstartfrac*2), //location
				(surfacev1*(divendfrac-divstartfrac)*2), //width or length
				surfacev2*2, //length or width
				surfacev3.normalized()*this.total_prism_height //height
				);
			//Console.WriteLine("DIV:"+idiv+" has "+this.division[idiv].objects.Count+" children");
			}
		}


	}}