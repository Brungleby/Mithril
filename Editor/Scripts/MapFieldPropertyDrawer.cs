
/** MapFieldPropertyDrawer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEditor;
using UnityEngine;


#endregion

namespace Mithril.Editor
{
	#region MapFieldPropertyDrawer

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>
	[CustomPropertyDrawer(typeof(MapField<,>))]
	public sealed class MapFieldPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{           // Example: Displaying a property field for a specific property of MyCustomClass
			SerializedProperty customProperty = property.FindPropertyRelative("_FieldPairs");
			EditorGUI.PropertyField(position, customProperty, label);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_FieldPairs"));
		}
	}

	#endregion
}
