
/** Utils.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using System.Text;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

#endregion

namespace Mithril.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public static class Utils
	{
		#region GraphView

		public static Vector2 GetPositionOnly(this GraphElement element) =>
			element.GetPosition().position;

		public static Vector2 GetSizeOnly(this GraphElement element) =>
			element.GetPosition().size;

		public static void SetPositionOnly(this GraphElement element, Vector2 position) =>
			element.SetPosition(new Rect(position, element.GetSizeOnly()));

		public static void SetSizeOnly(this GraphElement element, Vector2 size) =>
			element.SetPosition(new Rect(element.GetPositionOnly(), size));

		public static void AddToSelection(this GraphView graph, IEnumerable<ISelectable> selectables)
		{
			foreach (var iSelectable in selectables)
				graph.AddToSelection(iSelectable);
		}

		public static void SetSelection(this GraphView graph, ISelectable selectable)
		{
			graph.ClearSelection();
			graph.AddToSelection(selectable);
		}

		public static void SetSelection(this GraphView graph, IEnumerable<ISelectable> selectable)
		{
			graph.ClearSelection();
			graph.AddToSelection(selectable);
		}

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

		// public static void SaveAsset(EditableObject obj)
		// {
		// 	var __path = AssetDatabase.GetAssetPath(obj);
		// 	var __clone = (UnityEngine.Object)obj.Clone();

		// 	AssetDatabase.DeleteAsset(__path);
		// 	AssetDatabase.CreateAsset(__clone, __path);
		// 	AssetDatabase.SaveAssets();
		// }

		public static void SaveAsset(UnityEngine.Object obj)
		{
			var __path = AssetDatabase.GetAssetPath(obj);

			if (__path == null)
				AssetDatabase.CreateAsset(obj, __path);
			else
				EditorUtility.SetDirty(obj);

			AssetDatabase.SaveAssetIfDirty(obj);
			AssetDatabase.Refresh();
		}

		public static void SaveAssetSerialized<T>(T obj)
		where T : ScriptableObject
		{
			SaveAsset(obj);

			Serialization.EncodeToFile(obj, false);
		}

		// public static void SaveAssetAtFilePath(UnityEngine.Object obj, string path, bool warnIfExisting = true)
		// {
		// 	var __path = AssetDatabase.GetAssetPath(obj);

		// 	if (__path != null && __path == path)
		// 	{
		// 		if (warnIfExisting)
		// 			try { PromptConfirmation($"\"{path}\"\n\nThis path is already in use. Overwrite?"); }
		// 			catch { return; }

		// 		AssetDatabase.DeleteAsset(path);
		// 	}

		// 	AssetDatabase.CreateAsset(obj, path);
		// 	AssetDatabase.SaveAssets();
		// }
		// public static void SaveAssetAtFilePath(EditableObject obj, string path, bool warnIfExisting = true) =>
		// 	SaveAssetAtFilePath((UnityEngine.Object)obj.Clone(), path, warnIfExisting);

		// public static void SaveAssetInCurrentFolder(UnityEngine.Object o, string localPath) =>
		// 	SaveAssetAtFilePath(o, $"{CurrentProjectWindowFolderPath}/{localPath}");
	}
}
