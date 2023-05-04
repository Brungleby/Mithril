
/** NewMirror.cs
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
	/// Stores a string representation of an object.
	///</summary>

	public sealed class NewMirror : object
	{
		#region Data

		#region

		private string _jsonValue;
		#endregion

		#endregion
		#region Methods

#if UNITY_INCLUDE_TESTS
		public string jsonValue => _jsonValue;
#endif
		// public object value => NewSerialize.Decode(_jsonValue);



		#region

		public NewMirror(object realObj)
		{
			_jsonValue = NewSerialize.Encode(realObj);
		}

		#endregion

		#endregion
	}
}
