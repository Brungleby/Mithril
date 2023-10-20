
/** PawnCapsule.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril.Pawn
{
	#region CapsulePawn

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class CapsulePawn : Pawn<CapsuleCollider>
	{
		#region Members

		private float _startHeight;

		#endregion
		#region Properties

		public override float height
		{
			get => collider.height;
			set => collider.height = value;
		}

		protected override float maxSkinWidth => collider.radius;

		public float skinnedRadius => collider.radius + skinWidth;

		#endregion
		#region Methods

		protected override void Awake()
		{
			base.Awake();

			_startHeight = height;
		}

		/// <summary>
		/// Sets the height and offsets the capsule as if the pawn were on the ground.
		///</summary>

		public void SetHeightOnGround(float value)
		{
			height = value;
			collider.center = Vector3.down * (_startHeight - value) / 2f;
			if (rigidbody.automaticCenterOfMass) rigidbody.ResetCenterOfMass();
		}

		#endregion
	}
	#endregion
}
