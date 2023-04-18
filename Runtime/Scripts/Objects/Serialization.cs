
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

		#region String Helpers

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

		#region

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

			__result += $"\"{TYPE_LABEL}\":{SPACE}{Serialize(type)}" + ITERATE;
			__result += $"\"{DATA_LABEL}\":{SPACE}{SerializeAny(type, obj)}";

			__result += CLOSE_BRACE;
			return __result;
		}

		private string SerializeAny(Type type, object obj)
		{
			if (obj.GetType() == typeof(string))
				return Serialize((string)obj);

			if (obj.GetType().IsArray)
			{
				object[] __arr = new object[((Array)obj).Length];
				Array.Copy((Array)obj, __arr, __arr.Length);
				return SerializeArray(type, __arr);
			}

			// if (type == typeof(ICollection))
			// 	return Serialize((IEnumerable)((List<object>)obj).ToArray());

			if (obj.GetType().IsPrimitive)
				return obj.ToString();

			return SerializeFields(type, obj);
		}

		private string SerializeFields(Type type, object obj)
		{
			var __result = string.Empty;
			__result += OPEN_BRACE;

			var __fields = new List<FieldInfo>();

			__fields.AddAll(
				type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(i =>
					i.GetCustomAttributes(typeof(SerializeField), true).Any()
					|| (i.Attributes & FieldAttributes.Public) != 0)
				.ToArray()
			);

			for (var i = 0; i < __fields.Count; i++)
			{
				var iField = __fields[i];
				__result += $"\"{iField.Name}\":{SPACE}{Serialize(iField, obj)}";

				if (i != __fields.Count - 1)
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

		private static string Serialize(string value) =>
			$"\"{value}\"";

		private static string Serialize(Type value) =>
			Serialize(value.ToString());

		private string Serialize(FieldInfo field, object parent) =>
			Serialize(field.FieldType, field.GetValue(parent));



		/// <summary>
		/// Constructs a new object from the given serialized JSON <paramref name="data"/>.
		///</summary>

		public T Extract<T>(string data)
		{
			throw new NotImplementedException();
		}

		#endregion

		#endregion
	}
}
