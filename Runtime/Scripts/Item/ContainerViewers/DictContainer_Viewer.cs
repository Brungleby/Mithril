
/** DictContainer_Viewer.cs
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
	#region DictContainer_Viewer

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class DictContainer_Viewer : ContainerViewer<DictContainer, Item>
	{
		[SerializeField]
		private string nameID = "Title";

		[SerializeField]
		private string quantityID = "Quantity";

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

				var textComponents = obj.GetComponentsInChildren<TMP_Text>();
				foreach (var iText in textComponents)
				{
					if (iText.name.Contains(nameID))
						iText.text = iItem.Key.title;
					else if (iText.name.Contains(quantityID))
						iText.text = iItem.Value.ToString();
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
