
/** Geometry.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril
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
			return capsule.direction switch
			{
				0 => capsule.transform.right,
				1 => capsule.transform.up,
				2 => capsule.transform.forward,
				_ => throw new UnityException($"Capsule direction is not valid. Check your index when querying the capsule on {capsule.gameObject.name}"),
			};
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

		/// <returns>
		/// A valid rotation to set the capsule defined by the given two points to.
		///</returns>

		public static Quaternion GetCapsuleRotation(Vector3 point1, Vector3 point2) =>
			Quaternion.LookRotation(point1 - point2, Vector3.up) *
			Quaternion.FromToRotation(Vector3.forward, Vector3.up)
		;

		#endregion
	}

	#endregion
}
