
/** EditableObjectTests.cs
*/

#region Includes

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

using UnityEditor;

#endregion

namespace Mithril
{
	#region EditableObjectTests

	public class EditableObjectTests
	{
		private TestForgeObject _testObject;
		private TestForgeObject testObject
		{
			get
			{
				if (_testObject == null)
					_testObject = AssetDatabase.LoadAssetAtPath<TestForgeObject>("Assets/Mithril/Mithril.Core/Tests/Mithril.Core.Tests.EditMode/Resources/New Test Forge Object.asset");

				return _testObject;
			}
		}

		[Test]
		public void __Template_Pass()
		{
			/** <<============================================================>> **/
			#region Arrange

			var __expected = true;
			bool __result;

			#endregion

			/** <<============================================================>> **/
			#region Act

			__result = true;

			#endregion

			/** <<============================================================>> **/
			#region Assert

			Assert.True(__result.Equals(__expected));

			#endregion
		}

		[Test]
		public void TestObject_Exists()
		{
			/** <<============================================================>> **/
			#region Arrange

			TestForgeObject __testObject;

			#endregion

			/** <<============================================================>> **/
			#region Act

			__testObject = testObject;

			#endregion

			/** <<============================================================>> **/
			#region Assert

			Assert.True(__testObject != null);

			#endregion
		}

		[Test]
		public void TestObject_Name()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __expected = "New Test Forge Object";

			/** <<==  ACT      ===============================================>> **/

			/** <<==  ASSERT   ===============================================>> **/
			Assert.AreEqual(__expected, testObject.name, testObject.name);
		}

		[Test]
		public void TestObject_NotUsuallySaving()
		{
			/** <<==  ARRANGE  ===============================================>> **/

			/** <<==  ACT      ===============================================>> **/


			/** <<==  ASSERT   ===============================================>> **/
			Assert.False(testObject.isSaving);

		}
	}

	#endregion
}
