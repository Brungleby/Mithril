
/** Container.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Mithril.Inventory
{
	#region IContainer<TItem>

	public interface IContainer<TItem>
	{
		/// <summary>
		/// The total count of item listings in this container.
		///</summary>
		int count { get; }

		/// <summary>
		/// If there are currently no items in this container.
		///</summary>
		bool isEmpty => count == 0;

		/// <summary>
		/// Destroys all items in this container.
		///</summary>
		void Clear();

		/// <summary>
		/// Returns true if at least the <paramref name="quantity"/> of the <paramref name="item"/> is inside this container.
		///</summary>
		bool Contains(TItem item, int quantity) =>
			GetQuantityOf(item).CompareTo(quantity) != -1;

		/// <summary>
		/// Returns the total quantity of the given <paramref name="item"/> inside this container.
		///</summary>
		int GetQuantityOf(TItem item);

		/// <summary>
		/// Adds a single <paramref name="item"/> to this container.
		///</summary>
		bool Add(TItem item);

		/// <summary>
		/// Adds the <paramref name="quantity"/> of <paramref name="item"/> to this container.
		///</summary>
		/// <returns>
		/// The number of <paramref name="item"/>s that were NOT added.
		///</returns>
		int Add(TItem item, int quantity)
		{
			while (quantity > 0)
			{
				if (!Add(item)) break;
				quantity--;
			}
			return quantity;
		}

		/// <summary>
		/// Removes a single <paramref name="item"/> from this container.
		///</summary>
		/// <returns>
		/// True if the item was successfully removed.
		///</returns>
		bool Remove(TItem item);

		/// <summary>
		/// Removes multiple of an item from this container.
		///</summary>
		/// <returns>
		/// The number of <paramref name="item"/>s that were NOT removed.
		///</returns>
		int Remove(TItem item, int quantity)
		{
			while (quantity > 0)
			{
				if (!Remove(item)) break;
				quantity--;
			}
			return quantity;
		}

		/// <summary>
		/// Removes all of the specified <paramref name="item"/>.
		///</summary>
		void RemoveAll(TItem item);
	}

	#endregion
	#region ContainerBase<TItem>

	/// <summary>
	/// Base class for a data structure that contains a variety of items.
	///</summary>

	public abstract class ContainerBase<TItem> : MithrilComponent, IContainer<TItem>
	{
		public abstract int count { get; }

		public abstract bool Add(TItem item);
		public abstract void Clear();
		public abstract int GetQuantityOf(TItem item);
		public abstract bool Remove(TItem item);
		public abstract void RemoveAll(TItem item);

		public override string ToString() => $"{base.ToString()} [{count} items]";
	}

	#endregion
}
