
/** Math.cs
*
*	Created by LIAM WOFFORD.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Unity.Mathematics;

#endregion

namespace Mithril
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

		public static Vector3 Max(this Vector3 a, float b = 0f) => new(
			System.Math.Max(a.x, b),
			System.Math.Max(a.y, b),
			System.Math.Max(a.z, b)
		);

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
		#region Floor, Round, Ceil

		public static float Floor(this float value) =>
			Mathf.Floor(value);

		public static int FloorToInt(this float value) =>
			Mathf.FloorToInt(value);

		public static float Round(this float value) =>
			Mathf.Round(value);

		public static int RoundToInt(this float value) =>
			Mathf.RoundToInt(value);

		public static float Ceil(this float value) =>
			Mathf.Ceil(value);

		public static int CeilToInt(this float value) =>
			Mathf.CeilToInt(value);

		#endregion
		#region Wrap

		/// <summary>
		/// Keeps the <paramref name="value"/> constrained within the <paramref name="min"/> and <paramref name="max"/> by wrapping the value around to the other side if it goes under or over. This does NOT include the max number.
		///</summary>
		public static int Wrap(this int value, int min, int max)
		{
			int range = max - min + 1;
			return ((value - min) % range + range) % range + min;
		}
		/// <summary>
		/// Keeps the <paramref name="value"/> constrained within 0 and <paramref name="max"/> by wrapping the value around to the other side if it goes under or over. This does NOT include the max number.
		///</summary>
		public static int Wrap(this int value, int max) => value.Wrap(0, max);

		public static float Wrap(this float value, float min, float max) =>
			(value - min) % (max - min) + min;
		public static float Wrap(this float value, float max) =>
			value % max;

		#endregion
		#region Clamp

		/// <returns>
		/// The given <paramref name="value"/> clamped between <paramref name="min"/> and <paramref name="max"/>.
		///</returns>

		public static float Clamp(this float value, float min, float max) =>
			(value > max) ? max : ((value < min) ? min : value);

		/// <returns>
		/// The given <paramref name="value"/> clamped between 0 and <paramref name="max"/>.
		///</returns>

		public static float Clamp(this float value, float max) =>
			(value > max) ? max : ((value < 0f) ? 0f : value);

		/// <returns>
		/// The given <paramref name="value"/> clamped between 0 and 1.
		///</returns>

		public static float Clamp(this float value) =>
			(value > 1f) ? 1f : ((value < 0f) ? 0f : value);

		/// <returns>
		/// The input value, clamped between negative <paramref name="minMax"/> and positive <paramref name="minMax"/>.
		///</returns>

		public static float ClampAbs(this float value, float minMax = 1f) =>
			Mathf.Clamp(value, -(minMax.Abs()), minMax.Abs());

		/// <inheritdoc cref="Clamp(float, float, float)"/>

		public static int Clamp(this int value, int min, int max) =>
			(value > max) ? max : ((value < min) ? min : value);

		/// <inheritdoc cref="Clamp(float, float)"/>

		public static int Clamp(this int value, int max) =>
			(value > max) ? max : ((value < 0) ? 0 : value);

		/// <inheritdoc cref="Clamp(float)"/>

		public static int Clamp(this int value) =>
			(value > 1) ? 1 : ((value < 0) ? 0 : value);

		/// <inheritdoc cref="ClampAbs(float, float)"/>

		public static int ClampAbs(this int value, int minMax = 1) =>
			Mathf.Clamp(value, -(minMax.Abs()), minMax.Abs());

		/// <inheritdoc cref="Clamp(Vector3, Vector3, Vector3)"/>

		public static Vector2 Clamp(this Vector2 vector, Vector2 min, Vector2 max) =>
			new(
				vector.x.Clamp(min.x, max.x),
				vector.y.Clamp(min.y, max.y)
			);
		public static Vector2 Clamp(this Vector2 vector, Vector2 min, float max) =>
			new(
				vector.x.Clamp(min.x, max),
				vector.y.Clamp(min.y, max)
			);
		public static Vector2 Clamp(this Vector2 vector, float min, Vector2 max) =>
			new(
				vector.x.Clamp(min, max.x),
				vector.y.Clamp(min, max.y)
			);
		public static Vector2 Clamp(this Vector2 vector, float min, float max) =>
			new(
				vector.x.Clamp(min, max),
				vector.y.Clamp(min, max)
			);

		/// <returns>
		/// The given <paramref name="vector"/> clamped between 0 and <paramref name="max"/>, per axis.
		///</returns>

		public static Vector2 Clamp(this Vector2 vector, Vector2 max) =>
			new(
				vector.x.Clamp(0f, max.x),
				vector.y.Clamp(0f, max.y)
			);
		public static Vector2 Clamp(this Vector2 vector, float max) =>
			new(
				vector.x.Clamp(0f, max),
				vector.y.Clamp(0f, max)
			);

		/// <returns>
		/// The given <paramref name="vector"/> clamped between 0 and 1, per axis.
		///</returns>

		public static Vector2 Clamp(this Vector2 vector) =>
			new(
				vector.x.Clamp(0f, 1f),
				vector.y.Clamp(0f, 1f)
			);

		/// <returns>
		/// The input value clamped between negative <paramref name="minMax"/> and positive <paramref name="minMax"/>, per axis.
		///</returns>

		public static Vector2 ClampAbs(this Vector2 vector, Vector2 minMax) =>
			vector.Clamp(-minMax, minMax);
		public static Vector2 ClampAbs(this Vector2 vector, float minMax) =>
			vector.Clamp(-minMax, minMax);

		/// <returns>
		/// The input value clamped between -1 and +1, per axis.
		///</returns>

		public static Vector2 ClampAbs(this Vector2 vector) =>
			vector.Clamp(Vector2.one);

		/// <returns>
		/// The given <paramref name="vector"/> clamped between <paramref name="min"/> and <paramref name="max"/>, per axis.
		///</returns>

		public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max) =>
			new(
				vector.x.Clamp(min.x, max.x),
				vector.y.Clamp(min.y, max.y),
				vector.z.Clamp(min.z, max.z)
			);
		public static Vector3 Clamp(this Vector3 vector, Vector3 min, float max) =>
			new(
				vector.x.Clamp(min.x, max),
				vector.y.Clamp(min.y, max),
				vector.z.Clamp(min.z, max)
			);
		public static Vector3 Clamp(this Vector3 vector, float min, Vector3 max) =>
			new(
				vector.x.Clamp(min, max.x),
				vector.y.Clamp(min, max.y),
				vector.z.Clamp(min, max.z)
			);
		public static Vector3 Clamp(this Vector3 vector, float min, float max) =>
			new(
				vector.x.Clamp(min, max),
				vector.y.Clamp(min, max),
				vector.z.Clamp(min, max)
			);

		/// <returns>
		/// The given <paramref name="vector"/> clamped between 0 and <paramref name="max"/>, per axis.
		///</returns>

		public static Vector3 Clamp(this Vector3 vector, Vector3 max) =>
			new(
				vector.x.Clamp(0f, max.x),
				vector.y.Clamp(0f, max.y),
				vector.z.Clamp(0f, max.z)
			);
		public static Vector3 Clamp(this Vector3 vector, float max) =>
			new(
				vector.x.Clamp(0f, max),
				vector.y.Clamp(0f, max),
				vector.z.Clamp(0f, max)
			);

		/// <returns>
		/// The given <paramref name="vector"/> clamped between 0 and 1, per axis.
		///</returns>

		public static Vector3 Clamp(this Vector3 vector) =>
			new(
				vector.x.Clamp(0f, 1f),
				vector.y.Clamp(0f, 1f),
				vector.z.Clamp(0f, 1f)
			);

		/// <returns>
		/// The input value clamped between negative <paramref name="minMax"/> and positive <paramref name="minMax"/>, per axis.
		///</returns>

		public static Vector3 ClampAbs(this Vector3 vector, Vector3 minMax) =>
			vector.Clamp(-minMax, minMax);
		public static Vector3 ClampAbs(this Vector3 vector, float minMax) =>
			vector.Clamp(-minMax, minMax);

		/// <returns>
		/// The input value clamped between -1 and +1, per axis.
		///</returns>

		public static Vector3 ClampAbs(this Vector3 vector) =>
			vector.Clamp(Vector3.one);

		#endregion
		#region Abs

		public static float Abs(this float f) =>
			System.Math.Abs(f);

		public static int Abs(this int i) =>
			System.Math.Abs(i);

		/// <returns>
		/// A Vector2 containing the absolute value of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector2 Abs(this Vector2 v) =>
			new(v.x.Abs(), v.y.Abs());

		/// <inheritdoc cref="Abs(Vector2)"/>

		public static Vector2Int Abs(this Vector2Int v) =>
			new(v.x.Abs(), v.y.Abs());

		/// <returns>
		/// A Vector3 containing the absolute value of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector3 Abs(this Vector3 v) =>
			new(v.x.Abs(), v.y.Abs(), v.z.Abs());

		/// <inheritdoc cref="Abs(Vector3)"/>

		public static Vector3Int Abs(this Vector3Int v) =>
			new(v.x.Abs(), v.y.Abs(), v.z.Abs());

		#endregion
		#region Sign

		public static float Sign(this float value) =>
			System.Math.Sign(value);

		public static int Sign(this int value) =>
			System.Math.Sign(value);

		/// <returns>
		/// A Vector2 containing the sign of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector2 Sign(this Vector2 v) =>
			new(v.x.Sign(), v.y.Sign());

		/// <returns>
		/// A Vector2Int containing the sign of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector2Int Sign(this Vector2Int v) =>
			new(v.x.Sign(), v.y.Sign());

		/// <returns>
		/// A Vector3 containing the sign of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector3 Sign(this Vector3 v) =>
			new(v.x.Sign(), v.y.Sign(), v.z.Sign());

		/// <returns>
		/// A new Vector3Int containing the sign of input vector <paramref name="v"/>'s components.
		///</returns>

		public static Vector3Int Sign(this Vector3Int v) =>
			new(v.x.Sign(), v.y.Sign(), v.z.Sign());

		#endregion
		#region Pow

		/// <returns>
		/// The input float <paramref name="f"/> raised to the power of <paramref name="e"/>.
		///</returns>

		public static float Pow(this float f, float e) =>
			MathF.Pow(f, e);

		/// <returns>
		/// The input Vector2 <paramref name="v"/> raised to the power of <paramref name="e"/>.
		///</returns>

		public static Vector2 Pow(this Vector2 v, float e) => new(
			MathF.Pow(v.x, e),
			MathF.Pow(v.y, e)
		);

		/// <inheritdoc cref="Pow(Vector2, float)"/>

		public static Vector2 Pow(this Vector2 v, Vector2 e) => new(
			MathF.Pow(v.x, e.x),
			MathF.Pow(v.y, e.y)
		);

		/// <returns>
		/// The input Vector3 <paramref name="v"/> raised to the power of <paramref name="e"/>.
		///</returns>

		public static Vector3 Pow(this Vector3 v, float e) => new(
			MathF.Pow(v.x, e),
			MathF.Pow(v.y, e),
			MathF.Pow(v.z, e)
		);

		/// <inheritdoc cref="Pow(Vector3, float)"/>

		public static Vector3 Pow(this Vector3 v, Vector3 e) => new(
			MathF.Pow(v.x, e.x),
			MathF.Pow(v.y, e.y),
			MathF.Pow(v.z, e.z)
		);

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

		public static float Remap(this float value, float inMin = 0f, float inMax = 1f, float outMin = 0f, float outMax = 1f, bool clamp = false)
		{
			var __result = outMin + ((outMax - outMin) * ((value - inMin) / (inMax - inMin)));
			return clamp ? __result.Clamp(outMin, outMax) : __result;
		}

		/// <inheritdoc cref="Remap"/>

		public static int Remap(this int value, int inMin = 0, int inMax = 1, int outMin = 0, int outMax = 1, bool clamp = false)
		{
			var __result = outMin + ((outMax - outMin) * ((value - inMin) / (inMax - inMin)));
			return clamp ? __result.Clamp(outMin, outMax) : __result;
		}

		/// <inheritdoc cref="Remap"/>

		public static Vector2 Remap(this Vector2 value, Vector2 inMin, Vector2 inMax, Vector2 outMin, Vector2 outMax, bool clamp = false) =>
			new(
				value.x.Remap(inMin.x, inMax.x, outMin.x, outMax.x, clamp),
				value.y.Remap(inMin.y, inMax.y, outMin.y, outMax.y, clamp)
			);

		/// <inheritdoc cref="Remap"/>

		public static Vector2Int Remap(this Vector2Int value, Vector2Int inMin, Vector2Int inMax, Vector2Int outMin, Vector2Int outMax, bool clamp = false) =>
			new(
				value.x.Remap(inMin.x, inMax.x, outMin.x, outMax.x, clamp),
				value.y.Remap(inMin.y, inMax.y, outMin.y, outMax.y, clamp)
			);

		/// <inheritdoc cref="Remap"/>

		public static Vector3 Remap(this Vector3 value, Vector3 inMin, Vector3 inMax, Vector3 outMin, Vector3 outMax, bool clamp = false) =>
			new(
				value.x.Remap(inMin.x, inMax.x, outMin.x, outMax.x, clamp),
				value.y.Remap(inMin.y, inMax.y, outMin.y, outMax.y, clamp),
				value.z.Remap(inMin.z, inMax.z, outMin.z, outMax.z, clamp)
			);

		/// <inheritdoc cref="Remap"/>

		public static Vector3Int Remap(this Vector3Int value, Vector3Int inMin, Vector3Int inMax, Vector3Int outMin, Vector3Int outMax, bool clamp = false) =>
			new(
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

		public static bool Approx(this float a, float b, float d = DEFAULT_APPROX_THRESHOLD) =>
			(a - b).Abs() <= d;

		/// <inheritdoc cref="Approx(float, float, float)"/>/

		public static bool Approx(this Vector2 a, Vector2 b, float d = DEFAULT_APPROX_THRESHOLD) =>
			a.x.Approx(b.x, d) && a.y.Approx(b.y, d);

		/// <inheritdoc cref="Approx(float, float, float)"/>

		public static bool Approx(this Vector3 a, Vector3 b, float d = DEFAULT_APPROX_THRESHOLD) =>
			a.x.Approx(b.x, d) && a.y.Approx(b.y, d) && a.z.Approx(b.z, d);

		#endregion
		#region Closest

		/// <returns>
		/// The value <paramref name="values"/> that is closest to the <paramref name="target"/> value.
		///</returns>
		public static float Closest(this IEnumerable<float> values, float target)
		{
			float minDistance = float.MaxValue;
			float result = 0f;

			foreach (var iValue in values)
			{
				var iDist = (target - iValue).Abs();
				if (iDist > minDistance) continue;

				minDistance = iDist;
				result = iValue;
			}

			if (minDistance == float.MaxValue) throw new Exception($"Enumerable {values} must contain at least one element to find the closest value.");

			return result;
		}
		/// <returns>
		/// The position <paramref name="positions"/> that is closest to the <paramref name="target"/> position.
		///</returns>
		public static Vector2 Closest(this IEnumerable<Vector2> positions, Vector2 target)
		{
			float minDistance = float.MaxValue;
			Vector2 result = Vector2.zero;

			foreach (var iPosition in positions)
			{
				var iDist = Vector2.SqrMagnitude(target - iPosition);
				if (iDist > minDistance) continue;

				minDistance = iDist;
				result = iPosition;
			}

			if (minDistance == float.MaxValue) throw new Exception($"Enumerable {positions} must contain at least one element to find the closest value.");

			return result;
		}
		/// <returns>
		/// The position <paramref name="positions"/> that is closest to the <paramref name="target"/> position.
		///</returns>
		public static Vector3 Closest(this IEnumerable<Vector3> positions, Vector3 target)
		{
			float minDistance = float.MaxValue;
			Vector3 result = Vector3.zero;

			foreach (var iPosition in positions)
			{
				var iDist = Vector3.SqrMagnitude(target - iPosition);
				if (iDist > minDistance) continue;

				minDistance = iDist;
				result = iPosition;
			}

			if (minDistance == float.MaxValue) throw new Exception($"Enumerable {positions} must contain at least one element to find the closest value.");

			return result;
		}

		#endregion
		#region Average

		public static float Average(this IEnumerable<float> values) =>
			Enumerable.Average(values);

		public static Vector2 Average(this IEnumerable<Vector2> positions) =>
			new(
				positions.Select(i => i.x).Average(),
				positions.Select(i => i.y).Average()
			);

		public static Vector3 Average(this IEnumerable<Vector3> positions) =>
			new(
				positions.Select(i => i.x).Average(),
				positions.Select(i => i.y).Average(),
				positions.Select(i => i.z).Average()
			);

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
			value = Mathf.Repeat(value, 360f);
			if (value > 180f)
				return Mathf.Max(value, min + 360f);
			else
				return Mathf.Min(value, max);
		}

		public static float ClampAngle(this float value, float minMax) =>
			ClampAngle(value, -minMax.Abs(), minMax.Abs());

		#endregion
		#region RemapBool

		public static int RemapBool(this bool b, int f = 0, int t = 1) =>
			b ? t : f;
		public static float RemapBool(this bool b, float f = 0f, float t = 1f) =>
			b ? t : f;
		public static Vector2Int RemapBool2(this bool2 b, int f = 0, int t = 1) =>
			new(b.x.RemapBool(f, t), b.y.RemapBool(f, t));
		public static Vector2 RemapBool2(this bool2 b, float f = 0f, float t = 1f) =>
			new(b.x.RemapBool(f, t), b.y.RemapBool(f, t));
		public static Vector3Int RemapBool3(this bool3 b, int f = 0, int t = 1) =>
			new(b.x.RemapBool(f, t), b.y.RemapBool(f, t), b.z.RemapBool(f, t));
		public static Vector3 RemapBool3(this bool3 b, float f = 0f, float t = 1f) =>
			new(b.x.RemapBool(f, t), b.y.RemapBool(f, t), b.z.RemapBool(f, t));

		#endregion
		#region SwapValues

		public static void SwapValues(ref float a, ref float b) =>
			(b, a) = (a, b);

		#endregion

		#endregion

		#endregion

		#region Vectors

		#region Components

		/// <returns>
		/// The largest of all components this vector.
		///</returns>

		public static float MaxComponent(this Vector3 v) => System.Math.Max(System.Math.Max(v.x, v.y), v.z);

		/// <returns>
		/// The largest of all components' absolute values this vector.
		///</returns>

		public static float MaxComponentAbs(this Vector3 v) => System.Math.Max(System.Math.Max(v.x.Abs(), v.y.Abs()), v.z.Abs());

		/// <returns>
		/// The smallest of all components this vector.
		///</returns>

		public static float MinComponent(this Vector3 v) => System.Math.Min(System.Math.Min(v.x, v.y), v.z);

		/// <returns>
		/// The smallest of all components' absolute values this vector.
		///</returns>

		public static float MinComponentAbs(this Vector3 v) => System.Math.Min(System.Math.Min(v.x.Abs(), v.y.Abs()), v.z.Abs());

		/// <inheritdoc cref="MaxComponent(Vector3)"/>

		public static float MaxComponent(this Vector2 v) => System.Math.Max(v.x, v.y);

		/// <inheritdoc cref="MaxComponentAbs(Vector3)"/>

		public static float MaxComponentAbs(this Vector2 v) => System.Math.Max(v.x.Abs(), v.y.Abs());

		/// <inheritdoc cref="MinComponent(Vector3)"/>

		public static float MinComponent(this Vector2 v) => System.Math.Min(v.x, v.y);

		/// <inheritdoc cref="MinComponentAbs(Vector3)"/>

		public static float MinComponentAbs(this Vector2 v) => System.Math.Min(v.x.Abs(), v.y.Abs());

		#endregion
		#region Vector3 Translation

		/// <returns>
		/// A Vector3 which the input x is mapped to XZ and the input y is mapped to Y.
		///</returns>

		public static Vector3 XLateral_YVertical(this Vector2 v) =>
			new(v.x, v.y, v.x);

		/// <inheritdoc cref="XLateral_YVertical(Vector2)"/>

		public static Vector3 XLateral_YVertical(float x, float y) =>
			new(x, y, x);

		/// <returns>
		/// The given input vector <paramref name="v"/> with only the Y and Z components.
		///</returns>
		/// <param name="x">
		/// Optional X component to set. Defaults to 0.
		///</param>

		public static Vector3 YZ(this Vector3 v, float x = 0f) =>
			new(x, v.y, v.z);

		/// <summary>
		/// Converts the given <see cref="Vector2"/> <paramref name="v"/> to a <see cref="Vector3"/> with the <see cref="Vector2.x"/> mapped to the <see cref="Vector3.z"/>.
		///</summary>
		/// <param name="y">
		/// Optional X component to set. Defaults to 0.
		///</param>

		public static Vector3 YZ(this Vector2 v, float x = 0f) =>
			new(x, v.y, v.x);

		/// <returns>
		/// The given input vector <paramref name="v"/> with only the X and Z components.
		///</returns>
		/// <param name="y">
		/// Optional Y component to set. Defaults to 0.
		///</param>

		public static Vector3 XZ(this Vector3 v, float y = 0f) =>
			new(v.x, y, v.z);

		/// <summary>
		/// Converts the given <see cref="Vector2"/> <paramref name="v"/> to a <see cref="Vector3"/> with the <see cref="Vector2.y"/> mapped to the <see cref="Vector3.z"/>.
		///</summary>
		/// <param name="y">
		/// Optional Y component to set. Defaults to 0.
		///</param>

		public static Vector3 XZ(this Vector2 v, float y = 0f) =>
			new(v.x, y, v.y);

		/// <returns>
		/// The given input vector <paramref name="v"/> with only the X and Y components.
		///</returns>
		/// <param name="z">
		/// Optional Z component to set. Defaults to 0.
		///</param>

		public static Vector3 XY(this Vector3 v, float z = 0f) =>
			new(v.x, v.y, z);

		/// <summary>
		/// Converts the given <see cref="Vector2"/> <paramref name="v"/> to a <see cref="Vector3"/>.
		///</summary>
		/// <param name="z">
		/// Optional Z component to set. Defaults to 0.
		///</param>

		public static Vector3 XY(this Vector2 v, float z = 0f) =>
			new(v.x, v.y, z);

		public static Vector3 Reciprocal(this Vector3 v) =>
			new(1f / v.x, 1f / v.y, 1f / v.z);

		public static Vector3 Floor(this Vector3 v) =>
			new(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
		public static Vector3Int FloorToInt(this Vector3 v) =>
			new(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));

		public static Vector3 Ceil(this Vector3 v) =>
			new(Mathf.Ceil(v.x), Mathf.Ceil(v.y), Mathf.Ceil(v.z));
		public static Vector3Int CeilInt(this Vector3 v) =>
			new(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));

		public static Vector3 Round(this Vector3 v) =>
			new(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
		public static Vector3Int RoundToInt(this Vector3 v) =>
			new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));

		#endregion
		#region Vector2 Translation

		public static Vector2 ZY(this Vector3 v) =>
			new(v.z, v.y);
		public static Vector2 XZ(this Vector3 v) =>
			new(v.x, v.z);
		public static Vector2 XY(this Vector3 v) =>
			new(v.x, v.y);

		public static Vector2 Reciprocal(this Vector2 v) =>
			new(1f / v.x, 1f / v.y);

		public static Vector2 Floor(this Vector2 v) =>
			new(Mathf.Floor(v.x), Mathf.Floor(v.y));
		public static Vector2Int FloorToInt(this Vector2 v) =>
			new(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));

		public static Vector2 Ceil(this Vector2 v) =>
			new(Mathf.Ceil(v.x), Mathf.Ceil(v.y));
		public static Vector2Int CeilInt(this Vector2 v) =>
			new(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));

		public static Vector2 Round(this Vector2 v) =>
			new(Mathf.Round(v.x), Mathf.Round(v.y));
		public static Vector2Int RoundToInt(this Vector2 v) =>
			new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));

		#endregion

		#region Midpoint

		/// <returns>
		/// A new <see cref="Vector2"/> that is halfway inbetween <paramref name="a"/> and <paramref name="b"/>.
		///</returns>

		public static Vector2 Midpoint(Vector2 a, Vector2 b) =>
			Vector2.Lerp(a, b, 0.5f);

		/// <returns>
		/// A new <see cref="Vector3"/> that is halfway inbetween <paramref name="a"/> and <paramref name="b"/>.
		///</returns>

		public static Vector3 Midpoint(Vector3 a, Vector3 b) =>
			Vector3.Lerp(a, b, 0.5f);

		#endregion
		#region SmoothDampEulerAngles

		public static Vector3 SmoothDampEulerAngles(Vector3 current, Vector3 target, ref Vector3 currentVelocity, Vector3 smoothTime, Vector3 maxSpeed, float deltaTime)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime.x, maxSpeed.x, deltaTime),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime.y, maxSpeed.y, deltaTime),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime.z, maxSpeed.z, deltaTime)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);
			return result;
		}

		public static Vector3 SmoothDampEulerAngles(Vector3 current, Vector3 target, ref Vector3 currentVelocity, Vector3 smoothTime, Vector3 maxSpeed)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime.x, maxSpeed.x),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime.y, maxSpeed.y),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime.z, maxSpeed.z)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);
			return result;
		}

		public static Vector3 SmoothDampEulerAngles(Vector3 current, Vector3 target, ref Vector3 currentVelocity, Vector3 smoothTime)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime.x),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime.y),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime.z)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);
			return result;
		}

		public static Vector3 SmoothDampEulerAngles(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime, maxSpeed, deltaTime),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime, maxSpeed, deltaTime),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime, maxSpeed, deltaTime)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);

			return result;
		}

		public static Vector3 SmoothDampEulerAngles(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime, maxSpeed),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime, maxSpeed),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime, maxSpeed)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);

			return result;
		}

		public static Vector3 SmoothDampEulerAngles(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
		{
			float rotationX = currentVelocity.x, rotationY = currentVelocity.y, rotationZ = currentVelocity.z;

			Vector3 result = new(
				Mathf.SmoothDampAngle(current.x, target.x, ref rotationX, smoothTime),
				Mathf.SmoothDampAngle(current.y, target.y, ref rotationY, smoothTime),
				Mathf.SmoothDampAngle(current.z, target.z, ref rotationZ, smoothTime)
			);

			currentVelocity = new Vector3(rotationX, rotationY, rotationZ);

			return result;
		}

		#endregion

		#region SnapToGrid

		public static Vector3 SnapToGrid(this Vector3 position, Vector3 gridSize, Vector3 offset) =>
			Vector3.Scale(Vector3.Scale((position - offset), gridSize.Reciprocal()).Floor(), gridSize) + offset;
		public static Vector3 SnapToGrid(this Vector3 position, float gridSize, Vector3 offset) =>
			position.SnapToGrid(Vector3.one * gridSize, offset);
		public static Vector3 SnapToGrid(this Vector3 position, Vector3 gridSize) =>
			position.SnapToGrid(gridSize, Vector3.zero);
		public static Vector3 SnapToGrid(this Vector3 position, float gridSize = 1f) =>
			position.SnapToGrid(Vector3.one * gridSize, Vector3.zero);

		#endregion
		#region ProjectOnSphere

		/// <summary>
		/// Imagine you have a sphere and a point 3D space, and a ray line going from the center of the sphere to the point (and beyond). This function will find the point along that ray line at which it intersects with the surface of the sphere.
		///</summary>
		/// <returns>
		/// A point on the surface of a sphere with a specified <paramref name="spherePosition"/> and <paramref name ="radius"/>.
		///</returns>
		/// <param name="position">
		/// The position world space that we wish to project onto the surface of the sphere.
		///</param>
		/// <param name="spherePosition">
		/// The position world space of the center of the sphere.
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

		#region Quaternions

		public static Quaternion ProjectRotationOnAxis(this Quaternion rotation, Vector3 axis)
		{
			return Quaternion.Euler(Vector3.Scale(rotation.eulerAngles, axis));
		}

		#endregion

		#region Miscellaneous

		#region Color

		public static Color IgnoreAlpha(this Color color, float a) =>
			new(color.r, color.g, color.b, a);
		public static Color IgnoreAlpha(this Color color) =>
			new(color.r, color.g, color.b, 1f);

		#endregion

		#endregion
	}
}
