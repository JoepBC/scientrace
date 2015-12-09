// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;


namespace Scientrace {


// A opportunity-struct (mainly) for drawing purposes
public struct ObjectLinePiece {
	public LinePiece lp;
	public Object3d o3d;
	public string col;
	}


public class LinePiece : Line {

	public Location endingpoint;

	public LinePiece(double l1x, double l1y, double l1z, double l2x, double l2y, double l2z) {
		this.init(new Scientrace.Location(l1x, l1y, l1z), new Scientrace.Location(l2x, l2y, l2z));
		}

	public LinePiece(Scientrace.Location startloc, Scientrace.Location endloc) {
		this.init(new Location(startloc), new Location(endloc));
		}
	
	public void init(Scientrace.Location startloc, Scientrace.Location endloc) {
		if (startloc.distanceTo(endloc) <= 0) {
			throw new ArgumentOutOfRangeException("ERROR: Startloc ("+startloc.ToString()+") and endloc ("+endloc.ToString()+") are equal for linepiece, no line nor direction possible");
			}
		this.startingpoint = startloc;
		this.endingpoint = endloc;
		this.calculateDirection();
		}
				
	public void calculateDirection() {
		this.direction = (this.startingpoint == this.endingpoint)?null:(this.startingpoint-this.endingpoint).tryToUnitVector();
		}
	
	public Line toLine() {
		this.calculateDirection();
		return new Line(this.startingpoint, this.direction);
		}

	public List<LinePiece> sliceIfBetween(Location aLocation) {
		if ( // is the location further away from the start or end than the length of thice LinePiece? Do not slice!
			(this.startingpoint.distanceTo(aLocation) >= this.getLength()) || 
			(this.endingpoint.distanceTo(aLocation) >= this.getLength())
			) {
			List<LinePiece> retList = new List<LinePiece>();
			retList.Add(this);
			return retList;
			}
		// Any other condition -> Slice the line!
		//Console.WriteLine("SLICING FAIRY PRINCESS");
		List<LinePiece> retlist = this.slice(aLocation);
		/*Console.Write("SLICING Loc: "+aLocation+" to: ");
		foreach (LinePiece anLP in retlist) {
			Console.Write(" lp: "+anLP.ToString());
			}
		Console.WriteLine("!");*/
		return retlist;
//		return this.slice(aLocation);
		}


	public List<LinePiece> slice(Location aLocation) {
		List<LinePiece> retList = new List<LinePiece>();
		retList.Add(new LinePiece(this.startingpoint, aLocation));
		retList.Add(new LinePiece(aLocation, this.endingpoint));
		return retList;
		}

	public double getLength() {
		return this.startingpoint.distanceTo(this.endingpoint);
		}

	public Location getCenter() {
		return (this.startingpoint+this.endingpoint)*0.5;
		}

	static public string drawLinePiecesXML(List<ObjectLinePiece> linePieces) {
		System.Text.StringBuilder retstr = new System.Text.StringBuilder(5000);
		foreach (ObjectLinePiece olp in linePieces) {
			retstr.Append(X3DGridPoint.get_RGBA_Line_XML(olp.lp, olp.col));
			}
		return retstr.ToString();
		}


	static public string drawLinePiecesXML(List<LinePiece> linePieces, string colour) {
		System.Text.StringBuilder retstr = new System.Text.StringBuilder(5000);
		foreach (LinePiece lp in linePieces) {
			retstr.Append(X3DGridPoint.get_RGBA_Line_XML(lp, colour));
			}
		return retstr.ToString();
		}

	/*
	 * Overloaded operators
	 */
	public static Scientrace.LinePiece operator -(Scientrace.LinePiece linePiece, Scientrace.Location loc) {
		//the - operator for a line with a location works on "startingpoint" only, not on direction of the line
		return new Scientrace.LinePiece(linePiece.startingpoint - loc, linePiece.endingpoint+loc);
		}
	public static Scientrace.LinePiece operator +(Scientrace.LinePiece linePiece, Scientrace.Location loc) {
		//the + operator for a line with a location works on "startingpoint" only, not on direction of the line
		return new Scientrace.LinePiece(linePiece.startingpoint + loc, linePiece.endingpoint+loc);
		}

	public override string ToString ()	{
		return "[linepiece: "+this.startingpoint.ToString()+" > "+this.endingpoint.ToString()+"]";
		}


	public static bool operator ==(Scientrace.LinePiece lp1, Scientrace.LinePiece lp2) {
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(lp1, lp2)) {
			return true;
			}
		// If one is null, but not both, return false.
		if (((object)lp1 == null) || ((object)lp2 == null)) {
			return false;
			}
		//poking whether state v1 equals state v2
		return (
			((lp1.startingpoint == lp2.startingpoint) && (lp1.endingpoint == lp2.endingpoint)) || 
			((lp1.startingpoint == lp2.endingpoint) && (lp1.endingpoint == lp2.startingpoint))
			) ;
		}

	public static bool operator !=(Scientrace.LinePiece lp1, Scientrace.LinePiece lp2) {
		return !(lp1 == lp2);
		//This will call to operator == simple way to implement !=
		}

	public override bool Equals(object o) {
		return this == (Scientrace.LinePiece)o;
		}

	public override int GetHashCode() {
		return base.GetHashCode();
		}
		
	}}

