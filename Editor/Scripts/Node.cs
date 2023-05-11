
/** Node.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
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

		public static readonly string EXEC_LABEL_IN = "execIn";
		public static readonly string EXEC_LABEL_OUT = "execOut";

		protected static readonly Dictionary<string, Type> PRESET_PORTS_IN_EXEC = new Dictionary<string, Type>
		{ { EXEC_LABEL_IN, typeof(Exec) } };
		protected static readonly Dictionary<string, Type> PRESET_PORTS_OUT_EXEC = new Dictionary<string, Type>
		{ { EXEC_LABEL_OUT, typeof(Exec) } };

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

		private bool _isBeingHandledByUser = false;
		public bool isBeingHandledByUser => _isBeingHandledByUser;

		/** <<============================================================>> **/
		/**	Properties
		*/

		public virtual string defaultTitle => "New Custom Node";
		public virtual Orientation defaultOrientation => Orientation.Horizontal;
		public virtual Vector2 defaultSize => new Vector2(
			DEFAULT_NODE_WIDTH,
			NODE_HEADER_HEIGHT + (NODE_PORT_HEIGHT * maxPortCount)
		);
		public virtual System.Type DataType => typeof(NodeData);

		public Rect rect
		{
			// get => this.GetPosition();
			// set => this.SetPosition(value);
			get => new Rect(position, size);
			set
			{
				position = value.position;
				size = value.size;
			}
		}

		public Vector2 position
		{
			// get => this.GetPositionOnly();
			// set => this.SetPositionOnly(value);
			get => new Vector2(style.left.value.value, style.top.value.value);
			set
			{
				style.left = value.x;
				style.top = value.y;
			}
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
			Math.Max(GetPorts_In().Count, GetPorts_Out().Count);

		#endregion
		#region Methods

		#region Construction

		public Node()
		{
			AssignNewGuid();
			title = defaultTitle;
			onModified = new UnityEvent();

			CreateDefaultPorts();

			RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
		}

		public virtual void InitInGraph(NodeGraphView graph)
		{
			onModified.AddListener(() => { graph.onModified.Invoke(); });
		}

		public void AssignNewGuid() =>
			guid = Guid.GenerateNew();

		#endregion

		#region Fundamentals

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Node that)
				return guid.Equals(that.guid);
			return false;
		}

		public override int GetHashCode() =>
			guid.GetHashCode();

		public static bool operator ==(Node left, Node right) =>
			left.guid.Equals(right.guid);
		public static bool operator !=(Node left, Node right) =>
			!left.guid.Equals(right.guid);

		public override string ToString() =>
			$"{title} [{GetType().Name}] : {position}";

		#endregion
		#region Serialization

		public virtual void OnAfterDeserialize()
		{
			title = _title;
			rect = _rect;
		}

		public virtual void OnBeforeSerialize()
		{
			_title = title;
			_rect = rect;
		}

		#endregion
		#region Events

		protected virtual void OnGeometryChangedEvent(GeometryChangedEvent context) { }

		protected void NotifyIsModified() =>
			onModified.Invoke();

		#endregion

		#region Manipulation

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

		#endregion

		#region Port Handling

		#region Retrieval

		public VisualElement GetPortContainerFor(UnityEditor.Experimental.GraphView.Port port) =>
			port.direction == Direction.Input ?
			inputContainer : outputContainer;

		public UnityEditor.Experimental.GraphView.Port.Capacity GetPortCapacityForType(Type type, Direction direction) =>
			direction == Direction.Input ^ typeof(Exec) == type ?
						UnityEditor.Experimental.GraphView.Port.Capacity.Single : UnityEditor.Experimental.GraphView.Port.Capacity.Multi;

		public List<UnityEditor.Experimental.GraphView.Port> GetPorts_All()
		{
			var __result = GetPorts_In();
			__result.AddRange(GetPorts_Out());
			return __result;
		}
		public List<UnityEditor.Experimental.GraphView.Port> GetPorts_In() =>
						inputContainer.Query<UnityEditor.Experimental.GraphView.Port>().ToList();
		public List<UnityEditor.Experimental.GraphView.Port> GetPorts_Out() =>
						outputContainer.Query<UnityEditor.Experimental.GraphView.Port>().ToList();

		public UnityEditor.Experimental.GraphView.Port GetPortByName(string portName)
		{
			try
			{ return GetPorts_All().Find(i => i.portName == portName); }
			catch
			{ throw new System.Exception($"The port \"{portName}\" was not found on {title}."); }
		}

		#endregion
		#region Creation

		public void SetupPort(UnityEditor.Experimental.GraphView.Port port)
		{
			GetPortContainerFor(port).Add(port);

			RefreshAll();
			NotifyIsModified();
		}

		public UnityEditor.Experimental.GraphView.Port CreatePort(System.Type portType, string portName, Direction direction)
		{
			var __port = InstantiatePort(
				defaultOrientation,
				direction,
				GetPortCapacityForType(portType, direction),
				portType
			);

			__port.portName = portName;

			SetupPort(__port);

			return __port;
		}
		public UnityEditor.Experimental.GraphView.Port CreatePort<T>(string portName, Direction direction) =>
			CreatePort(typeof(T), portName, direction);

		public UnityEditor.Experimental.GraphView.Port CreateExecutiveInputPort(string name = null) =>
			CreatePort<Exec>(name ?? EXEC_LABEL_IN, Direction.Input);
		public UnityEditor.Experimental.GraphView.Port CreateExecutiveOutputPort(string name = null) =>
			CreatePort<Exec>(name ?? EXEC_LABEL_OUT, Direction.Output);

		protected virtual void CreateDefaultPorts()
		{
			CreateDefaultPortsIn();
			CreateDefaultPortsOut();
		}

		private void CreateDefaultPortsIn()
		{
			foreach (var i in defaultPortsIn)
			{
				var iName = i.Key;
				var iType = i.Value;

				CreatePort(iType, iName, Direction.Input);
			}
		}

		private void CreateDefaultPortsOut()
		{
			foreach (var i in defaultPortsOut)
			{
				var iName = i.Key;
				var iType = i.Value;

				CreatePort(iType, iName, Direction.Output);
			}
		}

		protected virtual Dictionary<string, Type> defaultPortsIn => new Dictionary<string, Type>();
		protected virtual Dictionary<string, Type> defaultPortsOut => new Dictionary<string, Type>();

		#endregion
		#region Destruction

		public void RemovePort(UnityEditor.Experimental.GraphView.Port port)
		{
			GetPortContainerFor(port).Remove(port);

			RefreshAll();
			NotifyIsModified();
		}

		#endregion

		#endregion

		#region Edge Handling

		#region Retrieval

		public List<Edge> GetAllConnectedEdges()
		{
			var __result = new List<Edge>();
			var __ports = GetPorts_All();

			foreach (var iPort in __ports)
			{
				// __result.AddRange(iPort.connections);
			}

			return __result;
		}

		#endregion

		#endregion

		#endregion
	}

	public struct Exec { }
}
