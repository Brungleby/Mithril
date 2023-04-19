
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

		private readonly static string TYPE_LABEL = "type";
		private readonly static string DATA_LABEL = "data";

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

		private static bool IsEscapeChar(char c) =>
			ALL_ESCAPE_CHARS.Contains(c);

		#region Serialization

		#region String Helpers

		private static char GetEscapeChar(char c)
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

		public string Serialize<T>(T obj) =>
			Serialize(typeof(T), obj);

		public static string Serialize<T>(T obj, bool prettyPrint)
		{
			var _ = new Serializer();
			_._prettyPrint = prettyPrint;
			return _.Serialize(obj);
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
					__result += $"{value.Substring(0, __escapeIndex)}\\{GetEscapeChar(value[__escapeIndex])}";
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

			return data.Substring(__start, __end - __start);
		}

		private static string UnwrapSimple(string data, char key) =>
			Unwrap(data, key, key);

		private static (string, string) Split(string data, char sep = ',')
		{
			bool __isInQuotes = false;

			for (var i = 0; i < data.Length; i++)
			{
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

		#endregion

		/// <summary>
		/// Constructs a new object from the given serialized JSON <paramref name="data"/>.
		///</summary>

		public static T Extract<T>(string data) =>
			(T)Extract(data);

		public static object Extract(string data)
		{
			var __fieldPairs = GetObjectFields(data);

			var __typeString = ValueToString(__fieldPairs[0].Item2);
			var __dataString = __fieldPairs[0].Item2;

			var __type = Type.GetType(__typeString);
			var __extractObject = Activator.CreateInstance(__type);

			var __fields = GetSerializableFields(__type);

			foreach (var iField in __fields)
				iField.SetValue(__extractObject, ExtractValue(iField.FieldType, __dataString));

			return __extractObject;
		}

		private static object ExtractValue(Type type, string data)
		{
			if (type.IsPrimitive)
				return ValueToPrimitive(type, data);

			if (type == typeof(string))
				return ValueToString(data);

			if (type == typeof(Array))
				return ValueToArray(type, data);

			return Extract(data);
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
			var __parameters = new object[] { data };

			try
			{ return __method.Invoke(null, __parameters); }
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
					__result += value.Substring(0, __escapeIndex - 1);
					value = value.Substring(__escapeIndex + 1);
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
			var __arr = Array.CreateInstance(type, __elements.Length);

			int i = 0;
			foreach (var iElement in __elements)
			{
				__arr.SetValue(Extract(iElement), i);
				i++;
			}

			return __arr;
		}

		// private object ValueToObject(Type type, string data)
		// {
		// 	var __fieldPairs = GetObjectFields(data);
		// }

		#endregion

		#endregion
	}
}
