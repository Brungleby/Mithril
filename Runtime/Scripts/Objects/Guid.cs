
/** Guid.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// A simplified and serializable GUID represented as a string.
	///</summary>
	[System.Serializable]

	public struct Guid : ISerializable
	{
		#region Data

		#region

		[SerializeField]
		private string _guid;

		#endregion

		#endregion
		#region Methods

		#region Constructors

		// public Guid()
		// {
		// 	this._guid = GUID.Generate().ToString();
		// }

		public static Guid GenerateNew() =>
			new Guid(GUID.Generate());

		public Guid(GUID guid)
		{
			this._guid = guid.ToString();
		}

		public Guid(string guid)
		{
			this._guid = guid;
		}

		#endregion
		#region Operators

		public static implicit operator Guid(string _) =>
			new Guid(_);
		public static implicit operator Guid(GUID _) =>
			new Guid(_);

		public static implicit operator string(Guid _) =>
			_._guid;
		public static implicit operator GUID(Guid _)
		{
			GUID __result;
			GUID.TryParse(_, out __result);
			return __result;
		}

		public static bool operator ==(Guid a, Guid b) =>
			a.Equals(b);
		public static bool operator !=(Guid a, Guid b) =>
			!a.Equals(b);

		#endregion

		public string Serialize() =>
			JsonUtility.ToJson(_guid);

		public override bool Equals(object obj) =>
			_guid.Equals(((Guid)obj)._guid);

		public override int GetHashCode() =>
			_guid.GetHashCode();

		public override string ToString() =>
			_guid.ToString();

		#endregion
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(Guid))]
	public class GuidPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			/** <<============================================================>> **/

			position = EditorGUI.PrefixLabel(position, label);

			/** <<============================================================>> **/

			var __indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			GUI.enabled = false;

			var __text = property.FindPropertyRelative("_guid").stringValue;
			EditorGUI.TextField(position, __text);

			GUI.enabled = true;
			EditorGUI.indentLevel = __indent;

			/** <<============================================================>> **/

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUIUtility.singleLineHeight;
	}
#endif
}
