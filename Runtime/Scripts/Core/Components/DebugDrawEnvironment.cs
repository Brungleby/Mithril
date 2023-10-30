
/** DebugDrawEnvironment.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Mithril
{
	#region DebugDrawEnvironment

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	[DefaultExecutionOrder(1000)]
	public sealed class DebugDrawEnvironment : MithrilComponent
	{
		public enum DrawType
		{
			SingleUpdate,
			Duration,
			Persistent
		}

		private static DebugDrawEnvironment inst;

		private readonly List<(Hit, float, float)> durationBasedHits = new();
		private readonly List<Hit> singleFrameHits = new();
		private readonly List<Hit> persistentHits = new();

		protected override void Awake()
		{
			base.Awake();

			inst = this;

			var hit = Hit.Linecast(Vector3.zero, Vector3.one, 1 << 32);
			hit.AddToDrawStack(DrawType.SingleUpdate);
		}

		private static void Create()
		{
			var obj = new GameObject();
			inst = obj.AddComponent<DebugDrawEnvironment>();
		}

		public static void Add(Hit hit, DrawType type, float duration = 1f)
		{
			if (inst == null) Create();

			switch (type)
			{
				case DrawType.SingleUpdate:
					inst.singleFrameHits.Add(hit);
					break;
				case DrawType.Duration:
					inst.durationBasedHits.Add((hit, duration, Time.time));
					break;
				case DrawType.Persistent:
					inst.persistentHits.Add(hit);
					break;
			}
		}

		private void Update()
		{
			for (int i = 0; i < durationBasedHits.Count; i++)
			{
				if (Time.time > durationBasedHits[i].Item2 + durationBasedHits[i].Item3)
				{
					durationBasedHits.RemoveAt(i);
					i--;
				}
			}
		}


		private void OnDrawGizmos()
		{
			foreach (var iHit in durationBasedHits)
				iHit.Item1.Draw();

			foreach (var iHit in singleFrameHits)
				iHit.Draw();
			singleFrameHits.Clear();

			foreach (var iHit in persistentHits)
				iHit.Draw();
		}
	}

	#endregion
}
