
/** ColliderGravityField.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using Mithril.Pawn;
using UnityEngine;

#endregion

namespace Mithril
{
	#region ColliderGravityField

	/// <summary>
	/// Creates a gravity field that conforms to the kind of sibling collider it has.
	///</summary>

	public sealed class ColliderGravityField : GravityField<Collider, Vector3>
	{
		#region Fields

		[Min(0f)]
		[SerializeField]
		private float _falloffDistance = 0f;
		public float falloffDistance { get => _falloffDistance; set => _falloffDistance = value.Clamp(0f, GetColliderRadius()); }

		#endregion
		#region Members

		private Func<Vector3, Vector3> m_GetForceAtPosition;
		private Func<float> m_GetColliderRadius;
#if UNITY_EDITOR
		private static readonly Color GIZMO_COLOR_FALLOFF = Color.yellow;
		private static readonly Color GIZMO_COLOR_MAIN = Color.green;

		private Action m_OnDrawGizmos;
		private Action m_OnDrawGizmosSelected;
#endif
		private BoxCollider colliderAsBox;
		private SphereCollider colliderAsSphere;
		private CapsuleCollider colliderAsCapsule;

		#endregion
		#region Properties

		public float falloffPercent => falloffDistance / GetColliderRadius();

		#endregion
		#region Methods

		private void OnValidate()
		{
			AssignShapedMethods();
			falloffDistance = _falloffDistance;
		}

		public override Vector3 GetForceAtPosition(Vector3 position) => m_GetForceAtPosition.Invoke(position);

		public float GetColliderRadius() => m_GetColliderRadius.Invoke();
		public float GetFalloffRadius() => falloffDistance;

		private void AssignShapedMethods()
		{
			var collider = GetComponent<Collider>();
			var colliderType = collider.GetType();
			if (colliderType == typeof(BoxCollider))
			{
				colliderAsBox = (BoxCollider)collider;
				m_GetForceAtPosition = GetForceAtPosition_Box;
				m_GetColliderRadius = GetColliderRadius_Box;
#if UNITY_EDITOR
				m_OnDrawGizmos = OnDrawGizmos_Box;
				m_OnDrawGizmosSelected = OnDrawGizmosSelected_Box;
#endif
			}
			else if (colliderType == typeof(SphereCollider))
			{
				colliderAsSphere = (SphereCollider)collider;
				m_GetForceAtPosition = GetForceAtPosition_Sphere;
				m_GetColliderRadius = GetColliderRadius_Sphere;
#if UNITY_EDITOR
				m_OnDrawGizmos = OnDrawGizmos_Sphere;
				m_OnDrawGizmosSelected = OnDrawGizmosSelected_Sphere;
#endif
			}
			else if (colliderType == typeof(CapsuleCollider))
			{
				colliderAsCapsule = (CapsuleCollider)collider;
				m_GetForceAtPosition = GetForceAtPosition_Capsule;
				m_GetColliderRadius = GetColliderRadius_Capsule;
#if UNITY_EDITOR
				m_OnDrawGizmos = OnDrawGizmos_Capsule;
				m_OnDrawGizmosSelected = OnDrawGizmosSelected_Capsule;
#endif
			}
			else
				throw new NotImplementedException();
		}

		private Vector3 GetForceAtPosition_Box(Vector3 position)
		{
			var radius = GetColliderRadius_Box();
			var verticalProjection = Vector3.Project(position - transform.position, transform.up);
			var distanceFromBottom = verticalProjection.y + radius / 2f;
			var falloff = 1f - distanceFromBottom.Remap(falloffDistance, radius, 0f, 1f, true);
			return -transform.up * falloff * strengthScale;
		}

		private Vector3 GetForceAtPosition_Sphere(Vector3 position)
		{
			throw new NotImplementedException();
		}

		private Vector3 GetForceAtPosition_Capsule(Vector3 position)
		{
			throw new NotImplementedException();
		}

		private float GetColliderRadius_Box()
		{
			return transform.lossyScale.y * colliderAsBox.size.y;
		}

		private float GetColliderRadius_Sphere()
		{
			return transform.lossyScale.MaxComponentAbs() * colliderAsSphere.radius;
		}

		private float GetColliderRadius_Capsule()
		{
			return transform.lossyScale.MaxComponentAbs() * colliderAsCapsule.radius;
		}
#if UNITY_EDITOR
		private float gizmoAlpha =>
			1f - ((float)UnityEditor.EditorApplication.timeSinceStartup * 0.1f * strengthScale.Sign() % 1f);
		private float ShiftGizmoAlpha(int i) =>
			((gizmoAlpha + (i / 5f)) % 1f).Pow(3f);

		private void OnDrawGizmos_Box()
		{
			UnityEditor.Handles.color = GIZMO_COLOR_FALLOFF;
			DrawSquareAtBoxAlpha(falloffPercent);
		}

		private void OnDrawGizmosSelected_Box()
		{
			UnityEditor.Handles.color = GIZMO_COLOR_MAIN;
			for (var i = 0; i < 5; i++)
			{
				var ia = ShiftGizmoAlpha(i).Remap(0f, 1f, falloffPercent, 1f);

				DrawSquareAtBoxAlpha(ia);
			}
		}

		private void DrawSquareAtBoxAlpha(float alpha)
		{
			var positions = new Vector3[8];
			for (var ix = 0; ix < 2; ix++)
			{
				for (var iz = 0; iz < 2; iz++)
				{
					var relativePosition = colliderAsBox.center + Vector3.Scale(colliderAsBox.size / 2f,
						Vector3.right * (ix * 2 - 1)
						+ (Vector3.up * alpha.Remap(0f, 1f, -1f, 1f))
						+ Vector3.forward * (iz * 2 - 1)
					);

					var worldPosition = transform.position + transform.rotation * Vector3.Scale(relativePosition, transform.lossyScale);

					positions[ix * 2 + iz] = worldPosition;
				}
			}

			positions[4] = positions[1];
			positions[5] = positions[3];
			positions[6] = positions[2];
			positions[7] = positions[0];

			UnityEditor.Handles.DrawLines(positions);
		}

		private void OnDrawGizmos_Sphere()
		{
			UnityEditor.Handles.color = GIZMO_COLOR_FALLOFF;
			DrawCircleAtSphereAlpha(falloffPercent);
		}

		private void OnDrawGizmosSelected_Sphere()
		{
			UnityEditor.Handles.color = GIZMO_COLOR_MAIN;
			DrawCircleAtSphereAlpha(1f);
			for (var i = 0; i < 5; i++)
			{
				var ia = ShiftGizmoAlpha(i).Remap(0f, 1f, falloffPercent, 1f);
				DrawCircleAtSphereAlpha(ia);
			}
		}

		private void DrawCircleAtSphereAlpha(float alpha)
		{
			var normal = Camera.current.transform.forward;

			UnityEditor.Handles.DrawWireDisc(transform.position + Vector3.Scale(transform.lossyScale, colliderAsSphere.center), normal, alpha * GetColliderRadius_Sphere());
		}

		private void OnDrawGizmos_Capsule()
		{

		}

		private void OnDrawGizmosSelected_Capsule()
		{

		}

		private void DrawCircle(Vector3 position, Vector3 normal, float radius)
		{
			UnityEditor.Handles.DrawWireDisc(position, normal, radius);
		}
#endif
		#endregion
		#region Debug
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			m_OnDrawGizmos.Invoke();
		}

		private void OnDrawGizmosSelected()
		{
			m_OnDrawGizmosSelected.Invoke();
		}
#endif
		#endregion
	}

	#endregion
}
