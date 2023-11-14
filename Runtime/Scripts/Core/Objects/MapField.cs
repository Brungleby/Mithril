
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
	public sealed class MapField<Key, Value> : object, IDictionary<Key, Value>
	{
		#region Constructors

		public MapField()
		{
			_dict = new();

			foreach (var item in _FieldPairs)
			{
				_dict.Add(item.key, item.value);
			}
		}

		public MapField(FieldKeyValuePair<Key, Value>[] _pairs)
		{
			_dict = new();

			_FieldPairs = _pairs;
			foreach (var item in _FieldPairs)
			{
				_dict.Add(item.key, item.value);
			}
		}

		public MapField(IEnumerable<(Key, Value)> pairs)
		{
			_dict = new();

			var __fieldPairs = new List<FieldKeyValuePair<Key, Value>>();
			foreach (var iPair in pairs)
			{
				var iFieldPair = new FieldKeyValuePair<Key, Value> { key = iPair.Item1, value = iPair.Item2 };
				__fieldPairs.Add(iFieldPair);
				_dict.Add(iFieldPair.key, iFieldPair.value);
			}
		}

		#endregion
		#region Fields

		[SerializeField]
		private FieldKeyValuePair<Key, Value>[] _FieldPairs = new FieldKeyValuePair<Key, Value>[0];

		#endregion
		#region Members

		private Dictionary<Key, Value> _dict;

		#endregion
		#region IDictionary Overrides

		public Value this[Key key] { get => _dict[key]; set => _dict[key] = value; }

		public ICollection<Key> Keys => _dict.Keys;
		public ICollection<Value> Values => _dict.Values;
		public int Count => _dict.Count;
		public bool IsReadOnly => false;

		public void Add(Key key, Value value) => _dict.Add(key, value);
		public void Add(KeyValuePair<Key, Value> item) => _dict.Add(item.Key, item.Value);
		public void Add((Key, Value) item) => _dict.Add(item.Item1, item.Item2);
		public void Clear() => _dict.Clear();
		public bool Contains(KeyValuePair<Key, Value> item) => _dict.ContainsKey(item.Key);
		public bool Contains((Key, Value) item) => _dict.ContainsKey(item.Item1);
		public bool ContainsKey(Key key) => _dict.ContainsKey(key);
		public void CopyTo(KeyValuePair<Key, Value>[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}
		public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator() => _dict.GetEnumerator();
		public bool Remove(Key key) => _dict.Remove(key);
		public bool Remove(KeyValuePair<Key, Value> item) => _dict.Remove(item.Key);
		public bool Remove((Key, Value) item) => _dict.Remove(item.Item1);
		public bool TryGetValue(Key key, out Value value) => _dict.TryGetValue(key, out value);
		public Value TryGetValue(Key key)
		{
			_dict.TryGetValue(key, out Value __value);
			return __value;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion
	}

	[System.Serializable]
	public sealed class FieldKeyValuePair<K, V> : object
	{
		public K key;
		public V value;
	}

}
