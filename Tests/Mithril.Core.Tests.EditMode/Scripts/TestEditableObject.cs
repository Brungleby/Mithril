
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
using System.Collections.Generic;
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
		public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			var __result = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("Available Nodes"), 0),

				new SearchTreeEntry(new GUIContent("Custom Node")) { level = 1, userData = typeof(Node) },
				new SearchTreeEntry(new GUIContent("String Entry Node")) { level = 1, userData = typeof(TestStringEntryNode) },
				new SearchTreeEntry(new GUIContent("String Receiver Node")) { level = 1, userData = typeof(TestStringReceiverNode) },
				new SearchTreeEntry(new GUIContent("Int Node")) { level = 1, userData = typeof(TestIntNode) },
			};

			return __result;
		}
	}

	public sealed class TestIntNode : Node
	{
		[SerializeField]
		private int _intValue;
		public int intValue
		{
			get => _intValue;
			set
			{
				_intValue = value;
				RefreshTextField();
			}
		}

		private TextField _textField;

		public override string defaultTitle =>
			"Int Node";

		protected override Dictionary<string, Type> defaultPortsIn => new Dictionary<string, Type>
		{
			{ "In", typeof(int) },
		};

		protected override Dictionary<string, Type> defaultPortsOut => new Dictionary<string, Type>
		{
			{ "Out", typeof(int) },
		};

		public TestIntNode() : base()
		{
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

			RefreshTextField();
		}

		private void OnTextFieldValueChanged(ChangeEvent<string> context)
		{
			RefreshSize();
		}

		private void OnBlurEvent(BlurEvent context)
		{
			try
			{
				_intValue = int.Parse(_textField.text);
				NotifyIsModified();
			}
			catch
			{
				RefreshTextField();
			}
		}

		private void RefreshTextField() =>
			_textField.SetValueWithoutNotify(_intValue.ToString().Trim());
	}

	public sealed class TestStringReceiverNode : Node
	{
		#region Data

		public override string defaultTitle =>
			"String Receiver Node";

		#endregion
		#region Methods

		protected override Dictionary<string, Type> defaultPortsIn => Utils.CombineCollection<Dictionary<string, Type>, KeyValuePair<string, Type>>(PRESET_PORTS_IN_EXEC, new Dictionary<string, Type> { { "Text", typeof(string) } });

		protected override Dictionary<string, Type> defaultPortsOut => PRESET_PORTS_OUT_EXEC;
		#endregion
	}

	public sealed class TestStringEntryNode : Node
	{
		#region Data

		#region

		public override string defaultTitle =>
			"String Entry Node";

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

		public TestStringEntryNode() : base()
		{
			_message = string.Empty;

			_textField = new TextField(string.Empty);
			_textField.multiline = true;
			_textField.SetValueWithoutNotify(string.Empty);
			_textField.RegisterCallback<ChangeEvent<string>>(OnTextFieldValueChanged);
			_textField.RegisterCallback<BlurEvent>(OnBlurEvent);

			contentContainer.Add(_textField);
		}

		protected override Dictionary<string, Type> defaultPortsOut => new Dictionary<string, Type>
		{ { "Text", typeof(string) } };

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
