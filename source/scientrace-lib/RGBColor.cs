// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;

namespace Scientrace {
public class RGBColor {

	/// <summary>
	/// The fraction of "red" in a colour, [0 - 1]
	/// </summary>
	double red;
	/// <summary>
	/// The fraction of "green" in a colour, [0 - 1]
	/// </summary>
	double green;
	/// <summary>
	/// The fraction of "blue" in a colour, [0 - 1]
	/// </summary>
	double blue;

	public RGBColor(double red_fraction, double green_fraction, double blue_fraction) {
		this.red = red_fraction;
		this.green = green_fraction;
		this.blue = blue_fraction;
		}

	public static RGBColor FromHTML(string html) {
		if (html == null || html.Substring(0,1) != "#") return null;
		double red = 1.0*Convert.ToInt32(html.Substring(1,2), 16) / 255.0;
		double green = 1.0*Convert.ToInt32(html.Substring(3,2), 16) / 255.0;
		double blue = 1.0*Convert.ToInt32(html.Substring(5,2), 16) / 255.0;
		return new RGBColor(red, green, blue);
		}

	public static string twoDigitHex(double fraction) {
		string retstr = ((int)(fraction*255)).ToString("X");
		if (retstr.Length < 2)
			return "0"+retstr;
		return retstr;
		}

	public static string rgbToHtml(double redFraction, double greenFraction, double blueFraction) {
		return "#"
				+RGBColor.twoDigitHex(redFraction)
				+RGBColor.twoDigitHex(greenFraction)
				+RGBColor.twoDigitHex(blueFraction);
		}

	public string toHtml() {
		return RGBColor.rgbToHtml(this.red, this.green, this.blue);
		}

	public bool isValid() {
		if (Double.IsNaN(this.red)) 
			return false;
		if (Double.IsNaN(this.green)) 
			return false;
		if (Double.IsNaN(this.blue)) 
			return false;
		return true;
		}

	public string exportWithGlue(string glue) {
		if (!this.isValid())
			throw new NullReferenceException("Cannot export Vector with NaN values.");
		return this.red.ToString()+glue+this.green.ToString()+glue+this.blue.ToString();
	}

	public string trico() {
		//export as "0.1 0.2 0.3" for red=0.1, green=0.2, blue=0.3. For export purposes.
		return this.exportWithGlue(" ");
	}


}
}

