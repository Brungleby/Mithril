
/** EditableObjectPropertyDrawer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
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
			var __types = __target.UsableEditorTypes;

			if (__types.Length > 0)
			{
				for (var i = 0; i < __types.Length; i++)
				{
					if (GUILayout.Button($"Open with {__types[i].Name}"))
						__target.Open(__types[i]);
				}
			}
			else
				GUILayout.Label("This EditableObject does not support any editors.");

			base.OnInspectorGUI();
		}
	}
}
