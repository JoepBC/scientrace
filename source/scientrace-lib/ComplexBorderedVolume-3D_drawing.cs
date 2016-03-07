// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace Scientrace {

public partial class ComplexBorderedVolume : EnclosedVolume {

	public double ERROR_MARGIN = 1E-10;
		
	public override string exportX3D(Object3dEnvironment env) {
		// Step 1: Collect base linepieces that would draw the basic structure
		List<ObjectLinePiece> baseLinePieces = new List<ObjectLinePiece>();		
		foreach (PlaneBorderEnclosedVolume aSubVolume in this.subVolumes) {
			baseLinePieces.AddRange(aSubVolume.getOLPBorderEdges());
			}

		// Step 2: Slice all linepieces that are intersected by other subVolumes
		List<ObjectLinePiece> slicedLinePieces = new List<ObjectLinePiece>();
		foreach (ObjectLinePiece anOLP in baseLinePieces) {
			// Distances/linepieces smaller than "significance" are considered insignificant ignored.
			if(anOLP.lp.getLength() < MainClass.SIGNIFICANTLY_SMALL)
				continue;
			List<ObjectLinePiece> olpChunks = new List<ObjectLinePiece>();
			List<ObjectLinePiece> newOLPChunks = new List<ObjectLinePiece>();
			
			olpChunks.Add(anOLP);
			foreach (PlaneBorderEnclosedVolume aSubVolume in this.subVolumes) {
				newOLPChunks = new List<ObjectLinePiece>();	//create empty new list
				foreach (ObjectLinePiece anOLPChunk in olpChunks) {
					newOLPChunks.AddRange(aSubVolume.sliceObjectLinePiece(anOLPChunk));
					} // end oldChunks
				olpChunks = newOLPChunks;
				} // end subVolumes
			
			slicedLinePieces.AddRange(newOLPChunks);
			} // end anOLP loop
		

		List<ObjectLinePiece> markedSlices = this.markInsideOutsideOLPs(this.cleanupLinePieceList(slicedLinePieces));

		Console.WriteLine("ComplexBorder gridline count: "+markedSlices.Count+" out of "+slicedLinePieces.Count);
		double foo = 0;
		foreach (ObjectLinePiece anOLP in markedSlices) {
			//foo += env.radius/2000;
			anOLP.lp.startingpoint.x= anOLP.lp.startingpoint.x + foo;
			anOLP.lp.endingpoint.x= anOLP.lp.endingpoint.x + foo;
			anOLP.lp.startingpoint.y= anOLP.lp.startingpoint.y + foo;
			anOLP.lp.endingpoint.y= anOLP.lp.endingpoint.y + foo;
			}

		// draw the linepieces after having checked and marked what's inside and outside
		//return LinePiece.drawLinePiecesXML(this.markInsideOutsideOLPs(baseLinePieces)); // <-- show non-sliced pieces only.
		string line_pieces_xml = LinePiece.drawLinePiecesXML(markedSlices);
		if (line_pieces_xml.Length < 1)
			Console.WriteLine("WARNING: the volume {"+this.tag+"} cannot be drawn.");
		return "<!-- ComplexVolume: "+this.tag+" -->"+line_pieces_xml+"<!-- END OF ComplexVolume: "+this.tag+" -->";
		}
		

	public bool olpDictHasLine(Dictionary<LinePiece, ObjectLinePiece> probeDict, LinePiece aLinePiece, double vectorErrorMargin) {
		if (aLinePiece.startingpoint.distanceTo(aLinePiece.endingpoint) < vectorErrorMargin) {
			//Console.WriteLine("`Zero length` LinePiece found. What does not really exist may be assumed always present, therefor returning true.");
			return true;
			}
		Scientrace.Location[] locs = {aLinePiece.startingpoint, aLinePiece.endingpoint};
		foreach (LinePiece dictLP in probeDict.Keys) {
			for (int iLoc = 0; iLoc <= 1; iLoc++) {
				if (dictLP.startingpoint.distanceTo(locs[iLoc]) < vectorErrorMargin)
					if (dictLP.endingpoint.distanceTo(locs[1-iLoc]) < vectorErrorMargin) {
						//Console.WriteLine("DUPLICATE FOUND: "+dictLP+" overlaps with: "+aLinePiece);
						return true;
						}
				}
			}
		//Console.WriteLine("\nNEW FOUND: "+aLinePiece);
		// No overlapping linepieces found. Returning false.
		return false;
		}


	/// <summary>
	/// Removes all duplicate entries for lines to be drawn in the X3D.
	/// </summary>
	public Dictionary<LinePiece, ObjectLinePiece> cleanupLinePieceListDict(List<ObjectLinePiece> olps) {
		Dictionary<LinePiece, ObjectLinePiece> retDict = new Dictionary<LinePiece, ObjectLinePiece>();
		foreach (ObjectLinePiece olp in olps) {
			if (retDict.ContainsKey(olp.lp) || this.olpDictHasLine(retDict, olp.lp, this.ERROR_MARGIN)) {
				/*ObjectLinePiece oldOLP = retDict[olp.lp];
				Console.WriteLine("List has double linepiece: "+olp.lp.ToString()+
					" Previous object: "+oldOLP.o3d.tag+ " / colour: "+oldOLP.col+
					" New/skpd object: "+olp.o3d.tag+ " / colour: "+olp.col); 
				/*Console.WriteLine("Previous object: "+oldOLP.o3d.tag+ " / colour: "+oldOLP.col);
				Console.WriteLine("New/skpd object: "+olp.o3d.tag+ " / colour: "+olp.col); */
				} else {
				//add object
				retDict.Add(olp.lp, olp);
				}
			}
		return retDict;
		}
		
		
	public List<ObjectLinePiece> cleanupLinePieceList(List<ObjectLinePiece> olps) {
		Dictionary<LinePiece, ObjectLinePiece> tdict = this.cleanupLinePieceListDict(olps);
		return new List<ObjectLinePiece>(tdict.Values);
		}
	
	public string getMarkCol() {		
//		string markColor = "0 1 0 1";
		return ""+this.rnd.NextDouble()+" "+this.rnd.NextDouble()+" "+this.rnd.NextDouble()+" "+" 1";
		}	

	/// <summary>
	/// From a list of LinePieces, find out for each linepiece whether they are inside all other volumes or not.
	/// The reason for this method is that the "inner borders" don't have to be drawn, only the outer borders, those
	/// not enclosed by the other subvolumes borders.
	/// </summary>
	/// <returns>The inside outside OL ps.</returns>
	/// <param name="olps">Olps.</param>
	public List<ObjectLinePiece> markInsideOutsideOLPs(List<ObjectLinePiece> olps) {
		List<ObjectLinePiece> markedOLPs = new List<ObjectLinePiece>();
		foreach (ObjectLinePiece anOLP in olps) {
			ObjectLinePiece tOLP = anOLP;
			//tOLP.col = this.getMarkCol();
			//Console.Write("+");
			foreach (PlaneBorderEnclosedVolume aSubVolume in this.subVolumes) {
				// do not use the subvolume that created the border for container-checking
				if (tOLP.o3d == aSubVolume) { 
					continue;
					} 
				// Replaced "0.0000001" below with MainClass.SIGNIFICANTLY_SMALL. Not sure though if exception was made on purpose. Restore, or replace with Math.Sqrt(MainClass.SIGNIFICANTLY_SMALL) in case of errors. jbos@20160307
				if (aSubVolume.contains(tOLP.lp.getCenter(), null, MainClass.SIGNIFICANTLY_SMALL)) {
					tOLP.col = "0 0 0 0.1";
					}
				}
				// Distances/linepieces smaller than "significance" are considered insignificant and are ignored.
				if(tOLP.lp.getLength() > MainClass.SIGNIFICANTLY_SMALL)
					markedOLPs.Add(tOLP);
			}
		return markedOLPs;
		}

}} //end of class + namespace