
/** FollowerComponent.cs
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
	#region FollowerComponent

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>
	[DefaultExecutionOrder(11)]
	public sealed class FollowerComponent : MithrilComponent
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

		public bool enablePositionLag => _positionLagTime > 0f;
		public bool enablePositionMax => _positionMaxDistance > 0f;

		public bool enableRotationLag => _rotationLagTime > 0f;

		#endregion
		#region Methods

		protected override void Awake()
		{
			base.Awake();

			anchor = new GameObject($"{gameObject.name} [Anchor]").transform;
			anchor.SetParent(null);

			anchor.position = transform.position;
			anchor.rotation = transform.rotation;
		}

		private void Update()
		{
			if (followPosition)
			{
				if (enablePositionLag)
				{
					var targetPosition = transform.parent.position;
					var deltaPosition = targetPosition - anchor.position;

					Vector3 startPosition;
					if (deltaPosition.magnitude >= positionMaxDistance)
						startPosition = targetPosition - deltaPosition.normalized * positionMaxDistance;
					else
						startPosition = anchor.position;

					anchor.position = Vector3.SmoothDamp(startPosition, targetPosition, ref _positionVelocity, positionLagTime);
				}
				else
					anchor.position = transform.parent.position;
			}

			if (followRotation)
			{
				if (enableRotationLag)
				{
					anchor.eulerAngles = Math.SmoothDampEulerAngles(anchor.eulerAngles, transform.parent.eulerAngles, ref _rotationVelocity, _rotationLagTime);
				}
				else
					anchor.rotation = transform.rotation;
			}

			transform.position = anchor.position;
			transform.rotation = anchor.rotation;
		}

		#endregion
	}

	#endregion
}
