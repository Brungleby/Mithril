
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
		#region Data

		#region



		#endregion

		#endregion
		#region Methods

		#region

		public static void Copy(ISerializable obj) =>
			GUIUtility.systemCopyBuffer = obj.Serialize();
		public static void Copy(object obj)
		{
			var __data = JsonUtility.ToJson(obj);

			if (JsonUtility.FromJson<object>(__data) == null)
				throw new Exception($"{obj} is not safely serializable. Please consider implementing the Cuberoot.ISerializable interface for this object.");

			GUIUtility.systemCopyBuffer = __data;
		}

		public static object Paste(string data) =>
			JsonUtility.FromJson<object>(data);
		public static T Paste<T>(string data) =>
			JsonUtility.FromJson<T>(data);

		#endregion

		#endregion
	}
}
