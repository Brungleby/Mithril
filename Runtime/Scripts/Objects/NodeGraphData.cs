/** NodeData_Compiled.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor.Experimental.GraphView;

using Cuberoot.Editor;
using Node = Cuberoot.Editor.Node;

#endregion

namespace Cuberoot
{
	#region NodeGraphData

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>
	[Serializable]

	public abstract class NodeGraphData : EditableObject
	{
		#region Bake Data
#if UNITY_EDITOR
		public Vector2 viewPosition;
#endif
		public NodeData[] nodes = new NodeData[0];
		public EdgeData[] edges = new EdgeData[0];

		#endregion
		#region Methods

		public override object Clone()
		{
			var that = (NodeGraphData)ScriptableObject.CreateInstance(GetType());

			that.nodes = this.nodes;
			that.edges = this.edges;

			return that;
		}

#if UNITY_EDITOR
		public void CompileNodes(List<Editor.Node> nodes)
		{
			this.nodes = new NodeData[nodes.Count];

			for (int i = 0; i < this.nodes.Length; i++)
			{
				var iNode = nodes[i];
				this.nodes[i] = NodeData.CreateFrom(iNode);
			}
		}

		public void CompileEdges(List<Edge> edges)
		{
			this.edges = new EdgeData[edges.Count];

			for (int i = 0; i < this.edges.Length; i++)
			{
				var iEdge = edges[i];
				this.edges[i] = iEdge;
			}
		}
#endif

		#endregion
	}

	#endregion

	#region NodeData

	/// <summary>
	/// Stores data for a single Node within a NodeGraph.
	///</summary>
	[Serializable]

	public class NodeData : object, ISerializable
	{
		public Guid guid;
		public string title;
		public bool isPredefined;

#if UNITY_EDITOR
		public Rect rect;
#endif
		public PortData[] ports;

		[SerializeField]
		private string _selfType;
		public Type selfType
		{
			get => Type.GetType(_selfType);
			set => _selfType = value.AssemblyQualifiedName;
		}

		[SerializeField]
		private string _nodeType;
		public Type nodeType
		{
			get => Type.GetType(_nodeType);
			set => _nodeType = value.AssemblyQualifiedName;
		}

#if UNITY_EDITOR
		public virtual void Init(Node node)
		{
			guid = node.guid;
			title = node.title;
			isPredefined = node.IsPredefined;

			rect = node.GetPosition();

			ports = GetPortsFrom(node);

			selfType = GetType();
			nodeType = node.GetType();
		}

		private static NodeData CreateFrom(Type type, Node node)
		{
			var __result = (NodeData)Activator.CreateInstance(type);
			__result.Init(node);
			return __result;
		}
		public static NodeData CreateFrom(Node node) =>
			CreateFrom(node.DataType, node);
		public static T CreateFrom<T>(Node node)
		where T : NodeData, new() =>
			(T)CreateFrom(typeof(T), node);

		public static PortData[] GetPortsFrom(Node node)
		{
			var __nPorts = node.GetInputPorts();
			var __oPorts = node.GetOutputPorts();

			var __ports = new PortData[__nPorts.Count + __oPorts.Count];

			for (int i = 0; i < __nPorts.Count; i++)
				__ports[i] = __nPorts[i];
			for (int i = 0; i < __oPorts.Count; i++)
				__ports[i + __nPorts.Count] = __oPorts[i];

			return __ports;
		}
#endif
		public override string ToString() =>
			guid.ToString();
	}

	#endregion
	#region PortData

	[Serializable]

	public struct PortData : ISerializable
	{
		public Guid NodeGuid;
		public string PortName;
		public Direction Direction;
		public Orientation Orientation;
		public Port.Capacity Capacity;
		public string Type;

#if UNITY_EDITOR
		// public PortData(GUID guid, string portName, Direction direction, Orientation orientation, Port.Capacity capacity, Type type)
		// {
		// 	NodeGuid = guid;
		// 	PortName = portName;
		// 	Direction = direction;
		// 	Orientation = orientation;
		// 	Capacity = capacity;
		// 	Type = type;

		public PortData(Port port)
		{
			NodeGuid = ((Node)port.node).guid;
			PortName = port.portName;
			Direction = port.direction;
			Orientation = port.orientation;
			Capacity = port.capacity;
			Type = port.portType.AssemblyQualifiedName;
		}

		public static implicit operator PortData(Port _) =>
			new PortData(_);
#endif
	}

	#endregion
	#region Edge Data

	/// <summary>
	/// Stores data for a single link between two Nodes.
	///</summary>
	[Serializable]

	public struct EdgeData : ISerializable
	{
		public PortData nPort;
		public PortData oPort;

#if UNITY_EDITOR
		public EdgeData(Edge edge)
		{
			nPort = new PortData(edge.input);
			oPort = new PortData(edge.output);
		}

		public static implicit operator EdgeData(Edge _) =>
			new EdgeData(_);
#endif
	}

	#endregion
}
