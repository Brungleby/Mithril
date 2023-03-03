
/** BasicNodeGraphView.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;


#endregion

namespace Cuberoot.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public class BasicNodeGraphView : GraphView
	{
		#region Construction

		public BasicNodeGraphView()
		{
			styleSheets.Add(Resources.Load<StyleSheet>("Stylesheets/GraphBackgroundDefault"));

			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			var __background = new GridBackground();
			Insert(0, __background);
			__background.StretchToParentSize();
		}

		#endregion
		#region Data

		#region



		#endregion

		#endregion
		#region Properties

		public virtual Vector2 DefaultNodeSize => BasicNode.DEFAULT_NODE_SIZE;

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
		#region 

		public void ClearAllNodes()
		{
			foreach (var iNode in nodes)
			{
				var iBasicNode = (BasicNode)iNode;
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
		#region Node Handling

		public T CreateNewNode<T>(GUID guid, string title, Rect rect)
		where T : BasicNode, new()
		{
			var __node = new T
			{
				Guid = GUID.Generate(),
				title = title,
			};
			__node.SetPosition(rect);
			__node.InitializeFor(this);

			AddElement(__node);

			return __node;
		}
		public T CreateNewNode<T>(string title, Rect rect)
		where T : BasicNode, new() =>
			CreateNewNode<T>(GUID.Generate(), title, rect);
		public T CreateNewNode<T>(GUID guid, string title, Vector2 position)
		where T : BasicNode, new() =>
			CreateNewNode<T>(guid, title, new Rect(position, DefaultNodeSize));
		public T CreateNewNode<T>(string title, Vector2 position)
		where T : BasicNode, new() =>
			CreateNewNode<T>(GUID.Generate(), title, new Rect(position, DefaultNodeSize));
		public T CreateNewNode<T>(GUID guid, string title)
		where T : BasicNode, new() =>
			CreateNewNode<T>(guid, title, new Rect(Vector2.zero, DefaultNodeSize));
		public T CreateNewNode<T>(string title)
		where T : BasicNode, new() =>
			CreateNewNode<T>(GUID.Generate(), title, new Rect(Vector2.zero, DefaultNodeSize));

		#endregion

		#endregion
	}
}
