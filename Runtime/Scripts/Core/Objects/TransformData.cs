
/** TransformData.cs
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
	#region TransformData

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public struct TransformData
	{
		public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		public TransformData(Vector3 position, Quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;
			scale = Vector3.one;
		}

		public TransformData(Vector3 position)
		{
			this.position = position;
			rotation = Quaternion.identity;
			scale = Vector3.one;
		}

		public TransformData(Transform transform, bool isLocalSpace)
		{
			position = isLocalSpace ? transform.localPosition : transform.position;
			rotation = isLocalSpace ? transform.localRotation : transform.rotation;
			scale = isLocalSpace ? transform.localScale : transform.lossyScale;
		}

		// public TransformData()
		// {
		// 	position = Vector3.zero;
		// 	rotation = Quaternion.identity;
		// 	scale = Vector3.one;
		// }

		public readonly Vector3 position;
		public readonly Quaternion rotation;
		public readonly Vector3 scale;
	}

	#endregion
	#region TransformData2D



	#endregion
}
