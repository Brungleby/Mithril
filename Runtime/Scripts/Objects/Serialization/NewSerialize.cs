
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
using System.Reflection;

#endregion

namespace Mithril
{
	/// <summary>
	/// __TODO_ANNOTATE__
	///</summary>

	public sealed class NewSerialize : object
	{
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

		#region Macro

		public static string Encode(object obj) =>
			(new NewSerialize())._Encode(obj);

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
				return EncodePrimitive((int)obj);

			if (__type.IsPrimitive)
				return EncodePrimitive(obj);

			if (IsEncodedAsArray(__type))
				return EncodeArray(obj);

			return EncodeObject(obj);
		}

		#endregion
		#region String Arithmetic

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

		#region Primitive

		private string EncodePrimitive(object obj)
		{
			return obj.ToString().ToLower();
		}

		#endregion

		#region Char

		private bool IsEscapeChar(char c) =>
			JSON_ESCAPE_CHARS.Contains(c);

		private string EncodeChar(char obj)
		{
			if (IsEscapeChar(obj))
				return $"'\\{GetChar_NonEscapeVersion(obj)}'";
			return $"'{obj}'";
		}

		private char GetChar_NonEscapeVersion(char c)
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

		#endregion
		#region String

		private string EncodeString(string obj)
		{
			if (obj == null)
				return "\"\"";

			var __result = string.Empty;

			do
			{
				var __escapeIndex = obj.IndexOfAny(JSON_ESCAPE_CHARS);

				if (__escapeIndex > -1)
				{
					__result += $"{obj.Substring(0, __escapeIndex)}\\{GetChar_NonEscapeVersion(obj[__escapeIndex])}";
					obj = obj.Substring(__escapeIndex + 1);

					continue;
				}

				__result += obj;
				break;
			} while (true);

			return $"\"{__result}\"";
		}

		#endregion

		#region Array

		private bool IsEncodedAsArray(Type type)
		{
			return type.GetInterfaces().Contains(typeof(IEnumerable));
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

		private string EncodeObject(object obj)
		{
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
