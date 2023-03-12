
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

	public abstract class CustomNodeGraph<TGraphView, TGraphData> :
	InstantiableEditorWindow<TGraphData>

	where TGraphView : CustomNodeGraphView
	where TGraphData : CustomNodeGraphData
	{
		#region Data

		private TGraphView _graph;
		public TGraphView graph => _graph;

		#endregion
		#region Methods

		private List<Edge> GetEdges() => _graph.edges.ToList();
		private List<CustomNode> GetNodes() => _graph.nodes.ToList().Cast<CustomNode>().ToList();

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

		protected override void SaveData(in TGraphData data)
		{
			/** <<============================================================>> **/

			foreach (var iNode in GetNodes())
			{
				var __nPorts = iNode.GetAllPorts();
				var __nPortData = new List<PortData>();
				__nPorts.ForEach(i => __nPortData.Add(new PortData
				{
					NodeGuid = iNode.Guid,
					Name = i.portName,
					Direction = i.direction,
					Orientation = i.orientation,
					Capacity = i.capacity,
					Type = i.portType,
				}));

				data.Nodes.Add(new NodeData
				{
					Guid = iNode.Guid,
					Subtype = iNode.GetType(),
					SubtypeName = iNode.GetType().ToString(),
					Title = iNode.title,
					Rect = iNode.GetPosition(),

					Ports = __nPortData.ToArray(),
				});
			}

			/** <<============================================================>> **/

			// foreach (var iEdge in GetEdges().ToArray())
			// {
			// 	var __nNode = (CustomNode)iEdge.input.node;
			// 	var __oNode = (CustomNode)iEdge.output.node;

			// 	data.Links.Add(new LinkData(iEdge, __nNode, __oNode));
			// }

			/** <<============================================================>> **/

			data.Initialize();
		}

		protected override void LoadData(in TGraphData data)
		{
			/** <<============================================================>> **/

			if (!data.isInitialized)
			{
				_graph.CreatePredefinedNodes();
				SaveData(data);
			}

			/** <<============================================================>> **/

			else
			{
				foreach (var iNodeData in data.Nodes)
				{
					var __node = _graph.CreateNewNode(System.Type.GetType(iNodeData.SubtypeName), iNodeData.Guid, iNodeData.Title, iNodeData.Rect, false);

					iNodeData.Ports.ToList().ForEach(i => __node.CreatePort(i));
				}
			}
		}

		#region

		#endregion

		#endregion
	}
	public class CustomNodeGraph : CustomNodeGraph<CustomNodeGraphView, CustomNodeGraphData> { }
}
