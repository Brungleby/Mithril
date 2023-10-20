
/** ActionComponent.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

#endregion

namespace Mithril.Pawn
{
	#region ActionComponent

	public abstract class ActionComponent : MithrilComponent
	{
		#region Inners

		#region EActivationType

		/// <summary>
		/// Defines whether an Action should be treated like like a Button or a Toggle switch.
		///</summary>

		private enum EActionType
		{
			/// <summary>
			/// The action will always remain active until the input is triggered twice (or the condition fails).
			///</summary>

			[Tooltip("The action will always remain active until the input is triggered twice (or the condition fails).")]

			Toggle,

			/// <summary>
			/// The action will only be active as long as the input is held (or the condition fails).
			///</summary>

			[Tooltip("The action will only be active as long as the input is held (or the condition fails).")]

			Button,

			/// <summary>
			/// The action will act like a button if it is held for longer than the specified Delay. Otherwise, it will act like a switch.
			///</summary>

			[Tooltip("The action will act like a button if it is held for longer than the specified Delay. Otherwise, it will act like a switch.")]

			Auto,
		}

		#endregion
		#region Handler

		[System.Serializable]

		private sealed class Handler
		{
			#region Fields

			/// <summary>
			/// The type of action this component will employ.
			///</summary>
			[Tooltip("The type of action this component will employ.")]
			[SerializeField]
			public EActionType actionType = EActionType.Button;

			/// <summary>
			/// If action Type is set to Auto, this is the maximum duration the input can be pressed to be treated like a toggle.
			///</summary>
			[Tooltip("If action Type is set to Auto, this is the maximum duration the input can be pressed to be treated like a toggle.")]
			[SerializeField]
			public float autoDelay = 0.2f;

			/// <summary>
			/// If enabled, this action will remain in its state if the component is disabled. If disabled, the action will be forced to cancel when the component is disabled.
			///</summary>
			[Tooltip("If enabled, this action will remain in its state if the component is disabled. If disabled, the action will be forced to cancel when the component is disabled.")]
			[SerializeField]
			public bool persistOnDisabled = false;

			#endregion
		}

		#endregion

		#endregion
		#region Fields

		[SerializeField]
		private Handler _handler;

		#endregion
		#region Members

		private float _whenActivated;

		#endregion
		#region Properties

		/// <summary>
		/// Whether or not the action is currently being performed.
		///</summary>
		private bool _activated;
		/// <inheritdoc cref="_activated"/>
		public bool activated
		{
			get => _activated; set
			{
				if (!enabled || _activated == value) return;

				if (value)
				{
					if (!mayBegin)
					{
						OnActivationFailed();
						return;
					}

					_activated = true;
					_whenActivated = Time.time;
					OnActivated();
				}
				else
				{
					_activated = false;
					OnDeactivated();
				}
			}
		}

		/// <returns>
		/// TRUE if the action may be performed, e.g. stamina is full.
		///</returns>

		protected virtual bool mayBegin => true;

		/// <returns>
		/// TRUE if the action must stop being performed, e.g. running out of stamina.
		///</returns>

		protected virtual bool mustCease => !mayBegin;

		private bool useToggle =>
			_handler.actionType == EActionType.Button ?
				false : _handler.actionType == EActionType.Toggle ?
				true : Time.time < _whenActivated + _handler.autoDelay
		;

		#endregion
		#region Methods

		protected virtual void Update()
		{
			if (activated && mustCease)
				activated = false;
		}

		protected virtual void OnDisable()
		{
			if (!_handler.persistOnDisabled)
				ForceSetActivated(false);
		}

		/// <summary>
		/// Force sets the activation to the specified <paramref name="value"/>.
		///</summary>

		public void ForceSetActivated(bool value)
		{
			if (value)
				OnActivated();
			else
				OnDeactivated();

			_activated = value;
		}

		public void Toggle()
		{
			activated = !activated;
		}

		protected virtual void OnActivated() { }
		protected virtual void OnActivationFailed() { }
		protected virtual void OnDeactivated() { }

		public void OnInputAction(InputContext context)
		{
			if (useToggle)
			{
				if (context.started)
					Toggle();
			}
			else
			{
				if (context.started)
					activated = true;
				else if (context.canceled)
					activated = false;
			}
		}

		#endregion
	}

	#endregion
}
