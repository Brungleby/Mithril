
/** Utils.cs
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

#endregion

namespace Cuberoot
{
	public static class Utils
	{
		#region Object

		public static T Safe<T>(this T obj) where T : new() =>
			obj ?? new T();

		#region AddAll

		public static void AddAll<T>(this ICollection<T> collection, ICollection<T> toAdd)
		{
			foreach (var i in toAdd)
				collection.Add(i);
		}

		#endregion
		#region RemoveAll

		public static bool[] RemoveAll<T>(this ICollection<T> collection, ICollection<T> toRemove)
		{
			var __passed = new List<bool>();

			foreach (var i in toRemove)
				__passed.Add(collection.Remove(i));

			return __passed.ToArray();
		}

		#endregion

		#region GetChildren

		public static Transform[] GetChildren(this Transform t)
		{
			var __result = new List<Transform>();

			for (var i = 0; i < t.childCount; i++)
				__result.Add(t.GetChild(i));

			return __result.ToArray();
		}

		#endregion

		#endregion

		#region LayerMask

		#region Contains

		/// <returns>
		/// TRUE if the given <paramref name="mask"/> contains the specified <paramref name="layer"/>.
		///</returns>

		public static bool Contains(in this LayerMask mask, in int layer) =>
			((mask.value & (1 << layer)) > 0);

		/// <returns>
		/// TRUE if the given <paramref name="mask"/> contains the specified <paramref name="gameObject"/>'s layer.
		///</returns>

		public static bool Contains(in this LayerMask mask, in GameObject gameObject) =>
			((mask.value & (1 << gameObject.layer)) > 0);

		/// <returns>
		/// TRUE if the given <paramref name="mask"/> contains the specified <paramref name="component"/>'s layer.
		///</returns>

		public static bool Contains(in this LayerMask mask, in Component component) =>
			((mask.value & (1 << component.gameObject.layer)) > 0);

		#endregion
	}

	#endregion
}
