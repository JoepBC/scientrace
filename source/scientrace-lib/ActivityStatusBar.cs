// /*
//  * Scientrace by Joep Bos-Coenraad
//  * primarily designed for researching concentrator systems
//  * at the Applied Material Science (AMS) department
//  * at the Radboud University Nijmegen, @see http://www.ru.nl/ams .
//  */
using System;
using System.Threading;

namespace Scientrace {
public class ConsoleActivityStatusBar {

		public int icount = 0;
		public int total_items;
		public int barsize = 100;
		int oldpercentage = 1;
		int currentpercentage = 0;
		
		Semaphore sema = new Semaphore(1, 1, "ActivityBarCount");
		

		public ConsoleActivityStatusBar(int bar_size, int total_item_count) {
			this.barsize = bar_size;
			this.total_items = total_item_count;
			this.openBar();
			}
		
		public void openBar() {
			//Console.WriteLine("Shining light, progressbar:");
			Console.Write("{");
			// Write heading of the bar: the "decades" (00000000001111111111222... etc.)
			for (int ib = 0; ib<this.barsize; ib++) { Console.Write(Math.Floor((10.0*ib/this.barsize)%10)); }
			Console.WriteLine("}");
			Console.Write("[");
			// Open the unit bar.
			}
			
		public void closeBar() {
			Console.WriteLine("]");
			}
			
		public void inc() {
			this.sema.WaitOne();
			this.icount++;
			this.currentpercentage = Convert.ToInt32(Math.Floor((this.barsize*(double)this.icount)/this.total_items));
			while (this.oldpercentage <= this.currentpercentage) {
				Console.Write(this.oldpercentage%10);
				this.oldpercentage++;
				}
			this.sema.Release();
			}
	}
}

