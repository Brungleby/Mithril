
/** EditableObject.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;

using UnityEngine;
using UnityEditor;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// This is a special kind of ScriptableObject that may be opened and editing using a predefined EditorWindow.
	///</summary>

	public abstract class EditableObject : ScriptableObject
	{
		public abstract Editor.ReplicableEditorWindow[] UsableEditorTypes { get; }
	}
}
