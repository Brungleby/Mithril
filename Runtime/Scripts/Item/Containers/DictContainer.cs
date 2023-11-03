
/** DictContainer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Mithril.Inventory
{
	#region Container<TItem>

	/// <summary>
	/// A basic container that can hold up to a specific number of item stacks, but an infinite number of items per stack.
	///</summary>

	public abstract class DictContainer<TItem> : ContainerBase<TItem>
	{
		[Min(-1)]
		public int capacity = -1;

		private Dictionary<TItem, int> contents;

		public sealed override int count => contents.Count;
		public int quantity
		{
			get
			{
				var result = 0;
				foreach (var i in contents)
					result += i.Value;
				return result;
			}
		}
		public sealed override bool isEmpty
		{
			get
			{
				foreach (var i in contents)
					if (i.Value != 0) return false;
				return true;
			}
		}
		public bool isFull
		{
			get
			{
				if (capacity == -1) return false;
				return count >= capacity;
			}
		}

		public sealed override void Clear() => contents.Clear();

		public void FlushAll()
		{
			foreach (var iKey in contents.Keys.ToArray())
				if (contents[iKey] <= 0) contents.Remove(iKey);
		}

		public sealed override int GetQuantityOf(TItem item) =>
			contents.ContainsKey(item) ? contents[item] : 0;

		public sealed override bool Add(TItem item)
		{
			if (isFull) return false;
			contents[item]++;
			return true;
		}

		public sealed override bool Remove(TItem item)
		{
			if (contents[item] == 0) return false;

			contents[item]--;
			if (contents[item] == 0)
				contents.Remove(item);
			return true;
		}

		public sealed override void RemoveAll(TItem item)
		{
			contents.Remove(item);
		}

		public bool RemoveNoFlush(TItem item)
		{
			if (contents[item] == 0) return false;

			contents[item]--;
			return true;
		}

		public int RemoveNoFlush(TItem item, int quantity)
		{
			while (quantity > 0)
			{
				if (!RemoveNoFlush(item)) break;
				quantity--;
			}
			return quantity;
		}
	}

	#endregion
	#region DictContainer

	public class DictContainer : DictContainer<Item> { }

	#endregion
}
