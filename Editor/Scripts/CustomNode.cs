
/** CustomNode.cs
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
using UnityEngine.Events;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;


#endregion

namespace Cuberoot.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public class CustomNode : Node
	{
		#region Data

		#region Static

		public static readonly Vector2 DEFAULT_NODE_SIZE = new Vector2(150f, 200f);

		#endregion
		#region

		public GUID Guid;
		public bool IsPredefined = false;

		public UnityEvent OnModified;

		#endregion

		#endregion
		#region Methods

		#region

		public void RefreshAll()
		{
			RefreshExpandedState();
			RefreshPorts();
		}

		public virtual void InitializeFor(CustomNodeGraphView view)
		{
			OnModified = new UnityEvent();
			OnModified.AddListener(() =>
			{
				view.OnModified.Invoke();
			});
		}

		#endregion

		#region Port Handling

		public VisualElement GetPortContainerFor(Port port)
		{
			switch (port.direction)
			{
				case Direction.Input:
					return inputContainer;
				case Direction.Output:
					return outputContainer;
				default:
					throw new System.Exception($"This port ({port}) has an invalid direction.");
			}
		}

		public Port CreatePort(string portName, Direction direction, Orientation orientation, Port.Capacity capacity, System.Type type = null)
		{
			/**	Create and initialize the port.
			*/
			var __port = InstantiatePort(orientation, direction, capacity, type);

			__port.portName = portName;

			AttachPort(__port);

			return __port;
		}
		public Port CreatePort<T>(string portName, Direction direction, Orientation orientation, Port.Capacity capacity) =>
			CreatePort(portName, direction, orientation, capacity, typeof(T));


		public void AttachPort(Port port)
		{
			/**	Add the port to the appropriate container oon this node.
			*/
			var __portContainer = GetPortContainerFor(port);

			__portContainer.Add(port);

			/**	Refresh this node.
			*/
			RefreshAll();
		}

		public void RemovePort(Port port)
		{
			var __portContainer = GetPortContainerFor(port);

			__portContainer.Remove(port);

			RefreshAll();
		}

		#endregion

		#endregion
	}
}
