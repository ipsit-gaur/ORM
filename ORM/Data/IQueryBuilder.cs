using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ORM.Data
{
    interface IQueryBuilder
    {
        string Translate(Expression expression, string operation);
        string PrepareQuery<T>(List<Expression<Func<T, bool>>> binaryPredicates) where T : DbEntity;
        string PrepareQuery<T>(List<Expression<Func<T, bool>>> binaryPredicates, Expression<Func<T, int>> predicate, string operation) where T : DbEntity;
        string PrepareQueryForInsert<T>(List<T> data) where T : DbEntity;
    }
}
