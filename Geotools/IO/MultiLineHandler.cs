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
using com.vividsolutions.jts.geom;
using com.vividsolutions.jts.geom.impl;

namespace Geotools.IO
{
	/// <summary>
	/// Converts a Shapefile multi-line to a OGIS LineString/ MultiLineString.
	/// </summary>
	public class MultiLineHandler : ShapeHandler
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MultiLineHandler">MultiLineHandler</see> class.
		/// </summary>
		public MultiLineHandler() : base()
		{
		}

		/// <summary>
		/// Gets the <see cref="ShapeType">ShapeType</see> the handler handles.
		/// </summary>
		public override ShapeType ShapeType
		{
			get
			{
				return ShapeType.Arc;
			}
		}

		/// <summary>
		/// Reads a stream and converts the shapefile record to an equilivent geometry object.
		/// </summary>
		/// <param name="reader">The stream to read.</param>
		/// <param name="factory">The geometry factory to use when making the object.</param>
		/// <returns>The Geometry object that represents the shape file record.</returns>
		public override Geometry Read(BigEndianBinaryReader reader, GeometryFactory factory)
		{
			int shapeTypeNum = reader.ReadInt32();
			ShapeType shapeType = (ShapeType)Enum.Parse(typeof(ShapeType),shapeTypeNum.ToString());


			if (shapeType != ShapeType.Arc)
			{
				throw new ShapefileException("Attempting to load a non-arc as arc.");
			}
			//read and for now ignore bounds.
			double[] box = new double[4];
			for (int i = 0; i < 4; i++) 
			{
				double d= reader.ReadDouble();
				box[i] =d;
			}


        
			int numParts = reader.ReadInt32();
			int numPoints = reader.ReadInt32();
			int[] partOffsets = new int[numParts];
			for (int i = 0; i < numParts; i++)
			{
				partOffsets[i] = reader.ReadInt32();
			}
			
			LineString[] lines = new LineString[numParts];
			int start, finish, length;
			for (int part = 0; part < numParts; part++)
			{
				start = partOffsets[part];
				if (part == numParts - 1)
				{
					finish = numPoints;
				}
				else 
				{
					finish = partOffsets[part + 1];
				}
				length = finish - start;
				Coordinate[] coords = new Coordinate[length];

				for (int i = 0; i < length; i++)
				{
					Coordinate coord = new Coordinate(reader.ReadDouble(), reader.ReadDouble());
					factory.getPrecisionModel().makePrecise(coord);

					coords[i] = coord;
				}

				lines[part] = factory.createLineString(new PackedCoordinateSequence.Float(coords, 2));
			}
			return factory.createMultiLineString(lines);
		}

		/// <summary>
		/// Writes to the given stream the equilivent shape file record given a Geometry object.
		/// </summary>
		/// <param name="geometry">The geometry object to write.</param>
		/// <param name="writer">The stream to write to.</param>
		/// <param name="factory">The geometry factory to use.</param>
		public override void Write(Geometry geometry, BinaryWriter writer, GeometryFactory factory)
		{
			MultiLineString multi = (MultiLineString) geometry;

			writer.Write((int)this.ShapeType);

			this.WriteBoundingBox(multi.getEnvelopeInternal(), writer);
        
			int numParts = multi.getNumGeometries();
			int numPoints = multi.getNumPoints();
        
			writer.Write(numParts);		
			writer.Write(numPoints);
      
			//LineString[] lines = new LineString[numParts];
        
			// write the offsets
			int offset=0;
			for (int i = 0; i < numParts; i++)
			{
				Geometry g =  multi.getGeometryN(i);
				writer.Write( offset );
				offset = offset + g.getNumPoints();
			}
        
			for (int part = 0; part < numParts; part++)
			{
				Coordinate[] coords = multi.getGeometryN(part).getCoordinates();

				for (int i = 0; i < coords.Length; i++)
				{
					factory.getPrecisionModel().makePrecise(coords[i]);

					writer.Write(coords[i].x);
					writer.Write(coords[i].y);
				}
			}
		}

		/// <summary>
		/// Returns the length in bytes the <b>Geometry</b> will need when written as a shape file record.
		/// </summary>
		/// <param name="geometry">The <b>Geometry</b> object to use.</param>
		/// <returns>The length in bytes the <b>Geometry</b> will use when represented as a shape file record.</returns>
		public override int GetLength(Geometry geometry)
		{
			return (22 + (2 * this.GetNumParts(geometry)) + geometry.getNumPoints() * 8);
		}

		private int GetNumParts(Geometry geometry)
		{
			int numParts=1;
			if (geometry is MultiLineString)
			{
				numParts = ((MultiLineString)geometry).getNumGeometries();
			}
			return numParts;
		}
	}
}