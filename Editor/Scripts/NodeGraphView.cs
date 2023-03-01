
/** NodeGraphView.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor.Experimental.GraphView;

using GUID = UnityEditor.GUID;

#endregion

namespace Cuberoot.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class NodeGraphView : GraphView
	{
		#region Constructors

		public NodeGraphView()
		{
			styleSheets.Add(Resources.Load<StyleSheet>("Stylesheets/GraphBackgroundDefault"));

			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			var __background = new GridBackground();
			Insert(0, __background);
			__background.StretchToParentSize();



			AddElement(CreateEntryPointNode());
		}

		#endregion
		#region Data

		#region Static

		private readonly Vector2 DEFAULT_NODE_SIZE = new Vector2(150f, 200f);

		#endregion

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

		private Port CreatePort(Node node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
		{
			return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
		}

		private Node CreateEntryPointNode()
		{
			var __node = new Node
			{
				title = "START",
				GUID = GUID.Generate().ToString(),
				DialogueText = "ENTRYPOINT",
				EntryPoint = true
			};

			var __port = CreatePort(__node, Direction.Output);
			__port.portName = "Next";

			__node.outputContainer.Add(__port);

			__node.RefreshExpandedState();
			__node.RefreshPorts();

			__node.SetPosition(new Rect(100f, 200f, 100f, 150f));

			return __node;
		}

		public Node CreateNewNode(string nodeName)
		{
			var __node = new Node
			{
				title = nodeName,
				DialogueText = nodeName,
				GUID = GUID.Generate().ToString()
			};

			var __button = new Button(() =>
			{
				CreateChoicePort(__node);
			});
			__button.text = "+";
			__node.titleContainer.Add(__button);

			var __input_exec = CreatePort(__node, Direction.Input, Port.Capacity.Multi);
			__input_exec.portName = "In";
			__node.inputContainer.Add(__input_exec);

			__node.RefreshExpandedState();
			__node.RefreshPorts();
			__node.SetPosition(new Rect(Vector2.zero, DEFAULT_NODE_SIZE));

			AddElement(__node);

			return __node;
		}

		private void CreateChoicePort(Node node)
		{
			var __port = CreatePort(node, Direction.Output);

			var __outptCount = node.outputContainer.Query("connector").ToList().Count;
			var __outptName = $"Choice {__outptCount}";

			__port.portName = __outptName;

			node.outputContainer.Add(__port);
			node.RefreshExpandedState();
			node.RefreshPorts();
		}

		#endregion

		#endregion
	}
}
