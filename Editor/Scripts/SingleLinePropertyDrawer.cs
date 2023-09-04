
/** SingleLinePropertyDrawer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#if UNITY_EDITOR
#region Includes

using UnityEngine;
using UnityEditor;

#endregion

namespace Mithril
{
	#region SingleLinePropertyDrawer

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public abstract class SingleLinePropertyDrawer : PropertyDrawer
	{
		/// <returns>
		/// Pairs of properties and their associated width percentages.
		///</returns>

		protected abstract (string, float)[] propertyWidths { get; }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			{
				var __labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);

				EditorGUI.LabelField(__labelRect, label);

				/** <<============================================================>> **/

				var __frameWidth = position.width - EditorGUIUtility.labelWidth;
				var __cursor = EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing + position.x;

				for (var i = 0; i < propertyWidths.Length; i++)
				{
					var __width = __frameWidth * propertyWidths[i].Item2 - EditorGUIUtility.standardVerticalSpacing;
					var __x = __cursor;
					var __rect = new Rect(__x, position.y, __width, position.height);
					var __prop = property.FindPropertyRelative(propertyWidths[i].Item1);

					EditorGUI.PropertyField(__rect, __prop, GUIContent.none);

					__cursor += __width + EditorGUIUtility.standardVerticalSpacing;
				}
			}
			EditorGUI.EndProperty();
		}
	}

	#endregion
}
#endif
