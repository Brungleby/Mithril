
/** NodeGraphView.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

#endregion

namespace Mithril.Editor
{
	public abstract class NodeGraphView<TSearchWindow> : NodeGraphView
	where TSearchWindow : NodeGraphSearchSubwindow
	{
		private TSearchWindow _searchWindow;

		protected override void AddSearchWindow()
		{
			_searchWindow = ScriptableObject.CreateInstance<TSearchWindow>();
			nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
			_searchWindow.InitializeFor(this);
		}
	}

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public abstract class NodeGraphView : GraphView
	{
		#region Data

		#region

		public UnityEvent onModified;

		private Vector2 _mousePosition;
		public Vector2 mousePosition => _mousePosition;

		private Node _hoveredNode;
		public Node hoveredNode => _hoveredNode;

		private bool _isNotifiable = false;
		private bool _shouldCreatePredefinedNodes = true;

		#endregion

		#endregion
		#region Methods

		#region Construction

		public NodeGraphView()
		{
			onModified = new UnityEvent();

			styleSheets.Add(Resources.Load<StyleSheet>("Stylesheets/GraphBackgroundDefault"));
			styleSheets.Add(Resources.Load<StyleSheet>("Stylesheets/StickyNoteDefault"));

			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new ContextualMenuManipulator(_CreateContextMenu));

			// nodeCreationRequest += OnNodeCreation;
			graphViewChanged += OnGraphViewChanged;

			deleteSelection += OnDelete;
			unserializeAndPaste += OnPaste;
			serializeGraphElements += OnCopy;

			RegisterCallback<MouseMoveEvent>(OnMouseMove);

			var __background = new GridBackground();
			Insert(0, __background);
			__background.StretchToParentSize();

			AddSearchWindow();

			this.StretchToParentSize();
		}

		// public void InitFromGraphData(NodeGraphData data)
		// {
		// 	/** <<============================================================>> **/

		// 	if (data == null)
		// 		return;

		// 	_isNotifiable = false;
		// 	ClearAllNodes();

		// 	/** <<============================================================>> **/

		// 	SetViewPosition(data.viewPosition);

		// 	/** <<============================================================>> **/
		// 	/**	Nodes
		// 	*/

		// 	foreach (var iNodeMirror in data.nodeMirrors)
		// 	{
		// 		var __node = iNodeMirror.GetReflection<Node>();
		// 		SetupNewNode(__node);
		// 	}

		// 	/** <<============================================================>> **/
		// 	/**	Edges
		// 	*/

		// 	foreach (var iEdgeMirror in data.edgeMirrors)
		// 	{
		// 		var __edge = iEdgeMirror.GetReflection<Edge>();
		// 		SetupEdgeAfterReflection(__edge);
		// 	}

		// 	_isNotifiable = true;
		// }

		public void UpdateEditorFromModel(NodeGraphData data)
		{
			if (data == null)
				return;

			_isNotifiable = false;
			ClearAllNodes();

			SetViewPosition(data.viewPosition);

			foreach (var iModelNode in data.nodeData)
				CreateNewNodeFromModel(iModelNode);

			if (_shouldCreatePredefinedNodes)
				CreatePredefinedNodes();

			foreach (var iModelEdge in data.edgeData)
				ConnectPortsFromModelEdge(iModelEdge);

			_isNotifiable = true;
		}

		#endregion
		#region Overrides

		public override List<UnityEditor.Experimental.GraphView.Port> GetCompatiblePorts(UnityEditor.Experimental.GraphView.Port startPort, NodeAdapter nodeAdapter)
		{
			var __result = new List<UnityEditor.Experimental.GraphView.Port>();

			ports.ForEach((port) =>
			{
				if (
					startPort != port &&
					startPort.node != port.node &&
					startPort.direction != port.direction &&
					startPort.portType == port.portType
				)
					__result.Add(port);
			});

			return __result;
		}

		#endregion
		#region Notifies

		// private void OnNodeCreation(NodeCreationContext context)
		// {

		// }

		private GraphViewChange OnGraphViewChanged(GraphViewChange context)
		{
			NotifyIsModified();

			return context;
		}

		protected void NotifyIsModified()
		{
			if (_isNotifiable)
				onModified.Invoke();
		}

		#endregion

		#region Context Menu

		private void _CreateContextMenu(ContextualMenuPopulateEvent context)
		{
			CreateContextMenu(context);
		}
		protected virtual void CreateContextMenu(ContextualMenuPopulateEvent context)
		{
			// context.menu.InsertAction(0, "Create Node", (_) =>
			// {
			// 	CreateNewNode<Node>();
			// });
			context.menu.InsertAction(1, "Create Sticky Note", (_) =>
			{
				CreateNewStickyNote();
			});
		}

		protected abstract void AddSearchWindow();

		#endregion
		#region Node Handling

		#region Predefined Nodes

		public virtual void CreatePredefinedNodes() { }

		#endregion
		#region Creation

		private void SetupNewNode(Node node, bool invokeOnModified = true)
		{
			node.InitInGraph(this);

			AddElement(node);

			node.RefreshAll();

			NotifyIsModified();
		}

		public Node CreateNewNode(Type type, Vector2? position = null, bool invokeOnModified = true)
		{
			var __node = (Node)Activator.CreateInstance(type);

			if (position.HasValue)
				__node.position = position.Value;

			SetupNewNode(__node, invokeOnModified);

			return __node;
		}
		public T CreateNewNode<T>(Vector2? position = null, bool invokeOnModified = true)
		where T : Node =>
			(T)CreateNewNode(typeof(T), position, invokeOnModified);

		public Node CreateNewNodeAtCursor(Type type) =>
			CreateNewNode(type, mousePosition);

		public Node CreateNewNodeFromModel(Model.Node modelNode)
		{
			if (modelNode.isPredefined)
				_shouldCreatePredefinedNodes = false;

			var __node = (Node)Activator
			.CreateInstance(
				modelNode.editorType,
				new object[] { modelNode }
			);

			SetupNewNode(__node, false);

			return __node;
		}

		#endregion

		#region Retrieval

		public IEnumerable<Node> GetAllNodes() =>
			nodes.Cast<Node>();

		public Node GetNodeByGuid(Guid guid)
		{
			foreach (var iNode in GetAllNodes())
				if (iNode.guid == guid)
					return iNode;
			return null;
		}

		public Node GetNodeByType(Type type)
		{
			foreach (var iNode in GetAllNodes())
				if (type.IsAssignableFrom(iNode.GetType()))
					return iNode;
			return null;
		}

		public T GetNodeByType<T>()
		where T : Node =>
			(T)GetNodeByType(typeof(T));

		#endregion

		#region Manipulation

		protected override bool canPaste => true;

		private string OnCopy(IEnumerable<GraphElement> elements) =>
			GUIUtility.systemCopyBuffer = JsonTranslator.Encode(elements);

		private void OnPaste(string operationName, string data)
		{
			GraphElement[] __elements = JsonTranslator.Decode<GraphElement[]>(data);

			if (!__elements.Any())
				return;

			/** <<============================================================>> **/

			var __links = new Dictionary<Guid, Guid>();

			/** <<============================================================>> **/

			var __nodes = __elements.Where(i => i is Node).Cast<Node>();

			var __averagePosition = __nodes.Select(i => i.position).Average();
			var __deltaMousePosition = mousePosition - __averagePosition;

			foreach (var iNode in __nodes)
			{
				var __guid = Guid.GenerateNew();
				__links.Add(iNode.guid, __guid);

				iNode.guid = __guid;
				iNode.position = iNode.position + __deltaMousePosition;

				SetupNewNode(iNode);
			}

			/** <<============================================================>> **/

			var __extractedEdges = __elements.Where(i => i is Edge).Cast<Edge>();
			var __edges = new List<Edge>();

			foreach (var iEdge in __extractedEdges)
			{
				var __edgeTuple = (Tuple<Guid, string, Guid, string>)iEdge.userData;

				var __nodeOut = FindNode(__links.GetValueOrDefault(__edgeTuple.Item1, __edgeTuple.Item1));
				var __portOut = __nodeOut.GetPortByName(__edgeTuple.Item2);
				var __nodeIn = FindNode(__links.GetValueOrDefault(__edgeTuple.Item3, __edgeTuple.Item3));
				var __portIn = __nodeIn.GetPortByName(__edgeTuple.Item4);

				var __edge = __portOut.ConnectTo(__portIn);
				__edges.Add(__edge);
				AddElement(__edge);
			}

			/** <<============================================================>> **/

			var __newSelection = new List<ISelectable>();

			__newSelection.AddRange(__nodes);
			__newSelection.AddRange(__edges);

			this.SetSelection(__newSelection);

			NotifyIsModified();
		}

		private void OnDelete(string operationName, AskUser askUser)
		{
			var __toRemove = new List<GraphElement>();
			foreach (GraphElement i in selection)
			{
				if (i is Node iNode)
				{
					if (iNode.isPredefined)
						continue;

					var __connectedEdges = iNode.GetAllConnectedEdges();

					__toRemove.AddRange(__connectedEdges);
				}

				__toRemove.Add(i);
			}

			if (!__toRemove.Any())
				return;

			DeleteElements(__toRemove);

			NotifyIsModified();
		}

		public void ClearAllNodes()
		{
			var __toRemove = new List<GraphElement>();
			var __nodes = nodes.Cast<Node>().ToList();
			foreach (var iNode in __nodes)
			{
				if (iNode != null)
				{
					if (iNode.isPredefined)
						continue;
				}

				edges.Where(i => (Node)i.input.node == iNode).ToList()
					.ForEach(iEdge => { __toRemove.Add(iEdge); });

				__toRemove.Add(iNode);
			}

			if (!__toRemove.Any())
				return;

			DeleteElements(__toRemove);

			NotifyIsModified();
		}

		public void ClearAllNodes_WithPrompt()
		{
			if (nodes.Any())
				try { Utils.PromptConfirmation("Are you sure you want to clear ALL nodes on this graph?"); }
				catch { return; }

			ClearAllNodes();
		}

		#endregion

		#region Modification

		public Edge CreateEdge(EdgeData edge)
		{
			var nPort = FindNode(edge.nPort.NodeGuid).GetPortByName(edge.nPort.PortName);
			var oPort = FindNode(edge.oPort.NodeGuid).GetPortByName(edge.oPort.PortName);

			return ConnectPorts(nPort, oPort);
		}

		public Edge ConnectPorts(UnityEditor.Experimental.GraphView.Port input, UnityEditor.Experimental.GraphView.Port output)
		{
			var __edge = new Edge
			{
				input = input,
				output = output
			};

			__edge?.input.Connect(__edge);
			__edge?.output.Connect(__edge);

			Add(__edge);

			return __edge;
		}

		#endregion

		#region Query

		public Node FindNode(Guid guid)
		{
			foreach (var i in nodes)
				if (i is Node iNode && iNode.guid == guid)
					return iNode;

			throw new System.Exception($"Node with GUID {guid} was not found in {this.name}.");
		}

		#endregion

		#endregion
		#region Edge Handling

		public IEnumerable<Edge> GetAllEdges() =>
			edges;

		public Edge ConnectPortsFromModelEdge(Model.Edge modelEdge)
		{
			try
			{
				var __nodeIn = GetNodeByGuid(modelEdge.guidIn);
				var __portIn = __nodeIn.GetPortByName(modelEdge.portIn);

				var __nodeOut = GetNodeByGuid(modelEdge.guidOut);
				var __portOut = __nodeOut.GetPortByName(modelEdge.portOut);

				var __result = ConnectPorts(__portIn, __portOut);
				// AddElement(__result);

				return __result;
			}
			catch
			{
				Debug.LogWarning($"Attempted to create an edge connection between nodes but either a port or node has been unexpectedly removed.");
				return null;
			}
		}

		public void SetupEdgeAfterReflection(Edge edge)
		{
			var __edgeTuple = (Tuple<Guid, string, Guid, string>)edge.userData;

			var __nodeOut = FindNode(__edgeTuple.Item1);
			var __portOut = __nodeOut.GetPortByName(__edgeTuple.Item2);
			var __nodeIn = FindNode(__edgeTuple.Item3);
			var __portIn = __nodeIn.GetPortByName(__edgeTuple.Item4);

			AddElement(__portOut.ConnectTo(__portIn));
		}

		#endregion
		#region Sticky Notes

		public StickyNote CreateNewStickyNote()
		{
			Debug.Log(typeof(StickyNote).GetSerializableFields().ContentsToString());

			var __result = new StickyNote
			{
				contents = "This is a sticky note!",
				title = "Sticky Note Title",
				capabilities = Capabilities.Movable | Capabilities.Deletable
			};
			__result.SetPositionOnly(_mousePosition);


			AddElement(__result);


			return __result;
		}

		#endregion

		#region Utils

		public void SetViewPosition(Vector2 position) =>
			UpdateViewTransform(position, this.viewTransform.scale);

		private void OnMouseMove(MouseMoveEvent context)
		{
			_mousePosition = viewTransform.matrix.inverse.MultiplyPoint(context.localMousePosition);
			_hoveredNode = panel.Pick(context.mousePosition).FindRootElement<Node>();
		}

		#endregion

		#endregion
	}
}
