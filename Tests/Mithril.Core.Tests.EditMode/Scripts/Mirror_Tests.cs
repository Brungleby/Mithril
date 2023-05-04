
/** Mirror_Tests.cs
*/

#region Includes

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

using Mithril;

#endregion

namespace Mithril.Tests
{
	#region Mirror_Tests

	public class Mirror_Tests
	{
		#region Data

		private static readonly string TEST_OBJECT_PATH = "Assets/Mithril/Mithril.Core/Tests/Mithril.Core.Tests.EditMode/Resources/New Mirrorable Test Object.asset";

		private MirrorableTestObject _testObject;

		#endregion
		#region Setup

		[SetUp]
		public void SetUp()
		{
			_testObject = UnityEditor.AssetDatabase
				.LoadAssetAtPath<MirrorableTestObject>(TEST_OBJECT_PATH)
			;
		}

		#endregion
		#region Tests

		[Test]
		public void _000_Template_Pass()
		{
			/**	<<==  ARRANGE  ==>>	**/


			/**	<<==  ACT      ==>>	**/


			/**	<<==  ASSERT   ==>>	**/

			Assert.Pass();
		}

		[Test]
		public void IsEncoded_IntegerValue_8()
		{
			/**	<<==  ARRANGE  ==>>	**/
			var __expected = "8";
			var __integer = 8;

			/**	<<==  ACT      ==>>	**/

			var __mirror = new NewMirror(__integer);

			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, __mirror.jsonValue);
		}

		[Test]
		public void IsEncoded_IntegerValue_25()
		{
			/**	<<==  ARRANGE  ==>>	**/
			var __expected = "25";
			var __integer = 25;

			/**	<<==  ACT      ==>>	**/

			var __mirror = new NewMirror(__integer);

			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, __mirror.jsonValue);
		}

		[Test]
		public void MatchesWithQuotes_StringValue_Pumpkin()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "\"Pumpkin\"";
			var __string = "Pumpkin";

			/**	<<==  ACT      ==>>	**/

			var __mirror = new NewMirror(__string);

			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, __mirror.jsonValue);
		}

		#endregion
	}


	#endregion
}
