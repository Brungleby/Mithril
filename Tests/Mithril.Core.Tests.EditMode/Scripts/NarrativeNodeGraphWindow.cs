
/** NarrativeNodeGraphWindow.cs
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

using UnityEditor;
using UnityEditor.Experimental.GraphView;

#endregion

namespace Mithril.Tests.EditMode
{
	#region NarrativeNodeGraphWindow

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class NarrativeNodeGraphWindow : Editor.BasicNodeGraphWindow<NarrativeSearchWindow>
	{
		#region Data



		#endregion
		#region Methods

		#region Construction

		public NarrativeNodeGraphWindow()
		{

		}

		#endregion

		#endregion
	}

	#endregion
	#region NarrativeSearchWindow

	public sealed class NarrativeSearchWindow : Editor.NodeGraphSearchSubwindow
	{
		public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			var __result = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("Available Nodes"), 0),
					new SearchTreeEntry(new GUIContent("Custom Node")) { level = 1, userData = typeof(Editor.Node) },
			};

			return __result;
		}
	}

	#endregion
}
