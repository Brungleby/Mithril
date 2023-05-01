
/** TestForgeObject.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

using Mithril;

#endregion

namespace Mithril
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[CreateAssetMenu(menuName = "Test Forge Object")]
	public sealed class TestForgeObject : ForgeObject
	{
		#region Data

		#region



		#endregion

		#endregion
		#region Methods

		#region

		public TestForgeObject()
		{

		}

		public override System.Type[] usableEditorWindows =>
			new System.Type[] {
				typeof(TestForgeWindow)
			};

		#endregion

		#endregion
	}
}
