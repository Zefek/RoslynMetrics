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
			EnumerableExtensions.<>c__DisplayClass4<T, TValue> variable = null;
			Verify.NotNull<IEnumerable<T>>(Expression.Lambda<Func<IEnumerable<T>>>(Expression.Field(Expression.Constant(variable), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions.<>c__DisplayClass4<T, TValue>).GetField("list").FieldHandle, typeof(EnumerableExtensions.<>c__DisplayClass4<T, TValue>).TypeHandle)), new ParameterExpression[0]), (string)null);
			Verify.NotNull<Func<T, TValue>>(Expression.Lambda<Func<Func<T, TValue>>>(Expression.Field(Expression.Constant(variable), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions.<>c__DisplayClass4<T, TValue>).GetField("body").FieldHandle, typeof(EnumerableExtensions.<>c__DisplayClass4<T, TValue>).TypeHandle)), new ParameterExpression[0]), (string)null);
			return list.Distinct<T>(new EnumerableExtensions.GenericEqualityComparer<T, TValue>(body));
		}

		public static void ForEach<T>(this IEnumerable<T> list, Action<T> body)
		{
			EnumerableExtensions.<>c__DisplayClass0<T> variable = null;
			Verify.NotNull<IEnumerable<T>>(Expression.Lambda<Func<IEnumerable<T>>>(Expression.Field(Expression.Constant(variable), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions.<>c__DisplayClass0<T>).GetField("list").FieldHandle, typeof(EnumerableExtensions.<>c__DisplayClass0<T>).TypeHandle)), new ParameterExpression[0]), (string)null);
			Verify.NotNull<Action<T>>(Expression.Lambda<Func<Action<T>>>(Expression.Field(Expression.Constant(variable), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions.<>c__DisplayClass0<T>).GetField("body").FieldHandle, typeof(EnumerableExtensions.<>c__DisplayClass0<T>).TypeHandle)), new ParameterExpression[0]), (string)null);
			foreach (T t in list)
			{
				body(t);
			}
		}

		public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> body)
		{
			EnumerableExtensions.<>c__DisplayClass2<T> variable = null;
			Verify.NotNull<IEnumerable<T>>(Expression.Lambda<Func<IEnumerable<T>>>(Expression.Field(Expression.Constant(variable), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions.<>c__DisplayClass2<T>).GetField("list").FieldHandle, typeof(EnumerableExtensions.<>c__DisplayClass2<T>).TypeHandle)), new ParameterExpression[0]), (string)null);
			Verify.NotNull<Action<T, int>>(Expression.Lambda<Func<Action<T, int>>>(Expression.Field(Expression.Constant(variable), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions.<>c__DisplayClass2<T>).GetField("body").FieldHandle, typeof(EnumerableExtensions.<>c__DisplayClass2<T>).TypeHandle)), new ParameterExpression[0]), (string)null);
			int num = 0;
			foreach (T t in list)
			{
				Action<T, int> action = body;
				int num1 = num;
				num = checked(num1 + 1);
				action(t, num1);
			}
		}

		public static IEnumerable<TValue> GetValues<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict)
		{
			EnumerableExtensions.<>c__DisplayClassf<TKey, TValue> variable = null;
			Verify.NotNull<ConcurrentDictionary<TKey, TValue>>(Expression.Lambda<Func<ConcurrentDictionary<TKey, TValue>>>(Expression.Field(Expression.Constant(variable), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions.<>c__DisplayClassf<TKey, TValue>).GetField("dict").FieldHandle, typeof(EnumerableExtensions.<>c__DisplayClassf<TKey, TValue>).TypeHandle)), new ParameterExpression[0]), (string)null);
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
			EnumerableExtensions.<>c__DisplayClass6<TKey, TValue> variable = null;
			Verify.NotNull<IEnumerable<KeyValuePair<TKey, TValue>>>(Expression.Lambda<Func<IEnumerable<KeyValuePair<TKey, TValue>>>>(Expression.Field(Expression.Constant(variable), FieldInfo.GetFieldFromHandle(typeof(EnumerableExtensions.<>c__DisplayClass6<TKey, TValue>).GetField("list").FieldHandle, typeof(EnumerableExtensions.<>c__DisplayClass6<TKey, TValue>).TypeHandle)), new ParameterExpression[0]), (string)null);
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