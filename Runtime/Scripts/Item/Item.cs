
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

namespace Mithril.Item
{
	#region Item

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[Serializable]
	[CreateAssetMenu(fileName = "New Item", menuName = "Mithril/Item", order = 1)]
	public class ItemData : ScriptableObject
	{
		[SerializeField]
		public string title = "Item";

		[Min(0)]
		[SerializeField]
		public int maxQuantity = 1;
	}

	#endregion
}
