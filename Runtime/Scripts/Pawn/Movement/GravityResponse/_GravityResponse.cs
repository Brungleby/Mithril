
/** GravityResponse.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril.Pawn
{
	#region GravityResponse

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public abstract class GravityResponse<TCollider, TRigidbody, TVector> : MovementComponent<TCollider, TRigidbody, TVector>
	where TVector : unmanaged
	{
		#region Fields

		[SerializeField]
		private Component _controller;

		[AutoAssign]
		public IGravityHost controller { get; protected set; }

		#endregion
		#region Properties

		public abstract TVector force { get; }
		public abstract TVector up { get; }

		#endregion
		#region Methods

		protected override void Awake()
		{
			base.Awake();

			var pawn = GetComponent<PawnBase<TCollider, TRigidbody, TVector>>() ?? GetComponentInParent<PawnBase<TCollider, TRigidbody, TVector>>();

			if (pawn != null)
			{
				collider = pawn.collider;
				rigidbody = pawn.rigidbody;
			}

			if (_controller != null)
				RefreshController();
			else
				_controller = (Component)controller;
		}

		public void Refresh()
		{
			enabled = controller.shouldUseGravity;
		}

		private void RefreshController()
		{
			if (typeof(IGravityHost).IsAssignableFrom(_controller.GetType()))
			{
				controller = (IGravityHost)_controller;
			}
			else
			{
				Debug.LogWarning($"{_controller.name} is not an IGravityHost and cannot be used to control this GravityComponent.");
				_controller = null;
			}
		}

		#endregion
	}

	#endregion
	#region IGravityHost

	public interface IGravityHost
	{
		bool shouldUseGravity { get; }
	}

	#endregion
}
