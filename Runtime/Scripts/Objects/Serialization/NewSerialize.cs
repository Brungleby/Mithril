
/** NewSerialize.cs
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
using System.Linq;
using System.Reflection;

#endregion

namespace Mithril
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class NewSerialize : object
	{
		#region Inners

		[System.Serializable]
		public class WrapperDecodeException : System.Exception
		{
			public WrapperDecodeException() { }
			public WrapperDecodeException(string message) : base(message) { }
			public WrapperDecodeException(string message, System.Exception inner) : base(message, inner) { }
			protected WrapperDecodeException(
				System.Runtime.Serialization.SerializationInfo info,
				System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}

		#endregion
		#region Data

		#region Static

#if UNITY_INCLUDE_TESTS
		public readonly static string TYPE_LABEL = "TYPE";
		public readonly static string DATA_LABEL = "DATA";
#else
		private readonly static string TYPE_LABEL = "TYPE";
		private readonly static string DATA_LABEL = "DATA";
#endif
		private readonly static string NULL_ENCODED = "null";

		private readonly static char[] JSON_ESCAPE_CHARS = new char[] { '\"', '\\', /*'\0',*/ /*'\a',*/ '\b', '\f', '\n', '\r', '\t', /*'\v'*/ };

		#endregion
		#region Members

		private int _indentDepth = 0;
		private bool _prettyPrint = false;

		#endregion

		#endregion
		#region Methods

		#region Public

		public static T Decode<T>(in string json) =>
			(T)Decode(typeof(T), json);
		public static object Decode(in string json) =>
			Decode(typeof(object), json);

		public static string Encode(object obj) =>
			(new NewSerialize())._Encode(obj);

		#endregion

		#region Macro

		public static object Decode(Type type, in string json)
		{
			if (RepresentsNull(json))
				return null;

			if (typeof(string) == type)
				return DecodeString(json);

			if (typeof(char) == type)
				return DecodeChar(json);

			if (type.IsEnum)
				return DecodeEnum(type, json);

			if (type.IsPrimitive)
				return DecodePrimitive(type, json);

			if (IsEncodedAsJsonArray(type))
				return DecodeEnumerable(type, json);

			return DecodeObject(type, json);
			// throw new NotImplementedException($"Couldn't parse the following JSON as type {type}:\n\"{json}\"");
		}

		private string _Encode(object obj)
		{
			if (obj == null)
				return NULL_ENCODED;

			var __type = obj.GetType();

			if (typeof(string) == __type)
				return EncodeString((string)obj);

			if (typeof(char) == __type)
				return EncodeChar((char)obj);

			if (__type.IsEnum)
				return EncodeEnum(obj);

			if (__type.IsPrimitive)
				return EncodePrimitive(obj);

			if (IsEncodedAsJsonArray(__type))
				return EncodeArray(obj);

			return EncodeObject(obj);
		}

		#endregion
		#region Decode String Arithmetic

		private static (string, string) Split(in string json, char q)
		{
			bool __isInQuotes = false;
			int __contextDepth = 0;

			for (var i = 0; i < json.Length; i++)
			{
				if (json[i] == '[' || json[i] == '{')
				{
					__contextDepth++;
					continue;
				}

				if (json[i] == ']' || json[i] == '}')
				{
					__contextDepth--;
					continue;
				}

				if (__contextDepth > 0)
					continue;

				if (json[i] == '\"' && (i == 0 || json[i - 1] != '\\'))
				{
					__isInQuotes = !__isInQuotes;
					continue;
				}

				if (!__isInQuotes && json[i] == q)
				{
					return (json.Substring(0, i), json.Substring(i + 1));
				}
			}

			throw new KeyNotFoundException();
		}

		private static string Unwrap(in string json, char start, char end)
		{
			var __start = json.IndexOf(start) + 1;
			var __end = json.LastIndexOf(end);

			try
			{
				return json.Substring(__start, __end - __start).Trim();
			}
			catch (Exception e)
			{
				if (__start == -1)
					throw new IndexOutOfRangeException("Start character not found in the given data string.", e);
				if (__end == -1)
					throw new IndexOutOfRangeException("End character not found in the given data string.", e);

				throw e;
			}
		}
		private static string Unwrap(in string json, char startEnd) =>
			Unwrap(json, startEnd, startEnd);

		private static string[] UnwrapIterative(in string json, char start, char end)
		{
			var __json = Unwrap(json, start, end);

			if (string.IsNullOrWhiteSpace(__json))
				return new string[0];

			var __result = new List<string>();

		loop:
			(string, string) __tuple;
			try
			{
				__tuple = Split(__json, ',');
			}
			catch
			{
				__result.Add(__json);
				return __result.ToArray();
			}

			__result.Add(__tuple.Item1);
			__json = __tuple.Item2;

			goto loop;
		}

		private static string[] UnwrapArray(in string json) =>
			UnwrapIterative(json, '[', ']');

		private static (string, string)[] UnwrapFieldPairs(in string json)
		{
			var __elements = UnwrapIterative(json, '{', '}');
			var __result = new (string, string)[__elements.Length];

			var i = 0;
			foreach (var iString in __elements)
			{
				var iPair = Split(iString, ':');

				__result[i] = (Unwrap(iPair.Item1, '\"'), iPair.Item2);

				i++;
			}

			return __result;
		}

		#endregion
		#region Encode String Arithmetic

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
				for (var i = 0; i < _indentDepth; i++)
					__result += "\t";
				return __result;
			}
		}

		private string INDENT_LINE
		{
			get
			{
				_indentDepth++;
				return NEWLINE;
			}
		}

		private string OUTDENT_LINE
		{
			get
			{
				_indentDepth--;
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

		#region Primitive | Enum | Null

		private static bool RepresentsNull(in string json)
		{
			return json == NULL_ENCODED;
		}

		private static object DecodeEnum(Type type, in string json)
		{
			return Enum.Parse(type, json);
		}

		private static object DecodePrimitive(Type type, in string json)
		{
			var __m_Parse = type.GetMethod("Parse", new Type[] { typeof(string) });

			try
			{
				return __m_Parse.Invoke(null, new object[] { json });
			}
			catch (Exception e)
			{
				throw new FormatException($"Couldn't parse the following JSON string to primitive type ({type}):\n\"{json}\"", e);
			}
		}

		private string EncodeEnum(object obj)
		{
			return EncodePrimitive((int)obj);
		}

		private string EncodePrimitive(object obj)
		{
			return obj.ToString().ToLower();
		}

		#endregion

		#region Char

		private bool IsEscapeChar(char c) =>
			JSON_ESCAPE_CHARS.Contains(c);

		private static char DecodeChar(in string json)
		{
			/** <<============================================================>> **/

			string __value;
			try
			{
				__value = Unwrap(json, '\'');
			}
			catch (Exception e)
			{
				throw new FormatException($"The following JSON data could not be parsed as a char:\n\"{json}\"", e);
			}

			/** <<============================================================>> **/

			if (__value[0] == '\\')
				return GetEscapeChar_DecodeVersion(__value[1]);
			return __value[0];
		}

		private string EncodeChar(char obj)
		{
			if (IsEscapeChar(obj))
				return $"'\\{GetEscapeChar_EncodeVersion(obj)}'";
			return $"'{obj}'";
		}

		private static char GetEscapeChar_DecodeVersion(char c)
		{
			switch (c)
			{
				case '\\':
				case '\"':
				case '\'':
				case '/':
					return c;
			}

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
			}

			throw new FormatException($"'{c}' is not a valid JSON escape char.");
		}

		private char GetEscapeChar_EncodeVersion(char c)
		{
			switch (c)
			{
				case '\\':
				case '\"':
				case '\'':
					// case '/':
					return c;
			}
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
			}

			throw new FormatException($"'{c}' is not a valid JSON escape char.");
		}

		#endregion
		#region String

		private static string DecodeString(in string json)
		{
			/** <<============================================================>> **/

			string __value;
			try
			{
				__value = Unwrap(json, '\"');
			}
			catch (Exception e)
			{
				throw new FormatException($"The following JSON data could not be parsed as a string:\n\"{json}\"", e);
			}

			/** <<============================================================>> **/

			var __result = string.Empty;

		loop:
			var __quoteIndex = __value.IndexOf('\"');
			var __escapeIndex = __value.IndexOf('\\');

			if (__quoteIndex != -1 && (
				__escapeIndex == -1 ||
				__quoteIndex < __escapeIndex
			))
				throw new FormatException($"The following JSON string could not be parsed because it contains an unescaped quote (at index {__quoteIndex}):\n\"{json}\"");

			if (__escapeIndex != -1)
			{
				__result += __value.Substring(0, __escapeIndex)
					+ GetEscapeChar_DecodeVersion(__value[__escapeIndex + 1])
				;
				__value = __value.Substring(__escapeIndex + 2);
				goto loop;
			}

			return __result + __value;
		}

		private string EncodeString(in string json)
		{
			if (json == null)
				return "\"\"";

			var __json = json;
			var __result = string.Empty;

			do
			{
				var __escapeIndex = __json.IndexOfAny(JSON_ESCAPE_CHARS);

				if (__escapeIndex > -1)
				{
					__result += $"{__json.Substring(0, __escapeIndex)}\\{GetEscapeChar_EncodeVersion(__json[__escapeIndex])}";
					__json = __json.Substring(__escapeIndex + 1);

					continue;
				}

				__result += __json;
				break;
			} while (true);

			return $"\"{__result}\"";
		}

		#endregion

		#region Array

		private static bool IsDecodedAsArray(Type type) =>
			typeof(Array).IsAssignableFrom(type);

		private static bool IsDecodedAsCollection_T_(Type type)
		{
			try
			{
				return type.GetGenericTypeDefinition().GetInterface(typeof(ICollection<>).Name) != null;
			}
			catch
			{
				return false;
			}
		}

		private static bool IsEncodedAsJsonArray(Type type) =>
			type.GetInterfaces().Contains(typeof(IEnumerable));

		private static object DecodeEnumerable(Type type, in string json)
		{
			if (IsDecodedAsArray(type))
				return DecodeArray(type.GetElementType(), json);

			if (IsDecodedAsCollection_T_(type))
				return DecodeCollection_T_(type, json);

			throw new NotImplementedException($"The data structure ({type.Name}) is not currently able to be decoded from a json array.");
		}

		private static Array DecodeArray(Type elementType, in string json)
		{
			var __elementStrings = UnwrapArray(json);

			var __result = Array.CreateInstance(elementType, __elementStrings.Length);

			for (var i = 0; i < __result.Length; i++)
				__result.SetValue(Decode(elementType, __elementStrings[i]), i);

			return __result;
		}

		private static object DecodeCollection_T_(Type type, in string json)
		{
			var __result = Activator.CreateInstance(type);

			var __elementType = type.GetGenericArguments()[0];
			var __elements = DecodeArray(__elementType, json);

			var __m_Add = type.GetMethod("Add", new Type[] { __elementType });

			foreach (var iElement in __elements)
				__m_Add.Invoke(__result, new object[] { iElement });

			return __result;
		}

		private string EncodeArray(object obj)
		{
			var __result = OPEN_BRACKET;
			var __array = (IEnumerable)obj;

			var __isFirstIteration = true;
			foreach (var i in __array)
			{
				if (!__isFirstIteration)
					__result += ITERATE;
				else
					__isFirstIteration = false;

				__result += _Encode(i);
			}

			return __result + CLOSE_BRACKET;
		}

		#endregion
		#region Object

		private static object DecodeObject(Type expectedType, in string json)
		{
			/** <<============================================================>> **/

			var __wrapperFields = UnwrapFieldPairs(json);

			if (__wrapperFields.Length == 0)
				return new object();

			if (
				__wrapperFields.Length != 2 ||
				__wrapperFields[0].Item1 != TYPE_LABEL ||
				__wrapperFields[1].Item1 != DATA_LABEL
			)
				throw new WrapperDecodeException($"The following JSON string does not properly represent an object value because its wrapper fields are missing or incorrect:\n\"{json}\"");

			/** <<============================================================>> **/

			var __objectType = DecodeType(__wrapperFields[0].Item2);
			var __objectFieldPairs = UnwrapFieldPairs(__wrapperFields[1].Item2);

			var __result = Activator.CreateInstance(__objectType);

			foreach (var iPair in __objectFieldPairs)
			{
				var iField = __objectType.GetField(iPair.Item1);
				var iValue = Decode(iField.FieldType, iPair.Item2);

				iField.SetValue(__result, iValue);
			}

			return __result;
		}

		private static Type DecodeType(in string json)
		{
			return Type.GetType(Unwrap(json, '\"'));
		}

		private string EncodeObject(object obj)
		{
			if (typeof(object) == obj.GetType())
			{
#if !UNITY_INCLUDE_TESTS
				UnityEngine.Debug.LogWarning("A pure, non-null object was encoded to json.");
#endif
				return "{}";
			}

			var __result = OPEN_BRACE;

			__result += $"\"{TYPE_LABEL}\":{SPACE}{EncodeType(obj.GetType())}";
			__result += ITERATE;
			__result += $"\"{DATA_LABEL}\":{SPACE}{EncodeFields(obj)}";

			return __result + CLOSE_BRACE;
		}

		private string EncodeType(Type type)
		{
			return EncodeString(type.AssemblyQualifiedName);
		}

		private string EncodeFields(object obj)
		{
			var __result = OPEN_BRACE;
			var __type = obj.GetType();

			var __isFirstIteration = true;
			foreach (var iField in __type.GetSerializableFields())
			{
				if (!__isFirstIteration)
					__result += ITERATE;
				else
					__isFirstIteration = false;

				var __fieldName = iField.Name;
				var __fieldValue = iField.GetValue(obj);
				__result += $"\"{__fieldName}\":{SPACE}{_Encode(__fieldValue)}";
			}

			return __result + CLOSE_BRACE;
		}

		#endregion

		#endregion
	}
}
