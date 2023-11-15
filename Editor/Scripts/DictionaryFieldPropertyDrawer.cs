
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
			EditorGUI.BeginProperty(position, label, property);

			var contentsProp = property.FindPropertyRelative("_contents");

			var validationDict = new Dictionary<object, object>();
			for (var i = 0; i < contentsProp.arraySize; i++)
			{
				var iKeyProp = contentsProp.GetArrayElementAtIndex(i).FindPropertyRelative("key");
				var iKey = iKeyProp.boxedValue;
				var iValue = contentsProp.GetArrayElementAtIndex(i).FindPropertyRelative("value").boxedValue;

				if (iKey == default) continue;
				if (validationDict.ContainsKey(iKey))
				{
					iKeyProp.boxedValue = default;
				}
				else
				{
					validationDict[iKey] = iValue;
				}
			}

			EditorGUI.PropertyField(position, contentsProp, label);
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_contents"));
		}
	}

	#endregion
}
