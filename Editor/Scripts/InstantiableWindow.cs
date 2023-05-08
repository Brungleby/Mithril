
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
	/// EditorWindow used for modifying <see cref="EditableObject"/>s.
	/// Each instance of this window is assigned to a corresponding <see cref="workObject"/>.
	///</summary>

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
				if (_isSaving)
					text = TEXT_INPROGRESS;
				else if (!isUnlocked)
					text = TEXT_AUTOMATIC;
				else
					text = TEXT_MANUAL;
			}
		}

		#endregion
		#region Data

		public readonly static string DEFAULT_ICON_PATH =
			"Assets/Mithril/Mithril.Core/Editor/Resources/Textures/Icon_Diamond.png";

		public readonly static float TOOLBAR_TOGGLE_PADDING_TOP =
			1f;
		public readonly static float TOOLBAR_PADDING_LEFT =
			3f;

		#region

		/// <summary>
		/// Object which is currently being worked on in this window.
		///</summary>

		private EditableObject _workObject;

		/// <inheritdoc cref="_workObject"/>

		public EditableObject workObject => _workObject;

		/// <summary>
		/// Slightly more involved representation of <see cref="hasUnsavedChanges"/>.
		/// Setting this value to true if <see cref="workObject"/> has autosaving enabled will automaticaly save the object and reset the value to false.
		///</summary>

		public bool isModified
		{
			get =>
				hasUnsavedChanges && !_workObject.isAutosaved;
			private set
			{
				if (value && _workObject.isAutosaved)
				{
					SoftSave();
					return;
				}

				hasUnsavedChanges = value;
			}
		}

		private Toolbar _toolbar;
		protected Toolbar toolbar => _toolbar;

		private Toggle _autosaveToggleElement;
		private SaveButton _saveButtonElement;

		#endregion

		#endregion
		#region Properties

		/// <summary>
		/// The path of the icon image to display at the top of the file.
		///</summary>

		public virtual string iconPath =>
			DEFAULT_ICON_PATH;

		/// <summary>
		/// Short-hand for whether or not the current <see cref="workObject"/> has autosaving enabled.
		///</summary>

		public bool isAutosaved =>
			_workObject.isAutosaved;

		#endregion
		#region Methods

		protected virtual void OnEnable()
		{
			SetupVisualElements();
		}

		protected virtual void OnDisable()
		{
			TeardownVisualElements();

			/**	Ensures the workObject's file is saved.
			*/
			if (isAutosaved)
				HardSave();
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
			AssertCompatibleWith(obj);

			_workObject = obj;
			OnSetupForWorkObject();

			Utils.InitializeWindowHeader(this, obj.fileName, iconPath);
			isModified = false;

			_autosaveToggleElement.value = _workObject.isAutosaved;
			saveChangesMessage = GetSaveChangesMessage(obj);

			OnGUI();
		}

		/// <summary>
		/// Override this method to set up this object. Called during <see cref="Initialize"/>.
		///</summary>

		public virtual void OnSetupForWorkObject() { }

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

		/// <summary>
		/// Initializes editor-only visual elements including the toolbar and working area.
		///</summary>

		protected virtual void SetupVisualElements()
		{
			_toolbar = new Toolbar();
			SetupToolbar(_toolbar);
			rootVisualElement.Add(_toolbar);

			rootVisualElement.style.paddingTop = _toolbar.style.height;
		}

		/// <summary>
		/// Initializes the toolbar. Called during <see cref="SetupVisualElements"/>.
		///</summary>

		protected virtual void SetupToolbar(Toolbar toolbar)
		{
			toolbar.style.paddingLeft = TOOLBAR_PADDING_LEFT;

			/** <<============================================================>> **/

			_saveButtonElement = new SaveButton(
				() => SaveChanges()
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

			/** <<============================================================>> **/

			toolbar.Add(Utils.newToolbarSeparator);

			/** <<============================================================>> **/
		}

		/// <summary>
		/// Discard any editor-only visual elements.
		///</summary>

		protected virtual void TeardownVisualElements() { }

		/** <<============================================================>> **/

		protected virtual void OnGUI()
		{
			RefreshToolbar();
		}

		protected virtual void RefreshToolbar()
		{
			RefreshSaveElements();
		}

		private void RefreshSaveElements()
		{
			var __shouldSaveOnToggleValueChange = _workObject.isAutosaved != _autosaveToggleElement.value;
			_workObject.isAutosaved = _autosaveToggleElement.value;
			_saveButtonElement.isUnlocked = !_workObject.isAutosaved;

			if (_autosaveToggleElement.value && __shouldSaveOnToggleValueChange)
				HardSave();
		}

		private void AssertCompatibleWith(EditableObject obj)
		{
			if (!obj.compatibleEditorWindows.Contains(GetType()))
				throw new NotSupportedException($"{obj.name} ({obj.GetType()}) cannot be opened with this type of EditorWindow ({GetType()}). To edit this object here, add this window type to {obj.GetType()}'s {nameof(obj.compatibleEditorWindows)}.");
		}

		#endregion
		#region Save/Load

		public override void SaveChanges()
		{
			HardSave();
		}

		/// <summary>
		/// Saves the <see cref="workObject"/>'s mirror and file. Suitable for occasional calls, i.e. when closing the window.
		///</summary>

		public void HardSave()
		{
			OnBeforeSaveWorkObject();

			_workObject.Save();

			isModified = false;
		}

		/// <summary>
		/// Saves the <see cref="workObject"/>'s mirror ONLY. Suitable for frequent calls, i.e. while autosaving.
		///</summary>

		public void SoftSave()
		{
			OnBeforeSaveWorkObject();

			/**	SaveMirror() is faster than Save() because it doesn't refresh the database.
			*	Save() must be called before closing Unity, or the mirror won't persist.
			*/
			_workObject.SaveMirror();

			isModified = false;
		}

		/// <summary>
		/// Override this method to finish preparing the <see cref="workObject"/> for saving. Called before both <see cref="HardSave"/> and <see cref="SoftSave"/>.
		///</summary>

		protected virtual void OnBeforeSaveWorkObject() { }

		/// <summary>
		/// Call this function any time a significant action is performed. If autosaving is enabled, it will perform a <see cref="SoftSave"/>. If it is disabled, it will update the header to display that.
		///</summary>

		protected void NotifyIsModified()
		{
			// Debug.Log("OnModified triggered");
			isModified = true;
		}

		private string GetSaveChangesMessage(EditableObject obj) =>
			$"{obj.name} has unsaved changes.";

		#endregion

		#endregion
	}
}
