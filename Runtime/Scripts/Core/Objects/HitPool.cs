
/** HitPool.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Mithril
{
	#region HitPoolBase

	public abstract class HitPoolBase : object
	{
		/// <summary>
		/// The total number of hits detected; this can never be larger than <see cref="size"/>.
		///</summary>
		private int _length = 0;
		/// <inheritdoc cref="_length"/>
		public int length { get => _length; protected set => _length = value.Min(size); }

		/// <summary>
		/// Whether or not any hits were detected.
		///</summary>
		public bool blocked => length != 0;

		/// <summary>
		/// Maximmum number of hits it's possible to detect.
		///</summary>
		public abstract int size { get; }

		public void Clear()
		{
			length = 0;
		}
	}

	#endregion
	#region HitPool<T>

	public abstract class HitPool<T> : HitPoolBase, IEnumerable<T>, IEnumerator<T>, IComparer<T>
	{
		protected T[] hits { get; private set; }
		private int _iterator = -1;

		public sealed override int size => hits.Length;

		public T this[int i] => hits[i];

		public HitPool()
		{
			hits = Array.Empty<T>();
		}
		public HitPool(int size)
		{
			hits = new T[size];
		}

		public T nearest => blocked ? hits[0] : default;

		public abstract int Compare(T x, T y);
		protected abstract void Refresh();

		public IEnumerator<T> GetEnumerator() => this;
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public T Current => hits[_iterator];
		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			_iterator++;
			return _iterator < length;
		}
		public void Reset()
		{
			_iterator = -1;
		}
		public void Dispose()
		{
			Reset();
		}
	}

	#endregion
	#region HitPool

	public sealed class HitPool : HitPool<RaycastHit>
	{
		public HitPool() : base() { }
		public HitPool(int bufferSize) : base(bufferSize) { }

		public override int Compare(RaycastHit x, RaycastHit y) =>
			x.distance.CompareTo(y.distance);

		protected override void Refresh()
		{
			Array.Sort(hits, 0, length, this);
		}

		#region LineCast

		public void LineCast(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.RaycastNonAlloc(origin, direction, hits, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void LineCast(Ray ray, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.RaycastNonAlloc(ray, hits, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void LineCast(Vector3 origin, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var delta = target - origin;
			length = Physics.RaycastNonAlloc(origin, delta.normalized, hits, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}

		#endregion
		#region BoxCast

		public void BoxCast(Vector3 origin, Quaternion rotation, Vector3 halfExtents, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.BoxCastNonAlloc(origin, halfExtents, direction, hits, rotation, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void BoxCast(Vector3 origin, Quaternion rotation, Vector3 halfExtents, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var delta = target - origin;
			length = Physics.BoxCastNonAlloc(origin, halfExtents, delta.normalized, hits, rotation, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void BoxCast(Vector3 origin, Vector3 halfExtents, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.BoxCastNonAlloc(origin, halfExtents, direction, hits, Quaternion.identity, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void BoxCast(Vector3 origin, Vector3 halfExtents, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var delta = target - origin;
			length = Physics.BoxCastNonAlloc(origin, halfExtents, delta.normalized, hits, Quaternion.identity, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void BoxCast(Bounds bounds, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.BoxCastNonAlloc(bounds.center, bounds.extents, direction, hits, Quaternion.identity, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void BoxCast(Bounds bounds, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var delta = target - bounds.center;
			length = Physics.BoxCastNonAlloc(bounds.center, bounds.extents, delta.normalized, hits, Quaternion.identity, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void BoxCast(BoxCollider box, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.BoxCastNonAlloc(box.transform.position + box.center, box.size * 0.5f, direction, hits, box.transform.rotation, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void BoxCast(BoxCollider box, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var origin = box.transform.position + box.center;
			var delta = target - origin;
			length = Physics.BoxCastNonAlloc(origin, box.size * 0.5f, delta.normalized, hits, box.transform.rotation, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void BoxCast(Vector3 origin, BoxCollider box, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.BoxCastNonAlloc(origin, box.size * 0.5f, direction, hits, box.transform.rotation, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void BoxCast(Vector3 origin, BoxCollider box, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var delta = target - origin;
			length = Physics.BoxCastNonAlloc(origin, box.size * 0.5f, delta.normalized, hits, box.transform.rotation, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}

		#endregion
		#region SphereCast

		public void SphereCast(Vector3 origin, float radius, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.SphereCastNonAlloc(origin, radius, direction, hits, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void SphereCast(Vector3 origin, float radius, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var delta = target - origin;
			length = Physics.SphereCastNonAlloc(origin, radius, delta.normalized, hits, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void SphereCast(Bounds bounds, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.SphereCastNonAlloc(bounds.center, bounds.extents.MaxComponent(), direction, hits, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void SphereCast(Bounds bounds, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var delta = target - bounds.center;
			length = Physics.SphereCastNonAlloc(bounds.center, bounds.extents.MaxComponent(), delta.normalized, hits, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void SphereCast(SphereCollider sphere, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.SphereCastNonAlloc(sphere.transform.position + sphere.center, sphere.radius, direction, hits, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void SphereCast(SphereCollider sphere, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var origin = sphere.transform.position + sphere.center;
			var delta = target - origin;
			length = Physics.SphereCastNonAlloc(origin, sphere.radius, delta.normalized, hits, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void SphereCast(Vector3 origin, SphereCollider sphere, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.SphereCastNonAlloc(origin, sphere.radius, direction, hits, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void SphereCast(Vector3 origin, SphereCollider sphere, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var delta = target - origin;
			length = Physics.SphereCastNonAlloc(origin, sphere.radius, delta.normalized, hits, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}

		#endregion
		#region CapsuleCast

		public void CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.CapsuleCastNonAlloc(point1, point2, radius, direction, hits, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var midpoint = Math.Midpoint(point1, point2);
			var delta = target - midpoint;
			length = Physics.CapsuleCastNonAlloc(point1, point2, radius, delta.normalized, hits, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void CapsuleCast(Vector3 origin, Quaternion rotation, float radius, float height, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var pointOffset = rotation * Vector3.up * (height - radius * 2f).Max() * 0.5f;
			length = Physics.CapsuleCastNonAlloc(origin + pointOffset, origin - pointOffset, radius, direction, hits, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void CapsuleCast(Vector3 origin, Quaternion rotation, float radius, float height, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var pointOffset = rotation * Vector3.up * (height - radius * 2f).Max() * 0.5f;
			var delta = target - origin;
			length = Physics.CapsuleCastNonAlloc(origin + pointOffset, origin - pointOffset, radius, delta.normalized, hits, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void CapsuleCast(CapsuleCollider capsule, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.CapsuleCastNonAlloc(capsule.GetHeadPositionUncapped(), capsule.GetTailPositionUncapped(), capsule.radius, direction, hits, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void CapsuleCast(CapsuleCollider capsule, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var origin = capsule.transform.position + capsule.center;
			var delta = target - origin;
			length = Physics.CapsuleCastNonAlloc(capsule.GetHeadPositionUncapped(), capsule.GetTailPositionUncapped(), capsule.radius, delta.normalized, hits, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void CapsuleCast(Vector3 origin, CapsuleCollider capsule, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			length = Physics.CapsuleCastNonAlloc(origin + capsule.GetHeadPositionUncappedLocal(), origin + capsule.GetTailPositionUncappedLocal(), capsule.radius, direction, hits, maxDistance, layerMask, queryTriggerInteraction);
			Refresh();
		}
		public void CapsuleCast(Vector3 origin, CapsuleCollider capsule, Vector3 target, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var delta = target - origin;
			length = Physics.CapsuleCastNonAlloc(origin + capsule.GetHeadPositionUncappedLocal(), origin + capsule.GetTailPositionUncappedLocal(), capsule.radius, delta.normalized, hits, delta.magnitude, layerMask, queryTriggerInteraction);
			Refresh();
		}

		#endregion
	}

	#endregion
	#region RaycastUtils

	public static class RaycastUtils
	{
		public static bool IsValid(this RaycastHit hit) => hit.collider != null && hit.point != Vector3.zero;
		public static bool IsValid(this RaycastHit2D hit) => hit.collider != null && hit.point != Vector2.zero;

		public static PhysicMaterial GetPhysicMaterial(this RaycastHit hit) => hit.collider.material;
		public static Surface GetSurface(this RaycastHit hit) => hit.collider.GetSurface();
		public static Surface GetSurface(this RaycastHit2D hit) => hit.collider.GetSurface();

		public static Vector3 GetAdjustmentPoint(this RaycastHit hit, Vector3 origin, Vector3 direction) =>
			origin + direction * hit.distance;
		public static Vector2 GetAdjustmentPoint(this RaycastHit2D hit, Vector2 origin, Vector2 direction) =>
			origin + direction * hit.distance;
	}

	#endregion
}
