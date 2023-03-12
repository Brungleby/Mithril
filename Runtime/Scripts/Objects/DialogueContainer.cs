
/** DialogueContainer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>
	[System.Serializable]

	public sealed class DialogueContainer : ScriptableObject
	{
		public List<DialogueNodeData> Data = new List<DialogueNodeData>();
		public List<NodeLinkData> Linkage = new List<NodeLinkData>();
	}

	[System.Serializable]

	public sealed class DialogueNodeData : object
	{
		#region Data

		#region

		public string Guid;
		public string DialogueText;
		public Vector2 Position;

		#endregion

		#endregion
		#region Methods

		#region



		#endregion

		#endregion
	}

	[System.Serializable]

	public sealed class NodeLinkData : object
	{
		public string NodeGuid;
		public string TargetGuid;
		public string PortName;
	}
}
