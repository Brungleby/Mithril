
/** MirrorTestObject.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Mithril.Tests
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[CreateAssetMenu(menuName = "Mithril/Tests/MirrorTestObject")]
	public sealed class MirrorableTestObject : ScriptableObject
	{
		#region Data

		#region

		public string message;

		#endregion

		#endregion
		#region Methods

		#region

		public MirrorableTestObject()
		{

		}

		#endregion

		#endregion
	}

	public class Pizza
	{
		public Toppin feature;
	}

	public abstract class Toppin
	{

	}

	public class Mushroom : Toppin
	{
		public int count;
	}

	public class Cheese : Toppin
	{
		public float mass;
	}
}
