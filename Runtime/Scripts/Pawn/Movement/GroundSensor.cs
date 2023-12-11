
/** GroundSensor.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Mithril.Pawn
{
	#region GroundSensorBase

	public abstract class GroundSensorBase<TPawn, TGenericCollider, TCollider, TRigidbody, TVector, THit, TShapeInfo> :
	CasterComponent<TCollider, THit, TShapeInfo>, IPawnUser<TPawn>
	where TPawn : PawnBase<TGenericCollider, TRigidbody, TVector>
	where THit : HitBase, new()
	where TCollider : Component
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
					onGrounded.Invoke();
				else
					onAirborne.Invoke();
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

		#endregion
		#region Properties

		public abstract Rigidbody hitRigidbody { get; }
		public abstract Surface surface { get; }

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

			if (isGrounded)
			{
				groundHit = GetGroundHit();

				if (!motionHit.isBlocked || GetAngle(groundHit) > maxGroundAngle)
					isGrounded = false;
			}
			else
			{
				if (motionHit.isBlocked)
					isGrounded = true;
			}
		}

		protected abstract THit GetMotionHit();
		protected abstract THit GetGroundHit();

		protected abstract float GetAngle(THit hit);

		#endregion
	}

	#endregion
	#region GroundSensor

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class GroundSensor : GroundSensorBase<CapsulePawn, Collider, CapsuleCollider, Rigidbody, Vector3, Hit, CapsuleInfo>
	{
		#region Properties

		public override Rigidbody hitRigidbody => motionHit.rigidbody;
		public override Surface surface => motionHit.surface;

		public override Vector3 motionUp => motionHit.IsValidAndBlocked() ? motionHit.normal : pawn.up;

		// public override Collider hitCollider => directHit.collider;
		// public override Rigidbody hitRigidbody => directHit.rigidbody;
		// public override Surface surface => directHit.surface;

		// public override Vector3 right => Vector3.Cross(up, forward);
		// public override Vector3 up => infoHit.IsValidAndBlocked() ? infoHit.normal : pawn.up;
		// public Vector3 forward => Vector3.Cross(up, pawn.up).normalized;

		// public override Vector3 rightPrecise => Vector3.Cross(upPrecise, forwardPrecise);
		// public override Vector3 upPrecise => directHit.normal;
		// public Vector3 forwardPrecise => Vector3.Cross(upPrecise, pawn.up).normalized;
		// public override float anglePrecise => Mathf.Acos(Vector3.Dot(pawn.up, up).Clamp()) * Mathf.Rad2Deg;

		// public override Vector3 adjustmentPoint => directHit.adjustmentPoint - collider.center;
		// public override float stepHeight => Vector3.Dot(pawn.up, directHit.point - collider.GetTailPosition());

		#endregion
		#region Methods

		protected override Hit GetMotionHit()
		{
			var upOffset = pawn.up * pawn.skinWidth;
			return Hit.CapsuleCast(
				pawn.collider.GetHeadPositionUncapped() + upOffset,
				pawn.collider.GetTailPositionUncapped() + upOffset,
				pawn.collider.radius,
				-pawn.up,
				pawn.skinWidth * 2f,
				layers
			);
		}

		protected override Hit GetGroundHit()
		{
			return Hit.SphereCast(
				motionHit.point + pawn.up * pawn.skinWidth,
				pawn.skinWidth * 0.5f,
				-pawn.up,
				pawn.skinWidth * 2f,
				layers
			);
		}

		protected override float GetAngle(Hit hit)
		{
			return Mathf.Acos(Vector3.Dot(pawn.up, hit.normal).Clamp()) * Mathf.Rad2Deg;
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;

			try
			{
				Debug.Log($"{angle:00}");
				motionHit.OnDrawGizmos();
				groundHit.OnDrawGizmos();
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
