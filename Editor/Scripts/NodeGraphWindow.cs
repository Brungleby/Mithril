
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

		/** <<============================================================>> **/

		protected override void SetupVisualElements()
		{
			_graph = System.Activator.CreateInstance<TGraphView>();
			_graph.name = "New Custom Node Graph View";
			rootVisualElement.Add(_graph);

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

		protected virtual void SetupGraphView(TGraphView graph)
		{
			graph.name = "Basic Node Graph View";

			graph.InitFromGraphData((NodeGraphData)workObject);

			graph.onModified.AddListener(NotifyIsModified);
		}

		/** <<============================================================>> **/
		#endregion
	}
}
