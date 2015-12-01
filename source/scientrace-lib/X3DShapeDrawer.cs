// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Text;

namespace Scientrace {
public class X3DShapeDrawer {

	public string primaryRGB = "1 0 0";

	public X3DShapeDrawer() {
		}
		
	public string drawCircle(Scientrace.Location center, double radius, Scientrace.NonzeroVector aboutAxis) {
		return 
			this.getX3DTranslationTag(center)+ //where
			this.getX3DRotationTag(NonzeroVector.z1vector(), aboutAxis)+ //what direction
			"<Shape>"+this.emissiveColorAppearance(this.primaryRGB)+"<Circle2D radius='"+radius+"' />" + //what shape
			"</Shape></Transform></Transform>"; //close tags
		}
		
	public string emissiveColorAppearance(string colourstring) {
		return this.emissiveColorAppearance(colourstring, 1);
		}
	
	public string emissiveColorAppearance(string colourstring, double alpha) {
		string transpstr = (alpha < 1) ? " transparency='"+(1-alpha)+"'" : "";
		return "<Appearance><Material emissiveColor='"+colourstring+"' "+transpstr+" /></Appearance>";
		}
		
	public string getX3DTranslationTag(Scientrace.Location loc) {
		return "<Transform translation='"+loc.trico()+"'>";
		}
		
	public string getX3DRotationTag(Scientrace.NonzeroVector fromVector, Scientrace.NonzeroVector toVector) {
		Scientrace.Vector r = fromVector.crossProduct(toVector);
		double angle = 
			Math.Acos(toVector.normalized().dotProduct(fromVector.normalized())) // the angle to be rotated
			* Math.Sign(r.crossProduct(fromVector).dotProduct(toVector));
		try {
			return "<Transform rotation='"+r.tryToUnitVector().trico()+" "+angle+"' >";
			} catch { // if fromVector has the same direction as toVector, the crossProduct is a zerovector which cannot be normalized.
			return "<Transform>"; //no transformation, just open so it can be closed afterwards
			}
		}
		
		
	public StringBuilder drawSphereSlice(Scientrace.Object3d drawnObject3d, double lateral_circles, double meridians,
										Scientrace.Sphere sphere, double from_radians, double to_radians, 
										Scientrace.UnitVector sliceAlongDirection) {
		
		System.Text.StringBuilder retx3d = new System.Text.StringBuilder(1024);//"<!-- DOUBLECONVEXLENS GRID start -->");
		double pi2 = Math.PI*2;
		NonzeroVector orthoBaseVec1 = null;
		NonzeroVector orthoBaseVec2 = null;
		sliceAlongDirection.fillOrtogonalVectors(ref orthoBaseVec1, ref orthoBaseVec2);
		
		for (double iSphereCircle = 2*lateral_circles; iSphereCircle > 0; iSphereCircle--) { // the rings/parallels along the sliceAlongDirection axis
			double lateral_radians = (to_radians * (iSphereCircle / (2*lateral_circles)));
			double circle2DRadius = sphere.radius*Math.Sin(lateral_radians);
			double circle2DDistance = sphere.radius*Math.Cos(lateral_radians);
			retx3d.Append(this.drawCircle(sphere.loc+(sliceAlongDirection*circle2DDistance).toLocation(), circle2DRadius, sliceAlongDirection));
									
			for (double iSphereMerid = 0.5; iSphereMerid < 2*meridians; iSphereMerid++) { // meridians connect the rings/circles on the spherical surface

				Scientrace.Location tNodeLoc = sphere.getSphericalLoc(
							orthoBaseVec1, orthoBaseVec2,
							sliceAlongDirection,
							to_radians * (iSphereCircle / (2*lateral_circles)), // lat_angle = theta
							pi2 * (iSphereMerid/(2*meridians)) // mer_angle = phi
							);
				if (!tNodeLoc.isValid())
					throw new NullReferenceException("Cannot calculate base gridpoint at @ "+drawnObject3d.tag);
				Scientrace.Location tLatConnectLoc = sphere.getSphericalLoc(
							orthoBaseVec1, orthoBaseVec2,
							sliceAlongDirection,
							to_radians * ((iSphereCircle-1) / (2*lateral_circles)), // lat_angle = theta
							pi2 * ((iSphereMerid)/(2*meridians)) // mer_angle = phi
							);
				if (!tLatConnectLoc.isValid())
					throw new NullReferenceException("Cannot calculate lateral gridpoint at @ "+drawnObject3d.tag);

				Scientrace.X3DGridPoint tGridPoint = new Scientrace.X3DGridPoint(0, tNodeLoc, null, tLatConnectLoc);
				retx3d.AppendLine(tGridPoint.exportX3DnosphereRGB(this.primaryRGB));
				}} // end for iSphereCircle / iSphereMerid		
		return retx3d;
		}
		
		
	}}

