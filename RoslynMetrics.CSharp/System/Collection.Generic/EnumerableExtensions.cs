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
            return list.Distinct<T>(new EnumerableExtensions.GenericEqualityComparer<T, TValue>(body));
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> body)
        {
            foreach (T t in list)
            {
                body(t);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> body)
        {
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