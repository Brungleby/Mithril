
/** AnimationScript.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril.Animation
{
	#region AnimationScript

	/// <summary>
	/// Base class for a component used to control an animator.
	///</summary>

	public abstract class AnimationScript : MithrilComponent
	{
		[AutoAssign]
		public Animator animator { get; protected set; }
	}

	#endregion
}
