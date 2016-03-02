// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections.Generic;

namespace Scientrace {

public class X3DGridPoint {

	//From what locations are lines drawn?
	Scientrace.Location loc = null;
	//First neighbour, for illustrative purposes called "west"
	Scientrace.Location e = null;
	//Second neighbour, for illustrative purposes called "south"
	Scientrace.Location s = null;
	
	//There colour of the line drawn in between neighbouring nodes	
	string line_colour_rgba = "0 0 1 1";

	//The colour of the sphere (when drawn)	
	string sphere_colour = "0 0 0.7";
	//The radius of the sphere, if drawn. If sphere_radius == 0, no sphere will be drawn.
	double sphere_radius = 0;
	
	
	public X3DGridPoint(Scientrace.Object3dEnvironment env, Scientrace.Location loc, Scientrace.Location east, Scientrace.Location south) {
		this.init(X3DGridPoint.CalcRadiusFromEnv(env), loc, east, south);
		}
		
	public X3DGridPoint(double sphereRadius, Scientrace.Location loc, Scientrace.Location east, Scientrace.Location south) {
		this.init(sphereRadius, loc, east, south);
		}

		public void init (double sphereRadius, Scientrace.Location loc, Scientrace.Location east, Scientrace.Location south) {
		this.loc = loc;
		this.e = east;
		this.s = south;
		this.sphere_radius = sphereRadius;
		}
		
	public static double CalcRadiusFromEnv(Scientrace.Object3dEnvironment env) {
		return env.radius/1200;
		}

	public static string get_RGBA_Line_XML(LinePiece anLP, string colorRGBAstring) {
		return X3DGridPoint.get_RGBA_Line_XML(anLP.startingpoint, anLP.endingpoint, colorRGBAstring);
		}

	public static string get_RGBA_Line_XML(Scientrace.Location loc1, Scientrace.Location loc2, string colorRGBAstring) {
		if (loc1 == null || loc2 == null) {
			return "";
		}
		return @"<!--RGBA_Line--><Shape><LineSet vertexCount='2'><Coordinate point='"+loc2.trico()+" \t"+loc1.trico()+@"' />"+
		"<ColorRGBA color='"+colorRGBAstring+" \t"+colorRGBAstring+@"' /></LineSet></Shape>";
		}

	public static string get_RGB_Line_XML(Scientrace.Location loc1, Scientrace.Location loc2, string colourRGBstring) {
		if (loc1 == null || loc2 == null) {
			return "";
		}
		return @"<!--RGBA_Line--><Shape><LineSet vertexCount='2'><Coordinate point='"+loc2.trico()+" \t"+loc1.trico()+@"' />"+
		"<Color color='"+colourRGBstring+" \t"+colourRGBstring+@"' /></LineSet></Shape>";
		}

	public string x3DLineTo(Scientrace.Location aLoc) {
		return this.x3DLineTo(aLoc, this.line_colour_rgba);
		}
	public string x3DLineTo(Scientrace.Location aLoc, string colorRGBAstring) {
		return X3DGridPoint.get_RGBA_Line_XML(aLoc, this.loc, colorRGBAstring);
		}

	public string x3DSolidRGBALineTo(Scientrace.Location aLoc, string colorRGBstring) {
		return X3DGridPoint.get_RGBA_Line_XML(aLoc, this.loc, colorRGBstring);
		}


	public static string x3D_Transparant_Sphere(Scientrace.Location aLoc, double radius, double intensity) {
		return X3DGridPoint.x3D_Sphere(aLoc, radius, "0 0 1", intensity);
		}
       

	public static string getConnectAlleNodesXML(Object3dEnvironment env, List<Scientrace.Location> nodeList) {
		return X3DGridPoint.getConnectAlleNodesXML(X3DGridPoint.CalcRadiusFromEnv(env), nodeList);
		}

	public static string getConnectAlleNodesXML(double sphereRadius, List<Scientrace.Location> nodeList) {	
		//A list with two items produces a "line", not a figure that connects back to the first node.
		if (nodeList.Count == 2) {
			X3DGridPoint x3dGP = new X3DGridPoint(sphereRadius, nodeList[0], nodeList[1], null);
			return x3dGP.exportX3D();
			}
		//Any other list
		string retstr = "";
		for (int iNode = 0; iNode < nodeList.Count; iNode++) {
			X3DGridPoint x3dGP = new X3DGridPoint(sphereRadius, nodeList[iNode], nodeList[(iNode+1)%nodeList.Count], null);
			retstr += x3dGP.exportX3D();
			}
		return retstr;
		}
		                                 

	public string x3DSphere(double radius) {
		return X3DGridPoint.x3D_Sphere(this.loc, radius, this.sphere_colour, 0);
		}		                                                   

	public string x3DSphere(double radius, string colour, double sphere_colour_intensity) {
		return X3DGridPoint.x3D_Sphere(this.loc, radius, colour, sphere_colour_intensity);
		}
		        
	public static string x3D_Sphere(Scientrace.Location aLoc, double radius, string sphere_colour, double sphere_colour_intensity) {
		if ((aLoc == null) || !aLoc.isValid()) {
			throw new Exception("NULLS OR NANS DETECTED");
			}
		if (radius <= 0) return ""; // no sphere with no radius
		string transpstr = (sphere_colour_intensity < 1) ? " transparency='"+(1-Math.Sqrt(sphere_colour_intensity))+"'" : "";/*Sqrt for visibility */

		return @"<!--X3DSphere -->
<Transform scale='"+radius+" "+radius+" "+radius+@"' translation='"+aLoc.trico()+@"'>
<Shape><Sphere /> 
<Appearance><Material emissiveColor='"+sphere_colour+@"'"+transpstr+@" /></Appearance> 
</Shape></Transform><!-- End X3DSphere -->";
		}

	public string exportX3D() {
			return this.x3DLineTo(this.e)+this.x3DLineTo(this.s)+X3DGridPoint.x3D_Transparant_Sphere(this.loc, this.sphere_radius, 0.5);
		}

	public string exportX3DnosphereRGB(string colourstring_rgb) {
		string colourstring_rgba = colourstring_rgb+" 1";
		return this.x3DSolidRGBALineTo(this.e, colourstring_rgba)+this.x3DSolidRGBALineTo(this.s, colourstring_rgba);
		}

	public string exportX3DnosphereRGBA(string colourstring_rgba) {
			return this.x3DSolidRGBALineTo(this.e, colourstring_rgba)+this.x3DSolidRGBALineTo(this.s, colourstring_rgba);
		}

}
}
