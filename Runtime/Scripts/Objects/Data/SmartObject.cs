
/** SaveableObject.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;

using UnityEngine;

using UnityEditor;

#endregion

namespace Mithril
{
	#region SmartObject

	/// <summary>
	/// A more feature-rich implementation of <see cref="ScriptableObject"/> which serializes its contents properly, and is suitable for storing polymorphic data.
	///</summary>

	[Serializable]
	public abstract class SmartObject : ScriptableObject, IMirrorable, ISerializationCallbackReceiver
	{
		#region Inners

		#region Editor

		/// <summary>
		/// A special editor designed for use with <see cref="SmartObject"/>s (and any child class).
		///</summary>

		[CustomEditor(typeof(SmartObject), true)]
		public class SmartObjectEditor : UnityEditor.Editor
		{
			/// <summary>
			/// This implementation of <see cref="UnityEditor.Editor.OnInspectorGUI"/> is unique. Functional GUI elements should be implemented here, but fields that can be modified should be implemented in <see cref="OnInspectorGUI_Fields"/>.
			///</summary>

			public override void OnInspectorGUI()
			{
				var __target = (SmartObject)target;

				var currentEvent = Event.current;
				if (currentEvent.alt)
				{
					if (GUILayout.Button("Copy JSON"))
						GUIUtility.systemCopyBuffer = __target.mirror.ToString();
				}
				else
				{
					if (GUILayout.Button("Print JSON"))
						Debug.Log(__target.mirror.ToString());
				}

				EditorGUI.BeginChangeCheck();

				OnInspectorGUI_Fields();

				if (EditorGUI.EndChangeCheck())
					__target._isBeingModifiedInInspector = true;
			}

			/// <summary>
			/// Custom implementation of <see cref="OnInspectorGUI"/>. <see cref="SmartObject.OnAfterDeserialize"/> is postponed until after this method has completed to allow values to be edited properly.
			///</summary>

			public virtual void OnInspectorGUI_Fields()
			{
				base.OnInspectorGUI();
			}
		}

		#endregion

		#endregion

		#region Data

		/// <summary>
		/// Mirror of this object; stores json information for each field (not including this one).
		///</summary>

		[SerializeField]
		[HideInInspector]
		private Mirror _mirror;
		public Mirror mirror { get => _mirror; set => _mirror = value; }

		/// <summary>
		/// Updated by <see cref="SmartObject.SmartObjectEditor"/>; indicates whether or not it is being directly edited in the inspector.
		///</summary>

		private bool _isBeingModifiedInInspector = false;

		#endregion
		#region Methods

		#region ISerializationCallbackReceiver

		public virtual void OnAfterDeserialize()
		{
			if (!_isBeingModifiedInInspector)
				this.Load();

			_isBeingModifiedInInspector = false;
		}

		public virtual void OnBeforeSerialize()
		{
			if (!_isBeingModifiedInInspector)
				this.Save();
		}

		#endregion

		#endregion
	}

	#endregion
}
