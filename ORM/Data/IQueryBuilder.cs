using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ORM.Data
{
    interface IQueryBuilder
    {
        string Translate(Expression expression);
        string GetQuery<T>(List<Expression<Func<T, bool>>> predicates) where T : DbEntity;
    }
}
