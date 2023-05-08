
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

using NodeData = Mithril.NodeData;
using EdgeData = Mithril.EdgeData;

#endregion

namespace Mithril.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public class NodeGraphView : GraphView
	{
		#region Inner Classes

		[Serializable]

		private struct Clipboard : ISerializable
		{
			[SerializeField]
			public NodeData[] nodeData;

			[SerializeField]
			public EdgeData[] edgeData;

			[SerializeField]
			public Vector2 averagePosition;

			public Clipboard(IEnumerable<GraphElement> elements)
			{
				var __nodes = new List<NodeData>();
				var __edges = new List<EdgeData>();

				foreach (var iElement in elements)
				{
					if (iElement is Node iNode)
						__nodes.Add(NodeData.CreateFrom(iNode));
					else if (iElement is Edge iEdge)
						__edges.Add(iEdge);
				}

				nodeData = __nodes.ToArray();
				edgeData = __edges.ToArray();

				averagePosition = __nodes.Select(i => i.rect.position).Average();
			}
		}

		#endregion
		#region Data

		#region

		public UnityEvent onModified;

		private NodeGraphSearchSubwindow _searchWindow;

		private Vector2 _mousePosition;
		public Vector2 mousePosition => _mousePosition;

		#endregion

		#endregion
		#region Properties

		public virtual List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			var __result = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("Available Nodes"), 0),
					new SearchTreeEntry(new GUIContent("Custom Node")) { level = 1, userData = typeof(Node) },

			};

			return __result;
		}

		#endregion
		#region Methods

		#region Construction

		public NodeGraphView()
		{
			onModified = new UnityEvent();

			styleSheets.Add(Resources.Load<StyleSheet>("Stylesheets/GraphBackgroundDefault"));

			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			// this.AddManipulator(new ContextualMenuManipulator(_CreateContextMenu));

			nodeCreationRequest += OnNodeCreation;
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

			/**	Didn't work when I tried it
			*/
			// FrameAll();
		}

		public void InitFromGraphData(NodeGraphData data)
		{
			/** <<============================================================>> **/
			//
			SetViewPosition(data.viewPosition);

			/** <<============================================================>> **/
			/**	Nodes
			*/

			if (data != null)
				foreach (var iNodeMirror in data.nodeMirrors)
				{
					var __node = iNodeMirror.GetReflection<Node>();
					__node.OnAfterDeserialize();
					AddNodeToView(__node);
				}
		}

		#endregion
		#region Overrides

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			var __result = new List<Port>();

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

		private void OnNodeCreation(NodeCreationContext context)
		{

		}

		private GraphViewChange OnGraphViewChanged(GraphViewChange context)
		{
			// Debug.Log($"Graph view changed;");

			onModified.Invoke();

			return context;
		}

		#endregion

		#region Context Menu

		private void _CreateContextMenu(ContextualMenuPopulateEvent context)
		{
			// CreateContextMenu(context);
			// context.menu.InsertSeparator("/", 1);
		}
		protected virtual void CreateContextMenu(ContextualMenuPopulateEvent context)
		{
			context.menu.InsertAction(0, "Create Node", (_) =>
			{
				// CreateNewNode<CustomNode>("New Node");
			});
		}

		private void AddSearchWindow()
		{
			_searchWindow = ScriptableObject.CreateInstance<NodeGraphSearchSubwindow>();
			nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
			_searchWindow.InitializeFor(this);
		}

		#endregion
		#region Node Handling

		#region Predefined Nodes

		public virtual void CreatePredefinedNodes() { }

		#endregion
		#region Creation

		private void AddNodeToView(Node node, bool invokeOnModified = true)
		{
			node.InitInGraph(this);

			AddElement(node);
			node.RefreshAll();

			if (invokeOnModified)
				onModified.Invoke();
		}

		public Node CreateNewNode(Type type, Guid guid, Rect rect, string title = null, bool invokeOnModified = true)
		{
			var __node = (Node)Activator.CreateInstance(type);

			if (title != null)
				__node.title = title;

			__node.guid = guid;
			__node.SetPosition(rect);

			AddNodeToView(__node);

			return __node;
		}
		public Node CreateNewNode(Type type, Guid guid) =>
			CreateNewNode(type, guid, new Rect(Vector2.zero, Node.DEFAULT_NODE_SIZE));
		public Node CreateNewNode(Type type) =>
			CreateNewNode(type, Guid.GenerateNew());

		public T CreateNewNode<T>(Guid guid, Rect rect, string title = null, bool invokeOnModified = true)
		where T : Node, new() =>
			(T)CreateNewNode(typeof(T), guid, rect, title, invokeOnModified);
		public T CreateNewNode<T>(Guid guid, string title = null, bool invokeOnModified = true)
		where T : Node, new() =>
			CreateNewNode<T>(guid, new Rect(Vector2.zero, Node.DEFAULT_NODE_SIZE), title, invokeOnModified);
		public T CreateNewNode<T>(string title = null, bool invokeOnModified = true)
		where T : Node, new() =>
			CreateNewNode<T>(Guid.GenerateNew(), title, invokeOnModified);

		// public Node CreateNewNode(NodeData data, bool createNewGuid = false) =>
		// 	CreateNewNode(
		// 		Type.GetType(data.nodeType),
		// 		createNewGuid ? Guid.GenerateNew() : data.guid,
		// 		data.rect, data.title, false
		// 	)
		// ;
		// public Node CreateNewNode<T>(T data, bool createNewGUID = false)
		// where T : NodeData
		// {
		// 	var __node = (Node)Activator.CreateInstance(data.nodeType);

		// 	__node.Init(data);
		// 	__node.InitInGraph(this);

		// 	AddNodeToView(__node);

		// 	return __node;
		// }

		public Node CreateNewNodeAtCursor(Type type, Guid guid, Vector2 size, string title = null, bool invokeOnModified = true) =>
			CreateNewNode(type, guid, new Rect(mousePosition, size), title, invokeOnModified);
		public Node CreateNewNodeAtCursor(Type type, Guid guid) =>
			CreateNewNodeAtCursor(type, guid, Node.DEFAULT_NODE_SIZE);
		public Node CreateNewNodeAtCursor(Type type) =>
			CreateNewNodeAtCursor(type, Guid.GenerateNew());

		#endregion

		public T GetNode<T>()
		where T : Node
		{
			foreach (var i in nodes)
			{
				if (typeof(T).IsAssignableFrom(i.GetType()))
					return (T)i;
			}
			return null;
		}

		#region Manipulation

		protected override bool canPaste => true;

		private string OnCopy(IEnumerable<GraphElement> elements)
		{
			ISerializable __clipboard = new Clipboard(elements);

			Editor.Clipboard.Copy(__clipboard);

			return GUIUtility.systemCopyBuffer;
		}

		private void OnPaste(string operationName, string data)
		{
			var __clipboard = Editor.Clipboard.PasteText<Clipboard>(data);

			OnPaste(__clipboard);
		}

		private void OnPaste(Clipboard clipboard)
		{
			// /** <<============================================================>> **/

			// var __guidLinks = new MapField<Guid, Guid>();

			// /** <<============================================================>> **/

			// var __nodes = new List<Node>();

			// var __deltaPosition = mousePosition - clipboard.averagePosition;

			// foreach (var iNodeData in clipboard.nodeData)
			// {
			// 	var iNode = CreateNewNode(iNodeData, false);
			// 	iNode.SetPositionOnly(iNodeData.rect.position + __deltaPosition);

			// 	__nodes.Add(iNode);
			// 	__guidLinks.Add((iNodeData.guid, iNode.guid));
			// }

			// /** <<============================================================>> **/

			// var __edges = new List<Edge>();

			// foreach (var iEdgeData in clipboard.edgeData)
			// {
			// 	var __linkData = iEdgeData;

			// 	__linkData.nPort.NodeGuid = __guidLinks.TryGetValue(iEdgeData.nPort.NodeGuid);
			// 	__linkData.oPort.NodeGuid = __guidLinks.TryGetValue(iEdgeData.oPort.NodeGuid);

			// 	try
			// 	{ __edges.Add(CreateEdge(__linkData)); }
			// 	catch
			// 	{ continue; }
			// }

			// /** <<============================================================>> **/

			// var __newSelection = new List<ISelectable>();

			// __newSelection.AddRange(__nodes);
			// __newSelection.AddRange(__edges);

			// this.SetSelection(__newSelection);

			bool anythingHappened = false;
			if (anythingHappened)
				onModified.Invoke();
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

			DeleteElements(__toRemove);

			// var __anythingHappened = __toRemove.Count > 0;
			// if (__anythingHappened)
			// 	onModified.Invoke();
		}

		public void ClearAllNodes()
		{
			var __nodes = nodes.Cast<Node>().ToList();
			foreach (var iNode in __nodes)
			{
				if (iNode != null)
				{
					if (iNode.isPredefined)
						continue;
				}

				edges.Where(i => i.input.node == iNode).ToList()
					.ForEach(iEdge => { RemoveElement(iEdge); });

				RemoveElement(iNode);
			}

			var __anythingHappened = __nodes.Count > 0;
			if (__anythingHappened)
				onModified.Invoke();
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
			var nPort = FindNode(edge.nPort.NodeGuid).FindPort(edge.nPort.PortName);
			var oPort = FindNode(edge.oPort.NodeGuid).FindPort(edge.oPort.PortName);

			return ConnectPorts(nPort, oPort);
		}

		public Edge ConnectPorts(Port input, Port output)
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
		#region Utils

		public void SetViewPosition(Vector2 position) =>
			UpdateViewTransform(position, this.viewTransform.scale);

		private void OnMouseMove(MouseMoveEvent context)
		{
			_mousePosition = viewTransform.matrix.inverse.MultiplyPoint(context.localMousePosition);
		}

		#endregion

		#endregion
	}
}
