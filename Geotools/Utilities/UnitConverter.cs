using System;

namespace Geotools.Utilities
{
	/// <summary>
	/// Converts values from one unit of measure to another.
	/// </summary>
	public class UnitConverter
	{
		private UnitConverter()
		{
			// Prevent instantiation
		}

		#region Metric To Metric - Distance

		/// <summary>
		/// Converts a specified kilometer value to meters.
		/// </summary>
		/// <param name="kilometers">A kilometer value.</param>
		/// <returns>The <i>kilometers</i> value in meters.</returns>
		public static double KilometersToMeters(double kilometers)
		{
			return kilometers * 1000d;
		}

		/// <summary>
		/// Converts a specified meter value to kilometers.
		/// </summary>
		/// <param name="meters">A meter value.</param>
		/// <returns>The <i>meters</i> value in kilometers.</returns>
		public static double MetersToKilometers(double meters)
		{
			return meters / 1000d;
		}

		/// <summary>
		/// Converts a specified meter value to centimeters.
		/// </summary>
		/// <param name="meters">A meter value.</param>
		/// <returns>The <i>meters</i> value in centimeters.</returns>
		public static double MetersToCentimeters(double meters)
		{
			return meters * 100d;
		}

		/// <summary>
		/// Converts a specified centimeter value to millimeters.
		/// </summary>
		/// <param name="centimeters">A centimeter value.</param>
		/// <returns>The <i>centimeters</i> value in millimeters.</returns>
		public static double CentimetersToMillimeters(double centimeters)
		{
			return centimeters * 10d;
		}

		#endregion

		#region Metric To English - Distance

		/// <summary>
		/// Converts a specified meter value to feet.
		/// </summary>
		/// <param name="meters">A meter value.</param>
		/// <returns>The <i>meters</i> value in feet.</returns>
		public static double MetersToFeet(double meters)
		{
			return meters * 3.28d;
		}

		/// <summary>
		/// Converts a specified meter value to yards.
		/// </summary>
		/// <param name="meters">A meter value.</param>
		/// <returns>The <i>meters</i> value in yards.</returns>
		public static double MetersToYards(double meters)
		{
			return meters *  1.0936133d;
		}

		/// <summary>
		/// Converts a specified kilometer value to miles.
		/// </summary>
		/// <param name="kilometers">A kilometer value.</param>
		/// <returns>The <i>kilometers</i> value in miles.</returns>
		public static double KilometersToMiles(double kilometers)
		{
			return kilometers * 0.62d;
		}

		#endregion

		#region English To Metric - Distance

		/// <summary>
		/// Converts a specified inch value to centimeters.
		/// </summary>
		/// <param name="inches">An inch value.</param>
		/// <returns>The <i>inches</i> value in centimeters.</returns>
		public static double InchesToCentimeters(double inches)
		{
			return inches * 2.54d;
		}

		/// <summary>
		/// Converts a specified feet value to meters.
		/// </summary>
		/// <param name="feet">A feet value.</param>
		/// <returns>The <i>feet</i> value in meters.</returns>
		public static double FeetToMeters(double feet)
		{
			return feet * 0.305;
		}

		/// <summary>
		/// Converts a specified mile value to kilometers.
		/// </summary>
		/// <param name="miles">A mile value.</param>
		/// <returns>The <i>miles</i> value in kilometers.</returns>
		public static double MilesToKilometers(double miles)
		{
			return miles * 1.609d;
		}

		/// <summary>
		/// Converts a specified mile value to meters.
		/// </summary>
		/// <param name="miles">A mile value.</param>
		/// <returns>The <i>miles</i> value in meters.</returns>
		public static double MilesToMeters(double miles)
		{
			return UnitConverter.KilometersToMeters(UnitConverter.MilesToKilometers(miles));
		}

		#endregion

		#region English To English - Distance

		/// <summary>
		/// Converts a specified mile value to feet.
		/// </summary>
		/// <param name="miles">A mile value.</param>
		/// <returns>The <i>miles</i> value in feet.</returns>
		public static double MilesToFeet(double miles)
		{
			return miles * 5280d;
		}

		/// <summary>
		/// Converts a specified feet value to miles.
		/// </summary>
		/// <param name="feet">A feet value.</param>
		/// <returns>The <i>feet</i> value in miles.</returns>
		public static double FeetToMiles(double feet)
		{
			return feet / 5280d;
		}

		#endregion
	}
}
