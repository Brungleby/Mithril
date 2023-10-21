
/** JumpAction.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Timers;
using UnityEngine;

#endregion

namespace Mithril.Pawn
{
	#region JumpActionBase

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public abstract class JumpActionBase<TPawn, TGround, TCollider, TRigidbody, TVector> : ActionComponent,
	IGroundUser<TGround>, IRigidbodyUser<TRigidbody>
	where TPawn : PawnBase<TCollider, TRigidbody, TVector>
	where TGround : MithrilComponent
	where TVector : unmanaged
	{
		#region Fields

		/// <summary>
		/// Strength of the jump force.
		///</summary>
		[Tooltip("Strength of the jump force.")]
		[Min(0f)]
		[SerializeField]

		private float _strength = 5f;
		public virtual float strength { get => _strength; set => _strength = value.Max(); }

		/// <summary>
		/// Upon jumping, the associated groundSensor will be disabled for this amount of time.
		///</summary>
		[Tooltip("Upon jumping, the associated GroundSensor will be disabled for this amount of time.")]
		[Min(0f)]
		[SerializeField]

		private float _disableGroundCheckDelay = 0.05f;
		public float disableGroundCheckDelay { get => _disableGroundCheckTimer.duration; set => _disableGroundCheckTimer.duration = value.Max(); }

		#endregion
		#region Members

		private Timer _disableGroundCheckTimer = new Timer();

		[AutoAssign] public TPawn pawn { get; protected set; }
#pragma warning disable
		[AutoAssign] public new TRigidbody rigidbody { get; protected set; }
#pragma warning restore
		[AutoAssign] public TGround ground { get; protected set; }

		#endregion

		protected virtual TVector direction => pawn.up;

		protected override void Awake()
		{
			base.Awake();

			_disableGroundCheckTimer.duration = _disableGroundCheckDelay;
			_disableGroundCheckTimer.onStart.AddListener(DisableGround);
			_disableGroundCheckTimer.onCease.AddListener(EnableGround);
		}

		protected virtual void FixedUpdate()
		{
			_disableGroundCheckTimer.Update();
		}

		public abstract void Jump();
		private void _Jump()
		{
			_disableGroundCheckTimer.Start();
			Jump();
		}

		protected abstract void EnableGround();
		protected abstract void DisableGround();

		protected override void OnActivated()
		{
			base.OnActivated();

			_Jump();
		}
	}

	#endregion
	#region JumpAction

	public class JumpAction : JumpActionBase<Pawn, GroundSensor, Collider, Rigidbody, Vector3>
	{
		protected override bool mayBegin => ground.isGrounded;

		public override void Jump()
		{
			var __jumpVector = direction * strength;
			var __lateralVelocity = Vector3.Scale(rigidbody.velocity, Vector3.one - direction);
			rigidbody.velocity = __lateralVelocity + __jumpVector;
		}

		protected override void EnableGround() => ground.temporarilyDisabled = false;
		protected override void DisableGround() => ground.temporarilyDisabled = true;
	}

	#endregion
}
