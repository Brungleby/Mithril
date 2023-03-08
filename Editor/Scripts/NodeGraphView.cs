
/** NodeGraphView.cs
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

		public readonly Vector2 DEFAULT_NODE_SIZE = new Vector2(150f, 200f);

		#endregion

		#endregion
		#region Methods

		#region Overrides

		/// <summary>
		/// Returns the list of compatible ports. By default, this will allow any connections as long as they are not on the same node and facing opposite directions.
		///</summary>

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

		private Port CreatePort(PoopyNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
		{
			return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
		}

		private PoopyNode CreateEntryPointNode()
		{
			var __node = new PoopyNode
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

		public PoopyNode CreateNewNode(string nodeName)
		{
			var __node = new PoopyNode
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

		public void CreateChoicePort(PoopyNode node, string name)
		{
			var __port = CreatePort(node, Direction.Output);

			var __oldLabel = __port.contentContainer.Q<Label>("type");
			__port.contentContainer.Remove(__oldLabel);

			var __textField = new TextField
			{
				name = string.Empty,
				value = name,
			};
			__textField.RegisterValueChangedCallback(context => __port.portName = context.newValue);

			var __deleteButton = new Button(() => RemovePort(node, __port))
			{
				text = "X",
			};
			__port.contentContainer.Add(__deleteButton);

			// __port.contentContainer.Add(new Label("  "));
			__port.contentContainer.Add(__textField);
			__port.portName = name;



			node.outputContainer.Add(__port);
			node.RefreshExpandedState();
			node.RefreshPorts();
		}
		public void CreateChoicePort(PoopyNode node)
		{
			var __outptCount = node.outputContainer.Query("connector").ToList().Count;
			var __outptName = $"Choice {__outptCount}";

			CreateChoicePort(node, __outptName);
		}

		private void RemovePort(PoopyNode node, Port port)
		{
			var __targetEdge = edges.ToList().Where(x => x.output.portName == port.portName && x.output.node == port.node);

			if (!__targetEdge.Any())
				return;

			var __edge = __targetEdge.First();
			__edge.input.Disconnect(__edge);
			RemoveElement(__targetEdge.First());

			node.outputContainer.Remove(port);
			node.RefreshExpandedState();
			node.RefreshPorts();
		}


		#endregion

		#endregion
	}
}
