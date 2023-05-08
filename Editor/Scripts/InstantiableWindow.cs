
/** InstantiableWindow.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

using UnityEditor;
using UnityEditor.UIElements;

#endregion

namespace Mithril.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public enum ModifierKeys
	{
		Control = 1,
		Alt = 2,
		Shift = 4,
	}

	public abstract class InstantiableWindow : EditorWindow
	{
		#region Inners

		private class LockableButton : Button
		{
			private bool _isUnlocked;
			public virtual bool isUnlocked
			{
				get => _isUnlocked;
				set
				{
					_isUnlocked = value;
					SetEnabled(isUnlocked);
				}
			}

			public LockableButton(Action action) : base(action) { }
		}

		private class SaveButton : LockableButton
		{
			readonly static private float WIDTH =
				80f;

			readonly static private string TEXT_MANUAL =
				"Save Asset";
			readonly static private string TEXT_AUTOMATIC =
				"Autosaved";
			readonly static private string TEXT_INPROGRESS =
				"Saving Asset...";

			private bool _isSaving;
			public bool isSaving
			{
				get => _isSaving;
				set
				{
					_isSaving = value;
					RefreshText();
				}
			}

			public override bool isUnlocked
			{
				get => base.isUnlocked && !_isSaving;
				set
				{
					base.isUnlocked = value;
					RefreshText();
				}
			}

			public SaveButton(Action action) : base(action)
			{
				style.width = WIDTH;
			}

			private void RefreshText()
			{
				text = _isSaving ? TEXT_INPROGRESS :
					isUnlocked ? TEXT_MANUAL :
					TEXT_AUTOMATIC
				;
			}
		}

		#endregion
		#region Data

		readonly static public string DEFAULT_ICON_PATH =
			"Assets/Mithril/Mithril.Core/Editor/Resources/Textures/Icon_Diamond.png";

		readonly static public float TOOLBAR_TOGGLE_PADDING_TOP =
			1f;
		readonly static public float TOOLBAR_PADDING_LEFT =
			3f;

		#region

		private EditableObject _workObject;
		public EditableObject workObject => _workObject;

		private bool _isModified;
		public bool isModified
		{
			get =>
				_isModified && !_workObject.isAutosaved;
			private set
			{
				if (value && _workObject.isAutosaved)
				{
					Save();
					return;
				}

				_isModified = value;
				RefreshTitleText();
			}
		}

		private Toolbar _toolbar;
		protected Toolbar toolbar => _toolbar;

		private Toggle _autosaveToggleElement;
		private SaveButton _saveButtonElement;

		#endregion

		#endregion
		#region Properties

		public virtual string iconPath =>
			DEFAULT_ICON_PATH;

		public bool isAutosaved =>
			_workObject.isAutosaved;

		#endregion
		#region Methods

		protected virtual void OnEnable()
		{
			CreateVisualElements();
		}

		protected virtual void OnDisable()
		{
			DisposeVisualElements();
		}

		#region Instantiation

		/// <summary>
		/// Creates a window of the specified <paramref name="type"/> to edit the given <paramref name="obj"/>.
		/// Or, if the object is already being edited, it will simply focus that window.
		///</summary>

		public static InstantiableWindow Instantiate(Type type, EditableObject obj)
		{
			/** <<============================================================>> **/
			/**	If a window already editing this specific object exists,
			*	then we'll simply use that.
			*/

			var __allWindowsOfMatchingType = Resources
				.FindObjectsOfTypeAll(type)
				.Cast<InstantiableWindow>()
			;

			foreach (var iWindow in __allWindowsOfMatchingType)
			{
				if (iWindow._workObject.filePath == obj.filePath)
				{
					iWindow.Focus();
					return iWindow;
				}
			}

			/** <<============================================================>> **/
			/**	Otherwise, create a new window.
			*/

			var __window = (InstantiableWindow)EditorWindow.CreateInstance(type);
			__window.Initialize(obj);

			return __window;
		}

		/// <inheritdoc cref="Instantiate"/>

		public static T Instantiate<T>(EditableObject obj)
		where T : InstantiableWindow =>
			(T)Instantiate(typeof(T), obj);

		/// <summary>
		/// Loads the given <paramref name="obj"/> into the view(s) of this <see cref="InstantiableWindow"/>.
		///</summary>

		private void Initialize(EditableObject obj)
		{
			AssertObjectWindowCompatible(obj);

			_workObject = obj;
			InitializeForWorkObject();

			Utils.InitializeWindowHeader(this, obj.fileName, iconPath);
			isModified = false;

			_autosaveToggleElement.value = _workObject.isAutosaved;
			OnGUI();
		}

		public virtual void InitializeForWorkObject() { }

		/// <summary>
		/// Loads the given <paramref name="filePath"/> as a <see cref="EditableObject"/> into the view(s) of this <see cref="InstantiableWindow"/>.
		///</summary>

		public void Open(string filePath)
		{
			var __loadedObject = AssetDatabase.LoadAssetAtPath<EditableObject>(filePath);
			Initialize(__loadedObject);
		}

		#endregion
		#region Setup

		protected virtual void CreateVisualElements()
		{
			_toolbar = new Toolbar();
			InitializeToolbar(_toolbar);
			rootVisualElement.Add(_toolbar);

			rootVisualElement.style.paddingTop = _toolbar.style.height;
		}

		protected virtual void InitializeToolbar(Toolbar toolbar)
		{
			toolbar.style.paddingLeft = TOOLBAR_PADDING_LEFT;

			/** <<============================================================>> **/

			_saveButtonElement = new SaveButton(
				() => Save()
			)
			{
				text = "Save Asset"
			};

			toolbar.Add(_saveButtonElement);

			/** <<============================================================>> **/

			_autosaveToggleElement = new Toggle();
			_autosaveToggleElement.style.paddingTop = TOOLBAR_TOGGLE_PADDING_TOP;

			if (_workObject != null)
				_autosaveToggleElement.value = _workObject.isAutosaved;

			toolbar.Add(_autosaveToggleElement);
		}

		protected virtual void DisposeVisualElements() { }

		protected virtual void OnGUI()
		{
			RefreshSaveElements();
		}

		private void RefreshSaveElements()
		{
			_workObject.isAutosaved = _autosaveToggleElement.value;

			_saveButtonElement.isUnlocked = !_workObject.isAutosaved;

		}

		private void RefreshTitleText()
		{
			titleContent.text = _workObject.fileName + (isModified ? "*" : "");
		}

		private void AssertObjectWindowCompatible(EditableObject obj)
		{
			if (!obj.compatibleEditorWindows.Contains(GetType()))
				throw new NotSupportedException($"{obj.name} ({obj.GetType()}) cannot be opened with this type of EditorWindow ({GetType()}).");
		}

		#endregion
		#region Save/Load

		public virtual void Save()
		{
			_workObject.Save();

			isModified = false;
		}

		protected void NotifyIsModified()
		{
			isModified = true;
		}

		// /// <summary>
		// /// Updates the given <paramref name="data"/> using the "hot" data inside this window.
		// ///</summary>

		// protected abstract void PushChangesToObject(ref ForgeObject data);
		// private void PushChangesToObject() =>
		// 	PushChangesToObject(ref _editObject);

		// /// <summary>
		// /// Pulls the given <paramref name="data"/> and initializes this window using its "cold" data.
		// ///</summary>

		// protected abstract void PullObjectToWindow(ForgeObject data);
		// private void PullObjectToWindow() =>
		// 	PullObjectToWindow(_editObject);

		#endregion

		#endregion
	}
}
