
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

	public class BasicNodeGraph : InstantiableEditorWindow
	{
		#region Data

		private BasicNodeGraphView _graphView;
		private string _fileName;

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

		#endregion

		#endregion
	}
}
