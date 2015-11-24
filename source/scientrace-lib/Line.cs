// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace Scientrace {


public class Line {

	public Scientrace.Location startingpoint;
	public Scientrace.UnitVector direction;


	public Line() {
		}

	public Line(double x, double y, double z, double dirx, double diry, double dirz) {
		this.startingpoint = new Scientrace.Location(x, y, z);
		this.direction = new Scientrace.UnitVector(dirx, diry, dirz);
		}

	public Line(Scientrace.Location startloc, Scientrace.UnitVector dir) {
		if (startloc == null)
			throw new ArgumentNullException("startloc");
		if (dir == null)
			throw new ArgumentNullException("dir");
		this.startingpoint = new Location(startloc);
		this.direction = new UnitVector(dir); // dir.copy();
		}

	public Line(Line copyLine) {
		this.startingpoint = new Location(copyLine.startingpoint);
		this.direction = new UnitVector(copyLine.direction);
		}

	/*
	 * Overloaded operators
	 */
	public static Scientrace.Line operator -(Scientrace.Line line, Scientrace.Location loc) {
		//the - operator for a line with a location works on "startingpoint" only, not on direction of the line
		return new Scientrace.Line(line.startingpoint - loc, line.direction);
		}
	public static Scientrace.Line operator +(Scientrace.Line line, Scientrace.Location loc) {
		//the + operator for a line with a location works on "startingpoint" only, not on direction of the line
		return new Scientrace.Line(line.startingpoint + loc, line.direction);
		}

	public override string ToString ()	{
		return "[line @"+this.startingpoint.ToString()+" ^"+this.direction.ToString()+"]";
		}

	// Place the startingpiont "distance" back in space along the direction of the line.
	public void rewind(double distance) {
		this.startingpoint = this.startingpoint - (this.direction.toVector()*distance);
		}

	public bool throughLocation(Scientrace.Location aLoc, double maxdiff) {
		//Console.WriteLine("Gib mich:"+aLoc.trico());
		Scientrace.Location tloc = aLoc - this.startingpoint;
		double dist = tloc.dotProduct(this.direction);
		Scientrace.Location compareloc;
		//must convert to Vector below in case dist == 0
		compareloc = (this.direction.toVector()*dist).toLocation();
		if (compareloc.distanceTo(tloc)>maxdiff) {
			//Console.WriteLine("TEVEEL:"+compareloc.distanceTo(tloc));
			//Console.WriteLine("Op: "+this.startingpoint+" afstand: "+dist);
			}
		return (compareloc.distanceTo(tloc)<maxdiff);
		}
		
	public Scientrace.Location intersects(Scientrace.Parallelogram par) {
		return par.atPath(this);
		}

	public Line copy() {
		return new Line(this.startingpoint.copy(), this.direction.copy());
		}
		
	public Line reflectOnSurface(Scientrace.NonzeroVector norm, double offset) {

		//copy constructor
		Scientrace.Line retline = new Scientrace.Line(this);

		//the direction of the reflected line
		retline.direction = this.direction.reflectOnSurface(norm); 

		//after reflecting "jump" wavelength ahead not to keep reflecting at same point.
		retline.startingpoint = (this.startingpoint+retline.direction.toVector()*offset).toLocation();

		return retline;
		}


//REFLECTAT and REFLECTAROUND method moved to Trace



}
}
