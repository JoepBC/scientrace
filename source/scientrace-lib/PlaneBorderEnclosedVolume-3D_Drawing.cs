// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {
public partial class PlaneBorderEnclosedVolume:EnclosedVolume {

	public override string exportX3D(Object3dEnvironment env) {
		List<ObjectLinePiece> olps = this.getOLPBorderEdges();
		return LinePiece.drawLinePiecesXML(olps);
		}


	public List<ObjectLinePiece> getOLPBorderEdges() {
		return this.toOLP(this.getBorderEdges(), "0 1 1 1");
		}

	public List<LinePiece> getBorderEdges() {
		int AN_AWFUL_LOT_OF_BORDERS = 50;
		List<LinePiece> lpList = new List<LinePiece>();
		
		if (this.borders.Count >= AN_AWFUL_LOT_OF_BORDERS) {
			Console.WriteLine("WARNING: Iterating through "+this.borders.Count+"^2 (="+Math.Pow(this.borders.Count,2)+") borders");
			Console.Write("Parsing border: ");
			}
		// Iterate through all borders
		for (int iBorder = 0; iBorder < this.borders.Count; iBorder++) {
			if (this.borders.Count >= AN_AWFUL_LOT_OF_BORDERS)
				Console.Write(iBorder.ToString()+" - ");
			// Find interactions with all remaining borders
			for (int iOtherBorder = iBorder; iOtherBorder < this.borders.Count; iOtherBorder++) {
				// But only as long these borders are not parallel
				if (this.borders[iBorder].isParallelTo(this.borders[iOtherBorder]))
					continue;
				
				// Calculate the line where both iterated border-planes intersect
				Scientrace.Line sectLine = this.borders[iBorder].getIntersectionLineWith(this.borders[iOtherBorder]);
				if (sectLine == null)
					continue;
				// Build a list of location nodes 
				List<Scientrace.Location> validNodes = new List<Scientrace.Location>();

				// Find the edges  
				for (int iLimitBorder = 0; iLimitBorder < this.borders.Count; iLimitBorder++) {
					if ((iLimitBorder==iBorder)||(iLimitBorder==iOtherBorder))
						continue; //planes can never limit a line that's within the plane
					if (Math.Abs(this.borders[iLimitBorder].getNormal().dotProduct(sectLine.direction)) < MainClass.SIGNIFICANTLY_SMALL)
						continue; // line is within the plane, does not intersect. Continue for iLimitBorder loop
					Scientrace.Location node = this.borders[iLimitBorder].lineThroughPlane(sectLine);
					if (node == null) {
						Console.WriteLine("WARNING, NODE=null: "+this.borders[iLimitBorder].ToString()+" vs. "+sectLine.ToString());
						continue; // somehow the line didn't go through the plane in the end...
						}
					bool checkNode = true;
					for (int iCheckNodeBorder = 0; iCheckNodeBorder < this.borders.Count; iCheckNodeBorder++) {
						if ((iCheckNodeBorder==iBorder)||(iCheckNodeBorder==iOtherBorder)||(iCheckNodeBorder==iLimitBorder))
							continue;
						if (!this.borders[iCheckNodeBorder].contains(node, MainClass.SIGNIFICANTLY_SMALL))
							checkNode = false;
						} // end for iCheckNodeBorder
					if (checkNode) {
						node.addToListIfNew(validNodes);
						}
					} // end for iLimitBorder
					if (validNodes.Count == 2) 
						lpList.Add(new LinePiece(validNodes[0], validNodes[1]));
					// else //SHH this one, an enclosedvolume can also exist from parallel borders, e.g. a cube.
					if (validNodes.Count > 2) {
						Console.WriteLine("WARNING: "+validNodes.Count+" validNodes found."+MainClass.SIGNIFICANTLY_SMALL);
						foreach (Location aNode in validNodes) {
												Console.WriteLine("anode: "+aNode.ToString());
						}}
				} // end for iOtherBorder
			} // end for iBorder
		return lpList;
		}

}}
