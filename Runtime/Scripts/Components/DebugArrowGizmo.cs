
/** DebugArrowGizmo.cs
*
*	Created by LIAM WOFFORD, USA-TX, for the Public Domain.
*
*	Repo: https://github.com/Brungleby/Cuberoot
*	Kofi: https://ko-fi.com/brungleby
*/

#region Includes

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public class DebugArrowGizmo : MonoBehaviour
	{
		#region Fields

		public Color Color = Color.red;

		public Vector3 Offset = Vector3.zero;

		[Range(0f, 1f)]

		public float HeadHight = 0.25f;

		[Min(0f)]

		public float HeadWidth = 25f;

		#endregion
		#region Members
		#endregion
		#region Properties
		#endregion
		#region Methods

		void OnDrawGizmos()
		{
			DebugDraw.DrawArrow(transform.position + Offset, transform.right, Color.red, HeadHight, HeadWidth);
			DebugDraw.DrawArrow(transform.position + Offset, transform.up, Color.green, HeadHight, HeadWidth);
			DebugDraw.DrawArrow(transform.position + Offset, transform.forward, Color.blue, HeadHight, HeadWidth);
		}

		#endregion
	}
}
