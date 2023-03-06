
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
#if UNITY_EDITOR
		public abstract Type[] UsableEditorTypes { get; }


		public void OpenWithEditorIndex(int i)
		{
			var __filePath = AssetDatabase.GetAssetPath(this);
			var __window = Editor.InstantiableEditorWindow.Instantiate(UsableEditorTypes[i], __filePath);

			__window.Show();
		}
#endif
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
