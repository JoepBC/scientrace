using System;
using System.Xml.Linq;
using System.Collections;


namespace ScientraceXMLParser {
	
	public class CustomXMLDocumentOperations {
		public CustomXMLDocumentOperations () {
		}
		
	public bool hasAttribute(XElement xe, string attribute_name) {
		return !(xe.Attribute(attribute_name)==null);	
		}

	public bool hasElement(XElement xe, string element_name) {
		return !(xe.Element(element_name)==null);	
		}

	public int getXInt(XAttribute xe) {
		if (xe== null) { throw new ArgumentNullException(); }
		return Convert.ToInt32(xe.Value);
		}
		
	public int getXInt(XElement xe) {
		if (xe== null) { throw new ArgumentNullException(); }
		return Convert.ToInt32(xe.Value);
		}

	public int getXInt(XAttribute xe, int defval) {
		if (xe== null) { return defval; }
		return Convert.ToInt32(xe.Value);
		}

	public int getXInt(XElement xe, int	defval) {
		if (xe== null) { return defval; }
		return Convert.ToInt32(xe.Value);
		}

	public double getXDouble(XAttribute xe) {
		if (xe== null) { throw new ArgumentNullException(); }
		return Convert.ToDouble(xe.Value);
		}

	public double getXDouble(XElement xe) {
		if (xe== null) { throw new ArgumentNullException(); }
		return Convert.ToDouble(xe.Value);
		}

	public double getXDouble(XAttribute xe, double defval) {
		if (xe == null) { return defval; }
		try {
			return Convert.ToDouble(xe.Value);
			} catch {
			throw new XMLException("ERROR: Cannot parse "+xe.Value+" to a double");
			}
		}

	public double getXDouble(XElement xe, double defval) {
		if (xe == null) { return defval; }
		return Convert.ToDouble(xe.Value);
		}

	public string getXStringByAttribute(XElement xe, string name) {
		if (xe.Attribute(name) == null) {
			throw new XMLException("String attribute \""+name+"\" does not exist for \""+xe.Name.ToString()+"\".");
			}
		return this.getXString(xe.Attribute(name));
		}

	public string getXStringByElement(XElement xe, string name) {
		if (xe.Element(name) == null) {
			throw new XMLException("String element \""+name+"\" does not exist for \""+xe.Name.ToString()+"\".");
			}
		return this.getXString(xe.Element(name));
		}		
		
	public string getXString(XAttribute xe) {
		if (xe== null) { throw new ArgumentNullException(); }
		return xe.Value;
		}

	public string getXString(XElement xe) {
		if (xe== null) { throw new ArgumentNullException(); }
		return xe.Value;
		}

	public string getXString(XElement xe, string defval) {
		if (xe == null) { return defval; }
		return xe.Value;
		}

	public string getXString(XAttribute xe, string defval) {
		if (xe == null) { return defval; }
		return xe.Value;
		}

	public bool? getXBool(XAttribute xe) {
		if (xe == null) { return null; }
		if ((xe.Value == "true") || (xe.Value == "yes") || (xe.Value == "1")) { return true; }
		if ((xe.Value == "false") || (xe.Value == "no") || (xe.Value == "0")) { return false; }
		return null;
		}

	public bool? getXBool(XElement xe) {
		if (xe == null) { return null; }
		if ((xe.Value == "true") || (xe.Value == "yes") || (xe.Value == "1")) { return true; }
		if ((xe.Value == "false") || (xe.Value == "no") || (xe.Value == "0")) { return false; }
		return null;
		}

	public bool getXBool(XAttribute xe, bool defval) {
		if (xe == null) { return defval; }
		if ((xe.Value == "true") || (xe.Value == "yes") || (xe.Value == "1")) { return true; }
		if ((xe.Value == "false") || (xe.Value == "no") || (xe.Value == "0")) { return false; }
		return defval;
		}
	public bool getXBool(XElement xe, bool defval) {
		if (xe == null) { return defval; }
		if ((xe.Value == "true") || (xe.Value == "yes") || (xe.Value == "1")) { return true; }
		if ((xe.Value == "false") || (xe.Value == "no") || (xe.Value == "0")) { return false; }
		return defval;
		}



	/// <summary>
	/// This method returns an "Angle" element which may be defined in radians OR degrees, but is
	/// always returned as a value in radians. 
	/// </summary>
	/// <param name="xe">
	/// A <see cref="XElement"/> with a "Radians" or a "Degrees" attribute.
	/// </param>
	/// <param name="name">
	/// A <see cref="System.String"/> that represent the XML element name.
	/// </param>
	/// <returns>
	/// A <see cref="System.Double"/> containing the angle values in radians. (so degrees * pi / 180)
	/// </returns>
	public double? getXNullAngleByName(XElement xe, string name) {
		if (xe.Element(name) == null) {
			return null;
			}
		try {
			return this.getXAngle(xe.Element(name));
			} catch(Exception e) {
			throw new XMLException("Could not convert Angle <"+xe.Name.ToString()+"><"+name+"> to Double \n"+e.Message);
			}
		}

	public double getXAngleByName(XElement xe, string name) {
		double? radians = this.getXNullAngleByName(xe, name);
		if (radians == null) {
			throw new XMLException("XAngle element \""+name+"\" does not exist for \""+xe.Name.ToString()+"\".");
			}
		return (double)radians;
		}
				
	public double getXAngleByName(XElement xe, string name, double defaultValue) {
		try {
			return (xe.Element(name) == null) ?
				defaultValue
				: this.getXAngle(xe.Element(name));
			} catch(Exception e) {
			throw new XMLException("Could not convert Angle <"+xe.Name.ToString()+"><"+name+"> to Double \n"+e.Message);
			}
		}
						
	public double getXAngle(XElement xe) {
		double? radians = this.getXNullAngle(xe);
		if (radians==null) {
			throw new XMLException("Angle "+xe.Name.ToString()+" has no 'Radians' or 'Degrees' attribute");
			}
		return (double)radians;
		}
				
	/// <summary>
	/// This method returns an "Angle" element which may be defined in radians OR degrees, but is
	/// always returned as a value in radians.
	/// </summary>
	/// <param name="xe">
	/// A <see cref="XElement"/> with a "Radians" or a "Degrees" attribute.
	/// </param>
	/// <returns>
	/// A <see cref="System.Double"/> containing the angle values in radians. (so degrees * pi / 180)
	/// </returns>
	public double? getXNullAngle(XElement xe) {
		double factor = this.getXDouble(xe, "Factor", 1);
		XAttribute radians = xe.Attribute("Radians");
		if (radians != null) {
			try {
				return Convert.ToDouble(radians.Value)*factor;
				} catch(Exception e) {
				throw new XMLException("Could not convert radian Angle <"+xe.Name.ToString()+">=\""+radians.Value+"\" to Double \n"+e.Message);
				}
			}
		XAttribute degrees = xe.Attribute("Degrees");
		if (degrees != null) {
			try {
				return Convert.ToDouble(degrees.Value)*Math.PI*factor/180;
				} catch(Exception e) {
				throw new XMLException("Could not convert degree Angle <"+xe.Name.ToString()+">=\""+degrees.Value+"\" to Double \n"+e.Message);
				}
			}
		return null;
		}


	public Scientrace.Vector getXVectorByName(XElement xe, string name, Scientrace.Vector defval) {
		if (xe.Element(name) == null) {
			return defval;
			}
		return this.getXVector(xe.Element(name));
		}
		
	public Scientrace.Vector getXVectorByName(XElement xe, string name) {
		if (xe.Element(name) == null) {
			throw new XMLException("XVector element \""+name+"\" does not exist for \""+xe.Name.ToString()+"\".");
			}
		return this.getXVector(xe.Element(name));
		}
		
	public Scientrace.NonzeroVector getXNzVectorByName(XElement xe, string name) {
		if (xe.Element(name) == null) {
			throw new XMLException("XNzVector element \""+name+"\" does not exist for \""+xe.Name.ToString()+"\".");
			}
		return this.getXNzVector(xe.Element(name));
		}
		
	public Scientrace.NonzeroVector getXNzVectorByName(XElement xe, string name, Scientrace.NonzeroVector defval) {
		try {
			return this.getXNzVector(xe.Element(name));
			} catch { return defval; }
		}

	public Scientrace.NonzeroVector getXUnitVectorByName(XElement xe, string name, Scientrace.NonzeroVector defval) {
		try {
			return this.getXVector(xe.Element(name)).tryToUnitVector();
			} catch { return defval; }
		}
	

	public Scientrace.Vector getXVector(XElement xe) {
		if (xe == null) {
			throw new XMLException("XVector element does not exist");
			}
		XAttribute x, y, z;
		double dx, dy, dz;
		if (xe.Attribute("xyz") != null) {
			try {
				string xyz = xe.Attribute("xyz").Value;
				char[] delimiterChars = { ';', '\t' };
				string[] elements = xyz.Split(delimiterChars);
				if (elements.Length !=3) {
					throw new XMLException("Element "+xe.ToString()+" has \""+elements.Length+ "\" != 3 valid vector elements. ");
					}
				dx=Convert.ToDouble(elements[0]);
				dy=Convert.ToDouble(elements[1]);
				dz=Convert.ToDouble(elements[2]);
				//Console.WriteLine("XYZ component values: x:"+dx+" y:"+dy+" z:"+dz);
				} catch { throw new XMLException("Element "+xe.ToString()+" does not have proper parameters for {xyz} attribute (can't parse {"+xe.Attribute("xyz").Value.ToString()+"}. Perhaps incorrect use of PreProcess variables or decimal separators?)."); }
			} else {
			try { 
				x = xe.Attribute("x");
				y = xe.Attribute("y");
				z = xe.Attribute("z");
				} catch { throw new XMLException("Element "+xe.ToString()+" does not have proper parameters {x,y,z}"); }
			try {
				dx = this.getXDouble(x);
				dy = this.getXDouble(y);
				dz = this.getXDouble(z);
				} catch {
				throw new XMLException("XVector "+xe.ToString()+
				                       " contains values ("+x.ToString()+", "+y.ToString()+", "+z.ToString()+
				                       ") that cannot be converted to floating point values.");
				}
			}
		Scientrace.Vector beforeModifyVec = new Scientrace.Vector(dx, dy, dz);
		Scientrace.Vector retvec = this.modifyVectorForSubElements(beforeModifyVec, xe);
		if (retvec==null) {
			Console.WriteLine("WARNING: NULL MODIFIED VECTOR FOUND FOR "+xe.ToString());
			}
		return retvec;
		}
		
	public int countIElements(IEnumerable ielements) {
		int reti = 0;
		foreach (XElement xe in ielements) { if (xe!=null) {reti++;} }
		return reti;
		}
		
	public Scientrace.Vector oldPerformVectorTransformations(Scientrace.Vector aVector, XElement xe) {
		//MULTIPLE TRANSFORMATIONS NOW PERMITTED IN ORDER OF APPEARANCE
/*		if (this.countIElements(xe.Elements("Transformed"))>1) {
			throw new IndexOutOfRangeException("Number of vectormodifications ("+
				(this.countIElements(xe.Elements("Transformed")))+
				" too large!");
			}*/
		Scientrace.Vector retvec = aVector;
		foreach (XElement xel in xe.Elements("Transformed")) {
			retvec = this.oldTransformVector(retvec, xel);
			}
		return retvec;
		}

		public Scientrace.Vector modifyVectorForSubElements(Scientrace.Vector aVector, XElement xe) {
		Scientrace.Vector retvec = aVector;
		foreach (XElement xel in xe.Elements()) {
			string elementName = xel.Name.ToString();
			switch (elementName) {
				case "Transformed":
					retvec = this.oldPerformVectorTransformations(retvec, xel);
					break;
				case "Rotate":
					retvec = this.rotateVector(retvec, xel);
					break;
				case "Translate":
					retvec = this.translateVector(retvec, xel);
					break;
				case "Multiply":
					retvec = this.multiplyVector(retvec, xel);
					break;
				default: 
					Console.WriteLine("WARNING: Unknown vector modification {"+elementName+"} detected. Perhaps the tense has changed? Should be active (Rotate instead of Rotated etc).");
					break;
				}
			}
		return retvec;
		}

	public Scientrace.Vector translateVector(Scientrace.Vector aVector, XElement xtransl) {
		double factor = this.getXDouble(xtransl,"Factor", 1);
		return aVector + (this.getXVectorSuggestions(xtransl, Scientrace.Vector.ZeroVector())*factor);
		}

	public Scientrace.Vector multiplyVector(Scientrace.Vector aVector, XElement xel) {
		double multiply_scalar = this.getXDouble(xel.Attribute("Factor"),1);
		Scientrace.Vector multiplyvec = this.getXVectorSuggestions(xel, new Scientrace.Vector(1,1,1));
		return (aVector*multiply_scalar).elementWiseProduct(multiplyvec);
		}

	public Scientrace.Vector rotateVector(Scientrace.Vector aVector, XElement xrotation) {
		double angle;
		try {
			angle=this.getXAngleByName(xrotation, "Angle");
			} catch (Exception e) {
			Console.WriteLine("WARNING: No valid Angle set for rotation! \n"+xrotation.ToString());
			throw e;
			}
		Scientrace.NonzeroVector rotaxis;
		try {
			rotaxis = this.getXNzVectorByName(xrotation, "AboutAxis");
			} catch (Exception e) {
			Console.WriteLine("WARNING: No valid AboutAxis set for rotation! \n"+xrotation.ToString());
			throw e;
			}
		/* rotation about an origin is obtained by first subtracting the "origin" vector,
		 * then rotating about the axis normally and then adding the "origin" vector again */
		Scientrace.Vector aboutOrigin = this.getXVector(xrotation.Element("AboutOrigin"), Scientrace.Vector.ZeroVector());
		Scientrace.Vector retvec = (aVector-aboutOrigin).rotateAboutVector(rotaxis, angle)+aboutOrigin;
		return retvec;
		}


	//TODO: this method and its underlying methods (oldWHATEVER) can be removed at some point. Now still here for backwards compatibility.
	public Scientrace.Vector oldTransformVector(Scientrace.Vector aVector, XElement transformed) {
		//THIS FUNCTION IS NOW FULLY OBSOLETE. THE ORDER OF OPERATION WAS PRETTY POINTLESS AFTER ALL. NOW SIMPLY IN ORDER OF APPEARANCE.
		/* ORDER OF OPERATION:
		 * 1: Multiply
		 * 2: Rotate
		 * 3: Translate
		 */
		//XElement transformed = xe.Element("Transformed");
		if (transformed == null) { return aVector; }

		Scientrace.Vector addvec = Scientrace.Vector.ZeroVector();
		foreach (XElement xtransl in transformed.Elements("Translate")) {
			double factor = this.getXDouble(xtransl,"Factor", 1);
			addvec = addvec + 
				this.getXVectorSuggestions(xtransl, Scientrace.Vector.ZeroVector())*factor;
			//Console.WriteLine("ADDVEC: "+addvec.trico()+" to "+aVector.trico());
			}
		double multiplication = 1;
		Scientrace.Vector multiplyvec = new Scientrace.Vector(1,1,1);
		if (transformed.Element("Multiply") != null) {
			multiplication = this.getXDouble(transformed.Element("Multiply").Attribute("Factor"),1);
			multiplyvec = this.getXVectorSuggestions(transformed.Element("Multiply"),
				new Scientrace.Vector(1,1,1));
			}
			//Console.WriteLine("-->: "+addvec.trico()+" to "+aVector.trico());

		return this.oldPerformVectorTransformations(		//Fourth (include sub-transformations)
				addvec +													//Third (add/translate)
				this.rotateVectorForRotatedElementsIn(											//Second (rotated)
				(multiplyvec*multiplication).elementWiseProduct(aVector)	//First (multiply)
				, transformed)//(second argument of rotate-fuction)
				, transformed);  //(second argument of modify function)
/*		Scientrace.Vector retvec = ((aVector*multiplication) + addvec);
		Scientrace.Vector fretvec = this.modifyVector(retvec, translated);
		return fretvec;*/
		}		
		
	public Scientrace.Vector rotateVectorForRotatedElementsIn(Scientrace.Vector aVector, XElement xe) {
		Scientrace.Vector retvec = aVector;
		// foreach(XElement rotated in xe.Elements("Rotated")) { // IN ORDER OF APPEARANCE
		foreach(XElement rotated in xe.Elements("Rotate")) { // IN ORDER OF APPEARANCE
			if (rotated == null) { continue; /* return aVector;*/ }
			double angle=0;
			if (rotated.Element("Angle") == null) {throw new XMLException("Could not parse Angle for rotation"); }
			foreach (XElement xeangle in rotated.Elements("Angle")) {
				angle = angle + this.getXAngle(xeangle);
				}
			Scientrace.NonzeroVector rotaxis; try {
				rotaxis = this.getXNzVector(rotated.Element("AboutAxis"));
				} catch { throw new XMLException("Could not parse AboutAxis for rotation"); }
			/* rotation about an origin is obtained by first subtracting the "origin" vector,
			 * then rotating about the axis normally and then adding the "origin" vector again */
			Scientrace.Vector aboutOrigin = this.getXVector(rotated.Element("AboutOrigin"), Scientrace.Vector.ZeroVector());
			//Console.WriteLine("ABOUT ORIGIN: "+aboutOrigin.trico()+"\n aBOUT aXIS: "+rotaxis.tricon());
			retvec = (retvec-aboutOrigin).rotateAboutVector(rotaxis, angle)+aboutOrigin;
			//Scientrace.Vector fretvec = this.modifyVector(retvec, rotated);
			}
		return retvec;
		}

	public Scientrace.Vector getXVector(XElement xe, Scientrace.Vector defval) {
		try { return this.getXVector(xe); } catch { return defval; }
		}

	public Scientrace.NonzeroVector getXNzVectorSuggestions(XElement xe, string xmlname) {
		return this.getXNzVectorSuggestions(xe, xmlname, Scientrace.Vector.ZeroVector());
		}

	public Scientrace.NonzeroVector getXNzVectorSuggestions(XElement xe, string xmlname, Scientrace.Vector baseval, Scientrace.NonzeroVector defval) {
		if (xe.Element(xmlname) == null) return defval;
		Scientrace.Vector retval;
		try {
			retval = this.getXVectorSuggestions(xe.Element(xmlname), baseval);
			}
		catch { return defval; }
		return retval.tryToNonzeroVector("ERROR: ("+xmlname+") may not be 0 in {"+xe.ToString()+"}");
		}

	public Scientrace.NonzeroVector getXNzVectorSuggestions(XElement xe, string xmlname, Scientrace.Vector baseval) {
		if (xe.Element(xmlname) == null) throw new XMLException("ERROR: ("+xmlname+") not found in {"+xe.ToString()+"}");
		Scientrace.Vector retval = this.getXVectorSuggestions(xe.Element(xmlname), baseval);
		return retval.tryToNonzeroVector("ERROR: ("+xmlname+") may not be 0 in {"+xe.ToString()+"}");
		}

	public Scientrace.Vector getXVectorSuggestions(XElement xe, Scientrace.Vector baseval) {	
		if (xe == null) {
			throw new XMLException("XVector element does not exist");
			}
		Scientrace.Vector retvec;
		if (xe.Attribute("xyz") != null) {
			retvec = this.getXVector(xe);
			} else {
			retvec = new Scientrace.Vector(this.getXDouble(xe, "x", baseval.x),
				this.getXDouble(xe, "y", baseval.y),
				this.getXDouble(xe, "z", baseval.z));
			retvec = this.modifyVectorForSubElements(retvec, xe);
			}
		return retvec;
		}			


	public Scientrace.Location getXLocation(XElement xe) {
		return this.getXVector(xe).toLocation();
		}
		
	public Scientrace.Location getXLocation(XElement xe, Scientrace.Location defval) {
		return this.getXVector(xe,defval).toLocation();
		}
		
		
	public Scientrace.Location getXLocation(XElement xparent, string xmlkey, Scientrace.Location aDefLoc) {
		if (xparent.Element(xmlkey) == null) 
			return aDefLoc;
		return this.getXLocation(xparent.Element(xmlkey));
		}


	public Scientrace.Location getXLocation(XElement xparent, string xmlkey) {
		Scientrace.Location retLoc = this.getXLocation(xparent, xmlkey, null);
		if (retLoc == null)
			throw new XMLException("ERROR: no Location vector {"+xmlkey+"} found for "+xparent.Name+" element. XML source:\n" +
			xparent.ToString());
		return retLoc;
		}
				
	public Scientrace.NonzeroVector getXNzVector(XElement xe) {
		//try {
		return this.getXVector(xe).tryToNonzeroVector();
		//	} catch(Exception e) { throw(e); }
		}
		
	public Scientrace.NonzeroVector getXNzVector(XElement xe, Scientrace.NonzeroVector defval) {
		return this.getXVector(xe,defval).tryToNonzeroVector();
		}
		
	public XElement setXVector(string elname, Scientrace.Vector v) {
		XElement retx = new XElement(elname);
		retx.Add(new XAttribute("x", v.x));
		retx.Add(new XAttribute("y", v.y));
		retx.Add(new XAttribute("z", v.z));
		return retx;
		}

	public Scientrace.Vector getAxisVector(string xyzstring) {
		switch (xyzstring.ToLower()) {
			case "x": { return new Scientrace.Vector(1,0,0);}
			case "y": { return new Scientrace.Vector(0,1,0);}
			case "z": { return new Scientrace.Vector(0,0,1);}
			default: {throw new XMLException("getAxisVector error: "+xyzstring+" is no known axis");}
			}
		}

	public XElement setReplace(string varname, string tovalue) {
		if (varname[0] != '$') {
			throw new ArgumentOutOfRangeException("Key "+varname+" should start with a '$'");
			}
		varname = varname.Substring(1);
		return new XElement("Replace", new XAttribute("Key", varname), new XAttribute("Value", tovalue));
		}

	public XElement setReplace(string varname, XElement tovalue) {
		if (varname[0] != '$') {
			throw new ArgumentOutOfRangeException("Key "+varname+" should start with a '$'");
			}
		varname = varname.Substring(1);
		XElement retxe = new XElement("Replace", new XAttribute("Key", varname));
		retxe.Add(new XElement("Value", tovalue));
		return retxe;
		}
		
	public XElement setXAngleDeg(string elementname, double degrees) {
		return new XElement(elementname, new XAttribute("Degrees", degrees.ToString()));
		}
		
	public XElement setXAngleRad(string elementname, double radians) {
		return new XElement(elementname, new XAttribute("Radians", radians.ToString()));
		}

	public XElement setXRotationDeg(Scientrace.Vector nonzeroAxis, double degrees) {
		return this.setXRotationDeg(nonzeroAxis, degrees.ToString());
		}

	public XElement setXRotationDeg(Scientrace.Vector nonzeroAxis, string degrees) {
		XElement retx = new XElement("Rotated");
		retx.Add(new XElement("Angle", new XAttribute("Degrees", degrees)));
		retx.Add(this.setXVector("AboutAxis", nonzeroAxis));
		return new XElement("Transformed", retx);
		}

	public XElement setXRotationDeg(string xyzstring, double degrees) {
		return this.setXRotationDeg(xyzstring, degrees.ToString());	
		}

	public XElement setXRotationDeg(string xyzstring, string degrees) {
		return this.setXRotationDeg(this.getAxisVector(xyzstring), degrees);	
		}
		
	/* Use functions below for nested element purposes */ 
	//USING "object" instead of XElement or string for subelements
	public XElement setXVector(string elname, Scientrace.Vector v, object subElement) {
		XElement retx = this.setXVector(elname, v);
		retx.Add(subElement);
		return retx;
		}
	public XElement setXRotationDeg(string xyzstring, double degrees, object subElement) {
		XElement retx = this.setXRotationDeg(xyzstring, degrees);
		retx.Add(subElement);
		return retx;
		}
	public XElement setXRotationDeg(Scientrace.Vector nonzeroAxis, double degrees, object subElement) {
		XElement retx = this.setXRotationDeg(nonzeroAxis, degrees);
		retx.Add(subElement);
		return retx;
		}
	public XElement setXRotationRad(string xyzstring, double radians, object subElement) {
		XElement retx = this.setXRotationRad(xyzstring, radians);
		retx.Add(subElement);
		return retx;
		} 
	public XElement setXRotationRad(Scientrace.Vector nonzeroAxis, double radians, object subElement) {
		XElement retx = this.setXRotationRad(nonzeroAxis, radians);
		retx.Add(subElement);
		return retx;
		}
	/* end of nested element functions */


	public XElement setXRotationRad(Scientrace.Vector nonzeroAxis, double radians) {
		XElement retx = new XElement("Rotated");
		retx.Add(new XElement("Angle", new XAttribute("Radians", radians.ToString())));
		retx.Add(this.setXVector("AboutAxis", nonzeroAxis));
		return new XElement("Transformed", retx);
		}

	public XElement setXRotationRad(string xyzstring, double radians) {
		return this.setXRotationRad(this.getAxisVector(xyzstring), radians);
		}

	public XAttribute setXYNBool(string atname, bool aBool) {
		return new XAttribute(atname, (aBool?"yes":"no"));
		}

	public XAttribute setTFBool(string atname, bool aBool) {
		return new XAttribute(atname, (aBool?"true":"false"));
		}

	public XAttribute setXBool(string atname, bool aBool) {
		return this.setXYNBool(atname, aBool);
		}

	public XAttribute setXString(string atname, string aValue) {
		return new XAttribute(atname, aValue);
		}

	/* SECOND GENERATION GET FUNCTIONS: */

	public bool getXBool(XElement xparent, string xmlkey) {
		if (xparent.Attribute(xmlkey) != null) {
			try { return (bool)this.getXBool(xparent.Attribute(xmlkey));
				} catch {throw new XMLException("ERROR: COULD NOT PARSE BOOLEAN "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		if (xparent.Element(xmlkey) != null) { 
			try { return (bool)this.getXBool(xparent.Element(xmlkey));
				} catch {throw new XMLException("ERROR: COULD NOT PARSE BOOLEAN "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		throw new XMLException("ERROR: no "+xmlkey+" found for "+xparent.Name+" boolean element");
		}
	
	public bool getXBool(XElement xparent, string xmlkey, bool defvalue) {
		if (xparent == null)
			return defvalue;
		if (xparent.Attribute(xmlkey) != null) {
			try { return (bool)this.getXBool(xparent.Attribute(xmlkey));
				} catch {throw new XMLException("ERROR: COULD NOT PARSE BOOLEAN "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		if (xparent.Element(xmlkey) != null) { 
			try { return (bool)this.getXBool(xparent.Element(xmlkey));
				} catch {throw new XMLException("ERROR: COULD NOT PARSE BOOLEAN "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		return defvalue;
		}	

	public bool? getXNullBool(XElement xparent, string xmlkey, bool? defvalue) {
		if (xparent == null)
			return defvalue;
		if (xparent.Attribute(xmlkey) != null) {
			try { return this.getXBool(xparent.Attribute(xmlkey));
				} catch {throw new XMLException("ERROR: COULD NOT PARSE BOOLEAN "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		if (xparent.Element(xmlkey) != null) { 
			try { return this.getXBool(xparent.Element(xmlkey));
				} catch {throw new XMLException("ERROR: COULD NOT PARSE BOOLEAN "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		return defvalue;
		}	

		
	public int getXInt(XElement xparent, string xmlkey, int defval) {
		try { return this.getXInt(xparent, xmlkey); }
			catch {
			return defval;
			}
		}
		
	public int getXInt(XElement xparent, string xmlkey) {
		if (xparent.Attribute(xmlkey) != null) {
			try { return Convert.ToInt32(xparent.Attribute(xmlkey).Value);
				} catch {throw new XMLException("ERROR: COULD NOT PARSE "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		if (xparent.Element(xmlkey) != null) { 
			try { return Convert.ToInt32(xparent.Element(xmlkey).Value);
				} catch {throw new XMLException("ERROR: COULD NOT PARSE "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		throw new XMLException("ERROR: no "+xmlkey+" found for "+xparent.Name+" element");
		}

	public double getXDouble(XElement xparent, string xmlkey, double defval) {
		/*try { return this.getXDouble(xparent, xmlkey); }
			catch {
			return defval;
			}*/
		double? retval = this.getXNullDouble(xparent, xmlkey);
		if (retval == null)
			return defval;
		return (double) retval;			
		}		
		
	public double getXDouble(XElement xparent, string xmlkey) {
		//old code replaced to prevent redundancy
	/*
		if (xparent.Attribute(xmlkey) != null) { 
			try { return Convert.ToDouble(xparent.Attribute(xmlkey).Value);
				} catch {throw new XMLException("ERROR: COULD NOT PARSE "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		if (xparent.Element(xmlkey) != null) { 
			try { return Convert.ToDouble(xparent.Element(xmlkey).Value);
				} catch {throw new XMLException("ERROR: COULD NOT PARSE "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}*/
		double? retval = this.getXNullDouble(xparent, xmlkey);
		if (retval == null)
			throw new XMLException("ERROR: no "+xmlkey+" found for "+xparent.Name+" element");
		return (double) retval;
		}
		
	public string getXStringByName(XElement xparent, string xmlkey) {
		string aNullableString = this.getXNullString(xparent, xmlkey);
		if (aNullableString == null) {
			throw new XMLException("ERROR: no "+xmlkey+" found for "+xparent.Name+" element");
			}
		return aNullableString;
		}

		public string getXStringByName(XElement xparent, string xmlkey, string defval) {
		if (xparent == null)
			return defval;
		string aNullableString = this.getXNullString(xparent, xmlkey);
		if (aNullableString == null) {
			return defval;
			}
		return aNullableString;
		}


	public string getXNullString(XElement xparent, string xmlkey, string defval) {
		if (xparent == null)
			return defval;
		string retval = this.getXNullString(xparent, xmlkey);
		if (retval == null) return defval;
		return retval;
		}

	public string getXNullString(XElement xparent, string xmlkey) {
		if (xparent==null) return null;
		if (xparent.Attribute(xmlkey) != null) { 
			try { return xparent.Attribute(xmlkey).Value;
				} catch {throw new XMLException("ERROR: COULD NOT PARSE "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		if (xparent.Element(xmlkey) != null) { 
			try { return xparent.Element(xmlkey).Value;
				} catch {throw new XMLException("ERROR: COULD NOT PARSE "+xmlkey+"("+xparent.Element(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		return null;	
		}
		
	public double? getXNullDouble(XElement xparent, string xmlkey) {
		if (xparent==null) return null;
		if (xparent.Attribute(xmlkey) != null) { 
			try { return Convert.ToDouble(xparent.Attribute(xmlkey).Value);
				} catch {throw new XMLException("ERROR: COULD NOT PARSE "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		if (xparent.Element(xmlkey) != null) { 
			try { return Convert.ToDouble(xparent.Element(xmlkey).Value);
				} catch {throw new XMLException("ERROR: COULD NOT PARSE "+xmlkey+"("+xparent.Element(xmlkey).Value+") AT \""+xparent.Name+"\" element");}
			}
		return null;
		}

	public int? getXNullInt(XElement xparent, string xmlkey) {
		if (xparent==null) return null;
		if (xparent.Attribute(xmlkey) != null) { 
			try { return Convert.ToInt32(xparent.Attribute(xmlkey).Value);
				} catch {throw new XMLException("ERROR: COULD NOT PARSE "+xmlkey+"("+xparent.Attribute(xmlkey).Value+") {int} AT \""+xparent.Name+"\" element");}
			}
		if (xparent.Element(xmlkey) != null) { 
				try { return Convert.ToInt32(xparent.Element(xmlkey).Value);
				} catch {throw new XMLException("ERROR: COULD NOT PARSE "+xmlkey+"("+xparent.Element(xmlkey).Value+") {int} AT \""+xparent.Name+"\" element");}
			}
		return null;
		}

	
	}
}

