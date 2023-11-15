
/** MapFieldPropertyDrawer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#endregion

namespace Mithril.Editor
{
	#region MapFieldPropertyDrawer

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>
	[CustomPropertyDrawer(typeof(DictionaryField<,>))]
	public sealed class DictionaryFieldPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.PropertyField(position, property.FindPropertyRelative("_contents"), label);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_contents"));
		}
	}

	#endregion
}
