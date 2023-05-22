
/** Typewriter.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;
using UnityEngine.Events;

using TMPro;

#endregion

namespace Mithril
{
	/// <summary>
	/// This is a component that allows any given <see cref="TMP_Text"/> to be "printed out" over time. Primarily used within dialogue boxes.
	///</summary>

	public class Typewriter : MonoBehaviour
	{
		#region Fields

		#region (event) OnCompletedPrintout

		/// <summary>
		/// This event is called when the typewriter has finished printing out all characters.
		///</summary>

		[Tooltip("This event is called when the typewriter has finished printing out all characters.")]
		[SerializeField]

		private UnityEvent _OnCompletedPrintout;

		/// <inheritdoc cref="_OnCompletedPrintout"/>

		public UnityEvent OnCompletedPrintout => _OnCompletedPrintout;

		#endregion
		#region (field) Text

		/// <summary>
		/// The affected Text component.
		///</summary>

		[Tooltip("The affected Text component.")]
		[SerializeField]

		private TMP_Text _Text;

		/// <inheritdoc cref="_Text"/>

		public virtual TMP_Text Text
		{
			get => _Text;
			set => _Text = value;
		}

		#endregion
		#region (field) AudioSource

		/// <summary>
		/// The affected AudioSource component from which to play audio.
		///</summary>

		[Tooltip("The affected AudioSource component from which to play audio.")]
		[SerializeField]

		public AudioSource AudioSource;

		#endregion
		#region (field) PrintoutSpeed

		/// <summary>
		/// The number of characters to print out per second.
		///</summary>

		[Tooltip("The number of characters to print out per second.")]
		[Min(0f)]
		[SerializeField]

		public float PrintoutSpeed = 20.0f;

		#endregion

		/// <summary>
		/// The number of sounds to play per second (should generally be lower than <see cref="PrintoutSpeed"/>).
		///</summary>

		[Tooltip("The number of sounds to play per second (should generally be lower than PrintoutSpeed).")]
		[Min(0f)]

		public float AudioTriggerSpeed = 5.0f;

		/// <summary>
		/// The AudioClip to play during printout.
		///</summary>

		[Tooltip("The AudioClip to play during printout.")]

		public AudioClip PrintoutAudioClip;

		/// <summary>
		/// The AudioClip to play the moment printout is completed.
		///</summary>

		[Tooltip("The AudioClip to play the moment printout is completed.")]

		public AudioClip CompletedAudioClip;

		#endregion
		#region Members

		/// <summary>
		/// The current play state.
		///</summary>

		private EPlaybackState _playState = EPlaybackState.Stop;

		/// <summary>
		/// The current "position" of the "cursor." This value determines how many characters are to be shown.
		///</summary>
		/// <seealso cref="PrintoutSpeed"/>

		private float _cursorIndex = 0f;

		/// <summary>
		/// The current "position" of the "cursor," used for audio. This value determines when <see cref="PrintoutAudioClip"/> is played.
		///</summary>
		/// <seealso cref="AudioTriggerSpeed"/>

		private float _audioIndex = float.MaxValue;

		#endregion
		#region Properties

		/// <summary>
		/// Returns true if the typewriter is playing, false if stopped or paused. Set this value to play or pause.
		///</summary>
		public bool isPlaying
		{
			get => _playState == EPlaybackState.Play;
			set
			{
				if (value)
					Play();
				else
					Pause();
			}
		}

		/// <summary>
		/// The current number of characters that should be visible.
		///</summary>
		private int _charIndex { get => Mathf.FloorToInt(_cursorIndex); }

		#endregion
		#region Methods

		private void OnValidate()
		{
			if (Text == null)
				Text = GetComponent<TMP_Text>();
		}

		private void Awake()
		{
			OnValidate();
			Clear();
		}

		private void Update()
		{
			if (!isPlaying) return;

			_cursorIndex += PrintoutSpeed * Time.deltaTime;
			Text.maxVisibleCharacters = _charIndex;

			if (Text.maxVisibleCharacters >= Text.text.Length)
			{
				Abort();
				OnCompletedPrintout.Invoke();
			}
			else
			{
				_audioIndex += AudioTriggerSpeed * Time.deltaTime;

				if (_audioIndex >= 1f)
				{
					AudioSource.clip = PrintoutAudioClip;
					_audioIndex %= 1f;
				}
			}
		}

		#region Print

		/// <summary>
		/// Call this function to begin printing out the given <paramref name="message"/> to the <see cref="Text"/> component.
		///</summary>
		/// <remarks>
		/// Calling this function without using parameters will restart the printout without changing the existing text.
		///</remarks>
		/// <param name="message">
		/// The string to be printed.
		///</param>

		public void Print(string message)
		{
			Clear();

			Text.text = message;

			_playState = EPlaybackState.Play;

			_cursorIndex = 0f;
			_audioIndex = float.MaxValue;
		}

		/// <inheritdoc cref="Print"/>

		public void Print()
		{
			Print(Text.text);
		}

		#endregion

		/// <summary>
		/// Call this function to play or resume printout playback.
		///</summary>

		public void Play()
		{
			if (_playState == EPlaybackState.Play) return;
			if (_playState == EPlaybackState.Stop) Print();
			else _playState = EPlaybackState.Play;
		}

		/// <summary>
		/// Call this function to temporarily pause printout playback.
		///</summary>

		public void Pause()
		{
			if (_playState == EPlaybackState.Pause) return;

			_playState = EPlaybackState.Pause;
		}

		/// <summary>
		/// Call this function to toggle between playing and pausing printout playback.
		///</summary>

		public void TogglePlayPause()
		{
			isPlaying = !isPlaying;
		}

		/// <summary>
		/// Call this function to stop printout playback where it is.
		///</summary>

		public void Abort()
		{
			_playState = EPlaybackState.Stop;

			AudioSource.clip = CompletedAudioClip;
			AudioSource.Play();
		}

		/// <summary>
		/// Call this function to stop printout playback and finish it by displaying all characters.
		///</summary>

		public void Finish()
		{
			Abort();
			_cursorIndex = float.MaxValue;
		}

		/// <summary>
		/// Call this function to reset the <see cref="Text"/> component to be blank.
		///</summary>
		/// <param name="hardClear">
		/// If true, this will not only reset the visible characters but also reset the <see cref="Text"/> component to be empty.
		///</param>

		private void Clear(bool hardClear = false)
		{
			if (hardClear)
				Text.text = string.Empty;

			_cursorIndex = 0f;
			Text.maxVisibleCharacters = 0;
		}

		#endregion
	}
}
