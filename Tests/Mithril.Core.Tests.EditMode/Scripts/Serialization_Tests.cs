
/** Mirror_Tests.cs
*/

#region Includes

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

using Mithril;

#endregion

namespace Mithril.Tests.Serialization
{
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
		#region Objects

		private enum EToppinType
		{
			Mushroom = 1,
			Cheese = 2,
			Tomato = 4,
			Sausage = 8,
			Pineapple = 16,
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
		public void IsWrapperWithBracesOnly_BasicSystemObject()
		{
			/**	<<==  ARRANGE  ==>>	**/

			var __expected = "{" + $"\"{NewSerialize.TYPE_LABEL}\":\"{typeof(object).AssemblyQualifiedName}\",\"{NewSerialize.DATA_LABEL}\":" + "{}}";
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

	public class Pizza
	{
		public float diameter;
	}

	public class ThinPizza : Pizza
	{

	}

	public class DeepDish : Pizza
	{
		public float greasiness;
	}
}
