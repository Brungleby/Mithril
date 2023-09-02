
/** Debug.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/


#region Includes

using UnityEngine;
using UnityEditor;
// using UnityEditor

#endregion

namespace Mithril
{
	/// <summary>
	/// A static collection of debug functions.
	///</summary>

	public static class DebugDraw
	{
		public static readonly Color BLOCKING_COLOR = Color.red;
		public static readonly Color CAST_COLOR = Color.cyan;

		#region Drawing Methods

		#region DrawPoint

		/// <summary>
		/// Draws a debug point in 3D space.
		///</summary>
		/// <param name="origin">
		/// The location at which to draw the point.
		///</param>
		/// <param name="pointSize">
		/// The size at which to draw the point.
		///</param>

		public static void DrawPoint(Vector3 origin, float pointSize = 0.1f)
		{
			DrawPoint(origin, Color.white, pointSize);
		}

		/// <inheritdoc cref="DrawPoint(Vector3, float)"/>
		/// <param name="color">
		/// The color in which the point will be drawn.
		///</param>

		public static void DrawPoint(Vector3 origin, Color color, float pointSize = 0.1f)
		{
			Gizmos.color = color;
			Gizmos.DrawSphere(origin, pointSize);
		}

		#endregion

		#region DrawArrow

		/// <summary>
		/// Draws an arrow at a given <paramref name="origin"/> that points in a specified direction.
		///</summary>
		/// <param name="origin">
		/// The position at which to draw the tail of the arrow.
		///</param>
		/// <param name="direction">
		/// The position at which to draw the head of the arrow, relative to the <paramref name="origin"/>.
		///</param>
		/// <param name="headHight">
		/// The hight of the arrowhead, represented as a percentage of the length of the arrow.
		///</param>
		/// <param name="headWidth">
		/// The width of the arrowhead, represented as a percentage of a 90-degree angle.
		///</param>

		public static void DrawArrow(Vector3 origin, Vector3 direction, float headHight = 0.25f, float headWidth = 25f)
		{
			if (direction.Approx(Vector3.zero)) return;

			Gizmos.DrawLine(origin, origin + (direction * (1f - headHight)));

			Vector3 location = origin + direction;
			Quaternion rotation;
			if (Math.Approx(direction, Vector3.zero))
				rotation = Quaternion.identity;
			else
				rotation = Quaternion.LookRotation(-direction.normalized, Vector3.up);
			Gizmos.matrix = Matrix4x4.TRS(location, rotation, Vector3.one);

			Gizmos.DrawFrustum(Vector3.zero, headWidth, direction.magnitude * headHight, 0.0f, 1.0f);
			Gizmos.matrix = Matrix4x4.identity;
		}

		/// <inheritdoc cref="DrawArrow(Vector3, Vector3, float, float)"/>
		/// <param name="color">
		/// Specifies which color to draw the arrow as.
		///</param>

		public static void DrawArrow(Vector3 origin, Vector3 direction, Color color, float headHight = 0.25f, float headWidth = 25f)
		{
			Gizmos.color = color;
			DrawArrow(origin, direction);
		}

		#endregion
		#region DrawWireCapsule

		public static void DrawWireCapsule(Vector3 point1, Vector3 point2, Vector3 forward, float radius, float height, Color color)
		{
			var midpoint = Math.Midpoint(point1, point2);
			var rotation = Quaternion.LookRotation(forward, point2 - point1);

			DrawWireCapsule(midpoint, rotation, radius, height, color);
		}

		public static void DrawWireCapsule(Vector3 position, Quaternion rotation, float radius, float height, Color color)
		{
#if UNITY_EDITOR
			Handles.color = color;

			Matrix4x4 angleMatrix = Matrix4x4.TRS(position, rotation, Handles.matrix.lossyScale);
			using (new Handles.DrawingScope(angleMatrix))
			{
				var pointOffset = (height - (radius * 2)) / 2;

				//draw sideways
				Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
				Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
				Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
				Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);
				//draw frontways
				Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
				Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
				Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
				Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);
				//draw center
				Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
				Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);
			}
#endif
		}

		public static void DrawCapsuleCast(this Hit hit, Vector3 forward, Vector3 up, float radius, float height)
		{
			Gizmos.color = CAST_COLOR;
			DrawWireCapsule
			(
				hit.origin + up * height / 2f,
				hit.origin - up * height / 2f,
				forward,
				radius,
				height,
				CAST_COLOR
			);

			Vector3 closestPoint = hit.isBlocked ? hit.adjustmentPoint : hit.target;
			Gizmos.DrawLine(hit.origin, closestPoint);
			Color __resultColor = hit.isBlocked ? BLOCKING_COLOR : CAST_COLOR;
			Gizmos.color = __resultColor;

			DrawWireCapsule
			(
				closestPoint + up * height / 2f,
				closestPoint - up * height / 2f,
				forward,
				radius,
				height,
				__resultColor
			);

			if (hit.isBlocked)
				Gizmos.DrawLine(hit.adjustmentPoint, hit.target);
		}

		#endregion
		#region DrawLinecast

		/// <summary>
		/// Draws a representation of a linecast after it has been performed.
		///</summary>
		/// <param name="origin">
		/// The start position used to define the performed linecast.
		///</param>
		/// <param name="target">
		/// The end position used to define the performed linecast.
		///</param>
		/// <param name="hit">
		/// The result RaycastHit obtained after performing the linecast.
		///</param>
		/// <param name="pointSize">
		/// The size to draw the point to represent any hit position.
		///</param>

		public static void DrawLinecast(this RaycastHit hit, Vector3 origin, Vector3 target, float pointSize = 0.1f)
		{
			if (hit.IsBlocked())
			{
				Gizmos.color = CAST_COLOR;
				Gizmos.DrawLine(origin, hit.point);

				Gizmos.color = BLOCKING_COLOR;
				Gizmos.DrawSphere(hit.point, pointSize);

				Gizmos.DrawLine(hit.point, target);
			}
			else
			{
				Gizmos.color = CAST_COLOR;
				Gizmos.DrawLine(origin, target);
			}
		}

		/// <inheritdoc cref="DrawLinecast(RaycastHit, Vector3, Vector3, float)"/>
		/// <param name="ray">
		/// The ray used to define the performed linecast.
		///</param>
		/// <param name="distance">
		/// The float distance used to define the performed linecast.
		///</param>

		public static void DrawLinecast(this RaycastHit hit, Ray ray, float distance, float pointSize = 0.1f)
		{
			hit.DrawLinecast(ray.origin, ray.origin + ray.direction * distance, pointSize);
		}

		/// <inheritdoc cref="DrawLinecast(RaycastHit, Vector3, Vector3, float)"/>
		/// <param name="hit">
		/// The result <see cref="HitInfo"/> obtained after performing the linecast.
		///</param>

		public static void DrawLinecast(this Hit hit, float pointSize = 0.1f)
		{
			try
			{
				hit.hit.DrawLinecast(hit.origin, hit.target, pointSize);
			}
			catch { }
		}

		#endregion

		#region DrawSphereCast

		/// <summary>
		/// Draws a representation of a spherecast after it has been performed.
		///</summary>
		/// <param name="origin">
		/// The start position used to define the performed spherecast.
		///</param>
		/// <param name="target">
		/// The end position used to define the performed spherecast.
		///</param>
		/// <param name="sphereRadius">
		/// The sphere radius used to define the performed spherecast.
		///</param>
		/// <param name="hitPoint">
		/// The position of the hit point resulting from the performed spherecast.
		///</param>

		public static void DrawSphereCast(Vector3 origin, Vector3 target, float sphereRadius, Vector3 hitPoint)
		{
			Gizmos.color = CAST_COLOR;
			Gizmos.DrawWireSphere(origin, sphereRadius);

			if (!Math.Approx(hitPoint, target, 0.01f))
			{
				Gizmos.DrawLine(origin, hitPoint);

				Gizmos.color = BLOCKING_COLOR;
				Gizmos.DrawWireSphere(target, sphereRadius);
				Gizmos.DrawLine(hitPoint, target);

				Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
				Gizmos.DrawSphere(hitPoint, sphereRadius);
			}
			else
			{
				Gizmos.DrawLine(origin, target);
				Gizmos.DrawWireSphere(target, sphereRadius);
			}
		}

		/// <inheritdoc cref="DrawSphereCast(Vector3, Vector3, float, Vector3)"/>
		/// <param name="hit">
		/// The result <see cref="RaycastHit"/> obtained after performing the spherecast.
		///</param>

		public static void DrawSphereCast(this RaycastHit hit, Vector3 origin, Vector3 target, float sphereRadius)
		{
			DrawSphereCast(origin, target, sphereRadius, hit.point);
		}

		/// <inheritdoc cref="DrawSphereCast(Vector3, Vector3, float, Vector3)"/>
		/// <param name="hit">
		/// The result <see cref="HitInfo"/> obtained after performing the spherecast.
		///</param>

		public static void DrawSphereCast(this Hit hit, float radius)
		{
			Gizmos.color = CAST_COLOR;
			Gizmos.DrawWireSphere(hit.origin, radius);

			Vector3 closestPoint = hit.isBlocked ? hit.adjustmentPoint : hit.target;

			Gizmos.DrawLine(hit.origin, closestPoint);

			Gizmos.color = hit.isBlocked ? BLOCKING_COLOR : CAST_COLOR;
			Gizmos.DrawWireSphere(closestPoint, radius);

			if (hit.isBlocked)
			{
				Gizmos.DrawLine(hit.adjustmentPoint, hit.target);
			}

		}

		#endregion

		#endregion
	}
}
