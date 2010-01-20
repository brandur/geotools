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

namespace Geotools.IO
{
	/// <summary>
	/// This class writes ESRI Shapefiles (.shp and its corresponding .shx).
	/// </summary>
	/// <remarks>
	/// This class is used to write the .shp and .shx files.  In order to use these files 
	/// there must also be a corresponding .dbf file.
	/// </remarks>
	public class ShapefileWriter : IDisposable
	{
		private const int HEADER_shpLength = 4;

		private BigEndianBinaryWriter _shpWriter;
		private BigEndianBinaryWriter _shxWriter;
		private int _shpLength = 50;
		private int _count = 0;
		private ShapeType _type = ShapeType.Undefined;
		private Envelope _bounds = new Envelope();
		private GeometryFactory _factory;
		private bool _disposed = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="ShapefileWriter">ShapefileWriter</see> class with the specified filename and factory .
		/// </summary>
		/// <param name="filename">The name of the file to write.</param>
		/// <param name="factory">The <b>GeometryFactory</b> to use.</param>
		/// <exception cref="ArgumentNullException">The factory is a null reference (Nothing in Visual Basic).</exception>
		public ShapefileWriter(string filename, GeometryFactory factory): this(filename,factory, false )
		{
			/*if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}

			_shpWriter = new BigEndianBinaryWriter(File.Open(filename, FileMode.CreateNew));
			_shxWriter = new BigEndianBinaryWriter(File.Open(Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".shx"), FileMode.CreateNew));
			_factory = factory;

			// Write dummy headers as place holders
			this.WriteHeader(_shpWriter,0);
			this.WriteHeader(_shxWriter,0);*/
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ShapefileWriter">ShapefileWriter</see> class with the specified filename and factory .
		/// </summary>
		/// <param name="filename">The name of the file to write.</param>
		/// <param name="factory">The <b>GeometryFactory</b> to use.</param>
		/// <param name="append"></param>
		/// <exception cref="ArgumentNullException">The factory is a null reference (Nothing in Visual Basic).</exception>
		public ShapefileWriter(string filename, GeometryFactory factory, bool append)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}

			_factory = factory;
			string shxFilename = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".shx");


			if (append)
			{
				if (File.Exists(filename)==false || File.Exists(shxFilename)==false)
				{
					throw new ArgumentException("Cannot append to a file that does not exist.");
				}
				
				ShapefileHeader shpHeader = null;
				ShapefileHeader shxHeader = null;
				using (Stream stream = File.Open(filename, FileMode.Open))
				{
					using (BigEndianBinaryReader beBinaryReader = new BigEndianBinaryReader(stream))
					{
						shpHeader = new ShapefileHeader(beBinaryReader);
					}
				}
				using (Stream stream = File.Open(shxFilename, FileMode.Open))
				{
					using (BigEndianBinaryReader beBinaryReader = new BigEndianBinaryReader(stream))
					{
						shxHeader = new ShapefileHeader(beBinaryReader);
					}
				}
				this._type = shpHeader.ShapeType;
				this._shpLength = shpHeader.FileLength;
				this._bounds = shpHeader.Bounds;
				this._count = (shxHeader.FileLength-50)/4;
			

				_shpWriter = new BigEndianBinaryWriter(File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
				_shxWriter = new BigEndianBinaryWriter(File.Open(shxFilename, FileMode.Open,FileAccess.ReadWrite, FileShare.ReadWrite));
				
				// Write dummy headers as place holders
				this.WriteHeader(_shpWriter,0);
				this.WriteHeader(_shxWriter,0);
				_shpWriter.BaseStream.Position = _shpWriter.BaseStream.Length;
				_shxWriter.BaseStream.Position = _shxWriter.BaseStream.Length;

			}
			else
			{
				_shpWriter = new BigEndianBinaryWriter(File.Open(filename, FileMode.CreateNew));
				_shxWriter = new BigEndianBinaryWriter(File.Open(shxFilename, FileMode.CreateNew));
				
				// Write dummy headers as place holders
				this.WriteHeader(_shpWriter,0);
				this.WriteHeader(_shxWriter,0);
			}
		}

		
		/// <summary>
		/// Writes a <b>Geometry</b> to the file.
		/// </summary>
		/// <param name="geometry">The <b>Geometry</b> to write to the file.</param>
		/// <exception cref="ArgumentNullException">The geometry is a null reference (Nothing in Visual Basic).</exception>
		public void WriteGeometry(Geometry geometry)
		{
			if (geometry == null)
			{
				throw new ArgumentNullException("geometry");
			}

////			if (typeof(geometry ==  GeometryCollection)
////			{
////				this.WriteGeometryCollection((GeometryCollection)geometry);
////			}

			// Make sure we have the same shape type - if not the file is invalid
			if (_type == ShapeType.Undefined)
			{
				_type = Shapefile.GetShapeType(geometry);
			}
			else if (_type != Shapefile.GetShapeType(geometry))
			{
				throw new ArgumentException(String.Format(System.Globalization.CultureInfo.InvariantCulture, "An invalid shapet type has been encountered - expected '{0}' but got '{1}.", _type, Shapefile.GetShapeType(geometry)), "geometry");
			}

			// Get handler
			ShapeHandler handler = Shapefile.GetShapeHandler(_type);

			// Get the length of the geometry in bytes
			int length = handler.GetLength(geometry);

			// Write record number
			_shpWriter.WriteIntBE(_count + 1);

			// Write record length
			_shpWriter.WriteIntBE(length);

			// Write geometry
			handler.Write(geometry, _shpWriter, _factory);

			// Increase bounds to include geometry if needed
			_bounds.expandToInclude(geometry.getEnvelopeInternal());

			// Every time we write a geometry we need to increment the count
			_count++;

			// Write index information
			_shxWriter.WriteIntBE(_shpLength);
			_shxWriter.WriteIntBE(length);

			// Every time we write a geometry we need to increment the byte length
			_shpLength += HEADER_shpLength + length;
	
		}

		/// <summary>
		/// Writes a <b>GeometryCollection</b> to the file.
		/// </summary>
		/// <remarks>
		/// The <b>GeometryCollection</b> must contain a single geometry type.
		/// </remarks>
		/// <param name="collection">The <b>GeometryCollection</b> to write to the file.</param>
		/// <exception cref="ArgumentNullException">The collection is a null reference (Nothing in Visual Basic).</exception>
		public void WriteGeometryCollection(GeometryCollection collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}

			for (int i = 0; i < collection.getNumGeometries(); i++)
			{
				this.WriteGeometry(collection.getGeometryN(i));
			}
		}

		/// <summary>
		/// Closes the file.
		/// </summary>
		public void Close()
		{
			if (_shpWriter != null && _shxWriter != null)
			{
				// Write headers
				this.WriteHeader(_shpWriter, _shpLength);
				this.WriteHeader(_shxWriter, (_count*4)+50);
			}

			_shpWriter.Close();
			_shxWriter.Close();
		}

		/// <summary>
		/// Clears all buffers for and causes any buffered data to be written to the underlying file.
		/// </summary>
		public void Flush()
		{
			_shpWriter.Flush();
			_shxWriter.Flush();
		}

		public static void Test()
		{
			
		}
		public void WriteHeader(BigEndianBinaryWriter writer, int length)
		{
			ShapefileHeader header = new ShapefileHeader();

			// Go to the start of the stream (if we're not already there) 
			writer.BaseStream.Position = 0;

			// Set current header properties
			header.Bounds = _bounds;
			header.FileLength = length;
			header.ShapeType = _type;
			header.Write(writer);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (!_disposed)
				{
					this.Close();
				}
			}

			_disposed = true;
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
