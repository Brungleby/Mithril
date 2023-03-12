
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
	#region InstantiableEditorWindowBase

	public abstract class InstantiableEditorWindowBase : EditorWindow
	{
		#region Data

		#region Constants

		public readonly static string DEFAULT_ICON_PATH =
		"Assets/Cuberoot/Cuberoot.Core/Editor/Resources/Textures/Icon_Diamond.png";

		#endregion
		#region Members

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

		#endregion

		#endregion
		#region Methods
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

		public void Initialize(string filePath, string iconPath)
		{
			_rawTitle = GetTitleFromFilePath(filePath);
			Utils.InitializeWindowHeader(this, _rawTitle, iconPath);

			_filePath = filePath;
			LoadFile();
		}

		public static InstantiableEditorWindowBase Instantiate(System.Type type, string filePath, string iconPath)
		{
			/** <<============================================================>> **/

			var __allWindowsOfMatchingType = Resources.FindObjectsOfTypeAll(type)
				.Cast<InstantiableEditorWindowBase>()
			;

			/** <<============================================================>> **/

			foreach (var iWindow in __allWindowsOfMatchingType)
			{
				if (iWindow._filePath == filePath)
				{
					iWindow.Focus();
					return iWindow;
				}
			}

			/** <<============================================================>> **/

			var __window = (InstantiableEditorWindowBase)EditorWindow.CreateInstance(type);
			__window.Initialize(filePath, iconPath);

			/** <<============================================================>> **/

			return __window;
		}

		public static InstantiableEditorWindowBase Instantiate(System.Type type, string filePath) =>
			Instantiate(type, filePath, DEFAULT_ICON_PATH);

		public static T Instantiate<T>(string filePath, string iconPath)
		where T : InstantiableEditorWindowBase =>
			(T)Instantiate(typeof(T), filePath, iconPath);
		public static T Instantiate<T>(string filePath)
		where T : InstantiableEditorWindowBase =>
			Instantiate<T>(filePath, DEFAULT_ICON_PATH);

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
			toolbar.Add(new Button(() => SaveFile()) { text = "Save Asset" });
		}

		protected virtual void CleanUpVisualElements() { }

		#endregion
		#region Save/Load

		/// <summary>
		/// Saves the current <see cref="filePath"/> as a ScriptableObject.
		///</summary>

		public virtual void SaveFile()
		{
			isModified = false;
		}

		/// <summary>
		/// Loads the current <see cref="filePath"/> into the view(s) of this <see cref="EditorWindow"/>.
		///</summary>

		public virtual void LoadFile()
		{
			isModified = false;
		}

		#endregion

		#endregion
	}

	#endregion
	#region InstantiableEditorWindow

	public abstract class InstantiableEditorWindow<TData> :
	InstantiableEditorWindowBase

	where TData : ScriptableObject
	{
		#region Methods

		public override void SaveFile()
		{
			var __data = ScriptableObject.CreateInstance<TData>();
			SaveData(__data);

			Utils.CreateAssetAtFilePath(__data, filePath, false);

			base.SaveFile();
		}

		public override void LoadFile()
		{
			TData __data = AssetDatabase.LoadAssetAtPath<TData>(filePath);

			LoadData(__data);

			base.LoadFile();
		}

		/// <summary>
		/// Initializes the <paramref name="data"/> of the generic type before it is saved to the file.
		///</summary>

		protected abstract void SaveData(in TData data);

		/// <summary>
		/// Loads and initializes the given <paramref name="data"/> into the view(s) of this <see cref="EditorWindow"/>.
		///</summary>

		protected abstract void LoadData(in TData data);

		#endregion
	}
	public abstract class InstantiableEditorWindow : InstantiableEditorWindow<EditableObject> { }

	#endregion
}
