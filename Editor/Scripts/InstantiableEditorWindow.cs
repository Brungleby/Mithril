
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

	public abstract class InstantiableEditorWindow : EditorWindow
	{
		#region Data

		#region Constants

		public readonly static string DEFAULT_ICON_PATH =
			"Assets/Cuberoot/Cuberoot.Core/Editor/Resources/Textures/Icon_Diamond.png";

		#endregion
		#region Members

		private EditableObject _editObject;
		public EditableObject editObject => _editObject;

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

		#endregion

		#endregion
		#region Methods

		public string filePath =>
			_editObject.filePath;

		public virtual void InitializeHeader(string filePath, string iconPath)
		{
			_rawTitle = GetTitleFromFilePath(filePath);
			Utils.InitializeWindowHeader(this, _rawTitle, iconPath);
		}

		#region Static

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
		#region Instantiation

		public static InstantiableEditorWindow Instantiate(System.Type type, EditableObject data)
		{
			/** <<============================================================>> **/

			var __filePath = AssetDatabase.GetAssetPath(data);
			var __iconPath = DEFAULT_ICON_PATH;

			var __allWindowsOfMatchingType = Resources.FindObjectsOfTypeAll(type)
				.Cast<InstantiableEditorWindow>()
			;

			/** <<============================================================>> **/

			foreach (var iWindow in __allWindowsOfMatchingType)
			{
				if (iWindow.filePath == __filePath)
				{
					iWindow.Focus();
					return iWindow;
				}
			}

			/** <<============================================================>> **/

			var __window = (InstantiableEditorWindow)EditorWindow.CreateInstance(type);
			__window.Open(data);

			__window.InitializeHeader(__filePath, __iconPath);

			/** <<============================================================>> **/

			return __window;
		}

		public static T Instantiate<T>(EditableObject obj)
		where T : InstantiableEditorWindow =>
			(T)Instantiate(typeof(T), obj);

		#endregion
		#region Setup

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
			toolbar.Add(new Button(() => Save()) { text = "Save Asset" });
		}

		protected virtual void CleanUpVisualElements() { }

		#endregion
		#region Save/Load

		/// <summary>
		/// Saves the current <see cref="filePath"/> as an asset.
		///</summary>

		public virtual void Save()
		{
			PushChangesToObject();
			_editObject.Save();

			isModified = false;
		}

		/// <summary>
		/// Updates the given <paramref name="data"/> using the "hot" data inside this window.
		///</summary>

		protected abstract void PushChangesToObject(ref EditableObject data);
		private void PushChangesToObject() =>
			PushChangesToObject(ref _editObject);

		/// <summary>
		/// Pulls the given <paramref name="data"/> and initializes this window using its "cold" data.
		///</summary>

		protected abstract void PullObjectToWindow(EditableObject data);
		private void PullObjectToWindow() =>
			PullObjectToWindow(_editObject);

		/// <summary>
		/// Loads the given <paramref name="data"/> into the view(s) of this <see cref="EditorWindow"/>.
		///</summary>

		public void Open(EditableObject data)
		{
			data.AssertEditorType(GetType());

			_editObject = data;
			PullObjectToWindow();

			isModified = false;
		}
		public void Open(string filePath)
		{
			var __loadedObject = AssetDatabase.LoadAssetAtPath<EditableObject>(filePath);
			Open(__loadedObject);
		}

		#endregion

		#endregion
	}

	#endregion
}
