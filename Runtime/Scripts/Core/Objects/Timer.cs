
/** Timer.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

#endregion

namespace Mithril
{
	#region Timer

	/// <summary>
	/// This is an object that can be used as a shorthand for creating animations within an existing script.
	///</summary>
	[Serializable]
	public class Timer : object
	{
		#region Inners

		#region PropertyDrawer
#if UNITY_EDITOR
		[CustomPropertyDrawer(typeof(Timer))]
		protected internal class TimerPropertyDrawer : PropertyDrawer
		{
			protected const float TOGGLE_WIDTH = 16f;
			protected const float DURATION_WIDTH = 50f;

			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				EditorGUI.BeginProperty(position, label, property);
				EditorGUI.LabelField(position, label);

				var isLoopingLabel = new GUIContent("Loop");
				var w0 = EditorStyles.label.CalcSize(isLoopingLabel).x;
				var x0 = position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
				var r0 = new Rect(x0, position.y, w0, position.height);

				var isLooping = property.FindPropertyRelative("isLooping");
				var w1 = TOGGLE_WIDTH;
				var x1 = x0 + w0 + EditorGUIUtility.standardVerticalSpacing * 2f;
				var r1 = new Rect(x1, position.y, w1, position.height);

				var duration = property.FindPropertyRelative("duration");
				var w3 = DURATION_WIDTH;
				var x3 = position.x + position.width - w3;
				var r3 = new Rect(x3, position.y, w3, position.height);

				var durationLabel = new GUIContent("Duration");
				var w2 = EditorStyles.label.CalcSize(durationLabel).x;
				var x2 = x3 - (w2 + EditorGUIUtility.standardVerticalSpacing);
				var r2 = new Rect(x2, position.y, w2, position.height);

				/** <<============================================================>> **/

				EditorGUI.LabelField(r0, isLoopingLabel);
				isLooping.boolValue = EditorGUI.Toggle(r1, isLooping.boolValue);
				EditorGUI.LabelField(r2, durationLabel);
				duration.floatValue = EditorGUI.FloatField(r3, duration.floatValue);

				EditorGUI.EndProperty();
			}
		}
#endif
		#endregion

		#endregion
		#region Fields

		/// <summary>
		/// Whether or not this timer should loop once started.
		///</summary>
		[Tooltip("Whether or not this timer should loop once started.")]
		[SerializeField]
		public bool isLooping = false;

		/// <summary>
		/// The overall duration of this <see cref="Timer"/>. <see cref="onCease"/> is invoked once this amount of time has elapsed.
		///</summary>
		[Tooltip("The overall duration of this Timer. OnCease is invoked once this amount of time has elapsed.")]
		[SerializeField]
		public float duration = 1.0f;

		/// <summary>
		/// This event is called when this <see cref="Timer"/> is manually Started.
		///</summary>
		[Tooltip("This event is called when this Timer is manually Started.")]
		[SerializeField]
		private UnityEvent _onStart = new();
		public UnityEvent onStart => _onStart;

		/// <summary>
		/// This event is called after this <see cref="Timer"/> has started and has reached its full duration.
		///</summary>
		[Tooltip("This event is called after this Timer has started and has reached its full duration.")]
		[SerializeField]
		private UnityEvent _onCease = new();
		public UnityEvent onCease => _onCease;

		/// <summary>
		/// This event is called each time this <see cref="Timer"/> has started and has reached its duration, if it is set to loop.
		///</summary>
		[Tooltip("This event is called each time this Timer has started and has reached its duration, if it is set to loop.")]
		[SerializeField]
		private UnityEvent<int> _onCycle = new();
		public UnityEvent<int> onCycle => _onCycle;


		#endregion
		#region Members

		private bool _isPlaying = false;
		public bool isPlaying => _isPlaying;

		private float _whenStarted = 0f;
		private int _loopCount;

		#endregion
		#region Properties

		/// <returns>
		/// The current time relative to when this <see cref="Timer"/> was last <see cref="Start"/>ed. It cannot exceed the <see cref="duration"/>.
		///</returns>

		public float currentTime =>
			(Time.time - _whenStarted).Min(duration);

		#endregion
		#region Methods

		public static implicit operator Timer(float duration) => new() { duration = duration };

		/// <summary>
		/// Call this method to begin playing this <see cref="Timer"/>.
		///</summary>

		public void Start()
		{
			Restart();
			_loopCount = 0;
			_onStart.Invoke();
		}

		/// <summary>
		/// Call this method to begin playing this <see cref="Timer"/> without triggering any events.
		///</summary>

		public void Restart()
		{
			_isPlaying = true;
			_whenStarted = Time.time;
		}

		/// <summary>
		/// Calling this method will stop this <see cref="Timer"/> and trigger its <see cref="_onCease"/> event.
		///</summary>

		public void Cease()
		{
			Cancel();
			_onCease.Invoke();
		}

		/// <summary>
		/// Calling this method will stop this <see cref="Timer"/> without triggering any events.
		///</summary>

		public void Cancel()
		{
			_isPlaying = false;
		}

		/// <summary>
		/// This method should be called every relevent update. It may be placed inside either a <see cref="MonoBehaviour.Update"/> function or a <see cref="MonoBehaviour.FixedUpdate"/> function.
		///</summary>

		public void Update()
		{
			if (_isPlaying)
			{
				while (Time.time > _whenStarted + duration)
				{
					if (isLooping)
					{
						_whenStarted += duration;
						_loopCount++;

						_onCycle.Invoke(_loopCount);
					}
					else
					{
						Cease();
						break;
					}
				}
			}
		}

		#endregion
	}
	#endregion
	#region CurveTimer

	[Serializable]
	public class CurveTimer : Timer
	{
		#region Inner

		[CustomPropertyDrawer(typeof(CurveTimer))]
		protected internal class CurveTimerPropertyDrawer : TimerPropertyDrawer
		{
			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				EditorGUI.BeginProperty(position, label, property);
				EditorGUI.LabelField(position, label);

				var isLoopingLabel = new GUIContent("Loop");
				var w0 = EditorStyles.label.CalcSize(isLoopingLabel).x;
				var x0 = position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
				var r0 = new Rect(x0, position.y, w0, position.height);

				var isLooping = property.FindPropertyRelative("isLooping");
				var w1 = TOGGLE_WIDTH;
				var x1 = x0 + w0 + EditorGUIUtility.standardVerticalSpacing * 2f;
				var r1 = new Rect(x1, position.y, w1, position.height);

				var w3 = DURATION_WIDTH;

				var curve = property.FindPropertyRelative("curve");
				var w2 = position.width - (EditorGUIUtility.labelWidth + w0 + w1 + w3 + EditorGUIUtility.standardVerticalSpacing * 6f);
				var x2 = x1 + w1 + EditorGUIUtility.standardVerticalSpacing;
				var r2 = new Rect(x2, position.y, w2, position.height);

				var duration = property.FindPropertyRelative("duration");
				var x3 = x2 + w2 + EditorGUIUtility.standardVerticalSpacing * 2f;
				var r3 = new Rect(x3, position.y, w3, position.height);

				/** <<============================================================>> **/

				EditorGUI.LabelField(r0, isLoopingLabel);
				isLooping.boolValue = EditorGUI.Toggle(r1, isLooping.boolValue);
				curve.animationCurveValue = EditorGUI.CurveField(r2, curve.animationCurveValue);
				duration.floatValue = EditorGUI.FloatField(r3, duration.floatValue);

				EditorGUI.EndProperty();
			}
		}

		#endregion
		#region Fields

		/// <summary>
		/// The list of curves that this <see cref="Timer"/> oversees.
		///</summary>
		[Tooltip("The list of curves that this Timer oversees.")]
		[SerializeField]
		public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		#endregion
		#region Methods

		public float curveValue =>
			curve.Evaluate(currentTime);

		public float Evaluate(float value) =>
			curve.Evaluate(value);

		#endregion
	}

	#endregion
}
