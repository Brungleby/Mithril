
/** Arrow.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril
{
	#region Arrow

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class Arrow : MithrilComponent
	{
		public bool enableDrawAlways = true;
		public bool scaleWithViewport = true;
		public Color color = Color.cyan;
		public Vector3 direction = Vector3.forward;

		private void OnDrawGizmos()
		{
			if (enableDrawAlways)
				Draw();
		}

		private void OnDrawGizmosSelected()
		{
			if (!enableDrawAlways)
				Draw();
		}

		private void Draw()
		{
			float scale;
			if (scaleWithViewport)
				scale = Camera.current.WorldToViewportPoint(transform.position).z * 0.25f;
			else
				scale = 1f;

			DebugDraw.DrawArrow(transform.position, transform.rotation * direction * scale, color);
		}
	}

	#endregion
}
