
/** TimerEvent.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// A component that waits for a specified amount of time before triggering an event. This event can occur a specified or an unspecified (infinite) number of times.
	///</summary>

	public class TimerEvent : MonoBehaviour
	{
		#region Fields

		#region Duration

		/// <summary>
		/// The length of time per cycle.
		///</summary>
		[Tooltip("The length of time per cycle.")]
		[Min(0f)]
		[SerializeField]

		public float Duration = 1.0f;

		#endregion
		#region Cycles

		/// <summary>
		/// The number of cycles to perform before this event will stop.
		///</summary>
		[Tooltip("The number of cycles to perform before this event will stop. Set to 0 for infinite.")]
		[Min(0)]
		[SerializeField]

		public int Cycles = 1;

		#endregion

		#region PlayOnStart

		/// <summary>
		/// If enabled, this event will begin playing on <see cref="Start"/>.
		///</summary>
		[Tooltip("If enabled, this TimedEvent will begin playing on its Start().")]
		[SerializeField]

		public bool PlayOnStart = false;

		#endregion
		#region PauseOnCycle

		/// <summary>
		/// If enabled, this event will pause playing (but not stop) each time a cycle is completed.
		///</summary>
		[Tooltip("If enabled, this TimedEvent will pause playing (but not stop) each time a cycle is completed.")]
		[SerializeField]

		public bool PauseOnCycle = false;

		#endregion

		#region OnCycleCompleted

		/// <summary>
		/// This event is invoked EACH TIME a cycle elapses.
		///</summary>
		[Tooltip("This event is invoked EACH TIME a cycle elapses.")]

		public UnityEvent<int> OnCycleCompleted;

		#endregion
		#region OnEventCompleted

		/// <summary>
		/// This event is invoked once ALL cycles have elapsed.
		///</summary>
		[Tooltip("This event is invoked once ALL cycles have elapsed. This will never be invoked if there is an infinite (0) number of cycles.")]

		public UnityEvent OnEventCompleted;

		#endregion

		#endregion
		#region Private Members

		#region PlayState

		/// <summary>
		/// The current play state this event is in.
		///</summary>

		private EPlaybackState _playState = EPlaybackState.Stop;

		#endregion

		#region WhenStarted

		/// <summary>
		/// The recorded time at which the timer began playing.
		///</summary>
		private float _WhenStarted;

		#endregion
		#region WhenCycled

		/// <summary>
		/// The recorded time at which the timer last lapsed a cycle.
		///</summary>
		private float _WhenCycled;

		#endregion
		#region WhenPaused

		/// <summary>
		/// The recorded time at which the timer paused.
		///</summary>
		private float _WhenPaused;

		#endregion

		#endregion

		#region Properties

		#region IsInfinite

		/// <summary>
		/// Whether or not this timer will cycle infinitely.
		///</summary>

		public bool IsInfinite { get => Cycles == 0; }

		#endregion
		#region TotalDuration

		/// <summary>
		/// The total amount of time this timer will take to complete ALL cycles. If this <see cref="IsInfinite"/>, it will be the duration of just one cycle.
		///</summary>

		public float TotalDuration => IsInfinite ? Duration : Duration * Cycles;

		#endregion

		#region IsPlaying

		/// <summary>
		/// Whether or not timer is playing. This value will be false if this event is paused or stopped.
		///</summary>

		public bool isPlaying
		{
			get => _playState == EPlaybackState.Play;
			set { if (value) Play(); else Pause(); }
		}

		#endregion
		#region ElapsedCycles

		/// <summary>
		/// The number of cycles that have already completed.
		///</summary>

		public int elapsedCycles
		{
			get => Mathf.FloorToInt(elapsedTime / Duration);
			set => elapsedTime = (Duration * value) + elapsedTimeInCycle;
		}

		#endregion
		#region ElapsedTime

		/// <summary>
		/// The total amount of time that has already passed since the timer started.
		///</summary>

		public float elapsedTime
		{
			get
			{
				switch (_playState)
				{
					case EPlaybackState.Play:
						return Time.time - _WhenStarted;
					case EPlaybackState.Pause:
						return _WhenPaused - _WhenStarted;
					case EPlaybackState.Stop:
						return 0f;
				}

				return 0f;
			}
			set
			{
				switch (_playState)
				{
					case EPlaybackState.Play:
						_WhenStarted = Time.time - value;
						break;
					case EPlaybackState.Pause:
						_WhenStarted = _WhenPaused - value;
						break;
					case EPlaybackState.Stop:
						_playState = EPlaybackState.Pause;
						_WhenStarted = Time.time - value;
						_WhenPaused = Time.time;
						break;
				}
			}
		}

		#endregion
		#region PercentTime

		/// <summary>
		/// Returns the percent of time that has passed since the timer started between 0 and the <see cref="TotalDuration"/>."
		///</summary>

		public float percentTime
		{
			get => elapsedTime / TotalDuration;
			set => elapsedTime = value * TotalDuration;
		}

		#endregion
		#region ElapsedTimeInCycle

		/// <summary>
		/// Returns the amount of time that has passed since the last cycle.
		///</summary>

		public float elapsedTimeInCycle
		{
			get => elapsedTime % Duration;
			set => elapsedTime = Mathf.Clamp(value, 0f, Duration) + (Duration * elapsedCycles);
		}

		#endregion
		#region PercentTimeInCycle

		/// <summary>
		/// Returns the percent of time that has passed since the last cycle.
		///</summary>

		public float percentTimeInCycle
		{
			get => (elapsedTime / Duration) % 1f;
			set => elapsedTime = Mathf.Clamp01(value) + elapsedCycles;
		}

		#endregion
		#region PauseDuration

		/// <summary>
		/// Returns the amount of time since this timer was paused.
		///</summary>

		protected float pauseDuration => isPlaying ? 0f : Time.time - _WhenPaused;

		#endregion

		#endregion
		#region Methods

		#region Start

		protected virtual void Start()
		{
			if (PlayOnStart)
				Play();
		}

		#endregion
		#region Update

		protected virtual void Update()
		{
			if (isPlaying && Time.time >= _WhenCycled + Duration)
				OnCompleteCycle();
		}

		#endregion
		#region Play

		/// <summary>
		/// Call this function to begin the timer for this event.
		///</summary>

		public void Play()
		{
			if (isPlaying) return;

			_playState = EPlaybackState.Play;

			if (_playState == EPlaybackState.Pause)
			{
				float pauseDuration = Time.time - _WhenPaused;

				_WhenStarted += pauseDuration;
				_WhenCycled += pauseDuration;
			}
			else
			{
				_WhenStarted = _WhenCycled = Time.time;
			}
		}

		#endregion
		#region Pause

		/// <summary>
		/// Call this function to pause the timer for this event, without completely stopping and resetting it.
		///</summary>

		public void Pause()
		{
			if (_playState == EPlaybackState.Pause) return;

			_playState = EPlaybackState.Pause;
			_WhenPaused = Time.time;
		}

		#endregion
		#region Stop

		/// <summary>
		/// Call this function to stop and reset this event.
		///</summary>

		public void Stop()
		{
			if (_playState == EPlaybackState.Stop) return;

			_playState = EPlaybackState.Stop;
			_WhenPaused = _WhenStarted;
		}

		#endregion
		#region Toggle

		/// <summary>
		/// Call this function to toggle between playing and pausing.
		///</summary>

		public void Toggle()
		{
			isPlaying = !isPlaying;
		}

		#endregion

		#region OnCompleteCycle

		/// <summary>
		/// Function called when a cycle is completed.
		///</summary>

		private void OnCompleteCycle()
		{
			_WhenCycled = Time.time;

			OnCycleCompleted.Invoke(elapsedCycles);

			if (Cycles > 0 && elapsedCycles >= Cycles)
			{
				Stop();
				OnEventCompleted.Invoke();
			}
			else if (PauseOnCycle)
			{
				Pause();
				percentTimeInCycle = 0f;
			}
		}

		#endregion

		#endregion
	}
}
