
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
	/// Simple script that adds buttons to edit this object in compatible editors.
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
}
