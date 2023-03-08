
/** CustomNodeGraphView.cs
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

namespace Cuberoot.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public class CustomNodeGraphView : GraphView
	{
		#region Construction

		public CustomNodeGraphView()
		{
			OnModified = new UnityEvent();

			styleSheets.Add(Resources.Load<StyleSheet>("Stylesheets/GraphBackgroundDefault"));

			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new ContextualMenuManipulator(_CreateContextMenu));

			RegisterCallback<MouseMoveEvent>(OnMouseMove);

			var __background = new GridBackground();
			Insert(0, __background);
			__background.StretchToParentSize();

			this.StretchToParentSize();

			/**	Didn't work when I tried it
			*/
			// FrameAll();

			AddSearchWindow();
		}

		#endregion
		#region Data

		#region

		public UnityEvent OnModified;

		private NodeSearchWindow _searchWindow;

		private Vector2 _mousePosition;
		public Vector2 mousePosition => _mousePosition;

		#endregion

		#endregion
		#region Properties

		public virtual Vector2 DefaultNodeSize => CustomNode.DEFAULT_NODE_SIZE;
		public virtual string DefaultNodeName => "New Custom Node";

		public virtual List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			var __result = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("Available Nodes"), 0),
					new SearchTreeEntry(new GUIContent("Custom Node")) { level = 1, userData = typeof(CustomNode) },

			};

			return __result;
		}

		#endregion
		#region Methods

		#region Overrides

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			var __result = new List<Port>();

			ports.ForEach((port) =>
			{
				if (startPort != port && startPort.node != port.node)
					__result.Add(port);
			});

			return __result;
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
				CreateNewNode<CustomNode>("New Node");
			});
		}

		private void AddSearchWindow()
		{
			_searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
			nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
			_searchWindow.InitializeFor(this);
		}

		#endregion
		#region Node Handling


		public CustomNode CreateNewNode(Type type, GUID guid, string title, Rect rect, bool invokeOnModified = true)
		{
			var __node = (CustomNode)Activator.CreateInstance(type);

			__node.Guid = GUID.Generate();
			__node.title = title;
			__node.SetPosition(rect);
			__node.InitializeFor(this);

			AddElement(__node);

			if (invokeOnModified)
				OnModified.Invoke();

			return __node;
		}
		public CustomNode CreateNewNode(Type type, string title, Rect rect) =>
			CreateNewNode(type, GUID.Generate(), title, rect);
		public CustomNode CreateNewNode(Type type, GUID guid, string title, Vector2 position) =>
			CreateNewNode(type, guid, title, new Rect(position, DefaultNodeSize));
		public CustomNode CreateNewNode(Type type, string title, Vector2 position) =>
			CreateNewNode(type, GUID.Generate(), title, new Rect(position, DefaultNodeSize));
		public CustomNode CreateNewNode(Type type, GUID guid, string title) =>
			CreateNewNode(type, guid, title, new Rect(mousePosition, DefaultNodeSize));
		public CustomNode CreateNewNode(Type type, string title) =>
			CreateNewNode(type, GUID.Generate(), title, new Rect(mousePosition, DefaultNodeSize));
		public CustomNode CreateNewNode(Type type) =>
			CreateNewNode(type, DefaultNodeName);

		public T CreateNewNode<T>(GUID guid, string title, Rect rect, bool invokeOnModified = true)
		where T : CustomNode, new() =>
			(T)CreateNewNode(typeof(T), guid, title, rect, invokeOnModified);
		public T CreateNewNode<T>(string title, Rect rect)
		where T : CustomNode, new() =>
			CreateNewNode<T>(GUID.Generate(), title, rect);
		public T CreateNewNode<T>(GUID guid, string title, Vector2 position)
		where T : CustomNode, new() =>
			CreateNewNode<T>(guid, title, new Rect(position, DefaultNodeSize));
		public T CreateNewNode<T>(string title, Vector2 position)
		where T : CustomNode, new() =>
			CreateNewNode<T>(GUID.Generate(), title, new Rect(position, DefaultNodeSize));
		public T CreateNewNode<T>(GUID guid, string title)
		where T : CustomNode, new() =>
			CreateNewNode<T>(guid, title, new Rect(mousePosition, DefaultNodeSize));
		public T CreateNewNode<T>(string title)
		where T : CustomNode, new() =>
			CreateNewNode<T>(GUID.Generate(), title, new Rect(mousePosition, DefaultNodeSize));
		public T CreateNewNode<T>()
		where T : CustomNode, new() =>
			CreateNewNode<T>(DefaultNodeName);

		public void ClearAllNodes()
		{
			foreach (var iNode in nodes)
			{
				var iBasicNode = (CustomNode)iNode;
				if (iBasicNode != null)
				{
					if (iBasicNode.IsPredefined)
						continue;
				}

				edges.Where(i => i.input.node == iNode).ToList()
					.ForEach(iEdge => { RemoveElement(iEdge); });

				RemoveElement(iNode);
			}
		}

		public void ClearAllNodes_WithPrompt()
		{
			if (nodes.Any())
				try { Utils.PromptConfirmation("Are you sure you want to clear ALL nodes on this graph?"); }
				catch { return; }

			ClearAllNodes();
		}

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
