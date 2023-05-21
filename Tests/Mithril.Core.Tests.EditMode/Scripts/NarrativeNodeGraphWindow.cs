
/** NarrativeNodeGraphWindow.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

#endregion

namespace Mithril.Tests.EditMode
{
	#region NarrativeNodeGraphWindow

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class NarrativeNodeGraphWindow : Editor.BasicNodeGraphWindow<NarrativeSearchWindow>
	{
		#region Data



		#endregion
		#region Methods

		#region Construction

		public NarrativeNodeGraphWindow()
		{

		}

		#endregion

		#endregion
	}

	#endregion
	#region NarrativeSearchWindow

	public sealed class NarrativeSearchWindow : Editor.NodeGraphSearchSubwindow
	{
		public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			var __result = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("Available Nodes"), 0),
					new SearchTreeEntry(new GUIContent("Custom Node")) { level = 1, userData = typeof(Editor.Node) },
					new SearchTreeEntry(new GUIContent("String Entry Node")) { level = 1, userData = typeof(EditorStringEntryNode) }
			};

			return __result;
		}
	}

	#endregion
	#region StringEntryNode

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class ModelStringEntryNode : NodeData.Node
	{
		#region Data



		#endregion
		#region Methods

		#region Construction

		public ModelStringEntryNode() : base() { }
		public ModelStringEntryNode(Editor.Node editorNode) : base(editorNode) { }

		#endregion
		#region Overrides

		public override System.Type editorType => typeof(EditMode.EditorStringEntryNode);

		#endregion

		#endregion
	}

	#endregion
	#region StringEntryNode

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class EditorStringEntryNode : Editor.Node
	{
		#region Data

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
		#region Methods

		#region Construction

		public EditorStringEntryNode() : base() { }
		public EditorStringEntryNode(NodeData.Node model) : base(model) { }

		protected override void OnConstruct()
		{
			base.OnConstruct();

			_message = string.Empty;

			_textField = new TextField(string.Empty);
			_textField.multiline = true;
			_textField.SetValueWithoutNotify(string.Empty);
			_textField.RegisterCallback<ChangeEvent<string>>(OnTextFieldValueChanged);
			_textField.RegisterCallback<BlurEvent>(OnBlurEvent);

			contentContainer.Add(_textField);
		}

		#endregion
		#region Overrides

		public override Type modelType => typeof(ModelStringEntryNode);

		// protected override Dictionary<string, Type> defaultPortsIn => new Dictionary<string, Type>
		// { { "Temp", typeof(string) } };

		protected override Dictionary<string, Type> defaultPortsOut => new Dictionary<string, Type>
		{ { "Text", typeof(string) } };

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();

			if (message != _message)
				RefreshMessage();
		}


		#endregion


		private void OnTextFieldValueChanged(ChangeEvent<string> context)
		{
			RefreshSize();
		}

		private void OnBlurEvent(BlurEvent context)
		{
			if (message == _message)
				return;

			_message = message;

			// NotifyIsModified();
		}

		private void RefreshMessage() =>
			message = _message;

		public override string ToString() =>
			$"{base.ToString()}: \"{_message}\"";

		#endregion
	}
	#endregion
}
