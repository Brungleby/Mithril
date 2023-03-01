
/** NodeGraphSaveUtility.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

#endregion

namespace Cuberoot.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class NodeGraphSaveUtility : object
	{
		#region Data

		#region

		private NodeGraphView _target;
		private DialogueContainer _containerCache;

		private List<Edge> Edges => _target.edges.ToList();
		private List<Node> Nodes => _target.nodes.ToList().Cast<Node>().ToList();

		#endregion

		#endregion
		#region Methods

		#region

		public static NodeGraphSaveUtility GetInstance(NodeGraphView target)
		{
			return new NodeGraphSaveUtility
			{
				_target = target
			};
		}

		public void SaveGraph(string fileName)
		{
			if (Edges.Count == 0)
				return;

			var __dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

			foreach (var iPort in Edges.Where(x => x.input.node != null).ToArray())
			{
				var __outNode = (Node)iPort.output.node;
				var __inNode = (Node)iPort.input.node;

				__dialogueContainer.Linkage.Add(new NodeLinkData
				{
					NodeGuid = __outNode.GUID,
					PortName = iPort.output.portName,
					TargetGuid = __inNode.GUID,
				});
			}

			foreach (var iNode in Nodes.Where(node => !node.EntryPoint))
			{
				__dialogueContainer.Data.Add(new DialogueNodeData
				{
					Guid = iNode.GUID,
					DialogueText = iNode.DialogueText,
					Position = iNode.GetPosition().position,
				});
			}

			if (!AssetDatabase.IsValidFolder("Assets/Resources"))
				AssetDatabase.CreateFolder("Assets", "Resources");

			AssetDatabase.CreateAsset(__dialogueContainer, $"Assets/Resources/{fileName}.asset");
			AssetDatabase.SaveAssets();
		}

		public void LoadGraph(string fileName)
		{
			_containerCache = AssetDatabase.LoadAssetAtPath<DialogueContainer>(fileName);

			if (_containerCache == null)
			{
				EditorUtility.DisplayDialog("File Not Found", "Target load file name does not exist.", "OK");
				return;
			}

			ClearGraph();
			CreateCachedNodes();
			ConnectCachedNodes();

			_containerCache = null;
		}

		private void ClearGraph()
		{
			Nodes.Find(x => x.EntryPoint).GUID = _containerCache.Linkage[0].NodeGuid;

			foreach (var iNode in Nodes)
			{
				if (iNode.EntryPoint)
					continue;

				Edges.Where(x => x.input.node == iNode).ToList()
					.ForEach(edge => { _target.RemoveElement(edge); });

				_target.RemoveElement(iNode);
			}
		}

		private void CreateCachedNodes()
		{
			foreach (var iData in _containerCache.Data)
			{
				var iNode = _target.CreateNewNode(iData.DialogueText);
				iNode.GUID = iData.Guid;
				_target.AddElement(iNode);

				var iPorts = _containerCache.Linkage.Where(x => x.NodeGuid == iData.Guid).ToList();
				iPorts.ForEach(x => _target.CreateChoicePort(iNode, x.PortName));
			}
		}

		private void ConnectCachedNodes()
		{
			for (var i = 0; i < Nodes.Count; i++)
			{
				var iNode = Nodes[i];

				var __links = _containerCache.Linkage.Where(x => x.NodeGuid == iNode.GUID).ToList();

				for (var j = 0; j < __links.Count; j++)
				{
					var jLink = __links[j];

					var __targetNodeGuid = jLink.TargetGuid;
					var __targetNode = Nodes.First(x => x.GUID == __targetNodeGuid);

					__targetNode.SetPosition(new Rect(
						_containerCache.Data.First(x => x.Guid == __targetNodeGuid).Position,
						_target.DEFAULT_NODE_SIZE
					));

					LinkPorts(iNode.outputContainer.Q<Port>(), (Port)__targetNode.inputContainer[0]);
				}
			}
		}

		private void LinkPorts(Port output, Port input)
		{
			var __edge = new Edge
			{
				output = output,
				input = input,
			};

			__edge?.input.Connect(__edge);
			__edge?.output.Connect(__edge);
			_target.Add(__edge);
		}

		#endregion

		#endregion
	}

}

