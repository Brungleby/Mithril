
/** MithrilTemplate.cs
*
*	Created by LIAM WOFFORD.
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
	#region HitBase

	/// <summary>
	/// Base class for a more detailed RaycastHit.
	///</summary>

	public abstract class HitBase
	{
		#region Construction

		protected HitBase()
		{
			isBlocked = false;
		}

		protected HitBase(bool _isBlocked)
		{
			isBlocked = _isBlocked;
		}

		#endregion

		/// <summary>
		/// Whether or not the cast performed actually hit anything.
		///</summary>

		public readonly bool isBlocked;
	}

	#endregion
	#region Hit<TVector>

	/// <inheritdoc cref="HitBase"/>

	public abstract class Hit<TVector> : HitBase, IComparable<Hit<TVector>>
	where TVector : unmanaged
	{
		#region Construction

		protected Hit() : base()
		{
			maxDistance = distance = percent = 0f;
			origin = target = normal = point = pointAdjustment = default(TVector);
		}

		protected Hit(
			bool _isBlocked,
			float _maxDistance,
			float _distance,
			TVector _origin,
			TVector _target,
			TVector _normal,
			TVector _point,
			TVector _adjustmentPoint,
			Transform _transform
		) :
		base(
			_isBlocked
		)
		{
			maxDistance = _maxDistance;
			distance = _distance;
			percent = _distance / _maxDistance;

			origin = _origin;
			target = _target;
			normal = _normal;
			point = _point;
			pointAdjustment = _adjustmentPoint;

			transform = _transform;
		}

		#endregion

		#region Members

		/// <summary>
		/// The distance between <see cref="origin"/> and <see cref="target"/>.
		///</summary>

		public readonly float maxDistance;

		/// <inheritdoc cref="RaycastHit.distance"/>

		public readonly float distance;

		/// <summary>
		/// The percentage (between 0 and 1) as <see cref="distance"/> approaches <see cref="maxDistance"/>.
		///</summary>

		public readonly float percent;

		/// <summary>
		/// The position at which the cast began.
		///</summary>

		public readonly TVector origin;

		/// <summary>
		/// The position at which the cast tried to end at.
		///</summary>

		public readonly TVector target;

		/// <inheritdoc cref="RaycastHit.normal"/>

		public readonly TVector normal;

		/// <inheritdoc cref="RaycastHit.point"/>

		public readonly TVector point;

		/// <summary>
		/// The point at which we can set Collider to so as not to intersect with it.
		///</summary>

		public readonly TVector pointAdjustment;

		/// <inheritdoc cref="RaycastHit.transform"/>

		public readonly Transform transform;

		public int CompareTo(Hit<TVector> other) => (int)(distance - other.distance).Sign();

		#endregion
	}

	#endregion
	#region Hit<TRaycastHit, TCollider, TRigidbody, TVector>

	/// <inheritdoc cref="HitBase"/>

	public abstract class Hit<TRaycastHit, TCollider, TRigidbody, TVector> : Hit<TVector>
	where TCollider : Component
	where TRigidbody : Component
	where TVector : unmanaged
	{
		#region Construction

		protected Hit() : base() { }

		protected Hit(
			bool _isBlocked,
			float _maxDistance,
			float _distance,
			TVector _origin,
			TVector _target,
			TVector _normal,
			TVector _point,
			TVector _adjustmentPoint,
			TRaycastHit _hit,
			Transform _transform,
			TCollider _collider,
			TRigidbody _rigidbody
		) :
		base(
			_isBlocked,
			_maxDistance,
			_distance,
			_origin,
			_target,
			_normal,
			_point,
			_adjustmentPoint,
			_transform
		)
		{
			hit = _hit;
			collider = _collider;
			rigidbody = _rigidbody;

			if (_collider != null)
			{
				physicMaterial = GetPhysicMaterial(_collider);
				surface = GetSurface(_collider);
			}
			else
			{
				physicMaterial = default;
				surface = default;
			}
		}

		#endregion

		#region Members

		/// <summary>
		/// The default <see cref="RaycastHit"/> obtained from a standard cast method.
		///</summary>

		public readonly TRaycastHit hit;

		/// <inheritdoc cref="RaycastHit.collider"/>

		public readonly TCollider collider;

		/// <inheritdoc cref="RaycastHit.rigidbody"/>

		public readonly TRigidbody rigidbody;

		/// <summary>
		/// The physic material which was hit.
		///</summary>

		public readonly PhysicMaterial physicMaterial;

		/// <summary>
		/// The surface definition which was hit.
		///</summary>

		public readonly Surface surface;

		#endregion
		#region Methods

		protected abstract PhysicMaterial GetPhysicMaterial(in TCollider collider);
		protected abstract Surface GetSurface(in TCollider collider);

		#endregion
	}

	#endregion

	#region Hit

	/// <summary>
	/// This is a more detailed version of a <see cref="RaycastHit"/> to be used in 3D.
	/// It supports multiple static methods for which to produce a result as one would with a <see cref="RaycastHit"/>.
	///</summary>

	public sealed class Hit : Hit<RaycastHit, Collider, Rigidbody, Vector3>
	{
		#region Constructors

		public Hit() : base() { }

		private Hit(RaycastHit _hit, Vector3 _origin, Vector3 _target) :
		base(
			_hit.IsBlocked(),
			(_origin - _target).magnitude,
			_hit.distance,
			_origin,
			_target,
			_hit.normal,
			_hit.point,
			Vector3.Lerp(_origin, _target, _hit.distance / (_origin - _target).magnitude),
			_hit,
			_hit.transform,
			_hit.collider,
			_hit.rigidbody
		)
		{ }

		#endregion
		#region Methods

		protected override PhysicMaterial GetPhysicMaterial(in Collider collider) =>
			collider.material;

		protected override Surface GetSurface(in Collider collider) =>
			collider.GetSurface();

		#region Static

		public static Hit none => new Hit();

		private static Hit[] _HitArray(RaycastHit[] hits, Vector3 origin, Vector3 target)
		{
			Hit[] result = new Hit[hits.Length];

			for (int i = 0; i < result.Length; i++)
				result[i] = new Hit(hits[i], origin, target);

			return result;
		}

		#region Linecast

		/// <summary>
		/// Casts a straight line and stops at the first <see cref="UnityEngine.Collider"/> in its path, returning information about the <see cref="UnityEngine.Collider"/> and the nature of the collision.
		///</summary>
		/// <param name="origin">
		/// The starting world position of the cast.
		///</param>
		/// <param name="target">
		/// The ending world position of the cast.
		///</param>
		/// <param name="layerMask">
		/// These layers will be filtered and any other layers will not be detected.
		///</param>

		public static Hit Linecast(Vector3 origin, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			RaycastHit __hit;
			UnityEngine.Physics.Linecast(origin, target, out __hit, layerMask, queryTriggerInteraction);

			return new Hit(__hit, origin, target);
		}

		/// <inheritdoc cref="Linecast(Vector3, Vector3, int, QueryTriggerInteraction)"/>
		/// <param name="direction">
		/// The direction in which the line will be cast.
		///</param>
		/// <param name="maxDistance">
		/// The maximum distance which this cast can reach.
		///</param>

		public static Hit Linecast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			Linecast(origin, origin + direction.normalized * maxDistance, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="Linecast(Vector3, Vector3, float, int, QueryTriggerInteraction)"/>
		/// <param name="ray">
		/// Defining ray used to determine linecast origin and target.
		///</param>

		public static Hit Linecast(Ray ray, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			Linecast(ray.origin, ray.origin + ray.direction * maxDistance, layerMask, queryTriggerInteraction);

		#endregion
		#region LinecastAll

		/// <summary>
		/// Casts a straight line and detects any or all <see cref="UnityEngine.Collider"/>s between the origin and target position.
		///</summary>
		/// <inheritdoc cref="Linecast(Vector3, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] LinecastAll(Vector3 origin, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			RaycastHit[] __hits = UnityEngine.Physics.RaycastAll(origin, direction, maxDistance, layerMask, queryTriggerInteraction);

			return _HitArray(__hits, origin, origin + direction * maxDistance);
		}

		/// <inheritdoc cref="LinecastAll(Vector3, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] LinecastAll(Vector3 origin, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			LinecastAll(origin, (target - origin).normalized, (target - origin).magnitude, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="LinecastAll(Vector3, Vector3, float, int, QueryTriggerInteraction)"/>
		/// <inheritdoc cref="Linecast(Ray, float, int, QueryTriggerInteraction)"/>

		public static Hit[] LinecastAll(Ray ray, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			LinecastAll(ray.origin, ray.direction, maxDistance, layerMask, queryTriggerInteraction);

		#endregion

		#region SphereExpansionOverlap

		/// <summary>
		/// Casts a sphere out from a single location and returns a Linecast to the closest point on the nearest <see cref="Collider"/> it reaches.
		///</summary>

		public static Collider SphereExpansionOverlap(Vector3 origin, float radius, float complexMinDifference, int complexMaxDepth, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			/**	Get all overlapping objects.
			*/
			var colliders = SphereExpansionOverlapAll(origin, radius, layerMask, queryTriggerInteraction);

			if (colliders.Length == 0)
				return null;

			/**	Iterate through each collider that was hit and determine the distance from the origin for each one.
			*/
			var colliderTable = new (Collider, float)[colliders.Length];
			for (int i = 0; i < colliderTable.Length; i++)
			{
				var collider = colliders[i];
				float distance;

				/**	If the ClosestPoint function can be used, please do so.
				*/
				if (collider.IsConvex())
				{
					Vector3 closestPoint = UnityEngine.Physics.ClosestPoint(origin, collider, collider.transform.position, collider.transform.rotation);
					distance = (closestPoint - origin).magnitude;
				}
				else // if the collider is a concave mesh.
				{
					float minRadius = 0f;
					float maxRadius = radius;

					for (int j = 0; j <= complexMaxDepth && maxRadius - minRadius >= complexMinDifference; j++)
					{
						float iRadius = Mathf.Lerp(minRadius, maxRadius, 0.5f);
						HashSet<Collider> iOverlaps = new();
						iOverlaps.AddAll(SphereExpansionOverlapAll(origin, iRadius, layerMask, queryTriggerInteraction));
						bool isOverlapping = iOverlaps.Contains(colliders[i]);

						(isOverlapping ? ref maxRadius : ref minRadius) = iRadius;
					}

					distance = maxRadius;
				}

				colliderTable[i] = (colliders[i], distance);
			}

			Collider mostestClosest = null;
			float minDistance = radius;
			foreach (var collider in colliderTable)
			{
				if (collider.Item2 < minDistance)
				{
					mostestClosest = collider.Item1;
					minDistance = collider.Item2;
				}
			}

			return mostestClosest;
		}


		#endregion
		#region SphereExpansionOverlapAll

		public static Collider[] SphereExpansionOverlapAll(Vector3 origin, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			UnityEngine.Physics.OverlapSphere(origin, radius, layerMask, queryTriggerInteraction);

		#endregion
		#region SphereExpansionCast

		public static Hit SphereExpansionCast(Vector3 origin, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			/**	Get all overlapping objects.
			*/
			var colliders = SphereExpansionOverlapAll(origin, radius, layerMask, queryTriggerInteraction);

			if (colliders.Length == 0)
				return null;

			/**	Iterate through each collider that was hit and determine the distance from the origin for each one.
			*/
			List<(Vector3, float)> colliderTable = new();
			foreach (var collider in colliders)
			{
				if (!collider.IsConvex()) continue;

				Vector3 closestPoint = UnityEngine.Physics.ClosestPoint(origin, collider, collider.transform.position, collider.transform.rotation);

				colliderTable.Add(
				(
					closestPoint,
					(closestPoint - origin).magnitude
				));
			}

			/**	Determine which collider is closest.
			*/
			(Vector3, float) closestCollider = (default, radius);
			foreach (var entry in colliderTable)
			{
				if (entry.Item2 < closestCollider.Item2)
					closestCollider = entry;
			}

			/**	Perform a single cast towards that one collider.
			*/
			float extraDistance = 0.1f;
			Vector3 direction = (closestCollider.Item1 - origin).normalized;

			return Linecast(origin, closestCollider.Item1 + direction * extraDistance, layerMask, queryTriggerInteraction);
		}
		// public static Hit SphereExpansionCast(Vector3 origin, float radius, int layerMask)

		#endregion

		#region BoxCast

		/// <summary>
		/// Casts a box along a straight line and stops at the first <see cref="UnityEngine.Collider"/> in its path, returning information about the <see cref="UnityEngine.Collider"/> and the nature of the collision.
		///</summary>
		/// <inheritdoc cref="Linecast(Vector3, Vector3, float, int, QueryTriggerInteraction)"/>
		/// <param name="halfExtents">
		/// Defines the length, width, and height of the box to be cast.
		///</param>
		/// <param name="orientation">
		/// Defines the rotation of the box.
		///</param>

		public static Hit BoxCast(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			RaycastHit hit;
			UnityEngine.Physics.BoxCast(origin, halfExtents, direction, out hit, orientation, maxDistance, layerMask, queryTriggerInteraction);

			return new Hit(hit, origin, origin + direction * maxDistance);
		}

		/// <inheritdoc cref="BoxCast(Vector3, Vector3, Quaternion, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit BoxCast(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Vector3 delta = target - origin;
			return BoxCast(origin, halfExtents, orientation, delta.normalized, delta.magnitude, layerMask, queryTriggerInteraction);
		}

		/// <inheritdoc cref="BoxCast(Vector3, Vector3, Quaternion, Vector3, int, QueryTriggerInteraction)"/>

		public static Hit BoxCast(Vector3 origin, Vector3 halfExtents, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) => BoxCast(origin, halfExtents, Quaternion.identity, target, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="BoxCast(Vector3, Vector3, Quaternion, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit BoxCast(Vector3 origin, Vector3 halfExtents, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			BoxCast(origin, halfExtents, Quaternion.identity, direction, maxDistance, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="BoxCast(Vector3, Vector3, Vector3, float, int, QueryTriggerInteraction)"/>
		/// <param name="box">
		/// Template box collider used to define the origin and shape of the cast.
		///</param>

		public static Hit BoxCast(BoxCollider box, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			BoxCast(box.transform.position + box.center, box.size / 2f, box.transform.rotation, direction, maxDistance, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="BoxCast(BoxCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit BoxCast(BoxCollider box, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Vector3 delta = target - box.transform.position;
			return BoxCast(box, delta.normalized, delta.magnitude, layerMask, queryTriggerInteraction);
		}

		#endregion
		#region BoxCastAll

		/// <summary>
		/// Casts a box along a straight line and detects any or all <see cref="UnityEngine.Collider"/>s between the origin and target position.
		///</summary>
		/// <inheritdoc cref="BoxCast(Vector3, Vector3, Quaternion, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] BoxCastAll(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			RaycastHit[] hits = UnityEngine.Physics.BoxCastAll(origin, halfExtents, direction, orientation, maxDistance, layerMask, queryTriggerInteraction);

			return _HitArray(hits, origin, origin + direction * maxDistance);
		}

		/// <inheritdoc cref="BoxCastAll"/>
		/// <inheritdoc cref="BoxCast(Vector3, Vector3, Quaternion, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] BoxCastAll(Vector3 origin, Vector3 halfExtents, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			BoxCastAll(origin, halfExtents, Quaternion.identity, direction, maxDistance, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="BoxCastAll"/>
		/// <inheritdoc cref="BoxCast(Vector3, Vector3, Quaternion, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] BoxCastAll(BoxCollider box, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			BoxCastAll(box.transform.position + box.center, box.size / 2f, box.transform.rotation, direction, maxDistance, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="BoxCastAll"/>
		/// <inheritdoc cref="BoxCast(Vector3, Vector3, Quaternion, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] BoxCastAll(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Vector3 delta = target - origin;
			return BoxCastAll(origin, halfExtents, orientation, delta.normalized, delta.magnitude, layerMask, queryTriggerInteraction);
		}

		/// <inheritdoc cref="BoxCastAll"/>
		/// <inheritdoc cref="BoxCast(Vector3, Vector3, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] BoxCastAll(Vector3 origin, Vector3 halfExtents, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			BoxCastAll(origin, halfExtents, Quaternion.identity, target, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="BoxCastAll"/>
		/// <inheritdoc cref="BoxCast(Vector3, Vector3, Vector3, int, QueryTriggerInteraction)"/>

		public static Hit[] BoxCastAll(BoxCollider box, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			BoxCastAll(box.transform.position + box.center, box.size / 2f, box.transform.rotation, target, layerMask, queryTriggerInteraction);

		#endregion

		#region SphereCast

		/// <summary>
		/// Casts a sphere along a straight line and stops at the first <see cref="UnityEngine.Collider"/> in its path, returning information about the <see cref="UnityEngine.Collider"/> and the nature of the collision.
		///</summary>
		/// <inheritdoc cref="Linecast(Vector3, Vector3, float, int, QueryTriggerInteraction)"/>
		/// <param name="radius">
		/// Defines the size of the sphere to be cast.
		///</param>

		public static Hit SphereCast(Vector3 origin, float radius, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			RaycastHit hit;
			UnityEngine.Physics.SphereCast(origin, radius, direction, out hit, maxDistance, layerMask, queryTriggerInteraction);

			return new Hit(hit, origin, origin + direction * maxDistance);
		}

		/// <inheritdoc cref="SphereCast"/>

		public static Hit SphereCast(Vector3 origin, float radius, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			var delta = target - origin;
			return SphereCast(origin, radius, delta.normalized, delta.magnitude, layerMask, queryTriggerInteraction);
		}

		/// <inheritdoc cref="SphereCast"/>
		/// <param name="sphere">
		/// Template sphere collider used to define the origin and shape of the cast.
		///</param>

		public static Hit SphereCast(SphereCollider sphere, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			SphereCast(sphere.transform.position + sphere.center, sphere.radius, direction, maxDistance, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="SphereCast(SphereCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit SphereCast(SphereCollider sphere, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			SphereCast(sphere.transform.position + sphere.center, sphere.radius, target, layerMask, queryTriggerInteraction);

		#endregion
		#region SphereCastAll

		/// <summary>
		/// Casts a sphere along a straight line and detects any or all <see cref="UnityEngine.Collider"/>s between the origin and target position.
		///</summary>
		/// <inheritdoc cref="SphereCast"/>

		public static Hit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			RaycastHit[] hits = UnityEngine.Physics.SphereCastAll(origin, radius, direction, maxDistance, layerMask, queryTriggerInteraction);

			return _HitArray(hits, origin, origin + direction * maxDistance);
		}

		/// <inheritdoc cref="SphereCastAll"/>
		/// <inheritdoc cref="SphereCast(SphereCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] SphereCastAll(SphereCollider sphere, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			SphereCastAll(sphere.transform.position + sphere.center, sphere.radius, direction, maxDistance, layerMask, queryTriggerInteraction);


		#endregion

		#region CapsuleCast

		/// <summary>
		/// Casts a capsule along a straight line and stops at the first <see cref="UnityEngine.Collider"/> in its path, returning information about the <see cref="UnityEngine.Collider"/> and the nature of the collision.
		///</summary>
		/// <inheritdoc cref="Linecast(Vector3, Vector3, float, int, QueryTriggerInteraction)"/>
		/// <param name="point1">
		/// The world position of the first hemisphere (uncapped) of the capsule.
		///</param>
		/// <param name="point2">
		/// The world position of the second hemisphere (uncapped) of the capsule.
		///</param>

		public static Hit CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			RaycastHit __hit;
			UnityEngine.Physics.CapsuleCast(point1, point2, radius, direction, out __hit, maxDistance, layerMask, queryTriggerInteraction);

			Vector3 __midpoint = Math.Midpoint(point1, point2);
			return new Hit(__hit, __midpoint, __midpoint + direction * maxDistance);
		}

		/// <inheritdoc cref="CapsuleCast"/>
		/// <param name="capsule">
		/// Template capsule collider used to define the origin and shape of the cast.
		///</param>

		public static Hit CapsuleCast(CapsuleCollider capsule, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			CapsuleCast(capsule.GetTailPositionUncapped(), capsule.GetHeadPositionUncapped(), capsule.radius, direction, maxDistance, layerMask, queryTriggerInteraction);

		#endregion
		#region CapsuleCastAll

		/// <summary>
		/// Casts a capsule along a straight line and detects any or all <see cref="UnityEngine.Collider"/>s between the origin and target position.
		///</summary>
		/// <inheritdoc cref="CapsuleCast"/>

		public static Hit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			RaycastHit[] __hits = UnityEngine.Physics.CapsuleCastAll(point1, point2, radius, direction, maxDistance, layerMask, queryTriggerInteraction);

			Vector3 __midpoint = Math.Midpoint(point1, point2);
			return _HitArray(__hits, __midpoint, __midpoint + direction * maxDistance);
		}

		/// <inheritdoc cref="CapsuleCastAll"/>
		/// <inheritdoc cref="CapsuleCast(CapsuleCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] CapsuleCastAll(CapsuleCollider capsule, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			CapsuleCastAll(capsule.GetTailPositionUncapped(), capsule.GetHeadPositionUncapped(), capsule.radius, direction, maxDistance, layerMask, queryTriggerInteraction);

		#endregion

		#endregion

		#endregion
	}

	#endregion
	#region (sealed) Hit2D

	public sealed class Hit2D : Hit<RaycastHit2D, Collider2D, Rigidbody2D, Vector2>
	{
		#region Constructors

		public Hit2D() : base() { }

		private Hit2D(RaycastHit2D _hit, Vector2 _origin, Vector2 _target) :
		base(
			_hit.IsBlocked(),
			(_origin - _target).magnitude,
			_hit.distance,
			_origin,
			_target,
			_hit.normal,
			_hit.point,
			Vector3.Lerp(_origin, _target, _hit.distance / (_origin - _target).magnitude),
			_hit,
			_hit.transform,
			_hit.collider,
			_hit.rigidbody
		)
		{ }

		#endregion
		#region Methods

		protected override PhysicMaterial GetPhysicMaterial(in Collider2D collider) =>
			throw new System.NotImplementedException();

		protected override Surface GetSurface(in Collider2D collider) =>
			collider.GetSurface();

		#region Static

		/**
		*	__TODO_DEVELOP__
		*/

		public static Hit2D none => new Hit2D();

		#endregion

		#endregion
	}

	#endregion

	#region (static) HitExtensions

	public static class HitExtensions
	{
		public static bool IsBlocked(this HitBase hit) =>
			hit != null && hit.isBlocked;
		public static bool IsBlocked(this RaycastHit hit) =>
			hit.collider != null;
		public static bool IsBlocked(this RaycastHit2D hit) =>
			hit.collider != null;

		/// <inheritdoc cref="Hit.BoxCast(BoxCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit BoxCast(this BoxCollider box, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			Hit.BoxCast(box, direction, maxDistance, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="Hit.BoxCast(BoxCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit BoxCast(this BoxCollider box, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			Hit.BoxCast(box, target, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="Hit.BoxCastAll"/>
		/// <inheritdoc cref="Hit.BoxCast(BoxCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] BoxCastAll(this BoxCollider box, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			Hit.BoxCastAll(box, direction, maxDistance, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="Hit.BoxCastAll"/>
		/// <inheritdoc cref="Hit.BoxCast(BoxCollider, Vector3, int, QueryTriggerInteraction)"/>

		public static Hit[] BoxCastAll(this BoxCollider box, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			Hit.BoxCastAll(box, target, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="Hit.SphereCast(SphereCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit SphereCast(this SphereCollider sphere, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			Hit.SphereCast(sphere, direction, maxDistance, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="Hit.SphereCast(SphereCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit SphereCast(this SphereCollider sphere, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			Hit.SphereCast(sphere, target, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="Hit.SphereCastAll"/>
		/// <inheritdoc cref="Hit.SphereCast(SphereCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] SphereCastAll(this SphereCollider sphere, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			SphereCastAll(sphere, direction, maxDistance, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="Hit.CapsuleCast(CapsuleCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit CapsuleCast(this CapsuleCollider capsule, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			Hit.CapsuleCast(capsule, direction, maxDistance, layerMask, queryTriggerInteraction);

		/// <inheritdoc cref="Hit.CapsuleCastAll"/>
		/// <inheritdoc cref="Hit.CapsuleCast(CapsuleCollider, Vector3, float, int, QueryTriggerInteraction)"/>

		public static Hit[] CapsuleCastAll(this CapsuleCollider capsule, Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) =>
			Hit.CapsuleCastAll(capsule, direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	#endregion
}
