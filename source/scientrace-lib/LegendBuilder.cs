// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.IO;

namespace Scientrace {
public class LegendBuilder {
	public LegendBuilder() {
	}
		
	/// <summary>
	/// The width of the exported figure, EXCLUDING margins
	/// </summary>
	public int figure_size_width = 800;
		
	/// <summary>
	/// The height of the exported figure, EXCLUDING margins
	/// </summary>
	public int figure_size_height = 64;

	/// <summary>
	/// The orientation of the legend. If not horizontal (default) the figure will be exported vertical.
	/// </summary>
	public bool horizontal = true;
		
		
	public bool isHorizontal() {
		return this.horizontal;
		}
		
	public bool isVertical() {
		return !this.horizontal;
		}

			
}
}

