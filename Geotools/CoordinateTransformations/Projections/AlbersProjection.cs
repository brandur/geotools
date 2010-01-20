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
#endregion

namespace Geotools.CoordinateTransformations
{
	/// <summary>
	///		Implements the Albers projection.
	/// </summary>
	/// <remarks>
	/// 	<para>Implements the Albers projection. The Albers projection is most commonly
	/// 	used to project the United States of America. It gives the northern
	/// 	border with Canada a curved appearance.</para>
	/// 	
	///		<para>The <a href="http://www.geog.mcgill.ca/courses/geo201/mapproj/naaeana.gif">Albers Equal Area</a>
	///		projection has the property that the area bounded
	///		by any pair of parallels and meridians is exactly reproduced between the 
	///		image of those parallels and meridians in the projected domain, that is,
	///		the projection preserves the correct area of the earth though distorts
	///		direction, distance and shape somewhat.</para>
	/// </remarks>
	internal class AlbersProjection : MapProjection
	{
	
		double TOL = 1E-10;
		/// <summary>
		///  Maximum difference allowed when comparing real numbers.
		/// </summary>
		private static double EPS = 1E-7;

		/// <summary>
		/// Maximum number of itterations for the inverse calculation.
		/// </summary>
		private static int MAX_ITER = 15;

		/// <summary>
		///  Constants used by the spherical and elliptical Albers projection.
		/// </summary>
		private double n, c, rho0;

		/// <summary>
		/// An error condition indicating itteration will not converge for the inverse ellipse. See Snyder (14-20)
		/// </summary>
		private double ec;

		/// <summary>
		/// Standards parallels in radians, for {@link #toString} implementation.
		/// </summary>
		private double phi1, phi2;

		#region Constructors

		//double _semiMajor;
		//double _semiMinor;
		double _centralMeridian;
		double _latitudeOfOrigin;
		double _globalScale;
		double _scaleFactor;
		double _falseEasting;
		double _falseNorthing;

		/// <summary>
		/// Creates an instance of an Albers projection object.
		/// </summary>
		/// <param name="parameters">List of parameters to initialize the projection.</param>
		/// <remarks>
		/// <para>The parameters this projection expects are listed below.</para>
		/// <list type="table">
		/// <listheader><term>Items</term><description>Descriptions</description></listheader>		<item><term>latitude_of_false_origin</term><description>The latitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
		/// <item><term>longitude_of_false_origin</term><description>The longitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
		/// <item><term>latitude_of_1st_standard_parallel</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is nearest the pole.  Scale is true along this parallel.</description></item>
		/// <item><term>latitude_of_2nd_standard_parallel</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is furthest from the pole.  Scale is true along this parallel.</description></item>
		/// <item><term>easting_at_false_origin</term><description>The easting value assigned to the false origin.</description></item>
		/// <item><term>northing_at_false_origin</term><description>The northing value assigned to the false origin.</description></item>
		/// </list>
		/// </remarks>
		public AlbersProjection(ParameterList parameters) : this(parameters,false)
		{
			
		}
		
		/// <summary>
		/// Creates an instance of an Albers projection object.
		/// </summary>
		/// <remarks>
		/// <para>The parameters this projection expects are listed below.</para>
		/// <list type="table">
		/// <listheader><term>Items</term><description>Descriptions</description></listheader>		<item><term>latitude_of_false_origin</term><description>The latitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
		/// <item><term>longitude_of_false_origin</term><description>The longitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
		/// <item><term>latitude_of_1st_standard_parallel</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is nearest the pole.  Scale is true along this parallel.</description></item>
		/// <item><term>latitude_of_2nd_standard_parallel</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is furthest from the pole.  Scale is true along this parallel.</description></item>
		/// <item><term>easting_at_false_origin</term><description>The easting value assigned to the false origin.</description></item>
		/// <item><term>northing_at_false_origin</term><description>The northing value assigned to the false origin.</description></item>
		/// </list>
		/// </remarks>
		/// <param name="parameters">List of parameters to initialize the projection.</param>
		/// <param name="isInverse">Indicates whether the projection forward (meters to degrees or degrees to meters).</param>
		public AlbersProjection(ParameterList parameters, bool isInverse) : base(parameters,isInverse)
		{
		
			_semiMajor        =                    parameters.GetDouble("semi_major");
			_semiMinor        =                    parameters.GetDouble("semi_minor");
			_centralMeridian  = Degrees2Radians(parameters.GetDouble("central_meridian"));
			_latitudeOfOrigin =  Degrees2Radians(parameters.GetDouble("latitude_of_origin"));
			_scaleFactor      =                    parameters.GetDouble("scale_factor",1   );
			_falseEasting     =                    parameters.GetDouble("false_easting");
			_falseNorthing    =                    parameters.GetDouble("false_northing");
			_isSpherical      = (_semiMajor == _semiMinor);
			_globalScale      = _scaleFactor*_semiMajor;

			phi1 = Degrees2Radians( parameters.GetDouble("standard_parallel_1") );
			phi2 = Degrees2Radians( parameters.GetDouble("standard_parallel_2") );

			if (Math.Abs(phi1 + phi2) < TOL)
			{
				throw new ArgumentOutOfRangeException("Projection is converging....");
				/*throw new IllegalArgumentException(Resources.format(
					ResourceKeys.ERROR_ANTIPODE_LATITUDES_$2,
					new Latitude(Math.toDegrees(phi1)),
					new Latitude(Math.toDegrees(phi2))));*/
			}

         

			double  sinphi = Math.Sin(phi1);
			double  cosphi = Math.Cos(phi1);

			double  n      = sinphi;

			bool secant = (Math.Abs(phi1 - phi2) >= TOL);

			if (this._isSpherical)
			{
				if (secant) 
				{

					n = 0.5 * (n + Math.Sin(phi2));

				}           

				c    = cosphi * cosphi + n*2 * sinphi;

				rho0 = Math.Sqrt(c - n*2 * Math.Sin(_latitudeOfOrigin)) /n;

				ec   = Double.NaN;
			} 
			else 
			{
				double m1 = msfn(sinphi, cosphi);
				double q1 = qsfn(sinphi);

				if (secant) 
				{ /* secant cone */

					sinphi    = Math.Sin(phi2);

					cosphi    = Math.Cos(phi2);

					double m2 = msfn(sinphi, cosphi);

					double q2 = qsfn(sinphi);

					n = (m1 * m1 - m2 * m2) / (q2 - q1);

				}

				c = m1 * m1 + n * q1;

				rho0 = Math.Sqrt(c - n * qsfn(Math.Sin(_latitudeOfOrigin))) /n;

				ec = 1.0 - .5 * (1.0-_es) * Math.Log((1.0 - _e) / (1.0 + _e)) / _e;

			}

			this.n = n;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Converts coordinates in decimal degrees to projected meters.
		/// </summary>
		/// <param name="x">The longitude in decimal degrees.</param>
		/// <param name="y">The latitude in decimal degrees.</param>
		/// <param name="metersX">The resulting x coordinate in projected meters.</param>
		/// <param name="metersY">The resutting y coordinate in projected meters.</param>
		public override void DegreesToMeters(double x, double y,out double metersX, out double metersY)
		{
			x = Degrees2Radians(x);
			x=x- this._centralMeridian;

			y = Degrees2Radians(y);
			x *= n;

			double rho;

			if (_isSpherical) 
			{

				rho = c - n*2 * Math.Sin(y);

			} 
			else 
			{

				rho = c - n * qsfn(Math.Sin(y));

			}



			if (rho < 0.0) 
			{

				// TODO: fix message (and check when this condition will occur)

				// is this only checking for an impossible divide by 0 condition?

				throw new InvalidOperationException("Tolerance condition error");

			}

			rho = Math.Sqrt(rho) / n;

			y   = rho0 - rho * Math.Cos(x);

			x   =        rho * Math.Sin(x);

			
			metersX = _globalScale * x + this._falseEasting;
			metersY = _globalScale * y + this._falseNorthing;
			

		}

		/// <summary>
		/// Converts coordinates in projected meters to decimal degrees.
		/// </summary>
		/// <param name="x">The x coordinate in projected meters.</param>
		/// <param name="y">The y coordinate in projected meters.</param>
		/// <param name="dLongitude">The resulting longitude in decimal degrees.</param>
		/// <param name="dLatitude">The resulitng latitude in decimal degrees.</param>
		public override void MetersToDegrees(double x, double y,out double dLongitude, out double dLatitude) 
		{
			x = (x- _falseEasting)/_globalScale;
			y= (y- _falseNorthing)/ _globalScale;
			y = rho0 - y;


			double rho = Math.Sqrt(x*x + y*y);

			if (rho  != 0.0) 
			{

				if (n < 0.0) 
				{

					rho = -rho;

					x   = -x;

					y   = -y;

				}

				x = Math.Atan2(x, y) / n;

				y =  rho*n;

				if (_isSpherical) 
				{

					y = (c - y * y) / (n*2);

					if (Math.Abs(y) <= 1.0)
					{

						y = Math.Asin(y);

					}

					else 
					{

						y = (y < 0.0) ? -Math.PI/2.0 : Math.PI/2.0;

					}     

				} 
				else 
				{

					y = (c - y*y) / n;

					if (Math.Abs(ec - Math.Abs(y)) > EPS) 
					{

						y = Phi1(y);

					} 
					else 
					{

						y = (y < 0.0) ? -Math.PI/2.0 : Math.PI/2.0;

					} 

				}   

			} 
			else 
			{

				x = 0.0;

				y = n > 0.0 ? Math.PI/2.0 : - Math.PI/2.0;

			}




			dLongitude = Radians2Degrees(x + this._centralMeridian);
			dLatitude = Radians2Degrees(y);
	
		}
		
		/// <summary>
		/// Returns the inverse of this projection.
		/// </summary>
		/// <returns>IMathTransform that is the reverse of the current projection.</returns>
		public override IMathTransform GetInverse()
		{
			if (_inverse==null)
			{
				_inverse = new AlbersProjection(this._parameters, ! _isInverse);
			}
			return _inverse;
		}
		#endregion

	
		/// <summary>
		/// Calculates q, Snyder equation (3-12)
		/// </summary>
		/// <param name="sinphi">@param sinphi sin of the latitude q is calculated for</param>
		/// <returns>@return q from Snyder equation (3-12)</returns>
		private double qsfn(double sinphi) 
		{

			double one_es = 1 - _es;

			if (_e >= EPS) 
			{

				double con = _e * sinphi;

				return (one_es * (sinphi / (1.0 - con*con) -

					(0.5/_e) * Math.Log((1.0-con) / (1.0+con))));

			} 
			else 
			{
				return sinphi + sinphi;

			}

		}

		double msfn(double s, double c) 
		{
			return c / Math.Sqrt(1.0 - s*s*_es);
		}


		/// <summary>
		///  Iteratively solves equation (3-16) from Snyder.
		/// </summary>
		/// <param name="qs">@param qs arcsin(q/2), used in the first step of itteration</param>
		/// <returns>* @return the latitude</returns>
		private double Phi1(double qs) 
		{

			double tone_es = 1 - _es;

			double phi = Math.Asin(0.5 * qs);

			if (_e < EPS) 
			{

				return phi;

			}

			for (int i=0; i<MAX_ITER; i++) 
			{

				double sinpi = Math.Sin(phi);

				double cospi = Math.Cos(phi);

				double con   = _e * sinpi;

				double com   = 1.0 - con*con;

				double dphi  = 0.5 * com*com / cospi * 

					(qs/tone_es - sinpi / com + 0.5/_e * 

					Math.Log((1.0 - con) / (1.0 + con)));

				phi += dphi;

				if (Math.Abs(dphi) <= TOL) 
				{

					return phi;

				}

			} 

			throw new InvalidOperationException("No convergance.");

		}
	}
}
