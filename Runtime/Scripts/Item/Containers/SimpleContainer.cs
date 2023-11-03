
/** SimpleContainer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Mithril.Inventory
{
	#region SimpleContainer<TItem>

	/// <summary>
	/// A basic container that can hold up to a specific number of items.
	///</summary>

	public abstract class SimpleContainer<TItem> : ContainerBase<TItem>
	{
		[Min(-1)]
		public int capacity = -1;

		private List<TItem> contents;

		public sealed override int count => contents.Count;
		public bool isFull
		{
			get
			{
				if (capacity == -1) return false;
				return count >= capacity;
			}
		}

		public sealed override void Clear() => contents.Clear();

		public sealed override int GetQuantityOf(TItem item)
		{
			var result = 0;
			foreach (var i in contents)
				if (i.Equals(item)) result++;
			return result;
		}

		public sealed override bool Add(TItem item)
		{
			if (isFull) return false;
			contents.Add(item);
			return true;
		}

		public sealed override bool Remove(TItem item) => contents.Remove(item);

		public sealed override void RemoveAll(TItem item)
		{
			while (true)
				if (!Remove(item)) return;
		}
	}

	#endregion
	#region SimpleContainer

	public class SimpleContainer : SimpleContainer<Item>
	{

	}

	#endregion
}
