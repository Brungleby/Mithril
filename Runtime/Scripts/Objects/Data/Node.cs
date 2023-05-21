
/** Node.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;

using EditorEdge = UnityEditor.Experimental.GraphView.Edge;
using EditorNode = Mithril.Editor.Node;
using EditorPort = UnityEditor.Experimental.GraphView.Port;

using UnityEditor.Experimental.GraphView;

using UnityEngine;

#endregion

namespace Mithril.NodeData
{
	#region GraphObject

	public abstract class GraphObject : object
	{

	}

	#endregion
	#region Node

	/// <summary>
	/// A condensed version of a GraphView.Node that can be accessed at runtime.
	///</summary>

	public class Node : GraphObject
	{
		#region Data

		[SerializeField]
		private Guid _guid;
		public Guid guid => _guid;

		[SerializeField]
		private string _title;
		public string title => _title;

		[SerializeField]
		private bool _isPredefined;
		public bool isPredefined => _isPredefined;

		[SerializeField]
		private Port[] _portsIn;
		public Port[] portsIn => _portsIn;

		[SerializeField]
		private Port[] _portsOut;
		public Port[] portsOut => _portsOut;


		#endregion
		#region Editor-only Data
#if UNITY_EDITOR
		[SerializeField]
		private Rect _rect;
		public Rect rect => _rect;
#endif
		#endregion

		#region Methods

		#region Construction

		public Node() { }
#if UNITY_EDITOR
		public Node(EditorNode editorNode)
		{
			_guid = editorNode.guid;
			_title = editorNode.title;
			_isPredefined = editorNode.isPredefined;

			{
				var __editorPortsIn = editorNode.GetPortsIn();
				_portsIn = new Port[__editorPortsIn.Count];

				var i = 0;
				foreach (EditorPort iPort in __editorPortsIn)
				{
					portsIn[i] = new Port(iPort);
					i++;
				}
			}
			{
				var __editorPortsOut = editorNode.GetPortsOut();
				_portsOut = new Port[__editorPortsOut.Count];

				var i = 0;
				foreach (EditorPort iPort in __editorPortsOut)
				{
					portsOut[i] = new Port(iPort);
					i++;
				}
			}

			_rect = editorNode.rect;
		}
#endif
		#endregion

		public virtual Type editorType => typeof(EditorNode);

		#endregion
	}

	#endregion
	#region Port

	public class Port : GraphObject
	{
		#region Data

		[SerializeField]
		private string _portName;
		public string portName => _portName;

		[SerializeField]
		private string _portType;
		public Type portType
		{
			get => Type.GetType(_portType);
			private set => _portType = value.AssemblyQualifiedName;
		}

		#endregion
		#region Methods

		#region Construction

		public Port() { }
#if UNITY_EDITOR
		public Port(EditorPort port)
		{
			portType = port.portType;
			_portName = port.portName;
		}
#endif
		#endregion

		#endregion
	}

	#endregion
	#region Edge

	public class Edge : GraphObject
	{
		#region Data

		[SerializeField]
		private Guid _guidIn;
		public Guid guidIn => _guidIn;

		[SerializeField]
		private Guid _guidOut;
		public Guid guidOut => _guidOut;

		[SerializeField]
		private string _portIn;
		public string portIn => _portIn;

		[SerializeField]
		private string _portOut;
		public string portOut => _portOut;

		#endregion
		#region Methods

		#region Construction

		public Edge() { }
#if UNITY_EDITOR
		public Edge(EditorEdge editorEdge)
		{
			_guidIn = ((EditorNode)editorEdge.input.node).guid;
			_guidOut = ((EditorNode)editorEdge.output.node).guid;

			_portIn = editorEdge.input.portName;
			_portOut = editorEdge.output.portName;
		}
#endif
		#endregion

		#endregion
	}

	#endregion
}
