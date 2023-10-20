
/** SimpleGravityResponse.cs
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
	#region SimpleGravityResponse

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class SimpleGravityResponse : GravityResponse<Collider, Rigidbody, Vector3>
	{
		public override Vector3 force => Physics.gravity;
		public override Vector3 up => -force.normalized;

		protected override void FixedUpdate() { }

		private void OnEnable()
		{
			rigidbody.useGravity = true;
		}

		private void OnDisable()
		{
			rigidbody.useGravity = false;
		}
	}

	#endregion
}
