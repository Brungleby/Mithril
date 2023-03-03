
/** BasicNodeGraph.cs
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

	public class BasicNodeGraph : ReplicableEditorWindow
	{
		#region Data

		#region

		private BasicNodeGraphView _graphView;
		private string _fileName;
		private string _directoryPath;
		private string _filePath;

		#endregion

		#endregion
		#region Properties

		#endregion
		#region Methods

		#region

		[MenuItem("Cuberoot/Basic Node Graph Window")]
		public static void ShowWindow()
		{
			var window = Utils.GetShowWindow<BasicNodeGraph>("Window");
		}

		private void OnEnable()
		{
			/**	Create visual elements
			*/
			CreateElements();
		}

		private void OnDisable()
		{
			/**	Remove the graph view to clean up
			*/
			rootVisualElement.Remove(_graphView);
		}

		private void CreateElements()
		{
			#region Graph View
			{
				/**	Initialize the graph view
				*/
				_graphView = new BasicNodeGraphView
				{
					name = "Basic Node Graph"
				};
				_graphView.StretchToParentSize();

				/**	Add it to the window
				*/
				rootVisualElement.Add(_graphView);
			}
			#endregion
			#region Toolbar
			{
				/**	Initialize the toolbar
				*/
				var __toolbar = new Toolbar();

				/**	File name text field
				*/
				{
					var __fileNameField = new TextField("File name:");
					__fileNameField.SetValueWithoutNotify("New Asset");
					__fileNameField.MarkDirtyRepaint();
					__fileNameField.RegisterValueChangedCallback(context =>
					{
						_fileName = context.newValue;
					});
					__toolbar.Add(__fileNameField);
				}
				/**	Buttons
				*/

				__toolbar.Add(new Button(() =>
					SaveData())
				{
					text = "Save"
				});
				__toolbar.Add(new Button(() =>
					LoadData())
				{
					text = "Load"
				});

				__toolbar.Add(new Button(() =>
					_graphView.CreateNewNode<BasicNode>("New Node"))
				{
					text = "Create New"
				});

				__toolbar.Add(new Button(() =>
					_graphView.ClearAllNodes_WithPrompt())
				{
					text = "Clear All"
				});

				/**	Add the toolbar to the window
				*/
				rootVisualElement.Add(__toolbar);
			}
			#endregion
		}

		private void SaveData()
		{
			try
			{
				Utils.AssertFileName(_fileName);

				var __saveUtility = BasicNodeGraphSaveUtility.GetInstance(_graphView);
				__saveUtility.SaveTargetToFile(_filePath);
			}
			catch { }
		}

		private void LoadData()
		{
			try
			{
				Utils.AssertFileName(_fileName);

				var __saveUtility = BasicNodeGraphSaveUtility.GetInstance(_graphView);
				__saveUtility.LoadFileToTarget(_filePath);
			}
			catch { }
		}

		#endregion

		#endregion
	}
}
