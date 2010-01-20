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
using System.Diagnostics;
using com.vividsolutions.jts.geom;
using com.vividsolutions.jts.geom.impl;

namespace Geotools.IO
{
	/// <summary>
	///  Converts Well-known Binary representations to <b>Geometry</b> objects.
	/// </summary>
	/// <remarks>
	/// The Well-known Binary format is defined in the OpenGIS Simple Features Specification for SQL.
	/// </remarks> 
	public class GeometryWKBReader
	{
		private GeometryFactory _factory;

		/// <summary>
		/// Initializes a new instance of the <see cref="GeometryWKBReader">GeometryWKBReader</see> class.
		/// </summary>
		/// <param name="factory">The <b>GeometryFactory</b> used to create <b>Geometry</b> objects.</param>
		public GeometryWKBReader(GeometryFactory factory) 
		{
			_factory = factory;
		}

		/// <summary>
		/// Creates a <b>Geometry</b> from the supplied byte[] containing the Well-known Binary representation.
		/// </summary>
		/// <param name="bytes">byte[] containing the Well-known Binary representation.</param>
		/// <returns>A <b>Geometry</b> bases on the supplied Well-known Binary representation.</returns>
		public Geometry Create(byte[] bytes)
		{
			// Create a memory stream using the suppiled byte array.
			using (MemoryStream ms = new MemoryStream(bytes))
			{
				// Create a new binary reader using the newly created memorystream.
				using (BinaryReader reader = new BinaryReader(ms))
				{
					// Call the main create function.
					return Create(reader);
				}
			}
		}

		/// <summary>
		/// Creates a <b>Geometry</b> based on the Well-known binary representation.
		/// </summary>
		/// <param name="reader">A <see cref="System.IO.BinaryReader">BinaryReader</see> used to read the Well-known binary representation.</param>
		/// <returns>A <b>Geometry</b> based on the Well-known binary representation.</returns>
		public Geometry Create(BinaryReader reader)
		{
			// Get the first byte in the array.  This specifies if the WKB is in
			// XDR (big-endian) format of NDR (little-endian) format.
			byte byteOrder = reader.ReadByte();

			if (!Enum.IsDefined(typeof(WKBByteOrder), byteOrder))
			{
				throw new ArgumentException("Byte order not recognized");
			}

			// Get the type of this geometry.
			int type = (int)this.ReadUInt32(reader, (WKBByteOrder)byteOrder);
			
			if (!Enum.IsDefined(typeof(WKBGeometryType), type))
			{
				throw new ArgumentException("Geometry type not recognized");
			}

			switch((WKBGeometryType)type)
			{
				case WKBGeometryType.WKBPoint:
					return this.CreateWKBPoint(reader, (WKBByteOrder)byteOrder);
					
				case WKBGeometryType.WKBLineString:
					return this.CreateWKBLineString(reader, (WKBByteOrder)byteOrder);
					
				case WKBGeometryType.WKBPolygon:
					return this.CreateWKBPolygon(reader, (WKBByteOrder)byteOrder);
					
				case WKBGeometryType.WKBMultiPoint:
					return this.CreateWKBMultiPoint(reader, (WKBByteOrder)byteOrder);
					
				case WKBGeometryType.WKBMultiLineString:
					return this.CreateWKBMultiLineString(reader, (WKBByteOrder)byteOrder);
					
				case WKBGeometryType.WKBMultiPolygon:
					return this.CreateWKBMultiPolygon(reader, (WKBByteOrder)byteOrder);
					
				case WKBGeometryType.WKBGeometryCollection:
					return this.CreateWKBGeometryCollection(reader, (WKBByteOrder)byteOrder);
					
				default:
					throw new NotSupportedException("Geometry type not supported");
			}
		}

		private Point CreateWKBPoint(BinaryReader reader, WKBByteOrder byteOrder)
		{
			// Create and return the point.
			return _factory.createPoint(new Coordinate(this.ReadDouble(reader, byteOrder), this.ReadDouble(reader, byteOrder)));
		}

		private Coordinate[] ReadCoordinates(BinaryReader reader, WKBByteOrder byteOrder)
		{
			// Get the number of points in this linestring.
			int numPoints = (int)this.ReadUInt32(reader, byteOrder);

			// Create a new array of coordinates.
			Coordinate[] coords = new Coordinate[numPoints];

			// Loop on the number of points in the ring.
			for (int i = 0; i < numPoints; i++)
			{
				// Add the coordinate.
				coords[i] = new Coordinate(this.ReadDouble(reader, byteOrder), this.ReadDouble(reader, byteOrder));
			}

			return coords;
		}

		private LineString CreateWKBLineString(BinaryReader reader, WKBByteOrder byteOrder)
		{
			
			return _factory.createLineString(new PackedCoordinateSequence.Float(this.ReadCoordinates(reader, byteOrder), 2));
		}

		private LinearRing CreateWKBLinearRing(BinaryReader reader, WKBByteOrder byteOrder)
		{
			return _factory.createLinearRing(new PackedCoordinateSequence.Float(this.ReadCoordinates(reader, byteOrder), 2));
		}

		private Polygon CreateWKBPolygon(BinaryReader reader, WKBByteOrder byteOrder)
		{
			// Get the Number of rings in this Polygon.
			int numRings = (int)this.ReadUInt32(reader, byteOrder);

			Debug.Assert(numRings >= 1, "Number of rings in polygon must be 1 or more.");

			LinearRing shell = this.CreateWKBLinearRing(reader, byteOrder);
			
			// Create a new array of linearrings for the interior rings.
			LinearRing[] interiorRings = new LinearRing[numRings-1];

			for (int i = 0; i < (numRings - 1); i++)
			{
				interiorRings[i] = this.CreateWKBLinearRing(reader, byteOrder);
			}
				
			// Create and return the Poylgon.
			return _factory.createPolygon(shell, interiorRings);
		}

		private MultiPoint CreateWKBMultiPoint(BinaryReader reader, WKBByteOrder byteOrder)
		{
			// Get the number of points in this multipoint.
			int numPoints = (int)this.ReadUInt32(reader, byteOrder);

			// Create a new array for the points.
			Point[] points = new Point[numPoints];

			// Loop on the number of points.
			for (int i = 0; i < numPoints; i++)
			{
				// Read point header
				reader.ReadByte();
				this.ReadUInt32(reader, byteOrder);

				// TODO: Validate type

				// Create the next point and add it to the point array.
				points[i] = this.CreateWKBPoint(reader, byteOrder);
			}

			// Create and return the multipoint.
			return _factory.createMultiPoint(points);
		}

		private MultiLineString CreateWKBMultiLineString(BinaryReader reader, WKBByteOrder byteOrder)
		{
			// Get the number of linestrings in this multilinestring.
			int numLineStrings = (int)this.ReadUInt32(reader, byteOrder);

			// Create a new array for the linestrings .
			LineString[] lineStrings = new LineString[numLineStrings];
            
			// Loop on the number of linestrings.
			for (int i = 0; i < numLineStrings; i++)
			{
				// Read linestring header
				reader.ReadByte();
				this.ReadUInt32(reader, byteOrder);

				// TODO: Validate type

				// Create the next linestring and add it to the array.
				lineStrings[i] = this.CreateWKBLineString(reader, byteOrder);
			}

			// Create and return the MultiLineString.
			return _factory.createMultiLineString(lineStrings);
		}

		private MultiPolygon CreateWKBMultiPolygon(BinaryReader reader, WKBByteOrder byteOrder)
		{
			// Get the number of Polygons.
			int numPolygons = (int)this.ReadUInt32(reader, byteOrder);

			// Create a new array for the Polygons.
			Polygon[] polygons = new Polygon[numPolygons];

			// Loop on the number of polygons.
			for (int i = 0; i < numPolygons; i++)
			{
				// read polygon header
				reader.ReadByte();
				this.ReadUInt32(reader, byteOrder);
				
				// TODO: Validate type

				// Create the next polygon and add it to the array.
				polygons[i] = this.CreateWKBPolygon(reader, byteOrder);
			}

			//Create and return the MultiPolygon.
			return _factory.createMultiPolygon(polygons);
		}

		private Geometry CreateWKBGeometryCollection(BinaryReader reader, WKBByteOrder byteOrder)
		{
			// The next byte in the array tells the number of geometries in this collection.
			int numGeometries = (int)this.ReadUInt32(reader, byteOrder);

			// Create a new array for the geometries.
			Geometry[] geometries = new Geometry[numGeometries];

			// Loop on the number of geometries.
			for(int i = 0; i < numGeometries; i++)
			{
				// Call the main create function with the next geometry.
				geometries[i] = this.Create(reader);
			}

			// Create and return the next geometry.
			return _factory.createGeometryCollection(geometries);
		}

		private int ReadInt32(BinaryReader reader, WKBByteOrder byteOrder)
		{
			if (byteOrder == WKBByteOrder.Xdr)
			{
				byte[] bytes = BitConverter.GetBytes(reader.ReadInt32()); 
				Array.Reverse(bytes);

				return BitConverter.ToInt32(bytes, 0);
			}
			else
			{
				return reader.ReadInt32();
			}
		}

		private uint ReadUInt32(BinaryReader reader, WKBByteOrder byteOrder)
		{
			if (byteOrder == WKBByteOrder.Xdr)
			{
				byte[] bytes = BitConverter.GetBytes(reader.ReadUInt32()); 
				Array.Reverse(bytes);

				return BitConverter.ToUInt32(bytes, 0);
			}
			else
			{
				return reader.ReadUInt32();
			}
		}	

		private double ReadDouble(BinaryReader reader, WKBByteOrder byteOrder)
		{
			if (byteOrder == WKBByteOrder.Xdr)
			{
				byte[] bytes = BitConverter.GetBytes(reader.ReadDouble()); 
				Array.Reverse(bytes);

				return BitConverter.ToDouble(bytes, 0);
			}
			else
			{
				return reader.ReadDouble();
			}
		}
	}
}
