
/** NodeGraphSearchSubwindow.cs
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

namespace Mithril.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public abstract class NodeGraphSearchSubwindow : ScriptableObject, ISearchWindowProvider
	{
		#region Data

		#region

		private NodeGraphView _graphView;

		#endregion

		#endregion
		#region Methods

		#region

		public void InitializeFor(NodeGraphView graphView)
		{
			_graphView = graphView;
		}

		public abstract List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context);

		public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
		{
			Type type = (Type)searchTreeEntry.userData;

			_graphView.CreateNewNodeAtCursor(type);
			return true;
		}

		#endregion

		#endregion
	}
}
