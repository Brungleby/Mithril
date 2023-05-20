
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

using UnityEngine;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

#endregion

namespace Mithril
{
	#region JsonTranslator

	/// <summary>
	/// Serializes objects into proper JSON format.
	///</summary>

	public sealed class JsonTranslator : object
	{
		#region Inners
		#region Exceptions

		#region TranslationException

		[System.Serializable]
		public class TranslationException : System.Exception
		{
			public TranslationException() { }
			public TranslationException(string message) : base(message) { }
			public TranslationException(string message, System.Exception inner) : base(message, inner) { }
			protected TranslationException(
				System.Runtime.Serialization.SerializationInfo info,
				System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}

		[System.Serializable]
		public class DecodeException : TranslationException
		{
			public DecodeException(string json) : base($"The following JSON string failed to decode:\n{json}") { }

			// public DecodeException() { }
			// public DecodeException(string message) : base(message) { }
			// public DecodeException(string message, System.Exception inner) : base(message, inner) { }
			// protected DecodeException(
			// 	System.Runtime.Serialization.SerializationInfo info,
			// 	System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}

		[System.Serializable]
		public class DecodeWrapperException : DecodeException
		{
			public DecodeWrapperException(string json) : base($"The following JSON string does not properly represent an object value because its wrapper fields are missing or incorrect:\n{json}") { }
		}

		#endregion

		#endregion

		#endregion
		#region Data

		#region Static

		public readonly static string NULL = "null";
#if UNITY_INCLUDE_TESTS
		public readonly static string TYPE_LABEL = "TYPE";
		public readonly static string DATA_LABEL = "DATA";
#else
		private readonly static string TYPE_LABEL = "TYPE";
		private readonly static string DATA_LABEL = "DATA";
#endif

		private readonly static char[] JSON_ESCAPE_CHARS = new char[] { '\"', '\\', /*'\0',*/ /*'\a',*/ '\b', '\f', '\n', '\r', '\t', /*'\v'*/ };

		private readonly ReadOnlyDictionary<Type, Func<string, object>> DECODE_METHODS_BY_TYPE;
		private readonly ReadOnlyDictionary<Type, Func<object, string>> ENCODE_METHODS_BY_TYPE;

		#endregion
		#region Members

		private int _indentDepth = 0;
		private bool _prettyPrint = false;

		#endregion

		#endregion
		#region Methods

		#region Construction

		private JsonTranslator()
		{
			DECODE_METHODS_BY_TYPE = new ReadOnlyDictionary<Type, Func<string, object>>(new Dictionary<Type, Func<string, object>>
			{
				{ typeof(Vector2), DecodeVector2 },
				{ typeof(Vector3), DecodeVector3 },
				{ typeof(Rect), DecodeRect },
				{ typeof(Guid), DecodeGuid },
				{ typeof(Edge), DecodeGraphViewEdge },
				{ typeof(Mirror), DecodeMirror }
			});
			ENCODE_METHODS_BY_TYPE = new ReadOnlyDictionary<Type, Func<object, string>>(new Dictionary<Type, Func<object, string>>
			{
				{ typeof(Vector2), EncodeVector2 },
				{ typeof(Vector3), EncodeVector3 },
				{ typeof(Rect), EncodeRect },
				{ typeof(Guid), EncodeGuid },
				{ typeof(Edge), EncodeGraphViewEdge },
				{ typeof(Mirror), EncodeMirror }
			});
		}

		#endregion

		#region Overrides

		#endregion

		#region Public

		public static object Decode(in string json) =>
			new JsonTranslator().DecodeAny(typeof(object), json);
		public static object Decode(Type type, in string json) =>
			new JsonTranslator().DecodeAny(type, json);
		public static T Decode<T>(in string json) =>
			(T)new JsonTranslator().DecodeAny(typeof(T), json);

		public static string Encode(object obj) =>
			new JsonTranslator().EncodeAny(obj);

		#endregion
		#region Macro

		public object DecodeAny(Type type, in string json)
		{
			if (RepresentsNull(json))
				return null;

			object __result;

			if (DECODE_METHODS_BY_TYPE.TryGetValue(type, out var __m_decodeCustom))
				__result = __m_decodeCustom.Invoke(json);

			else if (typeof(string) == type)
				__result = DecodeString(json);

			else if (typeof(char) == type)
				__result = DecodeChar(json);

			else if (type.IsEnum)
				__result = DecodeEnum(type, json);

			else if (type.IsPrimitive)
				__result = DecodePrimitive(type, json);

			else if (IsEncodedAsJsonArray(type))
				__result = DecodeEnumerable(type, json);

			else
				__result = DecodeObject(type, json);

			if (__result is ISerializationCallbackReceiver __receiver)
				__receiver.OnAfterDeserialize();

			return __result;
		}
		public T DecodeAny<T>(in string json) =>
			(T)DecodeAny(typeof(T), json);

		public string EncodeAny(object obj)
		{
			if (obj == null)
				return NULL;

			if (obj is ISerializationCallbackReceiver __receiver)
				__receiver.OnBeforeSerialize();

			var __type = obj.GetType();

			if (ENCODE_METHODS_BY_TYPE.TryGetValue(__type, out var __m_encodeCustom))
				return __m_encodeCustom.Invoke(obj);

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

			if (__start == -1)
				throw new IndexOutOfRangeException("Start character not found in the given data string.");
			if (__end == -1)
				throw new IndexOutOfRangeException("End character not found in the given data string.");

			return json.Substring(__start, __end - __start).Trim();
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

		public string SPACE =>
			_prettyPrint ? " " : string.Empty;

		public string BLANKLINE =>
			_prettyPrint ? "\n" : string.Empty;

		public string NEWLINE =>
			_prettyPrint ? BLANKLINE + TAB : string.Empty;

		public string TAB
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

		public string INDENT_LINE
		{
			get
			{
				_indentDepth++;
				return NEWLINE;
			}
		}

		public string OUTDENT_LINE
		{
			get
			{
				_indentDepth--;
				return NEWLINE;
			}
		}

		public string OPEN_BRACE =>
			"{" + INDENT_LINE;

		public string CLOSE_BRACE =>
			OUTDENT_LINE + "}";

		public string OPEN_BRACKET =>
			"[" + INDENT_LINE;

		public string CLOSE_BRACKET =>
			OUTDENT_LINE + "]";

		public string ITERATE =>
			"," + NEWLINE;

		#endregion

		#region Primitive | Enum | Null

		private static bool RepresentsNull(in string json)
		{
			return json == NULL || string.IsNullOrEmpty(json);
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

		private object DecodeEnumerable(Type type, in string json)
		{
			if (IsDecodedAsArray(type))
				return DecodeArray(type.GetElementType(), json);

			if (IsDecodedAsCollection_T_(type))
				return DecodeCollection_T_(type, json);

			throw new NotImplementedException($"The data structure ({type.Name}) is not currently able to be decoded from a json array.");
		}

		private Array DecodeArray(Type elementType, in string json)
		{
			var __elementStrings = UnwrapArray(json);

			var __result = Array.CreateInstance(elementType, __elementStrings.Length);

			for (var i = 0; i < __result.Length; i++)
				__result.SetValue(DecodeAny(elementType, __elementStrings[i]), i);

			return __result;
		}

		private object DecodeCollection_T_(Type type, in string json)
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

				__result += EncodeAny(i);
			}

			return __result + CLOSE_BRACKET;
		}

		#endregion
		#region Object

		private object DecodeObject(Type expectedType, in string json)
		{
			/** <<============================================================>> **/

			var __wrapperFields = UnwrapFieldPairs(json);

			if (__wrapperFields.Length == 0)
				return new object();

			if (__wrapperFields.Length != 2)
				throw new DecodeWrapperException(json);

			/** <<============================================================>> **/

			var __objectType = DecodeType(__wrapperFields[0].Item2);

			if (DECODE_METHODS_BY_TYPE.TryGetValue(__objectType, out var __m_decodeCustom))
				return __m_decodeCustom.Invoke(json);

			/** <<============================================================>> **/

			var __objectData = __wrapperFields[1].Item2;
			var __objectFieldPairs = UnwrapFieldPairs(__objectData);

			var __result = CreateInstance(__objectType);

			foreach (var iPair in __objectFieldPairs)
			{
				var iField = __objectType.GetSerializableField(iPair.Item1);
				var iValue = DecodeAny(iField.FieldType, iPair.Item2);

				iField.SetValue(__result, iValue);
			}

			return __result;
		}

		private static Type DecodeType(in string json)
		{
			return Type.GetType(Unwrap(json, '\"'));
		}

		private object DecodeCustom(in string json, Func<string, object> __m_decodeMethod) =>
			__m_decodeMethod.Invoke(json);

		private object CreateInstance(Type type)
		{
			if (typeof(ScriptableObject).IsAssignableFrom(type))
				return ScriptableObject.CreateInstance(type);
			return Activator.CreateInstance(type);
		}

		private string EncodeObject(object obj)
		{
			return EncodeObject(obj, EncodeFields(obj));
		}

		private string EncodeObject(object obj, in string json)
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
			__result += $"\"{DATA_LABEL}\":{SPACE}{json}";

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
				__result += $"\"{__fieldName}\":{SPACE}{EncodeAny(__fieldValue)}";
			}

			return __result + CLOSE_BRACE;
		}

		private string EncodeCustom(object obj, Func<object, string> __m_encodeMethod) =>
			__m_encodeMethod.Invoke(obj);

		#endregion

		#region Miscellaneous

		#region Guid

		public object DecodeGuid(string json) =>
			new Guid(DecodeString(json));

		public string EncodeGuid(object obj) =>
			EncodeString(((Guid)obj).guid);

		#endregion
		#region Vector, Rect

		private object DecodeVector2(string json)
		{
			var __elements = UnwrapArray(json);

			var x = Decode<float>(__elements[0]);
			var y = Decode<float>(__elements[1]);

			return new Vector2(x, y);
		}

		private string EncodeVector2(object obj)
		{
			var __vector = (Vector2)obj;

			var __result = OPEN_BRACKET;

			__result += Encode(__vector.x) + ITERATE;
			__result += Encode(__vector.y);

			return __result + CLOSE_BRACKET;
		}

		private object DecodeVector3(string json)
		{
			var __elements = UnwrapArray(json);

			var x = Decode<float>(__elements[0]);
			var y = Decode<float>(__elements[1]);
			var z = Decode<float>(__elements[2]);

			return new Vector3(x, y, z);
		}

		private string EncodeVector3(object obj)
		{
			var __vector = (Vector3)obj;

			var __result = OPEN_BRACKET;

			__result += Encode(__vector.x) + ITERATE;
			__result += Encode(__vector.y) + ITERATE;
			__result += Encode(__vector.z);

			return __result + CLOSE_BRACKET;
		}

		private object DecodeRect(string json)
		{
			var __elements = UnwrapArray(json);

			var x = Decode<float>(__elements[0]);
			var y = Decode<float>(__elements[1]);
			var w = Decode<float>(__elements[2]);
			var h = Decode<float>(__elements[3]);

			return new Rect(x, y, w, h);
		}

		private string EncodeRect(object obj)
		{
			var __rect = (Rect)obj;

			var __result = OPEN_BRACKET;

			__result += Encode(__rect.x) + ITERATE;
			__result += Encode(__rect.y) + ITERATE;
			__result += Encode(__rect.width) + ITERATE;
			__result += Encode(__rect.height);

			return __result + CLOSE_BRACKET;
		}

		#endregion
		#region GraphView.Edge

		private Edge DecodeGraphViewEdge(string json)
		{
			var __data = UnwrapFieldPairs(json)[1].Item2;
			var __elements = UnwrapFieldPairs(__data);

			var __guidOut = Decode<Guid>(__elements[0].Item2);
			var __portOut = Decode<string>(__elements[1].Item2);
			var __guidIn = Decode<Guid>(__elements[2].Item2);
			var __portIn = Decode<string>(__elements[3].Item2);

			var __edge = new Edge();
			__edge.userData = new Tuple<Guid, string, Guid, string>(__guidOut, __portOut, __guidIn, __portIn);

			return __edge;
		}

		private string EncodeGraphViewEdge(object obj)
		{
			var __edge = (Edge)obj;
			var __data = OPEN_BRACE;

			__data += $"\"guidOut\":{SPACE}{Encode(((Mithril.Editor.Node)__edge.output.node).guid)}" + ITERATE;
			__data += $"\"portOut\":{SPACE}{Encode(__edge.output.portName)}" + ITERATE;
			__data += $"\"guidIn\":{SPACE}{Encode(((Mithril.Editor.Node)__edge.input.node).guid)}" + ITERATE;
			__data += $"\"portIn\":{SPACE}{Encode(__edge.input.portName)}";

			__data += CLOSE_BRACE;

			return EncodeObject(obj, __data);
		}

		#endregion
		#region Mirror

		private static Mirror DecodeMirror(string json) =>
			Mirror.CreateFromJsonDirect(json);

		private static string EncodeMirror(object obj) =>
			((Mirror)obj).data;

		#endregion

		#endregion

		#endregion
	}
	#endregion
	#region Extensions

	public static class TranslatorExtensions
	{
		public static readonly BindingFlags SERIALIZABLE_FIELD_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		private static void AddAllUnique(this List<FieldInfo> list, IEnumerable<FieldInfo> fields)
		{
			foreach (var iField in fields)
			{
				if (list.Select(i => i.Name).Contains(iField.Name))
					continue;

				list.Add(iField);
			}
		}

		private static void AddUnique(this List<FieldInfo> list, FieldInfo field)
		{
			if (!list.Select(i => i.Name).Contains(field.Name))
				list.Add(field);
		}

		private static bool ShouldGetSuperFields(this Type type)
		{
			return typeof(object) != type;
			// return typeof(object) != type && typeof(EditableObject) != type.BaseType;
		}

		private static IEnumerable<FieldInfo> GetLocalSerializableFields(this Type type)
		{
			return type
				.GetFields(SERIALIZABLE_FIELD_FLAGS)
				.Where(i => IsSerializableField(i))
			;
		}

		public static FieldInfo[] GetSerializableFields(this Type type)
		{
			var __result = new List<FieldInfo>();

			if (type.ShouldGetSuperFields())
				__result.AddAllUnique(type.BaseType.GetSerializableFields());

			__result.AddAllUnique(type.GetLocalSerializableFields());

			return __result.ToArray();
		}

		private static FieldInfo GetLocalSerializableField(this Type type, string name) =>
			type.GetField(name, SERIALIZABLE_FIELD_FLAGS);

		public static FieldInfo GetSerializableField(this Type type, string name)
		{
			var __result = type.GetLocalSerializableField(name);

			if (__result != null && !IsSerializableField(__result))
				__result = null;

			if (__result == null && type.ShouldGetSuperFields())
				__result = type.BaseType.GetSerializableField(name);

			return __result;
		}

		private static bool IsSerializableField(this FieldInfo field)
		{
			return
				field.GetCustomAttribute<NonSerializedAttribute>() == null &&
				field.GetCustomAttribute<NonMirroredAttribute>() == null &&
				(field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
			;
		}
	}

	#endregion
}
