/*
Copyright 2015 Yixin Zhang

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

SWF file decompression support is provided by the zlib 1.2.8 library,
distributed under zlib license 1.2.8.
*/

﻿
//#define debug

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TFM_Client
{
    class MemoryWriter
    {
        private UIntPtr bytesWritten;
        private int bytesRead;
        private int foundAddress = 0;
        private uint startAddress = 0;
        private IntPtr hProcess;
        private System.Diagnostics.Process proc;
        private byte[] buffer;
        private string strBuffer = "";
        private MEMORY_BASIC_INFORMATION mbi = new MEMORY_BASIC_INFORMATION();
        private SYSTEM_INFO si = new SYSTEM_INFO();
        private int region = 1;
        private string mask = "nnyyynnnn";

        public MemoryWriter()
        {
            GetSystemInfo(ref si);
            return;
        }

        public MemoryWriter(System.Diagnostics.Process p)
        {
            proc = p;
            GetSystemInfo(ref si);
        }

        public bool Patch(byte[] search, byte[] replace)
        {
            int replaced = 0;
            startAddress = 0;
            foundAddress = 0;
            region = 1;
            #if debug 
                System.Text.StringBuilder summary = new System.Text.StringBuilder();
                string strSearch = ByteArrayToHexString(search, "");
            #elif !debug
                string strSearch = HexStr(search).Replace("0x", "");
            #endif
            hProcess = OpenProcess(ProcessAccess.QueryInformation | ProcessAccess.VMOperation | ProcessAccess.VMRead | ProcessAccess.VMWrite, true, proc.Id);
            if (hProcess != IntPtr.Zero)
            {
                try
                {
                    while (startAddress < si.lpMaximumApplicationAddress.ToInt64())
                    {
                        if (VirtualQueryEx(hProcess, (IntPtr)startAddress, out mbi, (uint)Marshal.SizeOf(mbi)) != 0)
                        {
                            if ((mbi.State == (uint)State.MEM_COMMIT)
                            &&
                            (mbi.Protect != (uint)AllocationProtect.PAGE_READONLY)
                            &&
                            (mbi.Protect != (uint)AllocationProtect.PAGE_EXECUTE_READ)
                            &&
                            (mbi.Protect != (uint)AllocationProtect.PAGE_GUARD)
                            &&
                            (mbi.Protect != (uint)AllocationProtect.PAGE_NOACCESS)
                            )
                            {
                                startAddress = (uint)mbi.BaseAddress;
                                buffer = new byte[(int)mbi.RegionSize];
                                if (ReadProcessMemory(hProcess, (IntPtr)startAddress, buffer, (int)mbi.RegionSize, out bytesRead))
                                {
                                    #if !debug
                                        strBuffer = HexStr(buffer).Replace("0x", "");
                                    #elif debug
                                        strBuffer = ByteArrayToHexString(buffer, "");
                                    #endif
                                    while ((foundAddress = strBuffer.IndexOf(strSearch, foundAddress, StringComparison.Ordinal)) != -1)
                                    {

                                        if (region >= 2)
                                        {
                                            #if debug
                                                summary.Append((startAddress + (foundAddress / 2 - 1 )).ToString("X"));
                                                summary.AppendLine();
                                            #else
                                                if (mask[replaced] == 'y') if (!Write((IntPtr)((int)startAddress + (foundAddress / 2)), replace)) return false;
                                            #endif
                                            replaced++;
                                        }
                                        #if !debug
                                            if (replaced > mask.LastIndexOf('y'))
                                            {
                                                CloseHandle(hProcess);
                                                //mask = "nnnnnyyyyyy";
                                                mask = "yyyyyyyyy";
                                                return true;
                                            }
                                        #endif
                                        foundAddress++;
                                        if (strBuffer.IndexOf(strSearch, foundAddress, StringComparison.Ordinal) == -1) region++;
                                    }
                                    foundAddress = 0;
                                }

                                strBuffer = "";
                                buffer = null;
                                FlushMemory();
                            }
                        }
                        if (startAddress > (uint)si.lpMaximumApplicationAddress - (uint)mbi.RegionSize)
                        {
                            #if debug
                                summary.Append(replaced);
                                System.IO.File.WriteAllText("summary.txt", summary.ToString(), System.Text.Encoding.UTF8);
                            #endif
                            return false;
                        }
                        startAddress += (uint)mbi.RegionSize;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    #if debug
                        MessageBox.Show(e.ToString());
                    #endif
                    return false;
                }
            }
            return false;
        }

        public bool Patch(string search, string replace)
        {
            return Patch(StrToByteArray(search), StrToByteArray(replace));
        }

        public void Target(System.Diagnostics.Process p)
        {
            proc = p;
        }

#if !debug

        public bool Write(IntPtr address, byte[] replace)
        {
            return WriteProcessMemory(hProcess, (IntPtr)address, replace, (uint)replace.Length, out bytesWritten);
        }

        public bool Write(uint address, string s)
        {
            return Write((IntPtr)address, StrToByteArray(s));
        }

#endif

        public static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

#if !debug

        public static string HexStr(byte[] p)
        {

            char[] c = new char[p.Length * 2 + 2];

            byte b;

            c[0] = '0'; c[1] = 'x';

            for (int y = 0, x = 2; y < p.Length; ++y, ++x)
            {

                b = ((byte)(p[y] >> 4));

                c[x] = (char)(b > 9 ? b + 0x37 : b + 0x30);

                b = ((byte)(p[y] & 0xF));

                c[++x] = (char)(b > 9 ? b + 0x37 : b + 0x30);

            }

            return new string(c);

        }

#elif debug

        /// <summary>
        /// Helper array to speedup conversion
        /// </summary>
        static string[] BATHS = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F", "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF", "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF", "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF", "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF", "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF", "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF" };

        /// <summary>
        /// Function converts byte array to it's hexadecimal implementation
        /// </summary>
        /// <param name="ArrayToConvert">Array to be converted</param>
        /// <param name="Delimiter">Delimiter to be inserted between bytes</param>
        /// <returns>String to represent given array</returns>
        static string ByteArrayToHexString(byte[] ArrayToConvert, string Delimiter)
        {
            int LengthRequired = (ArrayToConvert.Length + Delimiter.Length) * 2;
            System.Text.StringBuilder tempstr = new System.Text.StringBuilder(LengthRequired, LengthRequired);
            foreach (byte CurrentElem in ArrayToConvert)
            {
                tempstr.Append(BATHS[CurrentElem]);
                tempstr.Append(Delimiter);
            }

            return tempstr.ToString();
        }
#endif

        #region PInvokes

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
          IntPtr hProcess,
          IntPtr lpBaseAddress,
          [Out] byte[] lpBuffer,
          int dwSize,
          out int lpNumberOfBytesRead
         );

        #if !debug
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);
        #endif

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [Flags()]
        public enum ProcessAccess : int
        {
            /// <summary>Specifies all possible access flags for the process object.</summary>
            AllAccess = CreateThread | DuplicateHandle | QueryInformation | SetInformation | Terminate | VMOperation | VMRead | VMWrite | Synchronize,
            /// <summary>Enables usage of the process handle in the CreateRemoteThread function to create a thread in the process.</summary>
            CreateThread = 0x2,
            /// <summary>Enables usage of the process handle as either the source or target process in the DuplicateHandle function to duplicate a handle.</summary>
            DuplicateHandle = 0x40,
            /// <summary>Enables usage of the process handle in the GetExitCodeProcess and GetPriorityClass functions to read information from the process object.</summary>
            QueryInformation = 0x400,
            /// <summary>Enables usage of the process handle in the SetPriorityClass function to set the priority class of the process.</summary>
            SetInformation = 0x200,
            /// <summary>Enables usage of the process handle in the TerminateProcess function to terminate the process.</summary>
            Terminate = 0x1,
            /// <summary>Enables usage of the process handle in the VirtualProtectEx and WriteProcessMemory functions to modify the virtual memory of the process.</summary>
            VMOperation = 0x8,
            /// <summary>Enables usage of the process handle in the ReadProcessMemory function to' read from the virtual memory of the process.</summary>
            VMRead = 0x10,
            /// <summary>Enables usage of the process handle in the WriteProcessMemory function to write to the virtual memory of the process.</summary>
            VMWrite = 0x20,
            /// <summary>Enables usage of the process handle in any of the wait functions to wait for the process to terminate.</summary>
            Synchronize = 0x100000
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        public enum AllocationProtect : uint
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0x00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400
        }

        public enum State : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_FREE = 0x10000,
            MEM_RESERVE = 0x2000
        }

        public enum Type : uint
        {
            MEM_IMAGE = 0x1000000,
            MEM_MAPPED = 0x40000,
            MEM_PRIVATE = 0x20000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        static extern bool SetProcessWorkingSetSize(IntPtr hProcess, UIntPtr
           dwMinimumWorkingSetSize, UIntPtr dwMaximumWorkingSetSize);

        [DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            internal _PROCESSOR_INFO_UNION uProcessorInfo;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public UIntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct _PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)]
            internal uint dwOemId;
            [FieldOffset(0)]
            internal ushort wProcessorArchitecture;
            [FieldOffset(2)]
            internal ushort wReserved;
        }

#endregion
    }
}
