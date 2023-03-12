
/** EditableObject.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;

using UnityEngine;
using UnityEditor;

#endregion

namespace Cuberoot
{
	[Serializable]
	public abstract class EditableObject : ScriptableObject
	{
		public abstract Type[] UsableEditorTypes { get; }

		private bool _isInitialized = false;
		public bool isInitialized => _isInitialized;

		public virtual void Initialize()
		{
			_isInitialized = true;
		}

		public void OpenWithEditorIndex(int i)
		{
			if (i >= UsableEditorTypes.Length)
				throw new System.IndexOutOfRangeException();

			var __filePath = AssetDatabase.GetAssetPath(this);
			var __window = Editor.InstantiableEditorWindow.Instantiate(UsableEditorTypes[i], __filePath);

			__window.Show();
		}

		public void Open() =>
			OpenWithEditorIndex(0);

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
	}

	/// <summary>
	/// This is a special kind of ScriptableObject that may be opened and editing using a predefined EditorWindow.
	///</summary>

	// [Serializable]
	// public sealed class EditorOpener : UnityEngine.Object
	// {
	// 	private Type _UsableEditor;
	// 	public string EditorName => _UsableEditor.Name;

	// 	public void OpenInEditor()
	// 	{
	// 		var window = (EditorWindow)EditorWindow.GetWindow(_UsableEditor);
	// 		window.Show();
	// 	}
	// }

	// [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
	// public sealed class EditableObjectPropertyAttribute : PropertyAttribute
	// {
	// 	public string label;

	// 	public EditableObjectPropertyAttribute(string label)
	// 	{
	// 		this.label = label;
	// 	}
	// }
}
