using System;
using System.Drawing;
using com.vividsolutions.jts.geom;
using Geotools.Utilities;
using NUnit.Framework;

namespace Geotools.UnitTests.Utilities
{
	[TestFixture]
	public class TestAirDistance
	{
        [Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestValidCoordinate1X()
		{
			Coordinate coord1 = new Coordinate(-191,0);
			Coordinate coord2 = new Coordinate(0,0);
			AirDistance.Calculate(coord1, coord2);
		}

        [Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestValidCoordinate1X2()
		{
			Coordinate coord1 = new Coordinate(181,0);
			Coordinate coord2 = new Coordinate(0,0);
			AirDistance.Calculate(coord1, coord2);
		}

        [Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestValidCoordinate1Y()
		{
			Coordinate coord1 = new Coordinate(0,-91);
			Coordinate coord2 = new Coordinate(0,0);
			AirDistance.Calculate(coord1, coord2);
		}

        [Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestValidCoordinate1Y2()
		{
			Coordinate coord1 = new Coordinate(0,91);
			Coordinate coord2 = new Coordinate(0,0);
			AirDistance.Calculate(coord1, coord2);
		}
	}
}