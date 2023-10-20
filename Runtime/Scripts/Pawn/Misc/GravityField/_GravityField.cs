
/** GravityField.cs
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
	#region GravityField<TVector>

	/// <summary>
	/// Base class that defines a volume of gravity. By default, all GravityFields will have the strength of <see cref="Physics.gravity"/>.
	///</summary>

	public abstract class GravityField<TVector> : MithrilComponent
	{
		#region Fields

		public float strengthScale = 1f;

		/// <summary>
		/// The force of GravityFields using different channels will be compounded. Those using the same channel will be either filtered by priority or averaged.
		///</summary>
		[Tooltip("The force of GravityFields using different channels will be compounded. Those using the same channel will be either filtered by priority or averaged.")]
		[Range(0, 8)]
		[SerializeField]
		private byte _channel;
		public byte channel { get => _channel; set => System.Math.Clamp(value, (byte)0, (byte)8); }

		/// <summary>
		/// If two GravityFields on the same channel have differing priorities, only the one with the highest priority will be considered. If they have the same priority, they will be averaged.
		///</summary>
		[Tooltip("If two GravityFields on the same channel have differing priorities, only the one with the highest priority will be considered. If they have the same priority, they will be averaged.")]
		public int priority;

		#endregion
		#region Methods

		public abstract TVector GetForceAtPosition(TVector position);

		#endregion
	}

	#endregion
	#region GravityField<TCollider, TVector>

	public abstract class GravityField<TCollider, TVector> : GravityField<TVector>
	{
		#region Members
#pragma warning disable
		[AutoAssign] public new TCollider collider { get; protected set; }
#pragma warning restore
		#endregion
	}

	#endregion
}
