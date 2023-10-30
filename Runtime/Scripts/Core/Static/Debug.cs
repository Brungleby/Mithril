
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
		public static readonly Color BLOCK_COLOR = Color.red;
		public static readonly Color CAST_COLOR = Color.green;

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

			Gizmos.DrawLine(origin, origin + direction * (1f - headHight));

			if (headHight <= 0f) return;

			Vector3 location = origin + direction;
			Quaternion rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

			Gizmos.matrix = Matrix4x4.TRS(location, rotation, Vector3.one);

			Gizmos.DrawFrustum(Vector3.zero, headWidth, direction.magnitude * -headHight, 0.0f, 1.0f);

			Gizmos.matrix = Matrix4x4.identity;
		}

		/// <inheritdoc cref="DrawArrow(Vector3, Vector3, float, float)"/>
		/// <param name="color">
		/// Specifies which color to draw the arrow as.
		///</param>

		public static void DrawArrow(Vector3 origin, Vector3 direction, Color color, float headHight = 0.25f, float headWidth = 25f)
		{
			Gizmos.color = color;
			DrawArrow(origin, direction, headHight, headWidth);
		}

		#endregion
		#region DrawWireBox

		public static void DrawWireBox(Vector3 position, Quaternion rotation, Vector3 size, Color color)
		{
			Handles.color = color;

			Matrix4x4 angleMatrix = Matrix4x4.TRS(position, rotation, Handles.matrix.lossyScale);
			using (new Handles.DrawingScope(angleMatrix))
			{
				Handles.DrawWireCube(Vector3.zero, size);
			}
		}

		#endregion
		#region DrawWireCapsule

		public static void DrawWireCapsule(Vector3 point1, Vector3 point2, float radius, Color color)
		{
			var midpoint = Math.Midpoint(point1, point2);

			var lookRot = Quaternion.LookRotation(point1 - point2, Vector3.up);
			var rotRot = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
			var rotation = lookRot * rotRot;

			var height = (point1 - point2).magnitude;

			DrawWireCapsule(midpoint, rotation, radius, height, color);
		}

		public static void DrawWireCapsule(Vector3 position, Quaternion rotation, float radius, float height, Color color)
		{
#if UNITY_EDITOR
			Handles.color = color;

			Matrix4x4 angleMatrix = Matrix4x4.TRS(position, rotation, Handles.matrix.lossyScale);
			using (new Handles.DrawingScope(angleMatrix))
			{
				var pointOffset = (height - (radius * 2f)) * 0.5f;

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

		#endregion

		#endregion
	}
}
