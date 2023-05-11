
/** Port.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

#endregion

namespace Mithril.Editor
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public class Port : UnityEditor.Experimental.GraphView.Port
	{
		#region Data

		#region



		#endregion

		#endregion
		#region Methods

		#region

		public Port(Orientation orientation, Direction direction, Capacity capacity, Type type) :
			base(orientation, direction, capacity, type)
		{

		}

		#endregion

		#endregion
	}

	public class ExecPort : Port
	{
		public ExecPort(Orientation orientation, Direction direction, Capacity capacity, Type type) :
			base(orientation, direction, capacity, type)
		{

		}
	}
}
