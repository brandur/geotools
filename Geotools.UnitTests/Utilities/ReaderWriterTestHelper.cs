#region SourceSafe Comments
/* 
 * $Header: /cvsroot/geotoolsnet/GeotoolsNet/Geotools.UnitTests/Utilities/ReaderWriterTestHelper.cs,v 1.5 2005/10/20 16:46:36 awcoats Exp $
 * $Log: ReaderWriterTestHelper.cs,v $
 * Revision 1.5  2005/10/20 16:46:36  awcoats
 * *** empty log message ***
 *
 * 
 * 5     5/25/04 2:50p Jzdecourcy
 * 
 * 4     11/04/02 3:20p Rabergman
 * Changed using namespaces
 * 
 * 3     10/31/02 11:01a Awcoats
 * changed namespace from UrbanScience.Geographic to Geotools.
 * 
 * 2     10/21/02 2:23p Rabergman
 * Changed call to CreateFromWkt to match new signature.
 * 
 * 1     10/11/02 5:04p Rabergman
 * 
 */ 
#endregion

#region Using
using System;
using System.IO;
//using Geotools.Geometries;
using Geotools.IO;
using com.vividsolutions.jts.geom;
#endregion

namespace Geotools.UnitTests.Utilities
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class ReaderWriterTestHelper
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the Class1 class.
		/// </summary>
		public ReaderWriterTestHelper() 
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		public static bool TestHelper(string wkt)
		{
			PrecisionModel pm = new PrecisionModel(1, 0, 0);
			GeometryFactory fact = new GeometryFactory(pm, 0);

			//read wkt
			Geometry a = new GeometryWKTReader(fact).Create(wkt);

			//write wkb
			FileStream fs = new FileStream("TestFile.wkb", FileMode.Create);
			BinaryWriter bw = new BinaryWriter(fs);
			GeometryWKBWriter bWriter = new GeometryWKBWriter(bw, fact);
			bWriter.Write(a, WKBByteOrder.Ndr);
			bw.Close();
			fs.Close();

			//read wkb
			fs = new FileStream("TestFile.wkb", FileMode.Open);
			byte[] bytes = new byte[fs.Length];
			for(int i = 0; i < fs.Length; i++)
			{
				bytes[i] = (byte)fs.ReadByte();
			}
			GeometryWKBReader bReader = new GeometryWKBReader(fact);
			Geometry geom = bReader.Create(bytes);
			fs.Close();

			//write to wkt & compare with original text.
			bool results = ( Compare.WktStrings(wkt,a.toText()));
			return results;
		}
		#endregion

	}
}