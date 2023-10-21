
/** PawnDescriptor.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril.Pawn
{
	#region PawnBase

	public abstract class PawnBase : MithrilComponent
	{
		#region Fields

		/// <summary>
		/// Transform from which to base our input around.
		///</summary>
		[Tooltip("Transform from which to base our input around. This should be the player's currently active camera.")]
		[SerializeField]

		public Transform viewTransform;

		/// <summary>
		/// Denotes the thickness around this <see cref="UnityEngine.Collider"/> that can be cast in a <see cref="PawnSensorBase"/>.
		///</summary>
		[Tooltip("This value is used in PawnSensor components. It denotes the thickness around this Pawn's Collider that will be cast. Smaller numbers are stiffer but more accurate, larger numbers are looser but less accurate.")]
		[Min(0f)]
		[SerializeField]

		private float _skinWidth = 0.01f;
		public float skinWidth { get => _skinWidth; set => _skinWidth = value.Clamp(0f, maxSkinWidth); }

		#endregion
		#region Members

		/// <summary>
		/// Record of <see cref="Time.fixedDeltaTime"/> from the previous frame.
		///</summary>

		protected float fixedDeltaTime_Previous { get; private set; }

		#endregion
		#region Properties

		protected abstract float maxSkinWidth { get; }

		public abstract float height { get; set; }

		public abstract float speed { get; set; }

		#endregion
		#region Methods

		private void OnValidate()
		{
			skinWidth = _skinWidth;
		}

		protected virtual void FixedUpdate()
		{
			fixedDeltaTime_Previous = Time.fixedDeltaTime;
		}

		#endregion
	}

	#endregion
	#region PawnBase<TCollider, TRigidbody, TMovementSpace>

	/// <summary>
	/// Base class for a pawn, which is defined as an object that has a collider, a rigidbody, and is designed to move independently in a space.
	///</summary>

	public abstract class PawnBase<TColliderBase, TRigidbody, TVector> : PawnBase, IColliderUser<TColliderBase>, IRigidbodyUser<TRigidbody>
	where TVector : unmanaged
	{
		#region Members

		[SerializeField]
		[AutoAssign]
		private TColliderBase _collider;
#pragma warning disable
		public new TColliderBase collider => _collider;
#pragma warning restore
		[SerializeField]
		[AutoAssign]
		private TRigidbody _rigidbody;
#pragma warning disable
		public new TRigidbody rigidbody => _rigidbody;
#pragma warning restore

		[AutoAssign]
		public GravityResponse<TColliderBase, TRigidbody, TVector> gravityResponse { get; protected set; }

		/// <summary>
		/// Record of <see cref="velocity"/> from the previous frame.
		///</summary>

		protected TVector _velocity_Previous = default;

		#endregion

		#region Properties

		/// <summary>
		/// When inputting controls to this Pawn, this is the direction which this Pawn perceives as right.
		///</summary>

		public abstract TVector right { get; }

		/// <summary>
		/// When inputting controls to this Pawn, this is the direction which this Pawn perceives as up.
		///</summary>

		public abstract TVector up { get; }

		/// <inheritdoc cref="_velocity"/>

		public abstract TVector velocity { get; set; }

		/// <summary>
		/// The current uniaxial velocity of this Pawn, relative to <see cref="up"/>.
		///</summary>

		public abstract float verticalVelocityRelative { get; set; }

		/// <summary>
		/// The absolute value of <see cref="verticalVelocityRelative"/>.
		///</summary>

		public float verticalSpeed
		{
			get => verticalVelocityRelative.Abs();
			set => verticalVelocityRelative = System.Math.Max(value, 0f) * verticalVelocityRelative.Sign();
		}

		/// <summary>
		/// The current acceleration of the <see cref="rigidbody"/> based on its <see cref="velocity"/>.
		///</summary>

		public abstract TVector acceleration { get; set; }

		#endregion
		#region Methods

		protected override void FixedUpdate()
		{
			base.FixedUpdate();

			_velocity_Previous = velocity;
		}

		/** <<============================================================>> **/

		/**
		*   __TODO_REVIEW__
		*   These functions don't need to use an input position/velocity...or do they?
		*/

		/// <summary>
		/// Predicts where this Pawn will be in a specified number of <paramref name="frames"/>.
		///</summary>

		public abstract TVector ExtrapolatePosition(in TVector position, in TVector velocity, in float deltaTime, int frames = 1);

		public abstract TVector ProjectVelocityOntoSurface(in TVector position, in TVector velocity, in TVector planeNormal);

		public abstract TVector ProjectVelocityOntoSurface(in TVector planeNormal);

		/** <<============================================================>> **/

		protected virtual void OnDrawGizmosSelected()
		{
#if UNITY_EDITOR
			UnityEditor.Handles.Label(transform.position - Vector3.up * ((height / 2f) + 0.25f), speed.ToString("0.0"));
#endif
		}

		#endregion
	}
	#endregion

	#region Pawn (3D)

	public abstract class Pawn : PawnBase<Collider, Rigidbody, Vector3>
	{
		#region Properties

		public override Vector3 right => transform.right;
		public override Vector3 up => -Physics.gravity.normalized;

		/// <summary>
		/// When inputting controls to this Pawn, this is the direction which this Pawn perceives as right.
		///</summary>

		public Vector3 forward => transform.forward;

		public sealed override Vector3 velocity
		{
			get => rigidbody.velocity;
			set => rigidbody.velocity = value;
		}

		public Vector3 velocityRelative
		{
			get => lateralVelocityRelative.XZ() + Vector3.up * verticalVelocityRelative;
		}

		public sealed override float speed
		{
			get => velocity.magnitude;
			set => velocity = velocity.normalized * value;
		}

		/// <summary>
		/// The current biaxial velocity of this Pawn, relative to <see cref="forward"/> and <see cref="right"/>.
		///</summary>

		public Vector2 lateralVelocityRelative
		{
			get => new(
				Vector3.Dot(velocity, forward),
				Vector3.Dot(velocity, right)
			);
			set => velocity = verticalVelocity +
				(forward * value.x) +
				(right * value.y)
			;
		}

		/// <summary>
		/// The vector in 3D space that represents only the <see cref="lateralVelocityRelative"/>.
		///</summary>

		public Vector3 lateralVelocity =>
			Vector3.Scale(velocity, Vector3.one - up);

		/// <summary>
		/// The absolute value magnitude of <see cref="lateralVelocityRelative"/>.
		///</summary>

		public float lateralSpeed
		{
			get => lateralVelocityRelative.magnitude;
			set => lateralVelocityRelative = lateralVelocityRelative.normalized * value;
		}

		public sealed override float verticalVelocityRelative
		{
			get => Vector3.Dot(velocity, up);
			set => velocity = lateralVelocity +
				(up * value);
		}

		/// <summary>
		/// The vector in 3D space that represents only the <see cref="verticalVelocityRelative"/>.
		///</summary>

		public Vector3 verticalVelocity =>
			Vector3.Scale(velocity, up);

		public sealed override Vector3 acceleration
		{
			get => (velocity - _velocity_Previous) / fixedDeltaTime_Previous;
			set => throw new System.NotImplementedException();
		}

		#endregion
		#region Methods

		public sealed override Vector3 ExtrapolatePosition(in Vector3 position, in Vector3 velocity, in float deltaTime, int frames = 1)
		{
			return position + (velocity * deltaTime * frames);
		}

		public sealed override Vector3 ProjectVelocityOntoSurface(in Vector3 planeNormal)
		{
			return ProjectVelocityOntoSurface(rigidbody.position, rigidbody.velocity, planeNormal);
		}

		public sealed override Vector3 ProjectVelocityOntoSurface(in Vector3 position, in Vector3 velocity, in Vector3 planeNormal)
		{
			Vector3 projected_current = Vector3.ProjectOnPlane(position, planeNormal);
			Vector3 projected_next = Vector3.ProjectOnPlane(ExtrapolatePosition(position, velocity, Time.fixedDeltaTime, 5), planeNormal);

			return (projected_next - projected_current).normalized * velocity.magnitude;
		}

		#endregion
		#region Debug

		protected override void OnDrawGizmosSelected()
		{
			DebugDraw.DrawArrow(rigidbody.position, rigidbody.velocity, Color.magenta);
		}

		#endregion
	}

	#endregion
	#region Pawn2D

	public abstract class Pawn2D : PawnBase<Collider2D, Rigidbody2D, Vector2>
	{
		#region Properties

		public override Vector2 right => transform.right;
		public override Vector2 up => transform.up;

		public sealed override Vector2 velocity
		{
			get => rigidbody.velocity;
			set => rigidbody.velocity = value;
		}

		public sealed override float speed
		{
			get => velocity.magnitude;
			set => velocity = velocity.normalized * value;
		}

		public float lateralVelocity
		{
			get => Vector3.Dot(velocity, right);
			set => velocity = verticalVelocityWorld +
				(right * value);
		}

		public Vector2 lateralVelocityWorld
		{
			get => Vector2.Scale(velocity, Vector2.one - up);
		}

		public sealed override float verticalVelocityRelative
		{
			get => Vector2.Dot(velocity, up);
			set => velocity = lateralVelocityWorld +
				(up * value);
		}

		public Vector2 verticalVelocityWorld
		{
			get => Vector2.Scale(velocity, up);
		}

		public sealed override Vector2 acceleration
		{
			get => (velocity - _velocity_Previous) / fixedDeltaTime_Previous;
			set => throw new System.NotImplementedException();
		}

		#endregion
		#region Methods

		public sealed override Vector2 ExtrapolatePosition(in Vector2 position, in Vector2 velocity, in float deltaTime, int frames = 1)
		{
			return position + (velocity * deltaTime * frames);
		}

		public sealed override Vector2 ProjectVelocityOntoSurface(in Vector2 planeNormal)
		{
			return ProjectVelocityOntoSurface(rigidbody.position, rigidbody.velocity, planeNormal);
		}

		public sealed override Vector2 ProjectVelocityOntoSurface(in Vector2 position, in Vector2 velocity, in Vector2 planeNormal)
		{
			// Vector2 __projected_current = Vector2.ProjectOnPlane(position, planeNormal);
			// Vector2 __projected_next = Vector2.ProjectOnPlane(ExtrapolatePosition(position, velocity, Time.fixedDeltaTime, 5), planeNormal);

			// return (__projected_next - __projected_current).normalized * velocity.magnitude;
			throw new System.NotImplementedException();
		}

		#endregion
	}

	#endregion

	#region Pawn<TCollider>

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public abstract class Pawn<TCollider> : Pawn, IColliderUser<TCollider>
	where TCollider : Collider
	{
		public new TCollider collider => (TCollider)base.collider;
	}

	#endregion
	#region Pawn2D<TCollider2D>

	public abstract class Pawn2D<TCollider2D> : Pawn2D, IColliderUser<TCollider2D>
	where TCollider2D : Collider2D
	{
		public new TCollider2D collider => (TCollider2D)base.collider;
	}

	#endregion
	#region IPawnUser

	public interface IPawnUser<TPawn>
	{
		TPawn pawn { get; }
	}

	#endregion
}
