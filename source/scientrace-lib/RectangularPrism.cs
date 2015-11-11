// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Scientrace {


public class RectangularPrism : Scientrace.Prism {

	public Scientrace.NonzeroVector width, height;
	//length inherited from Prism
	public RectangularPrism(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops, Scientrace.Location loc, Scientrace.NonzeroVector width, Scientrace.NonzeroVector height, Scientrace.NonzeroVector length) : base(parent, mprops) {
		this.loc = loc;
		this.width = width;
		this.length = length;
		this.height = height;
	}

	public override string ToString ()	{
		return "RectanglePrism dimensions: \nloc: "+this.loc.tricon()+
				"width: "+this.width.tricon()+
				"length: "+this.length.tricon()+
				"height: "+this.height.tricon();
		}
		
		
	public override Scientrace.Intersection intersects(Trace trace) {
		//NOT TESTED YET! COPIED FROM (tested) TRIANGULAR
		List<Scientrace.FlatShape2d> pgrams = new List<Scientrace.FlatShape2d>();
		//first: add parallelogram surfaces
		pgrams.Add(new Scientrace.Parallelogram(this.loc, this.height, this.length));
		pgrams.Add(new Scientrace.Parallelogram(this.loc, this.width, this.length));
		pgrams.Add(new Scientrace.Parallelogram(this.loc, this.height, this.width));
			
		pgrams.Add(new Scientrace.Parallelogram(this.loc+this.length.toLocation(), this.height, this.width));

		pgrams.Add(new Scientrace.Parallelogram(this.loc+this.width.toLocation(), this.height, this.length));
		pgrams.Add(new Scientrace.Parallelogram(this.loc+this.height.toLocation(), this.width, this.length));

		return this.intersectFlatShapes(trace, pgrams);
	}


	public override string exportX3D(Scientrace.Object3dEnvironment env) {
// prev after Transform      <Viewpoint description='	LineSet cube close up' position='0 0 5'/>
		//TODO: try to transform to indexedlineset
		Scientrace.Vector a, b, c, d, e, f, g, h;
		
		a = Vector.ZeroVector();
		//front square
		b = this.width;
		c = this.height+this.width;
		d = this.height;
		e = this.length;
		//back square
		f = this.length+this.width;
		g = this.length+this.width+this.height;
		h = this.length+this.height;
		
		//front square
		
		//back square
		
		//connecting squares
		
		
		
		return "<!-- Start RectangularPrism instance -->\n<Transform translation='"+this.loc.trico()+
				"'>\r\n      <Shape>\r\n        <!-- Coordinate rectangprism sequence (matches coordIndex actually) 0 1 2 3 0, 4 5 6 7 4, 0 4, 1 5, 2 6, 3 7 -->\r\n"+
				"        <LineSet vertexCount='5 5 2 2 2 2'>\r\n          <Coordinate point='\r\n"+
				a.tricon()+b.tricon()+c.tricon()+d.tricon()+a.tricon()+e.tricon()+f.tricon()+g.tricon()+h.tricon()+e.tricon()+a.tricon()+e.tricon()+b.tricon()+f.tricon()+c.tricon()+g.tricon()+d.tricon()+h.tricon()+
				"'/>\r\n"+
				"<ColorRGBA color='"+string.Concat(Enumerable.Repeat("0.5 0.5 0.5 1 ", 18))+"' />"+ // 18 due to 12 lines in 6 blocks produces 12+6=18, anyway this works?
				"        </LineSet>\r\n      </Shape>\r\n    </Transform>\n<!-- End RectangularPrism instance -->";
//          <Color color='1 0 0 1 0.5 0 1 1 0 0 1 0 1 0 0 0 0 1 0.8 0 0.8 0.3 0 0.3 1 1 1 0 0 1 1 0 0 0 0 1 1 0.5 0 0.8 0 0.8 1 1 0 0.3 0 0.3 0 1 0 1 1 1'/>
		
	}
}
}
