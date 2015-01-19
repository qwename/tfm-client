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
using System.Collections.Generic;

namespace ABCParser
{
    using UI8 = Byte;
    using UI16 = UInt16;
    using UI32 = UInt32;

    class SwfStruct
    {
        private TFM_Client.TFM m;
        public byte[] newSwf;
        private byte[] v_swf;
        private FileHeader Header;
        private abcFile ABC;
        private UInt32 currentIndex = 0;
        private Tag[] FoundTags;

        public SwfStruct(byte[] swf, TFM_Client.TFM t)
        {
            m = t;
            v_swf = swf;
            Start();
        }

        public SwfStruct(string swfPath, TFM_Client.TFM t)
        {
            m = t;
            v_swf = ABC_Tools.GetBytesFromFile(swfPath);
            Start();
        }

        public byte[] GetSwf { get { return newSwf; } }
        public Tag[] GetTags { get { return FoundTags; } }


        private void Start()
        {
            string temp_check = ABC_Tools.ByteArrayToString(v_swf, 0, 3);
            if (temp_check != "CWS" && temp_check != "FWS")
            {
                m.update("Invalid swf file detected.");
                return;
            }
            ParseHeader();
            ParseSwf();
            //DirectoryInfo dir = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + "/" + "modified");
            //if (!dir.Exists) dir.Create();
        }

        public void Delete()
        {
            v_swf = null;
            FoundTags = null;
            Header = null;
            ABC = null;
        }

        private void ParseHeader()
        {
            m.update("Header Information : \n");
            Header = new FileHeader(v_swf, currentIndex);
            currentIndex += 21;
            Object[] obj = Header.ParseHeader();
            v_swf = (byte[])obj[0];
            m.update((string)obj[1]);
            obj = null;
        }

        private void ParseSwf()
        {
            string mainClassName = "";
            ParseTags(v_swf);
            foreach (Tag t in FoundTags)
            {
                if (t.Header.TagType == 82) ParseABC(v_swf, t);
                if (t.Header.TagType == 76) mainClassName = ParseSymbolClass(v_swf, t);
            }
            UpdateSwf();
            /*
            m.update("Saving file as " + mainClassName + ".swf...");
            File.WriteAllBytes(mainClassName + ".swf", newSwf);
            m.update(mainClassName + ".swf saved to folder.");
            */
        }

        private string ParseSymbolClass(byte[] s, Tag t)
        {
            int index = (int)(t.Offset + t.Header.TagLength - t.Header.Length), numSymbols = (int)ABC_Tools.ReadUI16(s, (uint)index);
            index += 2;
            while (ABC_Tools.ReadUI16(s, (uint)index) != 0)
            {
                index += 2;
                index += ABC_Tools.ReadNullTerminatedString(s, (uint)(index)).Length + 1;
            }
            index += 2;
            return ABC_Tools.ReadNullTerminatedString(s, (uint)index);
        }

        private void UpdateSwf()
        {
            newSwf = new byte[v_swf.Length];
            Buffer.BlockCopy(v_swf, 0, newSwf, 0, (int)ABC.Begin);
            Buffer.BlockCopy(ABC.GetABCFile, 0, newSwf, (int)ABC.Begin, (int)ABC.Length);
            Buffer.BlockCopy(v_swf, (int)(ABC.Begin + ABC.Length), newSwf, (int)(ABC.Begin + ABC.Length), (int)(v_swf.Length - (ABC.Begin + ABC.Length)));
        }

        private void ParseTags(byte[] swf)
        {
            m.update("\nTag Information : (Offset TagType TagTypeName Name Length)\n");
            int i = 0;
            List<Tag> tempTag = new List<Tag>();
            while (currentIndex < v_swf.Length)
            {
                Tag temp = new Tag();
                temp.ID = (uint)((i++) + 1);
                temp.Offset = currentIndex;
                temp.Header = new RECORDHEADER(swf, currentIndex);
                if (Tags.ContainsKey(temp.Header.TagType))
                {
                    m.update("0x" + currentIndex.ToString("X").PadLeft(8, '0')  + "\t" + temp.Header.TagType.ToString() + "\t" + Tags[temp.Header.TagType] + "\t" + temp.Header.Length.ToString());
                }
                else
                {
                    m.update("0x" + currentIndex.ToString("X").PadLeft(8, '0') + "\t" + temp.Header.TagType.ToString() + "\tUnknown\t" + temp.Header.Length.ToString());
                }
                currentIndex += temp.Header.TagLength;
                tempTag.Add(temp);
            }
            FoundTags = tempTag.ToArray();
            m.update(FoundTags.Length.ToString() + " tags found\n");
        }

        private void ParseABC(byte[] swf, Tag t)
        {
            m.update("\nParsing Tag ID " + t.ID.ToString() + " at 0x" + t.Offset.ToString("X").PadLeft(8, '0') + " : \n");
            UI8 kDoAbcLazyInitializeFlag = (UI8)ABC_Tools.ReadUI32(swf, t.Offset + 6);
            m.update("kDoAbcLazyInitializeFlag : " + ((kDoAbcLazyInitializeFlag == 1) ? "Set" : "Not Set"));
            t.Name = ABC_Tools.ReadNullTerminatedString(swf, t.Offset + 10);
            m.update("Bytecode name : " + ((t.Name == "") ? "No Name" : t.Name));
            byte[] ABCFILE = new byte[t.Header.Length - 4];
            Buffer.BlockCopy(v_swf, (int)t.Offset + 11 + t.Name.Length, ABCFILE, 0, (int)t.Header.Length - 4);
            ABC = new abcFile(ABCFILE, (uint)(t.Offset + 11 + t.Name.Length));
            t.Offset += (uint)(7 + t.Name.Length + t.Header.Length);
            ABCFILE = null;
            m.update(ABC.ParseABC()) ;
        }

        #region Swf File Header
        internal class FileHeader
        {
            private byte[] s_swf;
            private string s_signature = "";
            private UI8 s_version = 0;
            private UI32 s_length = 0;
            private RECT s_framesize = null;
            private UI16 s_framerate = 0;
            private UI16 s_framecount = 0;
            private UI32 Index = 0;
            private string s_log = "";

            internal string Signature { get { return s_signature; } }

            internal UI8 Version { get { return s_version; } }

            internal UI32 FileLength { get { return s_length; } }

            internal RECT FrameSize { get { return s_framesize; } }

            internal UI16 FrameRate { get { return s_framerate; } }

            internal UI16 FrameCount { get { return s_framecount; } }

            public FileHeader(byte[] swf, uint startIndex)
            {
                Index = startIndex;
                s_swf = swf;
            }

            private void log(string s)
            {
                s_log += s + '\n';
            }

            internal Object[] ParseHeader()
            {
                ParseSignature(s_swf);
                ParseVersion(s_swf);
                ParseLength(s_swf);
                ParseFrameSize(s_swf);
                ParseFrameRate(s_swf);
                ParseFrameCount(s_swf);
                return new Object[]{s_swf, s_log};
            }

            private void ParseSignature(byte[] swf)
            {
                s_signature = ABC_Tools.ReadString(swf, Index, 3);
                Index += 3;
                if (s_signature == "CWS")
                {
                    log("Compressed swf file detected, decompressing file...");
                    s_swf = ABC_Tools.DecompressSwf(s_swf);
                    log("Swf file decompressed.");
                }
                else if (s_signature == "FWS")
                {
                    log("Uncompressed swf file detected, no changes will be made.");
                }
            }

            private void ParseVersion(byte[] swf)
            {
                s_version = ABC_Tools.ReadUI8(swf, Index);
                Index++;
                log("Flash player version : " + s_version.ToString());
            }

            private void ParseLength(byte[] swf)
            {
                s_length = ABC_Tools.ReadUI32(swf, Index);
                Index += 4;
                log("File length : " + s_length.ToString());
            }

            private void ParseFrameSize(byte[] swf)
            {
                s_framesize = new RECT(swf, Index);
                Index += 9;
                log("Frame width(pixels) : " + s_framesize.WidthInPixels.ToString());
                log("Frame height(pixels) : " + s_framesize.HeightInPixels.ToString());
            }

            private void ParseFrameRate(byte[] swf)
            {
                s_framerate = ABC_Tools.ReadUI8(swf, Index + 1);
                Index += 2;
                log("Frame rate : " + s_framerate.ToString());
            }

            private void ParseFrameCount(byte[] swf)
            {
                s_framecount = ABC_Tools.ReadUI16(swf, Index);
                Index += 2;
                log("Frame count : " + s_framecount.ToString());
            }

            internal class RECT
            {
                byte m_byNbits = 0;
                int m_Xmin = 0;
                int m_Xmax = 0;
                int m_Ymin = 0;
                int m_Ymax = 0;
                public RECT(byte[] swf, uint startIndex)
                {
                    int nBitcount = 0, nCurrentValue = 0, nCurrentBit = 2, Index = (int)startIndex;
                    byte byTemp = ABC_Tools.ReadUI8(swf, (uint)Index++);
                    m_byNbits = (byte)((int)byTemp >> 3);
                    byTemp &= 7;
                    byTemp <<= 5;
                    for (int nIndex = 0; nIndex < 4; nIndex++)
                    {
                        while (nBitcount < m_byNbits)
                        {
                            if ((byTemp & 128) == 128)
                            {
                                nCurrentValue += 1 << (m_byNbits - nBitcount - 1);
                            }
                            byTemp <<= 1;
                            byTemp &= 255;
                            nCurrentBit--;
                            nBitcount++;
                            if (nCurrentBit < 0)
                            {
                                byTemp = ABC_Tools.ReadUI8(swf, (uint)Index++);
                                nCurrentBit = 7;
                            }
                        }

                        switch (nIndex)
                        {
                            case 0:
                                m_Xmin = nCurrentValue;
                                break;
                            case 1:
                                m_Xmax = nCurrentValue;
                                break;
                            case 2:
                                m_Ymin = nCurrentValue;
                                break;
                            case 3:
                                m_Ymax = nCurrentValue;
                                break;
                        }

                        nBitcount = 0;
                        nCurrentValue = 0;
                    }

                }

                public int XMinInTwips
                {
                    get { return m_Xmin; }
                }

                public int XMinInPixels
                {
                    get { return XMinInTwips / 20; }
                }

                public int XMaxInTwips
                {
                    get { return m_Xmax; }
                }

                public int XMaxInPixels
                {
                    get { return XMaxInTwips / 20; }
                }

                public int YMinInTwips
                {
                    get { return m_Ymin; }
                }

                public int YMinInPixels
                {
                    get { return YMinInTwips / 20; }
                }

                public int YMaxInTwips
                {
                    get { return m_Ymax; }
                }

                public int YMaxInPixels
                {
                    get { return YMaxInTwips / 20; }
                }

                public int WidthInTwips
                {
                    get { return m_Xmax - m_Xmin; }
                }

                public int WidthInPixels
                {
                    get { return WidthInTwips / 20; }
                }

                public int HeightInTwips
                {
                    get { return m_Ymax - m_Ymin; }
                }

                public int HeightInPixels
                {
                    get { return HeightInTwips / 20; }
                }
            }
        }

        #endregion

        #region Swf Tags

        internal struct Tag
        {
            internal UInt32 ID;
            internal RECORDHEADER Header;
            internal UInt32 Offset;
            internal string Name;
        }

        internal class RECORDHEADER
        {
            private UI8 r_type = 0;
            private UI32 r_length = 0;
            private UI32 r_taglength = 0;

            public RECORDHEADER(byte[] swf, uint startIndex)
            {
                UI16 Header = ABC_Tools.ReadUI16(swf, startIndex);
                r_type = (UI8)(Header >> 6);
                if ((Header & 0x3F) == 0x3F)
                {
                    r_length = ABC_Tools.ReadUI32(swf, startIndex + 2);
                    r_taglength = r_length + 6;
                }
                else
                {
                    r_length = (UI32)(Header & 0x3F);
                    r_taglength = r_length + 2;
                }
            }

            public UI32 Length { get { return r_length; } }

            public UI32 TagLength { get { return r_taglength; } }

            public UI8 TagType { get { return r_type; } }
        }

        private Dictionary<int, string> Tags = new Dictionary<int, string>()
        {
            {0, "End"},
            {1, "ShowFrame"},
            {2, "DefineShape"},
            {3, "FreeCharacter"},
            {4, "PlaceObject"},
            {5, "RemoveObject"},
            {6, "DefineBits"},
            {7, "DefineButton"},
            {8, "JPEGTables"},
            {9, "SetBackgroundColor"},
            {10, "DefineFont"},
            {11, "DefineText"},
            {12, "DoAction"},
            {13, "DefineFontInfo"},
            {14, "DefineSound"},
            {15, "StartSound"},
            {16, "StopSound"},
            {17, "DefineButtonSound"},
            {18, "SoundStreamHead"},
            {19, "SoundStreamBlock"},
            {20, "DefineBitsLossless"},
            {21, "DefineBitsJPEG2"},
            {22, "DefineShape2"},
            {23, "DefineButtonCxform"},
            {24, "Protect"},
            {25, "PathsArePostScript"},
            {26, "PlaceObject2"},
            {27, "Unknown"}, // UNKNOWN
            {28, "RemoveObject2"},
            {29, "SyncFrame"}, // OBSOLETE
            {30, "Unknown"}, // UNKNOWN
            {31, "FreeAll"}, // OBSOLETE
            {32, "DefineShape3"},
            {33, "DefineText2"},
            {34, "DefineButton2"},
            {35, "DefineBitsJPEG3"},
            {36, "DefineBitsLossless2"},
            {37, "DefineEditText"},
            {38, "DefineMouseTarget"}, // OBSOLETE
            {39, "DefineSprite"},
            {40, "NameCharacter"}, // OBSOLETE
            //{41, "NameObject"}, // OBSOLETE
            {41, "ProductInfo"},
            {42, "DefineTextFormat"}, // OBSOLETE
            {43, "FrameLabel"},
            {44, "DefineButton2"}, // UNUSED See 34, DefineBehavior for Flash 5
            {45, "SoundStreamHead2"},
            {46, "DefineMorphShape"},
            {47, "FrameTag"}, // OBSOLETE
            {48, "DefineFont2"},
            {49, "GenCommand"}, // OBSOLETE
            {50, "DefineCommandObj"}, // OBSOLETE
            {51, "CharacterSet"}, // OBSOLETE
            {52, "FontRef"}, // OBSOLETE
            {53, "DefineFunction"}, // OBSOLETE
            {54, "PlaceFunction"}, // OBSOLETE
            {55, "GenTagObject	"}, // OBSOLETE
            {56, "ExportAssets"},
            {57, "ImportAssets"},
            {58, "EnableDebugger"},
            {59, "DoInitAction"},
            {60, "DefineVideoStream"},
            {61, "VideoFrame"},
            {62, "DefineFontInfo2"},
            {63, "DebugID"},
            {64, "EnableDebugger2"},
            {65, "ScriptLimits"},
            {66, "SetTabIndex"},
            {67, "DefineShape4"}, // OBSOLETE, Use 83
            {68, "Unknown"}, // UNKNOWN
            {69, "FileAttributes"},
            {70, "PlaceObject3"},
            {71, "ImportAssets2"},
            {72, "DoABC"}, // UNKNOWN
            {73, "DefineFontAlignZones"},
            {74, "CSMTextSettings"},
            {75, "DefineFont3"},
            {76, "SymbolClass"},
            {77, "Metadata"},
            {78, "DefineScalingGrid"},
            {79, "Unknown"}, // UNKNOWN
            {80, "Unknown"}, // UNKNOWN
            {81, "Unknown"}, // UNKNOWN
            {82, "DoABC"}, // DoABC2
            {83, "DefineShape4"},
            {84, "DefineMorphShape2"},
            {86, "DefineSceneAndFrameLabelData"},
            {87, "DefineBinaryData"},
            {88, "DefineFontName"},
            {89, "StartSound2"},
            {90, "DefineBitsJPEG4"},
            {91, "DefineFont4"}
        };

    }
}

        #endregion