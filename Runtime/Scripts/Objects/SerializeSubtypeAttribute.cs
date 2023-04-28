
/** SerializeSubtypeAttribute.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SerializeSubtypeAttribute : Attribute, ISerializationCallbackReceiver
	{
		#region Data

		// private static readonly string 

		[SerializeField]
		private object _objectValue;
		private object objectValue
		{
			get => _objectValue;
			set => _objectValue = value;
		}

		[SerializeField]
		private string _serializedObjectType;
		private Type objectType
		{
			get => Type.GetType(_serializedObjectType);
			/**	__TODO_REVIEW__
			*/
			set => _serializedObjectType = value.AssemblyQualifiedName;
		}

		#endregion
		#region Methods

		#region Construction

		public SerializeSubtypeAttribute()
		{

		}

		#endregion
		#region ISerializationCallbackReceiver

		public void OnBeforeSerialize()
		{

			objectType = objectValue.GetType();

			Debug.Log($"Serialized {objectValue} as a(n) {objectType.Name}");
		}

		public void OnAfterDeserialize()
		{


			Debug.Log($"Deserialized {objectValue} as a(n) {objectType.Name}");
		}

		#endregion

		#endregion
	}
}
