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

namespace Geotools.IO
{
	public class PointHandler : ShapeHandler
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PointHandler">PointHandler</see> class.
		/// </summary>
		public PointHandler() 
			: base()
		{
		}

		/// <summary>
		/// Gets the <see cref="ShapeType">ShapeType</see> this handler handles.
		/// </summary>
		public override ShapeType ShapeType
		{
			get
			{
				return ShapeType.Point;
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
			if (this.GetShapeType(reader) != ShapeType.Point)
			{
				throw new ShapefileException("Attempting to load a non-point as point.");
			}

			Coordinate coord = new Coordinate(reader.ReadDouble(), reader.ReadDouble());
			factory.getPrecisionModel().makePrecise(coord);

			return factory.createPoint(coord);
		}

		
		/// <summary>
		/// Writes to the given stream the equilivent shape file record given a Geometry object.
		/// </summary>
		/// <param name="geometry">The geometry object to write.</param>
		/// <param name="writer">The stream to write to.</param>
		/// <param name="factory">The geometry factory to use.</param>
		public override void Write(Geometry geometry, BinaryWriter writer, GeometryFactory factory)
		{
			writer.Write((int)this.ShapeType);

			writer.Write(geometry.getCoordinates()[0].x);
			writer.Write(geometry.getCoordinates()[0].y);
		}

		/// <summary>
		/// Returns the length in bytes the <b>Geometry</b> will need when written as a shape file record.
		/// </summary>
		/// <param name="geometry">The <b>Geometry</b> object to use.</param>
		/// <returns>The length in bytes the <b>Geometry</b> will use when represented as a shape file record.</returns>
		public override int GetLength(Geometry geometry)
		{
			return 10; //the length of two doubles in 16bit words + the shapeType
		}
	}
}
