using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;

namespace UoW.Implementation.Helpers.NHibernate
{
	public static class IQueryableExtensions
	{
		public static IFutureValue<TResult> ToFutureValue<TSource, TResult>(
			this IQueryable<TSource> source,
			Expression<Func<IQueryable<TSource>, TResult>> selector) where TResult : struct
		{
			var provider = (ISupportFutureBatchNhQueryProvider)source.Provider;
			var method = ((MethodCallExpression)selector.Body).Method;
			var expression = Expression.Call(null, method, source.Expression);
			return (IFutureValue<TResult>)provider.Session(expression);
		}
	}
}
