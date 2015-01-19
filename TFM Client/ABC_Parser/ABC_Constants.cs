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

﻿namespace ABCParser
{

    public class ActionBlockConstants
    {
	    public const int MINORwithDECIMAL = 17;
        /*
         * Constant pool tags
         */

        public const byte CONSTANT_Void = 0x00;  // not actually interned
        public const byte CONSTANT_Utf8 = 0x01;
        public const byte CONSTANT_Decimal = 0x02;
        public const byte CONSTANT_Integer = 0x03;
        public const byte CONSTANT_UInteger = 0x04;
        public const byte CONSTANT_PrivateNamespace = 0x05;
        public const byte CONSTANT_Double = 0x06;
        public const byte CONSTANT_Qname = 0x07;  // ns::name, const ns, const name
        public const byte CONSTANT_Namespace = 0x08;
        public const byte CONSTANT_Multiname = 0x09;    //[ns...]::name, const [ns...], const name
        public const byte CONSTANT_False = 0x0A;
        public const byte CONSTANT_True = 0x0B;
        public const byte CONSTANT_Null = 0x0C;
        public const byte CONSTANT_QnameA = 0x0D;    // @ns::name, const ns, const name
        public const byte CONSTANT_MultinameA = 0x0E;// @[ns...]::name, const [ns...], const name
        public const byte CONSTANT_RTQname = 0x0F;    // ns::name, var ns, const name
        public const byte CONSTANT_RTQnameA = 0x10;    // @ns::name, var ns, const name
        public const byte CONSTANT_RTQnameL = 0x11;    // ns::[name], var ns, var name
        public const byte CONSTANT_RTQnameLA = 0x12; // @ns::[name], var ns, var name
        public const byte CONSTANT_Namespace_Set = 0x15; // a set of namespaces - used by multiname
        public const byte CONSTANT_PackageNamespace = 0x16; // a namespace that was derived from a package
        public const byte CONSTANT_PackageInternalNs = 0x17; // a namespace that had no uri
        public const byte CONSTANT_ProtectedNamespace = 0x18;
        public const byte CONSTANT_ExplicitNamespace = 0x19;
        public const byte CONSTANT_StaticProtectedNs = 0x1A;
        public const byte CONSTANT_MultinameL = 0x1B;
        public const byte CONSTANT_MultinameLA = 0x1C;
        public const byte CONSTANT_TypeName = 0x1D;

        /*
         * Trait tags
         */

        public const int TRAIT_Var = 0x00;
        public const int TRAIT_Method = 0x01;
        public const int TRAIT_Getter = 0x02;
        public const int TRAIT_Setter = 0x03;
        public const int TRAIT_Class = 0x04;
        public const int TRAIT_Function = 0x05;
        public const int TRAIT_Const = 0x06;

        public const int TRAIT_FLAG_final = 0x01;
        public const int TRAIT_FLAG_override = 0x02;
        public const int TRAIT_FLAG_metadata = 0x04;

        // Class flags
        public const int CLASS_FLAG_sealed = 0x01;
        public const int CLASS_FLAG_final = 0x02;
        public const int CLASS_FLAG_interface = 0x04;
        public const int CLASS_FLAG_protected = 0x08;
        public const int CLASS_FLAG_non_nullable = 0x10;

        // Method flags

        public const int METHOD_Arguments = 0x1;
        public const int METHOD_Activation = 0x2;
        public const int METHOD_Needrest = 0x4;
        public const int METHOD_HasOptional = 0x8;
        public const int METHOD_IgnoreRest = 0x10;
        public const int METHOD_Native = 0x20;
        public const int METHOD_Setsdxns = 0x40;
        public const int METHOD_HasParamNames = 0x80;

        /*
         * Opcodes
         */

        public const int OP_bkpt =  0x01;
        public const int OP_nop =  0x02;
        public const int OP_throw =  0x03;
        public const int OP_getsuper = 0x04;      // <mname>(obj) : val
        public const int OP_setsuper = 0x05;      // <mname>(obj,val) : void
        public const int OP_dxns = 0x06;
        public const int OP_dxnslate = 0x07;
        public const int OP_kill = 0x08;
        public const int OP_label = 0x09;

        public const int OP_ifnlt =  0x0c;
        public const int OP_ifnle =  0x0d;
        public const int OP_ifngt =  0x0e;
        public const int OP_ifnge =  0x0f;
        public const int OP_jump =  0x10;
        public const int OP_iftrue =  0x11;
        public const int OP_iffalse =  0x12;
        public const int OP_ifeq =  0x13;
        public const int OP_ifne =  0x14;
        public const int OP_iflt =  0x15;
        public const int OP_ifle =  0x16;
        public const int OP_ifgt =  0x17;
        public const int OP_ifge =  0x18;
        public const int OP_ifstricteq =  0x19;
        public const int OP_ifstrictne =  0x1a;
        public const int OP_lookupswitch =  0x1b;
        public const int OP_pushwith =  0x1c;
        public const int OP_popscope =  0x1d;
        public const int OP_nextname =  0x1e;
        public const int OP_hasnext =  0x1f;

        public const int OP_pushnull =  0x20;
        public const int OP_pushundefined =  0x21;
        public const int OP_pushuninitialized =  0x22;
        public const int OP_nextvalue =  0x23;
        public const int OP_pushbyte =  0x24;
        public const int OP_pushshort =  0x25;
        public const int OP_pushtrue =  0x26;
        public const int OP_pushfalse =  0x27;
        public const int OP_pushnan =  0x28;
        public const int OP_pop =  0x29;
        public const int OP_dup =  0x2a;
        public const int OP_swap =  0x2b;
        public const int OP_pushstring = 0x2c;
        public const int OP_pushint = 0x2d;
        public const int OP_pushuint = 0x2e;
        public const int OP_pushdouble = 0x2f;
        public const int OP_pushscope = 0x30;
        public const int OP_pushnamespace = 0x31;
        public const int OP_hasnext2 = 0x32;
        public const int OP_pushdecimal = 0x33;
 	    public const int OP_pushdnan = 0x34;
 	
 	    public const int OP_li8 = 0x35;
	    public const int OP_li16 = 0x36;
	    public const int OP_li32 = 0x37;
	    public const int OP_lf32 = 0x38;
	    public const int OP_lf64 = 0x39;
	    public const int OP_si8 = 0x3A;
	    public const int OP_si16 = 0x3B;
	    public const int OP_si32 = 0x3C;
	    public const int OP_sf32 = 0x3D;
	    public const int OP_sf64 = 0x3E;

        public const int OP_newfunction =  0x40;
        public const int OP_call =  0x41;
        public const int OP_construct =  0x42;
        public const int OP_callmethod = 0x43;
        public const int OP_callstatic =  0x44;
        public const int OP_callsuper =  0x45;
        public const int OP_callproperty =  0x46;
        public const int OP_returnvoid =  0x47;
        public const int OP_returnvalue =  0x48;
        public const int OP_constructsuper =  0x49;
        public const int OP_constructprop =  0x4A;
        public const int OP_callproplex =  0x4C;
        public const int OP_callsupervoid = 0x4E;
        public const int OP_callpropvoid = 0x4F;
        public const int OP_sxi1 = 0x50;
        public const int OP_sxi8 = 0x51;
        public const int OP_sxi16 = 0x52;
        public const int OP_applytype = 0x53;

        public const int OP_newobject = 0x55;
        public const int OP_newarray = 0x56;
        public const int OP_newactivation = 0x57;

        public const int OP_newclass = 0x58;
        public const int OP_getdescendants = 0x59;
        public const int OP_newcatch = 0x5a;
        public const int OP_deldescendants = 0x5b;

        public const int OP_findpropstrict = 0x5d;
        public const int OP_findproperty   = 0x5e;
        public const int OP_finddef        = 0x5f;
        public const int OP_getlex          = 0x60;

        public const int OP_setproperty =  0x61;
        public const int OP_getlocal =  0x62;
        public const int OP_setlocal =  0x63;

        public const int OP_getglobalscope = 0x64;
        public const int OP_getscopeobject =  0x65;
        public const int OP_getproperty =  0x66;
        public const int OP_initproperty =  0x68;
        public const int OP_deleteproperty =  0x6a;
        public const int OP_getslot =  0x6c;
        public const int OP_setslot =  0x6d;
    
        /** @deprecated use getglobalscope+getslot */
        public const int OP_getglobalslot =  0x6e;

        /** @deprecated use getglobalscope+setslot */
        public const int OP_setglobalslot =  0x6f;


        public const int OP_convert_s = 0x70;
        public const int OP_esc_xelem = 0x71;
        public const int OP_esc_xattr = 0x72;
        public const int OP_convert_i = 0x73;
        public const int OP_convert_u = 0x74;
        public const int OP_convert_d = 0x75;
        public const int OP_convert_b = 0x76;
        public const int OP_convert_o = 0x77;
        public const int OP_checkfilter = 0x78;
        public const int OP_convert_m = 0x79;
 	    public const int OP_convert_m_p = 0x7a;

        public const int OP_coerce =  0x80;
    
        /** @deprecated use OP_convert_b */
        public const int OP_coerce_b        = 0x81;
        public const int OP_coerce_a        = 0x82;
        /** @deprecated use OP_convert_i */
        public const int OP_coerce_i        = 0x83;
        /** @deprecated use OP_convert_d */
        public const int OP_coerce_d        = 0x84;
        public const int OP_coerce_s        = 0x85;
        public const int OP_astype          = 0x86;
        public const int OP_astypelate      = 0x87;
        /** @deprecated use OP_convert_u */
        public const int OP_coerce_u        = 0x88;
	    public const int OP_coerce_o		   = 0x89;

 	    public const int OP_negate_p = 0x8f;
        public const int OP_negate =  0x90;
        public const int OP_increment =  0x91;
        public const int OP_inclocal =  0x92;
        public const int OP_decrement =  0x93;
        public const int OP_declocal =  0x94;
        public const int OP_typeof =  0x95;
        public const int OP_not =  0x96;
        public const int OP_bitnot =  0x97;

	    public const int OP_increment_p = 0x9c;
	    public const int OP_inclocal_p = 0x9d;
	    public const int OP_decrement_p = 0x9e;
	    public const int OP_declocal_p = 0x9f;
 
        public const int OP_add =  0xa0;
        public const int OP_subtract =  0xa1;
        public const int OP_multiply =  0xa2;
        public const int OP_divide =  0xa3;
        public const int OP_modulo =  0xa4;
        public const int OP_lshift =  0xa5;
        public const int OP_rshift =  0xa6;
        public const int OP_urshift =  0xa7;
        public const int OP_bitand =  0xa8;
        public const int OP_bitor =  0xa9;
        public const int OP_bitxor =  0xaa;
        public const int OP_equals =  0xab;
        public const int OP_strictequals =  0xac;
        public const int OP_lessthan =  0xad;
        public const int OP_lessequals =  0xae;
        public const int OP_greaterthan =  0xaf;

        public const int OP_greaterequals =  0xb0;
        public const int OP_instanceof =  0xb1;
        public const int OP_istype =  0xb2;
        public const int OP_istypelate =  0xb3;
        public const int OP_in =  0xb4;
        // arithmetic with decimal parameters
        public const int OP_add_p = 0xb5;
        public const int OP_subtract_p =  0xb6;
        public const int OP_multiply_p =  0xb7;
        public const int OP_divide_p =  0xb8;
        public const int OP_modulo_p =  0xb9;

        public const int OP_increment_i =  0xc0;
        public const int OP_decrement_i =  0xc1;
        public const int OP_inclocal_i =  0xc2;
        public const int OP_declocal_i =  0xc3;
        public const int OP_negate_i =  0xc4;
        public const int OP_add_i =  0xc5;
        public const int OP_subtract_i =  0xc6;
        public const int OP_multiply_i =  0xc7;

        public const int OP_getlocal0 = 0xd0;
        public const int OP_getlocal1 = 0xd1;
        public const int OP_getlocal2 = 0xd2;
        public const int OP_getlocal3 = 0xd3;    
        public const int OP_setlocal0 = 0xd4;
        public const int OP_setlocal1 = 0xd5;
        public const int OP_setlocal2 = 0xd6;
        public const int OP_setlocal3 = 0xd7;    
    
        public const int OP_debug = 0xef;

        public const int OP_debugline = 0xf0;
        public const int OP_debugfile = 0xf1;
        public const int OP_bkptline  = 0xf2;
        public const int OP_timestamp = 0xf3;
    }
}