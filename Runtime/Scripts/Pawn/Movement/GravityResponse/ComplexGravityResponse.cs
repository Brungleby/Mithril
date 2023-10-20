
/** ComplexGravityResponse.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Mithril.Pawn
{
	#region ComplexGravityResponse

	/// <summary>
	/// The component can sense when it is inside of a <see cref="GravityField"/> and will add a force based on that field's strength.
	///</summary>

	public sealed class ComplexGravityResponse : GravityResponse<Collider, Rigidbody, Vector3>
	{
		#region Members

		private List<GravityField<Vector3>> fieldsEntered = new List<GravityField<Vector3>>();
		private (int, Vector3, int)[] channelForces = new (int, Vector3, int)[8];

		#endregion
		#region Properties

		private Vector3 _force;
		public override Vector3 force => _force;

		public override Vector3 up => -force.normalized;

		#endregion
		#region Methods

		protected override void Awake()
		{
			base.Awake();

			rigidbody.useGravity = false;
		}

		protected override void FixedUpdate()
		{
			_force = GetForce();
			rigidbody.AddForce(_force, ForceMode.Acceleration);
		}

		private Vector3 GetForce()
		{
			if (fieldsEntered.Count == 0) return default;

			var baseStrength = Physics.gravity.magnitude;
			Vector3 cumulativeForce = default;
			var appliedFields = new HashSet<GravityField<Vector3>>();

			foreach (var iField in fieldsEntered)
			{
				if (appliedFields.Contains(iField)) continue;
				appliedFields.Add(iField);

				var iChannel = iField.channel;
				var iPriority = iField.priority;

				if (iPriority > channelForces[iChannel].Item1)
				{
					channelForces[iChannel].Item2 = iField.GetForceAtPosition(transform.position);
					channelForces[iChannel].Item1 = iPriority;
				}
				else if (iPriority == channelForces[iChannel].Item1)
				{
					channelForces[iChannel].Item2 += iField.GetForceAtPosition(transform.position);
					channelForces[iChannel].Item3++;
				}
			}

			for (int i = 0; i < channelForces.Length; i++)
			{
				if (channelForces[i].Item3 == 0) continue;

				cumulativeForce += channelForces[i].Item2 / channelForces[i].Item3;

				channelForces[i] = (int.MinValue, default, 0);
			}

			return cumulativeForce * baseStrength;
		}

		private void OnTriggerEnter(Collider other)
		{
			var otherField = other.GetComponent<GravityField<Vector3>>();
			if (otherField == null) return;

			fieldsEntered.Add(otherField);
		}

		private void OnTriggerExit(Collider other)
		{
			var otherField = other.GetComponent<GravityField<Vector3>>();
			if (otherField == null) return;

			fieldsEntered.Remove(otherField);
		}

		#endregion
	}
	#endregion
}
