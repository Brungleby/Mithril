using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mithril
{
	public class ArrowComponent : MonoBehaviour
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
}
