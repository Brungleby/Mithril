
/** Item.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using UnityEngine;

#endregion

namespace Mithril.Inventory
{
	#region Item<TQuantity>

	/// <summary>
	/// Stores static data for an item type with a generic quantity type.
	///</summary>

	public abstract class Item<TQuantity> : ScriptableObject
	where TQuantity : unmanaged, IComparable<TQuantity>
	{
		[SerializeField]
		public string title = "Item";

		[Min(0)]
		[SerializeField]
		public TQuantity capacity;
	}

	#endregion
	#region Item

	[Serializable]
	[CreateAssetMenu(fileName = "New Item", menuName = "Mithril/Item", order = 1)]
	public class Item : Item<int>
	{

	}

	#endregion
}
