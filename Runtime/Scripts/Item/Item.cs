
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
	public class Item : ScriptableObject
	{
		[SerializeField]
		public GameObject prefab;
	}

	#endregion
}
