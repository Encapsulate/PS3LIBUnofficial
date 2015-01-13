using PS3Lib.NET;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
namespace PS3Lib
{
	public class TMAPI
	{
		public class Parameters
		{
			public static PS3TMAPI.ConnectStatus connectStatus;
			public static string ConsoleName;
			public static string info;
			public static string MemStatus;
			public static uint ProcessID;
			public static uint[] processIDs;
			public static byte[] Retour;
			public static string snresult;
			public static string Status;
			public static string usage;
		}
		public enum ResetTarget
		{
			Hard,
			Quick,
			ResetEx,
			Soft
		}
		public class SCECMD
		{
			public PS3TMAPI.ConnectStatus DetailStatus()
			{
				return TMAPI.Parameters.connectStatus;
			}
			public string GetStatus()
			{
				string result;
				if (TMAPI.AssemblyLoaded)
				{
					result = "NotConnected";
				}
				else
				{
					TMAPI.Parameters.connectStatus = PS3TMAPI.ConnectStatus.Connected;
					PS3TMAPI.GetConnectStatus(TMAPI.Target, out TMAPI.Parameters.connectStatus, out TMAPI.Parameters.usage);
					TMAPI.Parameters.Status = TMAPI.Parameters.connectStatus.ToString();
					result = TMAPI.Parameters.Status;
				}
				return result;
			}
			public string GetTargetName()
			{
				if (TMAPI.Parameters.ConsoleName == null || TMAPI.Parameters.ConsoleName == string.Empty)
				{
					PS3TMAPI.InitTargetComms();
					PS3TMAPI.TargetInfo targetInfo = new PS3TMAPI.TargetInfo
					{
						Flags = PS3TMAPI.TargetInfoFlag.TargetID,
						Target = TMAPI.Target
					};
					PS3TMAPI.GetTargetInfo(ref targetInfo);
					TMAPI.Parameters.ConsoleName = targetInfo.Name;
				}
				return TMAPI.Parameters.ConsoleName;
			}
			public uint ProcessID()
			{
				return TMAPI.Parameters.ProcessID;
			}
			public uint[] ProcessIDs()
			{
				return TMAPI.Parameters.processIDs;
			}
			public string SNRESULT()
			{
				return TMAPI.Parameters.snresult;
			}
		}
		public static bool AssemblyLoaded = true;
		internal static Assembly LoadApi;
		public static PS3TMAPI.ResetParameter resetParameter;
		public static int Target = 255;
		public Extension Extension
		{
			get
			{
				return new Extension(SelectAPI.TargetManager);
			}
		}
		public TMAPI.SCECMD SCE
		{
			get
			{
				return new TMAPI.SCECMD();
			}
		}
		public bool AttachProcess()
		{
			PS3TMAPI.GetProcessList(TMAPI.Target, out TMAPI.Parameters.processIDs);
			bool flag = TMAPI.Parameters.processIDs.Length > 0;
			if (flag)
			{
				ulong value = (ulong)TMAPI.Parameters.processIDs[0];
				TMAPI.Parameters.ProcessID = Convert.ToUInt32(value);
				PS3TMAPI.ProcessAttach(TMAPI.Target, PS3TMAPI.UnitType.PPU, TMAPI.Parameters.ProcessID);
				PS3TMAPI.ProcessContinue(TMAPI.Target, TMAPI.Parameters.ProcessID);
				TMAPI.Parameters.info = "The Process 0x" + TMAPI.Parameters.ProcessID.ToString("X8") + " Has Been Attached !";
			}
			return flag;
		}
		public bool ConnectTarget(int TargetIndex = 0)
		{
			if (TMAPI.AssemblyLoaded)
			{
				this.PS3TMAPI_NET();
			}
			TMAPI.AssemblyLoaded = false;
			TMAPI.Target = TargetIndex;
			bool flag = PS3TMAPI.SUCCEEDED(PS3TMAPI.InitTargetComms());
			return PS3TMAPI.SUCCEEDED(PS3TMAPI.Connect(TargetIndex, null));
		}
		public bool ConnectTarget(string TargetName)
		{
			if (TMAPI.AssemblyLoaded)
			{
				this.PS3TMAPI_NET();
			}
			TMAPI.AssemblyLoaded = false;
			bool flag = PS3TMAPI.SUCCEEDED(PS3TMAPI.InitTargetComms());
			if (flag)
			{
				flag = PS3TMAPI.SUCCEEDED(PS3TMAPI.GetTargetFromName(TargetName, out TMAPI.Target));
				flag = PS3TMAPI.SUCCEEDED(PS3TMAPI.Connect(TMAPI.Target, null));
			}
			return flag;
		}
		public void DisconnectTarget()
		{
			PS3TMAPI.Disconnect(TMAPI.Target);
		}
		public byte[] GetBytes(uint Address, uint lengthByte)
		{
			byte[] result = new byte[lengthByte];
			PS3TMAPI.ProcessGetMemory(TMAPI.Target, PS3TMAPI.UnitType.PPU, TMAPI.Parameters.ProcessID, 0uL, (ulong)Address, ref result);
			return result;
		}
		public void GetMemory(uint Address, byte[] Bytes)
		{
			PS3TMAPI.ProcessGetMemory(TMAPI.Target, PS3TMAPI.UnitType.PPU, TMAPI.Parameters.ProcessID, 0uL, (ulong)Address, ref Bytes);
		}
		public string GetString(uint Address, uint lengthString)
		{
			byte[] bytes = new byte[lengthString];
			PS3TMAPI.ProcessGetMemory(TMAPI.Target, PS3TMAPI.UnitType.PPU, TMAPI.Parameters.ProcessID, 0uL, (ulong)Address, ref bytes);
			return TMAPI.Hex2Ascii(TMAPI.ReplaceString(bytes));
		}
		internal static string Hex2Ascii(string iMCSxString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i <= iMCSxString.Length - 2; i += 2)
			{
				stringBuilder.Append(Convert.ToString(Convert.ToChar(int.Parse(iMCSxString.Substring(i, 2), NumberStyles.HexNumber))));
			}
			return stringBuilder.ToString();
		}
		public void InitComms()
		{
			PS3TMAPI.InitTargetComms();
		}
		public void PowerOff(bool Force)
		{
			PS3TMAPI.PowerOff(TMAPI.Target, Force);
		}
		public void PowerOn(int numTarget = 0)
		{
			if (TMAPI.Target != 255)
			{
				numTarget = TMAPI.Target;
			}
			PS3TMAPI.PowerOn(numTarget);
		}
		public Assembly PS3TMAPI_NET()
		{
			AppDomain.CurrentDomain.AssemblyResolve += delegate(object s, ResolveEventArgs e)
			{
				string name = new AssemblyName(e.Name).Name;
				string path = string.Format("C:\\Program Files\\SN Systems\\PS3\\bin\\ps3tmapi_net.dll", name);
				string path2 = string.Format("C:\\Program Files (x64)\\SN Systems\\PS3\\bin\\ps3tmapi_net.dll", name);
				string text = string.Format("C:\\Program Files (x86)\\SN Systems\\PS3\\bin\\ps3tmapi_net.dll", name);
				if (File.Exists(path))
				{
					TMAPI.LoadApi = Assembly.LoadFile(path);
				}
				else
				{
					if (File.Exists(path2))
					{
						TMAPI.LoadApi = Assembly.LoadFile(path2);
					}
					else
					{
						if (File.Exists(text))
						{
							TMAPI.LoadApi = Assembly.LoadFile(text);
						}
						else
						{
							MessageBox.Show("Target Manager API cannot be founded to:\r\n\r\n" + text, "Error with PS3 API!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						}
					}
				}
				return TMAPI.LoadApi;
			};
			return TMAPI.LoadApi;
		}
		internal static string ReplaceString(byte[] bytes)
		{
			string text = BitConverter.ToString(bytes).Replace("00", string.Empty).Replace("-", string.Empty);
			for (int i = 0; i < 10; i++)
			{
				text = text.Replace("^" + i.ToString(), string.Empty);
			}
			return text;
		}
		public void ResetToXMB(TMAPI.ResetTarget flag)
		{
			if (flag == TMAPI.ResetTarget.Hard)
			{
				TMAPI.resetParameter = PS3TMAPI.ResetParameter.Hard;
			}
			else
			{
				if (flag == TMAPI.ResetTarget.Quick)
				{
					TMAPI.resetParameter = PS3TMAPI.ResetParameter.Quick;
				}
				else
				{
					if (flag == TMAPI.ResetTarget.ResetEx)
					{
						TMAPI.resetParameter = PS3TMAPI.ResetParameter.ResetEx;
					}
					else
					{
						if (flag == TMAPI.ResetTarget.Soft)
						{
							TMAPI.resetParameter = PS3TMAPI.ResetParameter.Soft;
						}
					}
				}
			}
			PS3TMAPI.Reset(TMAPI.Target, TMAPI.resetParameter);
		}
		public void SetMemory(uint Address, byte[] Bytes)
		{
			PS3TMAPI.ProcessSetMemory(TMAPI.Target, PS3TMAPI.UnitType.PPU, TMAPI.Parameters.ProcessID, 0uL, (ulong)Address, Bytes);
		}
		public void SetMemory(uint Address, ulong value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			PS3TMAPI.ProcessSetMemory(TMAPI.Target, PS3TMAPI.UnitType.PPU, TMAPI.Parameters.ProcessID, 0uL, (ulong)Address, bytes);
		}
		public void SetMemory(uint Address, string hexadecimal, EndianType Type = EndianType.LittleEndian)
		{
			byte[] array = TMAPI.StringToByteArray(hexadecimal);
			if (Type == EndianType.LittleEndian)
			{
				Array.Reverse(array);
			}
			PS3TMAPI.ProcessSetMemory(TMAPI.Target, PS3TMAPI.UnitType.PPU, TMAPI.Parameters.ProcessID, 0uL, (ulong)Address, array);
		}
		internal static byte[] StringToByteArray(string hex)
		{
			Func<int, byte> func = null;
			Func<int, byte> func2 = null;
			string replace = hex.Replace("0x", "");
			string Stringz = replace.Insert(replace.Length - 1, "0");
			bool flag;
			if (replace.Length % 2 == 0)
			{
				flag = true;
			}
			else
			{
				flag = false;
			}
			byte[] array;
			byte[] result;
			byte[] array2;
			try
			{
				if (flag)
				{
					if (func == null)
					{
						func = ((int x) => Convert.ToByte(replace.Substring(x, 2), 16));
					}
					array = (
						from x in Enumerable.Range(0, replace.Length)
						where x % 2 == 0
						select x).Select(func).ToArray<byte>();
					result = array;
					return result;
				}
				if (func2 == null)
				{
					func2 = ((int x) => Convert.ToByte(Stringz.Substring(x, 2), 16));
				}
				array2 = (
					from x in Enumerable.Range(0, replace.Length)
					where x % 2 == 0
					select x).Select(func2).ToArray<byte>();
			}
			catch
			{
				throw new ArgumentException("Value not possible.", "Byte Array");
			}
			array = array2;
			result = array;
			return result;
		}
	}
}
