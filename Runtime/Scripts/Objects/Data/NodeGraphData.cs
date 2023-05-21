/** NodeData_Compiled.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor.Experimental.GraphView;

using Mithril.Editor;
using EditorNode = Mithril.Editor.Node;

#endregion

namespace Mithril
{
	#region NodeGraphData

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>
	[Serializable]

	public abstract class NodeGraphData : EditableObject
	{
		#region Data

		[SerializeField]
		[HideInInspector]
		[NonMirrored]
		private Mirror[] _nodeMirrors = new Mirror[0];
		public Mirror[] nodeMirrors => _nodeMirrors;

		[SerializeField]
		[HideInInspector]
		[NonMirrored]
		private Mirror[] _edgeMirrors = new Mirror[0];
		public Mirror[] edgeMirrors => _edgeMirrors;

		[MirrorField]
		private NodeData.Node[] _nodeData = new NodeData.Node[0];
		public NodeData.Node[] nodeData => _nodeData;

		[MirrorField]
		private NodeData.Edge[] _edgeData = new NodeData.Edge[0];
		public NodeData.Edge[] edgeData => _edgeData;

		#endregion
		#region Editor-only Data
#if UNITY_EDITOR
		/**	__TODO_REFACTOR__
		*
		*	Because EditableObjects can be edited using multiple windows,
		*	we should either store the view position of each window type,
		*	or none at all.
		*/

		[SerializeField]
		[HideInInspector]
		public Vector2 viewPosition;

		private Editor.StickyNote[] _stickyNotes = new Editor.StickyNote[0];
#endif
		#endregion
		#region Methods

		#region Setup / Teardown
#if UNITY_EDITOR
		public void UpdateFromGraphView(NodeGraphView graphView)
		{
			/** <<============================================================>> **/

			/** <<============================================================>> **/

			viewPosition = graphView.viewTransform.position;

			/** <<============================================================>> **/
			/**	Nodes
			*/

			var __nodes = graphView.nodes.Cast<EditorNode>();

			var __nodeMirrorList = new List<Mirror>();
			foreach (var iNode in __nodes)
			{
				iNode.OnBeforeSerialize();
				__nodeMirrorList.Add(new Mirror(iNode));
			}

			_nodeMirrors = __nodeMirrorList.ToArray();

			/** <<============================================================>> **/
			/**	Edges
			*
			*	Note: Edges are not updated it graphView.edges until after they have been created.
			*	This means that adding or removing an edge from the graph will not add nor remove it
			*	from HERE until another change has been made (during autosaving only). However,
			*	the InstantiableWindow performs one final save before closing, making this issue rather benign.
			*/

			var __edges = graphView.edges;

			var __edgeMirrorList = new List<Mirror>();
			foreach (var iEdge in __edges)
			{
				__edgeMirrorList.Add(new Mirror(iEdge));
			}

			_edgeMirrors = __edgeMirrorList.ToArray();
		}

		public void UpdateModelFromEditor(NodeGraphView graphView)
		{
			viewPosition = graphView.viewTransform.position;

			var __editorNodes = graphView.GetAllNodes();
			var __modelNodes = new List<NodeData.Node>();
			foreach (var iEditorNode in __editorNodes)
			{
				var iModelNode = (NodeData.Node)Activator
				.CreateInstance(
					iEditorNode.modelType,
					new object[] { iEditorNode }
					);
				__modelNodes.Add(iModelNode);
			}
			_nodeData = __modelNodes.ToArray();

			var __editorEdges = graphView.GetAllEdges();
			var __modelEdges = new List<NodeData.Edge>();
			foreach (var iEditorEdge in __editorEdges)
				__modelEdges.Add(new NodeData.Edge(iEditorEdge));
			_edgeData = __modelEdges.ToArray();
		}
#endif
		#endregion
		#region Retrieval

		public NodeData.Node GetNodeByGuid(Guid guid) =>
			_nodeData.First(i => i.guid == guid);

		public NodeData.Node GetNodeByType(Type type) =>
			_nodeData.First(i => type.IsAssignableFrom(i.GetType()));
		public T GetNodeByType<T>()
		where T : NodeData.Node =>
			(T)GetNodeByType(typeof(T));

		#endregion

		#endregion
	}

	#endregion

	#region NodeData

	/// <summary>
	/// Stores data for a single EditorNode within a NodeGraph.
	///</summary>
	[Serializable]

	public class NodeDataObject : object, ISerializable
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

		// public NodeData(Mirror mirror)
		// {
		// 	// guid = serialObject.Ge
		// }

		public NodeDataObject()
		{

		}

#if UNITY_EDITOR
		public virtual void Init(EditorNode node)
		{
			guid = node.guid;
			title = node.title;
			isPredefined = node.isPredefined;

			rect = node.GetPosition();

			ports = GetPortsFrom(node);

			selfType = GetType();
			nodeType = node.GetType();
		}

		private static NodeDataObject CreateFrom(Type type, EditorNode node)
		{
			var __result = (NodeDataObject)Activator.CreateInstance(type);
			__result.Init(node);
			return __result;
		}
		public static T CreateFrom<T>(EditorNode node)
		where T : NodeDataObject, new() =>
			(T)CreateFrom(typeof(T), node);

		public static PortData[] GetPortsFrom(EditorNode node)
		{
			var __nPorts = node.GetPortsIn();
			var __oPorts = node.GetPortsOut();

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
		public UnityEditor.Experimental.GraphView.Port.Capacity Capacity;
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

		public PortData(UnityEditor.Experimental.GraphView.Port port)
		{
			NodeGuid = ((EditorNode)port.node).guid;
			PortName = port.portName;
			Direction = port.direction;
			Orientation = port.orientation;
			Capacity = port.capacity;
			Type = port.portType.AssemblyQualifiedName;
		}

		public static implicit operator PortData(UnityEditor.Experimental.GraphView.Port _) =>
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
