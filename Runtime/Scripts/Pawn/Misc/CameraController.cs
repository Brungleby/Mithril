
/** CameraController.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/


#region Includes

using UnityEngine;
using Unity.Mathematics;

using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

#endregion

namespace Mithril.Pawn
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[DefaultExecutionOrder(10)]

	public class CameraController : MithrilComponent, IPawnUser<Pawn>
	{
		#region Public Fields

		[SerializeField]
		private Pawn _pawn;
		public Pawn pawn { get => _pawn; set => _pawn = value; }

		[Space]

		/// <summary>
		/// Rotation amount to apply per input unit.
		///</summary>

		[Tooltip("Rotation amount to apply per input unit.")]

		public Vector2 speed = Vector2.one * 100f;

		public Vector2 mouseSpeed = Vector2.one * 100f;

		[Space]

		/// <summary>
		/// Max angular speed.
		///</summary>
		[Tooltip("Max angular speed. Useful if you're using a camera with a RotationFollower and a mouse.")]
		[Min(0f)]
		public Vector2 speedLimit = Vector2.one * 15f;

		[Range(-90f, 90f)]
		[SerializeField]
		private float _minPitchLimit = -90f;

		public float minPitchLimit
		{
			get => _minPitchLimit;
			set => _minPitchLimit = Mathf.Clamp(value, -90f, 90f);
		}

		[Range(-90f, 90f)]
		[SerializeField]
		private float _maxPitchLimit = 90f;

		public float maxPitchLimit
		{
			get => _maxPitchLimit;
			set => _maxPitchLimit = Mathf.Clamp(value, -90f, 90f);
		}

		/// <summary>
		/// Lock state to assign to the cursor on awake.
		///</summary>
		[Tooltip("Lock state to assign to the cursor on awake.")]
		[SerializeField]
		private CursorLockMode defaultCursorLockState = CursorLockMode.Locked;

		#endregion
		#region Private Members

		private Vector2 _rawMouseInputVector;
		private Vector2 _rawInputVector;

		/// <summary>
		/// The adjusted world input euler angles vector.
		///</summary>

		private Vector2 _inputVector;

		/// <inheritdoc cref="_inputVector"/>

		public Vector2 inputVector => _inputVector;

		private Vector2 _mouseInputVector;
		public Vector2 mouseInputVector => _mouseInputVector;

		#endregion
		#region Properties

		public bool2 enableSpeedLimits => new(speedLimit.x != 0f, speedLimit.y != 0f);

		#endregion
		#region Methods

		protected override void Awake()
		{
			base.Awake();
			Cursor.lockState = defaultCursorLockState;
		}

		protected virtual void Update()
		{
			_inputVector = _rawInputVector;
			_mouseInputVector = _rawMouseInputVector;

			/** <<============================================================>> **/
			/**	Don't apply Time.deltaTime to the mouse vector. It is automatically accounted for in input.
			*/

			Vector3 deltaAngles =
				(Vector3.Scale(_inputVector, speed) * Time.deltaTime) +
				(Vector3.Scale(_mouseInputVector, mouseSpeed) / 100f)
			;

			if (enableSpeedLimits.x)
				deltaAngles.x = deltaAngles.x.ClampAbs(speedLimit.x);
			if (enableSpeedLimits.y)
				deltaAngles.y = deltaAngles.y.ClampAbs(speedLimit.y);

			/** <<============================================================>> **/

			Vector3 selfLocalEulerAngles = transform.localEulerAngles;
			{
				selfLocalEulerAngles += deltaAngles;
				selfLocalEulerAngles.x = selfLocalEulerAngles.x.ClampAngle(_minPitchLimit, _maxPitchLimit);
			}

			transform.localEulerAngles = selfLocalEulerAngles;
		}

		public virtual void ResetRotation()
		{
			float y = pawn.transform.eulerAngles.y;
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
		}

		public void OnInputCamera_Mouse(InputContext context)
		{
			Vector2 input = context.ReadValue<Vector2>();
			Math.SwapValues(ref input.x, ref input.y);
			_rawMouseInputVector = input;
		}

		/// <summary>
		/// Updates the <see cref="_rawInputVector"/> from a <see cref="Vector2"/>.
		///</summary>

		public void OnInputCamera_Biaxial(InputContext context)
		{
			Vector2 input = context.ReadValue<Vector2>();
			Math.SwapValues(ref input.x, ref input.y);
			_rawInputVector = input;
		}

		/// <summary>
		/// Updates the <see cref="_rawInputVector"/> from a <see cref="Vector3"/>.
		///</summary>

		public void OnInputCamera_Triaxial(InputContext context)
		{
			_rawInputVector = context.ReadValue<Vector3>();
		}

		public void OnInputReset(InputContext context)
		{
			if (context.started)
				ResetRotation();
		}

		#endregion
	}
}
