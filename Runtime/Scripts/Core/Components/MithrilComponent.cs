
/** MithrilComponent.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;

#endregion

namespace Mithril
{
	#region MithrilComponent

	/// <summary>
	/// Base class for a <see cref="MonoBehaviour"/> with a little extra functionality.
	///</summary>

	public abstract class MithrilComponent : MonoBehaviour
	{
		#region Inners

		#region AutoAssignAttribute

		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
		protected sealed class AutoAssignAttribute : Attribute
		{
			public Type assignType { get; }

			public AutoAssignAttribute(Type assignType)
			{
				this.assignType = assignType;
			}
			public AutoAssignAttribute()
			{
				assignType = null;
			}
		}

		#endregion

		#endregion
		#region Members

		private const BindingFlags AUTO_ASSIGN_FIELD_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

#if UNITY_EDITOR
		private bool isAwake = false;
		protected virtual bool callAwakeOnValidate => true;
#endif

		#endregion
		#region Methods

		protected virtual void OnValidate()
		{
#if UNITY_EDITOR
			if (callAwakeOnValidate && Application.isPlaying && isAwake)
				Awake();
#endif
		}

		protected virtual void Awake()
		{
			try { AutoAssignComponents(); }
			catch (Exception e) { Debug.LogException(e); }
#if UNITY_EDITOR
			isAwake = true;
#endif
		}

		private void AutoAssignComponents()
		{
			var shouldWarnPrivate = !GetType().IsSealed;

			var fieldsToAssign = GetType().GetFields(AUTO_ASSIGN_FIELD_FLAGS).Where(i => i.GetCustomAttribute<AutoAssignAttribute>() != null);
			foreach (var iField in fieldsToAssign)
			{
				if (shouldWarnPrivate && iField.IsPrivate) Debug.LogWarning($"Field '{GetType().Name}.{iField.Name}' is private. It will not be auto-assigned in derived classes. Using a protected field instead will resolve this issue.");
				if (iField.GetValue(this) != null) continue;

				var iAttribute = iField.GetCustomAttribute<AutoAssignAttribute>();
				iField.SetValue(this, GetAutoAssignValue(iAttribute, iField.FieldType));
			}

			var propertiesToAssign = GetType().GetProperties(AUTO_ASSIGN_FIELD_FLAGS).Where(i => i.GetCustomAttribute<AutoAssignAttribute>() != null);
			foreach (var iProperty in propertiesToAssign)
			{
				if (!iProperty.CanWrite)
				{
					Debug.LogError($"Property '{iProperty.Name}' in {GetType().Name} cannot be auto-assigned because it is either missing a setter or its setter is private. Using a protected set will resolve this issue.");
					continue;
				}
				if (shouldWarnPrivate && iProperty.GetSetMethod(true).IsPrivate) Debug.LogWarning($"Property setter '{GetType().Name}.{iProperty.Name}' is private. It will not be auto-assigned in derived classes. Using a protected setter instead will resolve this issue.");

				var iAttribute = iProperty.GetCustomAttribute<AutoAssignAttribute>();
				iProperty.SetValue(this, GetAutoAssignValue(iAttribute, iProperty.PropertyType));
			}
		}

		private Component GetAutoAssignValue(AutoAssignAttribute attribute, Type verifyType)
		{
			var assignType = attribute.assignType ?? verifyType;

			if (!verifyType.IsAssignableFrom(assignType))
				throw new InvalidCastException($"AssignOnAwake Type ({assignType.Name}) must be of type ({verifyType.Name}).");

			return GetComponent(assignType) ?? GetComponentInParent(assignType) ?? throw new NullReferenceException($"AssignOnAwake: Null reference found for type ({assignType.Name})");
		}

		#endregion
	}

	#endregion
}
