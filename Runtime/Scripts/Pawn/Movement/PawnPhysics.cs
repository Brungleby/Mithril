
/** PawnPhysics.cs
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
	#region PawnPhysicsBase

	/// <summary>
	/// Compensates for changes in motion on the ground beneath the pawn.
	///</summary>

	public abstract class PawnPhysicsBase<TPawn, TGround, TCollider, TRigidbody, TVector> : MovementComponent<TPawn, TCollider, TRigidbody, TVector>, ILateFixedUpdaterComponent
	where TPawn : PawnBase<TCollider, TRigidbody, TVector>
	where TVector : unmanaged
	{
		#region Members

		[AutoAssign]
		public TGround ground { get; protected set; }

		protected TransformData _earlyGroundTransform;
		protected TRigidbody _earlyGroundRigidbody;
		protected TRigidbody _lastFrameRigidbody;

		private LateFixedUpdater _lateFixedUpdater;

		#endregion
		#region Properties

		public TVector lastValidGroundVelocity { get; protected set; }

		#endregion
		#region Methods

		public abstract void LateFixedUpdate();

		protected override void Awake()
		{
			base.Awake();

			_lateFixedUpdater = new(this);
		}

		protected virtual void OnEnable()
		{
			_lateFixedUpdater.SetupCoroutine();
		}

		#endregion
	}

	#endregion
	#region PawnPhysics

	public sealed class PawnPhysics : PawnPhysicsBase<Pawn, GroundSensor, Collider, Rigidbody, Vector3>
	{
		#region Fields

		/// <summary>
		/// If enabled, the rigidbody will rotate along with the ground beneath it. Otherwise, it will maintain its rotation.
		///</summary>
		[Tooltip("If enabled, the rigidbody will rotate along with the ground beneath it. Otherwise, it will maintain its rotation.")]

		public bool enableRotateWithGround = true;

		#endregion
		#region Methods

		protected override void FixedUpdate()
		{
			_earlyGroundRigidbody = ground.hitRigidbody;

			if (_earlyGroundRigidbody != null)
				lastValidGroundVelocity = _earlyGroundRigidbody.GetPointVelocity(rigidbody.position);

			/** <<============================================================>> **/
			/**	Add and remove velocity when touching and leaving moving platforms.
			*/

			if (_earlyGroundRigidbody != null && _lastFrameRigidbody == null)
				rigidbody.velocity -= lastValidGroundVelocity;
			else if (_earlyGroundRigidbody == null && _lastFrameRigidbody != null)
				rigidbody.velocity += lastValidGroundVelocity;

			/** <<============================================================>> **/

			_lastFrameRigidbody = _earlyGroundRigidbody;
			if (_earlyGroundRigidbody != null)
				_earlyGroundTransform = new TransformData(_earlyGroundRigidbody.position, _earlyGroundRigidbody.rotation, _earlyGroundRigidbody.transform.lossyScale);
		}

		public override void LateFixedUpdate()
		{
			var lateGroundRigidbody = ground.hitRigidbody;

			if (lateGroundRigidbody == null || lateGroundRigidbody != _earlyGroundRigidbody) return;

			var deltaRotation = lateGroundRigidbody.rotation * Quaternion.Inverse(_earlyGroundTransform.rotation);
			deltaRotation = deltaRotation.ProjectRotationOnAxis(pawn.up);

			var pivotPosition = deltaRotation * (rigidbody.position - lateGroundRigidbody.position) + lateGroundRigidbody.position;

			var deltaPosition = lateGroundRigidbody.position - _earlyGroundTransform.position;
			var newPosition = pivotPosition + deltaPosition;

			rigidbody.MovePosition(newPosition);

			/** <<============================================================>> **/

			if (enableRotateWithGround)
				rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
		}

		#endregion
	}

	#endregion
}
