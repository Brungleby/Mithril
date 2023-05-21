
/** SaveableObject.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections.Generic;
using Mithril.Editor;

using UnityEditor;
using UnityEngine;


#endregion

namespace Mithril
{
	#region MirroredObject

	public abstract class MirroredObject : ScriptableObject, ISerializationCallbackReceiver
	{
		#region Inners
#if UNITY_EDITOR
		[CustomEditor(typeof(MirroredObject), true)]
		public class MirroredObjectEditor : UnityEditor.Editor
		{
			#region Data

			private static bool _isInspectingDirectValues = false;

			#endregion
			#region Methods

			public override void OnInspectorGUI()
			{
				var __target = (MirroredObject)target;

				/** <<============================================================>> **/
				/**	Display Fields
				*/

				_isInspectingDirectValues = !EditorGUILayout.Toggle(
					new GUIContent("Lock Inspector", "If unlocked, this object can still be modified using the EditorWindows listed above."),
					!_isInspectingDirectValues
				);

				if (_isInspectingDirectValues)
					EditorGUI.BeginChangeCheck();
				else
					EditorGUI.BeginDisabledGroup(true);

				OnInspectorGUI_Fields();

				if (_isInspectingDirectValues)
				{
					if (EditorGUI.EndChangeCheck())
						__target._isBeingModifiedInInspector = true;
				}
				else
					EditorGUI.EndDisabledGroup();
			}

			/// <summary>
			/// Custom implementation of <see cref="OnInspectorGUI"/>. <see cref="EditableObject.OnAfterDeserialize"/> is postponed until after this method has completed to allow values to be edited properly.
			///</summary>

			protected virtual void OnInspectorGUI_Fields()
			{
				base.OnInspectorGUI();
			}

			#endregion
		}
#endif
		#endregion

		/// <summary>
		/// Mirror of this object; stores json information for each field (not including this one).
		///</summary>

		[NonMirrored]
		[SerializeField]
		[HideInInspector]
		private Mirror _reflection;
		public Mirror reflection { get => _reflection; set => _reflection = value; }

		/// <summary>
		/// Updated by <see cref="EditableObject.EditableObjectEditor"/>; indicates whether or not it is being directly edited in the inspector.
		///</summary>

		private bool _isBeingModifiedInInspector = false;
		private bool _isEnabled = false;

		private double _whenSaved;
		public double whenSaved => _whenSaved;

		#region Methods

		#region Construction

		protected virtual void OnEnable()
		{
			_isEnabled = true;

			LoadReflection();
		}

		protected virtual void OnDisable()
		{
			_isEnabled = false;

			SaveReflection();
		}

		#endregion
		#region ISerializationCallbackReceiver

		public virtual void Save()
		{
			SaveReflection();

			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssetIfDirty(this);
			AssetDatabase.Refresh();
		}

		public virtual void LoadReflection()
		{
			if (_reflection != null)
				_reflection.ApplyReflectionTo(this);
		}

		public virtual void SaveReflection()
		{
			_whenSaved = EditorApplication.timeSinceStartup;

			_isBeingModifiedInInspector = true;
			_reflection = new Mirror(this);
			_isBeingModifiedInInspector = false;
		}

		public virtual void OnAfterDeserialize()
		{
			// if (_isEnabled && !_isBeingModifiedInInspector)
			// 	LoadReflection();

			_isBeingModifiedInInspector = false;
		}

		public virtual void OnBeforeSerialize()
		{
			if (_isEnabled && !_isBeingModifiedInInspector)
				SaveReflection();

			_isEnabled = false;
		}

		#endregion
		#endregion

	}

	#endregion
	#region EditableObject

	/// <summary>
	/// A more feature-rich implementation of <see cref="ScriptableObject"/> which serializes its contents properly, and is suitable for storing polymorphic data.
	///</summary>

	[Serializable]
	public abstract class EditableObject : MirroredObject
	{
		#region Inners

		#region Editor

		/// <summary>
		/// A special editor designed for use with <see cref="EditableObject"/>s (and any child class).
		///</summary>

		[CustomEditor(typeof(EditableObject), true)]
		public class EditableObjectEditor : MirroredObjectEditor
		{
			#region Methods

			/// <summary>
			/// This implementation of <see cref="UnityEditor.Editor.OnInspectorGUI"/> is unique. Functional GUI elements should be implemented here, but fields that can be modified should be implemented in <see cref="OnInspectorGUI_Fields"/>.
			///</summary>

			public override void OnInspectorGUI()
			{
				/** <<============================================================>> **/

				var __target = (EditableObject)target;

				/** <<============================================================>> **/
				/**	Display Debug Print Button
				*/

				var currentEvent = Event.current;
				if (currentEvent.alt)
				{
					if (GUILayout.Button("Copy JSON"))
						GUIUtility.systemCopyBuffer = __target.reflection.ToString();
				}
				else
				{
					if (GUILayout.Button("Print JSON"))
						Debug.Log(__target.reflection.ToString());
				}

				/** <<============================================================>> **/
				/**	Display Editors
				*/

				var __types = __target.compatibleWindows;

				if (__types.Length > 0)
				{
					for (var i = 0; i < __types.Length; i++)
					{
						if (GUILayout.Button($"Open with {__types[i].Name}"))
							__target.Open(__types[i]);
					}
				}
				else
				{
					GUILayout.Label(
						$"No EditorWindows support editing this type of EditableObject.\nTo add one or more, override the {nameof(EditableObject.compatibleWindows)} property for this class.",
						GUILayout.ExpandHeight(true)
					);
				}

				base.OnInspectorGUI();
			}

			#endregion
		}

		#endregion

		#endregion

		#region Data

#if UNITY_EDITOR
#if !UNITY_INCLUDE_TESTS
		private InstantiableWindow _currentlyOpenEditor;
#else
		[NonMirrored]
		[HideInInspector]
		public EditableWindow _currentlyOpenEditor;
#endif
		[SerializeField]
		[HideInInspector]
		private bool _isAutosaved = true;
		public bool isAutosaved
		{
			get => _isAutosaved;
			set => _isAutosaved = value;
		}
#endif

		#endregion
		#region Properties

		public string filePath => AssetDatabase.GetAssetPath(this);
#if UNITY_EDITOR
		public string fileName => name;

		public virtual Type[] compatibleWindows =>
			new Type[0];
#endif
		#endregion
		#region Methods

		#region EditorWindow
#if UNITY_EDITOR
		#region Open

		public EditableWindow Open(Type type)
		{
			if (_currentlyOpenEditor == null)
			{
				// LoadReflection();

				_currentlyOpenEditor = EditableWindow.Instantiate(type, this);
				_currentlyOpenEditor.Show();
			}
			else
			{
				if (_currentlyOpenEditor.GetType() == type)
					_currentlyOpenEditor.Focus();
				else
				{
					Mithril.Editor.Utils.PromptConfirmation("A different type of window currently editing this object is still open. Click OK to save the asset, close the existing window, and proceed opening this one.");

					/**	__TODO_IMPLEMENT__
					*/
				}
			}

			return _currentlyOpenEditor;
		}
		public T Open<T>()
		where T : EditableWindow =>
			(T)Open(typeof(T));
		public EditableWindow Open() =>
			Open(compatibleWindows[0]);

		[UnityEditor.Callbacks.OnOpenAsset]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			try
			{
				var __target = (EditableObject)EditorUtility.InstanceIDToObject(instanceID);
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
		#endregion

		#endregion
	}

	#endregion
}
