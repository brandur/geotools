using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using com.vividsolutions.jts.geom;
using Geotools.CoordinateTransformations;
using Geotools.IO;
using Point = com.vividsolutions.jts.geom.Point;

namespace Geotools.Utilities
{
	/// <summary>
	/// Provides common helper methods for <b>Geometry</b> objects.
	/// </summary>
	public sealed class GeometryHelper
	{
		private GeometryHelper()
		{
		}

		/// <summary>
		/// Returns the <b>Point</b> at the center of an <b>Envelope</b>.
		/// </summary>
		/// <param name="env">The <b>Envelope</b>.</param>
		/// <param name="factory">The <b>GeometryFactory</b> used to create the <b>Point</b>.</param>
		/// <returns>The <b>Point</b> at the center of an <b>Envelope</b>.</returns>
		public static Point GetEnvelopeCenter( Envelope env, GeometryFactory factory )
		{
			return factory.createPoint(new Coordinate(env.getMaxX() - ( env.getWidth()/2 ), env.getMaxY() - ( env.getHeight()/2 )));
		}

		/// <summary>
		/// Converts a <b>Point</b> to a <see cref="System.Drawing.PointF">PointF</see>.
		/// </summary>
		/// <param name="point">The <b>Point</b> to convert.</param>
		/// <returns>A <see cref="System.Drawing.PointF">PointF</see> with the same values x and y values as <i>point</i>.</returns>
		public static PointF PointToPointF( Point point )
		{
			return new PointF((float) point.getX(), (float) point.getY());
		}


		/// <summary>
		/// Converts a <see cref="System.Drawing.PointF">PointF</see> to a <b>Point</b>.
		/// </summary>
		/// <param name="point">The <see cref="System.Drawing.PointF">Point</see> to convert.</param>
		/// <param name="factory">The <see cref="com.vividsolutions.jts.geom.GeometryFactory">GeometryFactory to be used to create the point.</see></param>
		/// <returns>A <b>Point</b> with the same values x and y values as <i>point</i>.</returns>
		public static Point PointFToPoint( PointF point, GeometryFactory factory )
		{
			Coordinate coord = new Coordinate( point.X,  point.Y);
			return factory.createPoint(coord);
		}

		/// <summary>
		/// Converts an <b>Envelope</b> to a <see cref="System.Drawing.RectangleF">RectangleF</see>.
		/// </summary>
		/// <param name="env">The <b>Envelope</b> to convert.</param>
		/// <returns>An <see cref="System.Drawing.RectangleF">RectangleF</see> matching the extents of <i>env</i>.</returns>
		public static RectangleF EnvelopeToRectangleF( Envelope env )
		{
			return RectangleF.FromLTRB((float) env.getMinX(), (float) env.getMinY(), (float) env.getMaxX(), (float) env.getMaxY());
		}

		/// <summary>
		/// Converts a <see cref="System.Drawing.RectangleF">RectangleF</see> to an <b>Envelope</b>.
		/// </summary>
		/// <param name="rect">The <see cref="System.Drawing.RectangleF">RectangleF</see> to convert.</param>
		/// <returns>An <b>Envelope</b> matching the extents of <i>rect</i>.</returns>
		public static Envelope RectangleFToEnvelope( RectangleF rect )
		{
			return new Envelope(rect.Left, rect.Right, rect.Top, rect.Bottom);
		}

		/// <summary>
		/// Returns an <b>Envelope</b> offset by the supplied values.
		/// </summary>
		/// <param name="env">The <b>Envelope</b>.</param>
		/// <param name="x">The distance to offset the envelope along the x-axis.</param>
		/// <param name="y">The distance to offset the envelope along the y-axis.</param>
		/// <returns>An <b>Envelope</b> offset by the supplied values.</returns>
		public static Envelope Offset( Envelope env, double x, double y )
		{
			return new Envelope(env.getMinX() + x, env.getMaxX() + x, env.getMinY() + y, env.getMaxY() + y);
		}

		/// <summary>
		/// Converts a <b>Geometry</b> object to its representation as a <see cref="System.Drawing.Drawing2D.GraphicsPath">GraphicsPath</see>.
		/// </summary>
		/// <param name="geometry">The <b>Geometry</b> object.</param>
		/// <returns>The <see cref="System.Drawing.Drawing2D.GraphicsPath">GraphicsPath</see> representation of the supplied <b>Geometry</b>.</returns>
		/// <exception cref="ArgumentNullException"><i>geometry</i> is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="InvalidOperationException"><i>geometry</i> cannot be converted to a <see cref="System.Drawing.Drawing2D.GraphicsPath">GraphicsPath</see>.</exception>
		public static GraphicsPath GeometryToGraphicsPath( Geometry geometry )
		{
			if ( geometry == null )
			{
				throw new ArgumentNullException("geometry");
			}

			GraphicsPath path = new GraphicsPath();

			if ( geometry is GeometryCollection )
			{
				GeometryCollection geometries = (GeometryCollection) geometry;

				for ( int i = 0; i < geometries.getNumGeometries(); i++ )
				{
					path.AddPath(GeometryToGraphicsPath(geometries.getGeometryN(i)), false);
				}
			}
			else if ( geometry is Point )
			{
				Point point = (Point) geometry;

				path.AddRectangle(new RectangleF((float) point.getX() - .5f, (float) point.getY() - .5f, 1, 1));
			}
			else if ( geometry is Polygon )
			{
				Polygon polygon = (Polygon) geometry;
				path.AddPath(GeometryToGraphicsPath(polygon.getExteriorRing()), true);

				for (int i = 0; i < polygon.getNumInteriorRing(); i++)
				{
					path.AddPath(GeometryToGraphicsPath(polygon.getInteriorRingN(i)), false);
				}
			}
			else if ( geometry is LinearRing )
			{
				Coordinate[] coords = geometry.getCoordinates();
				PointF[] points = new PointF[coords.Length];

				for ( int i = 0; i < points.Length; i++ )
				{
					PointF point = new PointF((float) coords[i].x, (float) coords[i].y);
					points[i] = point;
				}
				path.AddPolygon(points);
			}
			else if ( geometry is LineString )
			{
				Coordinate[] coords = geometry.getCoordinates();
				PointF[] points = new PointF[coords.Length];

				for ( int i = 0; i < points.Length; i++ )
				{
					PointF point = new PointF((float) coords[i].x, (float) coords[i].y);
					points[i] = point;
				}
				path.AddLines(points);
			}
			else
			{
				throw new InvalidOperationException("Cannot convert this to a GDI Graphics path");
			}

			Debug.Assert(path != null, "Should not return null.");

			return path;
		}

		/// <summary>
		/// Converts a <b>Geometry</b> object to its projected representation as a <see cref="System.Drawing.Drawing2D.GraphicsPath">GraphicsPath</see>.
		/// </summary>
		/// <param name="geometry">The <b>Geometry</b> object.</param>
		/// <param name="projection">The projection to use.</param>
		/// <returns>The <see cref="System.Drawing.Drawing2D.GraphicsPath">GraphicsPath</see> representation of the supplied <b>Geometry</b>.</returns>
		/// <exception cref="ArgumentNullException"><i>geometry</i> is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="InvalidOperationException"><i>geometry</i> cannot be converted to a <see cref="System.Drawing.Drawing2D.GraphicsPath">GraphicsPath</see>.</exception>
		public static GraphicsPath ProjectGeometryToGraphicsPath( Geometry geometry, ICoordinateTransformation projection )
		{
			if ( geometry == null )
			{
				throw new ArgumentNullException("geometry");
			}

			GraphicsPath path = new GraphicsPath();

			if ( geometry is GeometryCollection )
			{
				GeometryCollection geometries = (GeometryCollection) geometry;

				for ( int i = 0; i < geometries.getNumGeometries(); i++ )
				{
					path.AddPath(GeometryHelper.ProjectGeometryToGraphicsPath(geometries.getGeometryN(i), projection), false);
				}
			}
			else if ( geometry is Point )
			{
				Point point = (Point) geometry;
				PointF point2 = new PointF((float) point.getX(), (float) point.getY());

				if ( projection != null )
				{
					point2 = ProjectionHelper.Project(point2, projection);
				}

				float x = point2.X;
				float y = point2.Y;

				//HACK: need to determine this 15,000 number 
				path.AddRectangle(new RectangleF(x, y, 15000, 15000));

			}
			else if ( geometry is Polygon )
			{
				Polygon polygon = (Polygon) geometry;
				path.AddPath(GeometryHelper.ProjectGeometryToGraphicsPath(polygon.getExteriorRing(), projection), true);

				for(int i = 0; i < polygon.getNumInteriorRing(); i++)
				{
					path.AddPath(GeometryHelper.ProjectGeometryToGraphicsPath(polygon.getInteriorRingN(i), projection), false);
				}
			}
			else if ( geometry is LinearRing )
			{
			
				Coordinate[] coords = geometry.getCoordinates();

				PointF[] points = new PointF[coords.Length];
				bool project = projection != null;
				for ( int i = 0; i < points.Length; i++ )
				{
					PointF point = new PointF((float) coords[i].x, (float) coords[i].y);
					if ( project )
					{
						point = ProjectionHelper.Project(point, projection);
					}
					points[i] = point;
				}
				path.AddPolygon(points);
			}
			else if ( geometry is LineString )
			{
			
				Coordinate[] coords = geometry.getCoordinates();

				PointF[] points = new PointF[coords.Length];
				bool project = projection != null;
				for ( int i = 0; i < points.Length; i++ )
				{
					PointF point = new PointF((float) coords[i].x, (float) coords[i].y);
					if ( project )
					{
						point = ProjectionHelper.Project(point, projection);
					}
					points[i] = point;
				}
				path.AddLines(points);
			}
			else
			{
				throw new InvalidOperationException("Cannot convert this to a GDI Graphics path");
			}

			Debug.Assert(path != null, "Should not return null.");

			return path;
		}

		/// <summary>
		/// Returns the largest <b>Polygon</b> in terms of area contained in the <b>MultiPolygon</b>.
		/// </summary>
		/// <param name="multiPolygon">The <b>MultiPolygon</b>.</param>
		/// <returns>The largest <b>Polygon</b> in terms of area contained in the <b>MultiPolygon</b>.</returns>
		/// <exception cref="ArgumentException"><i>multiPolygon</i> does not contain any <b>Polygon</b> objects.</exception>
		public static Polygon GetLargestPolygon( MultiPolygon multiPolygon )
		{
			if ( multiPolygon.getNumGeometries() == 0 )
			{
				throw new ArgumentException("MultiPolygon contains no polygons.", "multiPolygon");
			}

			Polygon largest = (Polygon) multiPolygon.getGeometryN(0);

			for ( int i = 0; i < multiPolygon.getNumGeometries(); i++ )
			{
				if ( multiPolygon.getGeometryN(i).getArea() > largest.getArea() )
				{
					largest = (Polygon) multiPolygon.getGeometryN(i);
				}
			}
			return largest;
		}

		#region Line simplificication

		public static Geometry Simplify( Geometry geometry, double tolerance )
		{
			if ( geometry == null )
			{
				throw new ArgumentNullException("geometry");
			}
			if ( geometry.isEmpty() )
			{
				throw new ArgumentException("Cannot be an empty polygon");
			}
			else if ( geometry is LineString )
			{
				Coordinate[] coords = geometry.getCoordinates();
				return geometry.getFactory().createLineString(SimplifyLinearRing(ref coords, tolerance));
			}
			if ( geometry is Polygon )
			{
				return Simplify((Polygon) geometry, tolerance);
			}
			else if ( geometry is MultiPolygon )
			{
				return Simplify((MultiPolygon) geometry, tolerance);
			}

			throw new NotSupportedException("This geometry type is not supported");
		}

		private static Geometry Simplify( MultiPolygon multipolygon, double tolerance )
		{
			GeometryFactory gf = new GeometryFactory();

			Polygon[] generalizedPolygons = new Polygon[multipolygon.getNumGeometries()];
			for ( int i = 0; i < generalizedPolygons.Length; i++ )
			{
				Polygon polygon = (Polygon) multipolygon.getGeometryN(i);
				generalizedPolygons[i] = Simplify(polygon, tolerance);
			}
			MultiPolygon multiPolygon = gf.createMultiPolygon(generalizedPolygons);
			return multiPolygon;
		}

		private static Coordinate[] SimplifyLinearRing( ref Coordinate[] coords, double tolerance )
		{
			int endPoint = GetIndexOfFutherestPoint(ref coords);

			if ( coords.Length < 5 )
			{
				// no point in trying to simplify this polygon.
				// I chose 10 quite arbitrarily. I guess this could be made to be 8, but who is worried about two points.
				return coords;
			}

			bool[] keep = new bool[coords.Length];
			Simplify(ref coords, 0, endPoint, tolerance, ref keep);
			Simplify(ref coords, endPoint, coords.Length - 1, tolerance, ref keep);


			CoordinateList keepCoords = new CoordinateList();
			keep[0] = true;
			keep[endPoint] = true;
			keep[coords.Length - 1] = true;
			for ( int i = 0; i < keep.Length; i++ )
			{
				if ( keep[i] )
				{
					keepCoords.add(coords[i], true);
				}
			}

			Coordinate[] generalizedCoords = keepCoords.toCoordinateArray();
			if ( generalizedCoords.Length <= 3 )
			{
				return coords;
			}
			return generalizedCoords;
		}

		private static LinearRing Simplify( Coordinate[] coords, double tolerance )
		{
			Coordinate[] generalizedCoords = SimplifyLinearRing(ref coords, tolerance);

			GeometryFactory gf = new GeometryFactory();
			LinearRing generalizedLinearRing = gf.createLinearRing(generalizedCoords);
			return generalizedLinearRing;

		}

		private static Polygon Simplify( Polygon polygon, double tolerance )
		{
			LinearRing generalizedShell = Simplify(polygon.getExteriorRing().getCoordinates(), tolerance);
			GeometryFactory gf = new GeometryFactory();

			LinearRing[] generalizedHoles = new LinearRing[polygon.getNumInteriorRing()];
			for ( int i = 0; i < generalizedHoles.Length; i++ )
			{
				generalizedHoles[i] = Simplify(polygon.getInteriorRingN(i).getCoordinates(), tolerance);
			}
			return gf.createPolygon(generalizedShell, generalizedHoles);


		}


		private static void Simplify( ref Coordinate[] coords, int startIndex, int endIndex, double tolerance, ref bool[] keep )
		{
			// uses the Douglas-Peucker algorithm.


			keep[startIndex] = true;
			keep[endIndex] = true;

			double maxDistance = 0;
			int maxDistanceIndex = -1;
			for ( int i = startIndex + 1; i < endIndex; i++ )
			{
				double distance = PerpendicularDistance(coords[startIndex], coords[endIndex], coords[i]);
				if ( distance > maxDistance )
				{
					maxDistance = distance;
					maxDistanceIndex = i;
				}
			}
			if ( maxDistance >= tolerance )
			{
				if ( maxDistanceIndex >= 0 )
				{
					keep[maxDistanceIndex] = true;
					Simplify(ref coords, startIndex, maxDistanceIndex, tolerance, ref keep);
					Simplify(ref coords, maxDistanceIndex, endIndex, tolerance, ref keep);
				}
			}
		}

		private static int GetIndexOfFutherestPoint( ref Coordinate[] coords )
		{
			Debug.Assert(coords != null);
			Debug.Assert(coords.Length > 0);

			int indexOfFurthestPoint = -1;
			double maximumDistance = 0;

			for ( int i = 1; i < coords.Length; i++ )
			{
				double dist = coords[0].distance(coords[i]);
				if ( dist > maximumDistance )
				{
					maximumDistance = dist;
					indexOfFurthestPoint = i;
				}
			}
			Debug.Assert(indexOfFurthestPoint != -1);
			return indexOfFurthestPoint;
		}

		private static double PerpendicularDistance( Coordinate l1, Coordinate l2, Coordinate p )
		{
			Debug.Assert(l1 != null);
			Debug.Assert(l2 != null);
			Debug.Assert(p != null);

			return PerpendicularDistance(l1.x, l1.y, l2.x, l2.y, p.x, p.y);
		}

		private static double Distance( double x1, double y1, double x2, double y2 )
		{
			double xdiff = x2 - x1;
			double ydiff = y2 - y1;
			return Math.Sqrt(xdiff*xdiff + ydiff*ydiff);
		}

		private static double PerpendicularDistance( double x1, double y1, double x2, double y2, double x3, double y3 )
		{
			double x12 = x2 - x1;
			double y12 = y2 - y1;
			double x13 = x3 - x1;
			double y13 = y3 - y1;
			double D12 = Distance(x1, y1, x2, y2);

			return Math.Abs(( ( x12*y13 ) - ( x13*y12 ) )/D12);
		}

		private static double PerpendicularDistance2( double x1, double y1, double x2, double y2, double x3, double y3 )
		{
			// algorithm courtesy of Ray 1/16/98
			double x12 = x2 - x1;
			double y12 = y2 - y1;
			double x13 = x3 - x1;
			double y13 = y3 - y1;
			double D12 = Math.Sqrt(x12*x12 + y12*y12);
			double pp = ( x12*x13 + y12*y13 )/D12;
			if ( pp < 0.0 )
			{
				return Math.Sqrt(x13*x13 + y13*y13);
			}
			if ( pp > D12 )
			{
				double x23 = x3 - x2;
				double y23 = y3 - y2;
				return Math.Sqrt(x23*x23 + y23*y23);
			}
			return Math.Abs(( ( x12*y13 - y12*x13 )/D12 ));
		}

		#endregion

		/// <summary>
		/// Provides a union of multiple features.
		/// </summary>
		/// <remarks>
		/// Rather than union two polygons at one time to slowly build up a new polygon, it is more efficient
		/// to create a buffer of zero on a multi-polygon.
		/// </remarks>
		/// <param name="geometries">An array list of geometries to union.</param>
		/// <param name="arraySize">The number of geometries to union at any one time.</param>
		/// <param name="factory">The GeometryFactory to use.</param>
		/// <returns>The union of alll the geometries.</returns>
		public static Geometry Union( ArrayList geometries, int arraySize, GeometryFactory factory )
		{
			//TODO: Add a simple spatial index so we union geometries that are next to each other?

			if ( geometries == null )
			{
				throw new ArgumentNullException("geometries", " geometries must not be null");
			}

			if ( factory == null )
			{
				throw new ArgumentNullException("factory");
			}

			if ( arraySize <= 0 )
			{
				throw new ArgumentOutOfRangeException("arraySize must be a positive integer.");
			}

			// if no geometries were passed in, return an empty MultiPolygon...
			if ( geometries.Count == 0 )
			{
				return new GeometryFactory().createMultiPolygon(new Polygon[] { });
			}

			// if only one geometry in list, don't go through process just return it.
			if ( geometries.Count == 1 )
			{
				return geometries[0] as Geometry;
			}

			int unionGeometryCount = (int) Math.Ceiling((double) geometries.Count/arraySize);

			Geometry[] unionGeometryArray = new Geometry[unionGeometryCount];
			int index = 0;
			int i = 0;
			while ( index < geometries.Count )
			{
				int count = ( index + arraySize < geometries.Count ) ? arraySize : ( geometries.Count - index );
				Geometry[] geomArray = geometries.GetRange(index, count).ToArray(typeof ( Geometry )) as Geometry[];
				GeometryCollection geomClction = new GeometryCollection(geomArray, factory);
				unionGeometryArray[i] = geomClction.buffer(0);
				index += count;
				i ++;
			}

			Geometry union = null;
			if ( unionGeometryCount == 1 )
			{
				union = unionGeometryArray[0];
			}
			else
			{
				GeometryCollection geomCollectionUnionPolygons = new GeometryCollection(unionGeometryArray, factory);
				union = geomCollectionUnionPolygons.buffer(0);
			}
			return union;
		}


		/// <summary>
		/// Converts a byte array in the WKB format to a geometry.
		/// </summary>
		/// <param name="wkb">A byte[] with the WKB in it.</param>
		/// <returns>A Geometry.</returns>
		public static Geometry WkbToGeometry(byte[] wkb)
		{
			if (wkb == null)
			{
				throw new ArgumentNullException("wkb");
			}
			GeometryFactory factory = new GeometryFactory();
			GeometryWKBReader wkbReader = new GeometryWKBReader(factory);
			Geometry geometry = wkbReader.Create(wkb);
			return geometry;
		}

	}
}