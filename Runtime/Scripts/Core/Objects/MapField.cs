
/** HashMapField.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#endregion

namespace Mithril
{
	/// <summary>
	/// A map that can be serialized and modified in the Unity Editor.
	///</summary>
	[System.Serializable]
	public sealed class MapField<TKey, TValue> : object, IDictionary<TKey, TValue>
	{
		#region Constructors

		public MapField()
		{
			_dict = new();
			foreach (var iPair in _fieldContents)
				_dict[iPair.key] = iPair.value;
		}

		public MapField(FieldKeyValuePair<TKey, TValue>[] pairs)
		{
			_dict = new();
			_fieldContents = pairs;
			foreach (var item in _fieldContents)
				_dict.Add(item.key, item.value);
		}

		public MapField(IDictionary<TKey, TValue> dict)
		{
			_dict = new();
			foreach (var iKey in dict.Keys)
				_dict[iKey] = dict[iKey];
		}

		#endregion
		#region Fields

		[SerializeField]
		private FieldKeyValuePair<TKey, TValue>[] _fieldContents = new FieldKeyValuePair<TKey, TValue>[0];

		#endregion
		#region Members

		private Dictionary<TKey, TValue> _dict;

		#endregion
		#region IDictionary Overrides

		public TValue this[TKey key] { get => _dict[key]; set => _dict[key] = value; }

		public ICollection<TKey> Keys => _dict.Keys;
		public ICollection<TValue> Values => _dict.Values;
		public int Count => _dict.Count;
		public bool IsReadOnly => false;

		public void Add(TKey key, TValue value) => _dict.Add(key, value);
		public void Add(KeyValuePair<TKey, TValue> item) => _dict.Add(item.Key, item.Value);
		public void Add((TKey, TValue) item) => _dict.Add(item.Item1, item.Item2);
		public void Clear() => _dict.Clear();
		public bool Contains(KeyValuePair<TKey, TValue> item) => _dict.ContainsKey(item.Key);
		public bool Contains((TKey, TValue) item) => _dict.ContainsKey(item.Item1);
		public bool ContainsKey(TKey key) => _dict.ContainsKey(key);
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
			throw new System.NotImplementedException();
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
		public bool Remove(TKey key) => _dict.Remove(key);
		public bool Remove(KeyValuePair<TKey, TValue> item) => _dict.Remove(item.Key);
		public bool Remove((TKey, TValue) item) => _dict.Remove(item.Item1);
		public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);
		public TValue TryGetValue(TKey key)
		{
			_dict.TryGetValue(key, out TValue __value);
			return __value;
		}

		IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

		#endregion
	}

	[System.Serializable]
	public sealed class FieldKeyValuePair<K, V> : object
	{
		public K key;
		public V value;
	}

}
