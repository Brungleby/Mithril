
/** TestObjectFilter.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Linq;
using UnityEngine;
using TMPro;

#endregion

namespace Mithril.Tests
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class TestObjectFilter : MonoBehaviour
	{
		#region Data

		#region

		public TestEditableObject data;
		public string startGuid;
		private TMP_Text _text;

		#endregion

		#endregion
		#region Methods

		public int intValue
		{
			get => int.Parse(_text.text);
			set => _text.text = value.ToString();
		}

		#region

		private void Awake()
		{
			_text = GetComponent<TMP_Text>();
		}

		private void Update()
		{
			var __index = Mathf.FloorToInt(Time.time);
			intValue = __index;
		}

		// private Mithril.Editor.Node startNode
		// {
		// 	get
		// 	{

		// 	}
		// }

		private int GetValueAt(int index)
		{

			return 0;
		}

		#endregion

		#endregion
	}
}
