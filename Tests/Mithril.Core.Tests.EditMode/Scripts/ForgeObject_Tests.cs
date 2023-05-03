
/** EditableObjectTests.cs
*/

#region Includes

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

using UnityEditor;

using Mithril;
using Mithril.Editor;

#endregion

namespace Mithril.Tests
{
	#region ForgeObject_Tests

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
	#region ForgeNodeWindow_Tests

	public class ForgeNodeWindow_Tests
	{
		private TestForgeWindow window =>
			EditorWindow.GetWindow<TestForgeWindow>();

		private Node node =>
			window.graph.GetNode<Node>();

		[Test]
		public void Node_Exists()
		{
			/** <<==  ARRANGE  ===============================================>> **/


			/** <<==  ACT      ===============================================>> **/


			/** <<==  ASSERT   ===============================================>> **/

			Assert.IsNotNull(node);
		}

		[Test]
		public void Node_TitleValueIs_NewCustomNode()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __node = node;
			var __expected = __node.defaultName;

			/** <<==  ACT      ===============================================>> **/


			/** <<==  ASSERT   ===============================================>> **/
			Assert.AreEqual(__expected, __node.title);
		}

		[Test]
		public void Node_TitleProperty_Exists()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __node = node;

			/** <<==  ACT      ===============================================>> **/


			/** <<==  ASSERT   ===============================================>> **/


			Assert.IsNotNull(__node.GetType().GetProperty("title"));
		}

		[Test]
		public void NodeData_IsCreatedFromNode()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __node = node;
			var __data = new SmartNodeData(__node);

			/** <<==  ACT      ===============================================>> **/


			/** <<==  ASSERT   ===============================================>> **/
			Assert.IsNotNull(__data);
		}

		[Test]
		public void NodeData_BasicFieldValueSlotsAreCreatedFromNode()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			Node __node = node;
			var __data = new SmartNodeData(__node);

			/** <<==  ACT      ===============================================>> **/


			/** <<==  ASSERT   ===============================================>> **/

			Assert.GreaterOrEqual(__data.fieldValues.Length, 2);
		}

		[Test]
		public void NodeData_GuidFieldValueMatchesNode()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			Node __node = node;
			var __data = new SmartNodeData(__node);

			var __expected = __node.guid;

			/** <<==  ACT      ===============================================>> **/


			/** <<==  ASSERT   ===============================================>> **/
			Assert.AreEqual(__expected, __data.GetField("guid"));
		}

		[Test]
		public void NodeData_TitleFieldExists()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			Node __node = node;
			SmartNodeData __data;

			/** <<==  ACT      ===============================================>> **/
			__node.OnBeforeSerialize();
			__data = new SmartNodeData(__node);

			/** <<==  ASSERT   ===============================================>> **/
			Assert.IsNotNull(__data.GetField("_title"), __data.fieldValues.ContentsToString());
			// Assert.AreEqual("New Custom Node", __data.GetField("_title"));
		}

		[Test]
		public void NodeData_TitleValueIsDefault()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __node = (TestForgeNode)node;
			SmartNodeData __data;

			/** <<==  ACT      ===============================================>> **/
			__node.OnBeforeSerialize();
			__data = new SmartNodeData(__node);

			/** <<==  ASSERT   ===============================================>> **/
			Assert.AreEqual(__node.defaultName, __data.GetField("_title"));
		}

		[Test]
		public void NodeData_TestForgeNode_MessageValueExists()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __node = (TestForgeNode)node;
			SmartNodeData __data;

			/** <<==  ACT      ===============================================>> **/
			__node.OnBeforeSerialize();
			__data = new SmartNodeData(__node);

			/** <<==  ASSERT   ===============================================>> **/
			Assert.IsNotNull(__data.GetField("message"));
		}
	}

	#endregion

}
