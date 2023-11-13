
/** SimpleContainer_Viewer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion

namespace Mithril.Inventory
{
	#region SimpleContainer_Viewer

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class SimpleContainer_Viewer : ContainerViewer<SimpleContainer, Item>
	{
		[AutoAssign]
		public VerticalLayoutGroup vBox { get; private set; }

		public override void Refresh()
		{
			/** <<============================================================>> **/

			var i = -1;
			foreach (var iObj in transform.GetChildren().Select(i => i.gameObject))
			{
				if (iObj == EventSystem.current.currentSelectedGameObject)
					focusedItemListing = i + 1;

				Destroy(iObj);
				i++;
			}

			/** <<============================================================>> **/

			foreach (var iItem in focus.contents)
			{
				var obj = Instantiate(prefabListing, transform);
				var text = obj.GetComponentInChildren<TMP_Text>();
				if (text != null)
				{
					text.text = iItem.title;
				}
			}

			/** <<============================================================>> **/

			if (transform.childCount == 0) return;

			focusedItemListing = focusedItemListing.Min(transform.childCount - 1);
			EventSystem.current.SetSelectedGameObject(transform.GetChild(focusedItemListing).gameObject);
		}
	}

	#endregion
}
