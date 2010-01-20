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
	/// <summary>
	/// Abstract class that defines the interfaces that other 'Shape' handlers must implement.
	/// </summary>
	public abstract class ShapeHandler 
	{
		public ShapeHandler()
		{
		}
		
		/// <summary>
		/// Returns the <see cref="ShapeType">ShapeType</see> the handler handles.
		/// </summary>
		public abstract ShapeType ShapeType{get;}

		/// <summary>
		/// Reads a stream and converts the shapefile record to an equilivent <b>Geometry</b> object.
		/// </summary>
		/// <param name="reader">The stream reader.</param>
		/// <param name="geometryFactory">The <b>GeometryFactory</b> to use when making the object.</param>
		/// <returns>The <b>Geometry</b> object that represents the shape file record.</returns>
		public abstract Geometry Read(BigEndianBinaryReader reader, GeometryFactory geometryFactory);
		
		/// <summary>
		/// Writes to the given stream the equilivent shape file record given a <b>Geometry</b> object.
		/// </summary>
		/// <param name="geometry">The <b>Geometry</b> object to write.</param>
		/// <param name="writer">The stream writeer.</param>
		/// <param name="geometryFactory">The <b>GeometryFactory</b> to use.</param>
		public abstract void Write(Geometry geometry, BinaryWriter writer, GeometryFactory geometryFactory);

		/// <summary>
		/// Gets the length in bytes the Geometry will need when written as a shape file record.
		/// </summary>
		/// <param name="geometry">The <b>Geometry</b> object to use.</param>
		/// <returns>The length in bytes the geometry will use when represented as a shape file record.</returns>
		public abstract int GetLength(Geometry geometry); //length in 16bit words

		/// <summary>
		/// Returns an <b>Envelope</b> in external coordinates.
		/// </summary>
		/// <param name="precisionModel">The <b>PrecisionModel</b> used to create the <b>Envelope</b>.</param>
		/// <param name="envelope">The <b>Envelope</b> to transform.</param>
		/// <returns>An <b>Envelope</b> in external coordinates</returns>
		public static Envelope GetEnvelopeExternal(PrecisionModel precisionModel, Envelope envelope)
		{
			return new Envelope(envelope.getMinX(), envelope.getMaxX(), envelope.getMinY(), envelope.getMaxY());
		}

		/// <summary>
		/// Returns the <see cref="ShapeType">ShapeType</see> for the current record.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns>The <see cref="ShapeType">ShapeType</see> for the current record.</returns>
		/// <exception cref="ShapefileException">The shape type is invalid.</exception>
		protected ShapeType GetShapeType(BigEndianBinaryReader reader)
		{
			int type = reader.ReadInt32();

			if (!Enum.IsDefined(typeof(ShapeType), type))
			{
				throw new ShapefileException("Invalid shape type.");
			}
			
			return (ShapeType)type;
		}

		protected void WriteBoundingBox(Envelope boundingBox, BinaryWriter writer)
		{
			writer.Write(boundingBox.getMinX());
			writer.Write(boundingBox.getMinY());
			writer.Write(boundingBox.getMaxX());
			writer.Write(boundingBox.getMaxY());
		}
	}
}
