
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
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

#endregion

namespace Cuberoot
{
	#region Serialization

	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class Serialization : object
	{
		#region Data

		#region

		private readonly static string TYPE_LABEL = "TYPE";
		private readonly static string DATA_LABEL = "DATA";

		private readonly static string JSON_EXT = ".json";
		private readonly static char[] JSON_ESCAPE_CHARS = new char[] { '\"', '\\', /*'\0',*/ /*'\a',*/ '\b', '\f', '\n', '\r', '\t', /*'\v'*/ };

		private readonly bool _prettyPrint = false;
		private int _indentDepth = 0;

		private int indentDepth
		{
			get => _indentDepth;
			set => _indentDepth = value.Max(0);
		}

		#endregion

		#endregion
		#region Methods

		private Serialization(bool prettyPrint)
		{
			_prettyPrint = prettyPrint;
		}

		#region File Handling

		/// <summary>
		/// Serializes an object to JSON and saves it to the given <paramref name="filePath"/>.
		///</summary>

		public static void EncodeToFile(string filePath, object obj, bool prettyPrint = false)
		{
			File.WriteAllTextAsync(filePath, Encode(obj, prettyPrint));
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			var __jsonObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);
			__jsonObj.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Serializes the given <see cref="UnityEngine.Object"/> and saves it to a JSON file of the same name.
		///</summary>

		public static void EncodeToFile<T>(T obj, bool prettyPrint = false)
		where T : UnityEngine.Object
		{
			var __filePath = AssetDatabase.GetAssetPath(obj);
			__filePath = Path.ChangeExtension(__filePath, JSON_EXT);
			EncodeToFile(__filePath, obj, prettyPrint);
		}

		/// <summary>
		/// Decodes the JSON data as an object found at the given <paramref name="filePath"/>.
		///</summary>

		public static T DecodeFromFile<T>(string filePath)
		{
			string __data = File.ReadAllText(filePath);
			return Decode<T>(__data);
		}

		/// <summary>
		/// Decodes the JSON data linked to this <see cref="UnityEngine.Object"/>. Primarily used to refresh or load data for the first time.
		///</summary>

		public static T DecodeFromAsset<T>(T obj)
		where T : UnityEngine.Object
		{
			var __filePath = AssetDatabase.GetAssetPath(obj);
			__filePath = Path.ChangeExtension(__filePath, JSON_EXT);
			return DecodeFromFile<T>(__filePath);
		}

		#endregion

		#region Encoding

		/// <summary>
		/// Converts the given <paramref name="obj"/> into a serialized JSON string.
		///</summary>

		public static string Encode(object obj, bool prettyPrint = false)
		{
			var _ = new Serialization(prettyPrint);
			return _.EncodeAny(obj.GetType(), obj);
		}

		#region Internal

		private string EncodeAny(Type type, object value)
		{
			if (type == typeof(string))
				return EncodeString((string)value);

			if (type.IsPrimitive)
				return EncodePrimitive(value);

			if (type.IsEnum)
				return EncodeEnum(value);

			if (type.IsArray)
				return EncodeArray(type, (Array)value);

			return EncodeTypedObject(type, value);
		}

		private string EncodeTypedObject(Type type, object obj)
		{
			var __result = string.Empty;
			__result += OPEN_BRACE;

			__result += $"\"{TYPE_LABEL}\":{SPACE}{EncodeType(type)}" + ITERATE;
			__result += $"\"{DATA_LABEL}\":{SPACE}{EncodeObjectFields(type, obj)}";

			__result += CLOSE_BRACE;
			return __result;
		}

		private string EncodePrimitive(object value)
		{
			return value.ToString().ToLower();
		}

		private string EncodeEnum(object value)
		{
			return ((int)value).ToString();
		}

		private string EncodeObjectFields(Type type, object value)
		{
			var __result = string.Empty;
			__result += OPEN_BRACE;

			var __fields = GetSerializableFields(type);

			for (var i = 0; i < __fields.Length; i++)
			{
				var iField = __fields[i];
				try
				{

					__result += $"\"{iField.Name}\":{SPACE}{EncodeFieldInfo(iField, value)}";

					if (i != __fields.Length - 1)
						__result += ITERATE;
				}
				catch (Exception e)
				{
					throw new Exception($"Error encoding field \"{iField.Name}\" ({iField.FieldType}): {e.Message}");
				}
			}

			__result += CLOSE_BRACE;
			return __result;
		}

		private string EncodeArray(Type type, Array value)
		{
			object[] array = new object[(value).Length];
			Array.Copy(value, array, array.Length);

			var __result = string.Empty;
			__result += OPEN_BRACKET;

			for (var i = 0; i < array.Length; i++)
			{
				var iObject = array.GetValue(i);
				__result += EncodeAny(iObject.GetType(), iObject);

				if (i != array.Length - 1)
					__result += ITERATE;
			}

			__result += CLOSE_BRACKET;
			return __result;
		}

		private static string EncodeString(string value)
		{
			if (value == null)
				return "\"\"";

			var __result = string.Empty;

			do
			{
				var __escapeIndex = value.IndexOfAny(JSON_ESCAPE_CHARS);

				if (__escapeIndex > -1)
				{
					__result += $"{value.Substring(0, __escapeIndex)}\\{EncodeEscapeChar(value[__escapeIndex])}";
					value = value.Substring(__escapeIndex + 1);

					continue;
				}

				__result += value;
				break;
			} while (true);

			return $"\"{__result}\"";
		}

		private static string EncodeType(Type value) =>
			EncodeString(value.ToString());

		private string EncodeFieldInfo(FieldInfo field, object parent) =>
			EncodeAny(field.FieldType, field.GetValue(parent));

		#endregion
		#region String Helpers

		private static char EncodeEscapeChar(char c)
		{
			switch (c)
			{
				// case '\0':
				// 	return '0';
				// case '\a':
				// 	return 'a';
				case '\b':
					return 'b';
				case '\f':
					return 'f';
				case '\n':
					return 'n';
				case '\r':
					return 'r';
				case '\t':
					return 't';
				// case '\v':
				// 	return 'v';
				default:
					return c;
			}
		}

		private string SPACE =>
			_prettyPrint ? " " : string.Empty;

		private string BLANKLINE =>
			_prettyPrint ? "\n" : string.Empty;

		private string NEWLINE =>
			_prettyPrint ? BLANKLINE + TAB : string.Empty;

		private string TAB
		{
			get
			{
				if (!_prettyPrint) return string.Empty;

				var __result = string.Empty;
				for (var i = 0; i < indentDepth; i++)
					__result += "\t";
				return __result;
			}
		}

		private string INDENT_LINE
		{
			get
			{
				indentDepth++;
				return NEWLINE;
			}
		}

		private string OUTDENT_LINE
		{
			get
			{
				indentDepth--;
				return NEWLINE;
			}
		}

		private string OPEN_BRACE =>
			"{" + INDENT_LINE;

		private string CLOSE_BRACE =>
			OUTDENT_LINE + "}";

		private string OPEN_BRACKET =>
			"[" + INDENT_LINE;

		private string CLOSE_BRACKET =>
			OUTDENT_LINE + "]";

		private string ITERATE =>
			"," + NEWLINE;

		#endregion

		#endregion
		#region Decoding

		/// <summary>
		/// Constructs a new object from the given serialized JSON <paramref name="data"/>.
		///</summary>

		public static object Decode(string data)
		{
			try
			{
				var __fieldPairs = GetObjectFields(data);

				var __type = Type.GetType(DecodeString(__fieldPairs[0].Item2));
				var __data = __fieldPairs[1].Item2;

				return DecodeAny(__type, __data);
			}
			catch
			{
				throw new FormatException("The JSON string provided for decoding is not valid.");
			}
		}

		/// <inheritdoc cref="Decode"/>

		public static T Decode<T>(string data) =>
			(T)Decode(data);

		#region Internal



		private static object DecodeAny(Type type, string data)
		{
			if (type.IsPrimitive)
				return DecodePrimitive(type, data);

			if (type == typeof(string))
				return DecodeString(data);

			if (type.IsArray)
				return DecodeArray(type, data);

			return DecodeObject(type, data);
		}

		private static (string, string)[] GetObjectFields(string data)
		{
			var __fieldStrings = SplitArray(Unwrap(data, '{', '}'));
			var __result = new (string, string)[__fieldStrings.Length];

			int i = 0;
			foreach (var iFieldString in __fieldStrings)
			{
				__result[i] = GetFieldData(iFieldString);
				i++;
			}

			return __result;
		}

		private static (string, string) GetFieldData(string fieldString)
		{
			var __tuple = Split(fieldString, ':');

			return (
				UnwrapSimple(__tuple.Item1, '\"'),
				__tuple.Item2.Trim()
			);
		}

		private static object DecodePrimitive(Type type, string data)
		{
			var __method = type.GetMethod("Parse", new[] { typeof(string) });
			var __params = new object[] { data };

			try
			{ return __method.Invoke(null, __params); }
			catch
			{ throw new NotImplementedException(); }
		}

		private static string DecodeString(string value)
		{
			value = UnwrapSimple(value, '\"');

			var __result = string.Empty;

			do
			{
				var __escapeIndex = value.IndexOf('\\');

				if (__escapeIndex > -1)
				{
					__result += $"{value.Substring(0, __escapeIndex)}{DecodeEscapeChar(value[__escapeIndex + 1])}";
					value = value.Substring(__escapeIndex + 2);
					continue;
				}

				__result += value;
				break;
			} while (true);

			return __result;
		}

		private static Array DecodeArray(Type type, string data)
		{
			data = Unwrap(data, '[', ']');
			var __elements = SplitArray(data);
			var __arr = Array.CreateInstance(type.GetElementType(), __elements.Length);

			int i = 0;
			foreach (var iElement in __elements)
			{
				__arr.SetValue(Decode(iElement), i);
				i++;
			}

			return __arr;
		}

		private static object DecodeObject(Type type, string data)
		{
			data = Unwrap(data, '{', '}');

			object __result;
			if (type.IsSubclassOf(typeof(ScriptableObject)))
				__result = ScriptableObject.CreateInstance(type);
			else
				__result = Activator.CreateInstance(type);

			if (!string.IsNullOrWhiteSpace(data))
			{
				var __fieldStrings = SplitArray(data);

				foreach (var iFieldString in __fieldStrings)
				{
					var __fieldPair = GetFieldData(iFieldString);

					try
					{
						var __field = GetSerializableField(type, __fieldPair.Item1);
						var __value = Decode(__fieldPair.Item2);

						__field.SetValue(__result, __value);
					}
					catch (Exception e)
					{
						Debug.LogWarning($"[{type}] Failed to assign field '{__fieldPair.Item1}' from the given data string:\n{__fieldPair.Item2}");

						throw e;
					}
				}
			}

			return __result;
		}
		#endregion
		#region String Helpers

		private static string Unwrap(string data, char start, char end)
		{
			var __start = data.IndexOf(start) + 1;
			var __end = data.LastIndexOf(end);

			try
			{
				return data.Substring(__start, __end - __start).Trim();
			}
			catch (Exception e)
			{
				if (__start == -1)
					throw new KeyNotFoundException("Start character not found in the given data string.");
				if (__end == -1)
					throw new KeyNotFoundException("End character not found in the given data string.");

				throw e;
			}
		}

		private static string UnwrapSimple(string data, char key) =>
			Unwrap(data, key, key);

		private static (string, string) Split(string data, char sep = ',')
		{
			bool __isInQuotes = false;
			int __braceDepth = 0;

			for (var i = 0; i < data.Length; i++)
			{
				if (data[i] == '[' || data[i] == '{')
				{
					__braceDepth++;
					continue;
				}

				if (data[i] == ']' || data[i] == '}')
				{
					__braceDepth--;
					continue;
				}

				if (__braceDepth > 0)
					continue;

				if (data[i] == '\"' && (i == 0 || data[i - 1] != '\\'))
				{
					__isInQuotes = !__isInQuotes;
					continue;
				}

				if (!__isInQuotes && data[i] == sep)
				{
					return (data.Substring(0, i), data.Substring(i + 1));
				}
			}

			throw new KeyNotFoundException();
		}

		private static string[] SplitArray(string data, char sep = ',')
		{
			var __result = new List<string>();

			do
			{
				(string, string) __tuple;
				try
				{
					__tuple = Split(data, sep);
				}
				catch
				{
					__result.Add(data);
					break;
				}

				__result.Add(__tuple.Item1);
				data = __tuple.Item2;

			} while (true);

			return __result.ToArray();
		}

		private static char DecodeEscapeChar(char c)
		{
			switch (c)
			{
				// case '0':
				// 	return '\0';
				// case 'a':
				// 	return '\a';
				case 'b':
					return '\b';
				case 'f':
					return '\f';
				case 'n':
					return '\n';
				case 'r':
					return '\r';
				case 't':
					return '\t';
				// case 'v':
				// 	return '\v';
				default:
					return c;
			}
		}

		#endregion

		#endregion

		private static FieldInfo[] GetSerializableFields(Type type)
		{
			return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(i =>
					i.GetCustomAttributes(typeof(SerializeField), true).Any()
					|| (i.Attributes & FieldAttributes.Public) != 0)
				.ToArray()
			;
		}

		private static FieldInfo GetSerializableField(Type type, string name)
		{
			var __result = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			Debug.Assert(__result.GetCustomAttributes(typeof(SerializeField), true).Any()
				|| (__result.Attributes & FieldAttributes.Public) != 0);

			return __result;
		}

		#endregion
	}

	#endregion
}
