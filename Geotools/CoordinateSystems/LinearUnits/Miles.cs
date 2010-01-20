using System;

namespace Geotools.CoordinateReferenceSystems
{
	/// <summary>
	/// A strongly typed Miles class.
	/// </summary>
	public class Miles : LinearUnit	
	{
		public Miles() 
			: base(1609.344, "", "", "", "Miles", "", "mi")
		{
		}	

		public Miles(double value) 
			: this()
		{
			this.Value = value;
		}	
	}
}
