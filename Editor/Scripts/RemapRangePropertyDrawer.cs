
/** RemapRangePropertyDrawer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;
using UnityEditor;

#endregion

namespace Mithril
{
	#region RemapRangePropertyDrawer

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>
	[CustomPropertyDrawer(typeof(RemapRange))]
	public sealed class RemapRangePropertyDrawer : PropertyDrawer
	{
		private const float IO_LABEL_WIDTH = 28f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			float fieldWidth = position.width - labelWidth - EditorGUIUtility.standardVerticalSpacing;
			float halfFieldWidth = fieldWidth * 0.5f - EditorGUIUtility.standardVerticalSpacing;

			float fieldHeight = EditorGUIUtility.singleLineHeight;

			float x1 = position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
			float x2 = x1 + halfFieldWidth + EditorGUIUtility.standardVerticalSpacing * 2f;
			float y1 = position.y;
			float y2 = y1 + fieldHeight + EditorGUIUtility.standardVerticalSpacing;

			EditorGUI.BeginProperty(position, label, property);
			{
				Rect labelRect = new(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
				Rect iLabelRect = new(x1 - IO_LABEL_WIDTH, y1, IO_LABEL_WIDTH, EditorGUIUtility.singleLineHeight);
				Rect oLabelRect = new(x1 - IO_LABEL_WIDTH, y2, IO_LABEL_WIDTH, EditorGUIUtility.singleLineHeight);
				Rect clampLabelRect = new(position.x + 16f + EditorGUIUtility.standardVerticalSpacing, y2, 40f, fieldHeight);

				EditorGUI.LabelField(labelRect, label);
				EditorGUI.LabelField(clampLabelRect, "Clamp");
				EditorGUI.LabelField(iLabelRect, "In");
				EditorGUI.LabelField(oLabelRect, "Out");

				var clampToggle = property.FindPropertyRelative("clamp");
				Rect clampToggleRect = new(position.x, y2, 16f + EditorGUIUtility.standardVerticalSpacing + 40f, fieldHeight);

				var inMin = property.FindPropertyRelative("inMin");
				Rect inMinRect = new(x1, y1, halfFieldWidth, fieldHeight);

				var inMax = property.FindPropertyRelative("inMax");
				Rect inMaxRect = new(x2, y1, halfFieldWidth, fieldHeight);

				var outMin = property.FindPropertyRelative("outMin");
				Rect outMinRect = new(x1, y2, halfFieldWidth, fieldHeight);

				var outMax = property.FindPropertyRelative("outMax");
				Rect outMaxRect = new(x2, y2, halfFieldWidth, fieldHeight);

				clampToggle.boolValue = EditorGUI.Toggle(clampToggleRect, clampToggle.boolValue);

				inMin.floatValue = EditorGUI.FloatField(inMinRect, inMin.floatValue);
				inMax.floatValue = EditorGUI.FloatField(inMaxRect, inMax.floatValue);
				outMin.floatValue = EditorGUI.FloatField(outMinRect, outMin.floatValue);
				outMax.floatValue = EditorGUI.FloatField(outMaxRect, outMax.floatValue);
			}
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight * 2f;
		}
	}

	#endregion
}
