
/** Enums.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.

*/


#region Includes

using UnityEngine;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// Denotes a state of play for a video or event.
	///</summary>
	/// <seealso cref="TimerEvent"/>
	/// <seealso cref="Typewriter"/>

	public enum EPlaybackState
	{
		/// <summary>
		/// The function is completely stopped and reset, ready to play from the beginning.
		///</summary>
		[Tooltip("The function is completely stopped and reset, ready to play from the beginning.")]

		Stop,

		/// <summary>
		/// The function is stopped partway but is not reset.
		///</summary>
		[Tooltip("The function is stopped partway but is not reset.")]

		Pause,

		/// <summary>
		/// The function is currently executing.
		///</summary>
		[Tooltip("The function is currently executing.")]

		Play,
	}

	public enum ESignedAxis
	{
		[InspectorName("+X Axis")]

		X_Positive,

		[InspectorName("+Y Axis")]

		Y_Positive,

		[InspectorName("+Z Axis")]

		Z_Positive,

		[InspectorName("-X Axis")]

		X_Negative,

		[InspectorName("-Y Axis")]

		Y_Negative,

		[InspectorName("-Z Axis")]

		Z_Negative,
	}
}
