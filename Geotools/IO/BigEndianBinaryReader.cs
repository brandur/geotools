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
using System.Text;
using System.Diagnostics;

namespace Geotools.IO
{
	/// <summary>
	/// Binary reader class to allow reading of integers in the Big Endian format.
	/// </summary>
	/// <remarks>
	/// The <see cref="System.IO.BinaryReader">BinaryReader</see> uses Little Endian format when reading binary streams.
	/// </remarks>
	public class BigEndianBinaryReader : BinaryReader
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BigEndianBinaryReader">BigEndianBinaryReader</see> class based on the supplied stream and using UTF8Encoding.
		/// </summary>
		/// <param name="stream"></param>

		public BigEndianBinaryReader(Stream stream) 
			: base(stream)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BigEndianBinaryReader">BigEndianBinaryReader</see> class based on the supplied stream and a specific character encoding.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="encoding"></param>

		public BigEndianBinaryReader(Stream input, Encoding encoding) 
			: base(input, encoding)
		{
		}

		/// <summary>
		/// Reads a 4-byte signed integer using the big-endian layout from the current stream and advances the current position of the stream by two bytes.
		/// </summary>
		/// <returns></returns>
		public int ReadIntBE()
		{
			uint value = this.ReadUInt32();

			return (int)(((value & 0x000000ff) << 24) +
				((value & 0x0000ff00) << 8) +
				((value & 0x00ff0000) >> 8) +
				((value & 0xff000000) >> 24)); 
		}
	}
}
