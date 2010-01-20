namespace Geotools.CoordinateTransformations
{
	/// <summary>
	/// Provides public constants for well-known projection types.
	/// </summary>
	public class Projections
	{

		private Projections()
		{

		}
		
		/// <summary>
		/// Albers
		/// </summary>
		public const string Albers = "PROJCS[\"Albers\",GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"6277\"]]TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6277\"]]PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]]AXIS[\"Geodetic latitude\",\"NORTH\"],AXIS[\"Geodetic longitude\",\"EAST\"],AUTHORITY[\"EPSG\",\"4277\"]],PROJECTION[\"albers\"],  PARAMETER[\"false_easting\", 0.000000],PARAMETER[\"false_northing\", 0.000000],PARAMETER[\"central_meridian\",-96.000000],PARAMETER[\"standard_parallel_1\", 29.500000],PARAMETER[\"standard_parallel_2\", 45.500000],PARAMETER[\"latitude_of_origin\", 37.500000],	AXIS[\"Easting\",\"EAST\"],AXIS[\"Northing\",\"NORTH\"],AUTHORITY[\"EPSG\",\"9804\"]]";

		/// <summary>
		/// European Lambert for raster
		/// </summary>
		public const string EuropeanLambert = "PROJCS[\"Europe_Lambert\",GEOGCS[\"GCS_European_1950\",DATUM[\"D_Clarke_1880\",SPHEROID[\"Clarke_1880\",6378249.138,293.466307656,AUTHORITY[\"EPSG\",\"6277\"]]TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6277\"]]PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]		]		AXIS[\"Geodetic latitude\",\"NORTH\"],		AXIS[\"Geodetic longitude\",\"EAST\"],AUTHORITY[\"EPSG\",\"4277\"]],PROJECTION[\"lambert\"],PARAMETER[\"easting_at_false_origin\",2679984.29],PARAMETER[\"northing_at_false_origin\",-484330],PARAMETER[\"longitude_of_false_origin\",17],PARAMETER[\"latitude_of_1st_standard_parallel\",42],PARAMETER[\"latitude_of_2nd_standard_parallel\",56],PARAMETER[\"latitude_of_false_origin\",29.77931],AXIS[\"Easting\",\"EAST\"],		AXIS[\"Northing\",\"NORTH\"],AUTHORITY[\"EPSG\",\"9804\"]]";

		/// <summary>
		/// Mercator
		/// </summary>
		public const string Mercator = "PROJCS[\"OSGB 1936 / British National Grid\",GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"6277\"]]TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6277\"]]PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]]AXIS[\"Geodetic latitude\",\"NORTH\"],AXIS[\"Geodetic longitude\",\"EAST\"],AUTHORITY[\"EPSG\",\"4277\"]],PROJECTION[\"mercator\"],PARAMETER[\"latitude_of_natural_origin\",{0}],PARAMETER[\"longitude_of_natural_origin\",0],PARAMETER[\"scale_factor_at_natural_origin\",1],PARAMETER[\"false_easting\",0],PARAMETER[\"false_northing\",{1}],AXIS[\"Easting\",\"EAST\"],AXIS[\"Northing\",\"NORTH\"],AUTHORITY[\"EPSG\",\"9804\"]]";

		/// <summary>
		/// WGS 84 / UTM ZONE 30N
		/// </summary>
		public const string UTM30N = "PROJCS[\"WGS 84 / UTM zone 30N\",GEOGCS[\"WGS 84\",DATUM[\"World Geodetic System 1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]]TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6326\"]]		PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]]		AXIS[\"Geodetic latitude\",\"NORTH\"],AXIS[\"Geodetic longitude\",\"EAST\"],		AUTHORITY[\"EPSG\",\"4326\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_natural_origin\",0],		PARAMETER[\"longitude_of_natural_origin\",-3],		PARAMETER[\"scale_factor_at_natural_origin\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],AXIS[\"Easting\",\"EAST\"],AXIS[\"Northing\",\"NORTH\"],AUTHORITY[\"EPSG\",\"32630\"]]";

		/// <summary>
		/// World Geodetic System 1984
		/// </summary>
		public const string WGS84 = "GEOGCS[\"WGS 84\",DATUM[\"World Geodetic System 1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]]TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6326\"]]PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]]AXIS[\"Geodetic latitude\",\"NORTH\"],AXIS[\"Geodetic longitude\",\"EAST\"],AUTHORITY[\"EPSG\",\"4326\"]]";

		/// <summary>
		/// OSGB 1936 / British National Grid Mercator
		/// </summary>
		public const string UKNationalGrid = "PROJCS[\"OSGB 1936 / British National Grid\",GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"6277\"]]TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6277\"]]PRIMEM[\"Greenwich\",0,		AUTHORITY[\"EPSG\",\"8901\"]		]AXIS[\"Geodetic latitude\",\"NORTH\"],AXIS[\"Geodetic longitude\",\"EAST\"],AUTHORITY[\"EPSG\",\"4277\"]],		PROJECTION[\"mercator\"],PARAMETER[\"latitude_of_natural_origin\",{0}],		PARAMETER[\"longitude_of_natural_origin\",-2],PARAMETER[\"scale_factor_at_natural_origin\",0.999601272],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",{1}],AXIS[\"Easting\",\"EAST\"],AXIS[\"Northing\",\"NORTH\"],AUTHORITY[\"EPSG\",\"9804\"]		]";

	}
}
