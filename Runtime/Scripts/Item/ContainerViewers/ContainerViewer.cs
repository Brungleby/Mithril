
/** ContainerViewer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril.Inventory
{
	#region ContainerViewer<TContainer>

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>
	public abstract class ContainerViewer<TContainer, TItem> : MithrilComponent
	where TContainer : ContainerBase<TItem>
	{
		[SerializeField]
		public TContainer focus;

		[SerializeField]
		public GameObject prefabListing;

		public int focusedItemListing { get; protected set; } = 0;

		protected override void Awake()
		{
			base.Awake();

			Refresh();
		}

		public abstract void Refresh();
	}

	#endregion
}
