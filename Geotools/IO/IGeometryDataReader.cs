using System;
using System.Data;
using System.Collections;
using com.vividsolutions.jts.geom;

namespace Geotools.IO
{
	public interface IGeometryDataReader : IDataReader, IDataRecord, IEnumerable
	{
		Geometry GetGeometry();
	}
}
