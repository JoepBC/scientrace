// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;

namespace ShadowScientrace {

public enum rectangleFactoryMethod {
		Center_and_Sidelength = 1,
		Loc_width_height = 2,
		}}

namespace Scientrace {


public class Rectangle : Scientrace.FlatSurfaceObject3d {

	public Scientrace.Parallelogram parallelogram;

	public Rectangle(ShadowScientrace.ShadowObject3d shadowObject): base(shadowObject) {
		switch (shadowObject.factory_method) {
			case (int)ShadowScientrace.rectangleFactoryMethod.Loc_width_height: this.shadowFac_Loc_width_height(shadowObject);
				break;
			case (int)ShadowScientrace.rectangleFactoryMethod.Center_and_Sidelength: this.shadowFac_Center_and_Sidelength(shadowObject);
				break;
			default:
				throw new ArgumentOutOfRangeException("Factory method "+shadowObject.factory_method+" not found for "+shadowObject.typeString());
			}
		
		}

	/// <summary>
	/// Required parameters:
	/// (Location)center_location
	/// (Location)pointing_towards
	/// (UnitVector)orthogonal_direction
	/// (double)side_length
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>
	protected void shadowFac_Loc_width_height(ShadowScientrace.ShadowObject3d shadowObject) {
		//User values(s)
		Scientrace.Location corner_location = shadowObject.getLocation("corner_location");
		Scientrace.NonzeroVector width = shadowObject.getNzVector("width");
		Scientrace.NonzeroVector height = shadowObject.getNzVector("height");

		//construct!
		this.init_Loc_width_height(corner_location, width, height);
		}

	/// <summary>
	/// Required parameters:
	/// (Location)center_location
	/// (Location)pointing_towards
	/// (UnitVector)orthogonal_direction
	/// (double)side_length
	/// </summary>
	/// <param name='shadowObject'>
	/// Shadow object containing parameters from summary.
	/// </param>
	protected void shadowFac_Center_and_Sidelength(ShadowScientrace.ShadowObject3d shadowObject) {
		//User values(s)
		Scientrace.Location center_location = shadowObject.getLocation("center_location");
		Scientrace.Location pointing_towards = shadowObject.getLocation("pointing_towards");
		Scientrace.UnitVector orthogonal_direction = shadowObject.getUnitVector("orthogonal_direction");
		double side_length = shadowObject.getDouble("side_length");

		//construct!
		this.init_Center_and_Sidelength(center_location, pointing_towards, orthogonal_direction, side_length);
		}


	protected void init_Loc_width_height(Scientrace.Location loc, Scientrace.NonzeroVector u, Scientrace.NonzeroVector v) {
		this.parallelogram = new Scientrace.Parallelogram(loc, u, v);
		this.front_normal_direction = u.crossProduct(v).tryToUnitVector();
		this.preserveSVGRatio();	
		this.addSurfaceMarkers();
		}

	/// <summary>
	/// Initializes a new instance of the <see cref="Scientrace.Rectangle"/> class on a
	/// given location with a given width and height vector. The "pointingto" vector,
	/// which defines the front of the cell, is defined by the crossproduct u * v.
	/// </summary>
	/// <param name='parent'>
	/// What collection is this object part of?
	/// </param>
	/// <param name='mprops'>
	/// Materialproperties
	/// </param>
	/// <param name='loc'>
	/// The location of the rectangle.
	/// </param>
	/// <param name='u'>
	/// Width?
	/// </param>
	/// <param name='v'>
	/// Height?
	/// </param>
	public Rectangle(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops, 
		                  Scientrace.Location loc, Scientrace.NonzeroVector u, Scientrace.NonzeroVector v) : base(parent, mprops) {
		this.init_Loc_width_height(loc, u, v);
		}

	protected void init_Center_and_Sidelength(Scientrace.Location cellcenterloc, Scientrace.Location pointingto, Scientrace.UnitVector orthogonaldirection,
		                  double sidelength) {
		//Check whether cellcenterloc and pointingto values are different
		if (cellcenterloc.distanceTo(pointingto) == 0) {
			throw new ZeroNonzeroVectorException("The location ("+cellcenterloc.ToCompactString()+") of a Rectangle instance may not be equal to the direction ("+pointingto.ToCompactString()+") it is pointed towards");
			}

		NonzeroVector cellu, cellv, cellc;
		//define top-cell vector orthogonal to orthogonaldirection and the vectorpoint.
		Scientrace.UnitVector focusdir = (pointingto.toVector() - cellcenterloc.toVector()).tryToUnitVector();
		//Console.WriteLine("POINTINGTO: "+pointingto.trico()+ " focusdir: "+focusdir.trico());
		this.front_normal_direction = focusdir.copy();
		cellu = orthogonaldirection.crossProduct(focusdir).tryToNonzeroVector();
		cellu.normalize();
		//UPDATE 20151102: added "negative" to conserve surface direction after crossproduct operation.
		cellv = cellu.crossProduct(focusdir.negative()).tryToNonzeroVector();
		cellv.normalize();
		cellu = cellu * sidelength; //surface of cell eq: sidelength * sidelength
		cellv = cellv * sidelength;
		cellc = (cellu+cellv)*0.5;

		this.parallelogram = new Scientrace.Parallelogram(cellcenterloc-cellc.toLocation(), cellu, cellv);
		this.preserveSVGRatio();
		this.addSurfaceMarkers();
		}

	public Rectangle(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops, 
		                  Scientrace.Location cellcenterloc, Scientrace.Location pointingto, Scientrace.UnitVector orthogonaldirection,
		                  double sidelength) : base(parent, mprops) {
		this.init_Center_and_Sidelength(cellcenterloc, pointingto, orthogonaldirection, sidelength);
		}		
	
	/// <summary>
	/// Preserve x/y ratio based on the surface "square" (which is possibly rectangular) ratio.
	/// Must be ran before the SVG-export function of the surface markers but explicity AFTER
	/// defining the surface parallelogram in a constructor method.
	/// </summary>
	public void preserveSVGRatio() {
		this.svgysize = this.svgxsize * (this.parallelogram.plane.v.length / this.parallelogram.plane.u.length);
		}
			
		
	public void addSurfaceMarkers() {
		//TODO: not add vertical marker by default but optinally.
		VerticalGridSurfaceMarker vmark =  new VerticalGridSurfaceMarker(this);
		vmark.minval = 0;
		if (this.parallelogram.plane.v.length < 1) {
			vmark.maxval = this.parallelogram.plane.v.length*1000;
			vmark.yunits = "mm";
			} else {
			vmark.maxval = this.parallelogram.plane.v.length;
			vmark.yunits = "m";
			}	
		this.markers.Add(vmark);
		//TODO: not add horizontal marker by default but optinally.
		HorizontalGridSurfaceMarker hmark =  new HorizontalGridSurfaceMarker(this);
		hmark.minval = 0;
		if (this.parallelogram.plane.u.length < 1) {
			hmark.maxval = this.parallelogram.plane.u.length*1000;
			hmark.xunits = "mm";
			} else {
			hmark.maxval = this.parallelogram.plane.u.length;
			hmark.xunits = "m";
			}	
		this.markers.Add(hmark);
		}

	public override Parallelogram getDistributionSurface() {
		return this.parallelogram;
		}

	public override Scientrace.Location get2DLoc(Scientrace.Location loc) {
			
		//Scientrace.VectorTransformOrthonormal trf = new Scientrace.VectorTransformOrthonormal(this.surface.u, this.surface.v);
		//Scientrace.VectorTransform trf = new Scientrace.VectorTransform(this.surface.u, this.surface.v);
		Scientrace.VectorTransform trf = new Scientrace.VectorTransform(this.parallelogram.plane.u.normalized(), this.parallelogram.plane.v.normalized());
		return trf.transform(loc-this.parallelogram.plane.loc);
		}
		
	public override Scientrace.Location getSVG2DLoc(Scientrace.Location loc) {
		Scientrace.Location loc2d = this.get2DLoc(loc);
		Scientrace.Location retloc = loc2d.elementWiseProduct(new Scientrace.Vector(this.svgxsize / this.parallelogram.plane.u.length, this.svgysize / this.parallelogram.plane.v.length, 1)).toLocation();
		//Console.WriteLine("Loc: "+loc.trico()+" is 2D'd into: "+retloc.trico()+" x/y="+this.svgxsize+"/"+svgysize);
		return retloc;
		}
			                        
	public override Scientrace.UnitVector getSurfaceNormal() {
		//this.getDistributionSurface().getNorm(); ???
		return this.parallelogram.plane.getNorm();
		}
		
	/// <summary>
	/// The dimensions of the exported surface, for example: a square surface with sizes 640*480 will return 0 0 640 480,
	/// in certain situations, it may be relevant to place the origin away from the corner, resulting in a square with the
	/// same surface as the example, but different positioning, i.e. -320 -240 320 240.
	/// </summary>
	/// <returns>
	/// An <see cref="System.Double[]"/> array defined as: left-x, top-y, right-x, bottom-y.
	/// </returns>
	public override double[] getViewBorders() {
		//return new double[]{0, 0, this.surface.u.length, this.surface.v.length};
		return new double[]{Math.Min(0, this.svgxsize), Math.Min(0,this.svgysize), Math.Max(0, this.svgxsize), Math.Max(0,this.svgysize)};
		}

	public bool hasBitmap() {
		if (this.x3d_fill_bitmap == null) return false;
		return (this.x3d_fill_bitmap.Length > 0);
		}

	public override string exportX3D (Scientrace.Object3dEnvironment env)	{
		Scientrace.Vector a, b, c, d;
		//square
		a = Vector.ZeroVector();
		b = this.parallelogram.plane.u;
		c = this.parallelogram.plane.u+this.parallelogram.plane.v;
		d = this.parallelogram.plane.v;
//		return this.x3d_fill?
		return (this.x3d_fill_emissive_color!=null || this.x3d_fill_diffuse_color!=null || this.hasBitmap())?
			this.exportX3DFilledSurface(a,b,c,d)+this.exportX3DLineSet(a,b,c,d)
//			this.exportX3DFilledSurface(a,b,c,d)
			:this.exportX3DLineSet(a,b,c,d);
		}//end exportX3D


	public Scientrace.Vector getX3DNormal() {
		//Console.WriteLine("NORMAL: "+this.getSurfaceNormal());
		if (this.x3d_fill_normal == null) 
			return this.getSurfaceNormal();
		return this.x3d_fill_normal;
		}

	public string exportX3DFilledSurface(Scientrace.Vector a, Scientrace.Vector b, Scientrace.Vector c, Scientrace.Vector d) {
		return
@"    <Transform translation='"+this.parallelogram.plane.loc.trico()+@"'><Shape>
      <IndexedFaceSet DEF='"+this.tag+"' solid='"+(this.x3d_fill_both_sides?"false":"true")+@"' coordIndex='0 1 2 3' normalIndex='0 0 0 0'>
        <Coordinate point='"+a.trico()+", "+b.trico()+", "+c.trico()+", "+d.trico()+@"'/>
        <Normal vector='"+this.getX3DNormal().trico()+@"'/>
      </IndexedFaceSet>
 "+this.getX3DAppearance()+@"
    </Shape></Transform>";
		}

	public string getX3DDiffuseColorTag() {
		if (this.x3d_fill_diffuse_color == null) return "";
		return "diffuseColor=\""+this.x3d_fill_diffuse_color.trico()+"\"";
		}


	public string getX3DEmissiveColorTag() {
		if (this.x3d_fill_emissive_color == null) return "";
		return "emissiveColor=\""+this.x3d_fill_emissive_color.trico()+"\"";
		}

	public string getX3DAppearance() {
		return 
@"     <Appearance>" +
(this.x3d_fill_bitmap == null?
@"		<Material "+
								this.getX3DEmissiveColorTag()+" "+this.getX3DDiffuseColorTag()
//(this.x3d_fill_diffuse?"diffuseColor":"emissiveColor")+@"='"+this.x3d_fill_emissive_color.trico()
+@"/>"
:@"		<ImageTexture url='"""+this.x3d_fill_bitmap+@"""' />") +
"</Appearance>";
		}

	public string exportX3DLineSet(Scientrace.Vector a, Scientrace.Vector b, Scientrace.Vector c, Scientrace.Vector d) {
		return @"<!-- Start Rectangle instance -->\n<Transform translation='"+this.parallelogram.plane.loc.trico()+@"'>
<Shape>
<!-- Coordinate rectangle sequence (matches coordIndex actually) 0 1 2 3 0 -->
<LineSet vertexCount='5'>
<Coordinate point='
"+a.trico()+", "+b.trico()+", "+c.trico()+", "+d.trico()+", "+a.trico()+"'/>" +
	@"<ColorRGBA color='0 1 0 1, 0 1 0 1, 
						0 1 0 1, 0 1 0 1, 
						0 1 0 1, 0 1 0 1, 
						0 1 0 1, 0 1 0 1' />	
</LineSet>" +
	@"      </Shape>\" +
	@"    </Transform>\n<!-- End Rectangle instance -->";		
		}

	public override Intersection intersects (Trace trace) {
		Scientrace.Location intrloc = this.parallelogram.atPath(trace.traceline);
		if (intrloc == null) {
			return new Intersection(false, this);
			}
		return new Intersection(true, this, intrloc, this.parallelogram, null);
	}

}

}
