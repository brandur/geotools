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

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
//using Geotools.Algorithms;
//using Geotools.Geometries;
using com.vividsolutions.jts.geom;
using com.vividsolutions.jts.algorithm;
using com.vividsolutions.jts.geom.impl;

namespace Geotools.IO
{
	/// <summary>
	/// Converts a Shapefile point to a OGIS Polygon.
	/// </summary>
	public class PolygonHandler : ShapeHandler
	{
		protected static CGAlgorithms _cga = new RobustCGAlgorithms();

		/// <summary>
		/// Initializes a new instance of the <see cref="PolygonHandler">PolygonHandler</see> class.
		/// </summary>
		public PolygonHandler()
		{
		}

		/// <summary>
		/// Gets the <see cref="ShapeType">ShapeType</see> this handler handles.
		/// </summary>
		public override ShapeType ShapeType
		{
			get
			{
				return ShapeType.Polygon;
			}
		}

		/// <summary>
		/// Reads a stream and converts the shapefile record to an equilivent geometry object.
		/// </summary>
		/// <param name="reader">The stream to read.</param>
		/// <param name="factory">The geometry factory to use when making the object.</param>
		/// <returns>The Geometry object that represents the shape file record.</returns>

		public override Geometry Read(BigEndianBinaryReader reader, GeometryFactory factory)
		{
			if (this.GetShapeType(reader) != ShapeType.Polygon)
			{
				throw new ShapefileException("Attempting to load a non-polygon as polygon.");
			}

			// Read and for now ignore bounds.
			double[] box = new double[4];

			for (int i = 0; i < box.Length; i++) 
			{
				box[i] = reader.ReadDouble();
			}

			int numParts = reader.ReadInt32();
			int numPoints = reader.ReadInt32();
			int[] partOffsets = new int[numParts];

			for (int i = 0; i < numParts; i++)
			{
				partOffsets[i] = reader.ReadInt32();
			}

			ArrayList shells = new ArrayList();
			ArrayList holes = new ArrayList();
			int start, finish, length;

			for (int part = 0; part < numParts; part++)
			{
				start = partOffsets[part];

				if (part == numParts - 1)
				{
					finish = numPoints;
				}
				else 
				{
					finish = partOffsets[part + 1];
				}

				length = finish - start;
				Coordinate[] coords = new Coordinate[length];

				for (int i = 0; i < length; i++)
				{
					Coordinate coord = new Coordinate(reader.ReadDouble(), reader.ReadDouble());
					factory.getPrecisionModel().makePrecise(coord);
	
					coords[i] = coord;
				}

				LinearRing ring = factory.createLinearRing(new PackedCoordinateSequence.Float(coords, 2));

				if (com.vividsolutions.jts.algorithm.RobustCGAlgorithms.isCCW(coords))
				{
					holes.Add(ring);
				}
				else 
				{
					shells.Add(ring);
				}
			}

			// Now we have a list of all shells and all holes
			ArrayList holesForShells = new ArrayList(shells.Count);
			
			for (int i = 0; i < shells.Count; i++)
			{
				holesForShells.Add(new ArrayList());
			}

			//find homes
			for (int i = 0; i < holes.Count; i++)
			{
				LinearRing testRing = (LinearRing) holes[i];
				LinearRing minShell = null;
				Envelope minEnv = null;
				Envelope testEnv = testRing.getEnvelopeInternal();
				Coordinate testPt = testRing.getCoordinateN(0);
				LinearRing tryRing;
				for (int j = 0; j < shells.Count; j++)
				{
					tryRing = (LinearRing) shells[j];
					Envelope tryEnv = tryRing.getEnvelopeInternal();
					if (minShell != null) 
					{
						minEnv = minShell.getEnvelopeInternal();
					}
					bool isContained = false;

					Coordinate[] coordList = tryRing.getCoordinates();
					
					if (tryEnv.contains(testEnv)
						&& (com.vividsolutions.jts.algorithm.RobustCGAlgorithms.isPointInRing(testPt, coordList ) ||
						(PointInList(testPt,coordList)))) 
					{
						isContained = true;
					}
					// check if this new containing ring is smaller than the
					// current minimum ring
					if (isContained) 
					{
						if (minShell == null
							|| minEnv.contains(tryEnv)) 
						{
							minShell = tryRing;
						}
						ArrayList list = (ArrayList)holesForShells[j];
						list.Add(testRing);
					}
				}
				if (minShell==null)
				{
//					throw new InvalidOperationException("Could not find shell for a hole. Try a different precision model.");
				}
			}
			Polygon[] polygons = new Polygon[shells.Count];
			for (int i = 0; i < shells.Count; i++)
			{
				polygons[i] = factory.createPolygon((LinearRing) shells[i], (LinearRing[])((ArrayList) holesForShells[i]).ToArray(typeof(LinearRing)));
			}
        
			if (polygons.Length == 1)
			{
				return polygons[0];
			}
			//it's a multi part
			return factory.createMultiPolygon(polygons);

		}

		/// <summary>
		/// Writes a <b>Geometry</b> to the given binary wirter.
		/// </summary>
		/// <param name="geometry">The geometry to write.</param>
		/// <param name="writer">The file stream to write to.</param>
		/// <param name="factory">The geometry factory to use.</param>
		public override void Write(Geometry geometry, BinaryWriter writer, GeometryFactory factory)
		{
			/* This makes exporting really slow for complex polygons
			 * if (geometry.isValid() == false)
			{
				Trace.WriteLine("Invalid polygon being written.");
			}*/

			GeometryCollection multi;
			if(geometry is GeometryCollection)
			{
				multi = (GeometryCollection) geometry;
			}
			else 
			{
				GeometryFactory gf = new GeometryFactory(geometry.getPrecisionModel());
				//multi = new MultiPolygon(new Polygon[]{(Polygon) geometry}, geometry.PrecisionModel, geometry.GetSRID());
				multi = gf.createMultiPolygon( new Polygon[]{(Polygon) geometry} );
			}
			//file.setLittleEndianMode(true);
			writer.Write(int.Parse(Enum.Format(typeof(ShapeType),this.ShapeType,"d")));
        
			Envelope box = multi.getEnvelopeInternal();
			Envelope bounds = ShapeHandler.GetEnvelopeExternal(factory.getPrecisionModel(), box);
			writer.Write(bounds.getMinX());
			writer.Write(bounds.getMinY());
			writer.Write(bounds.getMaxX());
			writer.Write(bounds.getMaxY());
        
			int numParts = GetNumParts(multi);
			int numPoints = multi.getNumPoints();
			writer.Write(numParts);
			writer.Write(numPoints);
        
			
			// write the offsets to the points
			int offset=0;
			for (int part = 0; part < multi.getNumGeometries(); part++)
			{
				// offset to the shell points
				Polygon polygon = (Polygon)multi.getGeometryN(part);
				writer.Write(offset);
				offset = offset + polygon.getExteriorRing().getNumPoints();
				// offstes to the holes

				for (int i = 0; i < polygon.getNumInteriorRing(); i++)
				{
					writer.Write(offset);
					offset = offset + polygon.getInteriorRingN(i).getNumPoints();
				}	
			}


			// write the points 
			for (int part = 0; part < multi.getNumGeometries(); part++)
			{
				Polygon poly = (Polygon)multi.getGeometryN(part);
				Coordinate[] coords = poly.getExteriorRing().getCoordinates();
				if (com.vividsolutions.jts.algorithm.RobustCGAlgorithms.isCCW(coords)==true)
				{
					Array.Reverse(coords);
					//coords = coords.ReverseCoordinateOrder();
				}
				WriteCoords(coords, writer, factory);

				for (int i = 0; i < poly.getNumInteriorRing(); i++)
				{
					Coordinate[] coords2 = poly.getInteriorRingN(i).getCoordinates();
					if (com.vividsolutions.jts.algorithm.RobustCGAlgorithms.isCCW(coords2)==false)
					{
						Array.Reverse(coords2);
						//coords = coords.ReverseCoordinateOrder();
					}
					WriteCoords(coords2, writer, factory);
				}
			}
		}

		public void WriteCoords(Coordinate[] coords, BinaryWriter writer, GeometryFactory factory)
		{
			foreach (Coordinate coord in coords)
			{
				writer.Write(coord.x);
				writer.Write(coord.y);
			}
		}

		/// <summary>
		/// Gets the length of the shapefile record using the geometry passed in.
		/// </summary>
		/// <param name="geometry">The geometry to get the length for.</param>
		/// <returns>The length in bytes this geometry is going to use when written out as a shapefile record.</returns>
		public override int GetLength(Geometry geometry)
		{
			return (22 + (2 * GetNumParts(geometry)) + geometry.getNumPoints() * 8);
		}
		
		private int GetNumParts(Geometry geometry)
		{
			int numParts = 0;

			if (geometry is MultiPolygon)
			{
				numParts = 0;
				MultiPolygon multiPolygon = (MultiPolygon)geometry;
				for(int i=0; i<multiPolygon.getNumGeometries(); i++)
				{
					numParts = numParts + GetNumParts( multiPolygon.getGeometryN(i) );
				}
			}
			else if (geometry is Polygon)
			{
				numParts = ((Polygon)geometry).getNumInteriorRing() + 1;
			}
			else
			{
				throw new InvalidOperationException("Should not get here.");
			}

			return numParts;
		}

		private bool PointInList(Coordinate coord, Coordinate[] coords) 
		{
			foreach (Coordinate c in coords)
			{
				if (c.equals2D(coord))
				{
					return true;
				}
			}
			return false;
		}
	}
}
