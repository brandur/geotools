#region SourceSafe Comments
/* 
 * $Header: /cvsroot/geotoolsnet/GeotoolsNet/Geotools.UnitTests/CoordinateSystems/HorizontalCoordinateSystemTest.cs,v 1.4 2005/10/20 16:29:42 awcoats Exp $
 * $Log: HorizontalCoordinateSystemTest.cs,v $
 * Revision 1.4  2005/10/20 16:29:42  awcoats
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
#endregion

namespace Geotools.UnitTests.CoordinateSystems
{
	/// <summary>
	/// Tests the basic functionality of the Geotools.UnitTests.CoordinateSystems.HorizontalCoordinateSystemTest class
	/// </summary>
	[TestFixture]
	public class HorizontalCoordinateSystemTest  
	{
        [Test]
		public void Test_Constructor() 
		{
			IEllipsoid ellipsoid = new Ellipsoid(20926348,-1.0,294.26068, true,new LinearUnit(1));
			WGS84ConversionInfo wgsInfo = new WGS84ConversionInfo();
			wgsInfo.Dx=1.0;
			HorizontalDatum horizontalDatum = new HorizontalDatum("name",DatumType.IHD_Geocentric,ellipsoid, wgsInfo);
			
			IAxisInfo[] axisInfos = new IAxisInfo[2];
			axisInfos[0] = AxisInfo.Latitude;
			axisInfos[1] = AxisInfo.Longitude;
			HorizontalCoordinateSystem horzCS = new HorizontalCoordinateSystem(horizontalDatum,axisInfos,"remarks","authority","code","name","alias","abbreviation");

			Assertion.AssertEquals("ctor1.","remarks",horzCS.Remarks);
			Assertion.AssertEquals("ctor2.","authority",horzCS.Authority);
			Assertion.AssertEquals("ctor3.","code",horzCS.AuthorityCode);
			Assertion.AssertEquals("ctor4.","name",horzCS.Name);
			Assertion.AssertEquals("ctor5.","alias",horzCS.Alias);
			Assertion.AssertEquals("ctor6.","abbreviation",horzCS.Abbreviation);

			Assertion.AssertEquals("test 7",horizontalDatum,horzCS.HorizontalDatum);
			//Assertion.AssertEquals("test 8",axisInfos[0],horzCS.GetAxis(0));
			//Assertion.AssertEquals("test 9",axisInfos[1],horzCS.GetAxis(1));
		}
	}
}

