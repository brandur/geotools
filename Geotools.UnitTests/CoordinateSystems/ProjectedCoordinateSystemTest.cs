#region SourceSafe Comments
/* 
 * $Header: /cvsroot/geotoolsnet/GeotoolsNet/Geotools.UnitTests/CoordinateSystems/ProjectedCoordinateSystemTest.cs,v 1.4 2005/10/20 16:29:56 awcoats Exp $
 * $Log: ProjectedCoordinateSystemTest.cs,v $
 * Revision 1.4  2005/10/20 16:29:56  awcoats
 * *** empty log message ***
 *
 * 
 * 5     12/27/02 1:00p Awcoats
 * changes  when moving from NUnit 1.0 to Nunit 2.0
 * 
 * 4     10/31/02 11:01a Awcoats
 * changed namespace from UrbanScience.Geographic to Geotools.
 * 
 * 3     10/18/02 1:43p Awcoats
 * interface name change.
 * 
 * 2     10/18/02 12:54p Rabergman
 * Removed tests due to internal classes
 * 
 * 1     9/24/02 3:44p Awcoats
 * 
 */ 
#endregion

#region Using
using System;
using NUnit.Framework;

using Geotools.CoordinateReferenceSystems;
using Geotools.CoordinateTransformations;
#endregion

namespace Geotools.UnitTests.CoordinateSystems
{
	/// <summary>
	/// Tests the basic functionality of the Geotools.UnitTests.CoordinateSystems.ProjectedCoordinateSystemTest class
	/// </summary>
    [TestFixture]
	public class ProjectedCoordinateSystemTest 
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
			
			//PrimeMeridian primeMeridian = new PrimeMeridian("name", angularUnit, 0.5);
			IAxisInfo[] axisArray = new IAxisInfo[2];
			axisArray[0]=axis0;
			axisArray[1]=axis1;

			ProjectionParameter[] paramList = new ProjectionParameter[1];
			paramList[0].Name="test";
			paramList[0].Value=2.2;

			Projection projection = new Projection("mercator",paramList,"class","remarks","authority","authoritycode");

			ProjectedCoordinateSystem pjc = new ProjectedCoordinateSystem(horizontalDatum,
				axisArray,gcs, linearUnit, projection,
				"remarks","authority","authorityCode","name","alias","abbreviation");
		
			Assertion.AssertEquals("Test 1",linearUnit,pjc.LinearUnit);
			Assertion.AssertEquals("Test 2",horizontalDatum,pjc.HorizontalDatum);
			Assertion.AssertEquals("Test 3",axis0,pjc.GetAxis(0));
			Assertion.AssertEquals("Test 4",axis1,pjc.GetAxis(1));
			Assertion.AssertEquals("Test 5",gcs,pjc.GeographicCoordinateSystem);

			Assertion.AssertEquals("Test 6", "abbreviation", pjc.Abbreviation);
			Assertion.AssertEquals("Test 7", "alias", pjc.Alias);
			Assertion.AssertEquals("Test 8", "authority", pjc.Authority);
			Assertion.AssertEquals("Test 9", "authorityCode", pjc.AuthorityCode);
			Assertion.AssertEquals("Test 10", "name", pjc.Name);
			Assertion.AssertEquals("Test 11", "remarks", pjc.Remarks);
		}
	}
}

