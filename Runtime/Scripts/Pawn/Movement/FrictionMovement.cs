
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

	public abstract class FrictionMovementBase<TPawn, TGround, TCollider, TRigidbody, TVector> : MovementComponent<TPawn, TCollider, TRigidbody, TVector>, IFrictionCustomUser<TVector>, ILateFixedUpdaterComponent
	where TPawn : PawnBase<TCollider, TRigidbody, TVector>
	where TVector : unmanaged
	{
		#region Fields

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

		private LateFixedUpdater _lateFixedUpdater;

		[AutoAssign]
		public TGround ground { get; protected set; }
		protected IFrictionCustomUser<TVector> frictionUser { get; private set; }

		protected TransformData _earlyGroundTransform;
		protected TRigidbody _earlyGroundRigidbody;
		protected TRigidbody _lastFrameRigidbody;
		protected TVector _lastFramePosition;

		#endregion
		#region Properties

		public float strength => deceleration * surfaceStrength * airborneStrength;
		public abstract float airborneStrength { get; }
		public abstract float surfaceStrength { get; }

		public abstract TVector frictionVelocity { get; }
		public TVector groundVelocity { get; private set; }
		protected TVector _lastGroundVelocity;

		#endregion
		#region Methods

		protected override void Awake()
		{

			base.Awake();
		}

		protected override void Init()
		{
			base.Init();

			frictionUser = GetFrictionUser();
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

		public abstract void LateFixedUpdate();

		protected abstract TVector CalculateDeceleration(in TVector currentVelocity);

		protected abstract TVector CalculateGroundVelocity();

		private IFrictionCustomUser<TVector> GetFrictionUser()
		{
			var __components = GetComponents<IFrictionCustomUser<TVector>>();

			IFrictionCustomUser<TVector> __result = null;
			foreach (var iUser in __components)
			{
				if (__result == null)
				{
					__result = iUser;
					continue;
				}

				if (__result.Equals(this))
				{
					return iUser;
				}
			}

			return __result;
		}

		#endregion
	}

	#endregion
	#region FrictionMovement

	public sealed class FrictionMovement : FrictionMovementBase<Pawn, GroundSensor, Collider, Rigidbody, Vector3>
	{
		#region Fields

		/// <summary>
		/// If enabled, the rigidbody will rotate along with the ground beneath it. Otherwise, it will maintain its rotation.
		///</summary>
		[Tooltip("If enabled, the rigidbody will rotate along with the ground beneath it. Otherwise, it will maintain its rotation.")]

		public bool enableRotateWithGround = true;

		#endregion
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
			try
			{
				var __groundRigidbody = ground.hitRigidbody;

				/** <<============================================================>> **/
				/**	Account for changes in velocity of the ground
				*	This method factors in both linear and angular velocity.
				*/

				if (__groundRigidbody != null && _lastFrameRigidbody == null)
					rigidbody.velocity -= __groundRigidbody.GetPointVelocity(rigidbody.position);

				else if (__groundRigidbody == null && _lastFrameRigidbody != null)
				{
					_lastGroundVelocity = _lastFrameRigidbody.GetPointVelocity(rigidbody.position);
					rigidbody.velocity += _lastGroundVelocity;
				}

				/** <<============================================================>> **/

				if (__groundRigidbody != null)
				{
					_earlyGroundRigidbody = __groundRigidbody;
					_earlyGroundTransform = new TransformData(__groundRigidbody.position, __groundRigidbody.rotation, __groundRigidbody.transform.lossyScale);
					_lastFrameRigidbody = __groundRigidbody;
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

			base.FixedUpdate();

			var __motionVector = CalculateDeceleration(frictionUser.frictionVelocity - groundVelocity);
			rigidbody.AddForce(__motionVector, ForceMode.Acceleration);
		}

		public override void LateFixedUpdate()
		{
			try
			{
				var __groundRigidbody = ground.hitRigidbody;

				if (__groundRigidbody == null || __groundRigidbody != _earlyGroundRigidbody) return;

				var __deltaRotation = __groundRigidbody.rotation * Quaternion.Inverse(_earlyGroundTransform.rotation);
				__deltaRotation = __deltaRotation.ProjectRotationOnAxis(pawn.up);

				var __pivotPosition = __deltaRotation * (rigidbody.position - __groundRigidbody.position) + __groundRigidbody.position;

				var __deltaPosition = __groundRigidbody.position - _earlyGroundTransform.position;
				var __newPosition = __pivotPosition + __deltaPosition;

				rigidbody.MovePosition(__newPosition);

				/** <<============================================================>> **/

				if (enableRotateWithGround)
					rigidbody.MoveRotation(rigidbody.rotation * __deltaRotation);
			}
			catch { }
		}

		protected override Vector3 CalculateDeceleration(in Vector3 currentVelocity)
		{
			var __result = -currentVelocity.normalized * strength * Time.fixedDeltaTime;
			var __velocitySign = currentVelocity.Sign();

			if (__velocitySign.x != 0f && __velocitySign.x != Mathf.Sign(__result.x + currentVelocity.x))
				__result.x = -currentVelocity.x;
			if (__velocitySign.y != 0f && __velocitySign.y != Mathf.Sign(__result.y + currentVelocity.y))
				__result.y = -currentVelocity.y;
			if (__velocitySign.z != 0f && __velocitySign.z != Mathf.Sign(__result.z + currentVelocity.z))
				__result.z = -currentVelocity.z;

			return __result / Time.fixedDeltaTime;
		}

		protected override Vector3 CalculateGroundVelocity() =>
			ground.lastKnownRigidbody != null && !ground.isGrounded ? _lastGroundVelocity : default;

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
