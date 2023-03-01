
/** NodeGraph.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.UIElements;

#endregion

namespace Cuberoot.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public class NodeGraph : EditorWindow
	{
		#region Data

		#region

		private NodeGraphView _graphView;

		#endregion

		#endregion
		#region Methods

		#region

		[MenuItem("Cuberoot/Node Graph")]

		public static void OpenNodeGraphWindow()
		{
			var window = GetWindow<NodeGraph>();
			window.titleContent = new GUIContent("Node Graph");
		}

		private void OnEnable()
		{
			CreateGraphView();
			CreateToolbar();
		}

		private void OnDisable()
		{
			rootVisualElement.Remove(_graphView);
		}

		private void CreateGraphView()
		{
			_graphView = new NodeGraphView
			{
				name = "Node Graph"
			};

			_graphView.StretchToParentSize();
			rootVisualElement.Add(_graphView);
		}

		private void CreateToolbar()
		{
			var __toolbar = new Toolbar();

			var __createNodeButton = new Button(() =>
			{
				_graphView.CreateNewNode("Dialogue Node");
			});

			__createNodeButton.text = "Create New";

			__toolbar.Add(__createNodeButton);

			rootVisualElement.Add(__toolbar);
		}

		#endregion

		#endregion
	}
}
