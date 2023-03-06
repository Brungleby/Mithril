
/** BasicNodeGraphSaveUtility.cs
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
	/// Assists with saving and loading of Node Graphs.
	///</summary>

	public sealed class BasicNodeGraphSaveUtility : object
	{
		#region Data

		#region

		private BasicNodeGraphView _graph;
		private GraphData _dataCache;

		private List<Edge> Edges => _graph.edges.ToList();
		private List<BasicNode> Nodes => _graph.nodes.ToList().Cast<BasicNode>().ToList();

		#endregion

		#endregion
		#region Methods

		#region

		public static BasicNodeGraphSaveUtility GetInstance(BasicNodeGraphView target)
		{
			return new BasicNodeGraphSaveUtility
			{
				_graph = target,
			};
		}

		public void SaveTargetToFile(string filePath)
		{
			/**	Asserts no data if there are NOT any NON-predefined nodes.
			*/
			if (!Nodes.Where(i => !i.IsPredefined).Any())
			{
				try { Utils.PromptConfirmation($"\"{filePath}\"\n\nThis graph is empty. Proceed to save the file anyway?"); }
				catch { return; }
			}

			var __data = ScriptableObject.CreateInstance<GraphData>();

			foreach (var iNode in Nodes)
			{
				__data.Nodes.Add(new NodeData
				{
					Guid = iNode.Guid,
					Subtype = iNode.GetType(),
					Title = iNode.title,
					Rect = iNode.GetPosition(),
				});
			}

			Utils.CreateAssetAtFilePath(__data, filePath);
		}

		public void LoadFileToTarget(string filePath)
		{
			_dataCache = AssetDatabase.LoadAssetAtPath<GraphData>(filePath);

			try { Utils.AssertObject(_dataCache, $"No data was found at local path \"{filePath}\"."); }
			catch { return; }

			_graph.ClearAllNodes_WithPrompt();

			#region Create Nodes

			foreach (var iNodeData in _dataCache.Nodes)
			{
				var __node = _graph.CreateNewNode<BasicNode>(iNodeData.Guid, iNodeData.Title, iNodeData.Rect);

				// var __ports = 
			}

			#endregion

			_dataCache = null;
		}

		#endregion

		#endregion
	}
}
