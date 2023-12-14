
/** Interactor.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;
using UnityEngine.Events;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

#endregion

namespace Mithril
{
	#region Interactor

	/// <summary>
	/// A component that allows a player to interact with <see cref="InteractableSimple"/>s via an input event.
	///</summary>

	public class Interactor : CasterComponent
	{
		[Min(0f)]
		[SerializeField]
		public float sensorLength = 1f;

		[Header("Events")]

		/// <summary>
		/// This event is called when the focused interactable changes.
		///</summary>
		[Tooltip("This event is called when the focused interactable changes.")]
		[SerializeField]
		public UnityEvent<Interactable> onFocusChanged;

		[Header("Audio")]

		[SerializeField]
		public AudioSource audioSource;

		[SerializeField]
		public AudioClip SFXSuccess;

		[SerializeField]
		public AudioClip SFXFailure;

		private HitPool hitPool = new(1);

		private Interactable _focus;
		public Interactable focus
		{
			get => _focus; private set
			{
				if (_focus == value) return;
				_focus = value;

				_OnFocusChanged();
			}
		}

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
			hitPool.LineCast(transform.position, transform.forward, sensorLength, layers);
			return hitPool.blocked ? (hitPool.nearest.collider?.GetComponent<Interactable>()) : null;
		}

		protected virtual void OnFocusChanged() { }
		private void _OnFocusChanged()
		{
			OnFocusChanged();
			onFocusChanged.Invoke(_focus);
		}
	}

	#endregion
}
