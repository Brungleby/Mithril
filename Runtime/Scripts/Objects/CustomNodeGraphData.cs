
/** BasicNodeData.cs
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

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using Cuberoot.Editor;

#endregion

namespace Cuberoot
{
	#region Data

	/// <summary>
	/// Stores all Nodes and Links for a single NodeGraph into a file.
	///</summary>
	[Serializable]

	public class CustomNodeGraphData : EditableObject
	{
		[SerializeField]
		public List<NodeData> PredefinedNodes = new List<NodeData>();
		[SerializeField]
		public List<NodeData> Nodes = new List<NodeData>();
		[SerializeField]
		public List<EdgeData> Edges = new List<EdgeData>();

		public override Type[] UsableEditorTypes =>
			new Type[] { typeof(CustomNodeGraph) };
	}

	#endregion
	#region NodeData

	/// <summary>
	/// Stores data for a single Node within a NodeGraph.
	///</summary>
	[Serializable]

	public sealed class NodeData : object
	{

		[SerializeField]
		public GUID Guid;

		[SerializeField]
		public Type Subtype;

		[SerializeField]
		public string SubtypeName;

		[SerializeField]
		public string Title;

		[SerializeField]
		public Rect Rect;

		[SerializeField]
		public PortData[] Ports;

		public NodeData(CustomNode node)
		{
			Guid = node.Guid;
			Subtype = node.GetType();
			SubtypeName = node.GetType().AssemblyQualifiedName;
			Title = node.title;
			Rect = node.GetPosition();
		}

		public Type SubtypeNameAsType =>
			Type.GetType(SubtypeName);

		public static implicit operator NodeData(CustomNode node) =>
			new NodeData(node);
	}

	#endregion
	#region LinkData

	/// <summary>
	/// Stores data for a single link between two Nodes.
	///</summary>
	[Serializable]

	public sealed class EdgeData : object
	{
		// public GUID nNodeGuid;
		// public string nPortName;
		// public GUID oNodeGuid;
		// public string oPortName;


		[SerializeField]
		public PortData nPort;

		[SerializeField]
		public PortData oPort;

		// public EdgeData(Edge edge, CustomNode nNode, CustomNode oNode)
		// {
		// 	nNodeGuid = nNode.Guid;
		// 	nPortName = edge.input.portName;

		// 	oNodeGuid = oNode.Guid;
		// 	oPortName = edge.output.portName;
		// }

		public EdgeData(Edge edge, CustomNode nNode, CustomNode oNode)
		{
			nPort = new PortData
			{
				NodeGuid = nNode.Guid,
				PortName = edge.input.portName,
				Direction = edge.input.direction,
				Orientation = edge.input.orientation,
				Capacity = edge.input.capacity,
				Type = edge.input.portType,
			};
			oPort = new PortData
			{
				NodeGuid = oNode.Guid,
				PortName = edge.output.portName,
				Direction = edge.output.direction,
				Orientation = edge.output.orientation,
				Capacity = edge.output.capacity,
				Type = edge.output.portType,
			};
		}
	}

	[Serializable]

	public sealed class PortData : object
	{

		[SerializeField]
		public GUID NodeGuid;

		[SerializeField]
		public string PortName;

		[SerializeField]
		public Direction Direction;

		[SerializeField]
		public Orientation Orientation;

		[SerializeField]
		public Port.Capacity Capacity;

		[SerializeField]
		public Type Type;
	}

	#endregion
}
