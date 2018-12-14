using ORM.Common;
using System;
using System.Linq.Expressions;

namespace ORM.SQL
{
    internal class SQLQueryProvider : QueryProvider
    {
        public override object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public override string GetQueryText(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
