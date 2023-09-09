
/** FrameRateTester.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril
{
	#region FrameRateTester

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class FrameRateTester : MithrilComponent
	{
		[Min(1)]
		public int targetFrameRate = 30;

		protected override void Awake()
		{
			base.Awake();

			Application.targetFrameRate = targetFrameRate;
		}
	}

	#endregion
}
