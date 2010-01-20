using System;

namespace Geotools.CoordinateReferenceSystems
{
	/// <summary>
	/// Summary description for LinearUnitConverter.
	/// </summary>
	public class LinearUnitConverter
	{
		private LinearUnitConverter()
		{
			
		}

		/// <summary>
		/// Converts a linear unit to miles.
		/// </summary>
		/// <param name="linearUnit">The LinearUnit to convert.</param>
		/// <returns>The distance in Miles.</returns>
		public static Miles ToMiles(LinearUnit linearUnit)
		{
			if (linearUnit == null)
			{
				throw new ArgumentNullException("linearUnit");
			}
			Miles miles = new Miles();
			double unit = linearUnit.ToMeters() / miles.MetersPerUnit;
			return new Miles(unit);
		}

		/// <summary>
		/// Converts a linear unit to km.
		/// </summary>
		/// <param name="linearUnit">The LinearUnit to convert.</param>
		/// <returns>The distance in km.</returns>
		public static Kilometers ToKilometers(LinearUnit linearUnit)
		{
			if (linearUnit == null)
			{
				throw new ArgumentNullException("linearUnit");
			}
			Kilometers km = new Kilometers();
			double unit = linearUnit.ToMeters() / km.MetersPerUnit;
			return new Kilometers(unit);
		}

		/// <summary>
		/// Converts a linear unit to meters.
		/// </summary>
		/// <param name="linearUnit">The LinearUnit to convert.</param>
		/// <returns>The distance in meters.</returns>
		public static Meters ToMeters(LinearUnit linearUnit)
		{
			if (linearUnit == null)
			{
				throw new ArgumentNullException("linearUnit");
			}
			Meters meters = new Meters();
			double unit = linearUnit.ToMeters();
			return new Meters(unit);
		}
	}
}
