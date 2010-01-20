using System;

namespace Geotools.IO
{ 
	/// <summary>
	/// Specifies the geometry type.
	/// </summary>
	public enum WKBGeometryType
	{
		/// <summary>
		/// Point.
		/// </summary>
		WKBPoint = 1,
		/// <summary>
		/// Line string.
		/// </summary>
		WKBLineString = 2,
		/// <summary>
		/// Polygon.
		/// </summary>
		WKBPolygon = 3,
		/// <summary>
		/// Multi point.
		/// </summary>
		WKBMultiPoint = 4,
		/// <summary>
		/// Multi line string.
		/// </summary>
		WKBMultiLineString = 5,
		/// <summary>
		/// Multi polygon.
		/// </summary>
		WKBMultiPolygon = 6,
		/// <summary>
		/// Geometry collection.
		/// </summary>
		WKBGeometryCollection = 7
	}
}
