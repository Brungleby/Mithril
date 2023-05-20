
/** Utils.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

#endregion

namespace Mithril.Editor
{
	#region Utils

	public static class Utils
	{
		public static T FindRootElement<T>(this VisualElement element)
		where T : VisualElement
		{
			var __parent = element;

			while (__parent != null && !(__parent is T))
				__parent = __parent.parent;

			return (T)__parent;
		}
	}

	#endregion
}
