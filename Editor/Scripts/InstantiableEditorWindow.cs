
/** RootEditorWindow.cs
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
	#region InstantiableEditorWindow

	public abstract class InstantiableEditorWindow :
	EditorWindow
	{
		public readonly static string DEFAULT_ICON_PATH = "Assets/Cuberoot/Cuberoot.Core/Editor/Resources/Textures/Icon_Diamond.png";

		private BasicNodeGraphView _graph;
		public BasicNodeGraphView graph => _graph;

		private string _filePath;
		public string filePath => _filePath;

		private string _rawTitle;
		public string rawTitle => _rawTitle;

		private bool _isModified;
		public bool isModified
		{
			get => _isModified;
			set
			{
				_isModified = value;

				titleContent.text = rawTitle + (value ? "*" : "");
			}
		}

		#region Methods

		#region Instantiation

		public void Initialize(string filePath, string iconPath)
		{
			_rawTitle = GetTitleFromFilePath(filePath);
			Utils.InitializeWindow(this, _rawTitle, iconPath);

			_filePath = filePath;
			LoadData();
		}


		public static T Instantiate<T>(string filePath, string iconPath)
		where T : InstantiableEditorWindow
		{
			var __window = EditorWindow.GetWindow<T>();
			if (__window._filePath == filePath)
				__window = EditorWindow.CreateInstance<T>();

			__window.Initialize(filePath, iconPath);

			return __window;
		}
		public static T Instantiate<T>(string filePath)
		where T : InstantiableEditorWindow =>
			Instantiate<T>(filePath, DEFAULT_ICON_PATH);

		public static InstantiableEditorWindow Instantiate(System.Type type, string filePath, string iconPath)
		{
			var __window = (InstantiableEditorWindow)EditorWindow.GetWindow(type);
			if (__window._filePath == filePath)
				__window = (InstantiableEditorWindow)EditorWindow.CreateInstance(type);

			__window.Initialize(filePath, iconPath);

			return __window;
		}
		public static InstantiableEditorWindow Instantiate(System.Type type, string filePath) =>
			Instantiate(type, filePath, DEFAULT_ICON_PATH);

		private static string GetTitleFromFilePath(string filePath)
		{
			var __lastIndexOfSlash = filePath.LastIndexOf('/') + 1;
			var __lastIndexOfPeriod = filePath.LastIndexOf('.');
			var __result = filePath.Substring(
				__lastIndexOfSlash,
				__lastIndexOfPeriod - __lastIndexOfSlash
			);

			return __result;
		}

		#endregion
		#region 

		protected virtual void OnEnable()
		{
			CreateVisualElements();
		}

		protected virtual void OnDisable()
		{
			if (_graph != null)
				rootVisualElement.Remove(_graph);
		}

		protected virtual void CreateVisualElements()
		{
			_graph = new BasicNodeGraphView() { name = "Basic Node Graph View" };
			InitializeGraphView(_graph);
			rootVisualElement.Add(_graph);

			Toolbar __toolbar = new Toolbar();
			InitializeToolbar(__toolbar);
			rootVisualElement.Add(__toolbar);
		}

		protected virtual void InitializeGraphView(BasicNodeGraphView graph)
		{
			graph.name = "Basic Node Graph View";
			graph.OnModified.AddListener(() =>
			{
				isModified = true;
			});
		}

		protected virtual void InitializeToolbar(Toolbar toolbar)
		{
			toolbar.Add(new Button(() => SaveData()) { text = "Save Asset" });
		}

		#endregion
		#region Save/Load

		private void SaveData()
		{
			var __saveUtility = BasicNodeGraphSaveUtility.GetInstance(_graph);
			__saveUtility.SaveTargetToFile(_filePath);

			isModified = false;
		}

		private void LoadData()
		{
			var __saveUtility = BasicNodeGraphSaveUtility.GetInstance(_graph);
			__saveUtility.LoadFileToTarget(_filePath);
		}

		#endregion
		#endregion
	}

	#endregion
}
