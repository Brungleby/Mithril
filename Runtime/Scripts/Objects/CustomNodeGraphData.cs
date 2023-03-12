
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
		public List<Vector2> PredefinedNodePositions = new List<Vector2>();
		public List<NodeData> Nodes = new List<NodeData>();
		public List<LinkData> Links = new List<LinkData>();

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
	}

	#endregion
	#region LinkData

	/// <summary>
	/// Stores data for a single link between two Nodes.
	///</summary>
	[Serializable]

	public sealed class LinkData : object
	{
		public PortData NPort;
		public PortData OPort;

		public LinkData(Edge edge, CustomNode nNode, CustomNode oNode)
		{
			NPort = new PortData
			{
				NodeGuid = nNode.Guid,
				Name = edge.input.portName,
				Direction = edge.input.direction,
				Orientation = edge.input.orientation,
				Capacity = edge.input.capacity,
				Type = edge.input.portType,
			};
			OPort = new PortData
			{
				NodeGuid = oNode.Guid,
				Name = edge.output.portName,
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
		public string Name;
		public Direction Direction;
		public Orientation Orientation;
		public Port.Capacity Capacity;
		public Type Type;
	}

	#endregion
}
