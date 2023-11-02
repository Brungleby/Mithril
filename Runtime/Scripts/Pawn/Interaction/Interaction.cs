
/** Interaction.cs
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
	#region Interaction

	/// <summary>
	/// The resulting data spawned from an <see cref="Interactor"/> interacting with an <see cref="Interactable"/>.
	///</summary>

	public class Interaction : object
	{
		public static readonly Interaction success = new Interaction(true, true);
		public static readonly Interaction failure = new Interaction(false, true);
		public static readonly Interaction silent = new Interaction(true, false);
		public static readonly Interaction ignore = new Interaction(false, false);

		public readonly bool isSuccess;
		public readonly bool doFeedback;

		public Interaction(bool isSuccess, bool doFeedback)
		{
			this.isSuccess = isSuccess;
			this.doFeedback = doFeedback;
		}
		public Interaction(bool isSuccess)
		{
			this.isSuccess = isSuccess;
			doFeedback = true;
		}

		public static implicit operator bool(Interaction _) => _.isSuccess;
	}

	#endregion
}
