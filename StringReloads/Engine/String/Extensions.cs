using StringReloads.StringModifier;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringReloads
{
    public static class Extensions
    {
        public static bool ToBoolean(this string Value)
        {
            if (Value == null)
                return false;

            Value = Value.ToLowerInvariant();
            switch (Value)
            {
                case "1":
                case "true":
                case "yes":
                case "on":
                case "enable":
                case "enabled":
                    return true;
            }
            return false;
        }

        internal static Log.LogLevel ToLogLevel(this string ValueStr)
        {
            switch (ValueStr.ToLowerInvariant())
            {
                case "t":
                case "tra":
                case "trc":
                case "trace":
                    return Log.LogLevel.Trace;
                case "d":
                case "deb":
                case "dbg":
                case "debug":
                    return Log.LogLevel.Debug;
                case "i":
                case "inf":
                case "info":
                case "information":
                    return Log.LogLevel.Information;
                case "w":
                case "war":
                case "warn":
                case "warning":
                    return Log.LogLevel.Warning;
                case "e":
                case "err":
                case "erro":
                case "error":
                    return Log.LogLevel.Error;
                case "c":
                case "cri":
                case "crit":
                case "critical":
                    return Log.LogLevel.Critical;
            }

            return (Log.LogLevel)ValueStr.ToInt32();
        }

        public static uint ToUInt32(this string Value)
        {
            if (Value == null)
                return 0;

            if (Value.StartsWith("0x") && uint.TryParse(Value.Substring(2), NumberStyles.HexNumber, null, out uint Val))
                return Val;

            if (uint.TryParse(Value, out Val))
                return Val;

            return 0;
        }

        public static int ToInt32(this string Value)
        {
            if (Value == null)
                return 0;

            if (Value.StartsWith("0x") && int.TryParse(Value.Substring(2), NumberStyles.HexNumber, null, out int Val))
                return Val;

            if (int.TryParse(Value, out Val))
                return Val;

            return 0;
        }

        public static Encoding ToEncoding(this string Value)
        {
            if (int.TryParse(Value, out int CP))
                return Encoding.GetEncoding(CP);

            return Value.ToLowerInvariant() switch
            {
                "sjis" => Encoding.GetEncoding(932),
                "shiftjis" => Encoding.GetEncoding(932),
                "shift-jis" => Encoding.GetEncoding(932),
                "unicode" => Encoding.Unicode,
                "utf16" => Encoding.Unicode,
                "utf16be" => Encoding.BigEndianUnicode,
                "utf16wb" => new UnicodeEncoding(false, true),
                "utf16wbbe" => new UnicodeEncoding(true, true),
                "utf16bewb" => new UnicodeEncoding(true, true),
                "utf8" => Encoding.UTF8,
                "utf8wb" => new UTF8Encoding(true),
                "utf7" => Encoding.UTF7,
                _ => Encoding.GetEncoding(Value)
            };
        }

        public static string Unescape(this string String)
        {
            return Escape.Default.Restore(String);
        }
    }
}
