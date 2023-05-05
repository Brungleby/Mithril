
/** Mirror_Tests.cs
*/

#region Includes

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

#endregion

namespace Mithril.Tests.Serialization
{
	#region Objects

	public class Pizza
	{
		public float diameter;

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Pizza that)
			{
				return this.diameter == that.diameter;
			}

			return false;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 17;
				hash = hash * 31 + diameter.GetHashCode();
				return hash;
			}
		}
	}

	public class ThinPizza : Pizza
	{

	}

	public class DeepDish : Pizza
	{
		public float greasiness;
	}

	public enum EToppinType
	{
		Mushroom = 1,
		Cheese = 2,
		Tomato = 4,
		Sausage = 8,
		Pineapple = 16,
	}

	#endregion

	#region EncodeValue

	public class EncodeValue
	{
		#region Data



		#endregion
		#region Tests

		[Test]
		public void _000_Template_Pass()
		{
			/**	<<==  ARRANGE  ==>>	**/


			/**	<<==  ACT      ==>>	**/


			/**	<<==  ASSERT   ==>>	**/

			Assert.Pass();
		}

		[Test]
		public void IsNull_NullObjectValue()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "null";
			object __object = null;

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsEncoded_IntegerValue_8()
		{
			/**	<<==  ARRANGE  ==>>	**/
			var __expected = "8";
			var __object = 8;

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsEncoded_IntegerValue_25()
		{
			/**	<<==  ARRANGE  ==>>	**/
			var __expected = "25";
			var __object = 25;

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void MatchesWithQuotes_StringValue_Pumpkin()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "\"Pumpkin\"";
			var __object = "Pumpkin";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void MatchesWithQuotes_StringValue_PumpkinWithQuotes()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "\"\\\"Pumpkin\\\"\"";
			var __object = "\"Pumpkin\"";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void MatchesWithTicks_CharValue_A()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "'A'";
			var __object = 'A';

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void MatchesWithEscape_CharValue_Quote()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "'\\\"'";
			var __object = '\"';

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsEncodedAsInteger_EnumValue_PineappleTo16()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "16";
			var __object = EToppinType.Pineapple;

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		#endregion
	}


	#endregion
	#region EncodeEnumerable

	public class EncodeEnumerable
	{
		[Test]
		public void IsBracketsOnly_EmptyObjectArray()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "[]";
			var __object = new object[0];

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsBracketsOnly_EmptyIntegerArray()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "[]";
			var __object = new int[0];

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsAsExpected_IntegerArray()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "[0,-2,300]";
			var __object = new int[] { 0, -2, 300 };

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsAsExpected_StringArray()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "[\"Pineapple\",\"Cheese\",\"Sausage\"]";
			var __object = new string[] { "Pineapple", "Cheese", "Sausage" };

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsBracketsOnly_EmptyObjectList()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "[]";
			var __object = new List<object>();

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsAsExpected_IntegerList()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "[0,-2,300]";
			var __object = (new int[] { 0, -2, 300 }).ToList();

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsAsExpected_IntegerHashSet()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "[0,-2,300]";
			var __object = new HashSet<int>();
			__object.Add(0);
			__object.Add(-2);
			__object.Add(300);

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsAsExpected_PizzaArray()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "[" +
				"{" + $"\"{NewSerialize.TYPE_LABEL}\":\"{typeof(Pizza).AssemblyQualifiedName}\",\"{NewSerialize.DATA_LABEL}\":" + "{\"diameter\":14}}," +
				"{" + $"\"{NewSerialize.TYPE_LABEL}\":\"{typeof(Pizza).AssemblyQualifiedName}\",\"{NewSerialize.DATA_LABEL}\":" + "{\"diameter\":10.5}}" +
				"]";
			var __object = new Pizza[]{
				new Pizza() { diameter = 14f },
				new Pizza() { diameter = 10.5f }
			};

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsAsExpected_PizzaArrayWithSubtypes()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "[" +
				"{" + $"\"{NewSerialize.TYPE_LABEL}\":\"{typeof(ThinPizza).AssemblyQualifiedName}\",\"{NewSerialize.DATA_LABEL}\":" + "{\"diameter\":14}}," +
				"{" + $"\"{NewSerialize.TYPE_LABEL}\":\"{typeof(DeepDish).AssemblyQualifiedName}\",\"{NewSerialize.DATA_LABEL}\":" + "{\"diameter\":10.5,\"greasiness\":7}}," +
				"{" + $"\"{NewSerialize.TYPE_LABEL}\":\"{typeof(ThinPizza).AssemblyQualifiedName}\",\"{NewSerialize.DATA_LABEL}\":" + "{\"diameter\":12}}" +
				"]";
			var __object = new Pizza[]{
				new ThinPizza() { diameter = 14f },
				new DeepDish() { diameter = 10.5f, greasiness = 7f },
				new ThinPizza() { diameter = 12f },
			};

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}
	}
	#endregion
	#region EncodeObject

	public class EncodeObject
	{
		[Test]
		public void IsBracesOnly_PureObject()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "{}";
			var __object = new object();

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}

		[Test]
		public void IsAsExpected_Pizza_DiameterIs14()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "{" + $"\"{NewSerialize.TYPE_LABEL}\":\"{typeof(Pizza).AssemblyQualifiedName}\",\"{NewSerialize.DATA_LABEL}\":" + "{\"diameter\":14}}";
			var __object = new Pizza() { diameter = 14f };

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Encode(__object));
		}


	}

	#endregion

	#region DecodeValue

	public class DecodeValue
	{
		[Test]
		public void IsNull_NullJson()
		{
			/**	<<==  ARRANGE  ==>>	**/
			var __json = "null";


			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.IsNull(NewSerialize.Decode<object>(__json));
		}

		[Test]
		public void IsBoolTrue_BoolJson_True()
		{
			/**	<<==  ARRANGE  ==>>	**/
			var __expected = true;
			var __json = "true";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<bool>(__json));
		}

		[Test]
		public void IsInteger8_IntegerJson_8()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = 8;
			var __json = "8";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<int>(__json));
		}

		[Test]
		public void IsIntegerNeg25_IntegerJson_Neg25()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = -25;
			var __json = "-25";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<int>(__json));
		}

		[Test]
		public void IsFloat10p5_FloatJson_10p5()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = 10.5f;
			var __json = "10.5";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<float>(__json));
		}

		[Test]
		public void ThrowsFormatException_IntegerJson_Pumpkin()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __json = "Pumpkin";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Throws<FormatException>(() => { NewSerialize.Decode<int>(__json); });
		}

		[Test]
		public void IsPineapple_EnumJson_16()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = EToppinType.Pineapple;
			var __json = "16";

			/**	<<==  ACT      ==>>	**/

			var __result = NewSerialize.Decode<EToppinType>(__json);


			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(typeof(EToppinType), __result.GetType());
			Assert.AreEqual(__expected, __result);
		}

		[Test]
		public void IsEmptyString_StringJson_QuotesOnly()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = string.Empty;
			var __json = "\"\"";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<string>(__json));
		}

		[Test]
		public void IsAsExpected_StringJson_PumpkinWithQuotes()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "Pumpkin";
			var __json = "\"Pumpkin\"";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<string>(__json));
		}

		[Test]
		public void ThrowsFormatException_StringJson_PumpkinWithNoQuotes()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __json = "Pumpkin";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Throws<FormatException>(() => { NewSerialize.Decode<string>(__json); });
		}

		[Test]
		public void ThrowsFormatException_StringJson_PumpkinWithOnlyOpenQuote()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __json = "\"Pumpkin";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Throws<FormatException>(() => { NewSerialize.Decode<string>(__json); });
		}

		[Test]
		public void ThrowsFormatException_StringJson_PumpkinWithOnlyOneQuote()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __json = "Pumpkin\"";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Throws<FormatException>(() => { NewSerialize.Decode<string>(__json); });
		}

		[Test]
		public void ThrowsFormatException_StringJson_PumpkinWithTripleQuote()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __json = "\"Pump\"kin\"";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Throws<FormatException>(() => { NewSerialize.Decode<string>(__json); });
		}

		[Test]
		public void ThrowsFormatException_StringJson_PumpkinWithEscapePrecedingQuote()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __json = "\"Pump\\kin\"Pie\"";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Throws<FormatException>(() => { NewSerialize.Decode<string>(__json); });
		}

		[Test]
		public void ThrowsNotFormatException_StringJson_PumpkinWithEscapeQuote()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "Pump\"kin";
			var __json = "\"Pump\\\"kin\"";
			string __result;

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.DoesNotThrow(() =>
			{
				__result = NewSerialize.Decode<string>(__json);
				Assert.AreEqual(__expected, __result);
			});
		}

		[Test]
		public void ThrowsNotFormatException_StringJson_PumpkinWithEscapeOnly()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "Pump\\kin";
			var __json = "\"Pump\\\\kin\"";
			string __result;

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.DoesNotThrow(() =>
			{
				__result = NewSerialize.Decode<string>(__json);
				Assert.AreEqual(__expected, __result);
			});
		}

		[Test]
		public void ThrowsFormatException_StringJson_PumpkinWithIncompleteEscape()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __json = "\"Pump\\kin\"";
			string __result;

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Throws<FormatException>(() => { __result = NewSerialize.Decode<string>(__json); });
		}

		[Test]
		public void IsAsExpected_CharJson_A()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = 'A';
			var __json = "\'A\'";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<char>(__json));
		}

		[Test]
		public void IsAsExpected_CharJson_DoubleQuote()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = '\"';
			var __json = "\'\"\'";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<char>(__json));
		}

		[Test]
		public void IsAsExpected_CharJson_Slash()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = '/';
			var __json = "\'/\'";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<char>(__json));
		}

		[Test]
		public void IsAsExpected_CharJson_EscapeSlash()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = '/';
			var __json = "\'\\/\'";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<char>(__json));
		}

	}

	#endregion
	#region DecodeEnumerable

	public class DecodeEnumerable
	{
		[Test]
		public void IsEmptyArray_PureArrayJson_BracketsOnly()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = new object[0];
			var __json = "[]";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<object[]>(__json));
		}

		[Test]
		public void IsAsExpected_IntegerArrayJson_BracketsOnly()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = new int[0];
			var __json = "[]";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<int[]>(__json));
		}

		[Test]
		public void IsAsExpected_IntegerArrayJson_0_n2_300()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = new int[] { 0, -2, 300 };
			var __json = "[0,-2,300]";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<int[]>(__json));
		}

		[Test]
		public void IsEmptyList_EmptyArrayJson()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = new List<object>();
			var __json = "[]";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<List<object>>(__json));
		}

		[Test]
		public void IsAsList_IntegerArrayJson_0_n2_300()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = new List<int>() { 0, -2, 300 };
			var __json = "[0,-2,300]";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<List<int>>(__json));
		}

		[Test]
		public void IsAsHashSet_StringArrayJson_Length5()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = new HashSet<string>() { "Mushroom", "Cheese", "Tomato", "Sausage", "Pineapple" };
			var __json = "[\"Mushroom\",\"Cheese\",\"Tomato\",\"Sausage\",\"Pineapple\"]";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode<HashSet<string>>(__json));
		}

		[Test]
		public void ThrowsNotImplementedException_Hashtable()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __json = "[]";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Throws<NotImplementedException>(() => { NewSerialize.Decode<Hashtable>(__json); });
		}
	}

	#endregion
	#region DecodeObject

	public class DecodeObject
	{
		[Test]
		public void IsPureObject_BracesOnly()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = new object();
			var __json = "{}";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected.GetType(), NewSerialize.Decode(__json).GetType());
		}

		[Test]
		public void Pizza_Equals_PizzaWithMatchingDiameter()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __a = new Pizza() { diameter = 5f };
			var __b = new Pizza() { diameter = 5f };

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__a, __b);
		}

		[Test]
		public void Pizza_NotEquals_PizzaWithDifferingDiameter()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __a = new Pizza() { diameter = 5f };
			var __b = new Pizza() { diameter = 10f };

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreNotEqual(__a, __b);
		}

		[Test]
		public void IsAsExpected_PizzaJson()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = new Pizza() { diameter = 27f };
			var __json = "{" + $"\"{NewSerialize.TYPE_LABEL}\":\"{typeof(Pizza).AssemblyQualifiedName}\",\"{NewSerialize.DATA_LABEL}\":" + "{\"diameter\":27}}";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.AreEqual(__expected, NewSerialize.Decode(__json));
		}

		[Test]
		public void ThrowsWrapperException_PizzaJson_WithExtraWrapperFields()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __json = "{" + $"\"{NewSerialize.TYPE_LABEL}\":\"{typeof(Pizza).AssemblyQualifiedName}\",\"{NewSerialize.DATA_LABEL}\":" + "{\"diameter\":27}," + "\"BACON\":\"GREASE\"" + "}";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Throws<NewSerialize.WrapperDecodeException>(() =>
			{
				NewSerialize.Decode(__json);
			});
		}

		[Test]
		public void ThrowsWrapperException_PizzaJson_WithIncorrectWrapperFields()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __json = "{" + $"\"FART\":\"{typeof(Pizza).AssemblyQualifiedName}\",\"STINKY\":" + "{\"diameter\":27}}";

			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Throws<NewSerialize.WrapperDecodeException>(() =>
			{
				NewSerialize.Decode(__json);
			});
		}

		[Test]
		public void ThrowsInvalidCastException_DecodeThinPizza_AsDeepDish()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __json = "{" + $"\"{NewSerialize.TYPE_LABEL}\":\"{typeof(ThinPizza).AssemblyQualifiedName}\",\"{NewSerialize.DATA_LABEL}\":" + "{\"diameter\":27}}";


			/**	<<==  ACT      ==>>	**/



			/**	<<==  ASSERT   ==>>	**/

			Assert.Throws<InvalidCastException>(() =>
			{
				NewSerialize.Decode<DeepDish>(__json);
			});
		}
	}

	#endregion
}
