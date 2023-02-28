
/** Geometry.cs
*
*   Created by LIAM WOFFORD, USA-TX, for the Public Domain.
*
*   Repo: https://github.com/Brungleby/Cuberoot
*   Kofi: https://ko-fi.com/brungleby
*/

#region Includes

using UnityEngine;

#endregion

namespace Cuberoot
{
	#region (class) Geometry

	/// <summary>
	/// A static class that contains functions that adds geometrical support various <see cref="Collider"/>s.
	///</summary>

	public static class Geometry
	{
		#region Generic

		public static bool IsConvex(this Collider collider) =>
			collider.GetType() != typeof(MeshCollider) || ((MeshCollider)collider).convex;

		#endregion

		#region Box



		#endregion

		#region Capsule

		/// <returns>
		/// Half the given <paramref name="capsule"/>'s hight.
		///</returns>

		public static float GetHalfHeight(this CapsuleCollider capsule) =>
			capsule.height / 2f;

		/// <returns>
		/// Half the given <paramref name="capsule"/>'s hight, not including the spherical cap at the end.
		///</returns>

		public static float GetHalfHeightUncapped(this CapsuleCollider capsule) =>
			System.Math.Max((capsule.height / 2f) - capsule.radius, 0f);

		/// <returns>
		/// The <paramref name="capsule"/>'s relative right, forward, or up vector depending on the <see cref="CapsuleCollider.direction"/>.
		///</returns>

		public static Vector3 GetDirectionAxis(this CapsuleCollider capsule)
		{
			switch (capsule.direction)
			{
				case 0: /**	X-Axis	*/
					return capsule.transform.right;
				case 1: /**	Y-Axis	*/
					return capsule.transform.up;
				case 2: /**	Z-Axis	*/
					return capsule.transform.forward;
				default:
					throw new UnityException($"Capsule direction is not valid. Check your index when querying the capsule on {capsule.gameObject.name}");
			}
		}

		/// <returns>
		/// The world position of the head (top) of the <paramref name="capsule"/>.
		///</returns>

		public static Vector3 GetHeadPosition(this CapsuleCollider capsule) =>
			capsule.transform.position + capsule.center + capsule.GetDirectionAxis() * capsule.GetHalfHeight();

		/// <returns>
		/// The world position of the Tail (bottom) of the <paramref name="capsule"/>.
		///</returns>

		public static Vector3 GetTailPosition(this CapsuleCollider capsule) =>
			capsule.transform.position + capsule.center - capsule.GetDirectionAxis() * capsule.GetHalfHeight();

		/// <returns>
		/// The world position of the uncapped head (upper sphere's origin) of the <paramref name="capsule"/>.
		///</returns>

		public static Vector3 GetHeadPositionUncapped(this CapsuleCollider capsule) =>
			capsule.transform.position + capsule.center + capsule.GetDirectionAxis() * capsule.GetHalfHeightUncapped();

		/// <returns>
		/// The world position of the uncapped Tail (lower sphere's origin) of the <paramref name="capsule"/>.
		///</returns>

		public static Vector3 GetTailPositionUncapped(this CapsuleCollider capsule) =>
			capsule.transform.position + capsule.center - capsule.GetDirectionAxis() * capsule.GetHalfHeightUncapped();

		#endregion
	}

	#endregion
}
