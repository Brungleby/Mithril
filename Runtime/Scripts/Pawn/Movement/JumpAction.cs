
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

		#endregion
		#region Members

		[AutoAssign] public TPawn pawn { get; protected set; }
#pragma warning disable
		[AutoAssign] public new TRigidbody rigidbody { get; protected set; }
#pragma warning restore
		[AutoAssign] public TGround ground { get; protected set; }

		#endregion

		protected virtual TVector direction => pawn.up;

		public abstract void Jump();

		protected override void OnActivated()
		{
			base.OnActivated();

			Jump();
		}
	}

	#endregion
	#region JumpAction

	public class JumpAction : JumpActionBase<Pawn, GroundSensor, Collider, Rigidbody, Vector3>
	{
		protected override bool mayBegin => ground.isGrounded;

		public override void Jump()
		{
			var jumpVector = direction * strength;
			var lateralVelocity = Vector3.Scale(rigidbody.velocity, Vector3.one - direction);
			rigidbody.velocity = lateralVelocity + jumpVector;

			ground.TemporarilyDisable();
		}
	}

	#endregion
}
