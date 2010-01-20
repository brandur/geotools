#region SourceSafe Comments
/* 
 * $Header: /cvsroot/geotoolsnet/GeotoolsNet/Geotools.UnitTests/CoordinateSystems/CoordinateSystemFactoryTest.cs,v 1.4 2005/10/20 16:29:42 awcoats Exp $
 * $Log: CoordinateSystemFactoryTest.cs,v $
 * Revision 1.4  2005/10/20 16:29:42  awcoats
 * *** empty log message ***
 *
 * 
 * 9     1/02/03 10:55a Awcoats
 * removed [ignore] comments.
 * 
 * 8     12/27/02 1:00p Awcoats
 * changes  when moving from NUnit 1.0 to Nunit 2.0
 * 
 * 7     10/31/02 11:01a Awcoats
 * changed namespace from UrbanScience.Geographic to Geotools.
 * 
 * 6     10/29/02 11:24a Awcoats
 * fixed comile. Changed IDatumType to DatumType.
 * 
 * 5     10/25/02 12:26p Rabergman
 * Changed CreateProjection to accept 3 arguments instead of 4.
 * Changed CreateLocalCoordinateSystem to accept 4 arguments instead of 5
 * with the fourth being an array.
 * 
 * 4     10/18/02 1:43p Awcoats
 * interface name change.
 * 
 * 3     10/18/02 12:46p Rabergman
 * Removed tests due to internal classes
 * 
 * 2     9/24/02 3:45p Awcoats
 * 
 * 1     9/13/02 8:42a Awcoats
 * 
 */ 
#endregion

#region Using
using System;

using Geotools.CoordinateReferenceSystems;
using Geotools.IO;
using NUnit.Framework;
#endregion

namespace Geotools.UnitTests.CoordinateSystems
{
	/// <summary>
	/// Tests the basic functionality of the Geotools.UnitTests.CoordinateSystems.CoordinateSystemFactoryTest class
	/// </summary>
	[TestFixture]
	public class CoordinateSystemFactoryTest 
	{
		CoordinateSystemFactory _csFactory;
	
		[SetUp]
		public void Init()
		{
			 _csFactory = new CoordinateSystemFactory();
		}
		[TearDown]
		public void Cleanup()
		{
			_csFactory = null;
		}

		#region CreateVerticalCoordinateSystem

        [Test]
		public void TestCreateVerticalCoordinateSystem1()
		{
			IAxisInfo axis = AxisInfo.Altitude;
			ILinearUnit unit = LinearUnit.Meters;
			IVerticalDatum verticalDatum = _csFactory.CreateVerticalDatum("vertdatum",DatumType.IVD_Ellipsoidal);
			IVerticalCoordinateSystem verticalCS = _csFactory.CreateVerticalCoordinateSystem("test", verticalDatum, unit, axis);
			Assertion.AssertEquals("ctor. 1","test",verticalCS.Name);
			Assertion.AssertEquals("ctor. 2",verticalDatum,verticalCS.VerticalDatum);
			Assertion.AssertEquals("ctor. 3",unit, verticalCS.VerticalUnit);
			Assertion.AssertEquals("ctor. 4",axis, verticalCS.GetAxis(0));
		}

		#endregion

		#region CreateFlattenedSphere
		
        [Test]
		public void TestCreateFlattenedSphere1() 
		{
			ICoordinateSystemFactory csFactory = new CoordinateSystemFactory();
			IEllipsoid ellipsoid = csFactory.CreateFlattenedSphere("test",20926348.0, 294.26068, new LinearUnit(1) );
			Assertion.AssertEquals("ctor. 1 ","test",ellipsoid.Name);
			Assertion.AssertEquals("ctor. 2 ",20926348.0,ellipsoid.SemiMajorAxis);
			Assertion.AssertEquals("ctor. 3 ",20855233.000877455, ellipsoid.SemiMinorAxis);
			Assertion.AssertEquals("ctor. 4 ",294.26068,ellipsoid.InverseFlattening);
		}
	
        [Test]
		public void TestCreateFlattenedSphere2()
		{
			ICoordinateSystemFactory csFactory = new CoordinateSystemFactory();
			try
			{
				IEllipsoid ellipsoid = csFactory.CreateFlattenedSphere("test",1,2, null );
				Assertion.Fail("ArgumentNullException should be thrown  for null linear unit parameter.");
			}
			catch (ArgumentNullException)
			{
			}
		}

		#endregion

		#region CreateEllipsoid
        
        [Test]
		public void TestCreateEllipsoid1() 
		{
			IEllipsoid ellipsoid = _csFactory.CreateEllipsoid("name", 1234.0,1235,  new LinearUnit(1));
			Assertion.AssertEquals("ctor. 1 ","name",ellipsoid.Name);
			Assertion.AssertEquals("ctor. 2 ",1234.0,ellipsoid.SemiMajorAxis);
			Assertion.AssertEquals("ctor. 3 ",1235.0,ellipsoid.SemiMinorAxis);
		}

        [Test]
		public void TestCreateEllipsoid2()
		{
			try
			{
				IEllipsoid ellipsoid = _csFactory.CreateEllipsoid("test",1,2, null );
				Assertion.Fail("ArgumentNullException should be thrown  for null linear unit parameter.");
			}
			catch (ArgumentNullException)
			{
			}
		}
		#endregion

		#region CreatePrimeMeridian

        [Test]
		public void TestCreatePrimeMeridian1() 
		{
			IAngularUnit angularUnit = new AngularUnit(1);
			IPrimeMeridian primeMeridian = _csFactory.CreatePrimeMeridian("name", angularUnit,2.0);
			Assertion.AssertEquals("ctor. 1 ", "name", primeMeridian.Name);
			Assertion.AssertEquals("ctor. 2", angularUnit, primeMeridian.AngularUnit);
			Assertion.AssertEquals("ctor. 3 ", 2.0, primeMeridian.Longitude);
		}

        [Test]
		public void TestCreatePrimeMeridian2()
		{
			try
			{
				IPrimeMeridian primeMeridian = _csFactory.CreatePrimeMeridian("name", null,2.0);
				Assertion.Fail("ArgumentNullException should have been thrown.");
			}
			catch(ArgumentNullException)
			{
			}
		}

		#endregion

		#region CreateVerticalDatum

        [Test]
		public void TestCreateVerticalDatum1()
		{
			IDatum datum = _csFactory.CreateVerticalDatum("name",DatumType.IHD_Geocentric);
			Assertion.AssertEquals("ctor.1","name",datum.Name);
			Assertion.AssertEquals("ctor.2",DatumType.IHD_Geocentric, datum.DatumType);
		}
		
        [Test]
		public void TestCreateVerticalDatum2()
		{
			try
			{
				IDatum datum = _csFactory.CreateVerticalDatum(null,DatumType.IHD_Geocentric);
				Assertion.Fail("ArgumentNullException should have been thrown.");
			}
			catch(ArgumentNullException)
			{
			}
		}

		#endregion

		#region CreateHorizontalDatum

        [Test]
		public void TestCreateHorizontalDatum1()
		{
			ILinearUnit linearUnit = new LinearUnit(1);
			IEllipsoid ellipsoid = _csFactory.CreateFlattenedSphere("test",1,2, linearUnit );
			WGS84ConversionInfo wgs = new WGS84ConversionInfo();
			wgs.Dx=1;
			
			IHorizontalDatum horizontalDatum = _csFactory.CreateHorizontalDatum("name", DatumType.IHD_Geocentric, ellipsoid, wgs);
			Assertion.AssertEquals("ctor 1","name",horizontalDatum.Name);
			Assertion.AssertEquals("ctor 2",DatumType.IHD_Geocentric,horizontalDatum.DatumType);
			Assertion.AssertEquals("ctor 3",ellipsoid,horizontalDatum.Ellipsoid);
			Assertion.AssertEquals("ctor 4",wgs,horizontalDatum.WGS84Parameters);
		}

        [Test]
		public void TestCreateHorizontalDatum2()
		{
			ILinearUnit linearUnit = new LinearUnit(1);
			IEllipsoid ellipsoid = _csFactory.CreateFlattenedSphere("test",1,2, linearUnit );
			WGS84ConversionInfo wgs = new WGS84ConversionInfo();
			wgs.Dx=1;
			
			try
			{
				IHorizontalDatum horizontalDatum = _csFactory.CreateHorizontalDatum("name",DatumType.IHD_Geocentric, null, wgs);
				Assertion.Fail("Should throw a ArgumentNullException.");
			}
			catch(ArgumentNullException)
			{
			}
		}
        
		#endregion

		#region CreateProjection
        
        [Test]
		public void TestCreateProjection1()
		{
			try
			{
				ProjectionParameter[] projectionParams = new ProjectionParameter[2];
				IProjection horizontalDatum = _csFactory.CreateProjection(null,"class",projectionParams);
				Assertion.Fail("Should throw a ArgumentNullException.");
			}
			catch(ArgumentNullException)
			{
			}
		}

        [Test]
		public void TestCreateProjection2()
		{
			try
			{
				ProjectionParameter[] projectionParams = new ProjectionParameter[2];
				IProjection horizontalDatum = _csFactory.CreateProjection("mercator","class",projectionParams);
				Assertion.Fail("Should throw a ArgumentException because the params are different lengths.");
			}
			catch(ArgumentException)
			{
			}
		}

        [Test]
		public void TestCreateProjection3()
		{
			try
			{
				ProjectionParameter[] projectionParams = new ProjectionParameter[1];
				ProjectionParameter param = new ProjectionParameter();
				param.Name="dummy";
				param.Value=2.0;
				projectionParams[0] = param;
				IProjection horizontalDatum = _csFactory.CreateProjection("notexistent","class",projectionParams);
				Assertion.Fail("Should throw a NotImplementedException because the projection does not exist.");
			}
			catch(NotImplementedException)
			{
			}
		}

		#endregion

		#region CreateProjectedCoordinateSystem

        [Test]
		public void TestCreateProjectedCoorinateSystem1()
		{
			//TODO:
		}

		#endregion

		#region TestCreateLocalDatum1

        [Test]
		public void TestCreateLocalDatum1()
		{
			ILocalDatum datum = _csFactory.CreateLocalDatum("name",DatumType.IHD_Geocentric);
			Assertion.AssertEquals("ctor. 1","name",datum.Name);
			Assertion.AssertEquals("ctor. 2",DatumType.IHD_Geocentric,datum.DatumType);
		}

        [Test]
		public void TestCreateLocalDatum2()
		{
			try
			{
				ILocalDatum datum = _csFactory.CreateLocalDatum(null,DatumType.IHD_Geocentric);
				Assertion.Fail("Should throw a ArgumentNullException.");
			}
			catch(ArgumentNullException)
			{
			}
		}

		#endregion
	
		#region Not implemented methods

        [Test]
		public void TestCreateFittedCoordinateSystem1()
		 {
			 try
			 {
				 IAxisInfo[] axis = new IAxisInfo[1];
				 axis[0] = AxisInfo.X;
				 _csFactory.CreateFittedCoordinateSystem("name",null,"aa",axis);
				 Assertion.Fail("Should throw a not implemented exception.");
			 }
			 catch(NotImplementedException)
			 {
			 }
		}

        [Test]
		public void TestCreateLocalCoordinateSystem2()
		{
			try
			{
				IAxisInfo[] axis = new IAxisInfo[1];
				axis[0] = AxisInfo.X;
				_csFactory.CreateLocalCoordinateSystem("name",null,null,axis);
				Assertion.Fail("Should throw a not implemented exception.");
			}
			catch(NotImplementedException)
			{
			}
		}
		
		#endregion
	}
}
