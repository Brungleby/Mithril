
/** NewMirror.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;

using UnityEngine;

#endregion

namespace Mithril
{
	#region Mirror

	/// <summary>
	/// Stores a string representation of an object.
	///</summary>

	[Serializable]
	public sealed class Mirror : object
	{
		#region Data

		#region

		[SerializeField]
		[HideInInspector]
		private string _json;
		public string json => _json;

		#endregion

		#endregion
		#region Methods

		#region Construction

		private Mirror(string json) =>
			_json = json;
		public Mirror(object obj) =>
			SetReflectionFrom(obj);

		public static Mirror CreateFromJsonDirect(string json) =>
			new Mirror(json);

		#endregion

		#region Fundamentals

		/// <returns>
		/// True if this Mirror's DECODED OBJECT equals the query Mirror's (or string's) DECODED OBJECT.
		///</returns>

		public bool HardEquals(object obj)
		{
			if (obj != null)
			{
				if (obj is Mirror that_Mirror)
					return GetReflection().Equals(that_Mirror.GetReflection());
				if (obj is string that_String)
					return GetReflection().Equals(JsonTranslator.Decode(that_String));
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj != null)
			{
				if (obj is Mirror that_Mirror)
					return this._json.Equals(that_Mirror._json);
				if (obj is string that_String)
					return this._json.Equals(that_String);
			}
			return false;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 17;
				hash = hash * 31 + _json.GetHashCode();
				return hash;
			}
		}

		#endregion

		public object GetReflection() =>
			JsonTranslator.Decode(_json);
		public object GetReflection(Type type) =>
			JsonTranslator.Decode(type, _json);
		public T GetReflection<T>() =>
			JsonTranslator.Decode<T>(_json);

		public void SetReflectionFrom(object obj) =>
			_json = JsonTranslator.Encode(obj);

		public void ApplyReflectionTo(object obj) =>
			CopySerializableFieldValues(GetReflection(), obj);

		public static void CopySerializableFieldValues(object source, object target)
		{
			foreach (var iField in source.GetType().GetSerializableFields())
			{
				var __sourceValue = iField.GetValue(source);
				iField.SetValue(target, __sourceValue);
			}
		}

		#endregion
	}

	#endregion
	#region NonMirroredAttribute

	[AttributeUsage(AttributeTargets.Field)]
	public class NonMirroredAttribute : Attribute { }

	#endregion
}
