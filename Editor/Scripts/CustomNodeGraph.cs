
/** CustomNodeGraph.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

#endregion

namespace Cuberoot.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public abstract class CustomNodeGraph<TGraphView> :
	InstantiableEditorWindow

	where TGraphView : CustomNodeGraphView
	{
		#region Data

		private TGraphView _graph;
		public TGraphView graph => _graph;

		#endregion
		#region Methods

		private List<Edge> GetEdges() => _graph.edges.ToList();
		private List<CustomNode> GetPredefinedNodes() => _graph.nodes
			.Cast<CustomNode>()
			.Where(i => i.IsPredefined)
			.ToList()
		;
		private List<CustomNode> GetNodes() => _graph.nodes
			.Cast<CustomNode>()
			.Where(i => !i.IsPredefined)
			.ToList()
		;
		private List<CustomNode> GetAllNodes() => _graph.nodes
			.Cast<CustomNode>()
			.ToList()
		;

		protected override void CreateVisualElements()
		{
			_graph = System.Activator.CreateInstance<TGraphView>();
			_graph.name = "New Custom Node Graph View";
			InitializeGraphView(_graph);
			rootVisualElement.Add(_graph);

			base.CreateVisualElements();
		}

		protected virtual void InitializeGraphView(TGraphView graph)
		{
			graph.name = "Basic Node Graph View";
			graph.OnModified.AddListener(() =>
			{
				isModified = true;
			});
		}

		protected override void CleanUpVisualElements()
		{
			if (_graph != null)
				rootVisualElement.Remove(_graph);
		}

		protected override void PushChangesToObject(ref EditableObject data)
		{
			var __data = (NodeGraphEditableObject)data;

			__data.CompileNodes(GetAllNodes());
			__data.CompileEdges(GetEdges());

			data = __data;
		}

		protected override void PullObjectToWindow(EditableObject data)
		{
			_graph.CreatePredefinedNodes();

			var __data = (NodeGraphEditableObject)data;
			var __nodes = __data.nodes.ToList();
			var __predefinedNodes = GetPredefinedNodes();

			/** <<============================================================>> **/

			foreach (var iPredefinedNode in __predefinedNodes)
			{
				var iMatchingPredefinedNodeData = __nodes
					.Where(i => i.IsPredefined && i.Title == iPredefinedNode.title)
					.First()
				;

				iPredefinedNode.Guid = iMatchingPredefinedNodeData.Guid;
				iPredefinedNode.SetPosition(iMatchingPredefinedNodeData.Rect);
			}

			foreach (var iNode in __nodes.Where(i => !i.IsPredefined))
				_graph.CreateNewNode(iNode);

			/** <<============================================================>> **/

			foreach (var iEdge in __data.edges)
				_graph.CreateEdge(iEdge);
		}

		#region

		#endregion

		#endregion
	}
}
