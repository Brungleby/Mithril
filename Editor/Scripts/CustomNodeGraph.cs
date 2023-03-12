
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
		#region Properties

		private List<Edge> Edges => _graph.edges.ToList();
		private List<CustomNode> Nodes => _graph.nodes.ToList().Cast<CustomNode>().ToList();

		#endregion
		#region Methods

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
			foreach (var iNode in Nodes)
			{
				data.Nodes.Add(new NodeData
				{
					Guid = iNode.Guid,
					Subtype = iNode.GetType(),
					Title = iNode.title,
					Rect = iNode.GetPosition(),
				});
			}
		}

		protected override void LoadData(in TGraphData data)
		{
			if (!data.Nodes.Any())
				_graph.CreatePredefinedNodes();

			foreach (var iNodeData in data.Nodes)
			{
				var __node = _graph.CreateNewNode<CustomNode>(iNodeData.Guid, iNodeData.Title, iNodeData.Rect, false);

				// var __ports = 
			}
		}

		#region

		#endregion

		#endregion
	}
	public class CustomNodeGraph : CustomNodeGraph<CustomNodeGraphView, CustomNodeGraphData> { }
}
