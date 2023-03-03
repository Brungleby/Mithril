
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
using Cuberoot.Editor;

#endregion

namespace Cuberoot
{
	#region Data

	/// <summary>
	/// Stores all Nodes and Links for a single NodeGraph into a file.
	///</summary>
	[Serializable]

	public class GraphData : ScriptableObject, IEditable
	{
		public List<NodeData> Nodes = new List<NodeData>();
		public List<LinkData> Links = new List<LinkData>();

		public ReplicableEditorWindow[] UsableEditorTypes => throw new NotImplementedException();
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
		public string Title;
		public Rect Rect;
	}

	#endregion
	#region LinkData

	/// <summary>
	/// Stores data for a single link between two Nodes.
	///</summary>
	[Serializable]

	public sealed class LinkData : object
	{
		public GUID OutGuid;
		public string OutPortName;
		public GUID InGuid;
		public string InPortName;
	}

	#endregion
}
