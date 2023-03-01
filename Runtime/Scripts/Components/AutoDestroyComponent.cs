
/** AutoDestroyComponent.cs
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
			DestroyTimeline.OnCease.AddListener(() =>
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
