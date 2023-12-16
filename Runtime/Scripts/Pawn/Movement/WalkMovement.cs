
/** WalkMovement.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

#endregion

namespace Mithril.Pawn
{
	#region WalkMovementBase

	/// <summary>
	/// This component translates input to movement along the ground in the world.
	///</summary>

	public abstract class WalkMovementBase<TCollider, TRigidbody, TPawn, TPhysics, TFriction, TGround, TMoveVector, TInputVector> :
	MovementComponent<TPawn, TCollider, TRigidbody, TMoveVector>,
	IFrictionCustomUser<TMoveVector>,
	IGravityHost
	where TPawn : PawnBase<TCollider, TRigidbody, TMoveVector>
	where TMoveVector : unmanaged
	where TInputVector : unmanaged
	{
		#region Fields

		/** <<============================================================>> **/
		/// <summary>
		/// Maximum speed to walk if fully inputting.
		///</summary>
		[Tooltip("Maximum speed to walk if fully inputting.")]
		[Min(0f)]
		[SerializeField]
		private float _maxSpeed = 5f;
		public virtual float maxSpeed { get => _maxSpeed; set => _maxSpeed = value.Max(); }

		/** <<============================================================>> **/
		/// <summary>
		/// Walk acceleration (how quickly we reach <see cref="maxSpeed"/>).
		///</summary>
		[Tooltip("Walk acceleration (how quickly we reach Max Speed). Modifying friction strength will affect our walk acceleration, but modifying this will not affect friction.")]
		[Min(1f)]
		[SerializeField]
		private float _maxAccel = 2f;
		public virtual float maxAccel { get => _maxAccel; set => _maxAccel = value.Max(1f); }

		[Header("Fine-Tuning")]

		/** <<============================================================>> **/
		/// <summary>
		/// If enabled, the pawn will stay glued to the surface on which they are standing.
		///</summary>
		[Tooltip("If enabled, the pawn will stay glued to the surface on which they are standing.")]
		public bool enableSurfaceClasping = true;

		/** <<============================================================>> **/
		/// <summary>
		/// Max speed will be multiplied by this curve depending on our directional slope angle. Upper and lower X limits are -90 and +90.
		///</summary>
		[Tooltip("Max speed will be multiplied by this curve depending on our directional slope angle. Upper and lower X limits are -90 and +90.")]
		public AnimationCurve speedByFloorAngle = new(
			new Keyframe[] { new(-90f, 1f), new(0f, 1f), new(+90f, 1f) }
		);

		/** <<============================================================>> **/
		/// <summary>
		/// Acceleration will be multiplied by this curve depending on our directional slope angle. Upper and lower X limits are -90 and +90.
		///</summary>
		[Tooltip("Acceleration will be multiplied by this curve depending on our directional slope angle. Upper and lower X limits are -90 and +90.")]
		public AnimationCurve accelByFloorAngle = new(
			new Keyframe[] { new(-90f, 1f), new(0f, 1f), new(+90f, 1f) }
		);

		/** <<============================================================>> **/
		/// <summary>
		/// When we land, our vertical speed will be multiplied by this amount to reduce carrying vertical momentum to lateral momentum.
		///</summary>
		[Tooltip("When we land, our vertical speed will be multiplied by this amount to reduce carrying vertical momentum to lateral momentum.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float _verticalSpeedPercentOnLand = 1f;
		/// <inheritdoc cref="_verticalSpeedPercentOnLand"/>
		public float verticalSpeedPercentOnLand { get => _verticalSpeedPercentOnLand; set => _verticalSpeedPercentOnLand = value.Clamp(); }

		#endregion
		#region Members

		[AutoAssign] public GravityResponse<TCollider, TRigidbody, TMoveVector> gravity { get; protected set; }
		[AutoAssign] public TPhysics physics { get; protected set; }
		[AutoAssign] public TFriction friction { get; protected set; }
		[AutoAssign] public TGround ground { get; protected set; }

		public TInputVector inputVectorRaw { get; private set; }
		public TMoveVector inputVector { get; protected set; }
		public TMoveVector lastValidInputVector { get; private set; }
		public TMoveVector walkAccelVector { get; protected set; }

		#endregion
		#region Properties

		/** <<============================================================>> **/

		public abstract TMoveVector velocityPercent { get; }
		public abstract float speedPercent { get; }
		public abstract TInputVector lateralVelocityPercent { get; }
		public abstract float lateralSpeedPercent { get; }

		public abstract TMoveVector frictionVelocity { get; }
		public abstract float directionalGroundAngle { get; }
		public abstract bool isStrafing { get; }
		protected abstract float targetWalkSpeed { get; }
		protected abstract float targetWalkAccel { get; }

		public abstract bool shouldUseGravity { get; }

		#endregion
		#region Methods

		protected virtual void Update()
		{
			inputVector = ConvertRawInputToWorld(inputVectorRaw);

			if (!inputVector.Equals(default(TMoveVector)))
				lastValidInputVector = inputVector;
		}

		protected abstract TMoveVector ConvertRawInputToWorld(in TInputVector input);

		protected abstract TMoveVector CalculateAcceleration(in TMoveVector direction, in TMoveVector velocity);

		protected abstract void OnGrounded();
		protected abstract void OnAirborne();

		public void OnInputWalk(InputContext context)
		{
			inputVectorRaw = context.ReadValue<TInputVector>();
		}

		#endregion
	}

	#endregion
	#region WalkMovement

	/// <summary>
	/// This component translates 2D input into 3D movement along the ground.
	///</summary>
	public sealed class WalkMovement : WalkMovementBase<Collider, Rigidbody, CapsulePawn, PawnPhysics, FrictionMovement, GroundSensor, Vector3, Vector2>
	{
		#region Fields

		[Header("Rotation")]

		/// <summary>
		/// Reference to a corresponding WalkRotation component.
		///</summary>
		[Tooltip("Reference to a corresponding WalkRotation component.")]
		public WalkRotation walkRotation;

		/// <summary>
		/// If enabled, this pawn will only walk in the direction it is facing. Allows for more control of how movement feels when used in conjunction with a WalkRotation component.
		///</summary>
		[Tooltip("If enabled, this pawn will only walk in the direction it is facing. Allows for more control of how movement feels when used in conjunction with a WalkRotation component.")]
		[SerializeField]
		public bool forwardOnly = false;

		#endregion
		#region Properties

		public float walkRotationSpeedModifier
		{
			get
			{
				try { return walkRotation.walkMovementSpeedModifier; }
				catch { return 1f; }
			}
		}

		public override bool shouldUseGravity => !ground.isGrounded;

		/** <<============================================================>> **/

		public override Vector3 velocityPercent =>
			pawn.velocity / maxSpeed;
		public override float speedPercent =>
			pawn.speed / maxSpeed;
		public override Vector2 lateralVelocityPercent =>
			pawn.lateralVelocityRelative / maxSpeed;
		public override float lateralSpeedPercent =>
			pawn.lateralSpeed / maxSpeed;

		/** <<============================================================>> **/

		public override Vector3 frictionVelocity =>
			ground.isGrounded ? pawn.velocity : pawn.lateralVelocity;

		public override float directionalGroundAngle =>
			Mathf.Asin(Vector3.Dot(pawn.up, walkAccelVector.normalized)) * Mathf.Rad2Deg;

		/** <<============================================================>> **/

		public override bool isStrafing => false;

		protected override float targetWalkSpeed =>
			speedByFloorAngle.Evaluate(directionalGroundAngle)
			* surfaceSpeedMultiplier
			* inputVector.magnitude * maxSpeed
			* walkRotationSpeedModifier
		;

		protected override float targetWalkAccel =>
			accelByFloorAngle.Evaluate(directionalGroundAngle)
			* maxAccel
			* friction.strength
		;

		private float surfaceSpeedMultiplier
		{
			get
			{
				try { return ground.surface.walkSpeedScale; }
				catch { return 1f; }
			}
		}

		#endregion
		#region Methods

		protected override void Awake()
		{
			base.Awake();

			walkRotation ??= GetComponent<WalkRotation>();

			ground.onGrounded.AddListener(OnGrounded);
			ground.onAirborne.AddListener(OnAirborne);
		}

		protected override void FixedUpdate()
		{
			#region Translate Input to Motion

			Vector3 motionNormal;

			if (inputVector.magnitude > 0f)
			{
				#region Determine Walk Vector Direction

				Vector3 flatWalkDirection = forwardOnly ? pawn.forward : inputVector.normalized;
				if (ground.isGrounded)
				{
					motionNormal = ground.GetDirectionalMotionVector(flatWalkDirection);
					walkAccelVector = Vector3.Cross(Vector3.Cross(pawn.up, flatWalkDirection), motionNormal).normalized;
				}
				else
				{
					motionNormal = pawn.up;
					walkAccelVector = flatWalkDirection;
				}

				#endregion
				#region Determine and Constrain Walk Vector Magnitude

				var perceivedVelocity =
					(ground.isGrounded ? pawn.velocity : pawn.lateralVelocity)
					- physics.lastValidGroundVelocity;

				walkAccelVector = CalculateAcceleration(walkAccelVector, perceivedVelocity);

				#endregion
				#region Apply Walk Vector

				rigidbody.AddForce(walkAccelVector, ForceMode.Acceleration);

				#endregion
			}
			else
			{
				motionNormal = pawn.up;
				walkAccelVector = Vector3.zero;
			}

			#endregion
			#region Snap Position to Ground

			if (enableSurfaceClasping && ground.isGrounded)
			{
				/**	Snap position
				*	Other attempts to get a more precise result have been ineffective.
				*	Sometimes this causes the pawn to get stuck in the ground when falling at high speeds.
				*/
				rigidbody.MovePosition(ground.adjustmentPoint);

				if (!ground.isHanging)
					rigidbody.velocity = pawn.ProjectVelocityOntoSurface(rigidbody.position, rigidbody.velocity, motionNormal);
			}

			#endregion
		}

		protected override Vector3 ConvertRawInputToWorld(in Vector2 input)
		{
			var __yawOnlyRotation = Quaternion.Euler(Vector3.Scale(
					pawn.viewTransform.eulerAngles,
					transform.up
			));

			return __yawOnlyRotation * input.XZ();
		}

		protected override Vector3 CalculateAcceleration(in Vector3 direction, in Vector3 velocity)
		{

			var __targetAccel = targetWalkAccel * Time.fixedDeltaTime;
			var __targetSpeed = targetWalkSpeed + friction.strength * Time.fixedDeltaTime;

			var __changeFactor = 1f - Vector3.Dot(direction, velocity.normalized).Max();
			var __limitedAccel = velocity.magnitude + __targetAccel <= __targetSpeed
				? __targetAccel
				: (__targetSpeed - velocity.magnitude)
			;
			var __accelLength = Mathf.Lerp(__limitedAccel, __targetAccel, __changeFactor).Max();
			__accelLength /= Time.fixedDeltaTime;

			return direction * __accelLength;
		}

		protected override void OnGrounded()
		{
			gravity.Refresh();

			/**	Vertical velocity mute
			*/

			pawn.verticalVelocityRelative *= verticalSpeedPercentOnLand;
		}

		protected override void OnAirborne()
		{
			gravity.Refresh();
		}

		#endregion
		#region Debug
#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			if (!Application.isPlaying) return;

			// Vector3 __arrowVector = walkAccelVector / friction.deceleration;

			// DebugDraw.DrawArrow(transform.position, __arrowVector, Color.yellow);
			// DebugDraw.DrawArrow(transform.position, velocityPercent, Color.cyan);

			// UnityEditor.Handles.Label(pawn.collider.GetTailPosition(), pawn.lateralVelocity.magnitude.ToString("0.00"));
		}
#endif
		#endregion
	}

	#endregion
}
