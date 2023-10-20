
/** SimplePusherVolume.cs
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
	#region SimplePusherVolume

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class SimplePusherVolume : PusherVolume<Collider>
	{
		[SerializeField]

		private Vector3 _force = Vector3.right;

		public override Vector3 force => transform.rotation * _force;
		public override Vector3 GetForceAtPosition(in Vector3 position) => force;

		public void SetForce(Vector3 value) => _force = value;
	}

	#endregion
}
