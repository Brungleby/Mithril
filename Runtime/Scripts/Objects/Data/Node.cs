
/** Node.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes



using EditorNode = Mithril.Editor.Node;

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

#if UNITY_EDITOR
		public Node(EditorNode rep_node)
		{
			_guid = rep_node.guid;
			_title = rep_node.title;
			_isPredefined = rep_node.isPredefined;

			_rect = rep_node.rect;
		}
#endif
		#endregion

		#endregion
	}

	#endregion
	#region Port

	public class Port : GraphObject
	{
		private Guid _nodeGuid;
		private string _portName;
	}

	#endregion
	#region Edge

	public class Edge : GraphObject
	{

	}

	#endregion
}
