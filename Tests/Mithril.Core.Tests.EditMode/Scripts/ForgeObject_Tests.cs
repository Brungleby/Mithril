
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

	public class ForgeObject_Tests
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
		public void TestObject_Exists() =>
			Assert.NotNull(testObject);

		[Test]
		public void TestObject_NameMatchesAssetFile()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __expected = "New Test Forge Object";

			/** <<==  ACT      ===============================================>> **/

			/** <<==  ASSERT   ===============================================>> **/
			Assert.AreEqual(__expected, testObject.name, testObject.name);
		}

		[Test]
		public void TestObject_IsNotSaving() =>
			Assert.IsFalse(testObject.isSaving);

		[Test]
		public void TestObject_NewIsAutosaved()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __testObject = ScriptableObject.CreateInstance<TestForgeObject>();

			/** <<==  ACT      ===============================================>> **/
			// __testObject.isAutosaved = true;

			/** <<==  ASSERT   ===============================================>> **/
			Assert.True(__testObject.isAutosaved);
		}

		[Test]
		public void TestObject_HasIsAutosavedField()
		{
			/** <<==  ARRANGE  ===============================================>> **/


			/** <<==  ACT      ===============================================>> **/


			/** <<==  ASSERT   ===============================================>> **/

			Assert.NotNull(testObject.GetType().GetSerializableField("_isAutosaved"));
		}

		[Test]
		public void TestObject_ContainsIsAutosavedField()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __allFields = testObject.GetType().GetSerializableFields();
			var __queryField = testObject.GetType().GetSerializableField("_isAutosaved");

			/** <<==  ACT      ===============================================>> **/


			/** <<==  ASSERT   ===============================================>> **/

			Assert.IsTrue(__allFields.Contains(__queryField), __allFields.ContentsToString());
		}
	}

	#endregion
}
