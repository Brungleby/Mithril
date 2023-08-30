
/** MithrilMonoEditor.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Reflection;

using UnityEditor;
using UnityEngine;

#endregion

namespace Mithril
{
	#region MithrilMonoEditor

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[CustomEditor(typeof(MonoBehaviour), true)]

	public sealed class MithrilMonoEditor : Editor
	{
		private const string LABEL_NAME = "Toggle Gizmos";
		private const string METHOD_NAME0 = "OnDrawGizmos";
		private const string METHOD_NAME1 = "OnDrawGizmosSelected";
		private const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		// private bool _isDrawGizmosToggled = true;

		private bool implementsGizmos
		{
			get
			{
				var __methodInfos = target.GetType().GetMethods(BINDING_FLAGS);

				foreach (var __methodInfo in __methodInfos)
					if (__methodInfo.Name == METHOD_NAME0 || __methodInfo.Name == METHOD_NAME1)
						return true;

				return false;
			}
		}

		public override void OnInspectorGUI()
		{
			// if (implementsGizmos)
			// {
			// _isDrawGizmosToggled = GUILayout.Toggle(_isDrawGizmosToggled, LABEL_NAME);
			// }

			base.OnInspectorGUI();
		}
	}

	#endregion
}
