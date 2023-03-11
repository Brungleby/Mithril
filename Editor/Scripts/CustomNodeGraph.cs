
/** CustomNodeGraph.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.UIElements;

#endregion

namespace Cuberoot.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public abstract class CustomNodeGraph<TGraphView, TSaveData> :
	InstantiableEditorWindow<TSaveData>

	where TGraphView : CustomNodeGraphView
	where TSaveData : ScriptableObject
	{
		#region Data

		private TGraphView _graph;
		public TGraphView graph => _graph;

		#endregion
		#region Properties

		#endregion
		#region Methods

		protected override void CreateVisualElements()
		{
			_graph = System.Activator.CreateInstance<TGraphView>();
			_graph.name = "New Custom Node Graph View";
			InitializeGraphView(_graph);
			rootVisualElement.Add(_graph);

			base.CreateVisualElements();
		}

		protected virtual void InitializeGraphView(TGraphView graph)
		{
			graph.name = "Basic Node Graph View";
			graph.OnModified.AddListener(() =>
			{
				isModified = true;
			});
		}

		protected override void CleanUpVisualElements()
		{
			if (_graph != null)
				rootVisualElement.Remove(_graph);
		}

		protected override void SaveData(out TSaveData data)
		{
			data = null;
		}

		protected override void LoadData(in TSaveData data)
		{

		}

		#region

		#endregion

		#endregion
	}
	public class CustomNodeGraph : CustomNodeGraph<CustomNodeGraphView, ScriptableObject> { }
}
