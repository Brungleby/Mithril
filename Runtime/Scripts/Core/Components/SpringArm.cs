
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

	public abstract class SpringArm<TCollider, THit, TShapeInfo> : CasterComponent<TCollider, THit, TShapeInfo>
	where TCollider : Component
	where THit : HitBase, new()
	where TShapeInfo : ShapeInfoBase
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
		protected override THit hitToDraw => hit;

		protected abstract void Update();
	}

	#endregion
	#region SpringArm

	[ExecuteAlways]
	public sealed class SpringArm : SpringArm<Collider, Hit, ShapeInfo>
	{
		private Transform child;
		private float distanceVelocity;

		public float distanceFromOrigin =>
			transform.localPosition.magnitude;

		private void OnValidate()
		{
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
			hit = Sense();
			try { UpdatePosition(); }
			catch (UnassignedReferenceException) { }
			catch (MissingReferenceException) { }
		}

		private void UpdatePosition()
		{
			var currentDistance = child.localPosition.magnitude;
			var targetDistance = hit.distance.Max(minDistance);
			var smoothDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, smoothTime);

#if UNITY_EDITOR
			/**	This conditional prevents stuttering when modifying transform in-editor
			*	and is ignored entirely in builds
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

		protected override Hit Sense_Line()
		{
			return Hit.Linecast(transform.position, transform.forward, maxDistance, layers);
		}

		protected override Hit Sense_Box()
		{
			var info = (BoxInfo)shapeInfo;
			return Hit.BoxCast(
				transform.position,
				transform.rotation,
				info.size / 2f,
				transform.forward,
				maxDistance,
				layers
			);
		}
	}

	#endregion
}
