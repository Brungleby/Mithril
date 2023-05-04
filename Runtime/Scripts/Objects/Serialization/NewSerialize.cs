
/** NewSerialize.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class NewSerialize : object
	{
		#region Data

		#region



		#endregion

		#endregion
		#region Methods

		#region

		public static string Encode(object obj)
		{
			if (typeof(string) == obj.GetType())
				return EncodeString((string)obj);

			return EncodePrimitive(obj);
		}

		private static string EncodePrimitive(object obj)
		{
			return obj.ToString().ToLower();
		}

		private static string EncodeString(string obj)
		{
			return $"\"{obj}\"";
		}

		#endregion

		#endregion
	}
}
