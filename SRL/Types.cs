using AdvancedBinary;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;

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
        internal struct Quote {
            public char Start;
            public char End;
        }

#pragma warning disable 649, 169
        struct SRLData3 {
            [FString(Length = 4)]
            public string Signature;

            public ushort Version;//Planned to Future

            [PArray(PrefixType = Const.UINT32), StructField]
            public SRLDatabase2[] Databases;

            uint CharsCount;

            [RArray(FieldName = "CharsCount")]
            public char[] OriLetters;

            [RArray(FieldName = "CharsCount")]
            public char[] MemoryLetters;


            uint UnkCount;

            [RArray(FieldName = "UnkCount")]
            public ushort[] UnkChars;

            [RArray(FieldName = "UnkCount")]
            public char[] UnkReps;


            uint RepCount;

            [RArray(FieldName = "RepCount"), PString(PrefixType = Const.UINT32)]
            public string[] RepOri;

            [RArray(FieldName = "RepCount"), PString(PrefixType = Const.UINT32)]
            public string[] RepTrg;
        }

        struct SRLIntro {
            [StructField, PArray(PrefixType = Const.UINT8)]
            public IntroContainer[] Intros;
        }

        struct IntroContainer {
            [PArray(PrefixType = Const.UINT32)]
            public byte[] Bitmap;

            [PArray(PrefixType = Const.UINT32)]
            public byte[] Wav;

            [Ignore]
            public Bitmap Texture => Image.FromStream(new MemoryStream(Bitmap)) as Bitmap;

            [Ignore]
            public bool HasSound => Wav.Length != 0;
        }

        struct SRLDatabase2 {
            [CString]
            public string Name;

            uint EntriesCount;

            [RArray(FieldName = "EntriesCount"), PString(PrefixType = Const.UINT16)]
            public string[] Original;

            [RArray(FieldName = "EntriesCount"), PString(PrefixType = Const.UINT16)]
            public string[] Replace;
            
        }

        #region Deprecated
        //Deprecated But Supported Formats
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
        #endregion

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

            [FieldParmaters(DefaultValue = null, Name = "WorkingDir;WorkDir;DataDir")]
            public string WorkDirectory;

            [FieldParmaters(DefaultValue = true, Name = "NoTrim;DisableTrim;SkipTrim")]
            public bool NoTrim;

            [FieldParmaters(DefaultValue = false, Name = "ReloadMaskArgs;ReloadMaskParameters;")]
            public bool ReloadMaskParameters;

            [FieldParmaters(DefaultValue = "", Name = "CustomCredits;Credits;About;")]
            public string CustomCredits;

            [FieldParmaters(DefaultValue = "", Name = "ReloadedPrefix;Prefix")]
            public string ReloadedPrefix;

            [FieldParmaters(DefaultValue = "", Name = "ReloadedSufix;Sufix")]
            public string ReloadedSufix;

            [FieldParmaters(DefaultValue = false, Name = "LiteMode;FastMode;CompatibiltyMode")]
            public bool LiteMode;

            [FieldParmaters(DefaultValue = false, Name = "RemoveIlegals;RemoveViolation;RemoveViolations;RemoveOutputViolation")]
            public bool RemoveViolations;

            [FieldParmaters(DefaultValue = false, Name = "AsianInput;FromAsian;JapaneseInput;FromJapanese")]
            public bool AsianInput;

            [FieldParmaters(DefaultValue = true, Name = "AutoUnkChar;AutoUnkCharReload;UnkCharByEncoding")]
            public bool AutoUnks;

            [FieldParmaters(DefaultValue = true, Name = "CaseSensitive")]
            public bool CaseSensitive;

            [FieldParmaters(DefaultValue = false, Name = "NotCachedOnly;IgnoreCached;IgnoreInCache")]
            public bool NotCachedOnly;

            [FieldParmaters(DefaultValue = false, Name = "SetOutEncoding;SetOutputEncoding;ForceOutputEncoding")]
            public bool SetOutputEncoding;

            [FieldParmaters(DefaultValue = false, Name = "AllowEmptyReloads;AllowEmptyReload;AllowEmpty;DisableEmptyFilter")]
            public bool AllowEmptyReloads;

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

        [FieldParmaters(Name = "Filter")]
        internal struct FilterSettings {
            [FieldParmaters(DefaultValue = "", Name = "DenyList")]
            public string DenyList;
            [FieldParmaters(DefaultValue = "", Name = "IgnoreList")]
            public string IgnoreList;
            [FieldParmaters(DefaultValue = "", Name = "QuoteList")]
            public string QuoteList;
            [FieldParmaters(DefaultValue = "", Name = "TrimList")]
            public string TrimList;
            [FieldParmaters(DefaultValue = false, Name = "UseDB")]
            public bool UseDB;
            [FieldParmaters(DefaultValue = false, Name = "ForceTrim")]
            public bool ForceTrim;
            [FieldParmaters(DefaultValue = 2, Name = "Sensitivity")]
            public int Sensitivity;
        }


        [FieldParmaters(Name = "Hook")]
        internal struct HookSettings {
#if DEBUG
            [FieldParmaters(Name = "CreateWindow", DefaultValue = false)]
            public bool CreateWindow;
            [FieldParmaters(Name = "CreateWindowEx", DefaultValue = false)]
            public bool CreateWindowEx;
            [FieldParmaters(Name = "SendMessage", DefaultValue = false)]
            public bool SendMessage;
#endif
            [FieldParmaters(Name = "MultiByteToWideChar", DefaultValue = false)]
            public bool MultiByteToWideChar;
            [FieldParmaters(Name = "SetWindowText", DefaultValue = false)]
            public bool SetWindowText;
            [FieldParmaters(Name = "GetGlyphOutline", DefaultValue = false)]
            public bool GetGlyphOutline;
            [FieldParmaters(Name = "TextOut", DefaultValue = false)]
            public bool TextOut;
            [FieldParmaters(Name = "ExtTextOut", DefaultValue = false)]
            public bool ExtTextOut;
            [FieldParmaters(Name = "CreateFont", DefaultValue = false)]
            public bool CreateFont;
            [FieldParmaters(Name = "CreateFontIndirect", DefaultValue = false)]
            public bool CreateFontIndirect;
            [FieldParmaters(Name = "FontCharset", DefaultValue = (byte)0x1)]
            public byte FontCharset;
            [FieldParmaters(Name = "FaceName", DefaultValue = "")]
            public string FaceName;
            [FieldParmaters(Name = "UndoChars;UndoReloads;UndoFakeChars", DefaultValue = false)]
            public bool UndoChars;
        }

        [FieldParmaters(Name = "Intro")]
        internal struct IntroSettings {
            [FieldParmaters(Name = "Seconds;Time;Duration", DefaultValue = 0)]
            public int Seconds;

            [FieldParmaters(Name = "ShowWindow", DefaultValue = true)]
            public bool ShowWindow;
            [FieldParmaters(Name = "SetWindowPos", DefaultValue = true)]
            public bool SetWindowPos;
            [FieldParmaters(Name = "MoveWindow", DefaultValue = false)]
            public bool MoveWindow;
        }

        internal struct FontRedirect {
            public string From;
            public string To;
            public string Size;
        }
#pragma warning restore 649, 169
    }
}
