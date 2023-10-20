
/** ICollider.cs
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
	#region IColliderUser

	public interface IColliderUser<TCollider>
	{
		TCollider collider { get; }
	}

	#endregion
}
