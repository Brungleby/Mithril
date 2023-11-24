
/** GroundSensor.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Linq;

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Mithril.Pawn
{
	#region GroundSensorBase

	public abstract class GroundSensorBase<TPawn, TGenericCollider, TCollider, TRigidbody, TVector, THit, TShapeInfo> :
	CasterComponent<TCollider, THit, TShapeInfo>, IPawnUser<TPawn>
	where THit : HitBase, new()
	where TCollider : Component
	where TVector : unmanaged
	where TShapeInfo : ShapeInfoBase
	{
		#region Fields

		/// <summary>
		/// Maximum length/size of the cast.
		///</summary>

		[Tooltip("Maximum length/size of the cast.")]
		[Min(0f)]
		[SerializeField]

		public float maxStepHeight = 1f;

		#endregion
		#region Members

		[HideInInspector] public UnityEvent onGrounded = new();
		[HideInInspector] public UnityEvent onAirborne = new();

		[AutoAssign]
		public TPawn pawn { get; protected set; }

		/// <summary>
		/// This is the primary hit which uses the <see cref="collider"/> as a template. It is calculated by casting from the step height, straight down.
		///</summary>

		private THit _directHit;
		protected THit directHit => _directHit;

		/// <summary>
		/// This is a secondary hit which is calculated by casting a small sphere just above the directHit result. It is only performed if we are grounded.
		///</summary>

		private THit _infoHit;
		protected THit infoHit => _infoHit ?? new();

		/// <summary>
		/// This is a secondary hit which is calculated by casting the skinned collider a short distance downward. It is only performed if we are airborne.
		///</summary>

		private THit _landingHit;

		/// <summary>
		/// This is a tertiary hit which is calculated by casting a line directly underneath the collider. It is only performed if we are grounded.
		///</summary>

		private THit _hangingHit;
		protected THit hangingHit => _hangingHit ?? new();

		/// <summary>
		/// This is a tertiary hit which is calculated by casting a small sphere along the infoHit to determine if there is or is not a wall which should block our movement.
		///</summary>

		private THit _wallHit;
		protected THit wallHit => _wallHit ?? new();

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
			set
			{
				_temporarilyDisabled = value;

				if (value)
					isGrounded = false;
			}
		}
		public TRigidbody lastKnownRigidbody { get; private set; }

		#endregion
		#region Properties

#if UNITY_EDITOR
		protected override THit hitToDraw => _directHit;
#endif

		public abstract TGenericCollider hitCollider { get; }
		public abstract TRigidbody hitRigidbody { get; }

		public abstract Surface surface { get; }

		public abstract TVector right { get; }
		public abstract TVector up { get; }
		public abstract float angle { get; }

		public abstract TVector rightPrecise { get; }
		public abstract TVector upPrecise { get; }
		public abstract float anglePrecise { get; }

		public abstract TVector adjustmentPoint { get; }
		public abstract float stepHeight { get; }

		public bool isHanging => !hangingHit.IsValidAndBlocked();

		#endregion
		#region Methods

		protected virtual void OnDisable()
		{
			isGrounded = false;
			lastKnownRigidbody = default;
		}

		protected virtual void FixedUpdate()
		{
			if (temporarilyDisabled) return;

			_directHit = SenseDirectHit();

			if (isGrounded)
			{
				if (directHit.isBlocked)
				{
					_infoHit = SenseInfoHit();
					_hangingHit = SenseHangingHit();

					lastKnownRigidbody = GetLastKnownRigidbody();
				}
				else
				{
					_infoHit = null;
					_hangingHit = null;
					isGrounded = false;
				}
			}
			else
			{
				_landingHit = SenseLandingHit();

				if (_landingHit.isBlocked)
					isGrounded = true;
			}
		}

		protected abstract THit SenseDirectHit();
		protected abstract THit SenseInfoHit();
		protected abstract THit SenseLandingHit();
		protected abstract THit SenseHangingHit();

		protected abstract TRigidbody GetLastKnownRigidbody();

		#endregion
	}

	#endregion
	#region GroundSensor

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class GroundSensor : GroundSensorBase<CapsulePawn, Collider, CapsuleCollider, Rigidbody, Vector3, Hit, CapsuleInfo>
	{
		#region Fields

		[Range(0f, 90f)]
		[SerializeField]

		public float maxWalkableAngle = 70f;

		#endregion
		#region Properties

		public override Collider hitCollider => directHit.collider;
		public override Rigidbody hitRigidbody => directHit.rigidbody;
		public override Surface surface => directHit.surface;

		public override Vector3 right => Vector3.Cross(up, forward);
		public override Vector3 up => infoHit.IsValidAndBlocked() ? infoHit.normal : pawn.up;
		public Vector3 forward => Vector3.Cross(up, pawn.up).normalized;
		public override float angle => Mathf.Acos(Vector3.Dot(pawn.up, up).Clamp()) * Mathf.Rad2Deg;

		public override Vector3 rightPrecise => Vector3.Cross(upPrecise, forwardPrecise);
		public override Vector3 upPrecise => directHit.normal;
		public Vector3 forwardPrecise => Vector3.Cross(upPrecise, pawn.up).normalized;
		public override float anglePrecise => Mathf.Acos(Vector3.Dot(pawn.up, up).Clamp()) * Mathf.Rad2Deg;

		public override Vector3 adjustmentPoint => directHit.adjustmentPoint - collider.center;
		public override float stepHeight => Vector3.Dot(pawn.up, directHit.point - collider.GetTailPosition());

		#endregion
		#region Methods

		protected override Hit SenseDirectHit()
		{
			float __upOffset = (maxStepHeight - collider.radius).Max(pawn.skinWidth);

			var __hits = Hit.CapsuleCastAll(
				collider.GetHeadPositionUncapped() + pawn.up * __upOffset,
				collider.GetTailPositionUncapped() + pawn.up * __upOffset,
				collider.radius,
				-pawn.up, maxStepHeight + __upOffset, layers
			).ToList();
			__hits.Sort();

			foreach (var iHit in __hits)
			{
				var __lateralPercentFromTail = (Vector3.Scale(iHit.point - collider.GetTailPosition(), Vector3.one - pawn.up).magnitude / collider.radius).Clamp();

				var __groundAngle = Mathf.Asin(__lateralPercentFromTail) * Mathf.Rad2Deg;

				if (__groundAngle <= maxWalkableAngle)
					return iHit;
			}

			return Hit.CapsuleCast(
				collider.GetHeadPositionUncapped() + pawn.up * __upOffset,
				collider.GetTailPositionUncapped() + pawn.up * __upOffset,
				collider.radius,
				-pawn.up, maxStepHeight + __upOffset, layers
			);
		}

		protected override Hit SenseLandingHit() => Hit.CapsuleCast
			(
				collider.GetHeadPositionUncapped() + pawn.up * pawn.skinWidth,
				collider.GetTailPositionUncapped() + pawn.up * pawn.skinWidth,
				collider.radius,
				-pawn.up, pawn.skinWidth * 2f, layers
			);

		protected override Hit SenseInfoHit() => Hit.SphereCast
			(
				directHit.point + (pawn.up * maxStepHeight),
				pawn.skinWidth,
				-pawn.up, maxStepHeight, layers
			);

		protected override Hit SenseHangingHit() => Hit.Linecast
			(
				collider.GetTailPosition(),
				-pawn.up, maxStepHeight, layers
			);

		protected override Rigidbody GetLastKnownRigidbody() =>
			directHit.rigidbody;
#if UNITY_EDITOR
		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();

			if (!Application.isPlaying) return;

			// Gizmos.color = isGrounded ? Color.green : Color.red;
			// Gizmos.DrawSphere(collider.GetTailPosition(), 0.1f);

			// DebugDraw.DrawArrow(directHit.point, right * 0.25f, Color.red);
			// DebugDraw.DrawArrow(directHit.point, up * 0.25f, Color.green);
			// DebugDraw.DrawArrow(directHit.point, forward * 0.25f, Color.blue);

			// DebugDraw.DrawCapsuleCast(directHit, pawn.forward, pawn.up, collider.radius, collider.height);

			// DebugDraw.DrawSphereCast(preciseHit, pawn.skinWidth);
			// DebugDraw.DrawLinecast(hangingHit);
		}
#endif
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
