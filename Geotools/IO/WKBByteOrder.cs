using System;

namespace Geotools.IO
{
	/// <summary>
	/// Specifies the byte order.
	/// </summary>
	public enum WKBByteOrder : byte
	{
		/// <summary>
		/// Big-endian.
		/// </summary>
		Xdr = 0,
		/// <summary>
		/// Little-endian.
		/// </summary>
		Ndr = 1
	}
}
