using System;
using Geotools.CoordinateReferenceSystems;

namespace Geotools.CoordinateReferenceSystems
{
	public class Feet : LinearUnit	
	{
		public Feet() 
			: base(1609.344 * 5280, "", "", "", "Feet", "", "ft")
		{
		}	

		public Feet(double value) 
			: this()
		{
			this.Value = value;
		}	
	}
}
