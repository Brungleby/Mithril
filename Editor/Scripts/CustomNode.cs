
/** CustomNode.cs
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

	public class CustomNode : Node
	{
		#region Data

		#region Static

		public static readonly Vector2 DEFAULT_NODE_SIZE = new Vector2(150f, 200f);

		#endregion
		#region

		public GUID Guid;
		public bool IsPredefined = false;

		public UnityEvent OnModified;

		#endregion

		#endregion
		#region Methods

		#region Construction

		public CustomNode()
		{
			Guid = GUID.Generate();
			this.title = DefaultName;
			SetPosition(new Rect(Vector2.zero, DefaultSize));
		}

		public virtual string DefaultName => "New Custom Node";
		public virtual Vector2 DefaultSize => DEFAULT_NODE_SIZE;
		public virtual Orientation DefaultOrientation => Orientation.Horizontal;

		#endregion

		#region

		public void RefreshAll()
		{
			RefreshExpandedState();
			RefreshPorts();
		}

		public virtual void InitializeFor(CustomNodeGraphView graph)
		{
			OnModified = new UnityEvent();
			OnModified.AddListener(() =>
			{
				graph.OnModified.Invoke();
			});

			// this.
		}

		#endregion

		#region Port Handling

		public VisualElement GetPortContainerFor(Port port)
		{
			switch (port.direction)
			{
				case Direction.Input:
					return inputContainer;
				case Direction.Output:
					return outputContainer;
				default:
					throw new System.Exception($"This port ({port}) has an invalid direction.");
			}
		}

		public List<Port> GetAllPorts()
		{
			var __result = GetInputPorts();
			__result.AddRange(GetOutputPorts());
			return __result;
		}
		public List<Port> GetInputPorts() =>
			inputContainer.Query<Port>().ToList();
		public List<Port> GetOutputPorts() =>
			outputContainer.Query<Port>().ToList();

		public Port CreatePort(System.Type type, string portName, Direction direction, Port.Capacity? capacity = null, Orientation? orientation = null)
		{
			var __port = InstantiatePort(
				orientation ?? DefaultOrientation,
				direction,
				capacity ?? (direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi),
				type
			);

			__port.portName = portName;

			AttachPort(__port);

			return __port;
		}
		public Port CreatePort(string portName, Direction direction, Port.Capacity? capacity = null, Orientation? orientation = null) =>
			CreatePort(typeof(bool), portName, direction, capacity, orientation);
		public Port CreatePort<T>(string name, Direction direction, Port.Capacity? capacity = null, Orientation? orientation = null) =>
			CreatePort(typeof(T), name, direction, capacity, orientation);
		public Port CreatePort(NodeGraphEditableObject.PortData data) =>
			CreatePort(data.Type, data.PortName, data.Direction, data.Capacity, data.Orientation);


		public void AttachPort(Port port)
		{
			var __portContainer = GetPortContainerFor(port);
			__portContainer.Add(port);

			RefreshAll();
		}

		public void RemovePort(Port port)
		{
			var __portContainer = GetPortContainerFor(port);

			__portContainer.Remove(port);

			RefreshAll();
		}

		public Port FindPort(string portName)
		{
			foreach (var iPort in GetAllPorts())
			{
				if (portName == iPort.portName)
					return iPort;
			}

			throw new KeyNotFoundException();
		}

		public Port CreateExecutiveInputPort(string name = null) =>
			CreatePort<bool>(name ?? "In", Direction.Input, Port.Capacity.Multi);
		public Port CreateExecutiveOutputPort(string name = null) =>
			CreatePort<bool>(name ?? "Out", Direction.Output, Port.Capacity.Single);

		#endregion

		#endregion
	}
}
