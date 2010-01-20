using System;
using System.Text;
using System.Data;
using System.Collections;
using System.IO;
using Geotools.IO;
using com.vividsolutions.jts.geom;

namespace Geotools.IO
{
	public class QueryableShapefileReader
	{
		private com.vividsolutions.jts.index.strtree.STRtree _spatialIndex;
		private DbaseFileHeader _dbfHeader;
		private long _dbfHeaderOffset = 0;
		private string _path;
		private ShapeType _type;
		private GeometryFactory _factory;
		private DataTable _table = null;
		private Envelope _extents;

		public QueryableShapefileReader(string path)
			: this(path, new GeometryFactory())
		{
		}

		public QueryableShapefileReader(string path, GeometryFactory factory)
		{
			_path = path;
			_factory = factory;

			this.Initialize();
		}

		private void Initialize()
		{
			// Cache the .dbf header
			_dbfHeader = new DbaseFileHeader();

			using (BinaryReader dbfReader = new BinaryReader(File.OpenRead(Path.Combine(Path.GetDirectoryName(_path), Path.GetFileNameWithoutExtension(_path) + ".dbf"))))
			{
				_dbfHeader.ReadHeader(dbfReader);
				_dbfHeaderOffset = dbfReader.BaseStream.Position;
			}

			// Need to make one pass over the geometries and pull out the bounding boxes
			_spatialIndex = new com.vividsolutions.jts.index.strtree.STRtree();
			_extents = new Envelope();

			using (BigEndianBinaryReader shpReader = new BigEndianBinaryReader(File.OpenRead(_path)))
			using (ShapefileIndexReader shxReader = new ShapefileIndexReader(Path.Combine(Path.GetDirectoryName(_path), Path.GetFileNameWithoutExtension(_path) + ".shx")))
			{
				// Get the shape type
				_type = new ShapefileHeader(shpReader).ShapeType;

				while (shxReader.Read())
				{
					int offset = shxReader.GetOffest();
					int length = shxReader.GetLength();

					// Move to the start of geometry
					shpReader.BaseStream.Position = offset * 2;

					double xMin; 
					double yMin;
					double xMax;
					double yMax;

					int recordNumber = shpReader.ReadIntBE();
					int contentLength = shpReader.ReadIntBE();

					// Read shape type
					int type = shpReader.ReadInt32();

					if (type != 1)
					{
						xMin = shpReader.ReadDouble();
						yMin = shpReader.ReadDouble();
						xMax = shpReader.ReadDouble();
						yMax = shpReader.ReadDouble();		
					}
					else
					{
						// Point - read x and y
						xMin = shpReader.ReadDouble();
						yMin = shpReader.ReadDouble();
						xMax = yMin;
						yMax = yMin;
					}

					// Build the envelope
					Envelope extents = new Envelope(xMin, xMax, yMin, yMax);
				
					// Add to total extents
					_extents.expandToInclude(extents);

					// Insert the index of the record into the spatial index
					_spatialIndex.insert(extents, new ShapefileRecordPointer(recordNumber, offset * 2, contentLength, (int)_dbfHeaderOffset + (_dbfHeader.RecordLength * (recordNumber - 1))));
				}

				// Build the index once
				_spatialIndex.build();
			}
		}

		public Envelope GetExtents()
		{
			return _extents;
		}

		public DataTable Query(Envelope extents)
		{
			DataTable table = this.GetDataTable();
			ArrayList indexes = new ArrayList(_spatialIndex.query(extents).toArray());

			// Sort the results so we can go through the files sequentially
			indexes.Sort();
			ShapeHandler handler = null;

			switch (_type)
			{
				case ShapeType.Point:
					handler = new PointHandler();
					break;
				case ShapeType.Arc:
					handler = new MultiLineHandler();
					break;
				case ShapeType.Polygon:
					handler = new PolygonHandler();
					break;
			}

			using (BinaryReader dbfReader = new BinaryReader(File.OpenRead(Path.Combine(Path.GetDirectoryName(_path), Path.GetFileNameWithoutExtension(_path) + ".dbf"))))
			using (BigEndianBinaryReader shpReader = new BigEndianBinaryReader(File.OpenRead(_path)))
			{
				foreach (ShapefileRecordPointer pointer in indexes)
				{
					ArrayList record = new ArrayList();

					// Step 1: Get the geometry
					// NOTE: We add 8 here to skip the content length and record numer - we
					//		 already have that information in the pointer object.
					shpReader.BaseStream.Position = pointer.GeometryOffset + 8;
					record.Add(handler.Read(shpReader, _factory));

					// Step 2: Get the attributes
					dbfReader.BaseStream.Position = pointer.AttributesOffset;
					record.AddRange(DbaseFileReader.ReadRecord(dbfReader, _dbfHeader));
				
					table.Rows.Add(record.ToArray());
				}
			}

			return table;
		}

		private DataTable GetDataTable()
		{
			if (_table == null)
			{
				// Create the table
				_table = new DataTable();
			
				// Add the geometry column
				_table.Columns.Add(new DataColumn("Geometry", typeof(Geometry)));

				for (int i = 0; i < _dbfHeader.Fields.Length; i++)
				{
					_table.Columns.Add(new DataColumn(_dbfHeader.Fields[i].Name, _dbfHeader.Fields[i].Type));
				}
			}

			return _table.Clone();
		}
	}
}
