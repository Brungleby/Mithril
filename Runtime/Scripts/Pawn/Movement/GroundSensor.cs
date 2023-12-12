
/** GroundSensor.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Mithril.Pawn
{
	#region GroundSensorBase

	public abstract class GroundSensorBase<TPawn, TCollider, TColliderShape, TRigidbody, TVector, THit, TShapeInfo> :
	CasterComponent<TColliderShape, THit, TShapeInfo>, IPawnUser<TPawn>
	where TPawn : PawnBase<TCollider, TRigidbody, TVector>
	where THit : HitBase, new()
	where TColliderShape : Component
	where TVector : unmanaged
	where TShapeInfo : ShapeInfoBase
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
					groundHit = null;

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

		internal THit motionHit { get; private set; }
		internal THit groundHit { get; private set; }

		private bool _isHanging;
		internal bool isHanging => isGrounded && _isHanging;

		#endregion
		#region Properties

		public bool isSliding => !isGrounded && motionHit.isBlocked;
		protected float sensorLength => isGrounded ? sensorRadiusGrounded : sensorRadiusAirborne;

		public abstract TCollider hitCollider { get; }
		public abstract TRigidbody hitRigidbody { get; }
		public abstract Surface surface { get; }

		public abstract TVector up { get; }

		// public abstract TVector motionRight { get; }
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

			_temporarilyDisableTimer.onStart.AddListener(() => temporarilyDisabled = true);
			_temporarilyDisableTimer.onCease.AddListener(() => temporarilyDisabled = false);
		}

		protected virtual void OnDisable()
		{
			isGrounded = false;
			lastKnownRigidbody = default;
		}

		protected virtual void FixedUpdate()
		{
			if (temporarilyDisabled)
			{
				_temporarilyDisableTimer.Update();
				return;
			}

			motionHit = GetMotionHit();
			groundHit = GetGroundHit();

			isGrounded = shouldBeGrounded;
			_isHanging = GetIsHanging();
		}

		private bool shouldBeGrounded => motionHit.isBlocked && GetAngle(groundHit) <= maxGroundAngle;

		public abstract TVector GetDirectionalMotionVector(TVector forward);

		protected abstract THit GetMotionHit();
		protected abstract THit GetGroundHit();
		protected abstract bool GetIsHanging();

		public float GetMotionDirectionalAngle(TVector forward) => GetDirectionalAngle(motionHit, forward);
		public float GetGroundDirectionalAngle(TVector forward) => GetDirectionalAngle(groundHit, forward);

		protected abstract float GetAngle(THit hit);
		protected abstract float GetDirectionalAngle(THit hit, TVector forward);

		#endregion
	}

	#endregion
	#region GroundSensor

	public sealed class GroundSensor : GroundSensorBase<CapsulePawn, Collider, CapsuleCollider, Rigidbody, Vector3, Hit, CapsuleInfo>
	{
		#region Properties

		public override Collider hitCollider => motionHit.collider;
		public override Rigidbody hitRigidbody => motionHit.rigidbody;
		public override Surface surface => motionHit.surface;

		public override Vector3 up => groundHit.IsValidAndBlocked() ? groundHit.normal : pawn.up;
		public Vector3 forward => Vector3.Cross(up, pawn.up).normalized;

		// public override Vector3 motionRight => Vector3.Cross(motionUp, motionForward);
		public override Vector3 motionUp => motionHit.IsValidAndBlocked() ? motionHit.normal : pawn.up;
		// public Vector3 motionForward => Vector3.Cross(motionUp, pawn.up).normalized;

		#endregion
		#region Methods

		public override Vector3 GetDirectionalMotionVector(Vector3 forward)
		{
			if (isHanging)
				try { return GetMotionDirectionalAngle(forward) < GetGroundDirectionalAngle(forward) ? motionUp : up; }
				catch { return pawn.up; }
			else
				return motionUp;
		}

		protected override Hit GetMotionHit()
		{
			var upOffset = pawn.up * sensorLength;
			var hits = Hit.CapsuleCastAll(
				pawn.collider.GetHeadPositionUncapped() + upOffset,
				pawn.collider.GetTailPositionUncapped() + upOffset,
				pawn.collider.radius,
				-pawn.up,
				sensorLength * 2f,
				layers
			).ToList();
			hits.Sort();

			foreach (var iHit in hits)
				if (GetAngle(iHit) > maxGroundAngle) continue;
				else return iHit;

			return Hit.CapsuleCast(
				pawn.collider.GetHeadPositionUncapped() + upOffset,
				pawn.collider.GetTailPositionUncapped() + upOffset,
				pawn.collider.radius,
				-pawn.up,
				sensorLength * 2f,
				layers
			);
		}

		protected override Hit GetGroundHit() =>
			Hit.SphereCast(
				motionHit.point + pawn.up * sensorRadiusAirborne,
				sensorRadiusAirborne * 0.5f,
				-pawn.up,
				sensorRadiusAirborne * 2f,
				layers
			);

		protected override bool GetIsHanging() =>
			!Hit.Linecast(
				pawn.collider.GetTailPosition() + pawn.up * pawn.skinWidth,
				-pawn.up,
				pawn.collider.radius + pawn.skinWidth,
				layers
			).isBlocked;

		protected override float GetAngle(Hit hit) =>
			Mathf.Acos(Vector3.Dot(pawn.up, hit.normal).Clamp()) * Mathf.Rad2Deg;

		protected override float GetDirectionalAngle(Hit hit, Vector3 forward) =>
			Mathf.Asin(Vector3.Dot(hit.normal, forward)) * Mathf.Rad2Deg;

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;
			try
			{
				// motionHit.OnDrawGizmos();
				// groundHit.OnDrawGizmos();
			}
			catch { }
		}

		#endregion
	}

	#endregion
	#region IGroundUser

	public interface IGroundUser<TGround>
	{
		TGround ground { get; }
	}

	#endregion
}
