
/** Serialization.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class Serializer : object
	{
		#region Data

		#region

		private readonly static string TYPE_LABEL = "TYPE";
		private readonly static string DATA_LABEL = "DATA";

		private readonly static char[] ALL_ESCAPE_CHARS = new char[] { '\"', '\\', /*'\0',*/ /*'\a',*/ '\b', '\f', '\n', '\r', '\t', /*'\v'*/ };

		private bool _prettyPrint = false;
		private int _indentDepth = 0;

		public int indentDepth
		{
			get => _indentDepth;
			private set => _indentDepth = value.Max(0);
		}


		#endregion

		#endregion
		#region Methods

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

		private static bool IsEscapeChar(char c) =>
			ALL_ESCAPE_CHARS.Contains(c);

		#region Serialization

		#region String Helpers

		private static char GetSerializationEscapeChar(char c)
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

		/// <summary>
		/// Converts the given <paramref name="obj"/> into a serialized JSON string.
		///</summary>

		public static string Serialize<T>(T obj, bool prettyPrint = false)
		{
			var _ = new Serializer();
			_._prettyPrint = prettyPrint;
			return _.Serialize(typeof(T), obj);
		}

		private string Serialize(Type type, object obj)
		{
			var __result = string.Empty;
			__result += OPEN_BRACE;

			__result += $"\"{TYPE_LABEL}\":{SPACE}{SerializedValue(type)}" + ITERATE;
			__result += $"\"{DATA_LABEL}\":{SPACE}{SerializeAny(type, obj)}";

			__result += CLOSE_BRACE;
			return __result;
		}

		private string SerializeAny(Type type, object obj)
		{
			if (obj.GetType().IsPrimitive)
				return obj.ToString();

			if (obj.GetType() == typeof(string))
				return SerializeValue((string)obj);

			if (obj.GetType().IsArray)
			{
				object[] __arr = new object[((Array)obj).Length];
				Array.Copy((Array)obj, __arr, __arr.Length);
				return SerializeArray(type, __arr);
			}

			return SerializeFields(type, obj);
		}

		private string SerializeFields(Type type, object obj)
		{
			var __result = string.Empty;
			__result += OPEN_BRACE;

			var __fields = GetSerializableFields(type);

			for (var i = 0; i < __fields.Length; i++)
			{
				var iField = __fields[i];
				__result += $"\"{iField.Name}\":{SPACE}{SerializeValue(iField, obj)}";

				if (i != __fields.Length - 1)
					__result += ITERATE;
			}

			__result += CLOSE_BRACE;
			return __result;
		}

		private string SerializeArray(Type type, Array arr)
		{
			var __result = string.Empty;
			__result += OPEN_BRACKET;

			for (var i = 0; i < arr.Length; i++)
			{
				var iObject = arr.GetValue(i);
				__result += Serialize(iObject.GetType(), iObject);

				if (i != arr.Length - 1)
					__result += ITERATE;
			}

			__result += CLOSE_BRACKET;
			return __result;
		}

		private static string SerializeValue(string value)
		{
			var __result = string.Empty;

			do
			{
				var __escapeIndex = value.IndexOfAny(ALL_ESCAPE_CHARS);

				if (__escapeIndex > -1)
				{
					__result += $"{value.Substring(0, __escapeIndex)}\\{GetSerializationEscapeChar(value[__escapeIndex])}";
					value = value.Substring(__escapeIndex + 1);

					continue;
				}

				__result += value;
				break;
			} while (true);

			return $"\"{__result}\"";
		}

		private static string SerializedValue(Type value) =>
			SerializeValue(value.ToString());

		private string SerializeValue(FieldInfo field, object parent) =>
			Serialize(field.FieldType, field.GetValue(parent));

		#endregion
		#region Extraction

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

		private static char GetExtractionEscapeChar(char c)
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

		/// <summary>
		/// Constructs a new object from the given serialized JSON <paramref name="data"/>.
		///</summary>

		public static T Extract<T>(string data) =>
			(T)Extract(data);

		public static object Extract(string data)
		{
			try
			{
				var __fieldPairs = GetObjectFields(data);

				var __type = Type.GetType(ValueToString(__fieldPairs[0].Item2));
				var __data = __fieldPairs[1].Item2;

				return ExtractValue(__type, __data);
			}
			catch
			{
				throw new FormatException("The JSON string provided for deserialization is not valid.");
			}
		}

		private static object ExtractValue(Type type, string data)
		{
			if (type.IsPrimitive)
				return ValueToPrimitive(type, data);

			if (type == typeof(string))
				return ValueToString(data);

			if (type.IsArray)
				return ValueToArray(type, data);

			return ValueToObject(type, data);
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

		private static object ValueToPrimitive(Type type, string data)
		{
			var __method = type.GetMethod("Parse", new[] { typeof(string) });
			var __params = new object[] { data };

			try
			{ return __method.Invoke(null, __params); }
			catch
			{ throw new NotImplementedException(); }
		}

		private static string ValueToString(string value)
		{
			value = UnwrapSimple(value, '\"');

			var __result = string.Empty;

			do
			{
				var __escapeIndex = value.IndexOf('\\');

				if (__escapeIndex > -1)
				{
					__result += $"{value.Substring(0, __escapeIndex)}{GetExtractionEscapeChar(value[__escapeIndex + 1])}";
					value = value.Substring(__escapeIndex + 2);
					continue;
				}

				__result += value;
				break;
			} while (true);

			return __result;
		}

		private static Array ValueToArray(Type type, string data)
		{
			data = Unwrap(data, '[', ']');
			var __elements = SplitArray(data);
			var __arr = Array.CreateInstance(type.GetElementType(), __elements.Length);

			int i = 0;
			foreach (var iElement in __elements)
			{
				__arr.SetValue(Extract(iElement), i);
				i++;
			}

			return __arr;
		}

		private static object ValueToObject(Type type, string data)
		{
			data = Unwrap(data, '{', '}');

			object __result;
			{
				if (type.IsSubclassOf(typeof(ScriptableObject)))
					__result = ScriptableObject.CreateInstance(type);
				else
					__result = Activator.CreateInstance(type);

				// var __methodInfo_CreateInstance = typeof(Cuberoot.Editor.TestScriptableObject).GetMethod("CreateInstance", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
				// var __params = new object[] { type };

				// Debug.Log(__methodInfo_CreateInstance);

				// try
				// {
				// 	__result = __methodInfo_CreateInstance.Invoke(null, __params);
				// }
				// catch
				// {
				// 	__result = Activator.CreateInstance(type);
				// }
			}

			if (!string.IsNullOrWhiteSpace(data))
			{
				var __fieldStrings = SplitArray(data);

				foreach (var iFieldString in __fieldStrings)
				{
					var __fieldPair = GetFieldData(iFieldString);

					try
					{
						var __field = GetSerializableField(type, __fieldPair.Item1);
						var __value = Extract(__fieldPair.Item2);

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

		#endregion
	}
}
