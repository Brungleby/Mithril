
/** Interactable.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Mithril
{
	#region Interactable

	/// <summary>
	/// Defines what happens when an <see cref="Interactor"/> interacts with this.
	///</summary>

	public class Interactable : MithrilComponent
	{
		[SerializeField]
		public UnityEvent onInteract;

		internal Interaction _OnInteract(Interactor user)
		{
			var result = OnInteract(user);
			if (result) onInteract.Invoke();
			return result;
		}

		/// <summary>
		/// Triggers an interaction between this and user.
		///</summary>
		/// <returns>
		/// True if the interaction was successful. False if it failed for some restrictive reason.
		///</returns>

		protected virtual Interaction OnInteract(Interactor user) => new(true);
	}

	#endregion
}
