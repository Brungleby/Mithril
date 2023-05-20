
/** NarrativeObject.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;

using Mithril.Tests.EditMode;

using UnityEngine;

#endregion

namespace Mithril.Tests
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[CreateAssetMenu(menuName = "Mithril/Tests/NarrativeObject")]
	public sealed class NarrativeObject : EditableObject
	{
		#region Data

		#region



		#endregion

		#endregion
		#region Methods

		public override Type[] compatibleWindows => new Type[]
			{ typeof(NarrativeEditorWindow) };

		#endregion
	}
}
