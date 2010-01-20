using System;

namespace Geotools.CoordinateReferenceSystems
{
	/// <summary>
	/// A strongly typed Kilometers class.
	/// </summary>
	public class Kilometers : LinearUnit	
	{
		public Kilometers() 
			: base(1000, "", "", "", "Kilometers", "", "km")
		{
		}
	
		public Kilometers(double value) 
			: this()
		{
			this.Value = value;
		}	
	}
}
