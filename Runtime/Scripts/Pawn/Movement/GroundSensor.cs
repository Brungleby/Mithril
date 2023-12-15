
/** GroundSensor.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Mithril.Pawn
{
	#region GroundSensorBase

	public abstract class GroundSensorBase<TPawn, TCollider, TColliderShape, TRigidbody, TVector, TPool, THit> :
	CasterComponent<TColliderShape, THit>, IPawnUser<TPawn>, ILateFixedUpdaterComponent
	where TPawn : PawnBase<TCollider, TRigidbody, TVector>
	where TColliderShape : Component
	where TVector : unmanaged
	where TPool : HitPool<THit>, new()
	{
		#region Fields

		/// <summary>
		/// Timer controlling temporary disablement.
		///</summary>
		[Tooltip("Timer controlling temporary disablement.")]
		[SerializeField]
		private Timer _temporarilyDisableTimer = new() { duration = 0.2f };
		public Timer temporarilyDisableTimer => _temporarilyDisableTimer;

		/// <summary>
		/// Maximum angle considered ground. Any surface steeper than this angle will be considered a wall (and also can't be walked on).
		///</summary>
		[Tooltip("Maximum angle considered ground. Any surface steeper than this angle will be considered a wall (and also can't be walked on).")]
		[Range(0f, 90f)]
		[SerializeField]
		public float maxGroundAngle = 70f;

		[Header("Fine-Tuning")]

		public float sensorRadiusGrounded = 0.15f;
		public float sensorRadiusAirborne = 0.05f;

		#endregion
		#region Members

		public UnityEvent onGrounded { get; } = new();
		public UnityEvent onAirborne { get; } = new();

		[AutoAssign]
		public TPawn pawn { get; protected set; }

		public TRigidbody lastKnownRigidbody { get; private set; }

		private bool _isGrounded;
		public bool isGrounded
		{
			get => _isGrounded;
			protected set
			{
				if (_isGrounded == value) return;
				_isGrounded = value;

				if (value)
				{
					onGrounded.Invoke();
				}
				else
				{
					motionHit = default;
					groundHit = default;
					onAirborne.Invoke();
				}
			}
		}

		private bool _temporarilyDisabled = false;
		public bool temporarilyDisabled
		{
			get => _temporarilyDisabled;
			protected set
			{
				_temporarilyDisabled = value;

				if (value)
					isGrounded = false;
			}
		}

		protected TPool motionPool;
		public THit motionHit { get; protected set; }

		protected TPool groundPool;
		public THit groundHit { get; protected set; }

		protected TPool hangingPool;
		internal bool isHanging { get; private set; }

		private LateFixedUpdater lateFixedUpdater;

		#endregion
		#region Properties

		public bool isSliding => !isGrounded && motionPool.blocked;
		protected float sensorLength => isGrounded ? sensorRadiusGrounded : sensorRadiusAirborne;

		public abstract TVector adjustmentPoint { get; }

		public abstract TCollider hitCollider { get; }
		public abstract TRigidbody hitRigidbody { get; }
		public abstract Surface surface { get; }

		public abstract TVector up { get; }

		public abstract TVector motionRight { get; }
		public abstract TVector motionUp { get; }

		public float angle => GetAngle(groundHit);

		#endregion
		#region Methods

		public void TemporarilyDisable()
		{
			_temporarilyDisableTimer.Start();
		}

		protected override void Awake()
		{
			base.Awake();

			lateFixedUpdater = new(this);

			_temporarilyDisableTimer.onStart.AddListener(() => temporarilyDisabled = true);
			_temporarilyDisableTimer.onCease.AddListener(() => temporarilyDisabled = false);
		}

		protected virtual void OnEnable()
		{
			lateFixedUpdater.SetupCoroutine();
		}

		protected virtual void OnDisable()
		{
			isGrounded = false;
			lastKnownRigidbody = default;
		}

		public void LateFixedUpdate()
		{
			_temporarilyDisableTimer.Update();
			if (temporarilyDisabled) return;

			SensorUpdate();

			isGrounded = shouldBeGrounded;
			isHanging = shouldBeHanging;
		}

		protected abstract void SensorUpdate();

		private bool shouldBeGrounded => groundPool.blocked && GetAngle(groundHit) <= maxGroundAngle;
		private bool shouldBeHanging => isGrounded && !hangingPool.blocked;

		public abstract TVector GetDirectionalMotionVector(TVector forward);

		public float GetMotionDirectionalAngle(TVector forward) => GetDirectionalAngle(motionHit, forward);
		public float GetGroundDirectionalAngle(TVector forward) => GetDirectionalAngle(groundHit, forward);

		protected abstract float GetAngle(THit hit);
		protected abstract float GetDirectionalAngle(THit hit, TVector forward);

		#endregion
	}

	#endregion
	#region GroundSensor

	public sealed class GroundSensor : GroundSensorBase<CapsulePawn, Collider, CapsuleCollider, Rigidbody, Vector3, HitPool, RaycastHit>
	{
		#region Members

		private float _lastSensorLength;

		#endregion
		#region Properties

		public override Collider hitCollider => motionHit.collider;
		public override Rigidbody hitRigidbody => motionHit.rigidbody;
		public override Surface surface => motionHit.GetSurface();

		public override Vector3 up => groundPool.blocked ? groundHit.normal : pawn.up;
		public Vector3 forward => Vector3.Cross(up, pawn.up).normalized;

		public override Vector3 motionRight => Vector3.Cross(motionUp, motionForward);
		public override Vector3 motionUp => motionPool.blocked ? motionHit.normal : pawn.up;
		public Vector3 motionForward => Vector3.Cross(motionUp, pawn.up).normalized;

		public override Vector3 adjustmentPoint =>
			motionHit.GetAdjustmentPoint(pawn.collider.transform.position + pawn.up * _lastSensorLength, -pawn.up);

		#endregion
		#region Methods

		protected override void Awake()
		{
			base.Awake();

			motionPool = new(4);
			groundPool = new(4);
			hangingPool = new(1);
		}

		public override Vector3 GetDirectionalMotionVector(Vector3 forward)
		{
			if (isHanging)
				try { return GetMotionDirectionalAngle(forward) < GetGroundDirectionalAngle(forward) ? motionUp : up; }
				catch { return pawn.up; }
			else
				return motionUp;
		}

		protected override void SensorUpdate()
		{
			/**	Motion Hit
			*/
			_lastSensorLength = sensorLength;
			var upOffset = pawn.up * _lastSensorLength;
			motionPool.CapsuleCast(
				pawn.collider.transform.position + upOffset,
				pawn.collider, -pawn.up, _lastSensorLength * 2f, layers
			);

			/**	Ground Hit
			*/
			groundPool.Clear();
			foreach (var iHit in motionPool)
			{
				/**	Check for walls
				*/

				var distance = Vector3.Scale(iHit.point - pawn.position, Vector3.one - pawn.up).sqrMagnitude;
				var compare = (pawn.collider.radius * pawn.collider.radius) - (pawn.skinWidth * pawn.skinWidth);

				if (distance > compare) continue;

				/**	Check for ground
				*/

				groundPool.SphereCast(
					iHit.point + pawn.up * pawn.collider.radius,
					0.01f, -pawn.up, pawn.collider.radius * 2f, layers
				);
				groundHit = groundPool.nearest;

				if (GetAngle(groundHit) > maxGroundAngle) continue;
				motionHit = iHit;
				break;
			}

			/**	Hanging Hit
			*/
			hangingPool.LineCast(pawn.collider.GetTailPosition(), -pawn.up, pawn.collider.radius, layers);
		}

		protected override float GetAngle(RaycastHit hit) =>
			Mathf.Acos(Vector3.Dot(pawn.up, hit.normal).Clamp()) * Mathf.Rad2Deg;

		protected override float GetDirectionalAngle(RaycastHit hit, Vector3 forward) =>
			Mathf.Asin(Vector3.Dot(hit.normal, forward)) * Mathf.Rad2Deg;

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;

			// if (motionPool.blocked) DebugDraw.DrawPoint(motionHit.point, Color.red);
		}

		#endregion
	}

	#endregion
}
