
/** Mirror_Tests.cs
*/

#region Includes

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

#endregion

namespace Mithril.Tests.Graph
{
	#region Mirror_Tests

	public class Mirror_Tests
	{
		private static readonly string TEST_OBJECT_PATH = "Assets/Mithril/Mithril.Core/Tests/Mithril.Core.Tests.EditMode/Resources/New Test Editable Object.asset";

		private TestEditableObject _testObject;
		private TestNodeGraphWindow _testWindow;
		private TestNodeGraphView _testGraph;
		private Mithril.Editor.Node _testNode;

		[SetUp]
		public void Setup()
		{
			_testObject = UnityEditor.AssetDatabase.LoadAssetAtPath<TestEditableObject>(TEST_OBJECT_PATH);
			_testWindow = (TestNodeGraphWindow)_testObject._currentlyOpenEditor;
			_testGraph = _testWindow.graph;
			_testNode = _testGraph.GetNode<Mithril.Editor.Node>();
			_testNode.OnBeforeSerialize();
		}

		[Test]
		public void __Template_Pass()
		{
			/**	<<==  ARRANGE  ==>>	**/



			/**	<<==  ACT      ==>>	**/

			Debug.Log(Mithril.JsonTranslator.Encode(_testObject));
			Debug.Log(Mithril.JsonTranslator.Encode(_testWindow));
			Debug.Log(Mithril.JsonTranslator.Encode(_testGraph));
			Debug.Log(Mithril.JsonTranslator.Encode(_testNode));

			/**	<<==  ASSERT   ==>>	**/

			Assert.Pass();
		}


	}

	#endregion
}
