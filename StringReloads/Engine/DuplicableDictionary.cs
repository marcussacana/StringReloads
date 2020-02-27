using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StringReloads.Engine
{
    public class DuplicableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public int LastKeyIndex { get; private set; } = 0;

        Collection<TKey> KeyCollection = new Collection<TKey>();
        Collection<TValue> ValueCollection = new Collection<TValue>();
        Dictionary<TKey, KeyValuePairs<TKey, TValue>> Base = new Dictionary<TKey, KeyValuePairs<TKey, TValue>>();

        public bool NearToNext = true;

        public TValue this[TKey key] {
            get {
                if (TryGetValue(key, out TValue value))
                    return value;
                throw new KeyNotFoundException();
            }
            set => throw new NotImplementedException("You Must use the Item Index");
        }

        public TValue this[int index] {
            get {
                return ValueCollection[index];
            }
            set {
                ValueCollection[index] = value;
                var Entry = Base[KeyCollection[index]];
                Entry.Update(index, value);
                Base[KeyCollection[index]] = Entry;
            }
        }

        public ICollection<TKey> Keys => KeyCollection;

        public ICollection<TValue> Values => ValueCollection;

        public int Count => ValueCollection.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            int ID = Count;
            KeyCollection.Add(item.Key);
            ValueCollection.Add(item.Value);

            if (Base.ContainsKey(item.Key))
            {
                var Entry = Base[item.Key];
                Entry.Insert(item.Value, ID);
                Base[item.Key] = Entry;
            }
            else
                Base[item.Key] = new KeyValuePairs<TKey, TValue>(item.Key, item.Value, ID);
        }

        public void Clear()
        {
            Base.Clear();
            KeyCollection.Clear();
            ValueCollection.Clear();
            LastKeyIndex = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!Base.ContainsKey(item.Key))
                return false;
            foreach (var Pair in Base[item.Key].GetKeyPairs())
            {
                if (Pair.Key.Equals(item.Key) && Pair.Value.Equals(item.Value))
                    return true;
            }

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return Base.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; i++)
                array[i + arrayIndex] = new KeyValuePair<TKey, TValue>(KeyCollection[i], ValueCollection[i]);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return new KeyValuePair<TKey, TValue>(KeyCollection[i], ValueCollection[i]);
        }

        /// <summary>
        /// This don't remove the entry, just disable the match
        /// </summary>
        public bool Remove(TKey key)
        {
            if (!ContainsKey(key))
                return false;

            return Base.Remove(key);
        }

        /// <summary>
        /// This don't remove the entry, just disable the match
        /// </summary>
        public void RemoveAt(int Index)
        {
            var Key = KeyCollection[Index];
            Base[Key].Remove(Index);
            if (Base[Key].Count == 0)
                Base.Remove(Key);
        }

        /// <summary>
        /// This don't remove the entry, just disable the match
        /// </summary>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!Base.ContainsKey(item.Key))
                return false;

            int Removed = 0;

            var Entry = Base[item.Key];

            foreach (var Pair in Entry.GetIdPairs())
            {
                if (Pair.Value.Equals(item.Value))
                {
                    Entry.Remove(Pair.Key);
                    Removed++;
                }
            }

            if (Entry.Count == 0)
                Base.Remove(item.Key);

            return Removed > 0;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!Base.ContainsKey(key))
            {
                value = default;
                return false;
            }

            if (KeyCollection[LastKeyIndex].Equals(key))
            {
                value = ValueCollection[LastKeyIndex];
                if (NearToNext && LastKeyIndex + 1 < Keys.Count)
                    LastKeyIndex++;
                return true;
            }

            var Entry = Base[key];

            var Closest = ClosestTo(Entry.Identifiers, LastKeyIndex);
            if (Closest == int.MaxValue)
                Closest = Entry.Identifiers.First();

            LastKeyIndex = Closest;
            if (NearToNext && LastKeyIndex + 1 < Keys.Count)
                LastKeyIndex++;

            foreach (var Pair in Entry.GetIdPairs())
            {
                if (Pair.Key == Closest)
                {
                    value = Pair.Value;
                    return true;
                }
            }
            value = default;
            return false;
        }

        public static int ClosestTo(IEnumerable<int> collection, int target)
        {
            // NB Method will return int.MaxValue for a sequence containing no elements.
            // Apply any defensive coding here as necessary.
            var closest = int.MaxValue;
            var minDifference = int.MaxValue;
            foreach (var element in collection)
            {
                var difference = Math.Abs((long)element - target);
                if (minDifference > difference)
                {
                    minDifference = (int)difference;
                    closest = element;
                }
            }

            return closest;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class Collection<Type> : ICollection<Type>
    {
        List<Type> List = new List<Type>();
        public int Count => List.Count;

        public bool IsReadOnly => false;
        public Type this[int index] { get => List[index]; set => List[index] = value; }

        public void Add(Type item)
        {
            List.Add(item);
        }

        public void Clear()
        {
            List.Clear();
        }

        public bool Contains(Type item)
        {
            return List.Contains(item);
        }

        public void CopyTo(Type[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public bool Remove(Type item)
        {
            return List.Remove(item);
        }

        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }
    }

    internal struct KeyValuePairs<TKey, TValue>
    {
        public TKey Key { get; private set; }
        public TValue[] Values { get; private set; }

        public int[] Identifiers { get; private set; }

        public int Count => Values.Length;

        public KeyValuePairs(TKey Key, TValue Value, int Identifier)
        {
            this.Key = Key;
            Values = new TValue[] { Value };
            Identifiers = new int[] { Identifier };
        }

        public void Insert(TValue Value, int Identifier) => Insert(new TValue[] { Value }, new int[] { Identifier });
        public void Insert(TValue[] Values, int[] Identifiers)
        {
            this.Values = this.Values.Concat(Values).ToArray();
            this.Identifiers = this.Identifiers.Concat(Identifiers).ToArray();
        }

        public void Update(int Identifier, TValue Value)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (Identifiers[i] == Identifier)
                {
                    Values[i] = Value;
                    break;
                }
            }
        }

        public void Remove(int Identifier)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (Identifiers[i] == Identifier)
                {
                    var tmpVal = Values.ToList();
                    var tmpIds = Identifiers.ToList();

                    tmpVal.RemoveAt(i);
                    tmpIds.RemoveAt(i);

                    Values = tmpVal.ToArray();
                    Identifiers = tmpIds.ToArray();
                    break;
                }
            }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> GetKeyPairs()
        {
            foreach (var Value in Values)
                yield return new KeyValuePair<TKey, TValue>(Key, Value);
        }
        public IEnumerable<KeyValuePair<int, TValue>> GetIdPairs()
        {
            for (int i = 0; i < Values.Length; i++)
                yield return new KeyValuePair<int, TValue>(Identifiers[i], Values[i]);
        }
    }
}