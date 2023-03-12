
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

	public abstract class InstantiableEditorWindow<TSaveData> :
	EditorWindow

	where TSaveData : EditableObject
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
			Utils.InitializeWindowHeader(this, _rawTitle, iconPath);

			_filePath = filePath;
			LoadFile();
		}

		public static InstantiableEditorWindow<TSaveData> Instantiate(System.Type type, string filePath, string iconPath)
		{
			/** <<============================================================>> **/

			var __allWindowsOfMatchingType = Resources.FindObjectsOfTypeAll(type)
			.Where(i => i != null)
			.Cast<InstantiableEditorWindow<TSaveData>>()
			// .ToList()
			;

			Debug.Log(__allWindowsOfMatchingType.Count());

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

			var __window = (InstantiableEditorWindow<TSaveData>)EditorWindow.CreateInstance(type);
			__window.Initialize(filePath, iconPath);

			/** <<============================================================>> **/

			return __window;
		}

		public static InstantiableEditorWindow<TSaveData> Instantiate(System.Type type, string filePath) =>
			Instantiate(type, filePath, DEFAULT_ICON_PATH);

		public static T Instantiate<T>(string filePath, string iconPath)
		where T : InstantiableEditorWindow<TSaveData> =>
			(T)Instantiate(typeof(T), filePath, iconPath);
		public static T Instantiate<T>(string filePath)
		where T : InstantiableEditorWindow<TSaveData> =>
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
			toolbar.Add(new Button(() => SaveFile()) { text = "Save Asset" });
		}

		protected virtual void CleanUpVisualElements() { }

		#endregion
		#region Save/Load

		/// <summary>
		/// Initializes the <paramref name="data"/> of the generic type before it is saved to the file.
		///</summary>

		protected abstract void SaveData(in TSaveData data);

		/// <summary>
		/// Saves the current <see cref="filePath"/> as a ScriptableObject.
		///</summary>

		public void SaveFile()
		{
			var __data = ScriptableObject.CreateInstance<TSaveData>();
			SaveData(__data);

			Utils.CreateAssetAtFilePath(__data, filePath, false);

			isModified = false;
		}

		/// <summary>
		/// Loads and initializes the given <paramref name="data"/> into the view(s) of this <see cref="EditorWindow"/>.
		///</summary>

		protected abstract void LoadData(in TSaveData data);

		/// <summary>
		/// Loads the current <see cref="filePath"/> into the view(s) of this <see cref="EditorWindow"/>.
		///</summary>

		public void LoadFile()
		{
			TSaveData __cache = AssetDatabase.LoadAssetAtPath<TSaveData>(filePath);

			LoadData(__cache);

			isModified = false;
		}

		#endregion
		#endregion
	}
	public abstract class InstantiableEditorWindow : InstantiableEditorWindow<EditableObject> { }

	#endregion
}
