// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections.Generic;

namespace Scientrace {


public class CircularPrism : Scientrace.Prism {

	public Scientrace.NonzeroVector width, height;
	//length inherited from Prism
	public CircularPrism(Scientrace.Object3dCollection parent, Scientrace.MaterialProperties mprops, Scientrace.Location loc, Scientrace.NonzeroVector width, Scientrace.NonzeroVector height, Scientrace.NonzeroVector length) : base(parent, mprops) {
		this.loc = loc;
		this.width = width;
		this.height = (height.tryToUnitVector())*width.length;
		this.length = length;
	}


	public override Intersection intersects(Trace trace) {
		List<Scientrace.FlatShape2d> pgrams = new List<Scientrace.FlatShape2d>();
		//first: add parallelogram circle2d surfaces
		pgrams.Add(new Scientrace.Circle2d(this.loc, this.height, this.width, this.width.length));
		pgrams.Add(new Scientrace.Circle2d(this.loc+this.length.toLocation(), this.height, this.width, this.width.length));
		//TODO: second: add connecting surface
		
		return this.intersectFlatShapes(trace, pgrams);
	}


	public override string exportX3D(Scientrace.Object3dEnvironment env) {
		//TODO: try to transform to indexedlineset
		Scientrace.Vector a, b, c, d, e, f, g, h;
		a = this.width;
		//front circle edges
		b = this.height;
		c = this.width.negative();
		d = this.height.negative();
		e = this.length+this.width;
		//back triangle
		f = this.length+this.height;
		g = this.length-this.width;
		h = this.length-this.height;
//circle front
//circle back (translated out of the "loc" starting point)
		//also end ratation transform. The connecting lines don't need that. 
//lineset for connecting lines to make the cylinder shape visible
		//connecting circles
		
		Scientrace.Plane circleplane = new Scientrace.Plane(new Scientrace.Location(0,0,0), width, height);
		
		
		return "<!-- Start CircularPrism instance -->\n<Transform translation='"+this.loc.trico()+"'>"+circleplane.x3dRotationTransform()+
				"<Shape>" +
				"<Circle2D radius='"+this.width.length+"' containerField='geometry'/>" +
				"</Shape></Transform>"+
					"<Transform translation='"+this.length.trico()+"'>"+circleplane.x3dRotationTransform()+
					"<Shape><Circle2D radius='"+this.width.length+"' containerField='geometry'/>" +
					"</Shape>" +
					"</Transform>" +
					"</Transform>"+"<Shape>" +
					"<LineSet vertexCount='2 2 2 2'>" +
					"<Coordinate point='"+
					a.tricon()+e.tricon()+b.tricon()+f.tricon()+c.tricon()+g.tricon()+d.tricon()+h.tricon()+"'/>" +
					"        </LineSet>     </Shape>   </Transform>\n<!-- End CircularPrism instance -->";
//          <Color color='1 0 0 1 0.5 0 1 1 0 0 1 0 1 0 0 0 0 1 0.8 0 0.8 0.3 0 0.3 1 1 1 0 0 1 1 0 0 0 0 1 1 0.5 0 0.8 0 0.8 1 1 0 0.3 0 0.3 0 1 0 1 1 1'/>
		
	}
}
}
