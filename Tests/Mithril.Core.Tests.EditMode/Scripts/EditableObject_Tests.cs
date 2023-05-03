
/** EditableObjectTests.cs
*/

#region Includes

using Mithril;
using Mithril.Editor;

using NUnit.Framework;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.TestTools;

using UnityEditor;

#endregion

namespace Mithril.Tests
{
	#region ForgeObject_Tests

	public class EditableObject_Tests
	{
		private TestEditableObject _testObject;
		private TestEditableObject testObject
		{
			get
			{
				if (_testObject == null)
					_testObject = AssetDatabase.LoadAssetAtPath<TestEditableObject>("Assets/Mithril/Mithril.Core/Tests/Mithril.Core.Tests.EditMode/Resources/New Test Editable Object.asset");

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
			var __expected = "New Test Editable Object";

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
			var __testObject = ScriptableObject.CreateInstance<TestEditableObject>();

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
	#region NodeGraph_Tests

	public class NodeGraph_Tests
	{
		private TestNodeGraphWindow window =>
			EditorWindow.GetWindow<TestNodeGraphWindow>();

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
			var __data = new Mirror(__node);

			/** <<==  ACT      ===============================================>> **/


			/** <<==  ASSERT   ===============================================>> **/
			Assert.IsNotNull(__data);
		}

		[Test]
		public void NodeData_GuidFieldValueMatchesNode()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			Node __node = node;
			var __data = new Mirror(__node);

			var __expected = __node.guid;

			/** <<==  ACT      ===============================================>> **/


			/** <<==  ASSERT   ===============================================>> **/
			Assert.AreEqual(__expected, __data.GetFieldValue("guid"));
		}

		[Test]
		public void NodeData_TitleFieldExists()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			Node __node = node;
			Mirror __data;

			/** <<==  ACT      ===============================================>> **/
			__node.OnBeforeSerialize();
			__data = new Mirror(__node);

			/** <<==  ASSERT   ===============================================>> **/
			Assert.IsNotNull(__data.GetFieldValue("_title"), __data.ToString());
		}

		[Test]
		public void NodeData_TitleValueIsDefault()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __node = (TestNode)node;
			Mirror __data;

			/** <<==  ACT      ===============================================>> **/
			__node.OnBeforeSerialize();
			__data = new Mirror(__node);

			/** <<==  ASSERT   ===============================================>> **/
			Assert.AreEqual(__node.defaultName, __data.GetFieldValue("_title"));
		}

		[Test]
		public void NodeData_TestForgeNode_MessageValueExists()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __node = (TestNode)node;
			Mirror __data;

			/** <<==  ACT      ===============================================>> **/
			__node.OnBeforeSerialize();
			__data = new Mirror(__node);

			/** <<==  ASSERT   ===============================================>> **/
			Assert.IsNotNull(__data.GetFieldValue("_message"));
		}

		[Test]
		public void NodeData_Pass()
		{
			/** <<==  ARRANGE  ===============================================>> **/
			var __node = (TestNode)node;
			Mirror __data;

			/** <<==  ACT      ===============================================>> **/
			__node.OnBeforeSerialize();
			__data = new Mirror(__node);

			/** <<==  ASSERT   ===============================================>> **/

			Debug.Log(Serialization.Encode(__data, true));

			Assert.Pass();
		}
	}

	#endregion

}
