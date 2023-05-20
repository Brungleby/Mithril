
/** NodeNetwork.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Linq;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

#endregion

namespace Mithril
{
	/// <summary>
	/// An <see cref="EditableObject"/> that contains <see cref="Node"/>s and their connections.
	///</summary>

	public class NodeNetwork : EditableObject
	{
		#region Data

		#region

		[MirrorField]
		private Node[] _nodes;
		public Node[] nodes => _nodes;

		#endregion

		#endregion
		#region Methods

		#region Construction
#if UNITY_EDITOR
		public void CompileFromEditor(GraphView graph)
		{

		}
#endif
		#endregion
		#region Nodes

		public Node GetNodeByGuid(Guid guid)
		{
			try
			{
				return _nodes.First(i => i.guid.Equals(guid));
			}
			catch (Exception e)
			{
				throw new Exception($"Couldn't find node by guid ({guid}) in this NodeNetwork", e);
			}
		}

		public T GetNodeByGuid<T>(Guid guid)
		where T : Node =>
			(T)GetNodeByGuid(guid);

		public T GetNodeByType<T>()
		where T : Node
		{
			try
			{
				return (T)_nodes.First(i => i is T);
			}
			catch (Exception e)
			{
				throw new Exception($"Couldn't find node by type ({typeof(T).Name}) in this NodeNetwork", e);
			}
		}

		#endregion

		#endregion
	}
}
