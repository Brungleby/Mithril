
/** HashMapField.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#endregion

namespace Mithril
{
	/// <summary>
	/// A map that can be serialized and modified in the Unity Editor.
	///</summary>
	[Serializable]
	public sealed class DictionaryField<TKey, TValue> : object, IDictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		#region Constructors

		public DictionaryField() { }

		public DictionaryField(IDictionary<TKey, TValue> dict)
		{
			foreach (var iKey in dict.Keys)
				_dict[iKey] = dict[iKey];
		}

		#endregion
		#region Fields

		[SerializeField]
		private List<KeyValuePairField<TKey, TValue>> _contents = new();

		[SerializeField]
		private int _contentsLength;

		#endregion
		#region Members

		private Dictionary<TKey, TValue> _dict = new();

		public int val = 11;

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

		public void OnAfterDeserialize()
		{
			_dict.Clear();
			_contentsLength = _contents.Count;

			foreach (var iKVPair in _contents)
			{
				if (iKVPair.key == null) continue;
				_dict[iKVPair.key] = iKVPair.value;
			}
		}

		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			ValidateGenerics();
#endif
			_contents.Clear();

			foreach (var iKVPair in _dict)
				_contents.Add(new(iKVPair.Key, iKVPair.Value));
#if UNITY_EDITOR
			/**	Add placeholder entries
			*/
			for (var i = _dict.Count; i < _contentsLength; i++)
				_contents.Add(new(default, default));
#endif
		}

		private void ValidateGenerics()
		{
			if (typeof(TKey).GetCustomAttribute<SerializableAttribute>() == null)
				Debug.LogWarning($"Type {typeof(TKey)} in {this} is not serializable.");
			if (typeof(TValue).GetCustomAttribute<SerializableAttribute>() == null)
				Debug.LogWarning($"Type {typeof(TValue)} in {this} is not serializable.");
		}

		#endregion
	}

	[Serializable]
	public sealed class KeyValuePairField<TKey, TValue> : object
	{
		public KeyValuePairField(TKey key, TValue value)
		{
			this.key = key;
			this.value = value;
		}

		public TKey key;
		public TValue value;

		public override string ToString()
		{
			return $"({key}: {value})";
		}
	}
}
