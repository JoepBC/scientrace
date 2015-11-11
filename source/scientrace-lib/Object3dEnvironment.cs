// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections;

namespace Scientrace {

public class Object3dEnvironment : Scientrace.Object3dCollection {

		/*
		 * A large sphere in which everything is "packed". The borders will not be shown
		 * in the exported image, but will prevent a ray to "wander forever".
		 */
		
	public double radius;
	public bool perishAtBorder = true;
		
	/// <summary>
	/// In the 3d-view the environment can be drawn with axes labeled X, Y and Z if the labelaxes
	/// boolean is set "true". If it's set false the axes will not be drawn at all.
	/// </summary>
	public bool labelaxes = false;
	public Vector cameraviewpoint;
	public Vector camrotationvector = new Vector(1,0,0);
	public double camrotationangle = 0;
		
	//An environment can have several lightsources
	//Attributes
	public ArrayList lightsources = new ArrayList();
	//Methods
	public void addLightSource(LightSource lightsrc) {
		this.lightsources.Add(lightsrc);
	}
		
		
	public Object3dEnvironment (MaterialProperties mp, double radius, Vector cameraviewpoint) : base(null, mp) {
			this.radius = radius;
			this.cameraviewpoint = cameraviewpoint;
	}
		
	public static Object3dEnvironment dummy() {
		return new Scientrace.Object3dEnvironment(Scientrace.AirProperties.Instance, 1, Scientrace.Vector.ZeroVector());
		}

	public Scientrace.Location traceLeavesEnvironment(Scientrace.Trace trace) {
			trace.currentObject = this;
			Scientrace.Line line = trace.traceline;
			UnitVector dir = line.direction;
			dir.check();
			Vector loc = line.startingpoint;
			/* find locations where line leaves a sphere of radius this.radius around 0,0,0
			 * derivation:
			 * r^2 = |l*dir + loc|^2
			 * hence:
			 * 0 = l^2 * |dir|^2 + 2*l*|dir.loc| + |loc|^2 - r^2    //the "." represents a dotproduct
			 * Solve ABC formula for l:
			 * a = |dir|^2 
			 * b = 2 * (loc . dir)
			 * c = |loc|^2 - r^2 */
			double a = Math.Pow(dir.x,2)+Math.Pow(dir.y,2)+Math.Pow(dir.z,2);
			double b = 2*(loc.x*dir.x+loc.y*dir.y+loc.z*dir.z);
			double c = Math.Pow(loc.x,2)+Math.Pow(loc.y,2)+Math.Pow(loc.z,2)-(Math.Pow(this.radius,2));
			double discriminant = Math.Pow(b,2) - 4* a*c;
			if (discriminant < 0) {
				throw new ArgumentOutOfRangeException("Trace leaves environment from within environment. Are the boundaries of your environment perhaps smaller than your objects?\n Environment radius: "+this.radius+"\n Trace data:"+trace.ToString());
				}
			//ABC formula
			double ans1 = (-b + Math.Sqrt(discriminant)) / (2*a);
			double ans2 = (-b - Math.Sqrt(discriminant)) / (2*a);
			double ans = Math.Max(ans1,ans2);
			//Console.WriteLine("\n"+ans.ToString()+" * "+dir.trico()+"( = "+(dir*ans).trico()+") +"+loc.trico()+" = "+((dir*ans)+loc).toLocation().ToCompactString()+" is ...");
//			throw new AccessViolationException();
			//Console.WriteLine("IT ENDS HERE: "+((dir*Math.Max(ans1,ans2))+loc).toLocation().ToString());
			
			Scientrace.Location leavelocation = (dir*ans+loc).toLocation();
/*			Console.WriteLine("Direction: "+dir.trico()+
				"ABS ANS: "+(Math.Pow(b,2) - 4* a*c)+
				"Location: "+loc.trico());*/
			trace.perish(leavelocation);
			return leavelocation;
		}

	public string x3dShowCoordinates() {
		return this.x3dShowCoordinates(this.radius);
		}

	public string x3dShowCoordinates(double axislength) {
			if (!this.labelaxes) {
				return "";
			}
			return @" <Shape>
        <LineSet vertexCount='9 9 9'>
          <Coordinate point='
-"+axislength+@" 0 0
"+axislength+@" 0 0
"+(axislength*0.95)+@" "+(axislength*0.02)+@" "+(axislength*0.02)+@"
"+axislength+@" 0 0
"+(axislength*0.95)+@" -"+(axislength*0.02)+@" -"+(axislength*0.02)+@"
"+axislength+@" 0 0
"+(axislength*0.95)+@" "+(axislength*0.02)+@" -"+(axislength*0.02)+@"
"+axislength+@" 0 0
"+(axislength*0.95)+@" -"+(axislength*0.02)+@" "+(axislength*0.02)+@"

0 -"+axislength+@" 0
0 "+axislength+@" 0
"+(axislength*0.02)+@" "+(axislength*0.95)+@" "+(axislength*0.02)+@"
0 "+axislength+@" 0
-"+(axislength*0.02)+@" "+(axislength*0.95)+@" -"+(axislength*0.02)+@" 
0 "+axislength+@" 0
-"+(axislength*0.02)+@" "+(axislength*0.95)+@" "+(axislength*0.02)+@" 
0 "+axislength+@" 0
"+(axislength*0.02)+@" "+(axislength*0.95)+@" -"+(axislength*0.02)+@"

0 0 -"+axislength+@"
0 0 "+axislength+@"
"+(axislength*0.02)+@" "+(axislength*0.02)+@" "+(axislength*0.95)+@"
0 0 "+axislength+@"
-"+(axislength*0.02)+@" -"+(axislength*0.02)+@" "+(axislength*0.95)+@"
0 0 "+axislength+@"
"+(axislength*0.02)+@" -"+(axislength*0.02)+@" "+(axislength*0.95)+@" 
0 0 "+axislength+@"
-"+(axislength*0.02)+@" "+(axislength*0.02)+@" "+(axislength*0.95)+@" 
'/>
<Color color='
1 1 1 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0 
1 1 1 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0 
1 1 1 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0 0 1 0
' />
</LineSet>
          
</Shape>
     <Transform scale='"+this.radius/10+" "+this.radius/10+" "+this.radius/10+@"' translation='"+axislength+@" 0 0'>
		<Transform rotation='0 1 0 -"+Math.PI/2+@"'>
        <Shape>
          <Text solid='true' string='""X""' />
          <Appearance>
            <Material diffuseColor='1 0 0'/>
          </Appearance>
        </Shape>
      </Transform>
        <Shape>
          <Text solid='false' string='""X""' />
          <Appearance>
            <Material diffuseColor='1 0 0'/>
          </Appearance>
        </Shape>
      </Transform>

	<Transform scale='"+this.radius/10+" "+this.radius/10+" "+this.radius/10+@"' translation='0 "+axislength+@" 0'>
		<Transform rotation='0 1 0 -"+Math.PI/2+@"'>
        <Shape>
          <Text solid='true' string='""Y""' />
          <Appearance>
            <Material diffuseColor='1 0 0'/>
          </Appearance>
        </Shape>
      </Transform>
        <Shape>
          <Text solid='false' string='""Y""' />
          <Appearance>
            <Material diffuseColor='1 0 0'/>
          </Appearance>
        </Shape>
      </Transform>

     <Transform scale='"+this.radius/10+" "+this.radius/10+" "+this.radius/10+@"' translation='0 0 "+axislength+@"'>
		<Transform rotation='0 1 0 -"+Math.PI/2+@"'>
        <Shape>
          <Text solid='true' string='""Z""' />
          <Appearance>
            <Material diffuseColor='1 0 0'/>
          </Appearance>
        </Shape>
      </Transform>
		<Shape>
          <Text solid='true' string='""Z""' />
          <Appearance>
            <Material diffuseColor='1 0 0'/>
          </Appearance>
        </Shape>
      </Transform>
";
 
		}

	public string exportX3D() {
			//old cameraviewpoint: 0 -10 75
		return @"<?xml version='1.0' encoding='UTF-8'?>
<!DOCTYPE X3D PUBLIC 'ISO//Web3D//DTD X3D 3.0//EN' 'http://www.web3d.org/specifications/x3d-3.0.dtd'>
<X3D profile='Immersive' version='3.0' xmlns:xsd='http://www.w3.org/2001/XMLSchema-instance' xsd:noNamespaceSchemaLocation='http://www.web3d.org/specifications/x3d-3.0.xsd'>
  <Scene>
<Viewpoint description='LineSet cube close up' position='"+this.cameraviewpoint.trico()+
	@"' orientation='"+this.camrotationvector.trico()+@" "+this.camrotationangle+@"'/>
" + this.x3dShowCoordinates()+
	this.exportX3D(this) +
	TraceJournal.Instance.exportX3D(this) +
@"  </Scene>
</X3D>";
	}

}
}	
