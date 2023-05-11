
/** Mirror_Tests.cs
*/

#region Includes

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Mithril.Editor;

using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

#endregion

namespace Mithril.Tests.NodeGraph
{
	#region Mirror_Tests

	public class Lab_NodeGraph
	{
		private static readonly string TEST_OBJECT_PATH = "Assets/Mithril/Mithril.Core/Tests/Mithril.Core.Tests.EditMode/Resources/New Test Editable Object.asset";

		private TestEditableObject _testObject;
		private TestNodeGraphWindow _testWindow;
		private NodeGraphView _testGraph;
		private TestStringEntryNode _stringEntryNode;
		private TestStringReceiverNode _stringReceiverNode;

		[SetUp]
		public void Setup()
		{

			Assert.DoesNotThrow(() =>
			{
				_testObject = UnityEditor.AssetDatabase.LoadAssetAtPath<TestEditableObject>(TEST_OBJECT_PATH);
				_testWindow = _testObject.Open<TestNodeGraphWindow>();
				_testGraph = _testWindow.graph;
				_stringEntryNode = _testGraph.GetNode<TestStringEntryNode>();
				_stringReceiverNode = _testGraph.GetNode<TestStringReceiverNode>();
			});
		}

		[Test]
		public void __Template_Pass()
		{
			/**	<<==  ARRANGE  ==>>	**/

			/**	<<==  ACT      ==>>	**/

			/**	<<==  ASSERT   ==>>	**/

			Assert.Pass();
		}

		[Test]
		public void EntryNode_Has1OutputPort()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = 1;

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, _stringEntryNode.GetPorts_Out().Count);
		}

		[Test]
		public void ReceiverNode_IsConnectedToEntryNode()
		{
			/**	<<==  ARRANGE  ==>>	**/



			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Fail();
		}
	}

	#endregion
}
