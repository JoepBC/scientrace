// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace Scientrace {


public abstract class LightSource {

	public int lightsource_shine_threads = 1;

	private List<Scientrace.Trace> traces = new List<Scientrace.Trace>();
	Stack<Scientrace.Trace> traceStack = null;
	Semaphore traceStackSema = new Semaphore(1, 1, "TraceStackSema");
	ConsoleActivityStatusBar bar = null;

	public LightSpectrum spectrum;
	public OpticalEfficiencyCharacteristics efficiency_characteristics;
	
	public List<Scientrace.UniformTraceModifier> lightsource_modifiers = new List<UniformTraceModifier>();
	
	public Object3dEnvironment env;
	public string tag;
	public double minimum_intensity_fraction = 0.001; //traces of less than 5% intensity will not be calculated.
	public double max_interactions = 10;
	public double weighted_intensity;

	public bool mandatory_spectrum = true;

	public Dictionary<Object3d,double> objectRevenues = new Dictionary<Object3d, double>();



	/// <summary>
	/// Very often, a lightsource is meant to light a certain area, but starting just there makes it difficult to
	/// see the direction of the light entering that surface. When a distance is added, the traces start this distance "earlier"
	/// enabling them to be better interpreted by the 3D-export viewer.
	/// </summary>
	public double distance = 0;
				
	/// <summary>
	/// Time (seconds) needed for shining this lightsource.
	/// </summary>
	public TimeSpan shine_duration;
		
	public double total_lightsource_intensity = 0;

				
	public LightSource(ShadowScientrace.ShadowLightSource aShadowLightSource) {
		this.setObjectEnvironment(aShadowLightSource.env);
		this.minimum_intensity_fraction = aShadowLightSource.getDouble("minimum_intensity_fraction", this.minimum_intensity_fraction);
		this.addTraceModifiers((IEnumerable<Scientrace.UniformTraceModifier>)aShadowLightSource.getObject("trace_modifiers", true));

		//AND it was put back like a normal ShadowClass object...
		//this.spectrum = aShadowLightSource.spectrum;
		this.spectrum = (Scientrace.LightSpectrum)aShadowLightSource.getObject("spectrum", !this.mandatory_spectrum);
		if (spectrum == null)
			Console.WriteLine("WARNING: No spectrum given.");
		this.efficiency_characteristics = (Scientrace.OpticalEfficiencyCharacteristics)aShadowLightSource.getObject("efficiency_characteristics");
		
		this.weighted_intensity = (double)(aShadowLightSource.getNDouble("weighted_intensity") ?? this.spectrum.total_intensity);
		}
		
	public void addTraceModifiers(IEnumerable<Scientrace.UniformTraceModifier> modifiersCollection) {
		if (modifiersCollection == null)
			return;
		this.lightsource_modifiers.AddRange(modifiersCollection);
		}
		
	public LightSource(Object3dEnvironment env) {
		this.setObjectEnvironment(env);
		}

	public void setObjectEnvironment(Object3dEnvironment env) {
		this.tag = this.defaultTag();
		//a lightsource works on an environment
		this.env = env;
		//but is also part of that environment (possibly amongst other lightsources)
		this.env.addLightSource(this);
		}

	public virtual string defaultTag() {	
		return this.GetType().ToString();
		}


	public void addTrace(Scientrace.Trace aTrace) {
		List<Scientrace.Trace> inTraces = new List<Scientrace.Trace>();
		List<Scientrace.Trace> outTraces = new List<Scientrace.Trace>();
		
		// No modifiers present? Add normal trace
		if (this.lightsource_modifiers.Count < 1) {
			aTrace.traceline.rewind(this.distance);
			this.traces.Add(aTrace);
			return;
			}
		
		// Modifiers present! Add (only) modified traces.
		inTraces.Add(aTrace);		
		foreach (Scientrace.UniformTraceModifier aModifier in this.lightsource_modifiers) {
			outTraces.AddRange(this.modifiedTraces(inTraces, aModifier));
			}
		foreach (Scientrace.Trace tTrace in outTraces) {
			tTrace.traceline.rewind(this.distance);
			}
		this.traces.AddRange(outTraces);
		}
		

	public void addRecursiveModifiedTrace(Scientrace.Trace aTrace) {
		List<Scientrace.Trace> tTraces = new List<Scientrace.Trace>();
		tTraces.Add(aTrace);
				
		foreach (Scientrace.UniformTraceModifier aModifier in this.lightsource_modifiers) {
			tTraces = this.modifiedTraces(tTraces, aModifier);
			}
		this.traces.AddRange(tTraces);
		}
	

	public List<Scientrace.Trace> modifiedTraces(List<Scientrace.Trace> inputTraces, Scientrace.UniformTraceModifier modifier) {
		List<Scientrace.Trace> retTraces = new List<Scientrace.Trace>();
		foreach (Scientrace.Trace iTrace in inputTraces) {
			if (modifier.add_self) {
				retTraces.Add(iTrace);
				}
			for (int node = 1; node <= modifier.modify_traces_count; node++) {
				retTraces.Add(modifier.modify(iTrace, node, modifier.modify_traces_count));
				}
			}
		return retTraces;
		}

	public int traceCount() {
		return this.traces.Count;
		}


	/// <summary>
	/// Export an "XML single trace" representation of the Light Source. This way, the lightsource traces
	// can be scrutinised for study, or reproduced as is.
	/// </summary>
	/// <returns>An XML LightSource element with a customtraces class and sub-elements.</returns>
	public XElement exportCustomTracesXML() {
		XElement xlight = new XElement("LightSource");
		xlight.Add(new XAttribute("Class", "CustomTraces"));
		xlight.Add(new XAttribute("Intensity", this.weighted_intensity.ToString()));
		xlight.Add(new XAttribute("MaxInteractions", this.max_interactions));
		xlight.Add(new XAttribute("MinIntensity", this.minimum_intensity_fraction.ToString()));
		xlight.Add(new XAttribute("Tag", "export_from_"+this.tag));
		foreach (Scientrace.Trace aTrace in this.traces) {
			XElement xtrace = new XElement("Trace");
			xtrace.Add(new XAttribute("Wavelength", aTrace.wavelenght));
			xtrace.Add(new XAttribute("Intensity", aTrace.intensity.ToString()));
			xtrace.Add(new XAttribute("Tag", "#"+aTrace.GetHashCode().ToString()));
			XElement loc = new XElement("Location");
			loc.Add(new XAttribute("x", aTrace.traceline.startingpoint.x));
			loc.Add(new XAttribute("y", aTrace.traceline.startingpoint.y));
			loc.Add(new XAttribute("z", aTrace.traceline.startingpoint.z));
			xtrace.Add(loc);
			XElement dir = new XElement("Direction");
			dir.Add(new XAttribute("x", aTrace.traceline.direction.x));
			dir.Add(new XAttribute("y", aTrace.traceline.direction.y));
			dir.Add(new XAttribute("z", aTrace.traceline.direction.z));
			xtrace.Add(dir);
			xlight.Add(xtrace);
			}
		return xlight;
		}

/*
	public void addNewdirTraceClone(Trace originalTrace, NonzeroVector newDir, double distance) {
		Scientrace.Line newLine = new Scientrace.Line( (originalTrace.traceline.startingpoint +
				originalTrace.traceline.direction.toVector()*distance - newDir.normalized().toVector()*distance).toLocation(),
			                                              newDir.toUnitVector());
//		this.traces.Add(new Trace(originalTrace, newLine));
		this.traces.Add(originalTrace.fork(newLine));
		}
*/

	void calculateTotalIntensity() {
		double intensity = 0;
		foreach (Scientrace.Trace trace in this.traces) {
			intensity += trace.intensity;
			}
		this.total_lightsource_intensity = intensity;
		}


//	public void emptyStack(Stack<Scientrace.Trace> traceStack, ActivityStatusBar bar) {
	public void emptyStack() {
		while (this.traceStack.Count > 0) { // condition is not really relevant, after hitting 0 it will return anyway.
			//LOCK STACK SEMAPHORE
			this.traceStackSema.WaitOne();
			//CHECK AVAILABILITY TRACES
			if (this.traceStack.Count < 1)
				return; //stack is empty, work done.
			//TRACE AVAILABLE: POP IT
			Trace tTrace = this.traceStack.Pop();
			//RELEASE SEMAPHORE
			this.traceStackSema.Release();
			//PERFORM HEAVY TASK
			tTrace.cycle();
			this.bar.inc();
			}
		}

	/// <summary>
	/// if not done at Construction, start traces can be created in the overriding of this method.
	/// </summary>
	public virtual void createStartTraces() {
		//possible implementation by subclasses
		}

	/// <summary>
	/// If the traces collection of the LightSource is empty, th createStartTraces() method is called
	/// which is implemented by some LightSource subclasses that do not create these traces on construction.
	/// </summary>
	public void createStartTracesOnEmpty() {
		if (this.traces.Count <= 0) {
			//The lightsource is still empty on traces and should be filled/created
			this.createStartTraces();
			return;
			} else {
			//The lightsource has already been created, do nothing
			return;
			}
		}

	public double shine() {
		//Make sure the traces collection for the LightSource is assigned some traces
		this.createStartTracesOnEmpty();		
	
		//starting timer
		DateTime startTime = DateTime.Now;

		this.calculateTotalIntensity();
		int tcount = this.traceCount();
		
		/* CODE BELOW SHOULD REPLACE OLD NON_THREADED ROUTINE:
		foreach (Scientrace.Trace trace in this.traces) {
			trace.cycle();
			}
		*/
		this.bar = new ConsoleActivityStatusBar(100, tcount);
		
		//create a stack with all traces, to be popped by thread processes below.
		this.traceStack = new Stack<Scientrace.Trace>(this.traces);

		List<Thread> threads = new List<Thread>();

		//number threads from 0 to lightsource_shine_threads - 1
		for (int iThread = 0; iThread < this.lightsource_shine_threads; iThread++) {
			Thread stackPopThread = new Thread(this.emptyStack);
			stackPopThread.Start();
			threads.Add(stackPopThread);
			Thread.Sleep(50);
			}
		foreach (Thread aThread in threads) {
			//Wainit for all threads to finish
			aThread.Join();
			}
		bar.closeBar();

		//stopping timer
		DateTime stopTime = DateTime.Now; 
		this.shine_duration = (stopTime-startTime);
			
		this.addObjectRevenues();
			
		return this.total_lightsource_intensity;
		}
		
	public double revenueForObject(Object3d object3d) {
		return this.objectRevenues[object3d];
		}
			
		
	public void addObjectRevenues() {
		Scientrace.TraceJournal tj = Scientrace.TraceJournal.Instance;
		foreach (PhysicalObject3d o3d in tj.registeredPerformanceObjects) {
			this.objectRevenues.Add(o3d, o3d.getRevenue());
			o3d.flushRevenue();
			}
		}

	}}
