
/** SmartWindow.cs
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

	public abstract class ForgeWindow : EditorWindow
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

			public SaveButton(Action action) : base(action) { }

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

		public readonly static string DEFAULT_ICON_PATH =
			"Assets/Mithril/Mithril.Core/Editor/Resources/Textures/Icon_Diamond.png";

		#region

		private ForgeObject _workObject;
		public ForgeObject workObject => _workObject;

		private bool _isModified;
		public bool isModified
		{
			get =>
				_isModified && !_workObject.isAutosaved;
			private set
			{
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

		public static ForgeWindow Instantiate(Type type, ForgeObject obj)
		{
			/** <<============================================================>> **/
			/**	If a window already editing this specific object exists,
			*	then we'll simply use that.
			*/

			var __allWindowsOfMatchingType = Resources
				.FindObjectsOfTypeAll(type)
				.Cast<ForgeWindow>()
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

			var __window = (ForgeWindow)EditorWindow.CreateInstance(type);
			__window.Open(obj);

			return __window;
		}

		/// <inheritdoc cref="Instantiate"/>

		public static T Instantiate<T>(ForgeObject obj)
		where T : ForgeWindow =>
			(T)Instantiate(typeof(T), obj);

		/// <summary>
		/// Loads the given <paramref name="obj"/> into the view(s) of this <see cref="ForgeWindow"/>.
		///</summary>

		public void Open(ForgeObject obj)
		{
			AssertObjectWindowCompatible(obj);

			_workObject = obj;

			Utils.InitializeWindowHeader(this, obj.fileName, iconPath);
			isModified = false;

			OnGUI();
		}

		/// <summary>
		/// Loads the given <paramref name="filePath"/> as a <see cref="ForgeObject"/> into the view(s) of this <see cref="ForgeWindow"/>.
		///</summary>

		public void Open(string filePath)
		{
			var __loadedObject = AssetDatabase.LoadAssetAtPath<ForgeObject>(filePath);
			Open(__loadedObject);
		}

		#endregion
		#region Setup

		protected virtual void CreateVisualElements()
		{
			_toolbar = new Toolbar();
			InitializeToolbar(_toolbar);
			rootVisualElement.Add(_toolbar);
		}

		protected virtual void InitializeToolbar(Toolbar toolbar)
		{
			/** <<============================================================>> **/

			_autosaveToggleElement = new Toggle();

			_autosaveToggleElement.style.flexDirection = FlexDirection.RowReverse;

			toolbar.Add(_autosaveToggleElement);

			/** <<============================================================>> **/

			_saveButtonElement = new SaveButton(
				() => Save()
			)
			{
				text = "Save Asset"
			};

			toolbar.Add(_saveButtonElement);
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
			_saveButtonElement.isSaving = _workObject.isSaving;

		}

		private void RefreshTitleText()
		{
			titleContent.text = _workObject.fileName + (isModified ? "*" : "");
		}

		private void AssertObjectWindowCompatible(ForgeObject obj)
		{
			if (!obj.usableEditorWindows.Contains(GetType()))
				throw new NotSupportedException($"{obj.name} ({obj.GetType()}) cannot be opened with this type of EditorWindow ({GetType()}).");
		}

		#endregion
		#region Save/Load

		public void Save()
		{
			_workObject.Save();

			isModified = false;
		}

		#endregion

		#endregion
	}
}
