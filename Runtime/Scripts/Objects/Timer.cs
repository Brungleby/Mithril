
/** Timer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Cuberoot
{
	#region (class) Timer

	/// <summary>
	/// This is an object that can be used as a shorthand for creating animations within an existing script.
	///</summary>

	[Serializable]

	public class Timer
	{
		#region Construction

		// public Timer(float duration)
		// {
		// 	this.duration = duration;
		// }
		// public Timer()
		// {
		// 	duration = 0f;
		// }

		#endregion
		#region Fields

		#region Loops

		/// <summary>
		/// The number of loops to perform before stopping. Set to 0 for infinite loops.
		///</summary>
		[Tooltip("The number of loops to perform before stopping. Set to 0 for infinite loops.")]
		[SerializeField]

		public int loopCount = 1;

		#endregion

		#region Duration

		/// <summary>
		/// The overall duration of this <see cref="Timer"/>. <see cref="OnCease"/> is invoked once this amount of time has elapsed.
		///</summary>
		[Tooltip("The overall duration of this Timer. OnCease is invoked once this amount of time has elapsed.")]
		[SerializeField]

		public float duration = 0f;

		#endregion

		#region Curves

		/// <summary>
		/// The list of curves that this <see cref="Timer"/> oversees.
		///</summary>
		[Tooltip("The list of curves that this Timer oversees.")]
		[SerializeField]

		private AnimationCurve[] _Curves = new AnimationCurve[0];
		public AnimationCurve[] curves => _Curves;

		#endregion

		#region OnStart

		/// <summary>
		/// This event is called when this <see cref="Timer"/> is manually Started.
		///</summary>
		[Tooltip("This event is called when this Timer is manually Started.")]
		[SerializeField]

		private UnityEvent _OnStart = new UnityEvent();
		public UnityEvent OnStart => _OnStart;

		#endregion
		#region OnCease

		/// <summary>
		/// This event is called after this <see cref="Timer"/> has started and has reached its full duration.
		///</summary>
		[Tooltip("This event is called after this Timer has started and has reached its full duration.")]
		[SerializeField]

		private UnityEvent _OnCease = new UnityEvent();
		public UnityEvent OnCease => _OnCease;

		#endregion
		#region OnUpdate

		/// <summary>
		/// This event is called each time <see cref="Update"/> is called and this <see cref="_isPlaying"/>. The <see cref="currentTime"/> is passed as a parameter.
		///</summary>
		[Tooltip("This event is called each time Update is called and this Timer is playing.. The current time is passed as a parameter.")]
		[SerializeField]

		private UnityEvent<float> _OnUpdate = new UnityEvent<float>();
		public UnityEvent<float> OnUpdate => _OnUpdate;

		#endregion
		#region OnCycle

		/// <summary>
		/// This event is called each time this <see cref="Timer"/> has started and has reached its duration, if it is set to loop.
		///</summary>
		[Tooltip("This event is called each time this Timer has started and has reached its duration, if it is set to loop.")]
		[SerializeField]

		private UnityEvent<int> _OnCycle = new UnityEvent<int>();
		public UnityEvent<int> OnCycle = new UnityEvent<int>();

		#endregion

		#endregion
		#region Members

		private bool _isPlaying;
		public bool isPlaying => _isPlaying;

		private float _whenStarted;

		#endregion
		#region Properties

		/// <returns>
		/// The current time relative to when this <see cref="Timer"/> was last <see cref="Start"/>ed. It cannot exceed the <see cref="duration"/>.
		///</returns>

		public float currentTime =>
			(Time.time - _whenStarted).Min(duration);

		// public float currentTimeInLoop =>


		#endregion
		#region Methods

		#region Start

		/// <summary>
		/// Call this method to begin playing this <see cref="Timer"/>.
		///</summary>

		public void Start()
		{
			Restart();
			_OnStart.Invoke();
		}

		#endregion
		#region Restart

		/// <summary>
		/// Call this method to begin playing this <see cref="Timer"/> without triggering any events.
		///</summary>

		public void Restart()
		{
			_isPlaying = true;
			_whenStarted = Time.time;
		}

		#endregion
		#region Cease

		/// <summary>
		/// Calling this method will stop this <see cref="Timer"/> and trigger its <see cref="_OnCease"/> event.
		///</summary>

		public void Cease()
		{
			Cancel();
			_OnCease.Invoke();
		}

		#endregion
		#region Cancel

		/// <summary>
		/// Calling this method will stop this <see cref="Timer"/> without triggering any events.
		///</summary>

		public void Cancel()
		{
			_isPlaying = false;
		}

		#endregion
		#region Update

		/// <summary>
		/// This method should be called every relevent update. It may be placed inside either a <see cref="MonoBehaviour.Update"/> function or a <see cref="MonoBehaviour.FixedUpdate"/> function.
		///</summary>

		public void Update()
		{
			if (_isPlaying)
			{
				_OnUpdate.Invoke(currentTime);

				if (Time.time > _whenStarted + duration)
					Cease();
			}
		}

		#endregion
		#region Evaluate

		/// <summary>
		/// This function returns the corresponding Y value at the given <see cref="currentTime"/> for the provided Curve index.
		///</summary>
		/// <param name="i">
		/// The index of the Curve which to evaluate.
		///</param>

		public float Evaluate(int i = 0) =>
			_Curves[i].Evaluate(currentTime);

		#endregion
		#endregion
	}
	#endregion
}
