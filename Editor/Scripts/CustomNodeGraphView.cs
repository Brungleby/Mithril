
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
		#region Data

		#region

		public UnityEvent OnModified;

		private NodeSearchWindow _searchWindow;

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
					new SearchTreeEntry(new GUIContent("Custom Node")) { level = 1, userData = typeof(CustomNode) },

			};

			return __result;
		}

		#endregion
		#region Methods

		#region Construction

		public CustomNodeGraphView()
		{
			OnModified = new UnityEvent();

			styleSheets.Add(Resources.Load<StyleSheet>("Stylesheets/GraphBackgroundDefault"));

			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			// this.AddManipulator(new ContextualMenuManipulator(_CreateContextMenu));

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

		#endregion

		#region Overrides

		protected override bool canDeleteSelection
		{
			get
			{
				foreach (var i in selection)
				{
					if (i is CustomNode iNode && iNode.IsPredefined)
						return false;
				}

				return base.canDeleteSelection;
			}
		}

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
				// CreateNewNode<CustomNode>("New Node");
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

		public CustomNode CreateNewNode(Type type, GUID guid, Rect rect, bool invokeOnModified = true)
		{
			var __node = (CustomNode)Activator.CreateInstance(type);

			__node.Guid = guid;
			__node.title = __node.DefaultName;
			__node.SetPosition(rect);
			__node.InitializeFor(this);

			AddElement(__node);

			if (invokeOnModified)
				OnModified.Invoke();

			return __node;
		}
		public CustomNode CreateNewNode(Type type, GUID? guid = null, Rect? rect = null, string title = null, bool invokeOnModified = true)
		{
			var __node = (CustomNode)Activator.CreateInstance(type);

			if (guid != null)
				__node.Guid = guid.Value;
			if (rect != null)
				__node.SetPosition(rect.Value);
			if (title != null)
				__node.title = title;

			__node.InitializeFor(this);

			AddElement(__node);

			if (invokeOnModified)
				OnModified.Invoke();

			return __node;
		}
		public CustomNode CreateNewNodeAtCursor(Type type, GUID? guid = null, Vector2? size = null, string title = null, bool invokeOnModified = true)
		{
			var __node = CreateNewNode(type, guid, null, title, invokeOnModified);

			__node.SetPosition(new Rect(
				mousePosition,
				(size != null) ?
					size.Value :
					__node.GetPosition().size
			));

			return __node;
		}

		public CustomNode CreateNewNode(NodeGraphEditableObject.NodeData data) =>
			CreateNewNode(Type.GetType(data.SubtypeName), data.Guid, data.Rect, data.Title, false);

		public T CreateNewNode<T>(GUID? guid = null, Rect? rect = null, string title = null, bool invokeOnModified = true)
		where T : CustomNode, new() =>
			(T)CreateNewNode(typeof(T), guid, rect, title, invokeOnModified);

		public void ClearAllNodes()
		{
			var __nodes = nodes.Cast<CustomNode>();
			foreach (var iNode in __nodes)
			{
				if (iNode != null)
				{
					if (iNode.IsPredefined)
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

		public virtual void CreatePredefinedNodes() { }

		public CustomNode FindNode(GUID guid)
		{
			foreach (var i in nodes)
			{
				if (i is CustomNode iNode && iNode.Guid == guid)
					return iNode;
			}

			throw new KeyNotFoundException();
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
