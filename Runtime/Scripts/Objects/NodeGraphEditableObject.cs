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

#endregion

namespace Cuberoot
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>
	[Serializable]

	public abstract class NodeGraphEditableObject : EditableObject
	{
		#region NodeData

		/// <summary>
		/// Stores data for a single Node within a NodeGraph.
		///</summary>
		[Serializable]

		public struct NodeData
		{
			public Guid Guid;
			public string SubtypeName;
			public string Title;
			public bool IsPredefined;

#if UNITY_EDITOR
			public Rect Rect;
#endif

			public PortData[] Ports;
#if UNITY_EDITOR
			public NodeData(CustomNode node)
			{
				Guid = node.Guid;

				SubtypeName = node.GetType().AssemblyQualifiedName;
				Title = node.title;
				IsPredefined = node.IsPredefined;

				Rect = node.GetPosition();

				Ports = GetPortsFrom(node);
			}

			public static implicit operator NodeData(CustomNode _) =>
				new NodeData(_);

			public static PortData[] GetPortsFrom(CustomNode node)
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
				Guid.ToString();
		}

		#endregion
		#region PortData

		[Serializable]

		public struct PortData
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
				NodeGuid = ((CustomNode)port.node).Guid;
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

		public struct EdgeData
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
		#region Bake Data

		public NodeData[] nodes = new NodeData[0];
		public EdgeData[] edges = new EdgeData[0];

		#endregion
		#region Methods

		public override object Clone()
		{
			var that = (NodeGraphEditableObject)ScriptableObject.CreateInstance(GetType());

			that.nodes = this.nodes;
			that.edges = this.edges;

			return that;
		}

#if UNITY_EDITOR
		public void CompileNodes(List<CustomNode> nodes)
		{
			this.nodes = new NodeData[nodes.Count];

			for (int i = 0; i < this.nodes.Length; i++)
			{
				var iNode = nodes[i];
				this.nodes[i] = iNode;
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
}
