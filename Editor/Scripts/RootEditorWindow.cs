
/** RootEditorWindow.cs
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

	public abstract class RootEditorWindow : EditorWindow
	{
		#region Data

		#region



		#endregion

		#endregion
		#region Methods

		#region

		public static T GetShowWindow<T>(string title, string iconPath)
		where T : EditorWindow
		{
			var __icon = AssetDatabase.LoadAssetAtPath<Texture>(iconPath);
			var __window = EditorWindow.GetWindow<T>();

			__window.titleContent = new GUIContent(title, __icon);

			return __window;
		}
		public static T GetShowWindow<T>(string title)
		where T : EditorWindow
		{
			return EditorWindow.GetWindow<T>(title);
		}

		#endregion

		#endregion
	}
}
