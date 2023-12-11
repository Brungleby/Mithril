
/** WalkRotation.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril.Pawn
{
	#region WalkRotation

	/// <summary>
	/// Generic class to define how a 3D walking character rotates.
	///</summary>

	public abstract class WalkRotation : MovementComponent<Pawn, Collider, Rigidbody, Vector3>
	{
		#region Members

		[AutoAssign]
		public WalkMovement walkMovement { get; protected set; }

		#endregion
		#region Properties

		public virtual float walkMovementSpeedModifier => 1f;

		#endregion
		#region Methods

		#endregion
	}

	#endregion
}
