using System;

namespace Geotools.CoordinateReferenceSystems
{
	/// <summary>
	/// This enumerataion is used to determine what type of units (km vs. miles) to use.
	/// </summary>
	public enum DistanceUnitSystem
	{
		/// <summary>
		/// The metric system (Kilometers/ meters/ centimeters).
		/// </summary>
		Metric,
		/// <summary>
		/// The imperial/ English system (miles, inches, feet).
		/// </summary>
		English
	}
}
