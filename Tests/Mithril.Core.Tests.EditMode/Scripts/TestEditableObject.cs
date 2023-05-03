
/** TestForgeNode.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using Mithril.Editor;
using Node = Mithril.Editor.Node;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

#endregion

namespace Mithril.Tests
{
	[CreateAssetMenu(menuName = "Test Editable Object")]
	public sealed class TestEditableObject : EditableObject
	{
		#region Data

		#region



		#endregion

		#endregion
		#region Methods

		#region

		public override System.Type[] usableEditorWindows =>
			new System.Type[] {
				typeof(TestNodeGraphWindow)
			};

		#endregion

		#endregion
	}

	public sealed class TestNodeGraphWindow : NodeGraphWindow<TestNodeGraphView>
	{
		#region Data

		#region



		#endregion

		#endregion
		#region Methods

		#region

		#endregion

		#endregion
	}

	public sealed class TestNodeGraphView : NodeGraphView
	{
		public TestNodeGraphView() : base()
		{
			CreateNewNode<TestNode>();
		}
	}

	public sealed class TestNode : Node
	{
		#region Data

		#region

		public override string defaultName =>
			"Test Node";

		[SerializeField]
		private string _message = string.Empty;
		public string message
		{
			get => _message;
			set => _message = value;
		}


		private TextField _textField;

		#endregion

		#endregion
		#region Methods

		#region

		private void Construct(string message)
		{
			_message = message;

			_textField = new TextField(string.Empty);
			_textField.SetValueWithoutNotify(message);
			_textField.multiline = true;
			_textField.RegisterCallback<ChangeEvent<string>>(OnTextFieldValueChanged);

			contentContainer.Add(_textField);
		}

		public TestNode(string message) : base()
		{
			Construct(message);
		}

		public TestNode()
		{
			Construct(string.Empty);
		}

		private void OnTextFieldValueChanged(ChangeEvent<string> context)
		{
			_message = context.newValue;

			RefreshSize();
		}

		#endregion

		#endregion
	}
}
