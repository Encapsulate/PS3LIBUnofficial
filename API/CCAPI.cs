using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace PS3Lib
{
	public class CCAPI
	{
		public enum BuzzerMode
		{
			Continuous,
			Single,
			Double
		}
        //https://www.nuget.org/packages/UnmanagedExports
		private class CCAPIGlobalPointer
		{
			private IntPtr _intPtr = IntPtr.Zero;
			public CCAPIGlobalPointer(IntPtr intPtr)
			{
				this._intPtr = intPtr;
			}
			~CCAPIGlobalPointer()
			{
				if (this._intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(this._intPtr);
				}
			}
			public IntPtr GetPtr()
			{
				return this._intPtr;
			}
		}
		public class ConsoleInfo
		{
			public string Ip;
			public string Name;
		}
		public enum ConsoleType
		{
			CEX = 1,
			DEX,
			TOOL
		}
		public enum LedColor
		{
			Green = 1,
			Red
		}
		public enum LedMode
		{
			Off,
			On,
			Blink
		}
		public enum NotifyIcon
		{
			INFO,
			CAUTION,
			FRIEND,
			SLIDER,
			WRONGWAY,
			DIALOG,
			DIALOGSHADOW,
			TEXT,
			POINTER,
			GRAB,
			HAND,
			PEN,
			FINGER,
			ARROW,
			ARROWRIGHT,
			PROGRESS
		}
		public enum ProcessType
		{
			VSH,
			SYS_AGENT,
			CURRENTGAME
		}
		public enum RebootFlags
		{
			HardReboot = 3,
			ShutDown = 1,
			SoftReboot
		}
		private class System
		{
			public static int connectionID = -1;
			public static uint processID = 0u;
			public static uint[] processIDs;
		}
		public class TargetInfo
		{
			public int CCAPI;
			public int ConsoleType;
			public int Firmware;
			public ulong SysTable;
			public int TempCell;
			public int TempRSX;
		}
		private CCAPI.TargetInfo pInfo = new CCAPI.TargetInfo();
		public Extension Extension
		{
			get
			{
				return new Extension(SelectAPI.ControlConsole);
			}
		}
		public CCAPI()
		{

		}
        [RGiesecke.DllExport.DllExport]
		public string BaSs_HaXoRwazHere()
		{
			string text = "Sup bro. Fancy meeting you here! ;p";
			return MessageBox.Show(string.Concat(new string[]
			{
				text,
				Environment.NewLine,
				"Go check out my youtube channel :D",
				Environment.NewLine,
				"www.youtube.com/CODNoobFriendly"
			})).ToString();
		}

        #region Get Mac and IP of PS3
        #region OUIFingerprint
        private string PS3OUIs = @"
00:1D:0D	
00:1F:A7
00:24:8D
00:D9:D1
28:0D:FC
70:9E:29
A8:E3:EE
F8:D0:AC
FC:0F:E6
001D0D	
001FA7
00248D
00D9D1
280DFC
709E29
A8E3EE
F8D0AC
FC0FE6

00248D 
271769 
0023f8 
0019c5 
FC0FE6 
002248 
A8E3EE 
00041F 
001315 
0015C1 
0019C5 
001D0D 
001FA7
00:24:8D 
27:17:69 
00:23:f8 
00:19:c5 
FC:0F:E6 
00:22:48 
A8:E3:EE 
00:04:1F 
00:13:15 
00:15:C1 
00:19:C5 
00:1D:0D 
00:1F:A7
";
        #endregion
       // public string ConsoleIP = null;
       // public string ConsoleMac = null;
       /* private void getpastie()
        {
            WebClient client = new WebClient();
            PS3OUIs = client.DownloadString("http://pastebin.com/raw.php?i=BpcKRq64");
        }*/

        #region NetSniff
        [StructLayout(LayoutKind.Sequential)]
        struct MIB_IPNETROW
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwIndex;
            [MarshalAs(UnmanagedType.U4)]
            public int dwPhysAddrLen;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac0;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac1;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac2;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac3;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac4;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac5;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac6;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac7;
            [MarshalAs(UnmanagedType.U4)]
            public int dwAddr;
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
        }

        /// <summary>
        /// GetIpNetTable external method
        /// </summary>
        /// <param name="pIpNetTable"></param>
        /// <param name="pdwSize"></param>
        /// <param name="bOrder"></param>
        /// <returns></returns>
        [DllImport("IpHlpApi.dll")]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int GetIpNetTable(IntPtr pIpNetTable,
              [MarshalAs(UnmanagedType.U4)] ref int pdwSize, bool bOrder);

        /// <summary>
        /// Error codes GetIpNetTable returns that we recognise
        /// </summary>
        const int ERROR_INSUFFICIENT_BUFFER = 122;
        /// <summary>
        /// Get the IP and MAC addresses of all known devices on the LAN
        /// </summary>
        /// <remarks>
        /// 1) This table is not updated often - it can take some human-scale time 
        ///    to notice that a device has dropped off the network, or a new device
        ///    has connected.
        /// 2) This discards non-local devices if they are found - these are multicast
        ///    and can be discarded by IP address range.
        /// </remarks>
        /// <returns></returns>

        private static Dictionary<IPAddress, PhysicalAddress> GetAllDevicesOnLAN()
        {
            Dictionary<IPAddress, PhysicalAddress> all = new Dictionary<IPAddress, PhysicalAddress>();
            // Add this PC to the list...
            all.Add(GetIPAddress(), GetMacAddress());
            int spaceForNetTable = 0;
            // Get the space needed
            // We do that by requesting the table, but not giving any space at all.
            // The return value will tell us how much we actually need.
            GetIpNetTable(IntPtr.Zero, ref spaceForNetTable, false);
            // Allocate the space
            // We use a try-finally block to ensure release.
            IntPtr rawTable = IntPtr.Zero;
            try
            {
                rawTable = Marshal.AllocCoTaskMem(spaceForNetTable);
                // Get the actual data
                int errorCode = GetIpNetTable(rawTable, ref spaceForNetTable, false);
                if (errorCode != 0)
                {
                    // Failed for some reason - can do no more here.
                    throw new Exception(string.Format(
                      "Unable to retrieve network table. Error code {0}", errorCode));
                }
                // Get the rows count
                int rowsCount = Marshal.ReadInt32(rawTable);
                IntPtr currentBuffer = new IntPtr(rawTable.ToInt64() + Marshal.SizeOf(typeof(Int32)));
                // Convert the raw table to individual entries
                MIB_IPNETROW[] rows = new MIB_IPNETROW[rowsCount];
                for (int index = 0; index < rowsCount; index++)
                {
                    rows[index] = (MIB_IPNETROW)Marshal.PtrToStructure(new IntPtr(currentBuffer.ToInt64() +
                                                (index * Marshal.SizeOf(typeof(MIB_IPNETROW)))
                                               ),
                                                typeof(MIB_IPNETROW));
                }
                // Define the dummy entries list (we can discard these)
                PhysicalAddress virtualMAC = new PhysicalAddress(new byte[] { 0, 0, 0, 0, 0, 0 });
                PhysicalAddress broadcastMAC = new PhysicalAddress(new byte[] { 255, 255, 255, 255, 255, 255 });
                foreach (MIB_IPNETROW row in rows)
                {
                    IPAddress ip = new IPAddress(BitConverter.GetBytes(row.dwAddr));
                    byte[] rawMAC = new byte[] { row.mac0, row.mac1, row.mac2, row.mac3, row.mac4, row.mac5 };
                    PhysicalAddress pa = new PhysicalAddress(rawMAC);
                    if (!pa.Equals(virtualMAC) && !pa.Equals(broadcastMAC) && !IsMulticast(ip))
                    {
                        //Console.WriteLine("IP: {0}\t\tMAC: {1}", ip.ToString(), pa.ToString());
                        if (!all.ContainsKey(ip))
                        {
                            all.Add(ip, pa);
                        }
                    }
                }
            }
            finally
            {
                // Release the memory.
                Marshal.FreeCoTaskMem(rawTable);
            }
            return all;
        }

        /// <summary>
        /// Gets the IP address of the current PC
        /// </summary>
        /// <returns></returns>
        private static IPAddress GetIPAddress()
        {
            String strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            foreach (IPAddress ip in addr)
            {
                if (!ip.IsIPv6LinkLocal)
                {
                    return (ip);
                }
            }
            return addr.Length > 0 ? addr[0] : null;
        }

        /// <summary>
        /// Gets the MAC address of the current PC.
        /// </summary>
        /// <returns></returns>
        private static PhysicalAddress GetMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    return nic.GetPhysicalAddress();
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if the specified IP address is a multicast address
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool IsMulticast(IPAddress ip)
        {
            bool result = true;
            if (!ip.IsIPv6Multicast)
            {
                byte highIP = ip.GetAddressBytes()[0];
                if (highIP < 224 || highIP > 239)
                {
                    result = false;
                }
            }
            return result;
        }

        #endregion
        //
        public string[] searchNetwork()
        {
            PS3API PS3 = new PS3API(SelectAPI.ControlConsole);
            Dictionary<IPAddress, PhysicalAddress> all = GetAllDevicesOnLAN();
            foreach (KeyValuePair<IPAddress, PhysicalAddress> kvp in all)
            {
                string Consoles = "";//clear console
                string infos = "IP: " + kvp.Key + Environment.NewLine + "Mac: " + kvp.Value;
                //http://pastebin.com/yKLTzdW8
                string fullmacaddress = Convert.ToString(kvp.Value);
                fullmacaddress = fullmacaddress.Substring(0, 6);
                MessageBox.Show("IPaddress: " + kvp.Key + Environment.NewLine + "MAC: " + kvp.Value);
                if (PS3OUIs.Contains(fullmacaddress))
                {
                    string[] consoleandmacs = null;
                    string ip = Convert.ToString(kvp.Key);
                    string mac = Convert.ToString(kvp.Value);
                    Consoles = Consoles + ip + Environment.NewLine + mac + " ";
                    consoleandmacs = new[] { Consoles };
                    MessageBox.Show("" + kvp.Key);
                }
            }
            return new string[0];
        }
        public string MACIP;
        public void Consoles()
        {
            PS3API PS3 = new PS3API(SelectAPI.ControlConsole);
            Dictionary<IPAddress, PhysicalAddress> all = GetAllDevicesOnLAN();
            IPList ip = new IPList();
            foreach (KeyValuePair<IPAddress, PhysicalAddress> kvp in all)
            {
                Thread.Sleep(100);
                string infos = "IP: " + kvp.Key + Environment.NewLine + "Mac: " + kvp.Value;
                string fullmacaddress = Convert.ToString(kvp.Value);
                fullmacaddress = fullmacaddress.Substring(0, 6);
                //    if (PS3OUIs.Contains(fullmacaddress))
                //     {
               // IPList ip = new IPList();
                MACIP = kvp.Key.ToString();
                //  ip.ipmac = ip.ipmac + kvp.Key + ", " + kvp.Value;
                List<String> iplist = new List<String>();
                iplist.Add(kvp.Key.ToString() + " - " + kvp.Value.ToString());
                foreach (string ips in iplist)
                {
                    ip.ipmac = new string[] { ips };
                }
            }
            ip.Show();
        }
        public bool CCAPIConnection = false;
        /*
        [DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi14connectConsoleEPKc")]
        private static extern int connectConsole(string targetIP);
        public int ConnectTarget(string targetIP)
        {
            if (this.SUCCESS(CCAPI.System.connectionID))
            {
                CCAPI.disconnectConsole(CCAPI.System.connectionID);
            }
            CCAPI.initTargetComms();
            CCAPI.System.connectionID = CCAPI.connectConsole(targetIP);
            CCAPI cCAPI = new CCAPI();
            cCAPI.Notify(CCAPI.NotifyIcon.FRIEND, "PS3Lib(4.4) [UNOFFICIAL]: By BaSs_HaXoR");
            return CCAPI.System.connectionID;
        }
        */
        [RGiesecke.DllExport.DllExport]
        public void CCAPIConnect()
        {
            PS3API PS3 = new PS3API(SelectAPI.ControlConsole);
            Dictionary<IPAddress, PhysicalAddress> all = GetAllDevicesOnLAN();
            foreach (KeyValuePair<IPAddress, PhysicalAddress> kvp in all)
            {
                Thread.Sleep(100);
                //"IP : {0}\n MAC {1}" + 
                string infos = "IP: " + kvp.Key + Environment.NewLine + "Mac: " + kvp.Value;
                //  MessageBox.Show(infos);
                //   consoleandmacs = new [] { infos };
                //http://pastebin.com/yKLTzdW8
               // MessageBox.Show(infos);
                string fullmacaddress = Convert.ToString(kvp.Value);
                fullmacaddress = fullmacaddress.Substring(0, 6);
/*
                if (fullmacaddress.Length >= 6)
                {
                    fullmacaddress = fullmacaddress.Substring(0, 6);
                }
                else
                {
                    try
                    {
                        fullmacaddress = fullmacaddress.Substring(0, 3);
                    }
                    catch
                    {
                        MessageBox.Show("Error getting Macaddresses on the Network!");
                        MessageBox.Show(fullmacaddress);
                    }
                }*/
                if (PS3OUIs.Contains(fullmacaddress))
                {
                    MACIP = kvp.Key.ToString();
                    
                    if (PS3.ConnectTarget(MACIP))
                    {
                        CCAPIConnection = true;
                        PS3.CCAPI.RingBuzzer(CCAPI.BuzzerMode.Double);
                        //   PS3.CCAPI.BaSs_HaXoRwazHere();
                        this.MACIP = kvp.Key.ToString();
                        return;
                    }
                    else
                    {
                        CCAPIConnection = false;
                        //MessageBox.Show("Failed to connect to Console!");
                        // PS3.ConnectTarget("192.168.137.201");
                        //PS3.CCAPI.Notify(CCAPI.NotifyIcon.WRONGWAY, "Failed!");
                    }
                }
                else
                {
                 //   MessageBox.Show("No PS3 Consoles found on your Network!");
                }
            }
        }
        #endregion

        private bool IsValidHexString(IEnumerable<char> hexString)
		{
			return (
				from currentCharacter in hexString
				select (currentCharacter >= '0' && currentCharacter <= '9') || (currentCharacter >= 'a' && currentCharacter <= 'f') || (currentCharacter >= 'A' && currentCharacter <= 'F')).All((bool isHexCharacter) => isHexCharacter);
		}
		private byte[] HexStringToByteArray(string hex)
		{
			return (
				from x in Enumerable.Range(0, hex.Length)
				where x % 2 == 0
				select Convert.ToByte(hex.Substring(x, 2), 16)).ToArray<byte>();
		}
		public void SetPSID(string PSID)
		{
			if (this.IsValidHexString(PSID) && PSID.Length == 32)
			{
				byte[] buffer = this.HexStringToByteArray(PSID);
				this.SetLv2Memory(9223372036859580196uL, buffer);
			}
			else
			{
				MessageBox.Show("Check Your PSID. Wrong Length or Hex Chars.");
			}
		}
		public string GetConsoleID()
		{
			byte[] array = new byte[16];
			this.GetLv2Memory(9223372036858981040uL, array);
			return BitConverter.ToString(array).Replace("-", " ");
		}
		[DllImport("CCAPI2.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int getLv1Memory(int connectionId, ulong offset, uint size, byte[] buffOut);
		public int GetLv1Memory(uint offset, byte[] buffer)
		{
			return CCAPI.getLv1Memory(CCAPI.System.connectionID, (ulong)offset, (uint)buffer.Length, buffer);
		}
		public int GetLv1Memory(ulong offset, byte[] buffer)
		{
			return CCAPI.getLv1Memory(CCAPI.System.connectionID, offset, (uint)buffer.Length, buffer);
		}
		[DllImport("CCAPI2.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int getLv2Memory(int connectionId, ulong offset, uint size, byte[] buffOut);
		public int GetLv2Memory(ulong offset, byte[] buffer)
		{
			return CCAPI.getLv2Memory(CCAPI.System.connectionID, offset, (uint)buffer.Length, buffer);
		}
		[DllImport("CCAPI2.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int setLv1Memory(int connectionId, ulong offset, uint size, byte[] buffIn);
		public int SetLv1Memory(uint offset, byte[] buffer)
		{
			return CCAPI.setLv1Memory(CCAPI.System.connectionID, (ulong)offset, (uint)buffer.Length, buffer);
		}
		public int SetLv1Memory(ulong offset, byte[] buffer)
		{
			return CCAPI.setLv1Memory(CCAPI.System.connectionID, offset, (uint)buffer.Length, buffer);
		}
		[DllImport("CCAPI2.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int setLv2Memory(int connectionId, ulong offset, uint size, byte[] buffIn);
		public int SetLv2Memory(ulong offset, byte[] buffer)
		{
			return CCAPI.setLv2Memory(CCAPI.System.connectionID, offset, (uint)buffer.Length, buffer);
		}
		public string GetPSID()
		{
			byte[] array = new byte[16];
			this.GetLv2Memory(9223372036859580196uL, array);
			return BitConverter.ToString(array).Replace("-", " ");
		}
		public int AttachProcess()
		{
			CCAPI.System.processID = 0u;
			int num = this.GetProcessList(out CCAPI.System.processIDs);
			int result;
			if (!this.SUCCESS(num) || CCAPI.System.processIDs.Length <= 0)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < CCAPI.System.processIDs.Length; i++)
				{
					string empty = string.Empty;
					num = this.GetProcessName(CCAPI.System.processIDs[i], out empty);
					if (!this.SUCCESS(num))
					{
						break;
					}
					if (!empty.Contains("flash"))
					{
						CCAPI.System.processID = CCAPI.System.processIDs[i];
						break;
					}
					num = -1;
				}
				if (CCAPI.System.processID == 0u)
				{
					CCAPI.System.processID = CCAPI.System.processIDs[CCAPI.System.processIDs.Length - 1];
				}
				result = num;
			}
			return result;
		}
		public int AttachProcess(CCAPI.ProcessType procType)
		{
			CCAPI.System.processID = 0u;
			int num = this.GetProcessList(out CCAPI.System.processIDs);
			int result;
			if (num < 0 || CCAPI.System.processIDs.Length <= 0)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < CCAPI.System.processIDs.Length; i++)
				{
					string empty = string.Empty;
					num = this.GetProcessName(CCAPI.System.processIDs[i], out empty);
					if (num < 0)
					{
						break;
					}
					if (procType == CCAPI.ProcessType.VSH && empty.Contains("vsh"))
					{
						CCAPI.System.processID = CCAPI.System.processIDs[i];
						break;
					}
					if (procType == CCAPI.ProcessType.SYS_AGENT && empty.Contains("agent"))
					{
						CCAPI.System.processID = CCAPI.System.processIDs[i];
						break;
					}
					if (procType == CCAPI.ProcessType.CURRENTGAME && !empty.Contains("flash"))
					{
						CCAPI.System.processID = CCAPI.System.processIDs[i];
						break;
					}
				}
				if (CCAPI.System.processID == 0u)
				{
					CCAPI.System.processID = CCAPI.System.processIDs[CCAPI.System.processIDs.Length - 1];
				}
				result = num;
			}
			return result;
		}
		public int AttachProcess(uint process)
		{
			uint[] array = new uint[64];
			int num = this.GetProcessList(out array);
			int num2;
			int result;
			if (this.SUCCESS(num))
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == process)
					{
						num = 0;
						CCAPI.System.processID = process;
						num2 = num;
						result = num2;
						return result;
					}
					num = -1;
				}
			}
			num2 = num;
			result = num2;
			return result;
		}
		public void ClearTargetInfo()
		{
			this.pInfo = new CCAPI.TargetInfo();
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi16closeTargetCommsEv")]
		private static extern int closeTargetComms();
		private void CompleteInfo(ref CCAPI.TargetInfo Info, int fw, int ccapi, ulong sysTable, int consoleType, int tempCELL, int tempRSX)
		{
			Info.Firmware = fw;
			Info.CCAPI = ccapi;
			Info.SysTable = sysTable;
			Info.ConsoleType = consoleType;
			Info.TempCell = tempCELL;
			Info.TempRSX = tempRSX;
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi14connectConsoleEPKc")]
		private static extern int connectConsole(string targetIP);
		public int ConnectTarget(string targetIP)
		{
			if (this.SUCCESS(CCAPI.System.connectionID))
			{
				CCAPI.disconnectConsole(CCAPI.System.connectionID);
			}
			CCAPI.initTargetComms();
			CCAPI.System.connectionID = CCAPI.connectConsole(targetIP);
			CCAPI cCAPI = new CCAPI();
			cCAPI.Notify(CCAPI.NotifyIcon.FRIEND, "PS3Lib(4.4) [UNOFFICIAL]: By BaSs_HaXoR");
			return CCAPI.System.connectionID;
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi17disconnectConsoleEi")]
		private static extern int disconnectConsole(int connectionID);
		public int DisconnectTarget()
		{
			return CCAPI.disconnectConsole(CCAPI.System.connectionID);
		}
		public uint GetAttachedProcess()
		{
			return CCAPI.System.processID;
		}
		public byte[] GetBytes(uint offset, uint length)
		{
			byte[] array = new byte[length];
			this.GetMemory(offset, array);
			return array;
		}
		public byte[] GetBytes(ulong offset, uint length)
		{
			byte[] array = new byte[length];
			this.GetMemory(offset, array);
			return array;
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN8Settings14getConsoleInfoEiPcS0_")]
		private static extern int getConsoleInfo(int index, IntPtr ptrN, IntPtr ptrI);
		public List<CCAPI.ConsoleInfo> GetConsoleList()
		{
			List<CCAPI.ConsoleInfo> list = new List<CCAPI.ConsoleInfo>();
			int numberOfConsoles = CCAPI.getNumberOfConsoles();
			IntPtr intPtr = Marshal.AllocHGlobal(256);
			IntPtr intPtr2 = Marshal.AllocHGlobal(256);
			for (int i = 0; i < numberOfConsoles; i++)
			{
				CCAPI.ConsoleInfo consoleInfo = new CCAPI.ConsoleInfo();
				CCAPI.getConsoleInfo(i, intPtr, intPtr2);
				consoleInfo.Name = Marshal.PtrToStringAnsi(intPtr);
				consoleInfo.Ip = Marshal.PtrToStringAnsi(intPtr2);
				list.Add(consoleInfo);
			}
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			return list;
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi13getDllVersionEv")]
		private static extern int getDllVersion();
        [RGiesecke.DllExport.DllExport]
        public int GetDllVersion()
		{
			return CCAPI.getDllVersion();
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi15getFirmwareInfoEiPiS0_PyS0_")]
		private static extern int getFirmwareInfo(int connectionId, ref int firmware, ref int ccapi, ref ulong systable, ref int consoleType);
		public string GetFirmwareType()
		{
			if (this.pInfo.ConsoleType.ToString() == "")
			{
				this.GetTargetInfo(out this.pInfo);
			}
			string text = string.Empty;
			string result;
			if (this.pInfo.ConsoleType == 1)
			{
				result = "CEX";
			}
			else
			{
				if (this.pInfo.ConsoleType == 2)
				{
					result = "DEX";
				}
				else
				{
					if (this.pInfo.ConsoleType == 3)
					{
						text = "TOOL";
					}
					result = text;
				}
			}
			return result;
		}
		public string GetFirmwareVersion()
		{
			if (this.pInfo.Firmware == 0)
			{
				this.GetTargetInfo();
			}
			string text = this.pInfo.Firmware.ToString("X8");
			string str = text.Substring(1, 1) + ".";
			string str2 = text.Substring(3, 1);
			string str3 = text.Substring(4, 1);
			return str + str2 + str3;
		}
		public int GetMemory(uint offset, byte[] buffer)
		{
			return CCAPI.getProcessMemory(CCAPI.System.connectionID, CCAPI.System.processID, (ulong)offset, (uint)buffer.Length, buffer);
		}
		public int GetMemory(ulong offset, byte[] buffer)
		{
			return CCAPI.getProcessMemory(CCAPI.System.connectionID, CCAPI.System.processID, offset, (uint)buffer.Length, buffer);
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN8Settings19getNumberOfConsolesEv")]
		private static extern int getNumberOfConsoles();
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi14getProcessListEiPjS0_")]
		private static extern int getProcessList(int connectionID, ref uint numberProcesses, IntPtr processIdPtr);
		public int GetProcessList(out uint[] processIds)
		{
			uint num = 0u;
			CCAPI.CCAPIGlobalPointer cCAPIGlobalPointer = new CCAPI.CCAPIGlobalPointer(Marshal.AllocHGlobal(256));
			int processList = CCAPI.getProcessList(CCAPI.System.connectionID, ref num, cCAPIGlobalPointer.GetPtr());
			processIds = new uint[num];
			if (this.SUCCESS(processList))
			{
				IntPtr unBuf = cCAPIGlobalPointer.GetPtr();
				for (uint num2 = 0u; num2 < num; num2 += 1u)
				{
					unBuf = this.ReadDataFromUnBufPtr<uint>(unBuf, ref processIds[(int)((uint)((UIntPtr)num2))]);
				}
			}
			return processList;
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi16getProcessMemoryEijyjPh")]
		private static extern int getProcessMemory(int connectionID, uint processID, ulong offset, uint size, byte[] buffOut);
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi14getProcessNameEijPc")]
		private static extern int getProcessName(int connectionID, uint processID, IntPtr strPtr);
		public int GetProcessName(uint processId, out string name)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(529);
			int processName = CCAPI.getProcessName(CCAPI.System.connectionID, processId, intPtr);
			name = string.Empty;
			if (this.SUCCESS(processName))
			{
				name = Marshal.PtrToStringAnsi(intPtr);
			}
			return processName;
		}
		private int GetTargetInfo()
		{
			int[] array = new int[2];
			int fw = 0;
			int ccapi = 0;
			int consoleType = 0;
			ulong sysTable = 0uL;
			int num = CCAPI.getFirmwareInfo(CCAPI.System.connectionID, ref fw, ref ccapi, ref sysTable, ref consoleType);
			int num2;
			int result;
			if (num >= 0)
			{
				CCAPI.CCAPIGlobalPointer cCAPIGlobalPointer = new CCAPI.CCAPIGlobalPointer(Marshal.AllocHGlobal(8));
				num = CCAPI.getTemperature(CCAPI.System.connectionID, cCAPIGlobalPointer.GetPtr());
				if (num < 0)
				{
					num2 = num;
					result = num2;
					return result;
				}
				IntPtr unBuf = cCAPIGlobalPointer.GetPtr();
				for (uint num3 = 0u; num3 < 2u; num3 += 1u)
				{
					unBuf = this.ReadDataFromUnBufPtr<int>(unBuf, ref array[(int)((uint)((UIntPtr)num3))]);
				}
				this.CompleteInfo(ref this.pInfo, fw, ccapi, sysTable, consoleType, array[0], array[1]);
			}
			num2 = num;
			result = num2;
			return result;
		}
		public int GetTargetInfo(out CCAPI.TargetInfo Info)
		{
			Info = new CCAPI.TargetInfo();
			int[] array = new int[2];
			int fw = 0;
			int ccapi = 0;
			int consoleType = 0;
			ulong sysTable = 0uL;
			int num = CCAPI.getFirmwareInfo(CCAPI.System.connectionID, ref fw, ref ccapi, ref sysTable, ref consoleType);
			int num2;
			int result;
			if (num >= 0)
			{
				CCAPI.CCAPIGlobalPointer cCAPIGlobalPointer = new CCAPI.CCAPIGlobalPointer(Marshal.AllocHGlobal(8));
				num = CCAPI.getTemperature(CCAPI.System.connectionID, cCAPIGlobalPointer.GetPtr());
				if (num < 0)
				{
					num2 = num;
					result = num2;
					return result;
				}
				IntPtr unBuf = cCAPIGlobalPointer.GetPtr();
				for (uint num3 = 0u; num3 < 2u; num3 += 1u)
				{
					unBuf = this.ReadDataFromUnBufPtr<int>(unBuf, ref array[(int)((uint)((UIntPtr)num3))]);
				}
				this.CompleteInfo(ref Info, fw, ccapi, sysTable, consoleType, array[0], array[1]);
				this.CompleteInfo(ref this.pInfo, fw, ccapi, sysTable, consoleType, array[0], array[1]);
			}
			num2 = num;
			result = num2;
			return result;
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi14getTemperatureEiPj")]
		private static extern int getTemperature(int connectionId, IntPtr tempPtr);
		public string GetTemperatureCELL()
		{
			if (this.pInfo.TempCell == 0)
			{
				this.GetTargetInfo(out this.pInfo);
			}
			return this.pInfo.TempCell.ToString() + " C";
		}
		public string GetTemperatureRSX()
		{
			if (this.pInfo.TempRSX == 0)
			{
				this.GetTargetInfo(out this.pInfo);
			}
			return this.pInfo.TempRSX.ToString() + " C";
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi15initTargetCommsEv")]
		private static extern int initTargetComms();
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi6notifyEiiPw")]
		private static extern int notify(int connectionId, int mode, [MarshalAs(UnmanagedType.LPWStr)] string msgWChar);
		public int Notify(CCAPI.NotifyIcon icon, string message)
		{
			return CCAPI.notify(CCAPI.System.connectionID, (int)icon, message);
		}
		public int Notify(int icon, string message)
		{
			return CCAPI.notify(CCAPI.System.connectionID, icon, message);
		}
		private IntPtr ReadDataFromUnBufPtr<T>(IntPtr unBuf, ref T storage)
		{
			storage = (T)((object)Marshal.PtrToStructure(unBuf, typeof(T)));
			return new IntPtr(unBuf.ToInt64() + (long)Marshal.SizeOf(storage));
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi10ringBuzzerEii")]
		private static extern int ringBuzzer(int connectionId, int type);
		public int RingBuzzer(CCAPI.BuzzerMode flag)
		{
			return CCAPI.ringBuzzer(CCAPI.System.connectionID, (int)flag);
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi12setConsoleIDEiPh")]
		private static extern int setConsoleID(int connectionId, byte[] consoleID);
		public int SetConsoleID(string consoleID)
		{
			string hex = string.Empty;
			if (consoleID.Length >= 32)
			{
				hex = consoleID.Substring(0, 32);
			}
			return CCAPI.setConsoleID(CCAPI.System.connectionID, CCAPI.StringToByteArray(hex));
		}
		public int SetConsoleID(byte[] consoleID)
		{
			return CCAPI.setConsoleID(CCAPI.System.connectionID, consoleID);
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi13setConsoleLedEiii")]
		private static extern int setConsoleLed(int connectionId, int color, int status);
		public int SetConsoleLed(CCAPI.LedColor color, CCAPI.LedMode mode)
		{
			return CCAPI.setConsoleLed(CCAPI.System.connectionID, (int)color, (int)mode);
		}
		public int SetMemory(uint offset, byte[] buffer)
		{
			return CCAPI.setProcessMemory(CCAPI.System.connectionID, CCAPI.System.processID, (ulong)offset, (uint)buffer.Length, buffer);
		}
		public int SetMemory(ulong offset, byte[] buffer)
		{
			return CCAPI.setProcessMemory(CCAPI.System.connectionID, CCAPI.System.processID, offset, (uint)buffer.Length, buffer);
		}
		public int SetMemory(ulong offset, string hexadecimal, EndianType Type = EndianType.LittleEndian)
		{
			byte[] array = CCAPI.StringToByteArray(hexadecimal);
			if (Type == EndianType.LittleEndian)
			{
				Array.Reverse(array);
			}
			return CCAPI.setProcessMemory(CCAPI.System.connectionID, CCAPI.System.processID, offset, (uint)array.Length, array);
		}
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi16setProcessMemoryEijyjPh")]
		private static extern int setProcessMemory(int connectionID, uint processID, ulong offset, uint size, byte[] buffIn);
		[DllImport("CCAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN5CcApi8shutdownEii")]
		private static extern int shutdown(int connectionId, int mode);
		public int ShutDown(CCAPI.RebootFlags flag)
		{
			return CCAPI.shutdown(CCAPI.System.connectionID, (int)flag);
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
		public bool SUCCESS(int Void)
		{
			return Void >= 0;
		}
	}
}
