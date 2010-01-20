#region SourceSafe Comments
/* 
 * $Header: /cvsroot/geotoolsnet/GeotoolsNet/Geotools.UnitTests/CoordinateSystems/PrimeMeridianTest.cs,v 1.4 2005/10/20 16:29:56 awcoats Exp $
 * $Log: PrimeMeridianTest.cs,v $
 * Revision 1.4  2005/10/20 16:29:56  awcoats
 * *** empty log message ***
 *
 * 
 * 8     12/27/02 1:00p Awcoats
 * changes  when moving from NUnit 1.0 to Nunit 2.0
 * 
 * 7     10/31/02 11:01a Awcoats
 * changed namespace from UrbanScience.Geographic to Geotools.
 * 
 * 6     10/18/02 1:43p Awcoats
 * interface name change.
 * 
 * 5     10/18/02 12:54p Rabergman
 * Removed tests due to internal classes
 * 
 * 4     9/24/02 3:45p Awcoats
 * 
 * 3     9/13/02 8:43a Awcoats
 * 
 * 2     8/15/02 11:21a Awcoats
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
	/// Tests the basic functionality of the UrbanScience.OpenGIS.UnitTests.CoordinateSystems.PrimeMeridianTest class
	/// </summary>
	[TestFixture]
	public class PrimeMeridianTest 
	{
        [Test]
		public void Test_Test1()
		{
			AngularUnit angularUnit = new AngularUnit(0.5);
			PrimeMeridian primeMeridian = new PrimeMeridian("name",angularUnit,5.0,"remarks",
				"authority","authorityCode","alias","abbreviation");

			Assertion.AssertEquals("Test 1", "abbreviation", primeMeridian.Abbreviation);
			Assertion.AssertEquals("Test 2", "alias", primeMeridian.Alias);
			Assertion.AssertEquals("Test 3", "authority", primeMeridian.Authority);
			Assertion.AssertEquals("Test 4", "authorityCode", primeMeridian.AuthorityCode);
			Assertion.AssertEquals("Test 5", "name", primeMeridian.Name);
			Assertion.AssertEquals("Test 6", "remarks", primeMeridian.Remarks);

			Assertion.AssertEquals("Test 7", angularUnit, primeMeridian.AngularUnit);
			Assertion.AssertEquals("Test 8",2.5,primeMeridian.Longitude);
		}
	}
}

