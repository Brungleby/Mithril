
/** Math.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections.Generic;

using UnityEngine;
using Unity.Mathematics;

#endregion

namespace Cuberoot
{
	public static class Math
	{
		#region Random

		public static int Random(int min = 0, int max = 1, int? seed = null) => seed.HasValue ? new System.Random(seed.GetValueOrDefault()).Next(min, max) : new System.Random().Next(min, max);

		public static float Random(float min = 0f, float max = 1f, int? seed = null) => Mathf.Lerp(min, max, (float)(seed.HasValue ? new System.Random(seed.GetValueOrDefault()).NextDouble() : new System.Random().NextDouble()));

		public static int RandomIndex(this object[] array, int? seed = null) => Random(0, array.Length - 1, seed);
		public static int RandomIndex(this System.Collections.IList list, int? seed = null) => Random(0, list.Count - 1, seed);

		public static T Random<T>(this T[] array, int? seed = null) => array[array.RandomIndex(seed)];
		public static T Random<T>(this System.Collections.Generic.IList<T> list, int? seed = null) => list[Random(0, list.Count - 1, seed)];

		#endregion

		#region Float, Int, Vector

		#region Bool ->> Float, Int

		/// <returns>
		/// 1.0f if <paramref name="value"/> is TRUE; 0.0f if FALSE.
		///</returns>

		public static float ToFloat(this bool value) =>
			value ? 1f : 0f;

		/// <returns>
		/// +1.0f if <paramref name="value"/> is TRUE; -1.0f if FALSE.
		///</returns>

		public static float ToFloatUnit(this bool value) =>
			value ? +1f : -1f;

		/// <returns>
		/// 1 if <paramref name="value"/> is TRUE; 0 if FALSE.
		///</returns>

		public static float ToInt(this bool value) =>
			value ? 1 : 0;

		/// <returns>
		/// +1 if <paramref name="value"/> is TRUE; -1 if FALSE.
		///</returns>

		public static int ToIntUnit(this bool value) =>
			value ? +1 : -1;

		#endregion
		#region Max, Min

		public static float Max(this float a, float b = 0f) =>
			System.Math.Max(a, b);

		public static int Max(this int a, int b = 0) =>
			System.Math.Max(a, b);

		public static float Max(this ICollection<float> collection)
		{
			if (collection.Count == 0) throw new System.Exception($"{collection} does not contain any values to compare.");

			float? result = null;

			foreach (var i in collection)
				result = System.Math.Max(result ?? i, i);

			return (float)result;
		}

		public static int Max(this ICollection<int> collection)
		{
			if (collection.Count == 0) throw new System.Exception($"{collection} does not contain any values to compare.");

			int? result = null;

			foreach (var i in collection)
				result = System.Math.Max(result ?? i, i);

			return (int)result;
		}

		public static float Min(this float a, float b = 0f) =>
			System.Math.Min(a, b);

		public static int Min(this int a, int b = 0) =>
			System.Math.Min(a, b);

		public static float Min(this ICollection<float> collection)
		{
			if (collection.Count == 0) return 0f;

			float? result = null;

			foreach (var i in collection)
				result = System.Math.Min(result ?? i, i);

			return result.Value;
		}

		public static int Min(this ICollection<int> collection)
		{
			if (collection.Count == 0) return 0;

			int? result = null;

			foreach (var i in collection)
				result = System.Math.Min(result ?? i, i);

			return result.Value;
		}
		#endregion
		#region Clamp

		/// <returns>
		/// The given <paramref name="value"/> clamped between <paramref name="min"/> and <paramref name="max"/>.
		///</returns>

		public static float Clamp(this in float value, in float min, in float max) =>
			(value > max) ? max : ((value < min) ? min : value);

		/// <returns>
		/// The given <paramref name="value"/> clamped between 0 and <paramref name="max"/>.
		///</returns>

		public static float Clamp(this in float value, in float max) =>
			(value > max) ? max : ((value < 0f) ? 0f : value);

		/// <returns>
		/// The given <paramref name="value"/> clamped between 0 and 1.
		///</returns>

		public static float Clamp(this in float value) =>
			(value > 1f) ? 1f : ((value < 0f) ? 0f : value);

		/// <returns>
		/// The input value, clamped between negative <paramref name="minMax"/> and positive <paramref name="minMax"/>.
		///</returns>

		public static float ClampAbs(this in float value, in float minMax = 1f) =>
			Mathf.Clamp(value, -(minMax.Abs()), minMax.Abs());

		/// <inheritdoc cref="Clamp(in float, in float, in float)"/>

		public static int Clamp(this in int value, in int min, in int max) =>
			(value > max) ? max : ((value < min) ? min : value);

		/// <inheritdoc cref="Clamp(in float, in float)"/>

		public static int Clamp(this in int value, in int max) =>
			(value > max) ? max : ((value < 0) ? 0 : value);

		/// <inheritdoc cref="Clamp(in float)"/>

		public static int Clamp(this in int value) =>
			(value > 1) ? 1 : ((value < 0) ? 0 : value);

		/// <inheritdoc cref="ClampAbs(in float, in float)"/>

		public static int ClampAbs(this in int value, in int minMax = 1) =>
			Mathf.Clamp(value, -(minMax.Abs()), minMax.Abs());

		/// <inheritdoc cref="Clamp(in Vector3, in Vector3, in Vector3)"/>

		public static Vector2 Clamp(this in Vector2 vector, in Vector2 min, in Vector2 max) =>
			new Vector2(
				vector.x.Clamp(min.x, max.x),
				vector.y.Clamp(min.y, max.y)
			);
		public static Vector2 Clamp(this in Vector2 vector, in Vector2 min, in float max) =>
			new Vector2(
				vector.x.Clamp(min.x, max),
				vector.y.Clamp(min.y, max)
			);
		public static Vector2 Clamp(this in Vector2 vector, in float min, in Vector2 max) =>
			new Vector2(
				vector.x.Clamp(min, max.x),
				vector.y.Clamp(min, max.y)
			);
		public static Vector2 Clamp(this in Vector2 vector, in float min, in float max) =>
			new Vector2(
				vector.x.Clamp(min, max),
				vector.y.Clamp(min, max)
			);

		/// <returns>
		/// The given <paramref name="vector"/> clamped between 0 and <paramref name="max"/>, per axis.
		///</returns>

		public static Vector2 Clamp(this in Vector2 vector, in Vector2 max) =>
			new Vector2(
				vector.x.Clamp(0f, max.x),
				vector.y.Clamp(0f, max.y)
			);
		public static Vector2 Clamp(this in Vector2 vector, in float max) =>
			new Vector2(
				vector.x.Clamp(0f, max),
				vector.y.Clamp(0f, max)
			);

		/// <returns>
		/// The given <paramref name="vector"/> clamped between 0 and 1, per axis.
		///</returns>

		public static Vector2 Clamp(this in Vector2 vector) =>
			new Vector2(
				vector.x.Clamp(0f, 1f),
				vector.y.Clamp(0f, 1f)
			);

		/// <returns>
		/// The input value clamped between negative <paramref name="minMax"/> and positive <paramref name="minMax"/>, per axis.
		///</returns>

		public static Vector2 ClampAbs(this in Vector2 vector, in Vector2 minMax) =>
			vector.Clamp(-minMax, minMax);
		public static Vector2 ClampAbs(this in Vector2 vector, in float minMax) =>
			vector.Clamp(-minMax, minMax);

		/// <returns>
		/// The input value clamped between -1 and +1, per axis.
		///</returns>

		public static Vector2 ClampAbs(this in Vector2 vector) =>
			vector.Clamp(Vector2.one);

		/// <returns>
		/// The given <paramref name="vector"/> clamped between <paramref name="min"/> and <paramref name="max"/>, per axis.
		///</returns>

		public static Vector3 Clamp(this in Vector3 vector, in Vector3 min, in Vector3 max) =>
			new Vector3(
				vector.x.Clamp(min.x, max.x),
				vector.y.Clamp(min.y, max.y),
				vector.z.Clamp(min.z, max.z)
			);
		public static Vector3 Clamp(this in Vector3 vector, in Vector3 min, in float max) =>
			new Vector3(
				vector.x.Clamp(min.x, max),
				vector.y.Clamp(min.y, max),
				vector.z.Clamp(min.z, max)
			);
		public static Vector3 Clamp(this in Vector3 vector, in float min, in Vector3 max) =>
			new Vector3(
				vector.x.Clamp(min, max.x),
				vector.y.Clamp(min, max.y),
				vector.z.Clamp(min, max.z)
			);
		public static Vector3 Clamp(this in Vector3 vector, in float min, in float max) =>
			new Vector3(
				vector.x.Clamp(min, max),
				vector.y.Clamp(min, max),
				vector.z.Clamp(min, max)
			);

		/// <returns>
		/// The given <paramref name="vector"/> clamped between 0 and <paramref name="max"/>, per axis.
		///</returns>

		public static Vector3 Clamp(this in Vector3 vector, in Vector3 max) =>
			new Vector3(
				vector.x.Clamp(0f, max.x),
				vector.y.Clamp(0f, max.y),
				vector.z.Clamp(0f, max.z)
			);
		public static Vector3 Clamp(this in Vector3 vector, in float max) =>
			new Vector3(
				vector.x.Clamp(0f, max),
				vector.y.Clamp(0f, max),
				vector.z.Clamp(0f, max)
			);

		/// <returns>
		/// The given <paramref name="vector"/> clamped between 0 and 1, per axis.
		///</returns>

		public static Vector3 Clamp(this in Vector3 vector) =>
			new Vector3(
				vector.x.Clamp(0f, 1f),
				vector.y.Clamp(0f, 1f),
				vector.z.Clamp(0f, 1f)
			);

		/// <returns>
		/// The input value clamped between negative <paramref name="minMax"/> and positive <paramref name="minMax"/>, per axis.
		///</returns>

		public static Vector3 ClampAbs(this in Vector3 vector, in Vector3 minMax) =>
			vector.Clamp(-minMax, minMax);
		public static Vector3 ClampAbs(this in Vector3 vector, in float minMax) =>
			vector.Clamp(-minMax, minMax);

		/// <returns>
		/// The input value clamped between -1 and +1, per axis.
		///</returns>

		public static Vector3 ClampAbs(this in Vector3 vector) =>
			vector.Clamp(Vector3.one);

		#endregion
		#region Abs

		public static float Abs(this in float f) =>
			System.Math.Abs(f);

		public static int Abs(this in int i) =>
			System.Math.Abs(i);

		/// <returns>
		/// A Vector2 containing the absolute value of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector2 Abs(this in Vector2 v) =>
			new Vector2(v.x.Abs(), v.y.Abs());

		/// <inheritdoc cref="Abs(in Vector2)"/>

		public static Vector2Int Abs(this in Vector2Int v) =>
			new Vector2Int(v.x.Abs(), v.y.Abs());

		/// <returns>
		/// A Vector3 containing the absolute value of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector3 Abs(this in Vector3 v) =>
			new Vector3(v.x.Abs(), v.y.Abs(), v.z.Abs());

		/// <inheritdoc cref="Abs(in Vector3)"/>

		public static Vector3Int Abs(this in Vector3Int v) =>
			new Vector3Int(v.x.Abs(), v.y.Abs(), v.z.Abs());

		#endregion
		#region Sign

		public static float Sign(this in float value) =>
			System.Math.Sign(value);

		public static int Sign(this in int value) =>
			System.Math.Sign(value);

		/// <returns>
		/// A Vector2 containing the sign of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector2 Sign(this in Vector2 v) =>
			new Vector2(v.x.Sign(), v.y.Sign());

		/// <returns>
		/// A Vector2Int containing the sign of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector2Int Sign(this in Vector2Int v) =>
			new Vector2Int(v.x.Sign(), v.y.Sign());

		/// <returns>
		/// A Vector3 containing the sign of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector3 Sign(this in Vector3 v) =>
			new Vector3(v.x.Sign(), v.y.Sign(), v.z.Sign());

		/// <returns>
		/// A new Vector3Int containing the sign of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector3Int Sign(this in Vector3Int v) =>
			new Vector3Int(v.x.Sign(), v.y.Sign(), v.z.Sign());

		#endregion
		#region Pow

		/// <returns>
		/// The input float <paramref name="f"/> raised to the power of <paramref name="e"/>.
		///</returns>

		public static float Pow(this in float f, float e) =>
			System.MathF.Pow(f, e);

		/// <returns>
		/// The input Vector2 <paramref name="v"/> raised to the power of <paramref name="e"/>.
		///</returns>

		public static Vector2 Pow(this in Vector2 v, float e) =>
			new Vector2(v.x.Pow(e), v.y.Pow(e));

		/// <returns>
		/// The input Vector3 <paramref name="v"/> raised to the power of <paramref name="e"/>.
		///</returns>

		public static Vector3 Pow(this in Vector3 v, float e) =>
			new Vector3(v.x.Pow(e), v.y.Pow(e), v.z.Pow(e));

		#endregion
		#region Remap

		/// <returns>
		/// The input float <paramref name="value"/> remapped to the given input parameters. This is the equivalent of performing an inverse lerp and then a standard lerp on a value.
		///</returns>

		/** Reminder:
		*
		*	Lerp = a + (b - a) * t;
		*	Inv. = (t - a) / (b - a);
		*/

		public static float Remap(this in float value, in float inMin = 0f, in float inMax = 1f, in float outMin = 0f, in float outMax = 1f, in bool clamp = false)
		{
			var __result = outMin + ((outMax - outMin) * ((value - inMin) / (inMax - inMin)));

			return clamp ? __result.Clamp(outMin, outMax) : __result;
		}

		/// <inheritdoc cref="Remap"/>

		public static int Remap(this in int value, in int inMin = 0, in int inMax = 1, in int outMin = 0, in int outMax = 1, in bool clamp = false)
		{
			var __result = outMin + ((outMax - outMin) * ((value - inMin) / (inMax - inMin)));

			return clamp ? __result.Clamp(outMin, outMax) : __result;
		}

		/// <inheritdoc cref="Remap"/>

		public static Vector2 Remap(this in Vector2 value, in Vector2 inMin, in Vector2 inMax, in Vector2 outMin, in Vector2 outMax, in bool clamp = false) =>
			new Vector2(
				value.x.Remap(inMin.x, inMax.x, outMin.x, outMax.x, clamp),
				value.y.Remap(inMin.y, inMax.y, outMin.y, outMax.y, clamp)
			);

		/// <inheritdoc cref="Remap"/>

		public static Vector2Int Remap(this in Vector2Int value, in Vector2Int inMin, in Vector2Int inMax, in Vector2Int outMin, in Vector2Int outMax, in bool clamp = false) =>
			new Vector2Int(
				value.x.Remap(inMin.x, inMax.x, outMin.x, outMax.x, clamp),
				value.y.Remap(inMin.y, inMax.y, outMin.y, outMax.y, clamp)
			);

		/// <inheritdoc cref="Remap"/>

		public static Vector3 Remap(this in Vector3 value, in Vector3 inMin, in Vector3 inMax, in Vector3 outMin, in Vector3 outMax, in bool clamp = false) =>
			new Vector3(
				value.x.Remap(inMin.x, inMax.x, outMin.x, outMax.x, clamp),
				value.y.Remap(inMin.y, inMax.y, outMin.y, outMax.y, clamp),
				value.z.Remap(inMin.z, inMax.z, outMin.z, outMax.z, clamp)
			);

		/// <inheritdoc cref="Remap"/>

		public static Vector3Int Remap(this in Vector3Int value, in Vector3Int inMin, in Vector3Int inMax, in Vector3Int outMin, in Vector3Int outMax, in bool clamp = false) =>
			new Vector3Int(
				value.x.Remap(inMin.x, inMax.x, outMin.x, outMax.x, clamp),
				value.y.Remap(inMin.y, inMax.y, outMin.y, outMax.y, clamp),
				value.z.Remap(inMin.z, inMax.z, outMin.z, outMax.z, clamp)
			);

		#endregion
		#region Approx

		public const float DEFAULT_APPROX_THRESHOLD = 0.005f;

		/// <returns>
		/// TRUE if <paramref name="a"/> and <paramref name="b"/> are approximately equal as defined under a threshold of <paramref name="d"/>.
		///</returns>

		public static bool Approx(this in float a, in float b, in float d = DEFAULT_APPROX_THRESHOLD) =>
			(a - b).Abs() <= d;

		/// <inheritdoc cref="Approx(in float, in float, float)"/>/

		public static bool Approx(this in Vector2 a, in Vector2 b, in float d = DEFAULT_APPROX_THRESHOLD) =>
			a.x.Approx(b.x, d) && a.y.Approx(b.y, d);

		/// <inheritdoc cref="Approx(in float, in float, float)"/>

		public static bool Approx(this in Vector3 a, in Vector3 b, in float d = DEFAULT_APPROX_THRESHOLD) =>
			a.x.Approx(b.x, d) && a.y.Approx(b.y, d) && a.z.Approx(b.z, d);

		#endregion

		#region Miscellaneous

		#region ReassignMinMax

		/// <summary>
		/// Given an input <paramref name="labelledMin"/> and <paramref name="labelledMax"/> (that could be of any value), this function will reassign these variables to ensure that <paramref name="labelledMin"/> <= <paramref name="labelledMax"/>.
		///</summary>
		/// <param name="labelledMin">
		/// This should be a variable that is labelled as a minimum value and will be assured to be <= <paramref name="labelledMax"/>.
		///</param>
		/// <param name="labelledMax">
		/// This should be a variable that is labelled as a maximum value and will be assured to be >= <paramref name="labelledMin"/>.
		///</param>

		public static void ReassignMinMax(ref float labelledMin, ref float labelledMax)
		{
			float actualMin = Mathf.Min(labelledMin, labelledMax);
			labelledMax = Mathf.Max(labelledMin, labelledMax);
			labelledMin = actualMin;
		}

		#endregion
		#region ClampMod

		public static float ClampMod(this float value, float min, float max, float rangeMin, float rangeMax)
		{
			var mod = (rangeMax - rangeMin).Abs();
			if ((value %= mod) < 0f)
				value += mod;

			return Mathf.Clamp(value + Mathf.Min(rangeMin, rangeMax), min, max);
		}

		#endregion
		#region ClampAngle

		public static float ClampAngle(this float value, float min, float max)
		{
			if (value < 0f)
				value += 360f;
			if (value > 180f)
				return Mathf.Max(value, min + 360f);
			return Mathf.Min(value, max);
		}

		public static float ClampAngle(this float value, float minMax) =>
			ClampAngle(value, -(minMax.Abs()), minMax.Abs());

		#endregion
		#region RemapBool

		public static float RemapBool(this in bool b, in float f = 0f, in float t = 1f) =>
			b ? t : f;

		public static Vector2 RemapBool2(this in bool2 b, in float f = 0f, in float t = 1f) =>
			new Vector2
			(
				b.x.RemapBool(f, t),
				b.y.RemapBool(f, t)
			);

		public static Vector3 RemapBool3(this in bool3 b, in float f = 0f, in float t = 1f) =>
			new Vector3
			(
				b.x.RemapBool(f, t),
				b.y.RemapBool(f, t),
				b.z.RemapBool(f, t)
			);

		#endregion
		#region SwapValues

		public static void SwapValues(ref float a, ref float b)
		{
			float t = a;
			a = b;
			b = t;
		}

		#endregion

		#endregion

		#endregion

		#region Vector Math

		#region Components

		/// <returns>
		/// The largest of all components in this vector.
		///</returns>

		public static float MaxComponent(this in Vector3 v) => System.Math.Max(System.Math.Max(v.x, v.y), v.z);

		/// <returns>
		/// The largest of all components' absolute values in this vector.
		///</returns>

		public static float MaxComponentAbs(this in Vector3 v) => System.Math.Max(System.Math.Max(v.x.Abs(), v.y.Abs()), v.z.Abs());

		/// <returns>
		/// The smallest of all components in this vector.
		///</returns>

		public static float MinComponent(this in Vector3 v) => System.Math.Min(System.Math.Min(v.x, v.y), v.z);

		/// <returns>
		/// The smallest of all components' absolute values in this vector.
		///</returns>

		public static float MinComponentAbs(this in Vector3 v) => System.Math.Min(System.Math.Min(v.x.Abs(), v.y.Abs()), v.z.Abs());

		/// <inheritdoc cref="MaxComponent(in Vector3)"/>

		public static float MaxComponent(this in Vector2 v) => System.Math.Max(v.x, v.y);

		/// <inheritdoc cref="MaxComponentAbs(in Vector3)"/>

		public static float MaxComponentAbs(this in Vector2 v) => System.Math.Max(v.x.Abs(), v.y.Abs());

		/// <inheritdoc cref="MinComponent(in Vector3)"/>

		public static float MinComponent(this in Vector2 v) => System.Math.Min(v.x, v.y);

		/// <inheritdoc cref="MinComponentAbs(in Vector3)"/>

		public static float MinComponentAbs(this in Vector2 v) => System.Math.Min(v.x.Abs(), v.y.Abs());

		#endregion
		#region Vector3 Translation

		/// <returns>
		/// A Vector3 in which the input x is mapped to XZ and the input y is mapped to Y.
		///</returns>

		public static Vector3 XLateral_YVertical(in this Vector2 v) =>
			new Vector3(v.x, v.y, v.x);

		/// <inheritdoc cref="XLateral_YVertical(Vector2)"/>

		public static Vector3 XLateral_YVertical(in float x, in float y) =>
			new Vector3(x, y, x);

		/// <returns>
		/// The given input vector <paramref name="v"/> with only the Y and Z components.
		///</returns>
		/// <param name="x">
		/// Optional X component to set. Defaults to 0.
		///</param>

		public static Vector3 YZ(this in Vector3 v, in float x = 0f) =>
			new Vector3(x, v.y, v.z);

		/// <summary>
		/// Converts the given <see cref="Vector2"/> <paramref name="v"/> to a <see cref="Vector3"/> with the <see cref="Vector2.x"/> mapped to the <see cref="Vector3.z"/>.
		///</summary>
		/// <param name="y">
		/// Optional X component to set. Defaults to 0.
		///</param>

		public static Vector3 YZ(this in Vector2 v, in float x = 0f) =>
			new Vector3(x, v.y, v.x);

		/// <returns>
		/// The given input vector <paramref name="v"/> with only the X and Z components.
		///</returns>
		/// <param name="y">
		/// Optional Y component to set. Defaults to 0.
		///</param>

		public static Vector3 XZ(this in Vector3 v, in float y = 0f) =>
			new Vector3(v.x, y, v.z);

		/// <summary>
		/// Converts the given <see cref="Vector2"/> <paramref name="v"/> to a <see cref="Vector3"/> with the <see cref="Vector2.y"/> mapped to the <see cref="Vector3.z"/>.
		///</summary>
		/// <param name="y">
		/// Optional Y component to set. Defaults to 0.
		///</param>

		public static Vector3 XZ(this in Vector2 v, in float y = 0f) =>
			new Vector3(v.x, y, v.y);

		/// <returns>
		/// The given input vector <paramref name="v"/> with only the X and Y components.
		///</returns>
		/// <param name="z">
		/// Optional Z component to set. Defaults to 0.
		///</param>

		public static Vector3 XY(this in Vector3 v, in float z = 0f) =>
			new Vector3(v.x, v.y, z);

		/// <summary>
		/// Converts the given <see cref="Vector2"/> <paramref name="v"/> to a <see cref="Vector3"/>.
		///</summary>
		/// <param name="z">
		/// Optional Z component to set. Defaults to 0.
		///</param>

		public static Vector3 XY(this in Vector2 v, in float z = 0f) =>
			new Vector3(v.x, v.y, z);

		public static Vector3 Reciprocal(this in Vector3 v) =>
			new Vector3(1f / v.x, 1f / v.y, 1f / v.z);

		public static Vector3 Floor(this in Vector3 v) =>
			new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
		public static Vector3Int FloorToInt(this in Vector3 v) =>
			new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));

		public static Vector3 Ceil(this in Vector3 v) =>
			new Vector3(Mathf.Ceil(v.x), Mathf.Ceil(v.y), Mathf.Ceil(v.z));
		public static Vector3Int CeilInt(this in Vector3 v) =>
			new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));


		#endregion
		#region Vector2 Translation

		public static Vector2 ZY(this in Vector3 v) =>
			new Vector2(v.z, v.y);
		public static Vector2 XZ(this in Vector3 v) =>
			new Vector2(v.x, v.z);
		public static Vector2 XY(this in Vector3 v) =>
			new Vector2(v.x, v.y);

		public static Vector2 Reciprocal(this in Vector2 v) =>
			new Vector2(1f / v.x, 1f / v.y);

		public static Vector2 Floor(this in Vector2 v) =>
			new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
		public static Vector2Int FloorToInt(this in Vector2 v) =>
			new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));

		public static Vector2 Ceil(this in Vector2 v) =>
			new Vector2(Mathf.Ceil(v.x), Mathf.Ceil(v.y));
		public static Vector2Int CeilInt(this in Vector2 v) =>
			new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));

		#endregion

		#region Midpoint

		/// <returns>
		/// A new <see cref="Vector2"/> that is halfway inbetween <paramref name="a"/> and <paramref name="b"/>.
		///</returns>

		public static Vector2 Midpoint(in Vector2 a, in Vector2 b) =>
			Vector2.Lerp(a, b, 0.5f);

		/// <returns>
		/// A new <see cref="Vector3"/> that is halfway inbetween <paramref name="a"/> and <paramref name="b"/>.
		///</returns>

		public static Vector3 Midpoint(in Vector3 a, in Vector3 b) =>
			Vector3.Lerp(a, b, 0.5f);

		#endregion
		#region SmoothDampEulerAngles

		public static Vector3 SmoothDampEulerAngles(in Vector3 current, in Vector3 target, ref Vector3 currentVelocity, in Vector3 smoothTime, in Vector3 maxSpeed, float deltaTime)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new Vector3(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime.x, maxSpeed.x, deltaTime),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime.y, maxSpeed.y, deltaTime),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime.z, maxSpeed.z, deltaTime)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);
			return result;
		}

		public static Vector3 SmoothDampEulerAngles(in Vector3 current, in Vector3 target, ref Vector3 currentVelocity, in Vector3 smoothTime, in Vector3 maxSpeed)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new Vector3(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime.x, maxSpeed.x),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime.y, maxSpeed.y),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime.z, maxSpeed.z)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);
			return result;
		}

		public static Vector3 SmoothDampEulerAngles(in Vector3 current, in Vector3 target, ref Vector3 currentVelocity, in Vector3 smoothTime)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new Vector3(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime.x),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime.y),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime.z)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);
			return result;
		}

		public static Vector3 SmoothDampEulerAngles(in Vector3 current, in Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new Vector3(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime, maxSpeed, deltaTime),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime, maxSpeed, deltaTime),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime, maxSpeed, deltaTime)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);

			return result;
		}

		public static Vector3 SmoothDampEulerAngles(in Vector3 current, in Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new Vector3(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime, maxSpeed),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime, maxSpeed),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime, maxSpeed)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);

			return result;
		}

		public static Vector3 SmoothDampEulerAngles(in Vector3 current, in Vector3 target, ref Vector3 currentVelocity, float smoothTime)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new Vector3(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);

			return result;
		}

		#endregion

		#region SnapToGrid

		public static Vector3 SnapToGrid(this in Vector3 position, in Vector3 gridSize, in Vector3 offset) =>
			Vector3.Scale(Vector3.Scale((position - offset), gridSize.Reciprocal()).Floor(), gridSize) + offset;
		public static Vector3 SnapToGrid(this in Vector3 position, in float gridSize, in Vector3 offset) =>
			position.SnapToGrid(Vector3.one * gridSize, offset);
		public static Vector3 SnapToGrid(this in Vector3 position, in Vector3 gridSize) =>
			position.SnapToGrid(gridSize, Vector3.zero);
		public static Vector3 SnapToGrid(this in Vector3 position, in float gridSize = 1f) =>
			position.SnapToGrid(Vector3.one * gridSize, Vector3.zero);

		#endregion
		#region ProjectOnSphere

		/// <summary>
		/// Imagine you have a sphere and a point in 3D space, and a ray line going from the center of the sphere to the point (and beyond). This function will find the point along that ray line at which it intersects with the surface of the sphere.
		///</summary>
		/// <returns>
		/// A point on the surface of a sphere with a specified <paramref name="spherePosition"/> and <paramref name ="radius"/>.
		///</returns>
		/// <param name="position">
		/// The position in world space that we wish to project onto the surface of the sphere.
		///</param>
		/// <param name="spherePosition">
		/// The position in world space of the center of the sphere.
		///</param>
		/// <param name="radius">
		/// The radius of the sphere.
		///</param>

		public static Vector3 ProjectOnSphere(Vector3 position, Vector3 spherePosition, float radius)
		{
			Vector3 p = position - spherePosition;
			float pLength = System.MathF.Sqrt((p.x * p.x + p.y * p.y + p.z * p.z));
			Vector3 q = p * (radius / pLength.Abs());
			Vector3 r = q + position;
			return r;
		}

		#endregion

		#endregion

		#region Miscellaneous

		#region Color

		public static Color IgnoreAlpha(this in Color color, in float a) =>
			new Color(color.r, color.g, color.b, a);
		public static Color IgnoreAlpha(this in Color color) =>
			new Color(color.r, color.g, color.b, 1f);

		#endregion

		#endregion
	}
}
