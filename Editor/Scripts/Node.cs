
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
using EditorPort = UnityEditor.Experimental.GraphView.Port;

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

		public static readonly string EXEC_LABEL_IN = "_i";
		public static readonly string EXEC_LABEL_OUT = "_o";

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

		public virtual string defaultTitle => GetType().Name;
		public virtual Orientation defaultOrientation => Orientation.Horizontal;
		public virtual Vector2 defaultSize => new Vector2(
			DEFAULT_NODE_WIDTH,
			NODE_HEADER_HEIGHT + (NODE_PORT_HEIGHT * maxPortCount)
		);

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
			Math.Max(GetPortsIn().Count, GetPortsOut().Count);

		#endregion
		#region Methods

		#region Construction

		public Node()
		{
			AssignNewGuid();
			title = defaultTitle;

			OnConstruct();
			CreateDefaultPorts();
		}

		public Node(NodeData.Node model)
		{
			guid = model.guid;
			title = model.title;

			OnConstruct();

			foreach (var iModelPort in model.portsIn)
				CreatePort(iModelPort.portType, iModelPort.portName, Direction.Input);
			foreach (var iModelPort in model.portsOut)
				CreatePort(iModelPort.portType, iModelPort.portName, Direction.Output);

			rect = model.rect;

		}

		protected virtual void OnConstruct()
		{
			onModified = new UnityEvent();
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

		public virtual Type modelType => typeof(NodeData.Node);

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

		#region EditorPort Handling

		public static readonly IStyle EXEC_PORT_STYLE;

		protected virtual Dictionary<string, Type> defaultPortsIn => new Dictionary<string, Type>();
		protected virtual Dictionary<string, Type> defaultPortsOut => new Dictionary<string, Type>();

		#region Retrieval

		public VisualElement GetPortContainerFor(EditorPort port) =>
			port.direction == Direction.Input ?
			inputContainer : outputContainer;

		public EditorPort.Capacity GetPortCapacityForType(Type type, Direction direction) =>
			direction == Direction.Input ^ typeof(Exec) == type ?
						EditorPort.Capacity.Single : EditorPort.Capacity.Multi;

		public List<EditorPort> GetPorts_All()
		{
			var __result = GetPortsIn();
			__result.AddRange(GetPortsOut());
			return __result;
		}
		public List<EditorPort> GetPortsIn() =>
						inputContainer.Query<EditorPort>().ToList();
		public List<EditorPort> GetPortsOut() =>
						outputContainer.Query<EditorPort>().ToList();

		public EditorPort GetPortByName(string portName)
		{
			try
			{ return GetPorts_All().Find(i => i.portName == portName); }
			catch
			{ throw new System.Exception($"The port \"{portName}\" was not found on {title}."); }
		}

		#endregion
		#region Creation

		public void SetupPort(EditorPort port)
		{
			if (typeof(Exec) == port.portType)
				ChangeStyleToExecStyle(port);

			GetPortContainerFor(port).Add(port);

			RefreshAll();
			NotifyIsModified();
		}

		public EditorPort CreatePort(System.Type portType, string portName, Direction direction)
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
		public EditorPort CreatePort<T>(string portName, Direction direction) =>
			CreatePort(typeof(T), portName, direction);

		public EditorPort CreateExecutiveInputPort(string name = null) =>
			CreatePort<Exec>(name ?? EXEC_LABEL_IN, Direction.Input);
		public EditorPort CreateExecutiveOutputPort(string name = null) =>
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

		private void ChangeStyleToExecStyle(EditorPort port)
		{
			port.portColor = Color.white;

			if (port.portName == EXEC_LABEL_IN || port.portName == EXEC_LABEL_OUT)
			{
				var __label = port.Q<Label>();
				__label.visible = false;
			}
		}

		#endregion
		#region Destruction

		public void RemovePort(EditorPort port)
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
				__result.AddRange(iPort.connections);
			}

			return __result;
		}

		#endregion

		#endregion

		#endregion
	}

	public struct Exec { }
}
