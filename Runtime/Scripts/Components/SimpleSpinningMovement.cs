
/** SimpleSpinningMovement.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// This is a simple script that makes its gameObject spin around along an axis.
	///</summary>
	public class SimpleSpinningMovement : MonoBehaviour
	{
		#region PerAxisSpeed

		/// <summary>
		/// How fast to spin along each axis, relatively speaking.
		///</summary>
		[Tooltip("How fast to spin along each axis, relatively speaking.")]
		public Vector3 PerAxisSpeed = Vector3.up;

		#endregion
		#region SpeedMultiplier

		/// <summary>
		/// Multiplier for how fast we will spin.
		///</summary>
		[Tooltip("Multiplier for how fast we will spin.")]
		public float SpeedMultiplier = 100.0f;

		#endregion
		#region Update

		void Update()
		{
			Vector3 __offset = PerAxisSpeed * SpeedMultiplier * Time.deltaTime;

			transform.localEulerAngles += __offset;
		}

		#endregion
	}
}
