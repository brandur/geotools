/*
 *  Copyright (C) 2002 Urban Science Applications, Inc. (translated from Java Topology Suite, 
 *  Copyright 2001 Vivid Solutions)
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

using System;
using System.IO;
using System.Collections;
using Geotools.Utilities;
using com.vividsolutions.jts.geom;

namespace Geotools.IO
{
	/// <summary>
	///  Converts a Well-known Text string to a Geometry object.
	/// </summary>
	/// <remarks>The Well-known
	///  <para>Text format is defined in the 
	///  OpenGIS Simple Features Specification for SQL</para>
	///
	///  <para>The Well-Known Text reader (GeometryWKTReader) is designed to allow
	///  extracting Geometry objects from either input streams or
	///  internal strings. This allows it to function as a parser to read Geometry
	///  objects from text blocks embedded in other data formats (e.g. XML).</para>
	///
	///  <para>Note: There is an inconsistency in the SFS. The WKT grammar states
	///  that MultiPoints are represented by MULTIPOINT ( ( x y), (x y) )
	///  , but the examples show MultiPoints as MULTIPOINT ( x y, x y )
	///  . Other implementations follow the latter syntax, so JTS will adopt it as
	///  well.</para>
	///
	///  <para>A GeometryWKTReader is parameterized by a GeometryFactory
	///  , to allow it to create Geometry objects of the appropriate
	///  implementation. In particular, the GeometryFactory will
	///  determine the PrecisionModel and SRID that is used.</para>
	///
	///  <para>The GeometryWKTReader will convert the input numbers to the precise
	///  internal representation.</para>
	/// </remarks> 
	public class GeometryWKTReader
	{
		private GeometryFactory _factory;
		private PrecisionModel _precisionModel;

		/// <summary>
		/// Creates a <see cref="GeometryWKTReader">GeometryWKTReader</see> that creates objects using the given <b>GeometryFactory</b>.
		/// </summary>
		/// <param name="factory">The <b>GeometryFactory</b> used to create geometries.</param>
		public GeometryWKTReader(GeometryFactory factory) 
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}

			_factory = factory;
			_precisionModel = factory.getPrecisionModel();
		}		

		/// <summary>
		/// Converts a Well-known text representation to a <b>Geometry</b>.
		/// </summary>
		/// <param name="wellKnownText">A geometry tagged text string (see the OpenGIS Simple Features Specification).</param>
		/// <returns>Returns a <b>Geometry</b> specified by wellKnownText.  Throws an exception if there is a parsing problem.</returns>
		public Geometry Create(string wellKnownText)
		{
			using (StringReader reader = new StringReader(wellKnownText))
			{
				return Create(reader);
			}
		}

		/// <summary>
		/// Converts a Well-known Text representation to a <b>Geometry</b>.
		/// </summary>
		/// <param name="reader">A reader which contains a geometry tagged text string (see the OpenGIS Simple Features Specification).</param>
		/// <returns>
		/// Returns a <b>Geometry</b> read from the reader.  An exception will be thrown if there is a
		/// parsing problem.
		/// </returns>
		public Geometry Create(TextReader reader) 
		{
			WktStreamTokenizer tokenizer = new WktStreamTokenizer(reader);
	
			return this.ReadGeometryTaggedText(tokenizer);
		}

		private Coordinate[] GetCoordinates(WktStreamTokenizer tokenizer) 
		{
			ArrayList coords = new ArrayList();

			string nextToken = this.GetNextEmptyOrOpener(tokenizer);

			if (nextToken == "EMPTY")
			{
				return new Coordinate[] {};
			}

			do
			{
				Coordinate coord = new Coordinate(this.GetNextNumber(tokenizer), this.GetNextNumber(tokenizer));
				_precisionModel.makePrecise(coord);
				coords.Add(coord);
			}
			while (this.GetNextCloserOrComma(tokenizer) == ",");

			return (Coordinate[])coords.ToArray(typeof(Coordinate));
		}

		private double GetNextNumber(WktStreamTokenizer tokenizer) 
		{
			tokenizer.NextToken();

			return tokenizer.GetNumericValue();
		}

		private string GetNextEmptyOrOpener(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();

			string nextWord = tokenizer.GetStringValue();

			if ((nextWord == "EMPTY") || (nextWord == "("))
			{
				return nextWord;
			}

			throw new ParseException("Expected 'EMPTY' or '(' but encountered '" +	nextWord + "'");
		}

		private string GetNextCloserOrComma(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();

			string nextWord = tokenizer.GetStringValue();

			if ((nextWord == ",") || (nextWord == ")"))
			{
				return nextWord;
			}

			throw new ParseException("Expected ')' or ',' but encountered '" + nextWord	+ "'");
	
		}

		private string GetNextCloser(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();

			string nextWord = tokenizer.GetStringValue();

			if (nextWord == ")")
			{
				return nextWord;
			}

			throw new ParseException("Expected ')' but encountered '" + nextWord + "'");
		}

		private string GetNextWord(WktStreamTokenizer tokenizer)
		{
			TokenType type = tokenizer.NextToken();
			string token = tokenizer.GetStringValue();

			if (type == TokenType.Number)
			{
				throw new ParseException("Expected  a number but got "+token);
			}
			else if (type == TokenType.Word)
			{
				return token.ToUpper();
			}
			else if (token == "(")
			{
				return "(";
			}
			else if (token == ")")
			{
				return ")";
			}
			else if (token == ",")
			{
				return ",";
			}
			
			throw new ParseException("Not a valid symbol in WKT format.");
		}

		private Geometry ReadGeometryTaggedText(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();

			string type = tokenizer.GetStringValue().ToUpper();

			switch (type)
			{
				case "POINT":
					return this.ReadPointText(tokenizer);

				case "LINESTRING":
					return this.ReadLineStringText(tokenizer);
	
				case "MULTIPOINT":
					return this.ReadMultiPointText(tokenizer);

				case "MULTILINESTRING":
					return this.ReadMultiLineStringText(tokenizer);

				case "POLYGON":
					return this.ReadPolygonText(tokenizer);
					
				case "MULTIPOLYGON":
					return this.ReadMultiPolygonText(tokenizer);

				case "GEOMETRYCOLLECTION":
					return this.ReadGeometryCollectionText(tokenizer);

				default:
					throw new ParseException(String.Format(System.Globalization.CultureInfo.InvariantCulture, "'{0}' is not WKT.", type));
			}
		}

		private Point ReadPointText(WktStreamTokenizer tokenizer)
		{
			string nextToken = this.GetNextEmptyOrOpener(tokenizer);

			if (nextToken == "EMPTY")
			{
				return _factory.createPoint(_factory.getCoordinateSequenceFactory().create(new Coordinate[] {}));
			}
			
			Coordinate coord = new Coordinate(GetNextNumber(tokenizer), GetNextNumber(tokenizer));
			_precisionModel.makePrecise(coord);

			this.GetNextCloser(tokenizer);

			return _factory.createPoint(coord);
		}

		private LineString ReadLineStringText(WktStreamTokenizer tokenizer) 
		{
			return _factory.createLineString(GetCoordinates(tokenizer));
		}

		private LinearRing ReadLinearRingText(WktStreamTokenizer tokenizer)
		{
			return _factory.createLinearRing(GetCoordinates(tokenizer));
		}

		private MultiPoint ReadMultiPointText(WktStreamTokenizer tokenizer)
		{
			return _factory.createMultiPoint(GetCoordinates(tokenizer));
		}

		private Polygon ReadPolygonText(WktStreamTokenizer tokenizer)
		{
			string nextToken = GetNextEmptyOrOpener(tokenizer);

			if (nextToken == "EMPTY")
			{
				return new Polygon(new LinearRing(_factory.getCoordinateSequenceFactory().create(new Coordinate[] {}), _factory), new LinearRing[] {}, _factory);
			}

			LinearRing shell = ReadLinearRingText(tokenizer);

			ArrayList holes = new ArrayList();
			nextToken = this.GetNextCloserOrComma(tokenizer);

			while (nextToken == ",")
			{
				holes.Add(ReadLinearRingText(tokenizer));
				nextToken = this.GetNextCloserOrComma(tokenizer);
			}

			return _factory.createPolygon(shell, (LinearRing[])holes.ToArray(typeof(LinearRing)));
		}

		private MultiLineString ReadMultiLineStringText(WktStreamTokenizer tokenizer) 
		{			
			string nextToken = this.GetNextEmptyOrOpener(tokenizer);
		
			if (nextToken == "EMPTY")
			{
				return new MultiLineString(new LineString[] {}, _factory);
			}

			ArrayList lineStrings = new ArrayList();

			lineStrings.Add(ReadLineStringText(tokenizer));
			nextToken = this.GetNextCloserOrComma(tokenizer);
			
			while (nextToken == ",")
			{
				lineStrings.Add(ReadLineStringText(tokenizer));
				nextToken = this.GetNextCloserOrComma(tokenizer);
			}

			return _factory.createMultiLineString((LineString[])lineStrings.ToArray(typeof(LineString)));
		}

		private MultiPolygon ReadMultiPolygonText(WktStreamTokenizer tokenizer) 
		{			
			string nextToken = this.GetNextEmptyOrOpener(tokenizer);

			if (nextToken == "EMPTY")
			{
				return _factory.createMultiPolygon(new Polygon[] {});
			}

			ArrayList polygons = new ArrayList();

			polygons.Add(ReadPolygonText(tokenizer));
			nextToken = this.GetNextCloserOrComma(tokenizer);

			while (nextToken == ",") 
			{
				polygons.Add(ReadPolygonText(tokenizer));
				nextToken = this.GetNextCloserOrComma(tokenizer);
			}

			return _factory.createMultiPolygon((Polygon[])polygons.ToArray(typeof(Polygon)));
		}

		private GeometryCollection ReadGeometryCollectionText(WktStreamTokenizer tokenizer) 
		{
			string nextToken = this.GetNextEmptyOrOpener(tokenizer);

			if (nextToken == "EMPTY")
			{
				return _factory.createGeometryCollection(new Geometry[] {});
			}

			ArrayList geometries = new ArrayList();

			geometries.Add(ReadGeometryTaggedText(tokenizer));
			nextToken = this.GetNextCloserOrComma(tokenizer);

			while (nextToken == ",")
			{
				geometries.Add(ReadGeometryTaggedText(tokenizer));
				nextToken = this.GetNextCloserOrComma(tokenizer);
			}

			return _factory.createGeometryCollection((Geometry[])geometries.ToArray(typeof(Geometry)));
		}
	}
}
