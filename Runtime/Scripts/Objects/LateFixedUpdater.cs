
/** LateFixedUpdater.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections;
using UnityEngine;

#endregion

namespace Mithril
{
	#region LateFixedUpdater

	/// <summary>
	/// Simple object that sets up a coroutine for its owner. The owner will need to cache and create this object as well as call <see cref="SetupCoroutine"/> on <see cref="MonoBehaviour.OnEnable"/> and implement <see cref="ILateFixedUpdaterComponent"/>.
	///</summary>

	public sealed class LateFixedUpdater : object
	{
		private ILateFixedUpdaterComponent _owner;

		public LateFixedUpdater(MonoBehaviour owner)
		{
			_owner = (ILateFixedUpdaterComponent)owner;
		}

		public void SetupCoroutine()
		{
			((MonoBehaviour)_owner).StartCoroutine(_SetupCoroutine());
		}

		private IEnumerator _SetupCoroutine()
		{
			while (true)
			{
				yield return new WaitForFixedUpdate();
				_owner.LateFixedUpdate();
			}
		}
	}

	#endregion
	#region ILateFixedUpdaterComponent

	public interface ILateFixedUpdaterComponent
	{
		void LateFixedUpdate();
	}

	#endregion
}
