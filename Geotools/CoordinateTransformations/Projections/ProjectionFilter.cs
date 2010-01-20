using System;
using com.vividsolutions.jts.geom;
using Geotools.Positioning;

namespace Geotools.CoordinateTransformations
{
	/// <summary>
	/// Filter used to apply a coordinate system projection to a <b>Geometry</b> object.
	/// </summary>
	public class ProjectionFilter : CoordinateFilter
	{
		private ICoordinateTransformation _projection;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectionFilter">ProjectionFilter</see> class.
		/// </summary>
		/// <param name="projection">The <see cref="Geotools.CoordinateTransformations.ICoordinateTransformation">ICoordinateTransformation</see> to apply.</param>
		public ProjectionFilter( ICoordinateTransformation projection )
		{
			if (projection==null)
			{
				throw new ArgumentNullException("projection");
			}
			_projection = projection;
		}

		/// <summary>
		/// Applies the filter to the <b>Coordinate</b>.
		/// </summary>
		/// <param name="coord">The <b>Coordinate</b>.</param>
		public void filter( Coordinate coord )
		{
			if ( _projection == null )
			{
				return;
			}

			CoordinatePoint projectedCoordinatePoint = _projection.MathTransform.Transform(this.CreateCoordinatePoint(coord));

			coord.x = projectedCoordinatePoint.Ord[0];
			coord.y = projectedCoordinatePoint.Ord[1];
		}

		private CoordinatePoint CreateCoordinatePoint( Coordinate coord )
		{
			CoordinatePoint pt = new CoordinatePoint();
			pt.Ord = new Double[2];
			pt.Ord[0] = coord.x;
			pt.Ord[1] = coord.y;

			return pt;
		}
	}
}
