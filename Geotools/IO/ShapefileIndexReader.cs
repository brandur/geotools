using System;
using System.IO;

namespace Geotools.IO
{
	public class ShapefileIndexReader : IDisposable
	{
		private BigEndianBinaryReader _reader;
		private ShapefileHeader _header;
		private bool _isClosed;
		private int _currentOffset;
		private int _currentLength;

		protected bool disposed = false;

		public ShapefileIndexReader(string path)
		{
			_reader = new BigEndianBinaryReader(File.OpenRead(path));
			_header = new ShapefileHeader(_reader);
		}

		public bool Read()
		{
			if (_reader.PeekChar() != -1)
			{
				_currentOffset = _reader.ReadIntBE();
				_currentLength = _reader.ReadIntBE();

				return true;
			}	
			else
			{
				return false;
			}
		}

		public int GetOffest()
		{
			return _currentOffset;
		}

		public int GetLength()
		{
			return _currentLength;
		}

		public void Close()
		{
			if (!_isClosed)
			{
				_reader.Close();
				_isClosed = true;
			}
		}
		
		public bool IsClosed
		{
			get
			{
				return _isClosed;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (!disposed)
				{
					this.Close();
					((IDisposable)_reader).Dispose();
					this.disposed = true;
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
