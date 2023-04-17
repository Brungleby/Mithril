
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

		#region IEnumerable.Contains

		public static bool Contains(this IEnumerable enumerable, object query)
		{
			foreach (var i in enumerable)
			{
				if (i.Equals(query))
					return true;
			}

			return false;
		}

		#endregion
		#region IEnumerable.AllToString

		public static string AllToString<T>(this ICollection<T> collection)
		{
			int __figures = (collection.Count / 10) + 1;

			string __result = $"{collection.ToString()}\n";
			int __index = 0;
			foreach (var i in collection)
			{
				string __indexString = __index.ToString().PadLeft(__figures, '0');

				__result += $"[{__indexString}] {i.ToString()}\n";

				__index++;
			}

			return __result.Substring(0, __result.Length - 1);
		}

		public static string AllToString<T>(this IEnumerable<T> enumerable)
		{
			var __list = new List<T>();
			__list.AddRange(enumerable);

			return __list.AllToString();
		}

		#endregion

		#region 

		public static T[] Combine<T>(this T[] a, T[] b)
		{
			var c = new T[a.Length + b.Length];

			int i = 0;
			foreach (var ia in a)
			{
				c[i] = ia;
				i++;
			}

			foreach (var ib in b)
			{
				c[i] = ib;
				i++;
			}

			return c;
		}

		#endregion

		#region ICollection.AddAll

		public static void AddAll<T>(this ICollection<T> collection, ICollection<T> toAdd)
		{
			foreach (var i in toAdd)
				collection.Add(i);
		}

		#endregion
		#region ICollection.RemoveAll

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
