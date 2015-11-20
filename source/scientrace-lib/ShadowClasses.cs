// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Collections.Generic;

namespace ShadowScientrace {



/*****************************************************************
 *  (parent)
 * SHADOWCLASS
 *
 *****************************************************************/

public class ShadowClass {

	public int? factory_method;
	public Type class_type;
	public string tag = null;
	public string debug_data = null;
	public Dictionary<string, object> arguments = new Dictionary<string, object>();

	public string printArgument(string varname) {
		if(!this.hasArgument(varname))
			return "{not set}";
		if(this.argumentIsNull(varname))
			return "{null}";
		return this.getObject(varname).ToString();
		}

	public string getDebugData() {
		return (this.debug_data == null?"[no debug data set]":this.debug_data);
		}
			

	public bool argumentIsNull(string varname) {
		return !this.hasArgument(varname) || (this.arguments[varname] == null);
		}

	public Scientrace.RGBColor getColorFromHTML(string varname, Scientrace.RGBColor defval) {
		Scientrace.RGBColor retval;
		retval = Scientrace.RGBColor.FromHTML(this.getString(varname));
		if (retval == null) return defval;
		return retval;
		}

	public Scientrace.RGBColor getColorFromHTML(string varname) {
		return Scientrace.RGBColor.FromHTML(this.getString(varname));
		}

	public Scientrace.NonzeroVector getNzVector(string varname) {
		return (Scientrace.NonzeroVector)this.getObject(varname, false);
		}

	public Scientrace.UnitVector getUnitVector(string varname) {
		return (Scientrace.UnitVector)this.getObject(varname, false);
		}

	public Scientrace.Vector getVector(string varname, Scientrace.Vector defval) {
		Scientrace.Vector retval = (Scientrace.Vector)this.getObject(varname, true);
		if (retval == null) 
			return defval;
		return retval;
		}

	public Scientrace.Vector getVector(string varname) {
		return (Scientrace.Vector)this.getObject(varname, false);
		}

	public Scientrace.Location getLocation(string varname) {
		return (Scientrace.Location)this.getObject(varname, false);
		}

	public bool getBool(string varname, bool defaultvalue) {
		bool? retval = (bool?)this.getObject(varname, true);
		if (retval == null)
			return defaultvalue;
		return (bool)retval;
		}

	public bool getBool(string varname) {
		return (bool)this.getObject(varname, false);
		}

	public bool? getNBool(string varname) {
		return (bool?)this.getObject(varname, true);
		}
				
	public double getDouble(string varname, double defaultvalue) {
		double? retval = (double?)this.getObject(varname, true);
		if (retval == null)
			return defaultvalue;
		return (double)retval;
		}

	public double getDouble(string varname) {
		return (double)this.getObject(varname);
		}

	public double? getNDouble(string varname) {
		return (double?)this.getObject(varname, true);
		}
				
		
	public int getInt(string varname, int defaultvalue) {
		int? retval = (int?)this.getObject(varname, true);
		if (retval == null)
			return defaultvalue;
		return (int)retval;
		}

	public int getInt(string varname) {
		return (int)this.getObject(varname, false);
		}

	public int? getNInt(string varname) {
		return (int?)this.getObject(varname, true);
		}

	public string getString(string varname) {
		return (string)this.getObject(varname, true);
		}

	/// <summary>
	/// Gets an "object" instance which may NOT be null. Use the 2-param. version for nullable values.
	/// </summary>
	/// <returns>The object from the dictionary</returns>
	/// <param name="varname">Varname.</param>
	public object getObject(string varname) {
		return this.getObject(varname, false);
		}

	public object getObject(string varname, bool nullable) {
		if (this.arguments.ContainsKey(varname)) {
			if (!nullable && this.arguments[varname] == null)
					throw new ArgumentNullException("Parameter {"+varname+"} is not set for Shadowtype "+this.typeString()+" and Factory Method "+this.factory_method+"\n\nDebugdata: \n"+this.getDebugData());
			return this.arguments[varname];
			}
		if (nullable) { return null; }
		throw new KeyNotFoundException("No key {"+varname+"} found for Shadowtype "+this.typeString());
		}
	
	public bool hasArgument(string argName) {
		return this.arguments.ContainsKey(argName)&&(this.arguments[argName]!=null);
		}		

	public bool hasTag() {
		return this.tag != null;
		}

	public string typeString() {
		return this.class_type.ToString();
		}
	
	public object factory(int fac_meth) {
		this.factory_method = fac_meth;
		return this.factory();
		}
	
	public object factory() {
		if (this.factory_method == null) {
			throw new ArgumentNullException("A ShadowClass cannot be factorized when the objects factory_method==null for Shadowtype "+this.typeString());
			}
		object a = Activator.CreateInstance(this.class_type, new ShadowClass[]{this});
		return a;
		}		
	} //end ShadowClass


/*****************************************************************
 *
 * SHADOWLIGHTSOURCE
 *
 *****************************************************************/

public class ShadowLightSource : ShadowClass {

	public Scientrace.Object3dEnvironment env;
	//Special property removed at 20151021 public Scientrace.LightSpectrum spectrum;
	public string class_name;

	public ShadowLightSource(Type lightSourceType, Scientrace.Object3dEnvironment anObject3dEnvironment) { //Special property removed at 20151021 , Scientrace.LightSpectrum aSpectrum) {
		this.env = anObject3dEnvironment;
		//Special property removed at 20151021 this.spectrum = aSpectrum;
		this.class_type = lightSourceType;
		}

	public ShadowLightSource(ShadowLightSource copyObject) {
		//Special property removed at 20151021 this.spectrum = copyObject.spectrum;
		this.class_type = copyObject.class_type;
		this.factory_method = copyObject.factory_method;
		this.arguments = new Dictionary<string, object>(copyObject.arguments);
		}
	
	public new Scientrace.LightSource factory(int fac_meth) {
		this.factory_method = fac_meth;
		return this.factory();
		}
	
	public new Scientrace.LightSource factory() {	
		/* NO LONGER TRUE DUE TO ADDITION OF CUSTOMTRACES
		//Special property removed at 20151021 
		if (this.spectrum==null) {
			throw new ArgumentNullException("A ShadowLightSource cannot be factorized when spectrum==null for Shadowtype "+this.typeString());
			} */
		Scientrace.LightSource a = (Scientrace.LightSource)base.factory();
		return a;
		}
	
	} //end ShadowLightSource


/*****************************************************************
 *
 * SHADOWOBJECT3D
 *
 *****************************************************************/

public class ShadowObject3d : ShadowClass {
	
	public Scientrace.Object3dCollection parent;
	public Scientrace.MaterialProperties materialprops;

	public bool register_performance = false;
	public bool register_distribution = false;

	public double? parseorder = null;

	public ShadowObject3d(Type object3dType, Scientrace.Object3dCollection parent, Scientrace.MaterialProperties materialprops) {
		this.parent = parent;
		this.materialprops = materialprops;
		this.class_type = object3dType;
		}

	public ShadowObject3d(ShadowObject3d copyObject) {
		this.parent = copyObject.parent;
		this.materialprops = copyObject.materialprops;
		this.class_type = copyObject.class_type;
		this.factory_method = copyObject.factory_method;
		this.arguments = new Dictionary<string, object>(copyObject.arguments);
		}
	
	public new Scientrace.Object3d factory(int fac_meth) {
		this.factory_method = fac_meth;
		return this.factory();
		}

	public bool hasParseOrder() {
		return this.parseorder != null;
		}

	public new Scientrace.Object3d factory() {	
		if (this.parent==null) {
			throw new ArgumentNullException("A ShadowObject3d cannot be factorized when parent==null for Shadowtype "+this.typeString());
			}
		if (this.materialprops==null) {
			throw new ArgumentNullException("A ShadowObject3d cannot be factorized when materialprops==null for Shadowtype "+this.typeString());
			}
		Scientrace.Object3d a = (Scientrace.Object3d)base.factory();
		if (this.register_performance)
			Scientrace.TraceJournal.Instance.registerObjectPerformance(a);
		//20151021: The DistributionSquare functionality has been removed as it was no longer considered useful.
		/*		if (this.register_distribution) 
			Scientrace.TraceJournal.Instance.registerDistributionObject(a); */
		return a;
		}

	}}

