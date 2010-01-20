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
using System.Data;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using com.vividsolutions.jts.geom;

namespace Geotools.IO
{
	public class ShapefileReader : IEnumerable
	{
		#region ShapefileEnumerator Class

		private class ShapefileEnumerator : IEnumerator, IDisposable
		{
			private ShapefileReader _parent;
			private Geometry _geometry;
			private ShapeHandler _handler;
			private BigEndianBinaryReader _shpBinaryReader = null;

			/// <summary>
			/// Initializes a new instance of the <see cref="ShapefileEnumerator">ShapefileEnumerator</see> class.
			/// </summary>
			public ShapefileEnumerator(ShapefileReader shapefile)
			{				
				_parent = shapefile;

				// create a file stream for each enumerator that is given out. This allows the same file
				// to have one or more enumerator. If we used the parents stream only one IEnumerator 
				// could be given out.
				_shpBinaryReader = new BigEndianBinaryReader(new FileStream(_parent._filename, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read));
				
				// skip header - since parent has already read this.
				_shpBinaryReader.ReadBytes(100);
				ShapeType type = _parent._mainHeader.ShapeType;
				_handler = Shapefile.GetShapeHandler(type);

				if (_handler == null) 
				{
					throw new NotSupportedException("Unsuported shape type:" + type);
				}
			}

			public void Dispose()
			{
				if (_shpBinaryReader!=null)
				{
					_shpBinaryReader.Close();
				}
			}

			#region Implementation of IEnumerator

			public void Reset()
			{
				throw new InvalidOperationException();
			}

			public bool MoveNext()
			{
				if  (_shpBinaryReader.PeekChar()!=-1)
				{
					int recordNumber = _shpBinaryReader.ReadIntBE();
					int contentLength = _shpBinaryReader.ReadIntBE();
					if (Shapefile.TraceSwitch.Enabled)
					{
						Trace.WriteLine("Record number :"+recordNumber);
						Trace.WriteLine("contentLength :"+contentLength);
					}
					_geometry  = _handler.Read(_shpBinaryReader, _parent._geometryFactory);
					return true;
				}
				else
				{
					// reached end of file, so close the reader.
					_shpBinaryReader.Close();
					return false;
				}
			}

			public object Current
			{
				get
				{
					return _geometry;
				}
			}

			#endregion
		}

		#endregion

		private ShapefileHeader _mainHeader = null;
		private GeometryFactory _geometryFactory=null;
		private string _filename;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ShapefileReader">ShapefileReader</see> class with the given parameters.
		/// </summary>
		/// <param name="filename">The filename of the shape file to read (with .shp).</param>
		/// <param name="factory">The <b>GeometryFactory</b> to use when creating Geometry objects.</param>
		public ShapefileReader(string filename, GeometryFactory factory)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}

			if (factory == null)
			{
				throw new ArgumentNullException("geometryFactory");
			}

			_filename = filename;
			Trace.WriteLineIf(Shapefile.TraceSwitch.Enabled,"Reading filename:"+filename);
			
			_geometryFactory = factory;

			// read header information. note, we open the file, read the header information and then
			// close the file. This means the file is not opened again until GetEnumerator() is requested.
			// For each call to GetEnumerator() a new BinaryReader is created.
			using (BigEndianBinaryReader shpBinaryReader = new BigEndianBinaryReader(File.OpenRead(filename)))
			{
				_mainHeader = new ShapefileHeader(shpBinaryReader);
			}
		}

		/// <summary>
		/// Gets the shapefile's <see cref="ShapefileHeader">ShapefileHeader</see>.
		/// </summary>
		public ShapefileHeader Header
		{
			get
			{
				return _mainHeader;
			}
		}

		/// <summary>
		/// Reads the shapefile and returns a <b>GeometryCollection</b> representing all the records in the shapefile.
		/// </summary>
		/// <returns>A <b>GeometryCollection</b> representing every record in the shapefile.</returns>
		public GeometryCollection ReadAll()
		{
			java.util.ArrayList list = new java.util.ArrayList();
			ShapeHandler handler = Shapefile.GetShapeHandler(_mainHeader.ShapeType);

			if (handler == null) 
			{
				throw new NotSupportedException("Unsupported shape type:" + _mainHeader.ShapeType);
			}

			foreach (Geometry geometry in this)
			{
				list.add(geometry);
			}
			
			Geometry[] geomArray = GeometryFactory.toGeometryArray(list);

			return _geometryFactory.createGeometryCollection(geomArray);
		}

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns a <see cref="System.Collections.IEnumerator">IEnumerator</see> implemented object that contains all <b>Geometry</b> objects within the shapefile.
		/// </summary>
		/// <returns>A <see cref="ShapefileEnumerator">ShapefileEnumerator</see> that can be used to iterate through the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return new ShapefileEnumerator(this);
		}
		
		#endregion
	}
}
