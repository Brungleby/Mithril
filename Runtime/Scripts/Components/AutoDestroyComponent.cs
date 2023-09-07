
/** AutoDestroyComponent.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#endregion

namespace Mithril
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class AutoDestroyComponent : MonoBehaviour
	{
		#region Fields

		public GameObject InstantDestroyObject;

		public Timer DestroyTimeline;

		#endregion
		#region Members
		#endregion
		#region Properties
		#endregion
		#region Methods

		private void Awake()
		{
			DestroyTimeline.onCease.AddListener(() =>
			{
				gameObject.SetActive(false);
			});
		}

		private void Update()
		{
			DestroyTimeline.Update();
		}

		public void Destroy()
		{
			InstantDestroyObject.SetActive(false);
			DestroyTimeline.Start();
		}

		#endregion
	}
}
