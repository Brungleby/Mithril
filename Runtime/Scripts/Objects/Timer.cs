
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
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

#endregion

namespace Mithril
{
	#region (class) Timer

	/// <summary>
	/// This is an object that can be used as a shorthand for creating animations within an existing script.
	///</summary>

	[Serializable]

	public sealed class Timer : object
	{
		#region Inners

		#region Editor
#if UNITY_EDITOR
		[CustomPropertyDrawer(typeof(Timer))]
		private sealed class PropertyDrawer : UnityEditor.PropertyDrawer
		{
			public static readonly float PROPERTY_SPACING = 3f;

			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				EditorGUI.BeginProperty(position, label, property);

				/** <<============================================================>> **/

				var loopsProperty = property.FindPropertyRelative("isLooping");
				var curveProperty = property.FindPropertyRelative("curve");
				var valueProperty = property.FindPropertyRelative("duration");

				/** <<============================================================>> **/

				var loopsTooltip = new GUIContent(string.Empty, loopsProperty.tooltip);
				var curveTooltip = new GUIContent(string.Empty, curveProperty.tooltip);
				var valueTooltip = new GUIContent(string.Empty, valueProperty.tooltip);

				/** <<============================================================>> **/

				position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

				float loopsWidth = 14f;

				float curveLeft = loopsWidth + PROPERTY_SPACING;

				float valueWidth = 30f;
				float valueLeft = position.width - valueWidth;

				float curveWidth = position.width - loopsWidth - valueWidth - PROPERTY_SPACING * 2f;

				Rect loopsRect = new Rect(position.x, position.y, loopsWidth, position.height);
				Rect curveRect = new Rect(position.x + curveLeft, position.y, curveWidth, position.height);
				Rect valueRect = new Rect(position.x + valueLeft, position.y, valueWidth, position.height);

				/** <<============================================================>> **/

				EditorGUI.PropertyField(loopsRect, loopsProperty, loopsTooltip);
				EditorGUI.PropertyField(curveRect, curveProperty, curveTooltip);
				EditorGUI.PropertyField(valueRect, valueProperty, valueTooltip);

				/** <<============================================================>> **/

				EditorGUI.EndProperty();
			}
		}
#endif
		#endregion

		#endregion
		#region Construction

		// public Timer(int loopCount, float duration)
		// {
		// 	this._isPlaying = false;
		// 	this._whenStarted = 0f;

		// 	this.loopCount = loopCount;
		// 	this.duration = duration;
		// 	this._Curves = new AnimationCurve[0];

		// 	this._OnStart = new UnityEvent();
		// 	this._OnCease = new UnityEvent();
		// 	this._OnUpdate = new UnityEvent<float>();
		// 	this._OnCycle = new UnityEvent<int>();
		// }
		// public Timer()
		// {
		// 	duration = 0f;
		// }

		#endregion
		#region Fields

		#region Loops

		// /// <summary>
		// /// The number of loops to perform before stopping. Set to 0 for infinite loops.
		// ///</summary>
		// [Tooltip("The number of loops to perform before stopping. Set to 0 for infinite loops.")]
		// [SerializeField]

		// // public int loopCount;
		// public int loopCount = 1;

		/// <summary>
		/// Whether or not this timer should loop once started.
		///</summary>

		[Tooltip("Whether or not this timer should loop once started.")]
		[SerializeField]

		public bool isLooping = false;

		#endregion

		#region Duration

		/// <summary>
		/// The overall duration of this <see cref="Timer"/>. <see cref="OnCease"/> is invoked once this amount of time has elapsed.
		///</summary>
		[Tooltip("The overall duration of this Timer. OnCease is invoked once this amount of time has elapsed.")]
		[SerializeField]

		// public float duration;
		public float duration = 1.0f;

		#endregion

		#region Curves

		/// <summary>
		/// The list of curves that this <see cref="Timer"/> oversees.
		///</summary>
		[Tooltip("The list of curves that this Timer oversees.")]
		[SerializeField]

		// public AnimationCurve curve;
		public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		#endregion

		#region OnStart

		/// <summary>
		/// This event is called when this <see cref="Timer"/> is manually Started.
		///</summary>
		[Tooltip("This event is called when this Timer is manually Started.")]
		[SerializeField]

		// private UnityEvent _OnStart;
		private UnityEvent _OnStart = new UnityEvent();
		public UnityEvent OnStart => _OnStart;

		#endregion
		#region OnCease

		/// <summary>
		/// This event is called after this <see cref="Timer"/> has started and has reached its full duration.
		///</summary>
		[Tooltip("This event is called after this Timer has started and has reached its full duration.")]
		[SerializeField]

		// private UnityEvent _OnCease;
		private UnityEvent _OnCease = new UnityEvent();
		public UnityEvent OnCease => _OnCease;

		#endregion
		#region OnUpdate

		/// <summary>
		/// This event is called each time <see cref="Update"/> is called and this <see cref="_isPlaying"/>. The <see cref="currentTime"/> is passed as a parameter.
		///</summary>
		[Tooltip("This event is called each time Update is called and this Timer is playing.. The current time is passed as a parameter.")]
		[SerializeField]

		// private UnityEvent<float> _OnUpdate;
		private UnityEvent<float> _OnUpdate = new UnityEvent<float>();
		public UnityEvent<float> OnUpdate => _OnUpdate;

		#endregion
		#region OnCycle

		/// <summary>
		/// This event is called each time this <see cref="Timer"/> has started and has reached its duration, if it is set to loop.
		///</summary>
		[Tooltip("This event is called each time this Timer has started and has reached its duration, if it is set to loop.")]
		[SerializeField]

		// private UnityEvent<int> _OnCycle;
		private UnityEvent<int> _OnCycle = new UnityEvent<int>();
		public UnityEvent<int> OnCycle => _OnCycle;

		#endregion

		#endregion
		#region Members

		private bool _isPlaying = false;
		public bool isPlaying => _isPlaying;

		private float _whenStarted = 0f;

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

		public float curveValue =>
			curve.Evaluate(currentTime);

		// /// <summary>
		// /// This function returns the corresponding Y value at the given <see cref="currentTime"/> for the provided Curve index.
		// ///</summary>
		// /// <param name="i">
		// /// The index of the Curve which to evaluate.
		// ///</param>

		// public float Evaluate(int i = 0) =>
		// 	_Curves[i].Evaluate(currentTime);

		#endregion
		#endregion
	}
	#endregion
}
