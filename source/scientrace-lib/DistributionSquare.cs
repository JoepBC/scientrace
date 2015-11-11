// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {

	/// <summary>
	/// Spots on an object can be copied to this DistributionSquare. It will determine whether they
	/// are within the specified square and calculate square-distributions on demand.
	/// </summary>

public class DistributionSquare {

	int order;
	int max_order;
	int count;
	double margin = 0.0001; //spots have to lie on the surface within this margin on the perpendicular axis (trf-w)
	VectorTransform trf;
	DistributionSquare tl, tr, bl, br; //top-left, top-right, bottom-left, bottom-right
	Location loc;
	NonzeroVector x, y, tx, ty;

	/// <summary>
	/// The distributionsquare is a measure for the equal-distribution of light over a surface.
	/// An entire surface is split up in quadrants out of which a measure is calculated by averaging
	/// the ratio between all quadrants and each other. Depending on the "order" each quadrant is having
	/// its own ratio calculated as well. For the weight of each order in the total is explained
	/// at the totalDistribution() method.
	/// </summary>
	/// <param name="order">
	/// A <see cref="System.Int32"/> value containing the "currently calculated order". When manually
	/// creating a new DistributionSquare the top order should always be 0. Higher orders are created
	/// internally.
	/// </param>
	/// <param name="max_order">
	/// How many orders should be calculated? A <see cref="System.Int32"/>.
	/// </param>
	/// <param name="loc">
	/// The "top left" location of the surface. A <see cref="Location"/>
	/// </param>
	/// <param name="x">
	/// The direction AND the width of the surface. A <see cref="NonzeroVector"/>
	/// </param>
	/// <param name="y">
	/// The direction AND the height of the surface. A <see cref="NonzeroVector"/>
	/// </param>
	public DistributionSquare(int order, int max_order, Location loc, NonzeroVector x, NonzeroVector y) {
		this.order = order;
		this.max_order = max_order;
		this.x = x;
		this.y = y;
		this.loc = loc;
		if (order<max_order) {
			//Console.WriteLine("Smaller "+order+"/"+max_order+", so...");
			this.tl = new DistributionSquare(order+1, max_order, loc, x*0.5, y*0.5);
			this.tr = new DistributionSquare(order+1, max_order, loc+(x*0.5).toLocation(), x*0.5, y*0.5);
			this.bl = new DistributionSquare(order+1, max_order, loc+(y*0.5).toLocation(), x*0.5, y*0.5);
			this.br = new DistributionSquare(order+1, max_order, loc+(x*0.5+y*0.5).toLocation(), x*0.5, y*0.5);
		}
		this.trf = new VectorTransform(x, y);
		this.tx = this.trf.transform(this.x);
		this.ty = this.trf.transform(this.y);
		this.checkxy();
	}

	public void checkxy() {
		if (this.tx.toLocation().distanceTo(new Location(1,0,0)) > this.margin) {
			throw new ArgumentOutOfRangeException("tx ("+this.tx+") in DistributionSquare is out of range");
			}
		if (this.ty.toLocation().distanceTo(new Location(0,1,0)) > this.margin) {
			throw new ArgumentOutOfRangeException("ty ("+this.ty+") in DistributionSquare is out of range");
			}
		}
	public void increaseCount() {
		this.count = this.count+1;
		}

	public bool addSpot(Location aLoc) {
		Location shiftloc = aLoc - this.loc;
		Location tloc = this.trf.transform(shiftloc);
		if (Math.Abs(tloc.z) > this.margin) {
			return false;
			}
		if ((tloc.x < 0) || (tloc.x > 1) ||
			(tloc.y < 0) || (tloc.y > 1)) {
			return false; //tloc doesn't lie in the plane
			}
		//tloc lies in the plane.
		this.increaseCount();
		if (this.order >= this.max_order) {
			return true;
			}
		if (tloc.y < 0.5) { //top
			if (tloc.x < 0.5) {	this.tl.addSpot(aLoc);	} else { this.tr.addSpot(aLoc); }
			} else { //bottom
			if (tloc.x < 0.5) {	this.bl.addSpot(aLoc);	} else { this.br.addSpot(aLoc); }
		}
		return true;
	}

	public double ratio(int a, int b) {
		if (a>b) {
			int t = a;
			a = b;
			b = t;
			}
			// a < b
			if (a == 0) {
				return 0;
			}
			double retval = (double)a/b;
			//Console.WriteLine("o"+this.order+"/"+this.max_order+": "+retval);
			return retval;
		}

	public double currentOrderDistribution() {
		if (this.order >= this.max_order) {
			throw new Exception("A bottom level square ("+this.order+"/"+this.max_order+") has no distribution");
			}
		//Console.WriteLine("CoD: o"+this.order+"/"+this.max_order);
		return (
			this.ratio(this.tl.count, this.tr.count) + //horizontal ratios
			this.ratio(this.bl.count, this.br.count) +
			this.ratio(this.tl.count, this.bl.count) + //vertical ratios
			this.ratio(this.tr.count, this.br.count) +
			this.ratio(this.tl.count, this.br.count) + //diagonal ratios
			this.ratio(this.tr.count, this.bl.count)
			) / 6;
		}

	public double totalDistribution() {
		if ((this.order+1) >= this.max_order) {
			return this.currentOrderDistribution(); }
		/* every order should have 50% shares, 
		 * 25% shares from its direct childs
		 * and a weighted 25% of all their childs.
		 * The bottom child should have a 100%/3 share compared to it's parent. */
		
		if (this.order == this.max_order) { //bottom child:
		//Console.WriteLine("BOTTOM CHILD"+this.order);
		return (
			((2*this.currentOrderDistribution()) /*66.7% */ + ((
			this.tl.totalDistribution() + /*33.3% */
			this.tr.totalDistribution() +
			this.bl.totalDistribution() +
			this.br.totalDistribution()
			) / 4 ))/3);
		}
		//regular childs
		//Console.WriteLine("REGULAR CHILD"+this.order+"/"+this.max_order);

		double retval = this.currentOrderDistribution() + ((
			this.tl.totalDistribution() +
			this.tr.totalDistribution() +
			this.bl.totalDistribution() +
			this.br.totalDistribution()
			) / 4.0 ) /2.0;
		//Console.WriteLine("RETTING: "+retval);
		return retval;
		}
		

	public void exportXML(string filename) {
		/*XmlTextWriter writer = new XmlTextWriter(filename, null);
    	 // Use indenting for readcd ..ability.
     	writer.Formatting = Formatting.Indented;
		writer.WriteStartElement("foo");
 		writer.WriteAttributeString("bar", "bla");
		writer.WriteEndElement();*/	
		}



}
}