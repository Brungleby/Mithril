
/** WeightedList.cs
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
	/// A list of items that have float weights associated to them. When selecting an item randomly from this list, items that have higher weights are more likely to be selected.
	///</summary>

	[System.Serializable]

	public class WeightedList<T> : ICollection<(T, float)>
	{
		#region Fields

		#region (field) Items

		/// <summary>
		/// The list of items and associated weights.
		///</summary>
		[Tooltip("The list of items and associated weights.")]
		[SerializeField]

		private List<(T, float)> _Items = new List<(T, float)>();

		#endregion

		#endregion
		#region Members
		#endregion
		#region Properties

		public int Count => _Items.Count;
		public bool IsReadOnly => false;

		public List<T> items
		{
			get
			{
				List<T> result = new List<T>();

				foreach (var item in _Items)
				{
					result.Add(item.Item1);
				}

				return result;
			}
		}

		public float totalWeight
		{
			get
			{
				float result = 0f;
				foreach (var item in _Items)
				{
					result += Mathf.Max(item.Item2, 0f);
				}

				return result;
			}
		}

		#endregion
		#region Functions

		public T Random_Unweighted(int? seed = null) => _Items.Random(seed).Item1;

		public T Random(int? seed = null)
		{
			if (_Items.Count == 0) return default(T);
			if (_Items.Count == 1) return _Items[0].Item1;

			float weight = Math.Random(0f, totalWeight, seed);

			for (int i = 0; i < _Items.Count - 1; i++)
			{
				if (weight < _Items[i].Item2) return _Items[i].Item1;
			}

			return _Items[_Items.Count - 1].Item1;
		}

		public void Add((T, float) item)
		{
			_Items.Add(item);
		}

		public void Clear()
		{
			_Items.Clear();
		}

		public bool Contains((T, float) item)
		{
			return _Items.Contains(item);
		}

		public void CopyTo((T, float)[] array, int arrayIndex)
		{
			_Items.CopyTo(array, arrayIndex);
		}

		public bool Remove((T, float) item)
		{
			return _Items.Remove(item);
		}

		public IEnumerator<(T, float)> GetEnumerator()
		{
			return _Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Items.GetEnumerator();
		}

		#endregion
	}
}