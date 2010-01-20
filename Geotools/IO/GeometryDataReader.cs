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

#region Using
using System;
using System.Collections;
using System.Data;
using Geotools.IO;
using Geotools.Geometries;

#endregion

namespace Geotools.IO
{
	/// <summary>
	/// Summary description for IGeometryDataReader.
	/// </summary>
	public class GeometryDataReader : IGetGeometry, IEnumerable, IDataReader, IDisposable, IDataRecord
	{

		IDataReader _reader;
		GeometryWkbReader _wkbReader;
		GeometryFactory _geometryFactory;
		Geometry _geometry;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the IGeometryDataReader class.
		/// </summary>
		public GeometryDataReader(GeometryFactory geometryFactory, IDataReader reader)
		{
			_reader = reader;
			_geometryFactory = geometryFactory;
			_wkbReader = new GeometryWkbReader( geometryFactory );
		}
		#endregion

		#region Implementation of IDataReader
		public bool NextResult()
		{
			return _reader.NextResult();
		}

		public void Close()
		{
			_reader.Close();
		}

		public bool Read()
		{
			_geometry = null;
			bool result=  _reader.Read();
			if (result)
			{
				byte[] wkb = (byte[])_reader["wkbgeometry"];
				_geometry = _wkbReader.Create(wkb);
			}
			return result;
		}

		public System.Data.DataTable GetSchemaTable()
		{
			return _reader.GetSchemaTable();
		}

		public int RecordsAffected
		{
			get
			{
				return _reader.RecordsAffected;
			}
		}

		public bool IsClosed
		{
			get
			{
				return _reader.IsClosed;
			}
		}

		public int Depth
		{
			get
			{
				return _reader.Depth;
			}
		}
		#endregion

		#region Implementation of IDisposable
		public void Dispose()
		{	
			_reader.Dispose();
		}
		#endregion

		#region IDataRecord
		public object this[string name]
		{
			get
			{
				return _reader[name];
			}
		}

		public object this[int i]
		{
			get
			{
				return _reader[i];
			}
		}

		public int FieldCount
		{
			get
			{
				return _reader.FieldCount;
			}
		}
	

		public int GetInt32(int i)
		{
			return _reader.GetInt32(i);
		}

		public object GetValue(int i)
		{
			return _reader.GetValue(i);
		}

		public bool IsDBNull(int i)
		{
			return _reader.IsDBNull(i);
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return _reader.GetBytes(i, fieldOffset, buffer,  bufferoffset, length);
		}

		public byte GetByte(int i)
		{
			return _reader.GetByte(i);
		}

		public System.Type GetFieldType(int i)
		{
			return _reader.GetFieldType(i);
		}

		public decimal GetDecimal(int i)
		{
			return _reader.GetDecimal(i);
		}

		public int GetValues(object[] values)
		{
			return _reader.GetValues(values);
		}

		public string GetName(int i)
		{
			return _reader.GetName(i);
		}

		public long GetInt64(int i)
		{
			return _reader.GetInt64(i);
		}

		public double GetDouble(int i)
		{
			return _reader.GetDouble(i);
		}

		public bool GetBoolean(int i)
		{
			return _reader.GetBoolean(i);
		}

		public System.Guid GetGuid(int i)
		{
			return _reader.GetGuid(i);
		}

		public System.DateTime GetDateTime(int i)
		{
			return _reader.GetDateTime(i);
		}

		public int GetOrdinal(string name)
		{
			return _reader.GetOrdinal(name);
		}

		public string GetDataTypeName(int i)
		{
			return _reader.GetDataTypeName(i);
		}

		public float GetFloat(int i)
		{
			return _reader.GetFloat(i);
		}

		public System.Data.IDataReader GetData(int i)
		{
			return _reader.GetData(i);
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return _reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		public string GetString(int i)
		{
			return _reader.GetString(i);
		}

		public char GetChar(int i)
		{
			return (char) _reader.GetChar(i);
		}

		public short GetInt16(int i)
		{
			return _reader.GetInt16(i);
		}
		#endregion
		
		#region Properties
		public Geometry Geometry
		{
			get
			{
				return _geometry;
			}
		}
		#endregion

		#region Methods
		public Geometry GetGeometry()
		{
			byte[] wkb = (byte[])_reader["wkbgeometry"];
			return _wkbReader.Create(wkb);
		}
		#endregion

		#region Implementation of IEnumerable
		public System.Collections.IEnumerator GetEnumerator()
		{
			IEnumerable enumerable = (IEnumerable)_reader;
			return enumerable.GetEnumerator();
		}
		#endregion

	}
}
