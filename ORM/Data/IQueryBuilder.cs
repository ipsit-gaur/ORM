using System;
using System.Linq.Expressions;

namespace ORM.Data
{
    interface IQueryBuilder
    {
        string Translate(Expression expression);
        string GetQuery<T>(Expression<Func<T, bool>> predicate) where T : DbEntity;
    }
}
