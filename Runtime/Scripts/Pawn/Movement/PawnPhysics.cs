
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
		protected TVector _lastFramePosition;
		protected TVector _lastGroundVelocity;

		private LateFixedUpdater _lateFixedUpdater;

		#endregion
		#region Properties

		public TVector groundVelocity { get; private set; }

		#endregion
		#region Methods

		public abstract void LateFixedUpdate();
		protected abstract TVector CalculateGroundVelocity();

		protected override void Awake()
		{
			base.Awake();

			_lateFixedUpdater = new LateFixedUpdater(this);
		}

		protected virtual void OnEnable()
		{
			_lateFixedUpdater.SetupCoroutine();
		}

		protected override void FixedUpdate()
		{
			groundVelocity = CalculateGroundVelocity();
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
			try
			{
				var groundRigidbody = ground.hitRigidbody;

				/** <<============================================================>> **/
				/**	Account for changes in velocity of the ground
				*	This method factors in both linear and angular velocity.
				*/

				if (groundRigidbody != null && _lastFrameRigidbody == null)
					rigidbody.velocity -= groundRigidbody.GetPointVelocity(rigidbody.position);

				else if (groundRigidbody == null && _lastFrameRigidbody != null)
				{
					_lastGroundVelocity = _lastFrameRigidbody.GetPointVelocity(rigidbody.position);
					rigidbody.velocity += _lastGroundVelocity;
				}

				/** <<============================================================>> **/

				if (groundRigidbody != null)
				{
					_earlyGroundRigidbody = groundRigidbody;
					_earlyGroundTransform = new TransformData(groundRigidbody.position, groundRigidbody.rotation, groundRigidbody.transform.lossyScale);
					_lastFrameRigidbody = groundRigidbody;
				}
				else
					_lastFrameRigidbody = null;
			}
			catch
			{
				_lastFrameRigidbody = null;
				_earlyGroundRigidbody = null;
			}

			_lastFramePosition = rigidbody.position;
		}

		public override void LateFixedUpdate()
		{
			try
			{
				var groundRigidbody = ground.hitRigidbody;

				if (groundRigidbody == null || groundRigidbody != _earlyGroundRigidbody) return;

				var deltaRotation = groundRigidbody.rotation * Quaternion.Inverse(_earlyGroundTransform.rotation);
				deltaRotation = deltaRotation.ProjectRotationOnAxis(pawn.up);

				var pivotPosition = deltaRotation * (rigidbody.position - groundRigidbody.position) + groundRigidbody.position;

				var deltaPosition = groundRigidbody.position - _earlyGroundTransform.position;
				var newPosition = pivotPosition + deltaPosition;

				rigidbody.MovePosition(newPosition);

				/** <<============================================================>> **/

				if (enableRotateWithGround)
					rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
			}
			catch { }
		}

		protected override Vector3 CalculateGroundVelocity() =>
			ground.lastKnownRigidbody != null && !ground.isGrounded ? _lastGroundVelocity : default;

		#endregion
	}

	#endregion
}
