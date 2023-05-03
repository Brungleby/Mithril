
/** Mirror.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

#endregion

namespace Mithril
{
	#region IMirrorable

	public interface IMirrorable
	{
		Mirror mirror { get; set; }
	}

	public static class IMirrorableExtensions
	{
		public static void Save(this IMirrorable realObj) =>
			realObj.mirror = new Mirror(realObj);

		public static void Load(this IMirrorable realObj) =>
			Mirror.Load(realObj.mirror, realObj);
	}

	#endregion
	#region ObjectMirror

	[Serializable]
	public sealed class Mirror
	{
		#region Inners

		#region MirrorField

		[Serializable]
		private sealed class Field
		{
			[SerializeField]
			private string _name;
			public string name => _name;

			// [SerializeField]
			// private string _type;
			// private Type type
			// {
			// 	get => Type.GetType(_type);
			// 	set => _type = value.AssemblyQualifiedName;
			// }

			[SerializeField]
			private string _json;
			public string json => _json;

			public object value =>
				Serialization.Decode(_json);
			public T GetValueAs<T>() =>
				(T)value;

			public Field(FieldInfo field, object valueHolder)
			{
				_name = field.Name;

				// var __type = __value.GetType();
				// type = __value.GetType();

				var __value = field.GetValue(valueHolder);
				_json = Serialization.Encode(__value);
			}

			public override string ToString() => _json;
		}

		#endregion

		#endregion
		#region Data

		[SerializeField]
		[HideInInspector]
		private Field[] _mirrorFields;

		#endregion

		public Mirror(object realObject)
		{
			var __fields = new List<Field>();
			foreach (var iField in realObject.GetType().GetSerializableFields())
				__fields.Add(new Field(iField, realObject));

			_mirrorFields = __fields.ToArray();
		}

		private void ApplyTo(object obj)
		{
			if (_mirrorFields == null)
				return;

			foreach (var iMirrorField in _mirrorFields)
			{
				FieldInfo iField;
				iField = obj.GetType().GetSerializableField(iMirrorField.name);

				iField.SetValue(obj, iMirrorField.value);
			}
		}

		public static void Save(out Mirror mirror, object realObject)
		{
			mirror = new Mirror(realObject);
		}
		public static void Load(Mirror mirror, object realObject)
		{
			if (mirror == null)
				return;

			mirror.ApplyTo(realObject);
		}

		public object GetFieldValue(string name)
		{
			foreach (var i in _mirrorFields)
				if (i.name == name)
					return i.value;

			return null;
		}
		public T GetFieldValue<T>(string name) =>
			(T)GetFieldValue(name);

		public override string ToString() =>
			_mirrorFields.ContentsToString();
	}

	#endregion
}
