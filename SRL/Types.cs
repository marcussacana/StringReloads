using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRL {
    partial class StringReloader {
        
        /// <summary>
        /// Universal Array Append
        /// </summary>
        /// <typeparam name="T">The Type of the array</typeparam>
        /// <param name="Arr">The Array to Append</param>
        /// <param name="Val">The value to append in the array</param>
        /// <param name="CheckDouble">Don't Append if the array already contains this value.</param>
        internal static void AppendArray<T>(ref T[] Arr, T Val, bool CheckDouble = false) {
            if (CheckDouble) {
                int Cnt = (from x in Arr where x.Equals(Val) select x).Count();
                if (Cnt > 0)
                    return;
            }
            T[] NArr = new T[Arr.Length + 1];
            Arr.CopyTo(NArr, 0);
            NArr[Arr.Length] = Val;
            Arr = NArr;
        }

#pragma warning disable 649
        struct SRLData {
            [FString(Length = 4)]
            public string Signature;

            [PArray(PrefixType = Const.UINT32), CString]
            public string[] Original;

            [PArray(PrefixType = Const.UINT32), CString]
            public string[] Replace;

            [PArray(PrefixType = Const.UINT32)]
            public char[] OriLetters;

            [PArray(PrefixType = Const.UINT32)]
            public char[] MemoryLetters;

            [PArray(PrefixType = Const.UINT32)]
            public ushort[] UnkChars;

            [PArray(PrefixType = Const.UINT32)]
            public char[] UnkReps;

            [PArray(PrefixType = Const.UINT32), CString]
            public string[] RepOri;

            [PArray(PrefixType = Const.UINT32), CString]
            public string[] RepTrg;
        }
        struct TLBC {
            [FString(Length = 4)]
            public string Signature;

            [PArray(PrefixType = Const.UINT32), CString]
            public string[] Original;

            [PArray(PrefixType = Const.UINT32), CString]
            public string[] Replace;

        }
#pragma warning restore 649
    }
}
