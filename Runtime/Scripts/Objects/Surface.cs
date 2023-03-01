
/** Surface.cs
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
	/// A Surface is Cuberoot's custom Physical Material class. It contains simple physics information for pawns to use when moving and allows for other things such as colliders with associated sound effects.
	///</summary>

	[CreateAssetMenu(fileName = "New Surface", menuName = "Cuberoot/Physics/Surface", order = 50)]

	public class Surface : ScriptableObject
	{
		#region Fields

		#region (field) ID

		/// <summary>
		/// This string is used to identify this surface. Each <see cref="Surface.ID"/> should be unique.
		///</summary>

		[Tooltip("This string is used to identify this surface. Each Surface.ID should be unique.")]
		[SerializeField]

		private string _ID = default(string);

		/// <inheritdoc cref="_ID"/>

		public string ID => _ID;

		#endregion

		#region (field) FrictionScale

		/// <summary>
		/// Pawns with a FrictionMovement component will have their acceleration and deceleration multiplied by this amount.
		///</summary>

		[Tooltip("Pawns with a FrictionMovement component will have their acceleration and deceleration multiplied by this amount.")]
		[SerializeField]

		private float _FrictionScale = 1f;

		/// <inheritdoc cref="_FrictionScale"/>

		public float FrictionScale => _FrictionScale;

		#endregion
		#region (field) FrictionSpeedScale

		/// <summary>
		/// The "viscosity" of this surface, pawns with a <see cref="WalkMovement"/> component will have their <see cref="WalkMovement.MaxWalkSpeed_cx"/> multiplied by this amount.
		///</summary>

		[Tooltip("The \"viscosity\" of this surface, pawns with a WalkMovement component will have their Max Walk Speed multiplied by this amount.")]
		[SerializeField]

		private float _FrictionSpeedScale = 1f;

		/// <inheritdoc cref="_FrictionSpeedScale"/>

		public float FrictionSpeedScale => _FrictionSpeedScale;

		#endregion

		#endregion
	}

	public static class SurfaceExtensions
	{
		#region GetSurface

		/// <returns>
		/// The Surface object (if one exists) associated with the given <paramref name="gameObject"/>.
		///</returns>

		public static Surface GetSurface(this GameObject gameObject)
		{
			SurfaceFilter filter = gameObject.GetComponent<SurfaceFilter>();

			if (filter != null)
				return filter.Surface;
			return default(Surface);
		}

		/// <returns>
		/// The Surface object (if one exists) associated with the given <paramref name="component"/>.
		///</returns>

		public static Surface GetSurface(this Component component) => GetSurface(component.gameObject);

		#endregion
	}
}
