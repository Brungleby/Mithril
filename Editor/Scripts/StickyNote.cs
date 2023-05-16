
/** StickyNote.cs
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

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

#endregion

namespace Mithril.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class StickyNote : UnityEditor.Experimental.GraphView.StickyNote
	{
		#region Data

		#region



		#endregion

		#endregion
		#region Methods

		#region

		public StickyNote() : base()
		{
			fontSize = StickyNoteFontSize.Small;
			theme = StickyNoteTheme.Black;

		}

		#endregion

		#endregion
	}
}
