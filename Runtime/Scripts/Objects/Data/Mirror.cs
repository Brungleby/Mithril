
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
#if UNITY_INCLUDE_TESTS
		public string json => _json;
#endif
		#endregion

		#endregion
		#region Methods

		#region Construction

		public Mirror(object obj) =>
			SetReflectionFrom(obj);

		#endregion

		public object GetReflection() =>
			Json.Decode(_json);
		public object GetReflection(Type type) =>
			Json.Decode(type, _json);
		public T GetReflection<T>() =>
			Json.Decode<T>(_json);

		public void SetReflectionFrom(object obj) =>
			_json = Json.Encode(obj);

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
