
/** RemapRange.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using UnityEngine;

#endregion

namespace Mithril
{
	#region RemapRange

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[Serializable]
	public struct RemapRange
	{
		public bool clamp;
		public float inMin;
		public float inMax;
		public float outMin;
		public float outMax;

		public RemapRange(bool clamp, float inMin = 0f, float inMax = 1f, float outMin = 0f, float outMax = 1f)
		{
			this.clamp = clamp;
			this.inMin = inMin;
			this.inMax = inMax;
			this.outMin = outMin;
			this.outMax = outMax;
		}

		public float Evaluate(float value) => value.Remap(inMin, inMax, outMin, outMax, clamp);
	}

	#endregion
}
