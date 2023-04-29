
/** Json.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Reflection;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#endregion

namespace Mithril
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public static class Json : object
	{
		#region Data

		#region



		#endregion

		#endregion
		#region Methods

		#region

		public static string ConvertTo(object obj)
		{
			throw new NotImplementedException();
		}

		public static object ConvertFrom(string data)
		{
			throw new NotImplementedException();
		}

		public static T ConvertFrom<T>(string data)
		{
			return (T)ConvertFrom(data);
		}

		#endregion

		#endregion
	}
}
