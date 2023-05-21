
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
		public abstract NodeGraphView graphView { get; protected set; }
	}

	public abstract class NodeGraphWindow<TGraphView> : NodeGraphWindow
	where TGraphView : NodeGraphView
	{
		#region Data

		#region

		private TGraphView _graphView;
		public sealed override NodeGraphView graphView
		{
			get => _graphView;
			protected set => _graphView = (TGraphView)value;
		}

		private Label _hoveredGuidLabel;

		#endregion

		#endregion
		#region Methods

		private List<Edge> GetEdges() => _graphView.edges.Cast<Edge>().ToList();
		private List<Node> GetPredefinedNodes() => _graphView.nodes
			.Cast<Node>()
			.Where(i => i.isPredefined)
			.ToList()
		;
		private List<Node> GetNodes() => _graphView.nodes
			.Cast<Node>()
			.Where(i => !i.isPredefined)
			.ToList()
		;
		private List<Node> GetAllNodes() => _graphView.nodes
			.Cast<Node>()
			.ToList()
		;

		protected override void OnGUI()
		{
			base.OnGUI();

			if (_graphView.hoveredNode is Node __hoveredNode)
				_hoveredGuidLabel.text = __hoveredNode.guid;
			else
				_hoveredGuidLabel.text = string.Empty;
		}

		/** <<============================================================>> **/

		public void UpdateEditorFromModel(NodeGraphData data)
		{
			_graphView.UpdateEditorFromModel(data);
		}

		protected override void SetupVisualElements()
		{
			_graphView = System.Activator.CreateInstance<TGraphView>();
			_graphView.onModified.AddListener(NotifyIsModified);

			rootVisualElement.Add(graphView);

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
			if (_graphView != null)
				rootVisualElement.Remove(_graphView);
		}

		/** <<============================================================>> **/

		public override void SetupWorkObject()
		{
			SetupGraphView(_graphView);
		}

		protected override void WrapupWorkObject()
		{
			// ((NodeGraphData)workObject).UpdateFromGraphView(graphView);
			((NodeGraphData)workObject).UpdateModelFromEditor(_graphView);
		}

		/** <<============================================================>> **/

		protected virtual void SetupGraphView(NodeGraphView graphView)
		{
			// graph.InitFromGraphData((NodeGraphData)workObject);
			graphView.UpdateEditorFromModel((NodeGraphData)workObject);
		}

		/** <<============================================================>> **/
		#endregion
	}

	public abstract class BasicNodeGraphWindow<TSearchWindow> : NodeGraphWindow<NodeGraphView<TSearchWindow>>
	where TSearchWindow : NodeGraphSearchSubwindow
	{ }
}
