
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
	[CreateAssetMenu(menuName = "Mithril/Tests/Test Editable Object")]
	public sealed class TestEditableObject : NodeGraphData
	{
		#region Data

		#region



		#endregion

		#endregion
		#region Methods

		#region

		public override System.Type[] compatibleEditorWindows =>
			new System.Type[] {
				typeof(TestNodeGraphWindow)
			};

		#endregion

		#endregion
	}

	public sealed class TestNodeGraphWindow : NodeGraphWindow
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
		public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			var __result = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("Available Nodes"), 0),

				new SearchTreeEntry(new GUIContent("Custom Node")) { level = 1, userData = typeof(Node) },
				new SearchTreeEntry(new GUIContent("Test Node")) { level = 1, userData = typeof(TestNode) },
			};

			return __result;
		}
	}

	public sealed class TestNode : Node
	{
		#region Data

		#region

		public override string defaultName =>
			"Test Node";

		private TextField _textField;

		[SerializeField]
		private string _message;

		public string message
		{
			get => _textField.text;
			set
			{
				_message = value;
				_textField.SetValueWithoutNotify(value);
			}
		}

		#endregion

		#endregion
		#region Methods

		#region

		public TestNode() : base()
		{
			_message = string.Empty;

			_textField = new TextField(string.Empty);
			_textField.multiline = true;
			_textField.SetValueWithoutNotify(string.Empty);
			_textField.RegisterCallback<ChangeEvent<string>>(OnTextFieldValueChanged);
			_textField.RegisterCallback<BlurEvent>(OnBlurEvent);

			contentContainer.Add(_textField);
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();

			if (message != _message)
				RefreshMessage();
		}

		private void OnTextFieldValueChanged(ChangeEvent<string> context)
		{
			RefreshSize();
		}

		private void OnBlurEvent(BlurEvent context)
		{
			if (message == _message)
				return;

			_message = message;

			NotifyIsModified();
		}

		private void RefreshMessage() =>
			message = _message;

		public override string ToString() =>
			$"{base.ToString()}: \"{_message}\"";

		#endregion

		#endregion
	}
}
