
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

	public abstract class NodeGraphWindow : EditableWindow
	{
		public abstract NodeGraphView graph { get; protected set; }
	}

	public abstract class NodeGraphWindow<TGraphView> : NodeGraphWindow
	where TGraphView : NodeGraphView
	{
		#region Data

		#region

		private TGraphView _graph;
		public sealed override NodeGraphView graph
		{
			get => _graph;
			protected set => _graph = (TGraphView)value;
		}

		private Label _hoveredGuidLabel;

		#endregion

		#endregion
		#region Methods

		private List<Edge> GetEdges() => _graph.edges.Cast<Edge>().ToList();
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

		protected override void OnGUI()
		{
			base.OnGUI();

			if (_graph.hoveredNode is Node __hoveredNode)
				_hoveredGuidLabel.text = __hoveredNode.guid;
			else
				_hoveredGuidLabel.text = string.Empty;
		}

		/** <<============================================================>> **/

		protected override void SetupVisualElements()
		{
			_graph = System.Activator.CreateInstance<TGraphView>();
			_graph.onModified.AddListener(NotifyIsModified);

			rootVisualElement.Add(graph);

			_hoveredGuidLabel = new Label();
			_hoveredGuidLabel.style.color = new StyleColor(new Color(1f, 1f, 1f, 0.25f));
			_hoveredGuidLabel.style.position = Position.Absolute;
			_hoveredGuidLabel.style.right = 0f;
			_hoveredGuidLabel.style.bottom = 0f;
			rootVisualElement.Add(_hoveredGuidLabel);

			base.SetupVisualElements();
		}

		protected override void TeardownVisualElements()
		{
			if (_graph != null)
				rootVisualElement.Remove(_graph);
		}

		/** <<============================================================>> **/

		public override void OnSetupForWorkObject()
		{
			SetupGraphView(_graph);
		}

		protected override void OnBeforeSaveWorkObject()
		{
			((NodeGraphData)workObject).UpdateFromGraphView(graph);
		}

		/** <<============================================================>> **/

		protected virtual void SetupGraphView(NodeGraphView graph)
		{
			graph.InitFromGraphData((NodeGraphData)workObject);
		}

		/** <<============================================================>> **/
		#endregion
	}

	public abstract class BasicNodeGraphWindow<TSearchWindow> : NodeGraphWindow<NodeGraphView<TSearchWindow>>
	where TSearchWindow : NodeGraphSearchSubwindow
	{ }
}
