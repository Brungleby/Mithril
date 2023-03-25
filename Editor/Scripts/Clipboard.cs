
/** SerializeClipboard.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

#endregion

namespace Cuberoot.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public static class Clipboard
	{
		#region Exception

		public class UnserializableException : System.Exception
		{
			public UnserializableException() { }
			public UnserializableException(object obj) : base($"{obj} is not safely serializable. Please consider implementing the Cuberoot.ISerializable interface for this object.") { }
		}

		#endregion
		#region Methods

		#region

		public static void Copy(string text)
		{
			GUIUtility.systemCopyBuffer = text;
		}
		public static void Copy(ISerializable obj)
		{
			var __data = obj.Serialize();

			AssertValidJson(obj, __data);

			Copy(__data);
		}
		public static void Copy(object obj)
		{
			var __data = JsonUtility.ToJson(obj, true);

			AssertValidJson(obj, __data);

			Copy(__data);
		}

		public static object Paste() =>
			JsonUtility.FromJson<object>(GUIUtility.systemCopyBuffer);
		public static T Paste<T>() =>
			JsonUtility.FromJson<T>(GUIUtility.systemCopyBuffer);
		public static string PasteText() =>
			GUIUtility.systemCopyBuffer;
		public static T PasteText<T>(string data) =>
			JsonUtility.FromJson<T>(data);

		public static bool IsValidJson(string text) =>
			text != "{}";
		public static void AssertValidJson(string text)
		{
			if (IsValidJson(text))
				return;
			throw new UnserializableException();

		}
		public static void AssertValidJson(object query, string text)
		{
			try
			{
				AssertValidJson(text);
			}
			catch
			{
				throw new UnserializableException(query);
			}
		}

		#endregion

		#endregion
	}
}
