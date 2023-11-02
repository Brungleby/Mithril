
/** Interactor.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

#endregion

namespace Mithril
{
	#region Interactor

	public class Interactor : CasterComponent
	{
		[Min(0f)]
		[SerializeField]
		public float sensorLength = 1f;

		[Header("Audio")]

		[AutoAssign]
		[SerializeField]
		public AudioSource audioSource;

		[SerializeField]
		public AudioClip SFXSuccess;

		[SerializeField]
		public AudioClip SFXFailure;

		public Interactable focus { get; private set; }

		public virtual void OnInteract(InputContext context)
		{
			if (context.started)
				TryInteract();
		}

		public Interaction TryInteract() => TryInteractWith(focus);

		public Interaction TryInteractWith(Interactable other)
		{
			if (other == null) return Interaction.ignore;
			var result = other._OnInteract(this);

			if (result.doFeedback)
				audioSource.PlayOneShot(result ? SFXSuccess : SFXFailure);

			return result;
		}

		protected virtual void Update()
		{
			focus = Sense();
		}

		protected virtual Interactable Sense()
		{
			var hit = Hit.Linecast(transform.position, transform.forward, sensorLength, layers);
			hit.Draw(DebugDrawEnvironment.DrawType.SingleUpdate);
			return hit.collider?.GetComponent<Interactable>();
		}
	}

	#endregion
}
