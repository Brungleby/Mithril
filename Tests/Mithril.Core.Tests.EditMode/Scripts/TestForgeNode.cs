
/** TestForgeNode.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

using Mithril;
using Mithril.Editor;

#endregion

namespace Mithril.Tests
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class TestForgeNode : Node
	{
		#region Data

		#region

		public string message = "something";

		public override string defaultName =>
			"Test Forge Node";

		#endregion

		#endregion
		#region Methods

		#region

		#endregion

		#endregion
	}

	public sealed class TestNodeGraphView : CustomNodeGraphView
	{
		public TestNodeGraphView() : base()
		{
			CreateNewNode<TestForgeNode>();
		}
	}
}
