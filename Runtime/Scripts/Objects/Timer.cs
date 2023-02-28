
/** Timer.cs
*
*	Created by LIAM WOFFORD, USA-TX, for the Public Domain.
*
*	Repo: https://github.com/Brungleby/Cuberoot
*	Kofi: https://ko-fi.com/brungleby
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
	#region (class) Timeline

	/// <summary>
	/// This is an object that can be used as a shorthand for creating animations within an existing script.
	///</summary>

	[Serializable]

	public class Timer
	{
		#region Construction

		// public Timeline(float duration)
		// {
		// 	Duration = duration;
		// }
		// public Timeline()
		// {
		// 	Duration = 0f;
		// }

		#endregion
		#region Fields

		#region Duration

		/// <summary>
		/// The overall duration of this Timeline. <see cref="OnCease"/> is invoked once this amount of time has elapsed.
		///</summary>
		[Tooltip("The overall duration of this Timeline. OnCease is invoked once this amount of time has elapsed.")]
		[SerializeField]

		public float Duration = 0f;

		#endregion

		#region Curves

		/// <summary>
		/// The list of curves that this Timeline oversees.
		///</summary>
		[Tooltip("The list of curves that this Timeline oversees.")]
		[SerializeField]

		private AnimationCurve[] _Curves = new AnimationCurve[0];
		public AnimationCurve[] Curves => _Curves;

		#endregion

		#region OnStart

		/// <summary>
		/// This event is called when this Timeline is manually Started.
		///</summary>
		[Tooltip("This event is called when this Timeline is manually Started.")]
		[SerializeField]

		private UnityEvent _OnStart = new UnityEvent();
		public UnityEvent OnStart => _OnStart;

		#endregion
		#region OnCease

		/// <summary>
		/// This event is called after this Timeline has started and has reached its full duration.
		///</summary>
		[Tooltip("This event is called after this Timeline has started and has reached its full duration.")]
		[SerializeField]

		private UnityEvent _OnCease = new UnityEvent();
		public UnityEvent OnCease => _OnCease;

		#endregion
		#region OnUpdate

		/// <summary>
		/// This event is called each time <see cref="Update"/> is called and this <see cref="_isPlaying"/>. The <see cref="currentTime"/> is passed as a parameter.
		///</summary>
		[Tooltip("This event is called each time Update is called and this Timeline is playing.. The current time is passed as a parameter.")]
		[SerializeField]

		private UnityEvent<float> _OnUpdate = new UnityEvent<float>();
		public UnityEvent<float> OnUpdate => _OnUpdate;

		#endregion

		#endregion
		#region Members

		private bool _isPlaying;
		public bool isPlaying => _isPlaying;

		private float _whenStarted;

		#endregion
		#region Properties

		#region CurrentTime

		/// <returns>
		/// The current time relative to when this Timeline was last <see cref="Start"/>ed. It cannot exceed the <see cref="Duration"/>.
		///</returns>

		public float currentTime =>
			(Time.time - _whenStarted).Min(Duration);

		#endregion

		#endregion
		#region Methods

		#region Start

		/// <summary>
		/// Call this method to begin playing this Timeline.
		///</summary>

		public void Start()
		{
			Restart();
			_OnStart.Invoke();
		}

		#endregion
		#region Restart

		/// <summary>
		/// Call this method to begin playing this Timeline without triggering any events.
		///</summary>

		public void Restart()
		{
			_isPlaying = true;
			_whenStarted = Time.time;
		}

		#endregion
		#region Cease

		/// <summary>
		/// Calling this method will stop this Timeline and trigger its <see cref="_OnCease"/> event.
		///</summary>

		public void Cease()
		{
			Cancel();
			_OnCease.Invoke();
		}

		#endregion
		#region Cancel

		/// <summary>
		/// Calling this method will stop this Timeline without triggering any events.
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

				if (Time.time > _whenStarted + Duration)
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
