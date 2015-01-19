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
using System.Collections.Generic;

namespace ABCParser
{
    using CON = ActionBlockConstants;

    /// The variable-length encoding for u30, u32, and s32 uses one to five bytes, depending on the magnitude of the
    /// value encoded. Each byte contributes its low seven bits to the value. If the high (eighth) bit of a byte is set,
    /// then the next byte of the abcFile is also part of the value. In the case of s32, sign extension is applied: the
    /// seventh bit of the last byte of the encoding is propagated to fill out the 32 bits of the decoded value.
    using u8 = Byte;
    using u16 = UInt16;
    using u30 = UInt32;
    using u32 = UInt32;
    using s32 = Int32;
    using d64 = Double;

    /// The abcFile structure describes an executable code block with all its constant data, type descriptors, code, and
    /// metadata. It comprises the following fields.

    class abcFile
    {
        private byte[] abcBytes;
        private string s_log = "";
        private UInt32 startIndex = 0;
        private UInt32 abcFileLength = 0;
        private UInt32 Index = 0;
        /// The values of major_version and minor_version are the major and minor version numbers of the
        /// abcFile format. A change in the minor version number signifies a change in the file format that is
        /// backward compatible, in the sense that an implementation of the AVM2 can still make use of a file of an
        /// older version. A change in the major version number denotes an incompatible adjustment to the file
        /// format.
        private u16 minor_version = 0;
        private u16 major_version = 0;
        /// The constant_pool is a variable length structure composed of integers, doubles, strings, namespaces,
        /// namespace sets, and multinames. These constants are referenced from other parts of the abcFile
        /// structure.
        private cpool_info constant_pool;
        /// The value of method_count is the number of entries in the method array. Each entry in the method array
        /// is a variable length method_info structure. The array holds information about every method defined in
        /// this abcFile. The code for method bodies is held separately in the method_body array (see below).
        /// Some entries in method may have no body—this is the case for native methods, for example.
        private u30 method_count;
        private method_info[] _method;
        private u30 metadata_count;
        private metadata_info[] metadata;
        private u30 class_count;
        private instance_info[] instance;
        private class_info[] _class;
        private u30 script_count;
        private script_info[] script;
        private u30 method_body_count;
        private method_body_info[] method_body;

        public abcFile(byte[] bytes, uint offsetInSwf)
        {
            abcBytes = bytes;
            startIndex = offsetInSwf;
            abcFileLength = (uint)abcBytes.Length;
        }

        public string ParseABC()
        {
            minor_version = ReadU16();
            major_version = ReadU16();
            log("\nabcFile version\nminor : " + minor_version.ToString() + "\nmajor : " + major_version.ToString());
            ParseConstantPool();
            ParseMethodSignature();
            ParseMetadataInfo();
            ParseClasses();
            ParseMethodBody();
            /*
            int foundMethod = 0;
            byte[] foundCode = null;
            foreach (instance_info ins in instance)
            {
                if (constant_pool.multiname[ins.name].data.Length >= 2 &&
                    constant_pool._string[constant_pool.multiname[ins.name].data[1]].utf8 == "Ecimrofsnart")
                {
                    foundMethod = (int)ins.iinit;
                    break;
                }
            }
            foreach (method_body_info m in method_body)
            {
                if (m.method == foundMethod)
                {
                    foundCode = m.code;
                    log((m.max_scope_depth - m.init_scope_depth).ToString("X"));
                }
            }
             */
            return s_log;
        }

        public byte[] GetABCFile { get { return abcBytes; } }

        public UInt32 Length { get { return abcFileLength; } }

        public UInt32 Begin { get { return startIndex; } }

        #region Constant Pool

        private void ParseConstantPool()
        {
            log("\nConstant Pool Information : \n");
            int i = 1;
            constant_pool = new cpool_info();
            constant_pool.int_count = ReadU30();
            if (constant_pool.int_count > 0)
            {
                constant_pool._integer = new s32[constant_pool.int_count--];
                constant_pool._integer[0] = 0;
                while (i <= constant_pool.int_count) constant_pool._integer[i++] = ReadS32();
            }
            i = 1;
            log("Integer Count : " + constant_pool.int_count.ToString());
            constant_pool.uint_count = ReadU30();
            if (constant_pool.uint_count > 0)
            {
                constant_pool._uinteger = new u32[constant_pool.uint_count--];
                constant_pool._uinteger[0] = 0;
                while (i <= constant_pool.uint_count) constant_pool._uinteger[i++] = ReadU30();
            }
            i = 1;
            log("Unsigned Integer Count : " + constant_pool.uint_count.ToString());
            constant_pool.double_count = ReadU30();
            //Index += sizeof(Double);
            if (constant_pool.double_count > 0)
            {
                constant_pool._double = new d64[constant_pool.double_count--];
                constant_pool._double[0] = new d64();
                while (i <= constant_pool.double_count) constant_pool._double[i++] = ReadDouble();
            }
            i = 1;
            log("Double Count : " + constant_pool.double_count.ToString());
            constant_pool.string_count = ReadU30();
            if (constant_pool.string_count > 0)
            {
                constant_pool._string = new string_info[constant_pool.string_count--];
                constant_pool._string[0] = new string_info();
                while (i <= constant_pool.string_count) constant_pool._string[i++] = ReadString();
            }
            i = 1;
            log("\n(Parsing string pool, strings containing null characters(\\0) will be replaced a hexadecimal index padded with \"Q\" on the left, e.g. \"QQQQA6\")\n");
            log("String Count : " + constant_pool.string_count.ToString());
            constant_pool.namespace_count = ReadU30();
            if (constant_pool.namespace_count > 0)
            {
                constant_pool._namespace = new namespace_info[constant_pool.namespace_count--];
                constant_pool._namespace[0] = new namespace_info();
                while (i <= constant_pool.namespace_count) constant_pool._namespace[i++] = ReadNameSpace();
            }
            i = 1;
            log("Namespace Count : " + constant_pool.namespace_count.ToString());
            constant_pool.ns_set_count = ReadU30();
            if (constant_pool.ns_set_count > 0)
            {
                constant_pool.ns_set = new ns_set_info[constant_pool.ns_set_count--];
                constant_pool.ns_set[0] = new ns_set_info();
                while (i <= constant_pool.ns_set_count) constant_pool.ns_set[i++] = ReadNSSet();
                i = 1;
            }
            log("Namespace Set Count : " + constant_pool.ns_set_count.ToString());
            constant_pool.multiname_count = ReadU30();
            if (constant_pool.multiname_count > 0)
            {
                constant_pool.multiname = new multiname_info[constant_pool.multiname_count--];
                constant_pool.multiname[0] = new multiname_info();
                while (i <= constant_pool.multiname_count) constant_pool.multiname[i++] = ReadMultiname();
            }
            log("Multiname Count : " + constant_pool.multiname_count.ToString());
        }


        internal struct cpool_info
        {
            internal u30 int_count;
            internal s32[] _integer;
            internal u32 uint_count;
            internal u32[] _uinteger;
            internal u30 double_count;
            internal d64[] _double;
            internal u30 string_count;
            internal string_info[] _string;
            internal u30 namespace_count;
            internal namespace_info[] _namespace;
            internal u30 ns_set_count;
            internal ns_set_info[] ns_set;
            internal u30 multiname_count;
            internal multiname_info[] multiname;
        }

        internal struct string_info
        {
            internal u30 size;
            internal string utf8;
        }

        internal struct namespace_info
        {
            internal u8 kind;
            internal u30 name;
        }

        internal struct ns_set_info
        {
            internal u30 count;
            internal u30[] ns;
        }

        internal struct multiname_info
        {
            internal u8 kind;
            internal u8[] data;

            internal struct multiname_kind_QName
            {
                internal u30 ns;
                internal u30 name;
            }

            internal struct multiname_kind_RTQName
            {
                internal u30 name;
            }

            internal struct multiname_kind_RTQNameL
            {
            }

            internal struct multiname_kind_Multiname
            {
                internal u30 name;
                internal u30 ns_set;
            }

            internal struct multiname_kind_MultinameL
            {
                internal u30 ns_set;
            }

            internal struct multiname_kind_MultinameUnknown
            {
                internal u30 name;
                internal multiname_info _multiname_;
                internal u30 count;
                internal multiname_info[] types;
            }

        }

        private multiname_info ReadMultiname()
        {
            multiname_info ret = new multiname_info();
            ret.kind = ReadU8();
            uint orin = Index;
            switch (ret.kind)
            {
                case CON.CONSTANT_Qname:
                case CON.CONSTANT_QnameA:
                    multiname_info.multiname_kind_QName temp = new multiname_info.multiname_kind_QName();
                    temp.ns = ReadU30();
                    temp.name = ReadU30();
                    ret.data = new u8[(Index - orin)];
                    Buffer.BlockCopy(abcBytes, (int)orin, ret.data, 0, (int)(Index - orin));
                    break;
                case CON.CONSTANT_RTQname:
                case CON.CONSTANT_RTQnameA:
                    multiname_info.multiname_kind_RTQName temp2 = new multiname_info.multiname_kind_RTQName();
                    temp2.name = ReadU30();
                    ret.data = new u8[(Index - orin)];
                    Buffer.BlockCopy(abcBytes, (int)orin, ret.data, 0, (int)(Index - orin));
                    break;
                case CON.CONSTANT_RTQnameL:
                case CON.CONSTANT_RTQnameLA:
                    multiname_info.multiname_kind_RTQNameL temp3 = new multiname_info.multiname_kind_RTQNameL();
                    ret.data = null;
                    break;
                case CON.CONSTANT_Multiname:
                case CON.CONSTANT_MultinameA:
                    multiname_info.multiname_kind_Multiname temp4 = new multiname_info.multiname_kind_Multiname();
                    temp4.name = ReadU30();
                    temp4.ns_set = ReadU30();
                    ret.data = new u8[(Index - orin)];
                    Buffer.BlockCopy(abcBytes, (int)orin, ret.data, 0, (int)(Index - orin));
                    break;
                case CON.CONSTANT_MultinameL:
                case CON.CONSTANT_MultinameLA:
                    multiname_info.multiname_kind_MultinameL temp5 = new multiname_info.multiname_kind_MultinameL();
                    temp5.ns_set = ReadU30();
                    ret.data = new u8[(Index - orin)];
                    Buffer.BlockCopy(abcBytes, (int)orin, ret.data, 0, (int)(Index - orin));
                    break;
                case CON.CONSTANT_TypeName:
                    multiname_info.multiname_kind_MultinameUnknown temp6 = new multiname_info.multiname_kind_MultinameUnknown();
                    temp6.name = ReadU30();
                    temp6._multiname_ = constant_pool.multiname[temp6.name];
                    temp6.count = ReadU30();
                    temp6.types = new multiname_info[temp6.count];
                    int j = 0;
                    while (j < temp6.count) temp6.types[j++] = constant_pool.multiname[ReadU30()];
                    ret.data = new u8[(Index - orin)];
                    Buffer.BlockCopy(abcBytes, (int)orin, ret.data, 0, (int)(Index - orin));
                    break;
            }
            return ret;
        }


        private ns_set_info ReadNSSet()
        {
            ns_set_info ret = new ns_set_info();
            ret.count = ReadU30();
            if (ret.count > 0)
            {
                ret.ns = new u30[ret.count];
                int i = 0;
                while (i < ret.count) ret.ns[i++] = ReadU30();
            }
            return ret;
        }

        private namespace_info ReadNameSpace()
        {
            namespace_info ret = new namespace_info();
            ret.kind = ReadU8();
            //if (ret.kind == 0x05) abcBytes[Index - 1] = 0x16; //Make everything public
            ret.name = ReadU30();
            return ret;
        }

        #endregion

        #region Method Signature

        private void ParseMethodSignature()
        {
            log("\nMethod Signature Information: \n");
            int i = 0;
            method_count = ReadU30();
            if (method_count > 0)
            {
                _method = new method_info[method_count];
                while (i < method_count) _method[i++] = ReadMethodInfo();
            }
            log("Method Count : " + method_count.ToString());
        }

        private method_info ReadMethodInfo()
        {
            int j = 0;
            method_info ret = new method_info();
            ret.param_count = ReadU30();
            ret.return_type = ReadU30();
            if (ret.param_count > 0)
            {
                ret._param_type = new u30[ret.param_count];
                while (j < ret.param_count) ret._param_type[j++] = ReadU30();
            }
            j = 0;
            ret.name = ReadU30();
            ret.flags = ReadU8();
            if ((ret.flags & CON.METHOD_HasOptional) == CON.METHOD_HasOptional)
            {
                ret.options = new method_info.option_info();
                ret.options.option_count = ReadU30();
                ret.options.option = new method_info.option_info.option_detail[ret.options.option_count];
                while (j < ret.options.option_count) ret.options.option[j++] = ReadOptionInfo();
            }
            j = 0;
            if ((ret.flags & CON.METHOD_HasParamNames) == CON.METHOD_HasParamNames)
            {
                ret.param_names = new method_info.param_info();
                ret.param_names.param_name = new u30[ret.param_count];
                while (j < ret.param_count) ret.param_names.param_name[j++] = ReadU30();
            }
            return ret;
        }

        internal method_info.option_info.option_detail ReadOptionInfo()
        {
            method_info.option_info.option_detail ret = new method_info.option_info.option_detail();
            ret.val = ReadU30();
            ret.kind = ReadU8();
            return ret;
        }

        internal struct method_info
        {
            internal u30 param_count;
            internal u30 return_type;
            internal u30[] _param_type;
            internal u30 name;
            internal u8 flags;
            internal option_info options;
            internal param_info param_names;

            internal struct option_info
            {
                internal u30 option_count;
                internal option_detail[] option;

                internal struct option_detail
                {
                    internal u30 val;
                    internal u8 kind;
                }
            }

            internal struct param_info
            {
                /// The param_names entry is available only when the HAS_PARAM_NAMES bit is set in the flags. Each param_info
                /// element of the array is an index into the constant pool’s string array. The parameter name entry exists solely
                /// for external tool use and is not used by the AVM2.
                internal u30[] param_name;
            }
        }

        #endregion

        #region metadata_info

        internal void ParseMetadataInfo()
        {
            log("\nmetadata_info Information : \n");
            int i = 0;
            metadata_count = ReadU30();
            if (metadata_count > 0)
            {
                metadata = new metadata_info[metadata_count];
                while (i < metadata_count) metadata[i++] = ReadMetadataInfo();
            }
            log("metadata_info Count : " + metadata_count.ToString());
        }

        internal metadata_info ReadMetadataInfo()
        {
            metadata_info ret = new metadata_info();
            ret.name = ReadU30();
            ret.item_count = ReadU30();
            if (ret.item_count > 0)
            {
                int i = 0;
                ret.keys = new u30[ret.item_count];
                while (i < ret.item_count) ret.keys[i++] = ReadU30();
                i = 0;
                ret.items = new u30[ret.item_count];
                while (i < ret.item_count) ret.items[i++] = ReadU30();
            }
            return ret;
        }

        internal struct metadata_info
        {
            /// The name field is an index into the string array of the constant pool; it provides a name for the metadata
            /// entry. The value of the name field must not be zero. Zero or more items may be associated with the entry;
            /// item_count denotes the number of items that follow in the items array.
            internal u30 name;
            internal u30 item_count;
            internal u30[] keys;
            internal u30[] items;

            /*
             * ADOBE AVM2 OVERVIEW PDF ERRATA
            internal struct item_info
            {
                /// The item_info entry consists of item_count elements that are interpreted as key/value pairs of indices into the
                /// string table of the constant pool. If the value of key is zero, this is a keyless entry and only carries a value.
                internal u30 key;
                internal u30 value;
            }
             * */
        }

        #endregion

        #region Classes

        internal void ParseClasses()
        {
            log("\nClass Information : \n");
            int i = 0;
            class_count = ReadU30();
            if (class_count > 0)
            {
                instance = new instance_info[class_count];
                while (i < class_count) instance[i++] = ReadInstanceInfo();
                i = 0;
                _class = new class_info[class_count];
                while (i < class_count) _class[i++] = ReadClassInfo();
            }
            i = 0;
            log("Class Count : " + class_count.ToString());
            script_count = ReadU30();
            if (script_count > 0)
            {
                script = new script_info[script_count];
                while (i < script_count) script[i++] = ReadScriptInfo();
            }
            i = 0;
            log("Script Count : " + script_count.ToString());
        }

        #region Instance Info

        internal instance_info ReadInstanceInfo()
        {
            instance_info ret = new instance_info();
            int i = 0;
            ret.name = ReadU30();
            ret.super_name = ReadU30();
            ret.flags = ReadU8();
            if ((ret.flags & CON.CLASS_FLAG_protected) == CON.CLASS_FLAG_protected)
            {
                ret.protectedNs = ReadU30();
            }
            ret.intrf_count = ReadU30();
            if (ret.intrf_count > 0)
            {
                ret._interface = new u30[ret.intrf_count];
                while (i < ret.intrf_count) ret._interface[i++] = ReadU30();
            }
            i = 0;
            ret.iinit = ReadU30();
            ret.trait_count = ReadU30();
            if (ret.trait_count > 0)
            {
                ret.trait = new traits_info[ret.trait_count];
                while (i < ret.trait_count) ret.trait[i++] = ReadTraitInfo();
            }
            i = 0;
            return ret;
        }

        internal struct instance_info
        {
            internal u30 name;
            internal u30 super_name;
            internal u8 flags;
            internal u30 protectedNs;
            internal u30 intrf_count;
            internal u30[] _interface;
            internal u30 iinit;
            internal u30 trait_count;
            internal traits_info[] trait;
        }

        #endregion

        #region Class Info

        internal class_info ReadClassInfo()
        {
            class_info ret = new class_info();
            ret.cinit = ReadU30();
            ret.trait_count = ReadU30();
            if (ret.trait_count > 0)
            {
                ret.traits = new traits_info[ret.trait_count];
                int i = 0;
                while (i < ret.trait_count) ret.traits[i++] = ReadTraitInfo();
            }
            return ret;
        }

        internal struct class_info
        {
            internal u30 cinit;
            internal u30 trait_count;
            internal traits_info[] traits;
        }

        #endregion

        #region Script Info

        internal script_info ReadScriptInfo()
        {
            script_info ret = new script_info();
            ret.init = ReadU30();
            ret.trait_count = ReadU30();
            if (ret.trait_count > 0)
            {
                ret.trait = new traits_info[ret.trait_count];
                int i = 0;
                while (i < ret.trait_count) ret.trait[i++] = ReadTraitInfo();
            }
            return ret;
        }

        internal struct script_info
        {
            internal u30 init;
            internal u30 trait_count;
            internal traits_info[] trait;
        }

        #endregion


        #endregion

        #region Method Body Info

        internal void ParseMethodBody()
        {
            log("\nMethod Body Information : \n");
            int i = 0;
            method_body_count = ReadU30();
            if (method_body_count > 0)
            {
                method_body = new method_body_info[method_body_count];
                while (i < method_body_count) method_body[i++] = ReadMethodBodyInfo();
            }
            i = 0;
            log("Method Body Count : " + method_body_count.ToString());
        }

        private method_body_info ReadMethodBodyInfo()
        {
            method_body_info ret = new method_body_info();
            int i = 0;
            ret.method = ReadU30();
            ret.max_stack = ReadU30();
            ret.local_count = ReadU30();
            ret.init_scope_depth = ReadU30();
            ret.max_scope_depth = ReadU30();
            ret.code_length = ReadU30();
            if (ret.code_length > 0)
            {
                ret.code = new u8[ret.code_length];
                while (i < ret.code_length) ret.code[i++] = ReadU8();
            }
            i = 0;
            ret.exception_count = ReadU30();
            if (ret.exception_count > 0)
            {
                ret.exception = new method_body_info.exception_info[ret.exception_count];
                while (i < ret.exception_count) ret.exception[i++] = ReadExceptionInfo();
            }
            i = 0;
            ret.trait_count = ReadU30();
            if (ret.trait_count > 0)
            {
                ret.trait = new traits_info[ret.trait_count];
                while (i < ret.trait_count) ret.trait[i++] = ReadTraitInfo();
            }
            return ret;
        }

        private method_body_info.exception_info ReadExceptionInfo()
        {
            method_body_info.exception_info ret = new method_body_info.exception_info();
            ret.from = ReadU30();
            ret.to = ReadU30();
            ret.target = ReadU30();
            ret.exc_type = ReadU30();
            ret.var_name = ReadU30();
            return ret;
        }

        internal struct method_body_info
        {
            internal u30 method;
            internal u30 max_stack;
            internal u30 local_count;
            internal u30 init_scope_depth;
            internal u30 max_scope_depth;
            internal u30 code_length;
            internal u8[] code;
            internal u30 exception_count;
            internal exception_info[] exception;
            internal u30 trait_count;
            internal traits_info[] trait;

            internal struct exception_info
            {
                internal u30 from;
                internal u30 to;
                internal u30 target;
                internal u30 exc_type;
                internal u30 var_name;
            }
        }

        #endregion


        #region Traits Info

        internal traits_info ReadTraitInfo()
        {
            traits_info ret = new traits_info();
            ret.name = ReadU30();
            ret.kind = ReadU8();
            uint orin = Index;
            switch (ret.kind & 0xF)
            {
                case CON.TRAIT_Var:
                case CON.TRAIT_Const:
                    traits_info.trait_slot temp = new traits_info.trait_slot();
                    temp.slot_id = ReadU30();
                    temp.type_name = ReadU30();
                    temp.vindex = ReadU30();
                    if (temp.vindex != 0) temp.vkind = ReadU8();
                    break;
                case CON.TRAIT_Class:
                    traits_info.trait_class temp2 = new traits_info.trait_class();
                    temp2.slot_id = ReadU30();
                    temp2.classi = ReadU30();
                    break;
                case CON.TRAIT_Function:
                    traits_info.trait_function temp3 = new traits_info.trait_function();
                    temp3.slot_id = ReadU30();
                    temp3.function = ReadU30();
                    break;
                case CON.TRAIT_Method:
                case CON.TRAIT_Getter:
                case CON.TRAIT_Setter:
                    traits_info.trait_method temp4 = new traits_info.trait_method();
                    temp4.disp_id = ReadU30();
                    temp4.method = ReadU30();
                    break;
            }
            ret.data = new u8[(Index - orin)];
            Buffer.BlockCopy(abcBytes, (int)orin, ret.data, 0, (int)(Index - orin));
            if ((ret.kind & (CON.TRAIT_FLAG_metadata << 4)) == (CON.TRAIT_FLAG_metadata << 4))
            {
                ret.metadata_count = ReadU30();
                if (ret.metadata_count > 0)
                {
                    int i = 0;
                    ret.metadata = new u30[ret.metadata_count];
                    while (i < ret.metadata_count) ret.metadata[i++] = ReadU30();
                }
            }
            return ret;
        }

        internal struct traits_info
        {
            /// The name field is an index into the multiname array of the constant pool; it provides a name for the trait.
            /// The value can not be zero, and the multiname entry specified must be a QName.
            internal u30 name;
            /// The kind field contains two four-bit fields. The lower four bits determine the kind of this trait. The
            /// upper four bits comprise a bit vector providing attributes of the trait. See the following tables and
            /// sections for full descriptions.
            internal u8 kind;
            /// The interpretation of the data field depends on the type of the trait, which is provided by the low four
            /// bits of the kind field. See below for a full description.
            internal u8[] data;
            /// These fields are present only if ATTR_Metadata is present in the upper four bits of the kind field.
            /// The value of the metadata_count field is the number of entries in the metadata array. That array
            /// contains indices into the metadata array of the abcFile.
            internal u30 metadata_count;
            internal u30[] metadata;

            internal struct trait_slot
            {
                /// The slot_id field is an integer from 0 to N and is used to identify a position in which this trait resides. A
                /// value of 0 requests the AVM2 to assign a position.
                internal u30 slot_id;
                /// This field is used to identify the type of the trait. It is an index into the multiname array of the
                /// constant_pool. A value of zero indicates that the type is the any type (*).
                internal u30 type_name;
                /// This field is an index that is used in conjunction with the vkind field in order to define a value for the
                /// trait. If it is 0, vkind is empty; otherwise it references one of the tables in the constant pool, depending on
                /// the value of vkind.
                internal u30 vindex;
                /// This field exists only when vindex is non-zero. It is used to determine how vindex will be interpreted.
                /// See the “Constant Kind” table above for details.
                internal u8 vkind;
            }

            internal struct trait_class
            {
                /// The slot_id field is an integer from 0 to N and is used to identify a position in which this trait resides. A
                /// value of 0 requests the AVM2 to assign a position.
                internal u30 slot_id;
                /// The classi field is an index that points into the class array of the abcFile entry.
                internal u30 classi;
            }

            internal struct trait_function
            {
                /// The slot_id field is an integer from 0 to N and is used to identify a position in which this trait resides.
                /// A value of 0 requests the AVM2 to assign a position.
                internal u30 slot_id;
                /// The function field is an index that points into the method array of the abcFile entry.
                internal u30 function;
            }

            internal struct trait_method
            {
                /// The disp_id field is a compiler assigned integer that is used by the AVM2 to optimize the resolution of
                /// virtual function calls. An overridden method must have the same disp_id as that of the method in the
                /// base class. A value of zero disables this optimization.
                internal u30 disp_id;
                /// The method field is an index that points into the method array of the abcFile entry.
                internal u30 method;
            }
        }

        #endregion

        #region Methods

        internal UInt32 GetIndex { get { return (startIndex + Index); } }

        private int weirdName = 0;

        private string_info ReadString()
        {
            string_info ret = new string_info();
            ret.size = ReadU30();
            if (ret.size > 0)
            {
                //ret.utf8 = ABC_Tools.ByteArrayToString(abcBytes, Index, ret.size).Replace('\0', ' ');
                ret.utf8 = ABC_Tools.ByteArrayToString(abcBytes, Index, ret.size);
                if (ret.utf8.Contains("\0"))
                {
                    string replace = weirdName++.ToString("X");
                    int i = 0, limit = (int)ret.size - replace.Length;
                    while (i++ < limit)
                    {
                        abcBytes[Index++] = 0x51;//"Q"
                    }
                    foreach (char c in replace)
                        abcBytes[Index++] = (byte)c;
                }
                else
                    Index += ret.size;
            }
            else ret.utf8 = "";
            return ret;
        }

        private d64 ReadDouble()
        {
            d64 ret = BitConverter.ToDouble(abcBytes, (int)Index);
            Index += 8;
            return ret;
        }

        private u8 ReadU8()
        {
            return abcBytes[Index++];
        }

        private u16 ReadU16()
        {
            u16 ret = ABC_Tools.ReadUI16(abcBytes, Index);
            Index += 2;
            return ret;
        }

        private u30 ReadU30()
        {
            int shift = 0;
            u30 ui30Result = (u30)(abcBytes[Index] & 0x7F);
            while ((abcBytes[Index++] & 0x80) != 0 || (abcBytes[Index - 1] == 0x80))
            {
                ui30Result |= (u30)((abcBytes[Index] & 0x7F) << (shift += 7));
            }
            return ui30Result;
        }

        private s32 ReadS32()
        {
            int shift = 0;
            u30 si32Result = (u30)abcBytes[Index] & 0x7F;
            while ((abcBytes[Index++] & 0x80) != 0)
            {
                si32Result |= (u30)((abcBytes[Index] & 0x7F) << (shift += 7));
            }
            return (s32)si32Result;
        }

        private void log(string s)
        {
            s_log += s + '\n';
        }

        #endregion
    }
}
