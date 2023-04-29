
/** MithrilTemplate.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;

#endregion

namespace Mithril
{
	#region (class) LogicGate

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public class LogicGate
	{
		public LogicGate(Action _action0 = null, Action _action1 = null, bool _flipOnExecute = false, bool _startOpen = true)
		{
			action0 = _action0 ?? (() => { });
			action1 = _action1 ?? (() => { });

			_isOpen = _startOpen;
			FlipOnExecute = _flipOnExecute;
		}

		private bool _isOpen;
		public bool isOpen => _isOpen;

		private bool FlipOnExecute;

		private Action action0;
		private Action action1;

		public void Execute()
		{
			if (_isOpen)
				action0.Invoke();
			else
				action1.Invoke();

			if (FlipOnExecute)
				Toggle();
		}

		public void Execute0(bool condition = true)
		{
			if (!_isOpen) return;

			if (condition)
				action0.Invoke();

			// if (FlipOnExecute)
			Toggle();
		}

		public void Execute1(bool condition = true)
		{
			if (_isOpen) return;

			if (condition)
				action1.Invoke();

			// if (FlipOnExecute)
			Toggle();
		}

		public void Open()
		{
			_isOpen = true;
		}

		public void Shut()
		{
			_isOpen = false;
		}

		public void Toggle()
		{
			_isOpen = !_isOpen;
		}

		public void SetAction0(Action value) =>
			action0 = value;

		public void SetAction1(Action value) =>
			action1 = value;
	}

	#endregion
}
