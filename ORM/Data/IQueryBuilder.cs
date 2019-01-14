using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ORM.Data
{
    interface IQueryBuilder
    {
        string Translate(Expression expression, string operation);
        string PrepareQuery<T>(IEnumerable<Expression<Func<T, bool>>> binaryPredicates) where T : DbEntity;
        string PrepareQuery<T>(IEnumerable<Expression<Func<T, bool>>> binaryPredicates, Expression<Func<T, int>> predicate, string operation) where T : DbEntity;
        string PrepareQueryForInsert<T>(IEnumerable<T> data) where T : DbEntity;
        string PrepareQueryForUpdate<T>(IEnumerable<T> data) where T : DbEntity;
        string PrepareQueryForDelete<T>(IEnumerable<T> data) where T : DbEntity;
    }
}
