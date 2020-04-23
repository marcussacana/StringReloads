using StringReloads.StringModifier;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StringReloads.Engine
{
    public class Database : IEnumerator<LSTEntry>, IEnumerable<LSTEntry>
    {
        int CurrentIndex = -1;

        bool Minifier = true;

        public string Name;

        public Database(string Name, bool Minifier) : this(Name) => this.Minifier = Minifier;

        public Database(string Name) => this.Name = Name;

        DuplicableDictionary<string, LSTEntry> DB = new DuplicableDictionary<string, LSTEntry>();
        List<string> TLs = new List<string>();

        public int Count => DB.Count;

        public LSTEntry Current { 
            get {
                if (CurrentIndex < 0)
                    CurrentIndex = 0;

                if (CurrentIndex >= DB.Count)
                    throw new Exception("Enumerator Overflow");

                return DB.Values.ElementAt(CurrentIndex);
            }
        }

        object IEnumerator.Current {
            get {
                if (CurrentIndex >= DB.Count)
                    return null;

                return DB.Values.ElementAt(CurrentIndex);
            }
        }

        public void Add(LSTEntry Entry) {
            var Key = Minify(Entry.OriginalLine);
            var Value = Minify(Entry.TranslationLine);
            DB.Add(Key, Entry);

            if (!TLs.Contains(Value))
                TLs.Add(Value);
        }

        public void AddRange(IEnumerable<LSTEntry> Entries) {
            foreach (var Entry in Entries) {
                Add(Entry);
            }
        }

        public bool HasKey(string Key) {
            return DB.ContainsKey(Minify(Key));
        }
        public bool HasValue(string Value) {
            return TLs.Contains(Minify(Value));
        }

        private string Minify(string String) {
            if (!Minifier)
                return String;
            return StringModifier.Minify.Default.Apply(String, null);
        }

        public bool MoveNext()
        {
            if (CurrentIndex + 1 >= DB.Count)
                return false;

            CurrentIndex++;
            return true;
        }

        public void Reset()
        {
            CurrentIndex = -1;
        }

        public void Dispose() { }

        public IEnumerator<LSTEntry> GetEnumerator()
        {
            CurrentIndex = -1;
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            CurrentIndex = -1;
            return this;
        }

        public LSTEntry this[string Key] => DB[Minify(Key)];
    }
}
