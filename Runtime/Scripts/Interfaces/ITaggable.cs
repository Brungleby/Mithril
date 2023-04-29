
/** ITaggable.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections.Generic;

#endregion

namespace Mithril
{
	/// <summary>
	/// Implements methods that allow others to ask if it contains a given filter.
	///</summary>
	public interface ITaggable
	{
		/// <summary>
		/// A set of categorizational tags that apply to this object. In a UI display, one can use these tags to order or filter items.
		///</summary>

		HashSet<string> Tags { get; }

		/// <summary>
		/// Returns true if the given <paramref name="tag"/> exists for this object.
		///</summary>

		sealed bool HasTag(string tag) => Tags.Contains(tag);

		/// <summary>
		/// Returns true if ALL of the given <paramref name="tags"/> exist for this object.
		///</summary>

		sealed bool HasAllTags(string[] tags)
		{
			foreach (string tag in tags)
			{
				if (!HasTag(tag))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Returns true if ANY of the given <paramref name="tags"/> exist for this object.
		///</summary>

		sealed bool HasAnyTags(string[] tags)
		{
			foreach (string tag in tags)
			{
				if (HasTag(tag))
					return true;
			}

			return false;
		}
	}
}
