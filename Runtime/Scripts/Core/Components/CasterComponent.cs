
/** CasterComponent.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Mithril
{
	#region CasterComponent

	/// <summary>
	/// This is the base class for any module that creates a sensation by firing a cast to produce a <see cref="Hit"/>.
	///</summary>
	[DefaultExecutionOrder(-10)]
	public abstract class CasterComponent : MithrilComponent
	{
		#region Fields

		/// <summary>
		/// Layers that this component can sense.
		///</summary>
		[Tooltip("Layers that this component can sense.")]
		[SerializeField]
		public LayerMask layers;

		#endregion
	}

	#endregion
	#region CasterComponent<TCollider, THit, TShapeInfo>

	/// <summary>
	/// The base class for a sensor that uses a collider component as the shape for its cast.
	///</summary>

	public abstract class CasterComponent<TCollider, THit, TShapeInfo> : CasterComponent, IColliderUser<TCollider>
	where TCollider : Component
	where THit : HitBase, new()
	where TShapeInfo : ShapeInfoBase
	{
		[SerializeField]
		private TCollider colliderTemplate;

		[SerializeField]
		public bool DisableTemplateOnAwake = false;

#pragma warning disable
		public new TCollider collider { get; protected set; }
#pragma warning restore

		public TShapeInfo shapeInfo { get; set; }
#if UNITY_EDITOR
		protected virtual THit hitToDraw => null;
#endif
		private Func<THit> m_Sense;

		protected override void Awake()
		{
			base.Awake();

			if (colliderTemplate == null)
			{
				colliderTemplate = GetComponent<TCollider>() ??
				throw new UnassignedReferenceException($"{GetType().Name}.colliderTemplate has not been assigned and no component in this gameObject ({name}) is available.");
			}

			collider = colliderTemplate;

			shapeInfo = ShapeInfoBase.CreateFrom<TShapeInfo>(collider);
			m_Sense = GetSenseMethod(shapeInfo);

			if (DisableTemplateOnAwake && colliderTemplate)
				colliderTemplate.SetEnabled(false);
		}

		protected THit Sense() => m_Sense.Invoke();

		protected virtual THit Sense_Line() => throw new NotImplementedException($"{GetType().Name} ({name}) needs a Line sensor method.");
		protected virtual THit Sense_Box() => throw new NotImplementedException($"{GetType().Name} ({name}) needs a Box sensor method.");
		protected virtual THit Sense_Sphere() => throw new NotImplementedException($"{GetType().Name} ({name}) needs a Sphere sensor method.");
		protected virtual THit Sense_Capsule() => throw new NotImplementedException($"{GetType().Name} ({name}) needs a Capsule sensor method.");

		private Func<THit> GetSenseMethod(TShapeInfo info)
		{
			if (info == null)
				return Sense_Line;
			if (info is BoxInfo || info is BoxInfo2D)
				return Sense_Box;
			if (info is SphereInfo || info is CircleInfo2D)
				return Sense_Sphere;
			if (info is CapsuleInfo || info is CapsuleInfo2D)
				return Sense_Capsule;

			throw new NotImplementedException();
		}

		protected virtual void OnDrawGizmosSelected()
		{
#if UNITY_EDITOR
			if (hitToDraw == null) return;
			var hit = (Hit)(HitBase)hitToDraw;
			hit.OnDrawGizmos();
#endif
		}
	}

	#endregion
}
