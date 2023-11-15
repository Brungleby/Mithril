/** KeyValuePairPropertyDrawer.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEditor;

#endregion

namespace Mithril.Editor
{
	#region KeyValuePairPropertyDrawer

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>
	[CustomPropertyDrawer(typeof(KeyValuePairField<,>))]
	public sealed class KeyValuePairPropertyDrawer : SingleLinePropertyDrawer
	{
		protected override (string, float)[] propertyWidths => new (string, float)[] { ("key", 0.5f), ("value", 0.5f) };
	}

	#endregion
}
