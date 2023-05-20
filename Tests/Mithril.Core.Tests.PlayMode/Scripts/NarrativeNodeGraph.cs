
/** NarrativeNodeGraph.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril.Tests
{
	#region NarrativeNodeGraph

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[CreateAssetMenu(menuName = "Mithril/Tests/Narrative Node Graph")]
	public sealed class NarrativeNodeGraph : NodeGraphData
	{
		#region Data



		#endregion
		#region Methods

		public override System.Type[] compatibleWindows => new System.Type[]
			{ typeof(EditMode.NarrativeNodeGraphWindow) };

		#endregion
	}

	#endregion
}
