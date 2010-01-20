using System;

namespace Geotools.IO
{
	public struct ShapefileRecordPointer : IComparable
	{
		private int _recordNumber;
		private int _geometryOffset;
		private int _geometryLength;
		private int _attributesOffset;
	
		public ShapefileRecordPointer(int recordNumber, int geometryOffset, int geometryLength, int attributesOffset)
		{
			_recordNumber = recordNumber;
			_geometryOffset = geometryOffset;
			_geometryLength = geometryLength;
			_attributesOffset = attributesOffset;
		}

		public int RecordNumber
		{
			get
			{
				return _recordNumber;
			}
			set
			{
				_recordNumber = value;
			}
		}

		public int GeometryOffset
		{
			get
			{
				return _geometryOffset;
			}
			set
			{
				_geometryOffset = value;
			}
		}

		public int GeometryLength
		{
			get
			{
				return _geometryLength;
			}
			set
			{
				_geometryLength = value;
			}
		}

		public int AttributesOffset
		{
			get
			{
				return _attributesOffset;
			}
			set
			{
				_attributesOffset = value;
			}
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj is ShapefileRecordPointer)
			{
				if (this.RecordNumber < ((ShapefileRecordPointer)obj).RecordNumber)
				{
					return -1;
				}
				else if (this.RecordNumber > ((ShapefileRecordPointer)obj).RecordNumber)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
			else
			{
				throw new ArgumentException("obj is not of type ShapeFileRecordPointer and is not a null reference (Nothing in Visual Basic).", "obj");
			}
		}

		#endregion
	}
}
