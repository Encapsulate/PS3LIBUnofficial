using System;
using System.Text;
namespace PS3Lib
{
	public class ArrayBuilder
	{
		public class ArrayReader
		{
			private byte[] buffer;
			private int size;
			public ArrayReader(byte[] BytesArray)
			{
				this.buffer = BytesArray;
				this.size = this.buffer.Length;
			}
			public bool GetBool(int pos)
			{
				return this.buffer[pos] != 0;
			}
			public byte GetByte(int pos)
			{
				return this.buffer[pos];
			}
			public byte[] GetBytes(int pos, int length)
			{
				byte[] array = new byte[length];
				for (int i = 0; i < length; i++)
				{
					array[i] = this.buffer[pos + i];
				}
				return array;
			}
			public char GetChar(int pos)
			{
				return this.buffer[pos].ToString()[0];
			}
			public float GetFloat(int pos)
			{
				byte[] array = new byte[4];
				for (int i = 0; i < 4; i++)
				{
					array[i] = this.buffer[pos + i];
				}
				Array.Reverse(array, 0, 4);
				return BitConverter.ToSingle(array, 0);
			}
			public short GetInt16(int pos, EndianType Type = EndianType.LittleEndian)
			{
				byte[] array = new byte[2];
				for (int i = 0; i < 2; i++)
				{
					array[i] = this.buffer[pos + i];
				}
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(array, 0, 2);
				}
				return BitConverter.ToInt16(array, 0);
			}
			public int GetInt32(int pos, EndianType Type = EndianType.LittleEndian)
			{
				byte[] array = new byte[4];
				for (int i = 0; i < 4; i++)
				{
					array[i] = this.buffer[pos + i];
				}
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(array, 0, 4);
				}
				return BitConverter.ToInt32(array, 0);
			}
			public long GetInt64(int pos, EndianType Type = EndianType.LittleEndian)
			{
				byte[] array = new byte[8];
				for (int i = 0; i < 8; i++)
				{
					array[i] = this.buffer[pos + i];
				}
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(array, 0, 8);
				}
				return BitConverter.ToInt64(array, 0);
			}
			private sbyte GetSByte(int pos)
			{
				return (sbyte)this.buffer[pos];
			}
			public string GetString(int pos)
			{
				int num = 0;
				while (this.buffer[pos + num] != 0)
				{
					num++;
				}
				byte[] array = new byte[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.buffer[pos + i];
				}
				return Encoding.UTF8.GetString(array);
			}
			public ushort GetUInt16(int pos, EndianType Type = EndianType.LittleEndian)
			{
				byte[] array = new byte[2];
				for (int i = 0; i < 2; i++)
				{
					array[i] = this.buffer[pos + i];
				}
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(array, 0, 2);
				}
				return BitConverter.ToUInt16(array, 0);
			}
			public uint GetUInt32(int pos, EndianType Type = EndianType.LittleEndian)
			{
				byte[] array = new byte[4];
				for (int i = 0; i < 4; i++)
				{
					array[i] = this.buffer[pos + i];
				}
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(array, 0, 4);
				}
				return BitConverter.ToUInt32(array, 0);
			}
			public ulong GetUInt64(int pos, EndianType Type = EndianType.LittleEndian)
			{
				byte[] array = new byte[8];
				for (int i = 0; i < 8; i++)
				{
					array[i] = this.buffer[pos + i];
				}
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(array, 0, 8);
				}
				return BitConverter.ToUInt64(array, 0);
			}
		}
		public class ArrayWriter
		{
			private byte[] buffer;
			private int size;
			public ArrayWriter(byte[] BytesArray)
			{
				this.buffer = BytesArray;
				this.size = this.buffer.Length;
			}
			public void SetBool(int pos, bool value)
			{
				byte[] array = new byte[0];
				this.buffer[pos] = array[0];
			}
			public void SetByte(int pos, byte value)
			{
				this.buffer[pos] = value;
			}
			public void SetBytes(int pos, byte[] value)
			{
				int num = value.Length;
				for (int i = 0; i < num; i++)
				{
					this.buffer[i + pos] = value[i];
				}
			}
			public void SetChar(int pos, char value)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(value.ToString());
				this.buffer[pos] = bytes[0];
			}
			public void SetFloat(int pos, float value)
			{
				byte[] bytes = BitConverter.GetBytes(value);
				Array.Reverse(bytes, 0, 4);
				for (int i = 0; i < 4; i++)
				{
					this.buffer[i + pos] = bytes[i];
				}
			}
			public void SetInt16(int pos, short value, EndianType Type = EndianType.LittleEndian)
			{
				byte[] bytes = BitConverter.GetBytes(value);
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(bytes, 0, 2);
				}
				for (int i = 0; i < 2; i++)
				{
					this.buffer[i + pos] = bytes[i];
				}
			}
			public void SetInt32(int pos, int value, EndianType Type = EndianType.LittleEndian)
			{
				byte[] bytes = BitConverter.GetBytes(value);
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(bytes, 0, 4);
				}
				for (int i = 0; i < 4; i++)
				{
					this.buffer[i + pos] = bytes[i];
				}
			}
			public void SetInt64(int pos, long value, EndianType Type = EndianType.LittleEndian)
			{
				byte[] bytes = BitConverter.GetBytes(value);
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(bytes, 0, 8);
				}
				for (int i = 0; i < 8; i++)
				{
					this.buffer[i + pos] = bytes[i];
				}
			}
			public void SetSByte(int pos, sbyte value)
			{
				this.buffer[pos] = (byte)value;
			}
			public void SetString(int pos, string value)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(value + "\0");
				for (int i = 0; i < bytes.Length; i++)
				{
					this.buffer[i + pos] = bytes[i];
				}
			}
			public void SetUInt16(int pos, ushort value, EndianType Type = EndianType.LittleEndian)
			{
				byte[] bytes = BitConverter.GetBytes(value);
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(bytes, 0, 2);
				}
				for (int i = 0; i < 2; i++)
				{
					this.buffer[i + pos] = bytes[i];
				}
			}
			public void SetUInt32(int pos, uint value, EndianType Type = EndianType.LittleEndian)
			{
				byte[] bytes = BitConverter.GetBytes(value);
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(bytes, 0, 4);
				}
				for (int i = 0; i < 4; i++)
				{
					this.buffer[i + pos] = bytes[i];
				}
			}
			public void SetUInt64(int pos, ulong value, EndianType Type = EndianType.LittleEndian)
			{
				byte[] bytes = BitConverter.GetBytes(value);
				if (Type == EndianType.BigEndian)
				{
					Array.Reverse(bytes, 0, 8);
				}
				for (int i = 0; i < 8; i++)
				{
					this.buffer[i + pos] = bytes[i];
				}
			}
		}
		private byte[] buffer;
		private int size;
		public ArrayBuilder.ArrayReader Read
		{
			get
			{
				return new ArrayBuilder.ArrayReader(this.buffer);
			}
		}
		public ArrayBuilder.ArrayWriter Write
		{
			get
			{
				return new ArrayBuilder.ArrayWriter(this.buffer);
			}
		}
		public ArrayBuilder(byte[] BytesArray)
		{
			this.buffer = BytesArray;
			this.size = this.buffer.Length;
		}
	}
}
