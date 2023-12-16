
/** SpringArm.cs
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
	#region SpringArm<THit>

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public abstract class SpringArm<TCollider, THit> : CasterComponent<TCollider, THit>
	where TCollider : Component
	{
		[Min(0f)]
		[SerializeField]
		private float _minDistance = 0f;
		public float minDistance { get => _minDistance; set => _minDistance = value.Clamp(0f, maxDistance); }

		[Min(0.0001f)]
		[SerializeField]
		private float _maxDistance = 1f;
		public float maxDistance { get => _maxDistance; set => _maxDistance = value.Max(0.0001f); }

		[Min(0f)]
		[SerializeField]
		private float _smoothTime = 0f;
		public float smoothTime { get => _smoothTime; set => _smoothTime = value.Max(); }

		public THit hit { get; protected set; }

		protected abstract void Update();
	}

	#endregion
	#region SpringArm

	[ExecuteAlways]
	public sealed class SpringArm : SpringArm<Collider, RaycastHit>
	{
		private Transform child;
		private float distanceVelocity;
		private HitPool hitPool = new(1);

		public float distanceFromOrigin =>
			transform.localPosition.magnitude;

		protected override void OnValidate()
		{
			base.OnValidate();

			if (transform.childCount == 1)
			{
				child = transform.GetChild(0);
			}
			else
			{
#pragma warning disable
				child = new GameObject($"[SpringArm] {name}").transform;
				child.gameObject.layer = gameObject.layer;
				child.parent = transform;
				foreach (var iChild in transform.GetChildren())
					iChild.parent = child;
#pragma warning restore
			}

			base.Awake();
		}

		protected override void Awake()
		{
			base.Awake();

			child = transform.GetChild(0);
		}

		protected override void Update()
		{
			Sense();
			UpdatePosition();
		}

		private void UpdatePosition()
		{
			var currentDistance = child.localPosition.magnitude;
			var targetDistance = hitPool.blocked ? hit.distance.Max(minDistance) : maxDistance;
			var smoothDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, smoothTime);

#if UNITY_EDITOR
			/**	This conditional prevents stuttering when modifying transform in-editor
			*	and is ignored entirely in non-editor builds
			*/

			Vector3 finalPosition;
			if (Application.isPlaying)
				finalPosition = Vector3.forward * smoothDistance;
			else
				finalPosition = Vector3.forward * targetDistance;
#else
			Vector3 finalPosition = Vector3.forward * smoothDistance;
#endif

			child.localPosition = finalPosition;
		}

		protected override void Sense_Line()
		{
			hitPool.LineCast(transform.position, transform.forward, maxDistance, layers);
			hit = hitPool.nearest;
		}

		protected override void Sense_Box()
		{
			hitPool.BoxCast((BoxCollider)collider, transform.forward, maxDistance, layers);
			hit = hitPool.nearest;
		}
	}

	#endregion
}
