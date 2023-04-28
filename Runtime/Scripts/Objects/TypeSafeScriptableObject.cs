
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

#endregion

namespace Cuberoot
{

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[Serializable]
	public abstract class TypeSafeScriptableObject : ScriptableObject, ISerializationCallbackReceiver
	{
		#region Inners

		[Serializable]
		private class ObjectMirror
		{
			[SerializeField]
			[HideInInspector]
			private SerializedField[] _serializedFields;

			// [SerializeField]
			// [HideInInspector]
			// private string _json;

			public ObjectMirror(object obj)
			{
				var __serializedFields = new List<SerializedField>();
				foreach (var iField in GetSerializableFieldInfos((obj.GetType())))
				{
					__serializedFields.Add(new SerializedField(iField, iField.GetValue(obj)));
				}
				_serializedFields = __serializedFields.ToArray();

				// _json = Serialization.Encode(obj);
			}

			public void ApplyTo(object obj)
			{
				foreach (var iSerializedField in _serializedFields)
				{
					var iField = obj.GetType().GetField(iSerializedField.name);
					iField.SetValue(obj, iSerializedField.Deserialize());
				}

				// var __obj = Serialization.Decode(_json);

				// if (__obj.GetType() != obj.GetType())
				// 	throw new InvalidCastException();

				// foreach (var iField in GetSerializableFieldInfos(obj.GetType()))
				// {
				// 	iField.SetValue(obj, iField.GetValue(__obj));
				// }
			}
		}

		[Serializable]
		private class SerializedField
		{
			[SerializeField]
			[HideInInspector]
			private string _name;
			public string name => _name;

			// [SerializeField]
			// [HideInInspector]
			// private string _type;
			// public Type type => Type.GetType(_type);

			[SerializeField]
			[HideInInspector]
			private string _json;
			public string json => _json;

			public SerializedField(FieldInfo field, object value)
			{
				if (!field.FieldType.IsAssignableFrom(value.GetType()))
					throw new InvalidCastException($"{value.GetType()} cannot be assigned to {field.FieldType}");

				_name = field.Name;
				// _type = value.GetType().AssemblyQualifiedName;

				// _json = JsonUtility.ToJson(value);
				_json = Serialization.Encode(value);
			}

			public object Deserialize()
			{
				// return JsonUtility.FromJson(_json, type);
				return Serialization.Decode(_json);
			}

			public override string ToString() => _json;
		}

		// [Serializable]
		// private struct SerializedType
		// {
		// 	[SerializeField]
		// 	private /* readonly */ string _serializedTypeName;

		// 	private SerializedType(Type type)
		// 	{
		// 		_serializedTypeName = type.ToString();
		// 	}

		// 	public static implicit operator Type(SerializedType _) =>
		// 		Type.GetType(_._serializedTypeName);

		// 	public static implicit operator SerializedType(Type _) =>
		// 		new SerializedType(_);
		// }

		// private abstract class ValueMirror : object
		// {
		// 	[SerializeField]
		// 	[HideInInspector]
		// 	private SerializedType _type;

		// 	[SerializeField]
		// 	[HideInInspector]
		// 	private object _value;
		// 	protected virtual object value
		// 	{
		// 		get => _value;
		// 		set => _value = value;
		// 	}

		// 	public ValueMirror(Type type, object value)
		// 	{
		// 		_type = (SerializedType)type;
		// 		_value = value;
		// 	}

		// 	public virtual object Deserialize()
		// 	{
		// 		throw new NotImplementedException();
		// 	}
		// }

		// [Serializable]
		// private class ObjectMirror : ValueMirror<string>
		// {
		// 	public ObjectMirror(object obj) : base(obj.GetType(), obj)
		// 	{

		// 	}
		// }

		// // [Serializable]
		// // private class ArrayMirror : ValueMirror<ObjectMirror[]>
		// // {

		// // }

		// [Serializable]
		// private class FieldMirror : object
		// {
		// 	[SerializeField]
		// 	[HideInInspector]
		// 	private string _name;

		// 	[SerializeField]
		// 	[HideInInspector]
		// 	private ObjectMirror _value;

		// 	public FieldMirror(FieldInfo field, object obj)
		// 	{
		// 		_name = field.Name;
		// 		_value = new ObjectMirror(field.GetValue(obj));
		// 	}
		// }

		#endregion

		#region Data

		[SerializeField]
		[HideInInspector]
		private ObjectMirror _mirror;

		private bool _hasLoaded = false;

		#endregion
		#region Methods

		#region ISerializationCallbackReceiver

		public void OnAfterDeserialize()
		{
			if (!_hasLoaded)
				LoadValues();
		}

		public void OnBeforeSerialize()
		{
			if (_hasLoaded)
				SaveValues();
		}

		#endregion

		private void LoadValues()
		{
			_hasLoaded = true;

			if (_mirror == null)
				return;

			_mirror.ApplyTo(this);
		}

		private void SaveValues()
		{
			_mirror = new ObjectMirror(this);
		}

		private static FieldInfo[] GetSerializableFieldInfos(Type type)
		{
			// var __ignoreFields = typeof(TypeSafeScriptableObject).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			var __ignoreField = typeof(TypeSafeScriptableObject).GetField("_mirror");

			return type
				.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy)
				.Where(i => i != __ignoreField)
				.Where(i => i.IsPublic || i.GetCustomAttribute<SerializeField>() != null)
				.ToArray()
			;
		}

		public string GetFieldJson(string name)
		{
			// return $"Not implemented";
			return (new SerializedField(GetType().GetField(name), GetType().GetField(name).GetValue(this))).ToString();
		}

		#endregion
	}
}
