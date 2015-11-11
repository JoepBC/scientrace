// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections;

namespace Scientrace {


public class CombinedBorder : AbstractGridBorder  {

	public AbstractGridBorder primaryborder;
	public ArrayList additionalBorders = new ArrayList();

	public CombinedBorder(AbstractGridBorder primaryborder) {
		this.primaryborder = primaryborder;
		}

	public override VectorTransform createNewTransform() {
		return this.primaryborder.createNewTransform();
		}

	public void addBorder(AbstractGridBorder aBorder) {
		this.additionalBorders.Add(aBorder);
		}

	public override bool contains (Location loc) {
		/* return true only if true for all containing borders */
		bool ret;
		ret = this.primaryborder.contains(loc);
		foreach (AbstractGridBorder ab in this.additionalBorders) {
			ret = (ret && ab.contains(loc));
			}
		return ret;
		}

	public override VectorTransform getTransform ()	{
		return this.primaryborder.getTransform();
		}

	public override double getRadius () {
		/* return largest value */
		double ret;
		ret = this.primaryborder.getRadius();
		foreach (AbstractGridBorder ab in this.additionalBorders) {
			ret = Math.Max(ret, ab.getRadius());
			}
		return ret;
		}


	public override VectorTransform getGridTransform (UnitVector griddirection) {
		return this.primaryborder.getGridTransform(griddirection);
		}

	public override Location getCenterLoc () {
		return this.primaryborder.getCenterLoc();
		}

 	public override UnitVector getOrthoDirection (){
		return this.primaryborder.getOrthoDirection();
		}

 	public override Location getOrthoStartCenterLoc(){
		return this.primaryborder.getOrthoStartCenterLoc();
		}



	}

}