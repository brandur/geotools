using System;
using NUnit.Framework;
using Geotools.CoordinateReferenceSystems;
namespace Geotools.UnitTests.CoordinateSystems
{
	[TestFixture]
	public class TestLinearUnitConverter
	{
        [Test]
		public void TestName()
		{
			Meters meters = new Meters(1000);
			Miles miles = LinearUnitConverter.ToMiles(meters);
			Assertion.AssertEquals(0.62137, miles.Value,0.001);

			Kilometers km1 = LinearUnitConverter.ToKilometers(miles);
			Assertion.AssertEquals(1,km1.Value);

			Kilometers km = LinearUnitConverter.ToKilometers(meters);
			Assertion.AssertEquals(1, km.Value);
		}
	}
}