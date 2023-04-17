
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

	public static class Serialization
	{
		public static void SerializeToFile<T>(this T obj, SerialSyntax syntax = SerialSyntax.Json)
		where T : UnityEngine.Object
		{
			var __filePath = UnityEditor.AssetDatabase.GetAssetPath(obj);
			SerializeToFile(obj, __filePath, syntax);
		}

		public static void SerializeToFile<T>(this T obj, string filePath, SerialSyntax syntax = SerialSyntax.Json)
		{
			var __fileText = Serialize(obj, syntax);
			File.WriteAllTextAsync(filePath, __fileText);
		}

		public static string Serialize<T>(this T obj, SerialSyntax syntax = SerialSyntax.Json)
		{
			switch (syntax)
			{
				case SerialSyntax.Json:
					return _SerializeJson(obj);
				default:
					throw new NotImplementedException();
			}

		}

		private static string _SerializeJson<T>(this T obj) =>
			JsonUtility.ToJson(obj);

		private static string _SerializeCube<T>(this T obj, string name, ref int depth)
		{
			string __result = string.Empty;

			if (!obj._IsSerializable())
			{
				depth--;
				throw new Exception();
			}

			__result += "{";
			// __result += $"\"type\":\"{obj.GetTypeName()}\",";
			__result += $"\"{name}\":{CURSOR}";
			__result += "}";

			string __subObject = string.Empty;

			__subObject += "{";

			// var fields = obj.GetFields();
			// foreach (var iField in fields)
			// {


			// 	// __subObject +=
			// }

			// __subObject += "}";

			return __result;
		}
		private static string _SerializeCube<T>(this T obj)
		{
			int __depth = 0;

			return obj._SerializeCube("master", ref __depth);
		}

		private static bool _IsSerializable(this object obj)
		{
			return true;
		}

		private readonly static char CURSOR = '|';

		private static string ReplaceCursorIn(string source, string insert)
		{
			var split = source.Split(CURSOR, 2);

			return $"{split[0]}{insert}{split[1]}";
		}

		private static ETypeCategory GetTypeCategory(this object obj)
		{
			if (obj is IEnumerable)
				return ETypeCategory.Array;
			if (obj.GetType() == typeof(string) || obj.GetType() == typeof(char))
				return ETypeCategory.String;
			throw new NotImplementedException();
		}

		private enum ETypeCategory
		{
			Unbound,
			String,
			Array,
			Struct,
		}

		private static string EncompassWithCurlybraces(this string input) =>
			input.EncompassWith("{", "}");

		private static string EncompassWithParentheses(this string input) =>
			input.EncompassWith("(", ")");

		private static string EncompassWithBrackets(this string input) =>
			input.EncompassWith("[", "]");

		private static string EncompassWithQuotes(this string input) =>
			input.EncompassWith("\"", "\"");

		private static string EncompassWith(this string input, string a, string b)
		{
			return $"{a}{input}{b}";
		}
	}

	public class CubeSerializer
	{
		private bool _format;
		private int _tabDepth = 0;

		public CubeSerializer(bool format)
		{
			_format = format;
		}

		public string Serialize(object obj)
		{
			string __result = string.Empty;

			__result += "{" + Line;

			__result += $"\"Type\":\"{GetTypeName(obj)}\"," + Line;

			var __fields = GetFieldInfo(obj);
			foreach (var iField in __fields)
			{
				// __fields
			}

			__result += "}";

			return __result;
		}

		private string Space =>
			_format ? " " : string.Empty;

		private string Line =>
			_format ? "\n" + TabStart : string.Empty;

		private string TabStart
		{
			get
			{
				if (!_format)
					return string.Empty;

				var __result = string.Empty;
				for (var i = 0; i < _tabDepth; i++)
				{
				}

				throw new NotImplementedException();
			}
		}

		private void TabIn() =>
			_tabDepth++;
		private void TabOut() =>
			_tabDepth--;

		private string GetTypeName(object obj)
		{
			return obj.GetType().AssemblyQualifiedName;
		}

		private FieldInfo[] GetFieldInfo(object obj)
		{
			return obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
				.Where(i => i.GetCustomAttributes(typeof(SerializeField), true).Any() || i.IsPublic)
				.ToArray();
		}
	}

	public enum SerialSyntax
	{
		Json
	}




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
			objA.fart = "fArt";
			objA.count = 24;

			TestObjectB objB = ScriptableObject.CreateInstance<TestObjectB>();
			objB.fart = "tBt";
			objB.volume = 99.0f;

			// Add them to the array
			objects = new TestObject[] { objA, objB };
		}

		public string Save()
		{
			return JsonUtility.ToJson(this, true);

		}

		public static void Load(string text)
		{
			var obj = ScriptableObject.CreateInstance<TestScriptableObject>();
			obj = JsonUtility.FromJson<TestScriptableObject>(text);

			Debug.Log(obj.ToString());
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
