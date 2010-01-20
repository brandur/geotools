using System;
using System.Drawing;
using System.Globalization;
using Geotools.CoordinateReferenceSystems;
using Geotools.CoordinateTransformations;
using Geotools.IO;
using Geotools.Positioning;
using Envelope = com.vividsolutions.jts.geom.Envelope;

namespace Geotools.CoordinateTransformations
{
	/// <summary>
	/// The purpose of this class is to provide frequently used functionality related to 
	/// projections that is not in the Geotools classes.
	/// </summary>
	public sealed class ProjectionHelper
	{

		/// <summary>
		/// Returns the extents of a rectangle (each point is project individually).
		/// </summary>
		/// <remarks>
		/// <para>The reason for this method is apparent when calculating the extents of the
		/// USA when using the Albers projection. If you project just the top-left
		/// and the bottom-right corners of a rectangle and display the rectangle
		/// – it will not fully enclose the USA. Part of California is cut off.
		/// This because, the rectangle when projected would really resemble a
		/// parallelogram. 
		/// </para>
		/// <para>
		/// If an empty rectangle is passed in, then an empty rectangle is returned.
		/// </para>
		/// </remarks>
		/// <param name="extents">The <b>Envelope</b> specified in longitude latitude coordinates.</param>
		/// <param name="projection">The projection to use.</param>
		/// <returns>The rectangle in projected meters.</returns>
		public static Envelope ProjectedExtents( Envelope extents, ICoordinateTransformation projection )
		{
			if ( projection == null )
			{
				throw new ArgumentNullException("projection");
			}

			if ( extents == null )
			{
				return extents;
			}

			float[] longitudes = new float[4];
			float[] latitudes = new float[4];


			DegreesToMeters((float) extents.getMinX(), (float) extents.getMaxY(), out longitudes[0], out latitudes[0], projection);
			DegreesToMeters((float) extents.getMinX(), (float) extents.getMinY(), out longitudes[1], out latitudes[1], projection);
			DegreesToMeters((float) extents.getMaxX(), (float) extents.getMaxY(), out longitudes[2], out latitudes[2], projection);
			DegreesToMeters((float) extents.getMaxX(), (float) extents.getMinY(), out longitudes[3], out latitudes[3], projection);

			float xMin = longitudes[0];
			float yMin = latitudes[0];
			float xMax = longitudes[0];
			float yMax = latitudes[0];

			for ( int i = 0; i < 4; i++ )
			{
				xMin = Math.Min(xMin, longitudes[i]);
				yMin = Math.Min(yMin, latitudes[i]);
				yMax = Math.Max(xMax, longitudes[i]);
				yMax = Math.Max(yMax, latitudes[i]);
			}

			return new Envelope(xMin, xMax, yMin, yMax);
		}

		public static Envelope UnprojectedExtents( Envelope extents, ICoordinateTransformation projection )
		{
			if ( projection == null )
			{
				throw new ArgumentNullException("projection");
			}

			if ( extents == null )
			{
				return extents;
			}

			float[] longitudes = new float[4];
			float[] latitudes = new float[4];


			MetersToDegrees((float) extents.getMinX(), (float) extents.getMaxY(), out longitudes[0], out latitudes[0], projection);
			MetersToDegrees((float) extents.getMinX(), (float) extents.getMinY(), out longitudes[1], out latitudes[1], projection);
			MetersToDegrees((float) extents.getMaxX(), (float) extents.getMaxY(), out longitudes[2], out latitudes[2], projection);
			MetersToDegrees((float) extents.getMaxX(), (float) extents.getMinY(), out longitudes[3], out latitudes[3], projection);

			float xMin = longitudes[0];
			float yMin = latitudes[0];
			float xMax = longitudes[0];
			float yMax = latitudes[0];

			for ( int i = 0; i < 4; i++ )
			{
				xMin = Math.Min(xMin, longitudes[i]);
				yMin = Math.Min(yMin, latitudes[i]);
				xMax = Math.Max(xMax, longitudes[i]);
				yMax = Math.Max(yMax, latitudes[i]);
			}

			return new Envelope(xMin, xMax, yMin, yMax);
		}


		private static void MetersToDegrees( float x, float y, out float projectedX, out float projectedY, ICoordinateTransformation projection )
		{
			PointF point = new PointF(x, y);
			PointF projectedPoint = Unproject(point, projection);
			projectedX = projectedPoint.X;
			projectedY = projectedPoint.Y;
		}

		private static void DegreesToMeters( float x, float y, out float projectedX, out float projectedY, ICoordinateTransformation projection )
		{
			PointF point = new PointF(x, y);
			PointF projectedPoint = Project(point, projection);
			projectedX = projectedPoint.X;
			projectedY = projectedPoint.Y;
		}


		private static CoordinatePoint CreateCoordinatePoint( PointF point )
		{
			CoordinatePoint pt = new CoordinatePoint();
			pt.Ord = new Double[2];
			pt.Ord[0] = point.X;
			pt.Ord[1] = point.Y;
			return pt;
		}

		/// <summary>
		/// Projects a point using the given coordinate transform.
		/// </summary>
		/// <param name="point">The point to project in degrees.</param>
		/// <param name="projection">The coordinate transform to use.</param>
		/// <returns>A projected point.</returns>
		public static PointF Project( PointF point, ICoordinateTransformation projection )
		{
			if ( projection == null )
			{
				// if no projection, just return the same point.
				return point;
			}
			CoordinatePoint coordinatePoint = CreateCoordinatePoint(point);
			CoordinatePoint projectedCoordinatePoint = projection.MathTransform.Transform(coordinatePoint);
			PointF projectedPoint = new PointF((float) projectedCoordinatePoint.Ord[0], (float) projectedCoordinatePoint.Ord[1]);
			return projectedPoint;
		}

		/// <summary>
		/// Projects a point using the given coordinate transform.
		/// </summary>
		/// <param name="point">The point to unproject in degrees</param>
		/// <param name="projection">The coordinate transform to use.</param>
		/// <returns>A point in degrees.</returns>
		public static PointF Unproject( PointF point, ICoordinateTransformation projection )
		{
			if ( projection == null )
			{
				// if no projection, just return the same point.
				return point;
			}
			CoordinatePoint coordinatePoint = CreateCoordinatePoint(point);
			CoordinatePoint projectedCoordinatePoint = projection.MathTransform.GetInverse().Transform(coordinatePoint);
			PointF projectedPoint = new PointF((float) projectedCoordinatePoint.Ord[0], (float) projectedCoordinatePoint.Ord[1]);
			return projectedPoint;
		}

		public static RectangleF Project( RectangleF rectangle, ICoordinateTransformation projection )
		{
			PointF topLeft = new PointF(rectangle.Left, rectangle.Top);
			PointF bottomRight = new PointF(rectangle.Right, rectangle.Bottom);
			topLeft = Project(topLeft, projection);
			bottomRight = Project(bottomRight, projection);
			return RectangleF.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);

		}


		public static RectangleF Unproject( RectangleF rectangle, ICoordinateTransformation projection )
		{
			PointF topLeft = new PointF(rectangle.Left, rectangle.Top);
			PointF bottomRight = new PointF(rectangle.Right, rectangle.Bottom);
			topLeft = Unproject(topLeft, projection);
			bottomRight = Unproject(bottomRight, projection);
			return RectangleF.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);

		}
		
		/// <summary>
		/// Creates the Albers projection.
		/// </summary>
		/// <returns>ICoordinateTransformation that implements the Albers projection.</returns>
		public static ICoordinateTransformation CreateAlbersProjection()
		{
			string wktSourceCS = Projections.Albers;

			IProjectedCoordinateSystem sourceCS = (IProjectedCoordinateSystem) CoordinateSystemWktReader.Create(wktSourceCS);
			IGeographicCoordinateSystem targetCS = CreateWgs84();

			CoordinateTransformationFactory coordTransFactory = new CoordinateTransformationFactory();
			ICoordinateTransformation projection = coordTransFactory.CreateFromCoordinateSystems(sourceCS, targetCS);

			return projection;
		}

		/// <summary>
		/// Creates the Albers projection.
		/// </summary>
		/// <returns>ICoordinateTransformation that implements the Albers projection.</returns>
		public static ICoordinateTransformation CreateUTM30N()
		{
			string wktSourceCS = Projections.UTM30N;

			IProjectedCoordinateSystem sourceCS = (IProjectedCoordinateSystem) CoordinateSystemWktReader.Create(wktSourceCS);
			IGeographicCoordinateSystem targetCS = CreateWgs84();

			CoordinateTransformationFactory coordTransFactory = new CoordinateTransformationFactory();
			ICoordinateTransformation projection = coordTransFactory.CreateFromCoordinateSystems(sourceCS, targetCS);

			return projection;
		}

		/// <summary>
		/// Creates a Meractor projection object. 
		/// </summary>
		/// <param name="centralMeridian">The central merdian to use.</param>
		/// <param name="falseNorthing">The false northing to use.</param>
		/// <returns>ICoordinateTransformation that implements the Mercator projection.</returns>
		public static ICoordinateTransformation CreateMercatorProjection( double centralMeridian, double falseNorthing )
		{
			string wktSourceCS = Projections.Mercator;
			wktSourceCS = String.Format(CultureInfo.InvariantCulture, wktSourceCS, falseNorthing, centralMeridian);

			IProjectedCoordinateSystem sourceCS = (IProjectedCoordinateSystem) CoordinateSystemWktReader.Create(wktSourceCS);
			IGeographicCoordinateSystem targetCS = CreateWgs84();

			CoordinateTransformationFactory coordTransFactory = new CoordinateTransformationFactory();
			ICoordinateTransformation projection = coordTransFactory.CreateFromCoordinateSystems(sourceCS, targetCS);
			return projection;
		}

		/// <summary>
		/// Creates a Meractor projection object. 
		/// </summary>
		/// <param name="centralMeridian">The central merdian to use.</param>
		/// <param name="falseNorthing">The false northing to use.</param>
		/// <returns>ICoordinateTransformation that implements the Mercator projection.</returns>
		public static ICoordinateTransformation CreateUKNationalGridProjection( double centralMeridian, double falseNorthing )
		{
			string wktSourceCS = Projections.UKNationalGrid;
			wktSourceCS = String.Format(CultureInfo.InvariantCulture, wktSourceCS, falseNorthing, centralMeridian);

			IProjectedCoordinateSystem sourceCS = (IProjectedCoordinateSystem) CoordinateSystemWktReader.Create(wktSourceCS);
			IGeographicCoordinateSystem targetCS = CreateWgs84();

			CoordinateTransformationFactory coordTransFactory = new CoordinateTransformationFactory();
			ICoordinateTransformation projection = coordTransFactory.CreateFromCoordinateSystems(sourceCS, targetCS);
			return projection;
		}

		/// <summary>
		/// Creates the projection required for the European raster data from the AA.
		/// </summary>
		/// <returns>ICoordinateTransformation that implements the Mercator projection.</returns>
		public static ICoordinateTransformation CreateEuropeanLambertProjection()
		{
			string wktSourceCS = Projections.EuropeanLambert;

			IProjectedCoordinateSystem sourceCS = (IProjectedCoordinateSystem) CoordinateSystemWktReader.Create(wktSourceCS);
			IGeographicCoordinateSystem targetCS = CreateWgs84();

			CoordinateTransformationFactory coordTransFactory = new CoordinateTransformationFactory();
			ICoordinateTransformation projection = coordTransFactory.CreateFromCoordinateSystems(sourceCS, targetCS);
			return projection;
		}

		/// <summary>
		/// Creates a coordinate system that conforms to WGS 84.
		/// </summary>
		/// <returns>IGeographicCoordinateSystem that represents the WKG84 coordinate system.</returns>
		public static IGeographicCoordinateSystem CreateWgs84()
		{
			string wktTargetCS = Projections.WGS84;
			IGeographicCoordinateSystem targetCS = (IGeographicCoordinateSystem) CoordinateSystemWktReader.Create(wktTargetCS);
			return targetCS;
		}
		
	}
}