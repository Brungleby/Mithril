
/** Node.cs
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

namespace Mithril.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>
	[System.Serializable]

	public class Node : UnityEditor.Experimental.GraphView.Node, ISerializationCallbackReceiver
	{
		#region Data

		/** <<============================================================>> **/
		/**	Static Data
		*/

		public static readonly float DEFAULT_NODE_WIDTH = 150f;
		public static readonly float NODE_HEADER_HEIGHT = 50f;
		public static readonly float NODE_PORT_HEIGHT = 25f;
		public static readonly float TEXT_LINE_HEIGHT = 15f;

		public static Vector2 DEFAULT_NODE_SIZE => new Vector2(
			DEFAULT_NODE_WIDTH,
			NODE_HEADER_HEIGHT + NODE_PORT_HEIGHT
		);

		/** <<============================================================>> **/
		/**	Mirrored Data
		*/

		[SerializeField]
		private string _title;

		[SerializeField]
		private Rect _rect;

		[SerializeField]
		public Guid guid;

		[SerializeField]
		public bool isPredefined = false;

		private UnityEvent onModified;

		/** <<============================================================>> **/
		/**	Local Data
		*/

		/** <<============================================================>> **/
		/**	Properties
		*/

		public virtual string defaultName => "New Custom Node";
		public virtual Orientation defaultOrientation => Orientation.Horizontal;
		public virtual Vector2 defaultSize => new Vector2(
			DEFAULT_NODE_WIDTH,
			NODE_HEADER_HEIGHT + (NODE_PORT_HEIGHT * maxPortCount)
		);
		public virtual System.Type DataType => typeof(NodeData);

		public Vector2 position
		{
			get => this.GetPositionOnly();
			set => this.SetPositionOnly(value);
		}
		public Vector2 size
		{
			// get => this.GetSizeOnly();
			// set => this.SetSizeOnly(value);
			get => new Vector2(style.width.value.value, style.height.value.value);
			set
			{
				style.width = value.x;
				style.height = value.y;
			}
		}

		public int maxPortCount =>
			Math.Max(GetInputPorts().Count, GetOutputPorts().Count);

		#endregion
		#region Methods

		#region Construction

		public Node()
		{
			this.guid = Guid.GenerateNew();
			this.title = defaultName;
		}

		public Node(Mirror mirror)
		{

		}

		public virtual void Init(NodeData data)
		{
			guid = data.guid;
			title = data.title ?? defaultName;
			this.SetPositionOnly(data.rect.position);
		}

		public virtual void InitInGraph(NodeGraphView graph)
		{
			onModified = new UnityEvent();
			onModified.AddListener(() =>
			{
				graph.onModified.Invoke();
			});
		}

		#endregion

		#region ISerializable

		public string GetSerializedString() =>
			JsonUtility.ToJson(NodeData.CreateFrom(this));

		public void OnAfterDeserialize()
		{
			title = _title;
			SetPosition(_rect);
		}

		public void OnBeforeSerialize()
		{
			_title = title;
			_rect = GetPosition();
		}

		#endregion
		#region Notifies



		#endregion

		public override bool IsCopiable() =>
			!isPredefined;

		public void RefreshAll()
		{
			RefreshSize();
			RefreshExpandedState();
			RefreshPorts();
		}

		public void RefreshSize()
		{
			this.size = defaultSize;
		}

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
				orientation ?? defaultOrientation,
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
		public Port CreatePort(PortData data) =>
			CreatePort(System.Type.GetType(data.Type), data.PortName, data.Direction, data.Capacity, data.Orientation);


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

			throw new System.Exception($"The port \"{portName}\" was not found on {this.title}.");
		}

		public Port CreateExecutiveInputPort(string name = null) =>
			CreatePort<Exec>(name ?? "In", Direction.Input, Port.Capacity.Multi);
		public Port CreateExecutiveOutputPort(string name = null) =>
			CreatePort<Exec>(name ?? "Out", Direction.Output, Port.Capacity.Single);

		#endregion

		#region Edge Handling

		public List<Edge> GetAllConnectedEdges()
		{
			var __result = new List<Edge>();
			var __ports = GetAllPorts();

			foreach (var iPort in __ports)
			{
				__result.AddRange(iPort.connections);
			}

			return __result;
		}

		#endregion

		#endregion
	}

	public struct Exec { }
}
