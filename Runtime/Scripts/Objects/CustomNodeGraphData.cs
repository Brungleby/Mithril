
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
		public List<NodeData> PredefinedNodes = new List<NodeData>();
		public List<NodeData> Nodes = new List<NodeData>();
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
		public GUID Guid;
		public Type Subtype;
		public string SubtypeName;
		public string Title;
		public Rect Rect;

		public PortData[] Ports;

		public NodeData(CustomNode node)
		{
			Guid = node.Guid;
			Subtype = node.GetType();
			SubtypeName = node.GetType().ToString();
			Title = node.title;
			Rect = node.GetPosition();
		}
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

		public PortData nPort;
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
		public GUID NodeGuid;
		public string PortName;
		public Direction Direction;
		public Orientation Orientation;
		public Port.Capacity Capacity;
		public Type Type;
	}

	#endregion
}
