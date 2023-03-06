
/** EditableObjectPropertyDrawer.cs
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
using UnityEditor;

#endregion

namespace Cuberoot.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[CustomEditor(typeof(EditableObject), true)]
	public class EditableObjectEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var __target = (EditableObject)target;

			for (var i = 0; i < __target.UsableEditorTypes.Length; i++)
			{
				if (GUILayout.Button($"Open with {__target.UsableEditorTypes[i].Name}"))
					__target.OpenWithEditorIndex(i);
			}

			base.OnInspectorGUI();
		}
	}

	// [CustomPropertyDrawer(typeof(EditorOpener))]
	// public sealed class EditableObjectPropertyDrawer : PropertyDrawer
	// {
	// 	#region Data

	// 	#region



	// 	#endregion

	// 	#endregion
	// 	#region Methods

	// 	#region

	// 	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	// 	{
	// 		if (property.propertyType == SerializedPropertyType.ObjectReference)
	// 		{
	// 			var editorOpener = (EditorOpener)(property.objectReferenceValue);

	// 			if (editorOpener)
	// 			{
	// 				if (GUI.Button(position, $"Open"))
	// 					Debug.Log(editorOpener.EditorName);
	// 			}
	// 			else
	// 			{
	// 				GUI.Label(position, new GUIContent("This EditorOpener needs a valid editor!"));
	// 			}
	// 		}
	// 	}

	// 	#endregion

	// 	#endregion
	// }
}
