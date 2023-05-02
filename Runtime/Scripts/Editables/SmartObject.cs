
/** SaveableObject.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;

using UnityEditor;

#endregion

namespace Mithril
{
	#region SmartObject

	/// <summary>
	/// A more feature-rich implementation of <see cref="ScriptableObject"/> which serializes its contents properly, and is suitable for storing polymorphic data.
	///</summary>

	[Serializable]
	public abstract class SmartObject : ScriptableObject, ISerializationCallbackReceiver
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
						GUIUtility.systemCopyBuffer = __target.GetJsonString(true);
				}
				else
				{
					if (GUILayout.Button("Print JSON"))
						Debug.Log(__target.GetJsonString());
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

		#region ObjectMirror

		[Serializable]
		private class ObjectMirror
		{
			[SerializeField]
			[HideInInspector]
			private SerializedField[] _serializedFields;

			public string json =>
				_serializedFields.ContentsToString();

			public ObjectMirror(object obj)
			{
				var __serializedFields = new List<SerializedField>();
				foreach (var iField in obj.GetType().GetSerializableFields())
				{
					__serializedFields.Add(new SerializedField(iField, iField.GetValue(obj)));
				}
				_serializedFields = __serializedFields.ToArray();
			}

			public void ApplyTo(object obj)
			{
				foreach (var iSerializedField in _serializedFields)
				{
					FieldInfo iField;
					try
					{
						iField = obj.GetType().GetSerializableField(iSerializedField.name);
					}
					catch
					{
						var __oldName = obj.GetType().GetCustomAttribute<FormerlySerializedAsAttribute>().oldName;
						iField = obj.GetType().GetSerializableField(__oldName);
					}

					iField.SetValue(obj, iSerializedField.Deserialize());
				}
			}
		}

		#endregion
		#region SerializedField

		[Serializable]
		private class SerializedField
		{
			[SerializeField]
			[HideInInspector]
			private string _name;
			public string name => _name;

			private string _expectedType;
			private Type expectedType
			{
				get => Type.GetType(_expectedType);
				set => _expectedType = value.ToString();
			}

			[SerializeField]
			[HideInInspector]
			private string _json;
			public string json => _json;

			public SerializedField(FieldInfo field, object value)
			{
				if (!field.FieldType.IsAssignableFrom(value.GetType()))
					throw new InvalidCastException($"{value.GetType()} cannot be assigned to {field.FieldType}");

				_name = field.Name;
				_json = Serialization.Encode(value);
			}

			public object Deserialize()
			{
				return Serialization.Decode(_json);
			}

			public override string ToString() => _json;
		}

		#endregion

		#endregion

		#region Data

		/// <summary>
		/// Mirror of this object; stores json information for each field (not including this one).
		///</summary>

		[SerializeField]
		[HideInInspector]
		[NonSerializedBySmartObjectAttribute]

		private ObjectMirror _mirror;

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
				LoadMirror();

			_isBeingModifiedInInspector = false;
		}

		public virtual void OnBeforeSerialize()
		{
			if (!_isBeingModifiedInInspector)
				SaveMirror();
		}

		#endregion

		protected void LoadMirror()
		{
			if (_mirror == null)
				return;

			_mirror.ApplyTo(this);
		}

		protected void SaveMirror()
		{
			_mirror = new ObjectMirror(this);
		}

		/// <returns>
		/// The JSON serialization of this object.
		///</returns>

		public string GetJsonString(bool prettyPrint = false)
		{
			return _mirror.json;
		}

		#endregion
	}

	#endregion

	#region NonSerializedInSmartObject

	[AttributeUsage(AttributeTargets.Field)]
	public sealed class NonSerializedBySmartObjectAttribute : Attribute { }

	#endregion
}
