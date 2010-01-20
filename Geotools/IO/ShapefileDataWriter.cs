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
using System.Collections;
using System.IO;
using com.vividsolutions.jts.geom;

namespace Geotools.IO
{
	/// <summary>
	/// Provides functionality for writing shapefiles including index and data files (.shp, .shx, .dbf).
	/// </summary>
	public class ShapefileDataWriter : IDisposable
	{
		private ShapefileWriter _shpWriter;
		private DbaseFileWriter _dbfWriter;
		private bool _disposed = false;

		private string _filename;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ShapefileDataWriter">ShapefileDataWriter</see> class.
		/// </summary>
		/// <param name="filename">The name of the file to write.</param>
		/// <param name="factory">The <b>GeometryFactory</b> to use.</param>
		/// <param name="fields">An <see cref="DbaseFieldDescriptor">DbaseFieldDescriptor[]</see> containing the data column definitions.</param>
		/// <remarks>
		/// The <see cref="ShapefileDataWriter.Close">Close</see> method must be called in order to update 
		/// the underlying file headers.  If <see cref="ShapefileDataWriter.Close">Close</see> is not called 
		/// the underlying files may be in an invalid state.
		/// </remarks>
		public ShapefileDataWriter(string filename, GeometryFactory factory, DbaseFieldDescriptor[] fields)
		{
			_filename = filename;
			// This may need to have more logic to it to ensure we end up with the proper paths....
			_shpWriter = new ShapefileWriter(_filename, factory);
			_dbfWriter = new DbaseFileWriter(GetDbasefilename(_filename));

			for (int i = 0; i < fields.Length; i++)
			{
				_dbfWriter.AddColumn(fields[i]);
			}
		}

		private static string GetDbasefilename(string filename)
		{
			string dbaseFilename = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".dbf");
			return dbaseFilename;
		}
		
		
		/// <summary>
		/// Creates a <see cref="ShapefileDataWriter"/> that appends to a shapefile instead of creating a new one.
		/// </summary>
		/// <param name="filename">The filename to append to.</param>
		/// <param name="factory">The Geometry factory to use.</param>
		public ShapefileDataWriter(string filename, GeometryFactory factory)
		{		
			// appends
			_filename = filename;
			_shpWriter = new ShapefileWriter(filename, factory,true);
			_dbfWriter = new DbaseFileWriter(GetDbasefilename(_filename),true);

		}

		/// <summary>
		/// Writes a <b>Geometry</b> along with the supplied column data.
		/// </summary>
		/// <param name="geometry">The <b>Geometry</b> object.</param>
		/// <param name="columnValues">An <see cref="System.Collections.ArrayList">ArrayList</see> containing the column values.</param>
		public void Write(Geometry geometry, ArrayList columnValues)
		{
			_shpWriter.WriteGeometry(geometry);
			_dbfWriter.Write(columnValues);
		}

		/// <summary>
		/// Flushes the output buffer, causes data to be written to the underlying .shp, .shx, and .dbf files.
		/// </summary>
		public void Flush()
		{
			_shpWriter.Flush();
			_dbfWriter.Flush();
		}

		/// <summary>
		/// Closes the underlying files and releases any resources associated with the current file stream. 
		/// </summary>
		/// <remarks>
		/// The <b>Close</b> method must be called in order to update 
		/// the file headers.  If <b>Close</b> is not called the files may be in an invalid state.
		/// </remarks>
		public void Close()
		{
			_shpWriter.Close();
			_dbfWriter.Close();
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

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="ShapefileDataWriter">ShapefileDataWriter</see> and optionally releases the managed resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}