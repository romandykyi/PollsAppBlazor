using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace PollsAppBlazor.DataAccess.Extensions;

public static class SetPropertyCallsExtensions
{
    public static Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> AppendSetProperty<TEntity>(
        this Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>>? left,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> right)
    {
        if (left == null) return right;

        var replace = new ReplacingExpressionVisitor(right.Parameters, [left.Body]);
        var combined = replace.Visit(right.Body);
        return Expression.Lambda<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>>(combined, left.Parameters);
    }
}
