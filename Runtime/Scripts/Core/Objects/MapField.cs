
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

	public class MapField<Key, Value> : IDictionary<Key, Value>
	{
		#region Constructors

		public MapField()
		{
			_Dictionary = new();

			foreach (var item in _FieldPairs)
			{
				_Dictionary.Add(item.Key, item.Value);
			}
		}

		public MapField(FieldKeyValuePair<Key, Value>[] _pairs)
		{
			_Dictionary = new();

			_FieldPairs = _pairs;
			foreach (var item in _FieldPairs)
			{
				_Dictionary.Add(item.Key, item.Value);
			}
		}

		public MapField(IEnumerable<(Key, Value)> pairs)
		{
			_Dictionary = new();

			var __fieldPairs = new List<FieldKeyValuePair<Key, Value>>();
			foreach (var iPair in pairs)
			{
				var iFieldPair = new FieldKeyValuePair<Key, Value> { Key = iPair.Item1, Value = iPair.Item2 };
				__fieldPairs.Add(iFieldPair);
				_Dictionary.Add(iFieldPair.Key, iFieldPair.Value);
			}
		}

		#endregion
		#region Fields

		[SerializeField]
		private FieldKeyValuePair<Key, Value>[] _FieldPairs = new FieldKeyValuePair<Key, Value>[0];

		#endregion
		#region Members

		private Dictionary<Key, Value> _Dictionary;

		#endregion
		#region IDictionary Overrides

		public Value this[Key key] { get => _Dictionary[key]; set => _Dictionary[key] = value; }

		public ICollection<Key> Keys => _Dictionary.Keys;
		public ICollection<Value> Values => _Dictionary.Values;
		public int Count => _Dictionary.Count;
		public bool IsReadOnly => false;

		public void Add(Key key, Value value) => _Dictionary.Add(key, value);
		public void Add(KeyValuePair<Key, Value> item) => _Dictionary.Add(item.Key, item.Value);
		public void Add((Key, Value) item) => _Dictionary.Add(item.Item1, item.Item2);
		public void Clear() => _Dictionary.Clear();
		public bool Contains(KeyValuePair<Key, Value> item) => _Dictionary.ContainsKey(item.Key);
		public bool Contains((Key, Value) item) => _Dictionary.ContainsKey(item.Item1);
		public bool ContainsKey(Key key) => _Dictionary.ContainsKey(key);
		public void CopyTo(KeyValuePair<Key, Value>[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}
		public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator() => _Dictionary.GetEnumerator();
		public bool Remove(Key key) => _Dictionary.Remove(key);
		public bool Remove(KeyValuePair<Key, Value> item) => _Dictionary.Remove(item.Key);
		public bool Remove((Key, Value) item) => _Dictionary.Remove(item.Item1);
		public bool TryGetValue(Key key, out Value value) => _Dictionary.TryGetValue(key, out value);
		public Value TryGetValue(Key key)
		{
			Value __value;
			_Dictionary.TryGetValue(key, out __value);
			return __value;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion
	}

	[System.Serializable]
	public class FieldKeyValuePair<K, V>
	{
		public K Key;
		public V Value;
	}

}