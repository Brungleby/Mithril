
/** RootEditorWindow.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Linq;
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

		public static InstantiableEditorWindow Instantiate(System.Type type, string filePath, string iconPath)
		{
			var __allWindowsOfMatchingType = Resources.FindObjectsOfTypeAll(type).Cast<InstantiableEditorWindow>();

			foreach (var iWindow in __allWindowsOfMatchingType)
			{
				if (iWindow._filePath == filePath)
				{
					iWindow.Focus();
					return iWindow;
				}
			}

			/**	else if no windows working with the filePath are found
			*/

			var __window = (InstantiableEditorWindow)EditorWindow.CreateInstance(type);
			__window.Initialize(filePath, iconPath);

			return __window;
		}
		public static InstantiableEditorWindow Instantiate(System.Type type, string filePath) =>
			Instantiate(type, filePath, DEFAULT_ICON_PATH);

		public static T Instantiate<T>(string filePath, string iconPath)
		where T : InstantiableEditorWindow =>
			(T)Instantiate(typeof(T), filePath, iconPath);
		public static T Instantiate<T>(string filePath)
		where T : InstantiableEditorWindow =>
			Instantiate<T>(filePath, DEFAULT_ICON_PATH);

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

		protected virtual void OnEnable() =>
			CreateVisualElements();

		protected virtual void OnDisable() =>
			CleanUpVisualElements();

		protected virtual void CreateVisualElements()
		{
			Toolbar __toolbar = new Toolbar();
			InitializeToolbar(__toolbar);
			rootVisualElement.Add(__toolbar);
		}

		protected virtual void InitializeToolbar(Toolbar toolbar)
		{
			toolbar.Add(new Button(() => SaveData()) { text = "Save Asset" });
		}

		protected virtual void CleanUpVisualElements() { }

		#endregion
		#region Save/Load

		protected virtual void SaveData()
		{
			isModified = false;
		}

		protected virtual void LoadData()
		{
			isModified = false;
		}

		#endregion
		#endregion
	}

	#endregion
}
