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
using System.Diagnostics;
using com.vividsolutions.jts.geom;

namespace Geotools.IO
{
	/// <summary>
	/// Class that represents a shape file header record.
	/// </summary>
	public class ShapefileHeader
	{
		private int _fileCode = Shapefile.ShapefileId;
		private int _fileLength = -1;
		private int _version = 1000;
		private ShapeType _shapeType = ShapeType.Undefined;
		private Envelope _bounds;
	
		/// <summary>
		/// Initializes a new instance of the <see cref="ShapefileHeader">ShapefileHeader</see> class with values read in from the stream.
		/// </summary>
		/// <remarks>Reads the header information from the stream.</remarks>
		/// <param name="reader">BigEndianBinaryReader stream to the shapefile.</param>
		public ShapefileHeader(BigEndianBinaryReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}

			_fileCode = reader.ReadIntBE();
	
			if (_fileCode!=Shapefile.ShapefileId)
			{
				throw new ShapefileException("The first four bytes of this file indicate this is not a shape file.");
			}

			// Skip 5 unsed bytes
			for (int i = 0; i < 5; i++)
			{
				reader.ReadIntBE();
			}

			_fileLength = reader.ReadIntBE();
			_version = reader.ReadInt32();

			Debug.Assert(_version == 1000, "Shapefile version", String.Format(System.Globalization.CultureInfo.InvariantCulture, "Expecting only one version (1000), but got {0}", _version));
			
			int shapeType = reader.ReadInt32();
			
			if (Enum.IsDefined(typeof(ShapeType), shapeType))
			{
				_shapeType = (ShapeType)shapeType;
			}
			else
			{
				throw new ShapefileException("Invalid shape type");
			}

			// Read in and store the bounding box
			double[] coords = new double[4];

			for (int i = 0; i < coords.Length; i++)
			{
				coords[i] = reader.ReadDouble();
			}

			_bounds = new Envelope(coords[0], coords[2], coords[1], coords[3]);
			
			// skip z and m bounding boxes.
			for (int i = 0; i < 4; i++)
			{
				reader.ReadDouble();	
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ShapefileHeader">ShapefileHeader</see> class.
		/// </summary>
		public ShapefileHeader()
		{
		}

		/// <summary>
		/// Gets or sets the bounds of the shape file.
		/// </summary>
		public Envelope Bounds
		{
			get
			{
				return _bounds;
			}
			set
			{
				_bounds = value;
			}
		}

		/// <summary>
		/// Gets or sets the shape file type i.e. polygon, point etc...
		/// </summary>
		public ShapeType ShapeType
		{
			get
			{
				return _shapeType;
			}
			set
			{
				_shapeType = value;
			}
		}

		/// <summary>
		/// Gets or sets the shapefile version.
		/// </summary>
		public int Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		/// <summary>
		/// Gets or sets the length of the shape file in words.
		/// </summary>
		public int FileLength
		{
			get
			{
				return _fileLength;
			}
			set
			{
				_fileLength = value;
			}
		}

		/// <summary>
		/// Writes a shapefile header to the given stream;
		/// </summary>
		/// <param name="writer">The binary writer to use.</param>
		public void Write(BigEndianBinaryWriter writer) 
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}

			if (_fileLength == -1)
			{
				throw new InvalidOperationException("The header properties need to be set before writing the header record.");
			}
			
			writer.WriteIntBE(_fileCode);
			
			for (int i = 0; i < 5; i++)
			{
				writer.WriteIntBE(0);	// Skip unused part of header
			}

			writer.WriteIntBE(_fileLength);
			writer.Write(_version);
			writer.Write((int)_shapeType);

			// Write the bounding box
			writer.Write(_bounds.getMinX());
			writer.Write(_bounds.getMinY());
			writer.Write(_bounds.getMaxX());
			writer.Write(_bounds.getMaxY());
        
			// Skip remaining unused bytes
			for (int i = 0; i < 4; i++)
			{
				writer.Write(0.0);	// Skip unused part of header
			}

			Trace.WriteLineIf(Shapefile.TraceSwitch.Enabled, "Header position: " + writer.BaseStream.Position);
		}
	}
}
