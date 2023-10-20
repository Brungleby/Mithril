
/** SensorModule.cs
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
	#region CasterModule

	/// <summary>
	/// This is the base class for any module that creates a sensation by firing a cast to produce a <see cref="Hit"/>.
	///</summary>

	[DefaultExecutionOrder(-10)]

	public abstract class SensorComponent<THit> : MithrilComponent
	where THit : HitBase, new()
	{
		#region Fields

		/// <summary>
		/// Layers that this module will sense.
		///</summary>
		[Tooltip("Layers that this module will sense.")]
		[SerializeField]
		public LayerMask layers;

		#endregion
	}

	#endregion
	#region ShapeCasterModuleBase

	/// <summary>
	/// The base class for a sensor that uses a collider component as the shape for its cast.
	///</summary>

	public abstract class ShapeSensorComponent<TCollider, THit> :
	SensorComponent<THit>, IColliderUser<TCollider>
	where THit : HitBase, new()
	{
#pragma warning disable
		[AutoAssign] public new TCollider collider { get; protected set; }
#pragma warning restore
	}

	#endregion
}
