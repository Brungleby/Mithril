
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
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	// public static class Serialization
	// {
	// 	public static void SerializeToFile<T>(this T obj, SerialSyntax syntax = SerialSyntax.Json)
	// 	where T : UnityEngine.Object
	// 	{
	// 		var __filePath = UnityEditor.AssetDatabase.GetAssetPath(obj);
	// 		SerializeToFile(obj, __filePath, syntax);
	// 	}

	// 	public static void SerializeToFile<T>(this T obj, string filePath, SerialSyntax syntax = SerialSyntax.Json)
	// 	{
	// 		var __fileText = Serialize(obj, syntax);
	// 		File.WriteAllTextAsync(filePath, __fileText);
	// 	}
	// }




	[Serializable]
	public class TestObject : ScriptableObject
	{
		public string fart;
	}

	[Serializable]
	public class TestObjectA : TestObject
	{
		public int count;
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
		[SerializeReference]
		private TestObject[] objects;

		public TestObject[] Objects => objects;

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
			objects = new TestObject[] { objA, objB };
		}

		public string Save()
		{
			// return Serializer.Serialize(this, true);
			return Serializer.Serialize("32.56f\nPoopy!!!", true);
		}

		public static void Load(string text)
		{
			// var obj = Serializer.Extract<TestScriptableObject>(text);
			var obj = Serializer.Extract(text);

			Debug.Log($"FINAL EXTRACTION RESULT: {obj.ToString()}");
		}
	}

	[CustomEditor(typeof(TestScriptableObject))]
	public class Thingy : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var __target = (TestScriptableObject)target;

			if (GUILayout.Button($"Copy as JSON"))
				EditorGUIUtility.systemCopyBuffer = __target.Save();

			if (GUILayout.Button($"Print JSON"))
				TestScriptableObject.Load(EditorGUIUtility.systemCopyBuffer);

			base.OnInspectorGUI();
		}
	}
}
