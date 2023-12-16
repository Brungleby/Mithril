
/** DuckComponent.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril
{
	#region DuckComponent

	/// <summary>
	/// This component detaches its gameObject from its parent and follows it gradually.
	///</summary>
	public sealed class DuckComponent : MithrilComponent
	{
		#region Fields

		public bool followPosition = true;

		[Min(0f)]
		[SerializeField]
		private float _positionLagTime = 0f;
		public float positionLagTime { get => _positionLagTime; set => _positionLagTime.Max(); }

		[Min(0f)]
		[SerializeField]
		private float _positionMaxDistance = 1f;
		public float positionMaxDistance { get => _positionMaxDistance; set => _positionMaxDistance.Max(); }

		[Space]

		public bool followRotation = true;

		[Min(0f)]
		[SerializeField]
		private float _rotationLagTime = 0f;
		public float rotationLagTime { get => _rotationLagTime; set => _rotationLagTime.Max(); }

		#endregion
		#region Members

		private Transform anchor;

		private Vector3 _positionVelocity;
		private Vector3 _rotationVelocity;

		#endregion
		#region Properties
#if UNITY_EDITOR
		protected override bool callAwakeOnValidate => false;
#endif
		public bool enablePositionLag => _positionLagTime > 0f;
		public bool enablePositionMax => _positionMaxDistance > 0f;

		public bool enableRotationLag => _rotationLagTime > 0f;

		#endregion
		#region Methods

		protected override void Awake()
		{
			base.Awake();

			anchor = new GameObject($"{gameObject.name} [Anchor]").transform;
			anchor.SetParent(transform.parent);

			anchor.localPosition = transform.localPosition;
			anchor.localRotation = transform.localRotation;
			anchor.localScale = transform.localScale;

			transform.SetParent(null);
		}

		private void Update()
		{
			if (followPosition)
			{
				if (enablePositionLag)
				{
					var targetPosition = anchor.position;
					var deltaPosition = targetPosition - transform.position;

					Vector3 startPosition;
					if (deltaPosition.magnitude >= positionMaxDistance)
						startPosition = targetPosition - deltaPosition.normalized * positionMaxDistance;
					else
						startPosition = transform.position;

					transform.position = Vector3.SmoothDamp(startPosition, targetPosition, ref _positionVelocity, positionLagTime);
				}
				else
					transform.position = anchor.position;
			}

			if (followRotation)
			{
				if (enableRotationLag)
				{
					transform.eulerAngles = Math.SmoothDampEulerAngles(transform.eulerAngles, anchor.eulerAngles, ref _rotationVelocity, _rotationLagTime);
				}
				else
					transform.rotation = anchor.rotation;
			}
		}

		#endregion
	}

	#endregion
}
