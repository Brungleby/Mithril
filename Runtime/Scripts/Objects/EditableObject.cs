
/** EditableObject.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Cuberoot.Editor;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// A ScriptableObject that initializes its own data using <see cref="EditableData"/>
	/// and can be edited using a <see cref="EditorWindow"/>.
	///</summary>
	[Serializable]

	public abstract class EditableObject : ScriptableObject
	{
		#region Data

		private string _filePath;
		public string filePath => _filePath;

		private string _iconPath;
		public string iconPath => _iconPath;

#if UNITY_EDITOR
		private InstantiableEditorWindow _currentlyOpenEditor;
#endif

		#endregion

		#region Methods

		public EditableObject() { }

		protected virtual void OnEnable()
		{
			_filePath = AssetDatabase.GetAssetPath(this);
		}

		protected virtual void OnDisable() { }

#if UNITY_EDITOR
		public abstract Type[] UsableEditorTypes { get; }

		public void Save()
		{
			Editor.Utils.SaveAssetAtFilePath(this, AssetDatabase.GetAssetPath(this), false);
		}

		/// <summary>
		/// Initializes this object's data from its associated <see cref="EditableData"/>.
		///</summary>

		protected abstract void Compile();

		[UnityEditor.Callbacks.OnOpenAsset]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			var __target = (EditableObject)EditorUtility.InstanceIDToObject(instanceID);

			if (__target != null)
			{
				__target.Open();
				return true;
			}

			return false;
		}


		/** <<============================================================>> **/

		#region Imported from EditableData

		public InstantiableEditorWindow Open(Type type)
		{
			AssertEditorType(type);

			if (_currentlyOpenEditor != null)
			{
				if (UnityEditor.EditorUtility.DisplayDialog("A window currently editing this object is still open. Click OK to save the asset, close the window, and proceed opening this one.", "OK", "Cancel"))
				{
					Close();
				}
				else
					throw new Exception("Editor failed to open.");
			}

			/** <<============================================================>> **/

			_currentlyOpenEditor = InstantiableEditorWindow.Instantiate(type, this);
			_currentlyOpenEditor.Show();

			return _currentlyOpenEditor;
		}

		public void Close()
		{
			_currentlyOpenEditor.Save();
			_currentlyOpenEditor.Close();
			_currentlyOpenEditor = null;
		}

		public T Open<T>()
		where T : InstantiableEditorWindow =>
			(T)Open(typeof(T));

		public InstantiableEditorWindow Open() =>
			Open(UsableEditorTypes[0]);

		public void AssertEditorType(Type type)
		{
			if (!UsableEditorTypes.Contains(type))
				throw new KeyNotFoundException($"\"{filePath}\" ({GetType()}) cannot be opened with \"{type}\".");
		}
		#endregion
#endif
		#endregion
	}
}
