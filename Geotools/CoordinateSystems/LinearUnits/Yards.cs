using System;

namespace Geotools.CoordinateReferenceSystems
{
	public class Yards : LinearUnit	
	{
		public Yards() 
				: base(1609.344 / 3, "", "", "", "Yards", "", "yds")
		{
		}	

		public Yards(double value) 
				: this()
		{
			this.Value = value;
		}	
	}
}
