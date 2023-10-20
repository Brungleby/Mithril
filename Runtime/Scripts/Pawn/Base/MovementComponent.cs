
/** MovementModule.cs
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
	#region MovementModule

	/// <summary>
	/// This component is for use on objects with simulated <see cref="Rigidbody"/>s.
	///</summary>

	public abstract class MovementComponent<TVector> : MithrilComponent
	{
		protected abstract void FixedUpdate();
	}

	public abstract class MovementComponent<TCollider, TRigidbody, TVector> :
	MovementComponent<TVector>, IColliderUser<TCollider>, IRigidbodyUser<TRigidbody>
	{
#pragma warning disable
		[AutoAssign] public new TCollider collider { get; set; }
		[AutoAssign] public new TRigidbody rigidbody { get; set; }
#pragma warning restore
	}

	public abstract class MovementComponent<TPawn, TCollider, TRigidbody, TVector> :
	MovementComponent<TVector>, IPawnUser<TPawn>, IColliderUser<TCollider>, IRigidbodyUser<TRigidbody>
	where TPawn : PawnBase<TCollider, TRigidbody, TVector>
	where TVector : unmanaged
	{
		[AutoAssign] public TPawn pawn { get; protected set; }
#pragma warning disable
		[AutoAssign] public new TCollider collider { get; protected set; }
		[AutoAssign] public new TRigidbody rigidbody { get; protected set; }
#pragma warning restore
	}

	#endregion
}
