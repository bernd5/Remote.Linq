﻿namespace Remote.Linq.Tests.Serialization.VariableQueryArgument
{
    using Remote.Linq.Expressions;
    using System;
    using Xunit;

    public class When_using_local_variable_query_argument
    {
        private class AType
        {
            public int Number { get; set; }
        }

        private LambdaExpression _remoteExpression;

        private LambdaExpression _serializedRemoteExpression;

        public When_using_local_variable_query_argument()
        {
            var value = 123;

            System.Linq.Expressions.Expression<Func<AType, bool>> expression = x => x.Number == value;

            _remoteExpression = expression.ToRemoteLinqExpression();

            // HINT: since this test is used in multiple assemblies as linked file, 
            //       use serialize extension method to find out the context 
            _serializedRemoteExpression = _remoteExpression.Serialize();
        }

        [Fact]
        public void Remote_expression_should_be_equal()
        {
            var str1 = _remoteExpression.ToString();
            var str2 = _serializedRemoteExpression.ToString();

            _remoteExpression.EqualsRemoteExpression(_serializedRemoteExpression);
        }

        [Fact]
        public void System_expresison_should_be_equal()
        {
            var exp1 = _remoteExpression.ToLinqExpression<AType, bool>();
            var exp2 = _serializedRemoteExpression.ToLinqExpression<AType, bool>();

            exp1.EqualsExpression(exp2);
        }
    }
}