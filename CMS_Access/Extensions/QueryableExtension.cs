using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CMS_Access.Extensions;

public class QueryableExtension<TQuery>
{
    public static IEnumerable<IQueryable<TQuery>> WhereIn<TKey>(IQueryable<TQuery> queryable,
        Expression<Func<TQuery, TKey>> keySelector, IEnumerable<TKey> values, int batchSize)
    {
        List<TKey> distinctValues = values.Distinct().ToList();
        int lastBatchSize = distinctValues.Count % batchSize;
        if (lastBatchSize != 0)
        {
            distinctValues.AddRange(Enumerable.Repeat(distinctValues.Last(), batchSize - lastBatchSize));
        }

        int count = distinctValues.Count;
        for (int i = 0; i < count; i += batchSize)
        {
            var body = distinctValues
                .Skip(i)
                .Take(batchSize)
                .Select(v =>
                {
                    // Create an expression that captures the variable so EF can turn this into a parameterized SQL query
                    Expression<Func<TKey>> valueAsExpression = () => v;
                    return Expression.Equal(keySelector.Body, valueAsExpression.Body);
                })
                .Aggregate(Expression.OrElse);
            var whereClause = Expression.Lambda<Func<TQuery, bool>>(body, keySelector.Parameters);
            yield return queryable.Where(whereClause);
        }
    }

// doesn't use batching
    public static IQueryable<TQuery> WhereIn<TKey>(IQueryable<TQuery> queryable,
        Expression<Func<TQuery, TKey>> keySelector, IEnumerable<TKey> values)
    {
        TKey[] distinctValues = values.Distinct().ToArray();


        int count = distinctValues.Length;
        for (int i = 0; i < count; ++i)
        {
            var body = distinctValues
                .Select(v =>
                {
                    // Create an expression that captures the variable so EF can turn this into a parameterized SQL query
                    Expression<Func<TKey>> valueAsExpression = () => v;
                    return Expression.Equal(keySelector.Body, valueAsExpression.Body);
                })
                .Aggregate(Expression.OrElse);

            var whereClause = Expression.Lambda<Func<TQuery, bool>>(body, keySelector.Parameters);
            return queryable.Where(whereClause);
        }

        return Enumerable.Empty<TQuery>().AsQueryable();
    }
}