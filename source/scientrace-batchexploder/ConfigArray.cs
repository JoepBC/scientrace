using System;
namespace BatchExplode {

public abstract class ConfigArray {

	public string name ="";
		
	public ConfigArray () {
		}
		
	public abstract void reset();
		
	public abstract void inc();
	
	public abstract bool EOF();
		
	public abstract string replaceForCurrentValues(string aString);
		
	public virtual string replaceForCurrentFilenameValues(string aString) {
		return this.replaceForCurrentValues(aString);	
		}

	}}