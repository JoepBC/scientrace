using System;
using System.Xml.Linq;

namespace ScientraceXMLParser {
	
	
public class XMLBorderParser : ScientraceXMLAbstractParser {

//	public XMLBorderParser(XElement xel, CustomXMLDocumentOperations X): base(xel, X) {
	public XMLBorderParser(): base() {
		}

	public Scientrace.AbstractGridBorder getXBorder(XElement xborder) {
		if (xborder == null) {
			Console.WriteLine("BORDER INEXISTENT");
			}
				
		Scientrace.AbstractGridBorder retborder;
		string borderclass;
		try {
			borderclass = this.X.getXString(xborder.Attribute("Class"));
			} catch {
			throw new XMLException("border class not specified");
			}
		switch (borderclass) {
		    case "CombinedBorder": 
		        //Console.WriteLine("Spiral Lightsource");
				retborder = this.getCombinedBorder(xborder);
		        break;
		    case "BoxBorder":
				retborder = this.getBoxBorder(xborder);
				break;
		    case "CylindricalBorder":
				retborder = this.getCylindricalBorder(xborder);
				//Console.WriteLine(retborder);
		        break;
		    default:
				throw new XMLException("unknown border class type: "+borderclass);
			}
			return retborder;
		}
	
	public Scientrace.BoxBorder getBoxBorder(XElement xborder) {
			Scientrace.Location location;
			Scientrace.NonzeroVector width, height, length;
			location = X.getXLocation(xborder.Element("Location"));
			width = X.getXNzVector(xborder.Element("Width"));
			height = X.getXNzVector(xborder.Element("Height"));
			length = X.getXNzVector(xborder.Element("Length"));
/*			Console.WriteLine("BOX WITH: "+location.tricon()+
			                  width.tricon()+
			                  height.tricon()+
			                  length.tricon()+
			                  " ... ");*/
			return new Scientrace.BoxBorder(location, width, height, length);
			}
	
	public Scientrace.CombinedBorder getCombinedBorder(XElement xborder) {
		Scientrace.CombinedBorder cb = null; 
		foreach (XElement xsubborder in xborder.Elements("Border")) {
			if (cb == null) {
				cb = new Scientrace.CombinedBorder(this.getXBorder(xsubborder));
				} else {
				cb.addBorder(this.getXBorder(xsubborder));
				}
			}
		return cb;
		}
		
	public Scientrace.CylindricalBorder getCylindricalBorder(XElement xborder) {
		Scientrace.Location loc = this.X.getXLocation(xborder.Element("Location"));
		Scientrace.NonzeroVector dirlen = this.X.getXNzVector(xborder.Element("DirectionLength"));
		double radius = this.X.getXDouble(xborder.Attribute("Radius"));
		Scientrace.CylindricalBorder retcyl = new Scientrace.CylindricalBorder(loc, dirlen, radius);
		return retcyl; 
		}
		
	}

}

