using System;
using System.Collections;
using System.Xml.Linq;

namespace ScientraceXMLParser {
	
	public class ScientraceXMLAbstractParser {
		
	protected CustomXMLDocumentOperations X;
	protected Scientrace.Object3dCollection parentcollection;
	//protected XElement xel;
		
		public ScientraceXMLAbstractParser() {
		//public ScientraceXMLAbstractParser(XElement xel, CustomXMLDocumentOperations X) {
			//this.xel = xel;
			this.X = new CustomXMLDocumentOperations();
			}
		}
	}