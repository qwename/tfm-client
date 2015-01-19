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

﻿using System;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Collections.Generic;
using ComponentAce.Compression.Libs.zlib;

namespace ABCParser
{
    using UI8 = Byte;
    using UI16 = UInt16;
    using UI30 = UInt32;
    using UI32 = UInt32;
    using SI32 = Int32;

    static class ABC_Tools
    {

        internal static byte[] DecompressSwf(byte[] swf)
        {
            byte[] header = new byte[8], content = new byte[swf.Length - 8];
            Buffer.BlockCopy(swf, 0, header, 0, 8);
            header[0] = 0x46;
            Buffer.BlockCopy(swf, 8, content, 0, swf.Length - 8);
            byte[] decompressed;
            using (System.IO.MemoryStream output = new System.IO.MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(output))
            using (System.IO.Stream InMemoryStream = new System.IO.MemoryStream(content))
            {
                CopyStream(InMemoryStream, outZStream);
                outZStream.finish();
                decompressed = output.ToArray();
            }
            byte[] final = new byte[header.Length + decompressed.Length];
            Buffer.BlockCopy(header, 0, final, 0, header.Length);
            Buffer.BlockCopy(decompressed, 0, final, header.Length, decompressed.Length);
            return final;
        }

        internal static byte[] CompressSwf(byte[] swf)
        {
            byte[] header = new byte[8], content = new byte[swf.Length - 8];
            Buffer.BlockCopy(swf, 0, header, 0, 8);
            header[0] = 0x43;
            Buffer.BlockCopy(swf, 8, content, 0, swf.Length - 8);
            byte[] compressed;
            using (System.IO.MemoryStream outMemoryStream = new System.IO.MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream, zlibConst.Z_DEFAULT_COMPRESSION))
            using (System.IO.Stream InMemoryStream = new System.IO.MemoryStream(content))
            {
                CopyStream(InMemoryStream, outZStream);
                outZStream.finish();
                compressed = outMemoryStream.ToArray();
            }
            byte[] final = new byte[header.Length + compressed.Length];
            Buffer.BlockCopy(header, 0, final, 0, header.Length);
            Buffer.BlockCopy(compressed, 0, final, header.Length, compressed.Length);
            return final;
        }

        internal static void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }

        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        internal static byte[] GetBytesFromFile(string path)
        {
            byte[] target;
            try
            {
                using (System.IO.FileStream fs = System.IO.File.OpenRead(path))
                {
                    target = new byte[fs.Length];
                    fs.Read(target, 0, (int)fs.Length);
                    fs.Close();
                }
            }
            catch (Exception)
            {
                target = null;
            }
            return target;
        }

        internal static bool Replace(ref byte[] ret, string search, string replace)
        {
            long index = 0;
            string s2 = replace.ToUpper().Replace(" ", "");
            if (search.IndexOf("??") == -1)
            {
                string s1 = search.ToUpper().Replace(" ", "");
                if ((index = IndexOf(ret, HexStringToByteArray(s1))) != -1)
                {
                    if (replace.IndexOf("??") == -1)
                    {
                        byte[] rep = HexStringToByteArray(s2);
                        Buffer.BlockCopy(rep, 0, ret, (int)index, rep.Length);
                    }
                    else
                    {
                        int i = 0;
                        while (i < s2.Length)
                        {
                            if (s2.Substring(i, 2) != "??")
                            {
                                ret[index + (i / 2)] = Convert.ToByte(s2.Substring(i, 2), 16);
                            }
                            i += 2;
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            else
            {
                int Current = 0, S_Current = 0;
                string[] s1 = search.ToUpper().Split(' ');
                while (Current < ret.Length)
                {
                    if (S_Current >= s1.Length)
                    {
                        break;
                    }
                    if (HEX[ret[Current]] == s1[S_Current] || s1[S_Current] == "??")
                    {
                        S_Current++;
                    }
                    else if (S_Current != 0)
                    {
                        S_Current = 0;
                        continue;
                    }
                    Current++;
                }
                if (S_Current >= s1.Length)
                {
                    Current -= s1.Length;
                    if (replace.IndexOf("??") == -1)
                    {
                        byte[] rep = HexStringToByteArray(s2);
                        Buffer.BlockCopy(rep, 0, ret, Current, rep.Length);
                    }
                    else
                    {
                        int i = 0;
                        while (i < s2.Length)
                        {
                            if (s2.Substring(i, 2) != "??")
                            {
                                ret[Current + (i / 2)] = Convert.ToByte(s2.Substring(i, 2), 16);
                            }
                            i += 2;
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
        }

        public static unsafe long IndexOf(byte[] Haystack, byte[] Needle)
        {
            fixed (byte* H = Haystack) fixed (byte* N = Needle)
            {
                long i = 0;
                for (byte* hNext = H, hEnd = H + Haystack.LongLength; hNext < hEnd; i++, hNext++)
                {
                    bool Found = true;
                    for (byte* hInc = hNext, nInc = N, nEnd = N + Needle.LongLength; Found && nInc < nEnd; Found = *nInc == *hInc, nInc++, hInc++) ;
                    if (Found) return i;
                }
                return -1;
            }
        }
        public static unsafe List<long> IndexesOf(byte[] Haystack, byte[] Needle)
        {
            List<long> Indexes = new List<long>();
            fixed (byte* H = Haystack) fixed (byte* N = Needle)
            {
                long i = 0;
                for (byte* hNext = H, hEnd = H + Haystack.LongLength; hNext < hEnd; i++, hNext++)
                {
                    bool Found = true;
                    for (byte* hInc = hNext, nInc = N, nEnd = N + Needle.LongLength; Found && nInc < nEnd; Found = *nInc == *hInc, nInc++, hInc++) ;
                    if (Found) Indexes.Add(i);
                }
                return Indexes;
            }
        }

        internal static byte[] StringToByteArray(string s)
        {
            byte[] ret = new byte[s.Length];
            int i = 0;
            foreach (char c in s)
            {
                ret[i++] = (byte)c;
            }
            return ret;
        }

        internal static string StringToHexString(string s)
        {
            string ret = "";
            foreach (char c in s)
            {
                ret += HEX[(int)c];
            }
            return ret;
        }

        /// <summary>
        /// Helper array to speedup conversion
        /// </summary>
        private static string[] HEX = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F", "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF", "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF", "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF", "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF", "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF", "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF" };

        /// <summary>
        /// Function converts byte array to it's hexadecimal implementation
        /// </summary>
        /// <param name="ArrayToConvert">Array to be converted</param>
        /// <param name="Delimiter">Delimiter to be inserted between bytes</param>
        /// <returns>String to represent given array</returns>
        internal static string ByteArrayToHexString(byte[] ArrayToConvert, string Delimiter)
        {
            int LengthRequired = (ArrayToConvert.Length + Delimiter.Length) * 2;
            System.Text.StringBuilder tempstr = new System.Text.StringBuilder(LengthRequired, LengthRequired);
            foreach (byte CurrentElem in ArrayToConvert)
            {
                tempstr.Append(HEX[CurrentElem]);
                tempstr.Append(Delimiter);
            }

            return tempstr.ToString();
        }

        internal static string ByteArrayToHexString(byte[] ArrayToConvert, uint startIndex = 0, uint ReadLength = 0)
        {
            int LengthRequired = ((ReadLength > 0 ? (int)ReadLength : ArrayToConvert.Length)) * 2, i = (int)startIndex;
            System.Text.StringBuilder tempstr = new System.Text.StringBuilder(LengthRequired, LengthRequired);
            while (i < (startIndex + ReadLength))
            {
                tempstr.Append(HEX[ArrayToConvert[i++]]);
            }
            return tempstr.ToString();
        }

        internal static string ByteArrayToString(byte[] b, int startIndex)
        {
            string ret = "";
            int i = startIndex;
            while (i < b.Length)
            {
                ret += (char)b[i++];
            }
            return ret;
        }

        internal static string ByteArrayToString(byte[] ArrayToConvert, uint startIndex, uint ReadLength)
        {
            int LengthRequired = (ReadLength > 0 ? (int)ReadLength : ArrayToConvert.Length), i = (int)startIndex;
            System.Text.StringBuilder tempstr = new System.Text.StringBuilder(LengthRequired, LengthRequired);
            while (i < (startIndex + ReadLength))
            {
                tempstr.Append((char)ArrayToConvert[i++]);
            }
            return tempstr.ToString();
        }

        internal static byte[] HexStringToByteArray(string s)
        {
            if (s.Length % 2 == 1)
                throw new Exception("The hex string is invalid.");
            byte[] ret = new byte[s.Length / 2];
            int i = 0;
            while (i < s.Length)
            {
                ret[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
                i += 2;
            }
            return ret;
        }

        internal static string ReadString(byte[] swf, uint startIndex, uint length)
        {
            return ABC_Tools.ByteArrayToString(swf, startIndex, length);
        }

        internal static string ReadNullTerminatedString(byte[] swf, uint startIndex)
        {
            int i = (int)startIndex;
            string tempstr = "";
            while (i < swf.Length && swf[i] != '\0')
            {
                tempstr += (char)swf[i++];
            }
            return tempstr;
        }

        internal static UI16 ReadUI16(byte[] swf, uint startIndex)
        {
            UI16 ui16Result = 0;

            ui16Result |= (UInt16)swf[startIndex];
            ui16Result |= (UInt16)(swf[startIndex+1] << 8);

            return ui16Result;
        }

        internal static UI32 ReadUI32(byte[] swf, uint startIndex)
        {
            UI32 ui32Result = 0;
            ui32Result |= (UI32)swf[startIndex];
            ui32Result |= (UI32)(swf[startIndex+1] << 8);
            ui32Result |= (UI32)(swf[startIndex+2] << 16);
            ui32Result |= (UI32)(swf[startIndex+3] << 24);
            return ui32Result;
        }

        internal static UI8 ReadUI8(byte[] swf, uint startIndex)
        {
            return (UI8)swf[startIndex];
        }

        internal static uint ReadUInt(byte[] swf, uint startIndex, bool little_endian = false)
        {
            if ((!BitConverter.IsLittleEndian && !little_endian) || (BitConverter.IsLittleEndian && !little_endian))
            {
                return (uint)BitConverter.ToInt32(swf, (int)startIndex);
            }
            else if ((BitConverter.IsLittleEndian && little_endian) || (!BitConverter.IsLittleEndian && little_endian))
            {
                return (uint)SwapUInt32((uint)BitConverter.ToInt32(swf, (int)startIndex));
            }
            return 0;
        }

	    internal static UInt16 SwapUInt16( UInt16 inValue )
	    {
		    return (UInt16)( ((inValue & 0xff00) >> 8) |
				     ((inValue & 0x00ff) << 8) );
	    }
 
	    internal static UInt32 SwapUInt32( UInt32 inValue )
	    {
		    return (UInt32)( ((inValue & 0xff000000) >> 24) |
				     ((inValue & 0x00ff0000) >> 8) |
				     ((inValue & 0x0000ff00) << 8) |
				     ((inValue & 0x000000ff) << 24) );
	    }

        internal static uint U32IntToU30Int(uint i)
        {
            int j = (int)i, k = 0, l = 0;
            while (true)
            {
                k |= (j & 0x7F) << l;
                if ((j >>= 7) == 0) return (uint)k;
                k |= 0x1 << ((l += 8) - 1);
            }
        }

        internal static UInt32 U30IntToU32Int(uint i)
        {
            int j = (int)i, k = 0, l = 0;
            while (true)
            {
                k |= (j & 0x7F) << l;
                if ((j & 0x80) != 0x80) return (uint)k;
                else if ((j >>= 8) == 0) return 0;
                else l += 7;
            }

        }
    }
}
