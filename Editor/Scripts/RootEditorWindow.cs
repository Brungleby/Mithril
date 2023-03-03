
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

	#region ReplicableEditorWindow

	public abstract class ReplicableEditorWindow : EditorWindow
	{
		public static T Instantiate<T>(string title, string iconPath)
		where T : ReplicableEditorWindow
		{
			var __window = EditorWindow.CreateInstance<T>();
			Utils.InitializeWindow(__window, title, iconPath);

			return __window;
		}
		public static T Instantiate<T>(string title)
		where T : ReplicableEditorWindow
		{
			var __window = EditorWindow.CreateInstance<T>();
			__window.titleContent = new GUIContent(title);

			return __window;
		}
	}

	#endregion
}
