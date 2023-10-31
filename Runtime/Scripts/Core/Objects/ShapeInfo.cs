
/** ShapeInfo.cs
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
	#region ShapeInfo

	public abstract class ShapeInfoBase : object
	{
		public static T CreateFrom<T>(Component collider) where T : ShapeInfoBase
		{
			if (collider is Collider collider3D)
				return (T)(ShapeInfoBase)(ShapeInfo)collider3D;
			if (collider is Collider2D collider2D)
				return (T)(ShapeInfoBase)(ShapeInfo2D)collider2D;
			throw new NotImplementedException();
		}

		public abstract void Draw(Vector3 position, Color color);
		public void Draw(Vector3 position) => Draw(position, Color.white);
	}

	public abstract class ShapeInfo<TVector> : ShapeInfoBase
	where TVector : unmanaged
	{
		public readonly TVector center;

		public ShapeInfo(TVector center)
		{
			this.center = center;
		}

	}

	public abstract class ShapeInfo : ShapeInfo<Vector3>
	{
		public ShapeInfo(Vector3 center) : base(center) { }
		public static implicit operator ShapeInfo(Collider collider)
		{
			if (collider == null)
				return null;
			if (collider is BoxCollider box)
				return (BoxInfo)box;
			if (collider is SphereCollider sphere)
				return (SphereInfo)sphere;
			if (collider is CapsuleCollider capsule)
				return (CapsuleInfo)capsule;
			throw new NotImplementedException();
		}
	}

	public abstract class ShapeInfo2D : ShapeInfo<Vector2>
	{
		public ShapeInfo2D(Vector2 center) : base(center) { }
		public static implicit operator ShapeInfo2D(Collider2D collider2D)
		{
			if (collider2D == null)
				return null;
			if (collider2D is BoxCollider2D box2D)
				return (BoxInfo2D)box2D;
			if (collider2D is CircleCollider2D circle2D)
				return (CircleInfo2D)circle2D;
			if (collider2D is CapsuleCollider2D capsule2D)
				return (CapsuleInfo2D)capsule2D;
			throw new NotImplementedException();
		}
	}

	#endregion
	#region RayInfo

	public sealed class RayInfo : ShapeInfo
	{
		public readonly Vector3 normal;
		public readonly float distance;

		public RayInfo(Vector3 center, Vector3 normal, float distance) : base(center)
		{
			this.normal = normal;
			this.distance = distance;
		}

		public override void Draw(Vector3 position, Color color)
		{
			// DebugDraw.DrawArrow(position + center, normal * distance, color, 0f);
		}
	}

	#endregion
	#region BoxInfo

	public sealed class BoxInfo : ShapeInfo
	{
		public readonly Vector3 size;
		public readonly Quaternion rotation;

		public BoxInfo(Vector3 center, Quaternion rotation, Vector3 size) : base(center)
		{
			this.rotation = rotation;
			this.size = size;
		}
		public BoxInfo(BoxCollider collider) : base(collider.center)
		{
			size = collider.size;
		}

		public static implicit operator BoxInfo(BoxCollider collider) => new(collider);

		public override void Draw(Vector3 position, Color color)
		{
			DebugDraw.DrawWireBox(position + center, rotation, size * 2f, color);
		}
	}

	#endregion
	#region SphereInfo

	public sealed class SphereInfo : ShapeInfo
	{
		public readonly float radius;

		public SphereInfo(Vector3 center, float radius) : base(center)
		{
			this.radius = radius;
		}
		public SphereInfo(SphereCollider collider) : base(collider.center)
		{
			radius = collider.radius;
		}

		public static implicit operator SphereInfo(SphereCollider collider) => new(collider);

		public override void Draw(Vector3 position, Color color)
		{
			Gizmos.color = color;
			Gizmos.DrawWireSphere(position + center, radius);
		}
	}

	#endregion
	#region CapsuleInfo

	public sealed class CapsuleInfo : ShapeInfo
	{
		public readonly Quaternion rotation;
		public readonly float radius;
		public readonly float height;
		public readonly int direction;

		public CapsuleInfo(Vector3 center, Quaternion rotation, float radius, float height, int direction) : base(center)
		{
			this.rotation = rotation;
			this.radius = radius;
			this.height = height;
			this.direction = direction;
		}
		public CapsuleInfo(Vector3 point1, Vector3 point2, float radius, int direction) : base(Vector3.zero)
		{
			rotation = Geometry.GetCapsuleRotation(point1, point2);
			this.radius = radius;
			height = (point1 - point2).magnitude + radius * 2f;
			this.direction = direction;
		}
		public CapsuleInfo(CapsuleCollider collider) : base(collider.center)
		{
			radius = collider.radius;
			direction = collider.direction;
		}

		public static implicit operator CapsuleInfo(CapsuleCollider collider) => new(collider);

		public override void Draw(Vector3 position, Color color)
		{
			DebugDraw.DrawWireCapsule(position, rotation, radius, height, color);
		}
	}

	#endregion
	#region BoxInfo2D

	public sealed class BoxInfo2D : ShapeInfo2D
	{
		public readonly Vector2 size;
		public readonly float edgeRadius;

		public BoxInfo2D(Vector2 center, Vector2 size, float edgeRadius) : base(center)
		{
			this.size = size;
			this.edgeRadius = edgeRadius;
		}
		public BoxInfo2D(BoxCollider2D collider) : base(collider.offset)
		{
			size = collider.size;
			edgeRadius = collider.edgeRadius;
		}

		public static implicit operator BoxInfo2D(BoxCollider2D collider) => new(collider);

		public override void Draw(Vector3 position, Color color)
		{

		}
	}

	#endregion
	#region SphereInfo2D

	public sealed class CircleInfo2D : ShapeInfo2D
	{
		public readonly float radius;

		public CircleInfo2D(Vector2 center, float radius) : base(center)
		{
			this.radius = radius;
		}
		public CircleInfo2D(CircleCollider2D collider) : base(collider.offset)
		{
			radius = collider.radius;
		}

		public static implicit operator CircleInfo2D(CircleCollider2D collider) => new(collider);

		public override void Draw(Vector3 position, Color color)
		{

		}
	}

	#endregion
	#region CapsuleInfo2D

	public sealed class CapsuleInfo2D : ShapeInfo2D
	{
		public readonly Vector2 size;
		public readonly CapsuleDirection2D direction;

		public CapsuleInfo2D(Vector2 center, Vector2 size, CapsuleDirection2D direction) : base(center)
		{
			this.size = size;
			this.direction = direction;
		}
		public CapsuleInfo2D(Vector2 center, Vector2 size, int direction) : base(center)
		{
			this.size = size;
			this.direction = (CapsuleDirection2D)direction;
		}
		public CapsuleInfo2D(CapsuleCollider2D collider) : base(collider.offset)
		{
			size = collider.size;
			direction = collider.direction;
		}

		public static implicit operator CapsuleInfo2D(CapsuleCollider2D collider) => new(collider);

		public override void Draw(Vector3 position, Color color)
		{

		}
	}

	#endregion
}
