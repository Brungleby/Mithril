
/** FrictionMovement.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using UnityEngine;

#endregion

namespace Mithril.Pawn
{
	#region FrictionMovementBase

	/// <summary>
	/// Simulates deceleration along a plane.
	///</summary>

	public abstract class FrictionMovementBase<TPawn, TPhysics, TGround, TCollider, TRigidbody, TVector> : MovementComponent<TPawn, TCollider, TRigidbody, TVector>, IFrictionCustomUser<TVector>
	where TPawn : PawnBase<TCollider, TRigidbody, TVector>
	where TVector : unmanaged
	{
		#region Fields

		/// <summary>
		/// Custom component used to calculate friction.
		///</summary>
		[Tooltip("Custom component used to calculate friction.")]
		[SerializeField]
		private Component _customFrictionUser;

		/// <summary>
		/// Self-explanatory. This is the rate of the pawn's speed decrease over time.
		///</summary>
		[Tooltip("Self-explanatory. This is the rate of the pawn's speed decrease over time.")]
		[Min(0f)]
		[SerializeField]

		private float _deceleration = 50f;
		public float deceleration { get => _deceleration; set => _deceleration = value.Max(); }

		/// <summary>
		/// The percent of <see cref="deceleration"/> to use while airborne. 0 is realistic, 1 is highly controllable.
		///</summary>
		[Tooltip("The percent of deceleration to use while airborne. 0 is realistic, 1 is highly controllable.")]
		[Range(0f, 1f)]
		[SerializeField]

		private float _airbornePercent = 0f;
		public float airbornePercent { get => _airbornePercent; set => _airbornePercent = value.Clamp(); }

		#endregion
		#region Members

		[AutoAssign]
		public TGround ground { get; protected set; }

		[AutoAssign]
		public TPhysics physics { get; protected set; }

		protected IFrictionCustomUser<TVector> frictionUser { get; private set; }

		#endregion
		#region Properties

		public float strength => deceleration * surfaceStrength * airborneStrength;
		public abstract float airborneStrength { get; }
		public abstract float surfaceStrength { get; }

		public abstract TVector frictionVelocity { get; }

		#endregion
		#region Methods

		protected override void Awake()
		{
			base.Awake();

			try { frictionUser = (IFrictionCustomUser<TVector>)_customFrictionUser; }
			catch { frictionUser = this; }
		}

		protected abstract TVector CalculateDeceleration(in TVector currentVelocity);

		#endregion
	}

	#endregion
	#region FrictionMovement

	public sealed class FrictionMovement : FrictionMovementBase<Pawn, PawnPhysics, GroundSensor, Collider, Rigidbody, Vector3>
	{
		#region Properties

		public override float surfaceStrength
		{
			get
			{
				try { return ground.surface.frictionScale; }
				catch { return 1f; }
			}
		}
		public override float airborneStrength
		{
			get
			{
				try { return ground.isGrounded ? 1f : airbornePercent; }
				catch { return 1f; }
			}
		}

		public override Vector3 frictionVelocity =>
			rigidbody.velocity;

		#endregion
		#region Methods

		protected override void FixedUpdate()
		{
			var motionVector = CalculateDeceleration(
				frictionUser.frictionVelocity -
				physics.groundVelocity
			);
			rigidbody.AddForce(motionVector, ForceMode.Acceleration);
		}

		protected override Vector3 CalculateDeceleration(in Vector3 currentVelocity)
		{
			var result = -currentVelocity.normalized * strength * Time.fixedDeltaTime;
			var velocitySign = currentVelocity.Sign();

			if (velocitySign.x != 0f && velocitySign.x != Mathf.Sign(result.x + currentVelocity.x))
				result.x = -currentVelocity.x;
			if (velocitySign.y != 0f && velocitySign.y != Mathf.Sign(result.y + currentVelocity.y))
				result.y = -currentVelocity.y;
			if (velocitySign.z != 0f && velocitySign.z != Mathf.Sign(result.z + currentVelocity.z))
				result.z = -currentVelocity.z;

			return result / Time.fixedDeltaTime;
		}

		#endregion
	}

	#endregion
	#region IFrictionCustomUser

	public interface IFrictionCustomUser<TVector>
	{
		TVector frictionVelocity { get; }
	}

	#endregion
}
