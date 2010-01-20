#region SourceSafe Comments
/* 
 * $Header: /cvsroot/geotoolsnet/GeotoolsNet/Geotools.UnitTests/CoordinateSystems/GeographicCoordinateSystemTest.cs,v 1.4 2005/10/20 16:29:42 awcoats Exp $
 * $Log: GeographicCoordinateSystemTest.cs,v $
 * Revision 1.4  2005/10/20 16:29:42  awcoats
 * *** empty log message ***
 *
 * 
 * 6     12/27/02 1:00p Awcoats
 * changes  when moving from NUnit 1.0 to Nunit 2.0
 * 
 * 5     10/31/02 11:01a Awcoats
 * changed namespace from UrbanScience.Geographic to Geotools.
 * 
 * 4     10/18/02 1:43p Awcoats
 * interface name change.
 * 
 * 3     10/18/02 12:54p Rabergman
 * Removed tests due to internal classes
 * 
 * 2     9/24/02 3:45p Awcoats
 * 
 * 1     8/14/02 2:21p Awcoats
 * 
 */ 
#endregion

#region Using
using System;
using NUnit.Framework;

using Geotools.CoordinateReferenceSystems;
#endregion

namespace Geotools.UnitTests.CoordinateSystems
{
	/// <summary>
	/// Tests the basic functionality of the UrbanScience.OpenGIS.UnitTests.CoordinateSystems.GeographicCoordinateSystemTest class
	/// </summary>
	[TestFixture]
	public class GeographicCoordinateSystemTest 
	{
		/// <summary>
		/// Tests the constructor
		/// </summary>
        [Test]
		public void Test_Constructor() 
		{
			ICoordinateSystemFactory csFactory = new CoordinateSystemFactory();
			IAngularUnit angularUnit = new AngularUnit(1);
			ILinearUnit linearUnit = new LinearUnit(1);
			IEllipsoid ellipsoid = csFactory.CreateFlattenedSphere("test",1,2, linearUnit );
			IAxisInfo axis0 = new AxisInfo("axis0name", AxisOrientation.Up);
			IAxisInfo axis1 = new AxisInfo("axis1name", AxisOrientation.Up);
			WGS84ConversionInfo wgs = new WGS84ConversionInfo();
			
			IPrimeMeridian primeMeridian = csFactory.CreatePrimeMeridian("name", angularUnit,2.0);
			IHorizontalDatum horizontalDatum = csFactory.CreateHorizontalDatum("datum",DatumType.IHD_Geocentric,ellipsoid, wgs);
			IGeographicCoordinateSystem gcs = csFactory.CreateGeographicCoordinateSystem("name",angularUnit, horizontalDatum, primeMeridian, axis0, axis1);

			Assertion.AssertEquals("ctor 1","name",gcs.Name);
			Assertion.AssertEquals("ctor 2",angularUnit,gcs.AngularUnit);
			Assertion.AssertEquals("ctor 3",horizontalDatum,gcs.HorizontalDatum);
			Assertion.AssertEquals("ctor 4",primeMeridian,gcs.PrimeMeridian);
			Assertion.AssertEquals("ctor 5",axis0,gcs.GetAxis(0));
			Assertion.AssertEquals("ctor 5",axis1,gcs.GetAxis(1));
		}
	}
}

