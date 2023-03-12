
/** Utils.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

#endregion

namespace Cuberoot.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public static class Utils
	{
		#region GraphView

		public static void SetPositionOnly(this GraphElement element, Vector2 position) =>
			element.SetPosition(new Rect(position, element.GetPosition().size));

		public static void SetSizeOnly(this GraphElement element, Vector2 size) =>
			element.SetPosition(new Rect(element.GetPosition().position, size));

		#endregion
		#region EditorWindow

		public static T GetShowWindow<T>(string title, string iconPath)
		where T : EditorWindow
		{
			var __window = EditorWindow.GetWindow<T>();
			InitializeWindowHeader(__window, title, iconPath);

			return __window;
		}
		public static T GetShowWindow<T>(string title)
		where T : EditorWindow =>
			EditorWindow.GetWindow<T>(title);

		public static void InitializeWindowHeader(EditorWindow window, string title, string iconPath)
		{
			var __icon = AssetDatabase.LoadAssetAtPath<Texture>(iconPath);

			window.titleContent = new GUIContent(title, __icon);
		}

		#endregion

		public static void AssertObject(object o, string message)
		{
			if (o == null)
			{
				EditorUtility.DisplayDialog("Invalid object", message, "OK");
				throw new NullReferenceException();
			}
		}

		public static void AssertFileName(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "OK");
				throw new UnityException();
			}
		}

		public static void PromptConfirmation(string message)
		{
			if (!EditorUtility.DisplayDialog("Confirm Operation", message, "OK", "Cancel"))
				throw new UnityException("User cancelled operation.");
		}

		public static string CurrentProjectWindowFolderPath
		{
			get
			{
				Type __utilType = typeof(ProjectWindowUtil);
				MethodInfo __get_ActiveFolderPath = __utilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
				object obj = __get_ActiveFolderPath.Invoke(null, new object[0]);
				return obj.ToString();

			}
		}

		public static void CreateAssetAtFilePath(UnityEngine.Object o, string filePath, bool warnIfExisting = true)
		{
			if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath) != null)
			{
				if (warnIfExisting)
					try { PromptConfirmation($"\"{filePath}\"\n\nThis path is already in use. Overwrite?"); }
					catch { return; }

				AssetDatabase.DeleteAsset(filePath);
			}

			try
			{
				AssetDatabase.CreateAsset(o, filePath);
				AssetDatabase.SaveAssets();
			}
			catch
			{
				UnityEngine.Debug.LogError(filePath);
			}
		}
		public static void CreateAssetInCurrentFolder(UnityEngine.Object o, string localPath) =>
			CreateAssetAtFilePath(o, $"{CurrentProjectWindowFolderPath}/{localPath}");
	}
}
