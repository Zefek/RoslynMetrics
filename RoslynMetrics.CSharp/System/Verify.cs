using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
	[DebuggerNonUserCode]
	internal static class Verify
	{
		public const string ArgumentNullExceptionDefaultMessage = "The passed reference is not accepted as a valid argument.";

		public const string ArgumentOutOfRangeExceptionDefaultMessage = "The argument is outside the allowable range of values as defined by the invoked method.";

		private static void BuildAndThrowException(ExceptionFactory factory, string memberName)
		{
			Exception exception = factory(memberName);
			if (exception != null)
			{
				throw exception;
			}
			throw new InvalidOperationException("The given factory returned an invalid exception object.");
		}

		public static void FieldNotNull<T>(Expression<Func<T>> memberExpression, string message = null)
		{
			Verify.NotNull<T>(memberExpression, (string paramName) => {
				if (!string.IsNullOrEmpty(message))
				{
					return new InvalidOperationException(message);
				}
				return new InvalidOperationException(string.Format("The \"{0}\" field has not been set.", paramName));
			});
		}

		private static string GetMemberName<T>(this Expression<Func<T>> memberExpression)
		{
			return ((MemberExpression)memberExpression.Body).Member.Name;
		}

		internal static bool IsInheritedFrom(Type instanceType, Type expectedType)
		{
			if (instanceType == null)
			{
				return false;
			}
			if (expectedType.IsInterface)
			{
				return instanceType.GetInterface(expectedType.FullName, false) != null;
			}
			if (instanceType == expectedType)
			{
				return true;
			}
			return Verify.IsInheritedFrom(instanceType.BaseType, expectedType);
		}

		public static void IsTypeOf<T>(Expression<Func<T>> memberExpression, Type expectedType)
		{
			if (memberExpression == null)
			{
				throw new ArgumentNullException("memberExpression", "The given member expression is invalid.");
			}
			if (!Verify.IsInheritedFrom(Verify.CachedExpressionCompiler<T>.Compile(memberExpression)().GetType(), expectedType))
			{
				Verify.BuildAndThrowException((string paramName) => new ArgumentException(string.Format("The \"{0}\" argument is not of type: {1}.", paramName, expectedType.FullName)), memberExpression.GetMemberName<T>());
			}
		}

		public static void NotInRange<T>(Expression<Func<T>> memberExpression, Func<T, bool> equalsTrue)
		{
			Verify.NotInRange<T>(memberExpression, equalsTrue, null);
		}

		public static void NotInRange<T>(Expression<Func<T>> memberExpression, Func<T, bool> equalsTrue, string errorMessage)
		{
			if (memberExpression == null)
			{
				throw new ArgumentNullException("memberExpression", "The given member expression is invalid.");
			}
			if (equalsTrue == null)
			{
				throw new ArgumentNullException("validator", "The given validator delegate is invalid.");
			}
			if (!equalsTrue(Verify.CachedExpressionCompiler<T>.Compile(memberExpression)()))
			{
				string memberName = memberExpression.GetMemberName<T>();
				if (!string.IsNullOrEmpty(errorMessage))
				{
					throw new ArgumentOutOfRangeException(memberName, errorMessage);
				}
				throw new ArgumentOutOfRangeException(memberName, "The argument is outside the allowable range of values as defined by the invoked method.");
			}
		}

		public static void NotNull<T>(Expression<Func<T>> memberExpression, string message = null)
		{
			Verify.NotNull<T>(memberExpression, (string paramName) => {
				if (string.IsNullOrEmpty(message))
				{
					return new ArgumentNullException(paramName, "The passed reference is not accepted as a valid argument.");
				}
				return new ArgumentNullException(paramName, message);
			});
		}

		public static void NotNull<T>(Expression<Func<T>> memberExpression, ExceptionFactory factory)
		{
			if (memberExpression == null)
			{
				throw new ArgumentNullException("memberExpression", "The given member expression is invalid.");
			}
			if (factory == null)
			{
				throw new ArgumentNullException("factory", "The given exception factory is invalid.");
			}
			if (object.Equals(Verify.CachedExpressionCompiler<T>.Compile(memberExpression)(), null))
			{
				Verify.BuildAndThrowException(factory, memberExpression.GetMemberName<T>());
			}
		}

		public static void NotNullOrEmpty(Expression<Func<string>> memberExpression, string message = null, bool allowEmpty = false)
		{
			if (memberExpression == null)
			{
				throw new ArgumentNullException("memberExpression", "The given member expression is invalid.");
			}
			string str = Verify.CachedExpressionCompiler<string>.Compile(memberExpression)();
			if (!string.IsNullOrEmpty(str))
			{
				return;
			}
			if (!(str == string.Empty) || !allowEmpty)
			{
				string memberName = memberExpression.GetMemberName<string>();
				if (!string.IsNullOrEmpty(message))
				{
					throw new ArgumentNullException(memberName, message);
				}
				throw new ArgumentNullException(memberName, "The passed reference is not accepted as a valid argument.");
			}
		}

		[DebuggerNonUserCode]
		public sealed class CachedExpressionCompiler<T>
		{
			private readonly static IDictionary<MemberInfo, Func<T>> Lookup;

			private readonly static object SyncObject;

			static CachedExpressionCompiler()
			{
				Verify.CachedExpressionCompiler<T>.Lookup = new Dictionary<MemberInfo, Func<T>>();
				Verify.CachedExpressionCompiler<T>.SyncObject = new object();
			}

			private CachedExpressionCompiler()
			{
			}

			public static Func<T> Compile(Expression<Func<T>> memberExpression)
			{
				if (memberExpression == null)
				{
					throw new ArgumentNullException("memberExpression");
				}
				if (memberExpression.Body is MemberExpression)
				{
					MemberExpression body = (MemberExpression)memberExpression.Body;
					if (body.Expression is MemberExpression)
					{
						return Verify.CachedExpressionCompiler<T>.GetMemberExpressionFunc(memberExpression);
					}
					ConstantExpression expression = body.Expression as ConstantExpression;
					ConstantExpression constantExpression = expression;
					if (expression != null)
					{
						object value = ((FieldInfo)body.Member).GetValue(constantExpression.Value);
						if (value is T)
						{
							return () => (T)value;
						}
					}
				}
				return memberExpression.Compile();
			}

			private static MemberExpression GetMemberExpression(MemberExpression expr)
			{
				MemberExpression expression = expr.Expression as MemberExpression;
				MemberExpression memberExpression = expression;
				if (expression == null)
				{
					return expr;
				}
				return Verify.CachedExpressionCompiler<T>.GetMemberExpression(memberExpression);
			}

			private static Func<T> GetMemberExpressionFunc(Expression<Func<T>> expression)
			{
				Func<T> func;
				MemberInfo member = Verify.CachedExpressionCompiler<T>.GetMemberExpression((MemberExpression)expression.Body).Member;
				if (!Verify.CachedExpressionCompiler<T>.Lookup.TryGetValue(member, out func))
				{
					lock (Verify.CachedExpressionCompiler<T>.SyncObject)
					{
						func = expression.Compile();
						Verify.CachedExpressionCompiler<T>.Lookup[member] = func;
					}
				}
				return func;
			}
		}
	}
}