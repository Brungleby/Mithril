
/** SurfaceFilter.cs
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

namespace Cuberoot
{
	/// <summary>
	/// Add this component to any <see cref="GameObject"/> with a Collider or Collider2D to apply a <see cref="Surface"/>'s properties to it.
	///</summary>

	public class SurfaceFilter : MonoBehaviour
	{
		#region Public Fields

		#region Surface

		/// <summary>
		/// The surface object that defines this collider's physical properties.
		///</summary>

		[Tooltip("The surface object that defines this collider's physical properties.")]
		[SerializeField]

		private Surface _Surface;

		/// <inheritdoc cref="_Surface"/>

		public Surface Surface => _Surface;

		#endregion

		#endregion
	}
}
