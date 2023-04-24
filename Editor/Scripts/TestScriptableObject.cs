
/** Serialization.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;

#endregion

namespace Cuberoot.Editor
{
	[Serializable]
	public class TestObject : ScriptableObject
	{
		public string fart;
	}

	[Serializable]
	public class TestObjectA : TestObject
	{
		public int count;

		public override string ToString()
		{
			return $"TestObjectA: Fart: {fart}, Count: {count}";
		}
	}

	[Serializable]
	public class TestObjectB : TestObject
	{
		public float volume;
	}

	[Serializable]
	[CreateAssetMenu(menuName = "Test Scriptable Object")]
	public class TestScriptableObject : ScriptableObject
	{
		[SerializeField]
		private TestObject[] _objects;

		public TestObject[] objects => _objects;

		private void OnEnable()
		{
			// Create instances of the subclasses
			TestObjectA objA = ScriptableObject.CreateInstance<TestObjectA>();
			objA.fart = "fArt\npoo";
			objA.count = 24;

			TestObjectB objB = ScriptableObject.CreateInstance<TestObjectB>();
			objB.fart = "tBt is \"stinky\".";
			objB.volume = 99.0f;

			// Add them to the array
			_objects = new TestObject[] { objA, objB };
		}

		public void Save()
		{
			Serialization.EncodeToFile(this);
		}

		public void Load()
		{
			var obj = Serialization.DecodeFromAsset(this);

			Debug.Log($"FINAL EXTRACTION RESULT: {obj}");
		}
	}

	[CustomEditor(typeof(TestScriptableObject))]
	public class Thingy : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var __target = (TestScriptableObject)target;

			if (GUILayout.Button($"Save JSON File"))
				__target.Save();

			if (GUILayout.Button($"Load JSON File"))
				__target.Load();

			if (GUILayout.Button($"Load from Clipboard"))
			{
				Debug.Log($"Decoding from clipboard: {EditorGUIUtility.systemCopyBuffer}");
				var __obj = Serialization.Decode(EditorGUIUtility.systemCopyBuffer);
				Debug.Log($"Decoded type: {__obj.GetType()}, Decoded value: {__obj.ToString()}");
			}

			base.OnInspectorGUI();
		}
	}
}
