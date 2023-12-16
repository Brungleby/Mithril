
/** ConveyorComponent.cs
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
	#region ConveyorComponent<>

	/// <summary>
	/// This component causes physics bodies to move along this collider's surface like a conveyor belt.
	///</summary>

	// [DefaultExecutionOrder(1000)]

	public abstract class ConveyorComponent<TCollider, TRigidbody, TVector> : MovementComponent<TCollider, TRigidbody, TVector>
	{
		public abstract TVector velocity { get; }
	}

	#endregion
	#region ConveyorComponent

	[RequireComponent(typeof(Rigidbody))]

	public sealed class ConveyorComponent : ConveyorComponent<Collider, Rigidbody, Vector3>
	{
		#region Fields

		[SerializeField]

		private Vector3 _velocity = Vector3.right;
		public override Vector3 velocity { get => _velocity; }

		#endregion

		protected override void FixedUpdate()
		{
			var __motionVector = transform.rotation * velocity * Time.fixedDeltaTime;

			rigidbody.position -= __motionVector;
			rigidbody.MovePosition(rigidbody.position + __motionVector);
		}

		public void SetVelocity(in Vector3 value) => _velocity = value;

		private void OnDrawGizmos()
		{
			DebugDraw.DrawArrow(transform.position, (transform.rotation * velocity).normalized, Color.green);
		}
	}

	#endregion
}
