using System;
using System.Linq;
using System.Text;
namespace PS3Lib
{
	public class Extension
	{
		private class Common
		{
			public static CCAPI CcApi;
			public static TMAPI TmApi;
		}
		private SelectAPI CurrentAPI;
		public Extension(SelectAPI API)
		{
			this.CurrentAPI = API;
			if (API == SelectAPI.TargetManager && Extension.Common.TmApi == null)
			{
				Extension.Common.TmApi = new TMAPI();
			}
			if (API == SelectAPI.ControlConsole && Extension.Common.CcApi == null)
			{
				Extension.Common.CcApi = new CCAPI();
			}
		}
		private byte[] GetBytes(uint offset, uint length, SelectAPI API)
		{
			byte[] array = new byte[length];
			byte[] result;
			if (API == SelectAPI.ControlConsole)
			{
				result = Extension.Common.CcApi.GetBytes(offset, length);
			}
			else
			{
				if (API == SelectAPI.TargetManager)
				{
					array = Extension.Common.TmApi.GetBytes(offset, length);
				}
				result = array;
			}
			return result;
		}
		private void GetMem(uint offset, byte[] buffer, SelectAPI API)
		{
			if (API == SelectAPI.ControlConsole)
			{
				Extension.Common.CcApi.GetMemory(offset, buffer);
			}
			else
			{
				if (API == SelectAPI.TargetManager)
				{
					Extension.Common.TmApi.GetMemory(offset, buffer);
				}
			}
		}
		public bool ReadBool(uint offset)
		{
			byte[] array = new byte[1];
			this.GetMem(offset, array, this.CurrentAPI);
			return array[0] != 0;
		}
		public byte ReadByte(uint offset)
		{
			return this.GetBytes(offset, 1u, this.CurrentAPI)[0];
		}
		public byte[] ReadBytes(uint offset, int length)
		{
			return this.GetBytes(offset, (uint)length, this.CurrentAPI);
		}
		public float ReadFloat(uint offset)
		{
			byte[] bytes = this.GetBytes(offset, 4u, this.CurrentAPI);
			Array.Reverse(bytes, 0, 4);
			return BitConverter.ToSingle(bytes, 0);
		}
		public short ReadInt16(uint offset)
		{
			byte[] bytes = this.GetBytes(offset, 2u, this.CurrentAPI);
			Array.Reverse(bytes, 0, 2);
			return BitConverter.ToInt16(bytes, 0);
		}
		public int ReadInt32(uint offset)
		{
			byte[] bytes = this.GetBytes(offset, 4u, this.CurrentAPI);
			Array.Reverse(bytes, 0, 4);
			return BitConverter.ToInt32(bytes, 0);
		}
		public long ReadInt64(uint offset)
		{
			byte[] bytes = this.GetBytes(offset, 8u, this.CurrentAPI);
			Array.Reverse(bytes, 0, 8);
			return BitConverter.ToInt64(bytes, 0);
		}
		public sbyte ReadSByte(uint offset)
		{
			byte[] array = new byte[1];
			this.GetMem(offset, array, this.CurrentAPI);
			return (sbyte)array[0];
		}
		public string ReadString(uint offset)
		{
			int num = 40;
			int num2 = 0;
			string text = "";
			do
			{
				byte[] bytes = this.ReadBytes(offset + (uint)num2, num);
				text += Encoding.UTF8.GetString(bytes);
				num2 += num;
			}
			while (!text.Contains('\0'));
			int length = text.IndexOf('\0');
			string result = text.Substring(0, length);
			text = string.Empty;
			return result;
		}
		public ushort ReadUInt16(uint offset)
		{
			byte[] bytes = this.GetBytes(offset, 2u, this.CurrentAPI);
			Array.Reverse(bytes, 0, 2);
			return BitConverter.ToUInt16(bytes, 0);
		}
		public uint ReadUInt32(uint offset)
		{
			byte[] bytes = this.GetBytes(offset, 4u, this.CurrentAPI);
			Array.Reverse(bytes, 0, 4);
			return BitConverter.ToUInt32(bytes, 0);
		}
		public ulong ReadUInt64(uint offset)
		{
			byte[] bytes = this.GetBytes(offset, 8u, this.CurrentAPI);
			Array.Reverse(bytes, 0, 8);
			return BitConverter.ToUInt64(bytes, 0);
		}
		private void SetMem(uint Address, byte[] buffer, SelectAPI API)
		{
			if (API == SelectAPI.ControlConsole)
			{
				Extension.Common.CcApi.SetMemory(Address, buffer);
			}
			else
			{
				if (API == SelectAPI.TargetManager)
				{
					Extension.Common.TmApi.SetMemory(Address, buffer);
				}
			}
		}
		public void WriteBool(uint offset, bool input)
		{
			byte[] buffer = new byte[0];
			this.SetMem(offset, buffer, this.CurrentAPI);
		}
		public void WriteByte(uint offset, byte input)
		{
			byte[] buffer = new byte[]
			{
				input
			};
			this.SetMem(offset, buffer, this.CurrentAPI);
		}
		public void WriteBytes(uint offset, byte[] input)
		{
			this.SetMem(offset, input, this.CurrentAPI);
		}
		public void WriteFloat(uint offset, float input)
		{
			byte[] array = new byte[4];
			BitConverter.GetBytes(input).CopyTo(array, 0);
			Array.Reverse(array, 0, 4);
			this.SetMem(offset, array, this.CurrentAPI);
		}
		public void WriteInt16(uint offset, short input)
		{
			byte[] array = new byte[2];
			BitConverter.GetBytes(input).CopyTo(array, 0);
			Array.Reverse(array, 0, 2);
			this.SetMem(offset, array, this.CurrentAPI);
		}
		public void WriteInt32(uint offset, int input)
		{
			byte[] array = new byte[4];
			BitConverter.GetBytes(input).CopyTo(array, 0);
			Array.Reverse(array, 0, 4);
			this.SetMem(offset, array, this.CurrentAPI);
		}
		public void WriteInt64(uint offset, long input)
		{
			byte[] array = new byte[8];
			BitConverter.GetBytes(input).CopyTo(array, 0);
			Array.Reverse(array, 0, 8);
			this.SetMem(offset, array, this.CurrentAPI);
		}
		public void WriteSByte(uint offset, sbyte input)
		{
			byte[] buffer = new byte[]
			{
				(byte)input
			};
			this.SetMem(offset, buffer, this.CurrentAPI);
		}
		public void WriteString(uint offset, string input)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			Array.Resize<byte>(ref bytes, bytes.Length + 1);
			this.SetMem(offset, bytes, this.CurrentAPI);
		}
		public void WriteUInt16(uint offset, ushort input)
		{
			byte[] array = new byte[2];
			BitConverter.GetBytes(input).CopyTo(array, 0);
			Array.Reverse(array, 0, 2);
			this.SetMem(offset, array, this.CurrentAPI);
		}
		public void WriteUInt32(uint offset, uint input)
		{
			byte[] array = new byte[4];
			BitConverter.GetBytes(input).CopyTo(array, 0);
			Array.Reverse(array, 0, 4);
			this.SetMem(offset, array, this.CurrentAPI);
		}
		public void WriteUInt64(uint offset, ulong input)
		{
			byte[] array = new byte[8];
			BitConverter.GetBytes(input).CopyTo(array, 0);
			Array.Reverse(array, 0, 8);
			this.SetMem(offset, array, this.CurrentAPI);
		}
	}
}
