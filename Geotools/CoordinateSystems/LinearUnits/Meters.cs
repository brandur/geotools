using System;

namespace Geotools.CoordinateReferenceSystems
{
	/// <summary>
	/// A strongly typed meters class.
	/// </summary>
	public class Meters : LinearUnit	
	{
		public Meters() : base( 1.0 )
		{
			
		}	
		public Meters(double value) : base(1, value)
		{
				
		}	
	}
}
