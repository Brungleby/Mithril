
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
		private const float IO_LABEL_WIDTH = 24f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			float fieldWidth = position.width - EditorGUIUtility.labelWidth - IO_LABEL_WIDTH - EditorGUIUtility.standardVerticalSpacing * 2f;
			float halfFieldWidth = fieldWidth * 0.5f - EditorGUIUtility.standardVerticalSpacing * 2f;

			float fieldHeight = EditorGUIUtility.singleLineHeight;

			float x0 = position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing * 2f;
			float x1 = x0 + IO_LABEL_WIDTH + EditorGUIUtility.standardVerticalSpacing * 2f;
			float x2 = x1 + halfFieldWidth + EditorGUIUtility.standardVerticalSpacing * 2f;

			float y1 = position.y;
			float y2 = y1 + fieldHeight + EditorGUIUtility.standardVerticalSpacing;

			EditorGUI.BeginProperty(position, label, property);
			{

				Rect labelRect = new(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
				Rect iLabelRect = new(x0, y1, IO_LABEL_WIDTH, EditorGUIUtility.singleLineHeight);
				Rect oLabelRect = new(x0, y2, IO_LABEL_WIDTH, EditorGUIUtility.singleLineHeight);

				float clampWidth = 16f + EditorGUIUtility.standardVerticalSpacing;
				Rect clampToggleRect = new(x0 - clampWidth + EditorGUIUtility.standardVerticalSpacing, y2, clampWidth + 40f, fieldHeight);
				Rect clampLabelRect = new(clampToggleRect.x - (EditorGUIUtility.standardVerticalSpacing + 40f), y2, 40f, fieldHeight);

				Rect inMinRect = new(x1, y1, halfFieldWidth, fieldHeight);
				Rect inMaxRect = new(x2, y1, halfFieldWidth, fieldHeight);
				Rect outMinRect = new(x1, y2, halfFieldWidth, fieldHeight);
				Rect outMaxRect = new(x2, y2, halfFieldWidth, fieldHeight);

				EditorGUI.LabelField(labelRect, label);
				EditorGUI.LabelField(clampLabelRect, "Clamp");
				EditorGUI.LabelField(iLabelRect, "In");
				EditorGUI.LabelField(oLabelRect, "Out");

				var clampToggle = property.FindPropertyRelative("clamp");

				var inMin = property.FindPropertyRelative("inMin");

				var inMax = property.FindPropertyRelative("inMax");

				var outMin = property.FindPropertyRelative("outMin");

				var outMax = property.FindPropertyRelative("outMax");

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
