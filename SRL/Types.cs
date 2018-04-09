using AdvancedBinary;
using System.Linq;

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
        struct SRLData2 {
            [FString(Length = 4)]
            public string Signature;

            [PArray(PrefixType = Const.UINT16), StructField]
            public SRLDatabase[] Databases;

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

        struct SRLDatabase {
            [PArray(PrefixType = Const.UINT32), CString]
            public string[] Original;

            [PArray(PrefixType = Const.UINT32), CString]
            public string[] Replace;
        }

        //Decrapted But Supported Formats
        struct TLBC {
            [FString(Length = 4)]
            public string Signature;

            [PArray(PrefixType = Const.UINT32), CString]
            public string[] Original;

            [PArray(PrefixType = Const.UINT32), CString]
            public string[] Replace;

        }
        struct SRLData1 {
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

        //Ini Settings

        [FieldParmaters(Name = "StringReloader")]
        struct SRLSettings {
            [FieldParmaters(DefaultValue = null, Name = "InEncoding;ReadEncoding;Encoding")]
            public string InEncoding;

            [FieldParmaters(DefaultValue = null, Name = "OutEncoding;WriteEncoding;Encoding")]
            public string OutEncoding;

            [FieldParmaters(DefaultValue = false, Name = "Wide;Unicode;MultiByte")]
            public bool Wide;

            [FieldParmaters(DefaultValue = false, Name = "Multithread;DisablePipe")]
            public bool Multithread;

            [FieldParmaters(DefaultValue = null, Name = "DenyChars;NoChars")]
            public string DenyChars;

            [FieldParmaters(DefaultValue = false, Name = "TrimRangeMismatch;TrimRange")]
            public bool TrimRangeMismatch;

            [FieldParmaters(DefaultValue = false, Name = "CachePointers;CachePointer;ReusePointer;ReusePointers")]
            public bool CachePointers;

            [FieldParmaters(DefaultValue = false, Name = "FreeOnExit;FreePointers;FreeMemory")]
            public bool FreeOnExit;

            [FieldParmaters(DefaultValue = false, Name = "NoDialogCheck;NoDiagCheck;DisableDiagCheck;DisableDialogCheck")]
            public bool NoDialogCheck;

            [FieldParmaters(DefaultValue = false, Name = "LiteralMask;MaskLiteralMatch;MaskMatch")]
            public bool LiteralMaskMatch;

            [FieldParmaters(DefaultValue = false, Name = "MultiDatabase;MultiDB;SplitDatabase;SplitDB")]
            public bool MultiDatabase;

            [FieldParmaters(DefaultValue = false, Name = "WindowHook;WindowReloader")]
            public bool WindowHook;

            [FieldParmaters(DefaultValue = false, Name = "InvalidateWindow;Invalidate;RedrawWindow")]
            public bool InvalidateWindow;

            [FieldParmaters(DefaultValue = null, Name = "AcceptableRanges;AcceptableRange;ValidRange;ValidRanges")]
            public string AcceptableRanges;

            [FieldParmaters(DefaultValue = false, Name = "DecodeFromInput;DecodeInputRemap;DecodeCharacterRemapFromInput;DecodeRemapChars")]
            public bool DecodeFromInput;

            [FieldParmaters(DefaultValue = false, Name = "ReadOnly;NoInjection;DisableReloader;NoReload")]
            public bool NoReload;

            [FieldParmaters(DefaultValue = true, Name = "LiveSettings;KeepSettingsUpdate;ReloadSettings")]
            public bool LiveSettings;

            [FieldParmaters(DefaultValue = true, Name = "AntiCrash;CrashHandler")]
            public bool AntiCrash;

            [FieldParmaters(DefaultValue = null, Name = "BreakLine;GameBreakLine")]
            public string GameLineBreaker;

            [FieldParmaters(DefaultValue = null, Name = "MatchIgnore;IgnoreMatchs")]
            public string MatchIgnore;

            [FieldParmaters(DefaultValue = null, Name = "TrimChars;TrimStrings")]
            public string TrimChars;

            [FieldParmaters(DefaultValue = null, Name = "WorkingDir;WorkDir;DataDir")]
            public string WorkDirectory;

            [FieldParmaters(DefaultValue = true, Name = "NoTrim;DisableTrim;SkipTrim")]
            public bool NoTrim;

            [FieldParmaters(DefaultValue = false, Name = "ReloadMaskArgs;ReloadMaskParameters;")]
            public bool ReloadMaskParameters;

            [FieldParmaters(DefaultValue = "", Name = "CustomCredits;Credits;About;")]
            public string CustomCredits;
        }

        [FieldParmaters(Name = "WordWrap")]
        public struct WordwrapSettings {
            [FieldParmaters(DefaultValue = false, Name = "Enable;Enabled")]
            public bool Enabled;

            [FieldParmaters(DefaultValue = 0, Name = "MaxWidth;Width;Length")]
            public uint Width;

            [FieldParmaters(DefaultValue = 0f, Name = "Size;FontSize")]
            public float Size;

            [FieldParmaters(DefaultValue = null, Name = "Face;FaceName;Font;FontName;FamilyName")]
            public string FontName;

            [FieldParmaters(DefaultValue = false, Name = "Bold")]
            public bool Bold;

            [FieldParmaters(DefaultValue = false, Name = "Monospaced;FixedSize;FixedLength")]
            public bool Monospaced;

            [FieldParmaters(DefaultValue = false, Name = "FakeBreakLine;NoBreakLine")]
            public bool FakeBreakLine;
        }

        [FieldParmaters(Name = "Overlay")]
        public struct OverlaySettings {
            [FieldParmaters(DefaultValue = false, Name = "EnableOverlay;Enabled;Enable;ShowOverlay")]
            public bool Enable;

            [FieldParmaters(DefaultValue = false, Name = "ShowNative;ShowNonReloaded")]
            public bool ShowNonReloaded;

            [FieldParmaters(DefaultValue = null, Name = "Padding")]
            public string Padding;
        }
#pragma warning restore 649
    }
}
