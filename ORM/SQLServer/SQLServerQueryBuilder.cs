﻿using ORM.Attributes;
using ORM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ORM.SQLServer
{
    // TODO: Should be in ORM.SQLServer Assembly
    internal class SQLServerQueryBuilder : IQueryBuilder
    {
        private SQLServerExpressionVisitor _expressionVisitor;

        public SQLServerQueryBuilder()
        {
            _expressionVisitor = new SQLServerExpressionVisitor();
        }

        public string Translate(Expression expression, string operation)
        {
            switch (operation)
            {
                case SQLServerKeywords.WHERE:
                    _expressionVisitor.Visit(expression);
                    return _expressionVisitor.WhereClause;
                default:
                    throw new NotSupportedException($"{operation} is not supported");
            }
        }

        private string GetQuery<T>(Expression<Func<T, bool>> predicate) where T : DbEntity
        {
            if (predicate == null)
                throw new ArgumentException("Predicate cannot be null");
            return Translate(predicate, SQLServerKeywords.WHERE);
        }

        // TODO: This should be made generic
        private string GetQuery<T>(Expression<Func<T, int>> predicate, string operation) where T : DbEntity
        {
            if (predicate == null)
                throw new ArgumentException("Predicate cannot be null");
            return Translate(predicate, operation);
        }

        public string PrepareQuery<T>(IEnumerable<Expression<Func<T, bool>>> predicates) where T : DbEntity
        {
            var sb = new StringBuilder();
            sb.Append(SQLServerKeywords.SELECT);
            sb.Append(SQLServerKeywords.ALL);
            sb.Append(SQLServerKeywords.FROM);
            sb.Append(GetTableNameFromType<T>());
            sb.Append(" ");

            if (predicates == null)
                return sb.ToString();

            var isWhereAdded = false;
            foreach (var predicate in predicates)
            {
                var whereClause = GetQuery<T>(predicate);
                if (!isWhereAdded)
                {
                    sb.Append(SQLServerKeywords.WHERE);
                    isWhereAdded = true;
                }
                else
                    sb.Append(SQLServerKeywords.AND);
                sb.Append(whereClause);
                sb.Append(" ");
            }
            return sb.ToString();
        }

        private string GetTableNameFromType<T>() where T : DbEntity
        {
            return $"[{typeof(T).Name}]";
        }

        public string PrepareQuery<T>(IEnumerable<Expression<Func<T, bool>>> binaryPredicates, Expression<Func<T, int>> predicate, string operation) where T : DbEntity
        {
            var clause = GetQuery(predicate, operation);
            return null;
        }

        public string PrepareQueryForInsert<T>(IEnumerable<T> data) where T : DbEntity
        {
            if (data == null || data.Count() == 0)
                return string.Empty;

            var insertQueryBuilder = new StringBuilder();
            foreach (var record in data)
            {
                insertQueryBuilder.Append(PrepareQueryForInsert(record));
            }
            return insertQueryBuilder.ToString();
        }

        private string PrepareQueryForInsert<T>(T record) where T : DbEntity
        {
            var sb = new StringBuilder();
            sb.Append(SQLServerKeywords.INSERT);
            sb.Append(SQLServerKeywords.INTO);
            sb.Append(GetTableNameFromType<T>());
            sb.Append(" ( ");
            var properties = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var index = 0; index < properties.Length; index++)
            {
                var property = properties[index];
                var attribs = property.GetCustomAttributes(typeof(AutoGeneratedColumnAttribute));
                var isAutoGeneratedColumn = attribs != null && attribs.Count() > 0;
                if (isAutoGeneratedColumn)
                    continue;
                sb.Append(property.Name);
                if (index != properties.Length - 1)
                    sb.Append(", ");
            }
            sb.Append(" ) ");

            sb.Append(SQLServerKeywords.VALUES);

            sb.Append(" ( ");
            for (var index = 0; index < properties.Length; index++)
            {
                var property = properties[index];
                var attribs = property.GetCustomAttributes(typeof(AutoGeneratedColumnAttribute));
                var isAutoGeneratedColumn = attribs != null && attribs.Count() > 0;
                if (isAutoGeneratedColumn)
                    continue;

                var value = property.GetValue(record, null);
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        sb.Append("'");
                        sb.Append(value);
                        sb.Append("'");
                        break;

                    case TypeCode.DateTime:
                        sb.Append("'");
                        sb.Append(value);
                        sb.Append("'");
                        break;

                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
                    default:
                        sb.Append(value);
                        break;
                }
                if (index != properties.Length - 1)
                    sb.Append(", ");
            }
            sb.Append(" ) ");

            return sb.ToString();
        }

        public class SQLServerExpressionVisitor : ExpressionVisitor
        {
            private StringBuilder _queryBuilder;

            public string WhereClause => _queryBuilder?.ToString();
            public string OrderBy { get; set; }
            public int? Take { get; set; }
            public int? Skip { get; set; }

            public SQLServerExpressionVisitor()
            {
                _queryBuilder = new StringBuilder();
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
                {
                    this.Visit(m.Arguments[0]);
                    LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    this.Visit(lambda.Body);
                    return m;
                }
                else if (m.Method.Name == "Take")
                {
                    if (this.ParseTakeExpression(m))
                    {
                        Expression nextExpression = m.Arguments[0];
                        return this.Visit(nextExpression);
                    }
                }
                else if (m.Method.Name == "Skip")
                {
                    if (this.ParseSkipExpression(m))
                    {
                        Expression nextExpression = m.Arguments[0];
                        return this.Visit(nextExpression);
                    }
                }
                else if (m.Method.Name == "OrderBy")
                {
                    if (this.ParseOrderByExpression(m, "ASC"))
                    {
                        Expression nextExpression = m.Arguments[0];
                        return this.Visit(nextExpression);
                    }
                }
                else if (m.Method.Name == "OrderByDescending")
                {
                    if (this.ParseOrderByExpression(m, "DESC"))
                    {
                        Expression nextExpression = m.Arguments[0];
                        return this.Visit(nextExpression);
                    }
                }

                throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
            }

            protected override Expression VisitUnary(UnaryExpression u)
            {
                switch (u.NodeType)
                {
                    case ExpressionType.Not:
                        _queryBuilder.Append(" NOT ");
                        this.Visit(u.Operand);
                        break;
                    case ExpressionType.Convert:
                        this.Visit(u.Operand);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
                }
                return u;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            protected override Expression VisitBinary(BinaryExpression b)
            {
                _queryBuilder.Append("(");
                this.Visit(b.Left);

                switch (b.NodeType)
                {
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        _queryBuilder.Append(" AND ");
                        break;

                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        _queryBuilder.Append(" OR ");
                        break;

                    case ExpressionType.Equal:
                        if (IsNullConstant(b.Right))
                            _queryBuilder.Append(" IS ");
                        else
                            _queryBuilder.Append(" = ");
                        break;

                    case ExpressionType.NotEqual:
                        if (IsNullConstant(b.Right))
                            _queryBuilder.Append(" IS NOT ");
                        else
                            _queryBuilder.Append(" <> ");
                        break;

                    case ExpressionType.LessThan:
                        _queryBuilder.Append(" < ");
                        break;

                    case ExpressionType.LessThanOrEqual:
                        _queryBuilder.Append(" <= ");
                        break;

                    case ExpressionType.GreaterThan:
                        _queryBuilder.Append(" > ");
                        break;

                    case ExpressionType.GreaterThanOrEqual:
                        _queryBuilder.Append(" >= ");
                        break;

                    default:
                        throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

                }

                this.Visit(b.Right);
                _queryBuilder.Append(")");
                return b;
            }

            protected override Expression VisitConstant(ConstantExpression c)
            {
                IQueryable q = c.Value as IQueryable;

                if (q == null && c.Value == null)
                {
                    _queryBuilder.Append("NULL");
                }
                else if (q == null)
                {
                    switch (Type.GetTypeCode(c.Value.GetType()))
                    {
                        case TypeCode.Boolean:
                            _queryBuilder.Append(((bool)c.Value) ? 1 : 0);
                            break;

                        case TypeCode.String:
                            _queryBuilder.Append("'");
                            _queryBuilder.Append(c.Value);
                            _queryBuilder.Append("'");
                            break;

                        case TypeCode.DateTime:
                            _queryBuilder.Append("'");
                            _queryBuilder.Append(c.Value);
                            _queryBuilder.Append("'");
                            break;

                        case TypeCode.Object:
                            throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                        default:
                            _queryBuilder.Append(c.Value);
                            break;
                    }
                }
                return c;
            }

            protected override Expression VisitMember(MemberExpression m)
            {
                if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
                {
                    _queryBuilder.Append(m.Member.Name);
                    return m;
                }
                else if (m.Expression is ConstantExpression)
                {
                    object container = ((ConstantExpression)m.Expression).Value;
                    var member = m.Member;
                    if (member is FieldInfo)
                    {
                        object value = ((FieldInfo)member).GetValue(container);
                        this.VisitConstant(Expression.Constant(value));
                        return Expression.Constant(value);
                    }
                    if (member is PropertyInfo)
                    {
                        object value = ((PropertyInfo)member).GetValue(container, null);
                        this.VisitConstant(Expression.Constant(value));
                        return Expression.Constant(value);
                    }
                }
                throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
            }

            private bool ParseOrderByExpression(MethodCallExpression expression, string order)
            {
                UnaryExpression unary = (UnaryExpression)expression.Arguments[1];
                LambdaExpression lambdaExpression = (LambdaExpression)unary.Operand;

                lambdaExpression = (LambdaExpression)SQLServerEvaluator.PartialEval(lambdaExpression);

                MemberExpression body = lambdaExpression.Body as MemberExpression;
                if (body != null)
                {
                    if (string.IsNullOrEmpty(OrderBy))
                        OrderBy = string.Format("{0} {1}", body.Member.Name, order);
                    else
                        OrderBy = string.Format("{0}, {1} {2}", OrderBy, body.Member.Name, order);
                    return true;
                }
                return false;
            }

            private bool ParseTakeExpression(MethodCallExpression expression)
            {
                ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

                int size;
                if (int.TryParse(sizeExpression.Value.ToString(), out size))
                {
                    Take = size;
                    return true;
                }

                return false;
            }

            private bool ParseSkipExpression(MethodCallExpression expression)
            {
                ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

                int size;
                if (int.TryParse(sizeExpression.Value.ToString(), out size))
                {
                    Skip = size;
                    return true;
                }
                return false;
            }

            private static Expression StripQuotes(Expression e)
            {
                while (e.NodeType == ExpressionType.Quote)
                {
                    e = ((UnaryExpression)e).Operand;
                }
                return e;
            }

            protected bool IsNullConstant(Expression exp)
            {
                return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
            }
        }
    }
}
