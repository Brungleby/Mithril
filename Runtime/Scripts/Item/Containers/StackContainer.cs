
/** StackContainer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Mithril.Inventory
{
	#region StackContainer

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public abstract class StackContainer<TItem> : ContainerBase<TItem>
	where TItem : Item
	{
		[Min(-1)]
		public int capacity = -1;

		private List<ItemStack<TItem>> contents;

		public sealed override int count => contents.Count;
		public int quantity
		{
			get
			{
				var result = 0;
				foreach (var i in contents)
					result += i.count;
				return result;
			}
		}
		public bool isEmpty
		{
			get
			{
				foreach (var i in contents)
					if (i.count != 0) return false;
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

		public void Flush()
		{
			foreach (var i in contents.Where(i => i.isEmpty))
				contents.Remove(i);
		}
		public bool Flush(ItemStack<TItem> stack)
		{
			if (!stack.isEmpty) return false;
			contents.Remove(stack);
			return true;
		}

		public sealed override int GetQuantityOf(TItem item)
		{
			var result = 0;
			foreach (var i in contents)
				if (item == i.rep) result += i.count;
			return result;
		}

		public sealed override bool Add(TItem item)
		{
			var stack = GetStackToAdd(item);
			if (stack == null) return false;
			stack++;
			return true;
		}
		public int Add(TItem item, int quantity)
		{
			while (quantity != 0)
			{
				var stack = GetStackToAdd(item);
				if (stack == null) break;
				quantity = stack.Add(quantity);
			}
			return quantity;
		}

		public ItemStack<TItem> GetStackToAdd(TItem item)
		{
			foreach (var i in contents)
				if (item == i.rep && !i.isFull) return i;
			return CreateNewStack(item);
		}

		public sealed override bool Remove(TItem item)
		{
			var stack = GetStackToRemove(item);
			if (stack == null) return false;
			stack--;
			return true;
		}
		public int Remove(TItem item, int quantity)
		{
			while (quantity != 0)
			{
				var stack = GetStackToRemove(item);
				if (stack == null) break;
				quantity = stack.Remove(quantity);
			}
			return quantity;
		}
		public void Remove(ItemStack<TItem> stack) => contents.Remove(stack);

		public sealed override void RemoveAll(TItem item)
		{
			foreach (var i in contents.Where(i => i.rep == item))
				contents.Remove(i);
		}

		public ItemStack<TItem> GetStackToRemove(TItem item)
		{
			foreach (var i in contents)
				if (item == i.rep && !i.isEmpty) return i;
			return null;
		}

		public ItemStack<TItem> CreateNewStack(TItem item)
		{
			if (isFull) return null;

			int id;
			var random = new System.Random();
			while (true)
			{
				id = random.Next();
				foreach (var i in contents)
					if (i.id == id) continue;
				break;
			}

			var result = new ItemStack<TItem>(id, item);
			contents.Add(result);
			return result;
		}
	}

	#endregion
	#region ItemStack

	[Serializable]
	public sealed class ItemStack<TItem> : object
	where TItem : Item
	{
		public readonly int id;

		[SerializeField]
		public TItem rep;

		[SerializeField]
		private int _count;
		public int count
		{
			get => _count; set
			{
				if (_count == value) return;
				_count = value.Clamp(0, rep.capacity);
			}
		}

		public ItemStack(int id, TItem rep, int count)
		{
			this.id = id;
			this.rep = rep;
			this.count = count;
		}

		public ItemStack(int id, TItem rep)
		{
			this.id = id;
			this.rep = rep;
			count = 0;
		}

		public override int GetHashCode() => id;
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;
			return id == ((ItemStack<TItem>)obj).id;
		}

		public bool isEmpty => _count == 0;
		public bool isFull => _count >= rep.capacity;

		public int Clear()
		{
			var result = count;
			count = 0;
			return result;
		}

		public int Add(int value)
		{
			var prev = count;
			count += value;
			return value - count - prev;
		}
		public int Remove(int value) => Add(-value);

		public static bool operator ==(ItemStack<TItem> a, ItemStack<TItem> b) =>
			a.id == b.id;
		public static bool operator !=(ItemStack<TItem> a, ItemStack<TItem> b) =>
			a.id != b.id;

		public static ItemStack<TItem> operator ++(ItemStack<TItem> _)
		{
			_._count++;
			return _;
		}

		public static ItemStack<TItem> operator --(ItemStack<TItem> _)
		{
			_._count--;
			return _;
		}

		public static ItemStack<TItem> operator +(ItemStack<TItem> _, int value)
		{
			_._count += value;
			return _;
		}

		public static ItemStack<TItem> operator -(ItemStack<TItem> _, int value)
		{
			_._count -= value;
			return _;
		}
	}

	#endregion
}
