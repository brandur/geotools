using System;
using System.Diagnostics;
using System.Drawing;
using com.vividsolutions.jts.geom;
using Geotools.CoordinateReferenceSystems;
namespace Geotools.Utilities
{
	/// <summary>
	/// Calculates the distance between to lat/long points.
	/// </summary>
	public sealed class AirDistance 
	{
		private AirDistance()
		{
			// private constructor - to avoid FxCopy warning.
		}
		/// <summary>
		/// Calculates the distance between to lat/long points.
		/// </summary>
		/// <param name="coordinate1">Coordinate in degrees.</param>
		/// <param name="coordinate2">Coordinate in degrees.</param>
		/// <returns>Distance between the two points in Kilometers.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If the coordinates are not valid latitude/ longitude coordinates.</exception>
		public static Kilometers Calculate(Coordinate coordinate1, Coordinate coordinate2)
		{
			if (coordinate1 == null)
			{
				throw new ArgumentNullException("coordinate1");
			}
			if (coordinate2 == null)
			{
				throw new ArgumentNullException("coordinate2");
			}
			return Calculate(coordinate1.x, coordinate1.y, coordinate2.x, coordinate2.y );
		}

		/// <summary>
		/// Calculates the distance between to lat/long points.
		/// </summary>
		/// <param name="point1"></param>
		/// <param name="point2"></param>
		/// <returns>Distance between the two points in Kilometers.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If the coordinates are not valid latitude/ longitude coordinates.</exception>
		public static Kilometers Calculate(PointF point1, PointF point2)
		{
			return Calculate(point1.X, point1.Y, point2.X, point2.Y);
		}


		/// <summary>
		///  Calculates the distance between to lat/long points.
		/// </summary>
		/// <param name="x1">The first x coordinate in degrees.</param>
		/// <param name="y1">The first y coordinate in degrees.</param>
		/// <param name="x2">The second x coordinate in degrees.</param>
		/// <param name="y2">The second y coordinate in degrees.</param>
		/// <returns>Distance between the two points in Kilometers.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If the coordinates are not valid latitude/ longitude coordinates.</exception>
		public static Kilometers Calculate(double x1, double y1, double x2, double y2)
		{
			if (x1 < -180 || x1 > 180)
			{
				throw new ArgumentOutOfRangeException("x1",x1,"Must be a valid lat/long.");
			}
			if (x2 < -180 || x2 > 180)
			{
				throw new ArgumentOutOfRangeException("x2",x2,"Must be a valid lat/long.");
			}
			if (y1 < -90 || y1 > 90)
			{
				throw new ArgumentOutOfRangeException("y1",y1,"Must be a valid lat/long.");
			}
			if (y2 < -90 || y2 > 90)
			{
				throw new ArgumentOutOfRangeException("y2",y2,"Must be a valid lat/long.");
			}

			double factor = (Math.PI / 180d);
			double a1 = y1 * factor ;
			double b1 = x1 * factor;
			double a2 = y2 * factor;
			double b2 = x2 * factor;

			double r = 6378; // Mean radius of the earth in kilometers

			double distance =  r * Math.Acos(Math.Cos(a1) * Math.Cos(b1) * Math.Cos(a2) * Math.Cos(b2) + Math.Cos(a1) * Math.Sin(b1) * Math.Cos(a2) * Math.Sin(b2) + Math.Sin(a1) * Math.Sin(a2));			
			return new Kilometers(distance);
		}
	}
}
