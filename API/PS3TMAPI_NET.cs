using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
namespace PS3Lib.NET
{
	public class PS3TMAPI
	{
		[Flags]
		public enum BootParameter : ulong
		{
			BluRayEmuOff = 4uL,
			BluRayEmuUSB = 32uL,
			DebugMode = 16uL,
			Default = 0uL,
			DualNIC = 128uL,
			HDDSpeedBluRayEmu = 8uL,
			HostFSTarget = 64uL,
			MemSizeConsole = 2uL,
			ReleaseMode = 1uL,
			SystemMode = 17uL
		}
		public enum ConnectStatus
		{
			Connected,
			Connecting,
			NotConnected,
			InUse,
			Unavailable
		}
		[Flags]
		public enum ResetParameter : ulong
		{
			Hard = 1uL,
			Quick = 2uL,
			ResetEx = 9223372036854775808uL,
			Soft = 0uL
		}
		private class ScopedGlobalHeapPtr
		{
			private IntPtr m_intPtr = IntPtr.Zero;
			public ScopedGlobalHeapPtr(IntPtr intPtr)
			{
				this.m_intPtr = intPtr;
			}
			~ScopedGlobalHeapPtr()
			{
				if (this.m_intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(this.m_intPtr);
				}
			}
			public IntPtr Get()
			{
				return this.m_intPtr;
			}
		}
		public enum SNRESULT
		{
			SN_E_BAD_ALIGN = -28,
			SN_E_BAD_MEMSPACE = -18,
			SN_E_BAD_PARAM = -21,
			SN_E_BAD_TARGET = -3,
			SN_E_BAD_UNIT = -11,
			SN_E_BUSY = -22,
			SN_E_CHECK_TARGET_CONFIGURATION = -33,
			SN_E_COMMAND_CANCELLED = -36,
			SN_E_COMMS_ERR = -5,
			SN_E_COMMS_EVENT_MISMATCHED_ERR = -39,
			SN_E_CONNECT_TO_GAMEPORT_FAILED = -35,
			SN_E_CONNECTED = -38,
			SN_E_DATA_TOO_LONG = -26,
			SN_E_DECI_ERROR = -23,
			SN_E_DEPRECATED = -27,
			SN_E_DLL_NOT_INITIALISED = -15,
			SN_E_ERROR = -2147483648,
			SN_E_EXISTING_CALLBACK = -24,
			SN_E_FILE_ERROR = -29,
			SN_E_HOST_NOT_FOUND = -8,
			SN_E_INSUFFICIENT_DATA = -25,
			SN_E_LICENSE_ERROR = -32,
			SN_E_LOAD_ELF_FAILED = -10,
			SN_E_LOAD_MODULE_FAILED = -31,
			SN_E_MODULE_NOT_FOUND = -34,
			SN_E_NO_SEL = -20,
			SN_E_NO_TARGETS,
			SN_E_NOT_CONNECTED = -4,
			SN_E_NOT_IMPL = -1,
			SN_E_NOT_LISTED = -13,
			SN_E_NOT_SUPPORTED_IN_SDK_VERSION = -30,
			SN_E_OUT_OF_MEM = -12,
			SN_E_PROTOCOL_ALREADY_REGISTERED = -37,
			SN_E_TARGET_IN_USE = -9,
			SN_E_TARGET_RUNNING = -17,
			SN_E_TIMEOUT = -7,
			SN_E_TM_COMMS_ERR,
			SN_E_TM_NOT_RUNNING = -2,
			SN_E_TM_VERSION = -14,
			SN_S_NO_ACTION = 6,
			SN_S_NO_MSG = 3,
			SN_S_OK = 0,
			SN_S_PENDING,
			SN_S_REPLACED = 5,
			SN_S_TARGET_STILL_REGISTERED = 7,
			SN_S_TM_VERSION = 4
		}
		public struct TargetInfo
		{
			public PS3TMAPI.TargetInfoFlag Flags;
			public int Target;
			public string Name;
			public string Type;
			public string Info;
			public string HomeDir;
			public string FSDir;
			public PS3TMAPI.BootParameter Boot;
		}
		[Flags]
		public enum TargetInfoFlag : uint
		{
			Boot = 32u,
			FileServingDir = 16u,
			HomeDir = 8u,
			Info = 4u,
			Name = 2u,
			TargetID = 1u
		}
		private struct TargetInfoPriv
		{
			public PS3TMAPI.TargetInfoFlag Flags;
			public int Target;
			public IntPtr Name;
			public IntPtr Type;
			public IntPtr Info;
			public IntPtr HomeDir;
			public IntPtr FSDir;
			public PS3TMAPI.BootParameter Boot;
		}
		[StructLayout(LayoutKind.Sequential)]
		public class TCPIPConnectProperties
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
			public string IPAddress;
			public uint Port;
		}
		public enum UnitType
		{
			PPU,
			SPU,
			SPURAW
		}
		private static IntPtr AllocUtf8FromString(string wcharString)
		{
			IntPtr result;
			if (wcharString == null)
			{
				result = IntPtr.Zero;
			}
			else
			{
				byte[] bytes = Encoding.UTF8.GetBytes(wcharString);
				IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length + 1);
				Marshal.Copy(bytes, 0, intPtr, bytes.Length);
				Marshal.WriteByte((IntPtr)(intPtr.ToInt64() + (long)bytes.Length), 0);
				result = intPtr;
			}
			return result;
		}
		public static PS3TMAPI.SNRESULT Connect(int target, string application)
		{
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.Is32Bit())
			{
				result = PS3TMAPI.ConnectX64(target, application);
			}
			else
			{
				result = PS3TMAPI.ConnectX86(target, application);
			}
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3Connect")]
		private static extern PS3TMAPI.SNRESULT ConnectX64(int target, string application);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3Connect")]
		private static extern PS3TMAPI.SNRESULT ConnectX86(int target, string application);
		public static PS3TMAPI.SNRESULT Disconnect(int target)
		{
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.Is32Bit())
			{
				result = PS3TMAPI.DisconnectX64(target);
			}
			else
			{
				result = PS3TMAPI.DisconnectX86(target);
			}
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3Disconnect")]
		private static extern PS3TMAPI.SNRESULT DisconnectX64(int target);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3Disconnect")]
		private static extern PS3TMAPI.SNRESULT DisconnectX86(int target);
		public static bool FAILED(PS3TMAPI.SNRESULT res)
		{
			return !PS3TMAPI.SUCCEEDED(res);
		}
		public static PS3TMAPI.SNRESULT GetConnectionInfo(int target, out PS3TMAPI.TCPIPConnectProperties connectProperties)
		{
			connectProperties = null;
			PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PS3TMAPI.TCPIPConnectProperties))));
			PS3TMAPI.SNRESULT sNRESULT = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetConnectionInfoX86(target, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetConnectionInfoX64(target, scopedGlobalHeapPtr.Get());
			if (PS3TMAPI.SUCCEEDED(sNRESULT))
			{
				connectProperties = new PS3TMAPI.TCPIPConnectProperties();
				Marshal.PtrToStructure(scopedGlobalHeapPtr.Get(), connectProperties);
			}
			return sNRESULT;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3GetConnectionInfo")]
		private static extern PS3TMAPI.SNRESULT GetConnectionInfoX64(int target, IntPtr connectProperties);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3GetConnectionInfo")]
		private static extern PS3TMAPI.SNRESULT GetConnectionInfoX86(int target, IntPtr connectProperties);
		public static PS3TMAPI.SNRESULT GetConnectStatus(int target, out PS3TMAPI.ConnectStatus status, out string usage)
		{
			uint num;
			IntPtr utf;
			PS3TMAPI.SNRESULT result = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetConnectStatusX86(target, out num, out utf) : PS3TMAPI.GetConnectStatusX64(target, out num, out utf);
			status = (PS3TMAPI.ConnectStatus)num;
			usage = PS3TMAPI.Utf8ToString(utf, 4294967295u);
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3GetConnectStatus")]
		private static extern PS3TMAPI.SNRESULT GetConnectStatusX64(int target, out uint status, out IntPtr usage);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3GetConnectStatus")]
		private static extern PS3TMAPI.SNRESULT GetConnectStatusX86(int target, out uint status, out IntPtr usage);
		public static PS3TMAPI.SNRESULT GetProcessList(int target, out uint[] processIDs)
		{
			processIDs = null;
			uint num = 0u;
			PS3TMAPI.SNRESULT sNRESULT = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessListX86(target, ref num, IntPtr.Zero) : PS3TMAPI.GetProcessListX64(target, ref num, IntPtr.Zero);
			PS3TMAPI.SNRESULT sNRESULT2;
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.FAILED(sNRESULT))
			{
				PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)(4u * num)));
				sNRESULT = (PS3TMAPI.Is32Bit() ? PS3TMAPI.GetProcessListX86(target, ref num, scopedGlobalHeapPtr.Get()) : PS3TMAPI.GetProcessListX64(target, ref num, scopedGlobalHeapPtr.Get()));
				if (PS3TMAPI.FAILED(sNRESULT))
				{
					sNRESULT2 = sNRESULT;
					result = sNRESULT2;
					return result;
				}
				IntPtr unmanagedBuf = scopedGlobalHeapPtr.Get();
				processIDs = new uint[num];
				for (uint num2 = 0u; num2 < num; num2 += 1u)
				{
					unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref processIDs[(int)((uint)((UIntPtr)num2))]);
				}
			}
			sNRESULT2 = sNRESULT;
			result = sNRESULT2;
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3ProcessList")]
		private static extern PS3TMAPI.SNRESULT GetProcessListX64(int target, ref uint count, IntPtr processIdArray);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3ProcessList")]
		private static extern PS3TMAPI.SNRESULT GetProcessListX86(int target, ref uint count, IntPtr processIdArray);
		public static PS3TMAPI.SNRESULT GetTargetFromName(string name, out int target)
		{
			PS3TMAPI.ScopedGlobalHeapPtr scopedGlobalHeapPtr = new PS3TMAPI.ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(name));
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.Is32Bit())
			{
				result = PS3TMAPI.GetTargetFromNameX64(scopedGlobalHeapPtr.Get(), out target);
			}
			else
			{
				result = PS3TMAPI.GetTargetFromNameX86(scopedGlobalHeapPtr.Get(), out target);
			}
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3GetTargetFromName")]
		private static extern PS3TMAPI.SNRESULT GetTargetFromNameX64(IntPtr name, out int target);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3GetTargetFromName")]
		private static extern PS3TMAPI.SNRESULT GetTargetFromNameX86(IntPtr name, out int target);
		public static PS3TMAPI.SNRESULT GetTargetInfo(ref PS3TMAPI.TargetInfo targetInfo)
		{
			PS3TMAPI.TargetInfoPriv targetInfoPriv = new PS3TMAPI.TargetInfoPriv
			{
				Flags = targetInfo.Flags,
				Target = targetInfo.Target
			};
			PS3TMAPI.SNRESULT sNRESULT = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetTargetInfoX86(ref targetInfoPriv) : PS3TMAPI.GetTargetInfoX64(ref targetInfoPriv);
			if (!PS3TMAPI.FAILED(sNRESULT))
			{
				targetInfo.Flags = targetInfoPriv.Flags;
				targetInfo.Target = targetInfoPriv.Target;
				targetInfo.Name = PS3TMAPI.Utf8ToString(targetInfoPriv.Name, 4294967295u);
				targetInfo.Type = PS3TMAPI.Utf8ToString(targetInfoPriv.Type, 4294967295u);
				targetInfo.Info = PS3TMAPI.Utf8ToString(targetInfoPriv.Info, 4294967295u);
				targetInfo.HomeDir = PS3TMAPI.Utf8ToString(targetInfoPriv.HomeDir, 4294967295u);
				targetInfo.FSDir = PS3TMAPI.Utf8ToString(targetInfoPriv.FSDir, 4294967295u);
				targetInfo.Boot = targetInfoPriv.Boot;
			}
			return sNRESULT;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3GetTargetInfo")]
		private static extern PS3TMAPI.SNRESULT GetTargetInfoX64(ref PS3TMAPI.TargetInfoPriv targetInfoPriv);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3GetTargetInfo")]
		private static extern PS3TMAPI.SNRESULT GetTargetInfoX86(ref PS3TMAPI.TargetInfoPriv targetInfoPriv);
		public static PS3TMAPI.SNRESULT InitTargetComms()
		{
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.Is32Bit())
			{
				result = PS3TMAPI.InitTargetCommsX64();
			}
			else
			{
				result = PS3TMAPI.InitTargetCommsX86();
			}
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3InitTargetComms")]
		private static extern PS3TMAPI.SNRESULT InitTargetCommsX64();
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3InitTargetComms")]
		private static extern PS3TMAPI.SNRESULT InitTargetCommsX86();
		private static bool Is32Bit()
		{
			return IntPtr.Size == 4;
		}
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int MultiByteToWideChar(int codepage, int flags, IntPtr utf8, int utf8len, StringBuilder buffer, int buflen);
		public static PS3TMAPI.SNRESULT PowerOff(int target, bool bForce)
		{
			int force = bForce ? 1 : 0;
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.Is32Bit())
			{
				result = PS3TMAPI.PowerOffX64(target, force);
			}
			else
			{
				result = PS3TMAPI.PowerOffX86(target, force);
			}
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3PowerOff")]
		private static extern PS3TMAPI.SNRESULT PowerOffX64(int target, int force);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3PowerOff")]
		private static extern PS3TMAPI.SNRESULT PowerOffX86(int target, int force);
		public static PS3TMAPI.SNRESULT PowerOn(int target)
		{
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.Is32Bit())
			{
				result = PS3TMAPI.PowerOnX64(target);
			}
			else
			{
				result = PS3TMAPI.PowerOnX86(target);
			}
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3PowerOn")]
		private static extern PS3TMAPI.SNRESULT PowerOnX64(int target);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3PowerOn")]
		private static extern PS3TMAPI.SNRESULT PowerOnX86(int target);
		public static PS3TMAPI.SNRESULT ProcessAttach(int target, PS3TMAPI.UnitType unit, uint processID)
		{
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.Is32Bit())
			{
				result = PS3TMAPI.ProcessAttachX64(target, (uint)unit, processID);
			}
			else
			{
				result = PS3TMAPI.ProcessAttachX86(target, (uint)unit, processID);
			}
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3ProcessAttach")]
		private static extern PS3TMAPI.SNRESULT ProcessAttachX64(int target, uint unitId, uint processId);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3ProcessAttach")]
		private static extern PS3TMAPI.SNRESULT ProcessAttachX86(int target, uint unitId, uint processId);
		public static PS3TMAPI.SNRESULT ProcessContinue(int target, uint processID)
		{
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.Is32Bit())
			{
				result = PS3TMAPI.ProcessContinueX64(target, processID);
			}
			else
			{
				result = PS3TMAPI.ProcessContinueX86(target, processID);
			}
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3ProcessContinue")]
		private static extern PS3TMAPI.SNRESULT ProcessContinueX64(int target, uint processId);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3ProcessContinue")]
		private static extern PS3TMAPI.SNRESULT ProcessContinueX86(int target, uint processId);
		public static PS3TMAPI.SNRESULT ProcessGetMemory(int target, PS3TMAPI.UnitType unit, uint processID, ulong threadID, ulong address, ref byte[] buffer)
		{
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.Is32Bit())
			{
				result = PS3TMAPI.ProcessGetMemoryX64(target, unit, processID, threadID, address, buffer.Length, buffer);
			}
			else
			{
				result = PS3TMAPI.ProcessGetMemoryX86(target, unit, processID, threadID, address, buffer.Length, buffer);
			}
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3ProcessGetMemory")]
		private static extern PS3TMAPI.SNRESULT ProcessGetMemoryX64(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3ProcessGetMemory")]
		private static extern PS3TMAPI.SNRESULT ProcessGetMemoryX86(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);
		public static PS3TMAPI.SNRESULT ProcessSetMemory(int target, PS3TMAPI.UnitType unit, uint processID, ulong threadID, ulong address, byte[] buffer)
		{
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.Is32Bit())
			{
				result = PS3TMAPI.ProcessSetMemoryX64(target, unit, processID, threadID, address, buffer.Length, buffer);
			}
			else
			{
				result = PS3TMAPI.ProcessSetMemoryX86(target, unit, processID, threadID, address, buffer.Length, buffer);
			}
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3ProcessSetMemory")]
		private static extern PS3TMAPI.SNRESULT ProcessSetMemoryX64(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3ProcessSetMemory")]
		private static extern PS3TMAPI.SNRESULT ProcessSetMemoryX86(int target, PS3TMAPI.UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);
		private static IntPtr ReadDataFromUnmanagedIncPtr<T>(IntPtr unmanagedBuf, ref T storage)
		{
			storage = (T)((object)Marshal.PtrToStructure(unmanagedBuf, typeof(T)));
			return new IntPtr(unmanagedBuf.ToInt64() + (long)Marshal.SizeOf(storage));
		}
		public static PS3TMAPI.SNRESULT Reset(int target, PS3TMAPI.ResetParameter resetParameter)
		{
			PS3TMAPI.SNRESULT result;
			if (!PS3TMAPI.Is32Bit())
			{
				result = PS3TMAPI.ResetX64(target, (ulong)resetParameter);
			}
			else
			{
				result = PS3TMAPI.ResetX86(target, (ulong)resetParameter);
			}
			return result;
		}
		[DllImport("PS3TMAPIX64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3Reset")]
		private static extern PS3TMAPI.SNRESULT ResetX64(int target, ulong resetParameter);
		[DllImport("PS3TMAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNPS3Reset")]
		private static extern PS3TMAPI.SNRESULT ResetX86(int target, ulong resetParameter);
		public static bool SUCCEEDED(PS3TMAPI.SNRESULT res)
		{
			return res >= PS3TMAPI.SNRESULT.SN_S_OK;
		}
		public static string Utf8ToString(IntPtr utf8, uint maxLength)
		{
			int num = PS3TMAPI.MultiByteToWideChar(65001, 0, utf8, -1, null, 0);
			if (num == 0)
			{
				throw new Win32Exception();
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			num = PS3TMAPI.MultiByteToWideChar(65001, 0, utf8, -1, stringBuilder, num);
			return stringBuilder.ToString();
		}
	}
}
