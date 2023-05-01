
/** ForgeObject.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Mithril.Editor;

#endregion

namespace Mithril
{
	/// <summary>
	/// A ForgeObject is a SmartObject that can store its data in a file and be edited using ForgeWindow(s).
	///</summary>

	public abstract class ForgeObject : SmartObject
	{
		#region Inners

		/// <summary>
		/// Simple script that adds buttons to edit this object in compatible editors.
		///</summary>

		[CustomEditor(typeof(ForgeObject), true)]
		public class ForgeObjectEditor : SmartObject.SmartObjectEditor
		{
			public override void OnInspectorGUI()
			{
				var __target = (ForgeObject)target;
				var __types = __target.usableEditorWindows;

				if (__types.Length > 0)
				{
					for (var i = 0; i < __types.Length; i++)
					{
						if (GUILayout.Button($"Open with {__types[i].Name}"))
							__target.Open(__types[i]);
					}
				}
				else
					GUILayout.Label("This ForgeObject does not support any editors.");

				base.OnInspectorGUI();
			}
		}

		#endregion
		#region Data

		#region

		private bool _isSaving = false;
		public bool isSaving => _isSaving;

#if UNITY_EDITOR
		private ForgeWindow _currentlyOpenEditor;

		private bool _isAutosaved = true;
		public bool isAutosaved
		{
			get => _isAutosaved;
			set => _isAutosaved = value;
		}
#endif

		#endregion

		#endregion
		#region Properties

		public string filePath => AssetDatabase.GetAssetPath(this);
#if UNITY_EDITOR
		public string fileName => name;

		public abstract Type[] usableEditorWindows { get; }

#endif
		#endregion
		#region Methods

		public void Save()
		{
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssetIfDirty(this);
			AssetDatabase.Refresh();
		}

#if UNITY_EDITOR
		#region Open

		public ForgeWindow Open(Type type)
		{
			if (_currentlyOpenEditor == null)
			{
				_currentlyOpenEditor = ForgeWindow.Instantiate(type, this);
				_currentlyOpenEditor.Show();
			}
			else
			{
				if (_currentlyOpenEditor.GetType() == type)
					_currentlyOpenEditor.Focus();
				else
					Mithril.Editor.Utils.PromptConfirmation("A different type of window currently editing this object is still open. Click OK to save the asset, close the existing window, and proceed opening this one.");
			}

			return _currentlyOpenEditor;
		}
		public T Open<T>()
		where T : ForgeWindow =>
			(T)Open(typeof(T));
		public void Open() =>
			Open(usableEditorWindows[0]);

		[UnityEditor.Callbacks.OnOpenAsset]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			try
			{
				var __target = (ForgeObject)EditorUtility.InstanceIDToObject(instanceID);
				__target.Open();
			}
			catch
			{ return false; }
			return true;
		}

		#endregion

		public void Close()
		{
			if (_currentlyOpenEditor == null)
				return;

			_currentlyOpenEditor.Close();
			_currentlyOpenEditor = null;
		}

#endif
		#region

		#endregion

		#endregion
	}
}
