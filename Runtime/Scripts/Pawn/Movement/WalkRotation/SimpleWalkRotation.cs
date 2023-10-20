
/** SimpleWalkRotation.cs
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
	#region SimpleWalkRotation

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class SimpleWalkRotation : WalkRotation
	{
		#region Data

		public float maxAngularVelocity = 10f;
		public float speed = 200f;

		#endregion
		#region Properties



		#endregion
		#region Methods

		protected override void Awake()
		{
			base.Awake();

			rigidbody.maxAngularVelocity = maxAngularVelocity;
		}

		protected override void FixedUpdate()
		{
			var rightDotForward = Vector3.Dot(transform.right, walkMovement.inputVector);
			var forwardDotForward = Vector3.Dot(transform.forward, walkMovement.inputVector);

			var forwardPowered = (1f - forwardDotForward.Max()).Pow(0.5f);
			if (float.IsNaN(forwardPowered)) return;

			var torque = Vector3.up * rightDotForward.Sign() * forwardPowered * speed;
			rigidbody.AddRelativeTorque(torque, ForceMode.Acceleration);
		}

		#endregion
	}

	#endregion
}
