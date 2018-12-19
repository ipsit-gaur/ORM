using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORM.SQL;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ORM.Tests.Data
{
    [TestClass]
    public class ContextTests
    {
        [TestMethod]
        public void ContextCreationTest()
        {
            TestContext myContext = new TestContext();
        }

        [TestMethod]
        public void VerifyDataSetsAreInitialized()
        {

            var myContext = new TestContext();

            ParameterExpression argParam = Expression.Parameter(typeof(Errors), "s");
            Expression nameProperty = Expression.Property(argParam, "ErrorID");
            Expression prop = Expression.Property(argParam, "Description");

            var val1 = Expression.Constant(1);
            var val2 = Expression.Constant("abc");

            Expression e1 = Expression.Equal(nameProperty, val1);
            Expression e2 = Expression.Equal(prop, val2);

            Expression andExp = Expression.AndAlso(e1, e2);
            var lambda = Expression.Lambda<Func<Errors, bool>>(andExp, argParam);
            var response = new SQLQueryBuilder().Translate(lambda);

            myContext.Errors.Read().Where(x => x.ErrorID == 1);

            //Assert.AreEqual(10, myContext.Errors.Read().Count);
        }
    }
}
