
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

			/** <<============================================================>> **/

			foreach (var iNode in GetAllNodes())
			{
				var __nPorts = iNode.GetAllPorts();
				var __nPortData = new List<NodeGraphEditableObject.PortData>();
				__nPorts.ForEach(i => __nPortData.Add(new NodeGraphEditableObject.PortData
				{
					NodeGuid = iNode.Guid,
					PortName = i.portName,
					Direction = i.direction,
					Orientation = i.orientation,
					Capacity = i.capacity,
					Type = i.portType,
				}));

				(iNode.IsPredefined ? __data.PredefinedNodes : __data.Nodes)
					.Add(iNode);
			}

			/** <<============================================================>> **/

			foreach (var iEdge in GetEdges().ToArray())
			{
				var __nNode = (CustomNode)iEdge.input.node;
				var __oNode = (CustomNode)iEdge.output.node;

				__data.Edges.Add(new NodeGraphEditableObject.EdgeData(iEdge, __nNode, __oNode));
			}

			data = __data;
		}

		protected override void PullObjectToWindow(EditableObject data)
		{
			var __data = (NodeGraphEditableObject)data;

			/** <<============================================================>> **/

			_graph.CreatePredefinedNodes();

			var __predefinedNodes = GetPredefinedNodes();
			foreach (var iNode in __data.PredefinedNodes)
			{
				var iMatchingPredefinedNode = __predefinedNodes
					.Where(i => i.title == iNode.Title)
					.First()
				;

				iMatchingPredefinedNode.Guid = iNode.Guid;
				iMatchingPredefinedNode.SetPosition(iNode.Rect);
			}

			/** <<============================================================>> **/

			foreach (var iNode in __data.Nodes)
			{
				var __node = _graph.CreateNewNode(iNode);
			}

			/** <<============================================================>> **/

			foreach (var iEdge in __data.Edges)
			{
				var nNode = _graph.FindNode(iEdge.nPort.NodeGuid);
				var oNode = _graph.FindNode(iEdge.oPort.NodeGuid);

				var nPort = nNode.FindPort(iEdge.nPort.PortName);
				var oPort = oNode.FindPort(iEdge.oPort.PortName);

				ConnectPorts(nPort, oPort);
			}

			// var __nodes = GetNodes();
			// for (var i = 0; i < __nodes.Count; i++)
			// {
			// 	var iNode = __nodes[i];

			// 	var __edges = data.Edges
			// 		.Where(i => i.nPort.NodeGuid == iNode.Guid)
			// 		.ToList()
			// 	;
			// 	for (var j = 0; j < __edges.Count; j++)
			// 	{
			// 		var jEdge = __edges[j];

			// 		var __targetPort = jEdge.oPort;
			// 		var __targetNode = __nodes.First(i => i.Guid == __targetPort.NodeGuid);

			// 		ConnectPorts((Port)__targetNode.inputContainer[0], iNode.outputContainer.Q<Port>());
			// 	}
			// }
		}

		#region

		private void ConnectPorts(Port input, Port output)
		{
			var __edge = new Edge
			{
				input = input,
				output = output
			};

			__edge?.input.Connect(__edge);
			__edge?.output.Connect(__edge);

			_graph.Add(__edge);
		}

		#endregion

		#endregion
	}
}
