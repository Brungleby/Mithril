
/** DestroyOnTouchComponent.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#endregion

namespace Mithril
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class DestroyOnTouchComponent : MonoBehaviour
	{
		#region Fields

		public Transform Respawn;

		#endregion
		#region Members



		#endregion
		#region Properties



		#endregion
		#region Methods

		private void OnTriggerEnter(Collider other)
		{
			if (Respawn != null)
			{
				var __rigidbody = other.GetComponent<Rigidbody>();
				if (__rigidbody != null)
				{
					__rigidbody.MovePosition(Respawn.position);
					__rigidbody.MoveRotation(Respawn.rotation);
					__rigidbody.velocity = Vector3.zero;
				}
				else
				{
					other.transform.position = Respawn.position;
					other.transform.rotation = Respawn.rotation;
				}
			}
			else
			{
				Destroy(other.gameObject);
			}
		}

		#endregion
	}
}
