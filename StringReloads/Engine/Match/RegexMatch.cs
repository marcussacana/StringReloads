using StringReloads.Engine.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace StringReloads.Engine.Match
{
    class RegexMatch : IMatch
    {
		SRL Engine;
        Thread DumpThread = null;
        public RegexMatch(SRL Engine) {
			this.Engine = Engine;
			string LSTPath = Path.Combine(Config.Default.WorkingDirectory, "Regex.lst");
			if (File.Exists(LSTPath))
			{
				using (var LST = new LSTParser(LSTPath))
				{
					foreach (var Entry in LST.GetEntries())
						AddRegex(Entry.OriginalLine, Entry.TranslationLine);
				}
			}
            DumpThread = new Thread(Dump);
			DumpThread.IsBackground = true;
            DumpThread.Start();
        }

        public void AddRegex(string OriginalLine, string TranslationLine)
		{
			RDB[new Regex(OriginalLine)] = TranslationLine;
		}

        ~RegexMatch() {
			DumpThread.Abort();
			RegexLST?.Close();
		}

        List<string> DumpCache = new List<string>();
        Dictionary<Regex, string> RDB = new Dictionary<Regex, string>();
        public bool HasMatch(string String) => FindMatch(String, out _);

        private bool FindMatch(string String, out Regex Pattern) {
			if (Config.Default.Filter.RegexFilter && !String.IsDialogue(UseDB: false)) {
				Pattern = null;
				return false;
			}
            foreach (var Regex in RDB) {
                if (Regex.Key.IsMatch(String)) {
                    Pattern = Regex.Key;
                    return true;
                }
            }
            Pattern = null;
            return false;
        }

        public bool HasValue(string String)
        {
            return false;
        }

        public LSTEntry? MatchString(string String)
        {
            if (!FindMatch(String, out Regex Pattern)) {
				NewDumps.Enqueue(String);
                return null;
            }
			var Result = string.Empty;
			if (Config.Default.ReloadRegexCaptures)
			{
				Dictionary<string, string> Pairs = new Dictionary<string, string>();
				var Matches = Pattern.Matches(String).Cast<System.Text.RegularExpressions.Match>();
				foreach (var Match in Matches) {
					var Groups = Match.Groups.Cast<Group>();
					foreach (var Group in Groups) {
						Pairs.Add($"${Pairs.Count}", Engine.MatchString(this, Group.Value)?.TranslationLine ?? Group.Value);
					}
				}
				
				Result = RDB[Pattern];
				foreach (var Replace in Pairs) {
					Result = Result.Replace(Replace.Key, Replace.Value);
				}
			}
			else
				Result = Pattern.Replace(String, RDB[Pattern]);


			return new LSTEntry(String, Result);
        }


		TextWriter RegexLST = null;
		Queue<string> NewDumps = new Queue<string>();
        private void Dump() {
            if (!Config.Default.DumpRegex)
                return;

			 if (RegexLST == null) {
                string LSTPath = Path.Combine(Config.Default.WorkingDirectory, "Regex.lst");
                if (File.Exists(LSTPath)) {
                    using (var Reader = File.OpenText(LSTPath)) {
                        while (Reader.Peek() != -1) {
                            DumpCache.Add(Engine.Minify(Reader.ReadLine()));
                            Reader.ReadLine();
                        }
                        Reader.Close();
                    }
                }
                RegexLST = File.AppendText(LSTPath);
            }

            while (true) {
				while (NewDumps.Count > 0)
				{
					var Str = NewDumps.Dequeue();
					if (Config.Default.Filter.DumpRegexFilter && !Str.IsDialogue(UseDB: false))
						continue;
					if (DumpCache.Contains(Str))
						continue;
					foreach (var Dump in DumpCache)
					{
						if (BuildPattern(Str, Dump, out string Pattern, out string Replace) || BuildPatternB(Str, Dump, out Pattern, out Replace))
						{
							RegexLST.WriteLine(Pattern);
							RegexLST.WriteLine(Replace);
							RegexLST.Flush();
							break;
						}
					}
					DumpCache.Add(Str);
				}
                Thread.Sleep(100);
            }
        }

		private bool BuildPatternB(string SampleA, string SampleB, out string Pattern, out string Replace) => BuildPattern(SampleB, SampleA, out Pattern, out Replace);
		private bool BuildPattern(string SampleA, string SampleB, out string Pattern, out string Replace)
		{
			Replace = null;
			Pattern = null;
			var EqualA = string.Empty;
			var EqualB = string.Empty;
			var BeginA = 0;
			var BeginB = 0;
			var Diffs = Differences(SampleA, SampleB);
			foreach (var Diff in Diffs)
			{
				EqualA += SampleA.Substring(BeginA, Diff.BeginA - BeginA);
				BeginA = Diff.BeginA + Diff.LengthA;
				EqualB += SampleB.Substring(BeginB, Diff.BeginB - BeginB);
				BeginB = Diff.BeginB + Diff.LengthB;
			}

			EqualA += SampleA.Substring(BeginA);
			EqualB += SampleB.Substring(BeginB);

			if (EqualA.Length < SampleA.Length / 3)
				return false;
			if (EqualB.Length < SampleB.Length / 3)
				return false;

			bool NeedsTrimNumbers = false;
			Pattern = string.Empty;
			Replace = string.Empty;
			BeginA = 0;
			int Arg = 1;
			foreach (var Diff in Diffs)
			{
				if (Diff.LengthA == 0)
				{
					Pattern = null;
					Replace = null;
					return false;
				}
				var EqualStr = SampleA.Substring(BeginA, Diff.BeginA - BeginA);
				if (NeedsTrimNumbers)
				{
					NeedsTrimNumbers = false;
					EqualStr = EqualStr.TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
				}
				Pattern += Regex.Escape(EqualStr).Replace("\\ ", " ");
				Replace += EqualStr;
				BeginA = Diff.BeginA + Diff.LengthA;
				var DiffVal = SampleA.Substring(Diff.BeginA, Diff.LengthA);

				//Hacky fix Missmatch in string that ends in a quote, like "You got Caws (x5)"
				foreach (var Quote in StringReloads.Extensions.Quotes) {
					if (DiffVal.EndsWith(Quote.End.ToString())) {
						DiffVal = DiffVal.Substring(0, DiffVal.Length - 1);
						BeginA--;
					}
				}

				if (DiffVal.Trim().Contains(" ")) {
					Pattern = null;
					Replace = null;
					return false;
				}

				bool HasSpace = DiffVal.Contains(' ');
				bool HasLetter = (from x in DiffVal where char.IsLetter(x) select x).Any();
				bool HasDigit = (from x in DiffVal where char.IsDigit(x) select x).Any();
				if (!HasSpace && !HasDigit)
					HasLetter = true;

				var Selector = (HasSpace, HasLetter) switch
				{
					(true, true) => "(.+)",
					(false, true) => "([^ ]+)",
					(true, false) => "([\\d ]+)",
					(false, false) => "(\\d+)"
				};
				if (HasDigit && !HasLetter && !HasSpace)
				{
					NeedsTrimNumbers = true;
					Pattern = Pattern.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
					Replace = Replace.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
				}

				var Replacer = $"${Arg++}";

				Pattern += Selector;
				Replace += Replacer;
			}
			var FinalEqualStr = SampleA.Substring(BeginA);
			if (NeedsTrimNumbers)
				FinalEqualStr = FinalEqualStr.TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

			Pattern += Regex.Escape(FinalEqualStr).Replace("\\ ", " "); ;
			Replace += FinalEqualStr;

			return true;
		}


		public IEnumerable<DiffDetail> Differences(string StrA, string StrB, int MinMatch = 3)
		{
			int maximumLength = Math.Max(StrA.Length, StrB.Length);
			for (int IndexA = 0, IndexB = 0; IndexA < maximumLength; IndexA++, IndexB++)
			{
				char? aChar = GetCharAt(StrA, IndexA);
				char? bChar = GetCharAt(StrB, IndexB);
				if (aChar == bChar)
					continue;

				if (NextMatch(StrA, StrB, IndexA, IndexB, out int NewIndexA, out int NewIndexB, MinMatch))
				{
					yield return new DiffDetail()
					{
						BeginA = IndexA,
						BeginB = IndexB,
						LengthA = NewIndexA - IndexA,
						LengthB = NewIndexB - IndexB
					};
					IndexA = NewIndexA - 1;
					IndexB = NewIndexB - 1;
				}
				else if (NextMatchB(StrA, StrB, IndexA, IndexB, out NewIndexA, out NewIndexB, MinMatch))
				{
					yield return new DiffDetail()
					{
						BeginA = IndexA,
						BeginB = IndexB,
						LengthA = NewIndexA - IndexA,
						LengthB = NewIndexB - IndexB
					};
					IndexA = NewIndexA - 1;
					IndexB = NewIndexB - 1;
				}
				else
				{
					yield return new DiffDetail()
					{
						BeginA = IndexA,
						BeginB = IndexB,
						LengthA = StrA.Length - IndexA,
						LengthB = StrB.Length - IndexB
					};
					yield break;
				}
			}
		}

		public struct DiffDetail
		{
			public int BeginA;
			public int BeginB;
			public int LengthA;
			public int LengthB;
		}

		private bool NextMatchB(string A, string B, int IndexA, int IndexB, out int BeginIndexA, out int BeginIndexB, int MinMatch = 3) => NextMatch(B, A, IndexB, IndexA, out BeginIndexB, out BeginIndexA, MinMatch);
		private bool NextMatch(string A, string B, int IndexA, int IndexB, out int BeginIndexA, out int BeginIndexB, int MinMatch = 3)
		{
			BeginIndexA = 0;
			BeginIndexB = 0;
			for (int i = IndexA; i < A.Length; i++)
			{
				for (int x = IndexB; x < B.Length; x++)
				{
					var CharA = GetCharAt(A, i);
					var CharB = GetCharAt(B, x);
					if (CharA != CharB)
					{
						continue;
					}
					if (CharA == null || CharB == null)
						return false;
					int Len = GetMatchLength(A, B, i, x);
					if (Len >= MinMatch)
					{
						BeginIndexA = i;
						BeginIndexB = x;
						return true;
					}
				}
			}
			return false;
		}

		private int GetMatchLength(string A, string B, int IndexA, int IndexB)
		{
			int Len = 0;
			int MaxLen = Math.Max(A.Length, B.Length);
			for (int i = 0; i < MaxLen; i++)
			{
				var CharA = GetCharAt(A, IndexA++);
				var CharB = GetCharAt(B, IndexB++);
				if (CharA == null || CharB == null)
					break;
				if (CharA != CharB)
					break;
				Len++;
			}
			return Len;
		}

		private char? GetCharAt(string Str, int At)
		{
			if (At >= Str.Length)
				return null;
			return Str[At];
		}

		public char? ResolveRemap(char Char) => null;

        public FontRemap? ResolveRemap(string Facename, int Width, int Height, uint Charset) => null;
    }
}
