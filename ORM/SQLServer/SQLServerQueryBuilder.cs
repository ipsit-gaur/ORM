﻿using ORM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ORM.SQLServer
{
    // TODO: Should be in ORM.SQLServer Assembly
    internal class SQLServerQueryBuilder : ExpressionVisitor, IQueryBuilder
    {
        private StringBuilder _queryBuilder;
        internal int? Skip { get; private set; }
        internal int? Take { get; private set; }
        internal string OrderBy { get; private set; } = string.Empty;
        internal string WhereClause { get; private set; }

        public SQLServerQueryBuilder()
        {
        }

        public string Translate(Expression expression)
        {
            this._queryBuilder = new StringBuilder();
            this.Visit(expression);
            WhereClause = this._queryBuilder.ToString();
            return WhereClause;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
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
                    _queryBuilder.Append(" AND ");
                    break;

                case ExpressionType.AndAlso:
                    _queryBuilder.Append(" AND ");
                    break;

                case ExpressionType.Or:
                    _queryBuilder.Append(" OR ");
                    break;

                case ExpressionType.OrElse:
                    _queryBuilder.Append(" OR ");
                    break;

                case ExpressionType.Equal:
                    if (IsNullConstant(b.Right))
                    {
                        _queryBuilder.Append(" IS ");
                    }
                    else
                    {
                        _queryBuilder.Append(" = ");
                    }
                    break;

                case ExpressionType.NotEqual:
                    if (IsNullConstant(b.Right))
                    {
                        _queryBuilder.Append(" IS NOT ");
                    }
                    else
                    {
                        _queryBuilder.Append(" <> ");
                    }
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
                        var property = c.Value.GetType().GetProperty("testID");
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
            else if (m.Expression != null && m.Expression.NodeType == ExpressionType.Constant)
            {
                this.VisitConstant((m.Expression as ConstantExpression));
                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        protected bool IsNullConstant(Expression exp)
        {
            return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
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
                {
                    OrderBy = string.Format("{0} {1}", body.Member.Name, order);
                }
                else
                {
                    OrderBy = string.Format("{0}, {1} {2}", OrderBy, body.Member.Name, order);
                }

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

        private string GetQuery<T>(Expression<Func<T, bool>> predicate) where T : DbEntity
        {
            if (predicate == null)
                throw new ArgumentException("Predicate cannot be null");
            return Translate(predicate);
        }

        public string GetQuery<T>(List<Expression<Func<T, bool>>> predicates) where T : DbEntity
        {
            var sb = new StringBuilder();
            sb.Append(SQLServerKeywords.SELECT);
            sb.Append(SQLServerKeywords.ALL);
            sb.Append(SQLServerKeywords.FROM);
            sb.Append(typeof(T).Name);
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
    }
}