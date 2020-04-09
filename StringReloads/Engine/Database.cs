using StringReloads.StringModifier;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace StringReloads.Engine
{
    internal class Database : IEnumerator<LSTParser.LSTEntry>, IEnumerable<LSTParser.LSTEntry>
    {
        int CurrentIndex = -1;

        public string Name;

        public Database(string Name) => this.Name = Name;

        DuplicableDictionary<string, LSTParser.LSTEntry> DB = new DuplicableDictionary<string, LSTParser.LSTEntry>();

        public int Count => DB.Count;

        public LSTParser.LSTEntry Current { 
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

        public void Add(LSTParser.LSTEntry Entry) {
            var Key = Minify.Default.Apply(Entry.OriginalLine, null);
            DB.Add(Key, Entry);
        }

        public void AddRange(IEnumerable<LSTParser.LSTEntry> Entries) {
            foreach (var Entry in Entries) {
                Add(Entry);
            }
        }

        public bool HasKey(string Key) {
            return DB.ContainsKey(Key);
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

        public IEnumerator<LSTParser.LSTEntry> GetEnumerator()
        {
            CurrentIndex = -1;
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            CurrentIndex = -1;
            return this;
        }

        public LSTParser.LSTEntry this[string Key] => DB[Key];
    }
}
