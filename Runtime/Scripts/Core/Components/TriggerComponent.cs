
/** TriggerComponent.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Mithril
{
	#region TriggerComponent<TCollider>

	public abstract class TriggerComponent<TCollider> : MithrilComponent
	where TCollider : Component
	{
		public readonly HashSet<TCollider> overlaps = new();

		protected virtual void OnTriggerEnter(Collider other) => overlaps.Add((TCollider)(Component)other);
		protected virtual void OnTriggerExit(Collider other) => overlaps.Remove((TCollider)(Component)other);
		protected virtual void OnTriggerEnter2D(Collider other) => overlaps.Add((TCollider)(Component)other);
		protected virtual void OnTriggerExit2D(Collider other) => overlaps.Remove((TCollider)(Component)other);

		public bool IsOverlapping(TCollider collider) =>
			overlaps.Contains(collider);
	}

	#endregion
	#region TriggerComponent

	public sealed class TriggerComponent : TriggerComponent<Collider>
	{
		public UnityEvent<Collider> onTriggerEnter;
		public UnityEvent<Collider> onTriggerExit;

		protected override void OnTriggerEnter(Collider other)
		{
			base.OnTriggerEnter(other);
			onTriggerEnter.Invoke(other);
		}

		protected override void OnTriggerExit(Collider other)
		{
			base.OnTriggerExit(other);
			onTriggerExit.Invoke(other);
		}
	}

	#endregion
}
