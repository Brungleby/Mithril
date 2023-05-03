
/** NodeGraphWindow.cs
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

namespace Mithril.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public class NodeGraphWindow<TGraphView> : InstantiableWindow
	where TGraphView : NodeGraphView
	{
		#region Data

		#region

		private TGraphView _graph;
		public TGraphView graph => _graph;

		#endregion

		#endregion
		#region Methods

		private List<Edge> GetEdges() => _graph.edges.ToList();
		private List<Node> GetPredefinedNodes() => _graph.nodes
			.Cast<Node>()
			.Where(i => i.isPredefined)
			.ToList()
		;
		private List<Node> GetNodes() => _graph.nodes
			.Cast<Node>()
			.Where(i => !i.isPredefined)
			.ToList()
		;
		private List<Node> GetAllNodes() => _graph.nodes
			.Cast<Node>()
			.ToList()
		;

		#region Setup

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
			graph.onModified.AddListener(NotifyIsModified);
		}

		protected override void DisposeVisualElements()
		{
			if (_graph != null)
				rootVisualElement.Remove(_graph);
		}

		#endregion

		// protected override void PushChangesToObject(ref EditableObject data)
		// {
		// 	var __data = (NodeGraphData)data;

		// 	__data.viewPosition = _graph.viewTransform.position;
		// 	__data.CompileNodes(GetAllNodes());
		// 	__data.CompileEdges(GetEdges());

		// 	data = __data;
		// }

		// protected override void PullObjectToWindow(EditableObject data)
		// {
		// 	var __data = (NodeGraphData)data;

		// 	/** <<============================================================>> **/

		// 	_graph.SetViewPosition(__data.viewPosition);
		// 	_graph.CreatePredefinedNodes();

		// 	/** <<============================================================>> **/

		// 	var __nodes = __data.nodes.ToList();
		// 	var __predefinedNodes = GetPredefinedNodes();

		// 	/** <<============================================================>> **/

		// 	if (__nodes.Any())
		// 		foreach (var iPredefinedNode in __predefinedNodes)
		// 		{
		// 			var iMatchingPredefinedNodeData = __nodes
		// 				.Where(i => i.isPredefined && i.title == iPredefinedNode.title)
		// 				.First()
		// 			;

		// 			iPredefinedNode.guid = iMatchingPredefinedNodeData.guid;
		// 			iPredefinedNode.SetPosition(iMatchingPredefinedNodeData.rect);
		// 		}

		// 	foreach (var iNode in __nodes.Where(i => !i.isPredefined))
		// 		_graph.CreateNewNode(iNode);

		// 	/** <<============================================================>> **/

		// 	foreach (var iEdge in __data.edges)
		// 		_graph.CreateEdge(iEdge);
		// }

		#endregion
	}

	public class ForgeNodeWindow : NodeGraphWindow<NodeGraphView>
	{

	}
}
