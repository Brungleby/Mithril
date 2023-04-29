
/** CollisionEvents.cs
*
*	Created by LIAM WOFFORD, USA-TX.
*	(c) 2023, CUBEROOT SOFTWARE, LLC.
**/

#region Includes

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Mithril;

#endregion

namespace Dreamwalker
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class CollisionEvents : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private string[] _FilterTags;
		public HashSet<string> FilterTags;

		public UnityEvent<Collision> _OnCollisionEnter;
		public UnityEvent<Collision> _OnCollisionExit;
		public UnityEvent<Collider> _OnTriggerEnter;
		public UnityEvent<Collider> _OnTriggerExit;

		#endregion
		#region Members
		#endregion
		#region Properties
		#endregion
		#region Methods

		private void OnValidate()
		{
			FilterTags = new HashSet<string>();
			FilterTags.AddAll(_FilterTags);
		}

		private void Awake() => OnValidate();

		public bool FilterTagsContainsOrIsEmpty(in string tag) =>
			FilterTags.Count == 0 || FilterTags.Contains(tag);

		private void OnCollisionEnter(Collision other)
		{
			if (FilterTagsContainsOrIsEmpty(other.gameObject.tag))
				_OnCollisionEnter.Invoke(other);
		}

		private void OnCollisionExit(Collision other)
		{
			if (FilterTagsContainsOrIsEmpty(other.gameObject.tag))
				_OnCollisionExit.Invoke(other);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (FilterTagsContainsOrIsEmpty(other.gameObject.tag))
				_OnTriggerEnter.Invoke(other);
		}

		private void OnTriggerExit(Collider other)
		{
			if (FilterTagsContainsOrIsEmpty(other.gameObject.tag))
				_OnTriggerExit.Invoke(other);
		}

		#endregion
	}
}
