
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
using UnityEngine.UIElements;

#endregion

namespace Mithril
{
	#region CasterComponent

	/// <summary>
	/// This is the base class for any component that creates a sensation by firing a cast to produce a <see cref="Hit"/>.
	///</summary>
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
	public abstract class CasterComponent<TCollider, THit> : CasterComponent, IColliderUser<TCollider>
	where TCollider : Component
	{
		[SerializeField]
		private TCollider colliderTemplate;

		[SerializeField]
		public bool DisableTemplateOnAwake = false;

#pragma warning disable
		public new TCollider collider { get; protected set; }
#pragma warning restore

		private Action m_Sense;

		protected override void Awake()
		{
			base.Awake();

			if (colliderTemplate == null)
			{
				colliderTemplate = GetComponent<TCollider>() ??
				throw new UnassignedReferenceException($"{GetType().Name}.colliderTemplate has not been assigned and no component in this gameObject ({name}) is available.");
			}

			collider = colliderTemplate;
			m_Sense = GetSenseMethod(collider.GetType());

			if (DisableTemplateOnAwake && colliderTemplate)
				colliderTemplate.SetEnabled(false);
		}

		protected void Sense() => m_Sense.Invoke();

		protected virtual void Sense_Line() =>
#if DEBUG
			throw new NotImplementedException($"{GetType().Name} ({name}) needs a Line sensor method.");
#endif
		protected virtual void Sense_Box() =>
#if DEBUG
			throw new NotImplementedException($"{GetType().Name} ({name}) needs a Box/Box2D sensor method.");
#endif
		protected virtual void Sense_Sphere() =>
#if DEBUG
			throw new NotImplementedException($"{GetType().Name} ({name}) needs a Sphere/Circle sensor method.");
#endif
		protected virtual void Sense_Capsule() =>
#if DEBUG
			throw new NotImplementedException($"{GetType().Name} ({name}) needs a Capsule/Capsule2D sensor method.");
#endif


		private Action GetSenseMethod(Type type)
		{
			if (type == typeof(BoxCollider) || type == typeof(BoxCollider2D))
				return Sense_Box;
			if (type == typeof(SphereCollider) || type == typeof(CircleCollider2D))
				return Sense_Sphere;
			if (type == typeof(CapsuleCollider) || type == typeof(CapsuleCollider2D))
				return Sense_Capsule;

			return Sense_Line;
		}
	}

	#endregion
}
