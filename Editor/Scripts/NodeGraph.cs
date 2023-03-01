
/** NodeGraph.cs
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

	public class NodeGraph : EditorWindow
	{
		#region Data

		#region

		private NodeGraphView _graphView;
		private string _fileName;

		#endregion

		#endregion
		#region Methods

		#region

		[MenuItem("Cuberoot/Node Graph")]

		public static void OpenNodeGraphWindow()
		{
			var window = GetWindow<NodeGraph>();
			window.titleContent = new GUIContent("Node Graph");
		}

		private void OnEnable()
		{
			CreateGraphView();
			CreateToolbar();
		}

		private void OnDisable()
		{
			rootVisualElement.Remove(_graphView);
		}

		private void CreateGraphView()
		{
			_graphView = new NodeGraphView
			{
				name = "Node Graph"
			};

			_graphView.StretchToParentSize();
			rootVisualElement.Add(_graphView);
		}

		private void CreateToolbar()
		{
			var __toolbar = new Toolbar();

			var __fileNameTextField = new TextField("File Name:");
			__fileNameTextField.SetValueWithoutNotify("New Narrative");
			__fileNameTextField.MarkDirtyRepaint();
			__fileNameTextField.RegisterValueChangedCallback(context =>
			{
				_fileName = context.newValue;
			});
			__toolbar.Add(__fileNameTextField);

			__toolbar.Add(new Button(() => SaveData()) { text = "Save" });
			__toolbar.Add(new Button(() => LoadData()) { text = "Load" });

			var __createNodeButton = new Button(() =>
			{
				_graphView.CreateNewNode("Dialogue Node");
			});

			__createNodeButton.text = "Create New";

			__toolbar.Add(__createNodeButton);

			rootVisualElement.Add(__toolbar);
		}

		private void SaveData() => RequestDataOperation(true);
		private void LoadData() => RequestDataOperation(false);

		private void RequestDataOperation(bool save)
		{
			if (string.IsNullOrEmpty(_fileName))
			{
				EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "OK");
			}

			var __saveUtility = NodeGraphSaveUtility.GetInstance(_graphView);
			if (save)
				__saveUtility.SaveGraph(_fileName);
			else
				__saveUtility.LoadGraph(_fileName);
		}

		#endregion

		#endregion
	}
}
