using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	internal static class EnumerableExtensions
	{
		public static IEnumerable<T> Distinct<T, TValue>(this IEnumerable<T> list, Func<T, TValue> body)
		{
			Verify.NotNull<IEnumerable<T>>(Expression.Lambda<Func<IEnumerable<T>>>(Expression.Field(Expression.Constant(null), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions).GetField("list").FieldHandle, typeof(EnumerableExtensions).TypeHandle)), new ParameterExpression[0]), (string)null);
			Verify.NotNull<Func<T, TValue>>(Expression.Lambda<Func<Func<T, TValue>>>(Expression.Field(Expression.Constant(null), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions).GetField("body").FieldHandle, typeof(EnumerableExtensions).TypeHandle)), new ParameterExpression[0]), (string)null);
			return list.Distinct<T>(new EnumerableExtensions.GenericEqualityComparer<T, TValue>(body));
		}

		public static void ForEach<T>(this IEnumerable<T> list, Action<T> body)
		{
			Verify.NotNull<IEnumerable<T>>(Expression.Lambda<Func<IEnumerable<T>>>(Expression.Field(Expression.Constant(null), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions).GetField("list").FieldHandle, typeof(EnumerableExtensions).TypeHandle)), new ParameterExpression[0]), (string)null);
			Verify.NotNull<Action<T>>(Expression.Lambda<Func<Action<T>>>(Expression.Field(Expression.Constant(null), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions).GetField("body").FieldHandle, typeof(EnumerableExtensions).TypeHandle)), new ParameterExpression[0]), (string)null);
			foreach (T t in list)
			{
				body(t);
			}
		}

		public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> body)
		{
			Verify.NotNull<IEnumerable<T>>(Expression.Lambda<Func<IEnumerable<T>>>(Expression.Field(Expression.Constant(null), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions).GetField("list").FieldHandle, typeof(EnumerableExtensions).TypeHandle)), new ParameterExpression[0]), (string)null);
			Verify.NotNull<Action<T, int>>(Expression.Lambda<Func<Action<T, int>>>(Expression.Field(Expression.Constant(null), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions).GetField("body").FieldHandle, typeof(EnumerableExtensions).TypeHandle)), new ParameterExpression[0]), (string)null);
			int num = 0;
			foreach (T t in list)
			{
				int num1 = num;
				num = num1 + 1;
				body(t, num1);
			}
		}

		public static IEnumerable<TValue> GetValues<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict)
		{
			Verify.NotNull<ConcurrentDictionary<TKey, TValue>>(Expression.Lambda<Func<ConcurrentDictionary<TKey, TValue>>>(Expression.Field(Expression.Constant(null), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions).GetField("dict").FieldHandle, typeof(EnumerableExtensions).TypeHandle)), new ParameterExpression[0]), (string)null);
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dict)
			{
				yield return keyValuePair.Value;
			}
		}

		public static IEnumerable<T> OfType<T>(IList list)
		{
			foreach (object obj in list)
			{
				if (!(obj is T))
				{
					continue;
				}
				yield return (T)obj;
			}
		}

		public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list)
		{
			Verify.NotNull<IEnumerable<KeyValuePair<TKey, TValue>>>(Expression.Lambda<Func<IEnumerable<KeyValuePair<TKey, TValue>>>>(Expression.Field(Expression.Constant(null), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions).GetField("list").FieldHandle, typeof(EnumerableExtensions).TypeHandle)), new ParameterExpression[0]), (string)null);
			IDictionary<TKey, TValue> tKeys = new Dictionary<TKey, TValue>();
			foreach (KeyValuePair<TKey, TValue> keyValuePair in list)
			{
				tKeys[keyValuePair.Key] = keyValuePair.Value;
			}
			return tKeys;
		}

		internal sealed class GenericEqualityComparer<T, TValue> : IEqualityComparer<T>
		{
			private readonly Func<T, TValue> comparer;

			public GenericEqualityComparer(Func<T, TValue> comparer)
			{
				this.comparer = comparer;
			}

			public bool Equals(T x, T y)
			{
				TValue tValue = this.comparer(x);
				return tValue.Equals(this.comparer(y));
			}

			public int GetHashCode(T obj)
			{
				return this.comparer(obj).GetHashCode();
			}
		}
	}
}