
/** PusherVolume.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Mithril.Pawn
{
	#region PusherVolume<>

	/// <summary>
	/// This is a volume that pushes rigidbodies within its collider. Good for simulating strong winds.
	///</summary>

	public abstract class PusherVolume<TCollider, TRigidbody, TVector> : MithrilComponent, IColliderUser<TCollider>
	where TVector : unmanaged
	{
		#region Members

		public List<IPusherVolumeUser<TVector>> affectedUsers { get; private set; }
		public List<TRigidbody> affectedBodies { get; private set; }
#pragma warning disable
		[AutoAssign] public new TCollider collider { get; protected set; }
#pragma warning restore
		#endregion
		#region Properties

		public abstract TVector force { get; }

		#endregion
		#region Methods

		protected override void Awake()
		{
			affectedUsers = new List<IPusherVolumeUser<TVector>>();
			affectedBodies = new List<TRigidbody>();

			base.Awake();
		}

		protected virtual void FixedUpdate()
		{
			foreach (var iUser in affectedUsers)
				iUser.ProcessPusherVolumeForce(GetForceAtPosition(iUser.position));
		}

		public abstract TVector GetForceAtPosition(in TVector position);

		#endregion
	}

	#endregion
	#region PusherVolume

	public abstract class PusherVolume<TCollider> : PusherVolume<TCollider, Rigidbody, Vector3>
	where TCollider : Collider
	{
		protected override void FixedUpdate()
		{
			base.FixedUpdate();

			foreach (var iBody in affectedBodies)
				iBody.AddForce(GetForceAtPosition(iBody.position), ForceMode.Force);
		}

		protected virtual void OnTriggerEnter(TCollider other)
		{
			var __user = other.GetComponent<IPusherVolumeUser<Vector3>>();

			if (__user != null)
			{
				affectedUsers.Add(__user);
				return;
			}

			var __body = other.GetComponent<Rigidbody>();

			if (__body != null)
			{
				affectedBodies.Add(__body);
				return;
			}
		}

		protected virtual void OnTriggerExit(TCollider other)
		{
			var __user = other.GetComponent<IPusherVolumeUser<Vector3>>();

			if (__user != null)
			{
				affectedUsers.Remove(__user);
				return;
			}

			var __body = other.GetComponent<Rigidbody>();

			if (__body != null)
			{
				affectedBodies.Remove(__body);
				return;
			}
		}

		private void OnDrawGizmos()
		{
			DebugDraw.DrawArrow(transform.position, force, Color.green);
		}
	}

	#endregion
	#region IPusherVolumeUser

	public interface IPusherVolumeUser<TVector>
	where TVector : unmanaged
	{
		TVector position { get; }
		void ProcessPusherVolumeForce(in TVector force);
	}

	#endregion
}
