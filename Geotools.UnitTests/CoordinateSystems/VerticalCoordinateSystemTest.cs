#region SourceSafe Comments
/* 
 * $Header: /cvsroot/geotoolsnet/GeotoolsNet/Geotools.UnitTests/CoordinateSystems/VerticalCoordinateSystemTest.cs,v 1.4 2005/10/20 16:29:56 awcoats Exp $
 * $Log: VerticalCoordinateSystemTest.cs,v $
 * Revision 1.4  2005/10/20 16:29:56  awcoats
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
 * 1     9/18/02 11:25a Awcoats
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
	/// Tests the basic functionality of the Geotools.UnitTests.CoordinateSystems.VerticalCoordinateSystemTest class
	/// </summary>
	[TestFixture]
	public class VerticalCoordinateSystemTest 
	{
        [Test]
		public void Test_Constructor() 
		{
			VerticalDatum datum = VerticalDatum.Ellipsoidal;
			IAxisInfo axis = AxisInfo.Altitude;
			ILinearUnit unit = LinearUnit.Meters;
			
			VerticalCoordinateSystem vcs = new VerticalCoordinateSystem("test1",datum, axis, unit);
			Assertion.AssertEquals("Test1",datum, vcs.VerticalDatum);
			Assertion.AssertEquals("Test2",1.0,vcs.VerticalUnit.MetersPerUnit);
			Assertion.AssertEquals("ctor. 3",unit, vcs.VerticalUnit);
			Assertion.AssertEquals("ctor. 4",axis, vcs.GetAxis(0));
		}

        [Test]
		public void Test_StaticConstructor() 
		{
			IVerticalCoordinateSystem vcs = VerticalCoordinateSystem.Ellipsoidal;
			Assertion.AssertEquals("Test1","Ellipsoidal",vcs.Name);
		}
	}
}

