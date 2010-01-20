/*
 *  Copyright (C) 2002 Urban Science Applications, Inc. 
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 */

using System;
using System.IO;
using System.Collections;
using com.vividsolutions.jts.geom;

namespace Geotools.IO
{
	/// <summary>
	///  Write <b>Geometry</b> objects as Well-Known Binary.
	/// </summary>
	/// <remarks>
	///  <para>
	///  The Well-known Binary format is defined in the 
	///  OpenGIS Simple Features Specification for SQL
	///  </para>
	/// </remarks> 
	public class GeometryWKBWriter
	{
		private BinaryWriter _writer;
		private GeometryFactory _factory;

		/// <summary>
		/// Initializes a new instance of the <see cref="GeometryWKBWriter">GeometryWKBWriter</see> class.
		/// </summary>
		/// <param name="writer">The <b>GeometryFactory</b> used when writing geometries.</param>
		/// <param name="factory">The <b>GeometryFactory</b> used when writing geometries.</param>
		public GeometryWKBWriter(BinaryWriter writer, GeometryFactory factory)
		{
			_writer = writer;
			_factory = factory;
		}

		/// <summary>
		/// Returns a byte[] containing the supplied <b>Geometry</b> object's Well-known binary representation.
		/// </summary>
		/// <param name="geometry">The <b>Geometry</b> object.</param>
		/// <param name="factory">The <b>GeometryFactory</b>.</param>
		/// <param name="byteOrder">The desired <see cref="WKBByteOrder">WKBByteOrder</see>.</param>
		/// <returns>A byte[] containing the supplied <b>Geometry</b> object's Well-known binary representation.</returns>
		public static byte[] GetBytes(Geometry geometry, GeometryFactory factory, WKBByteOrder byteOrder)
		{
			if (geometry == null)
			{
				throw new ArgumentNullException("geometry");
			}

			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(ms))
				{
					new GeometryWKBWriter(writer, factory).WriteGeometry(geometry, byteOrder);  
				
					return ms.ToArray();
				}
			}
		}

		/// <summary>
		/// Writes the <b>Geometry</b> object.
		/// </summary>
		/// <param name="geometry">The <b>Geometry</b> object to write.</param>
		/// <param name="byteOrder">The desired <see cref="WKBByteOrder">WKBByteOrder</see>.</param>
		public void Write(Geometry geometry, WKBByteOrder byteOrder)
		{
			// Write the geometry
			this.WriteGeometry(geometry, byteOrder);
		}

		private void WriteType(Geometry geometry, WKBByteOrder byteOrder)
		{
			switch(geometry.getGeometryType().ToUpper())
			{
				case "POINT":
					this.Write((uint)WKBGeometryType.WKBPoint, byteOrder);
					break;

				case "LINESTRING":
					this.Write((uint)WKBGeometryType.WKBLineString, byteOrder);
					break;

				case "POLYGON":
					this.Write((uint)WKBGeometryType.WKBPolygon, byteOrder);
					break;

				case "MULTIPOINT":
					this.Write((uint)WKBGeometryType.WKBMultiPoint, byteOrder);
					break;

				case "MULTILINESTRING":
					this.Write((uint)WKBGeometryType.WKBMultiLineString, byteOrder);
					break;

				case "MULTIPOLYGON":
					this.Write((uint)WKBGeometryType.WKBMultiPolygon, byteOrder);
					break;

				case "GEOMETRYCOLLECTION":
					this.Write((uint)WKBGeometryType.WKBGeometryCollection, byteOrder);
					break;

				default:
					throw new ArgumentException("Invalid Geometry Type");
			}
		}

		private void WriteGeometry(Geometry geometry, WKBByteOrder byteOrder)
		{
			switch (geometry.getGeometryType().ToUpper())
			{
				case "POINT":
					this.WritePoint((Point)geometry, byteOrder, true);
					break;

				case "LINESTRING":
					this.WriteLineString((LineString)geometry, byteOrder, true);
					break;

				case "POLYGON":
					this.WritePolygon((Polygon)geometry, byteOrder);
					break;

				case "MULTIPOINT":
					this.WriteMultiPoint((MultiPoint)geometry, byteOrder);
					break;

				case "MULTILINESTRING":
					this.WriteMultiLineString((MultiLineString)geometry, byteOrder);
					break;

				case "MULTIPOLYGON":
					this.WriteMultiPolygon((MultiPolygon)geometry, byteOrder);
					break;

				case "GEOMETRYCOLLECTION":
					this.WriteGeometryCollection((GeometryCollection)geometry, byteOrder);
					break;

				default:
					throw new ArgumentException("Invalid Geometry Type");
			}
		}

		private void WritePoint(Point point, WKBByteOrder byteOrder, bool writeHeader)
		{
			if (writeHeader)
			{
				// Write byte order
				_writer.Write((byte)byteOrder);

				// Write type
				this.WriteType(point, byteOrder);
			}

			// Write the x coordinate.
			this.Write(point.getX(), byteOrder);

			// Write the y coordinate.
			this.Write(point.getY(), byteOrder);
		}

		private void WriteLineString(LineString ls, WKBByteOrder byteOrder, bool writeHeader)
		{
			if (writeHeader)
			{
				// Write byte order
				_writer.Write((byte)byteOrder);

				// Write type
				this.WriteType(ls, byteOrder);
			}

			// Write the number of points in this linestring.
			this.Write((uint)ls.getNumPoints(), byteOrder);

			// Loop on each set of coordinates.
			foreach (Coordinate coord in ls.getCoordinates())
			{
				// Create a new point from the coordinates & write it.
				WritePoint(_factory.createPoint(coord), byteOrder, false);
			}
		}

		private void WritePolygon(Polygon poly, WKBByteOrder byteOrder)
		{
			// Write byte order
			_writer.Write((byte)byteOrder);

			// Write type
			this.WriteType(poly, byteOrder);

			// Get the number of rings in this polygon.
			int numRings = poly.getNumInteriorRing() + 1;

			// Write the number of rings to the stream (add one for the shell)
			this.Write((uint)numRings, byteOrder);

			// Get the shell of this polygon.
			this.WriteLineString(poly.getExteriorRing(), byteOrder, false);

			// Loop on the number of rings - 1 because we already wrote the shell.
			for (int i = 0; i < numRings-1; i++)
			{
				this.WriteLineString(poly.getInteriorRingN(i), byteOrder, false);
			}
		}

		private void WriteMultiPoint(MultiPoint mp, WKBByteOrder byteOrder)
		{
			// Write byte order
			_writer.Write((byte)byteOrder);

			// Write type
			this.WriteType(mp, byteOrder);

			// Get the number of points in this multipoint.
			int numPoints = mp.getNumPoints();

			// Write the number of points.
			this.Write((uint)numPoints, byteOrder);

			// Loop on the number of points.
			for (int i = 0; i < numPoints; i++)
			{
				// Write each point.
				this.WritePoint((Point)mp.getGeometryN(i), byteOrder, true);
			}
		}

		private void WriteMultiLineString(MultiLineString mls, WKBByteOrder byteOrder)
		{
			// Write byte order
			_writer.Write((byte)byteOrder);

			// Write type
			this.WriteType(mls, byteOrder);

			//Write the number of linestrings.
			this.Write((uint)mls.getNumGeometries(), byteOrder);

			//Loop on the number of linestrings.
			for (int i = 0; i < mls.getNumGeometries(); i++)
			{
				// Write each linestring.
				this.WriteLineString((LineString)mls.getGeometryN(i), byteOrder, true);
			}
		}

		private void WriteMultiPolygon(MultiPolygon mp, WKBByteOrder byteOrder)
		{
			// Write byte order
			_writer.Write((byte)byteOrder);

			// Write type
			this.WriteType(mp, byteOrder);

			// Get the number of polygons in this multipolygon.
			int numpolygons = mp.getNumGeometries();

			// Write the number of polygons.
			this.Write((uint)numpolygons, byteOrder);

			for (int i = 0; i < numpolygons; i++)
			{
				// Write each polygon.
				this.WritePolygon((Polygon)mp.getGeometryN(i), byteOrder);
			}
		}

		private void WriteGeometryCollection(GeometryCollection gc, WKBByteOrder byteOrder)
		{
			// Write byte order
			_writer.Write((byte)byteOrder);

			// Write type
			this.WriteType(gc, byteOrder);

			// Write the number of geometries.
			this.Write((uint)gc.getNumGeometries(), byteOrder);

			// Loop on the number of geometries.
			for (int i = 0; i < gc.getNumGeometries(); i++)
			{
				// Write each geometry.
				this.WriteGeometry(gc.getGeometryN(i), byteOrder);
			}
		}

		private void Write(int value, WKBByteOrder byteOrder)
		{
			if (byteOrder == WKBByteOrder.Xdr)
			{
				byte[] bytes = BitConverter.GetBytes(value); 
				Array.Reverse(bytes);

				_writer.Write(BitConverter.ToInt32(bytes, 0));
			}
			else
			{
				_writer.Write(value);
			}
		}

		private void Write(uint value, WKBByteOrder byteOrder)
		{
			if (byteOrder == WKBByteOrder.Xdr)
			{
				byte[] bytes = BitConverter.GetBytes(value); 
				Array.Reverse(bytes);

				_writer.Write(BitConverter.ToUInt32(bytes, 0));
			}
			else
			{
				_writer.Write(value);
			}
		}

		private void Write(double value, WKBByteOrder byteOrder)
		{
			if (byteOrder == WKBByteOrder.Xdr)
			{
				byte[] bytes = BitConverter.GetBytes(value); 
				Array.Reverse(bytes);

				_writer.Write(BitConverter.ToDouble(bytes, 0));
			}
			else
			{
				_writer.Write(value);
			}
		}
	}
}
