using Microsoft.VisualStudio.TestTools.UnitTesting;
using qckdev.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace qckdevTest.Linq
{

    [TestClass]
    public class ExpressionTest
    {

        [TestMethod]
        public void ReplaceParameterTest_MethodCallExpression_Constant()
        {
            var expression = CreateExpression<string, bool>(x => x.Contains("mucha"));
            var rdo =
                GetDemoQuery()
                    .Any(expression);

            Assert.AreEqual(true, rdo);
        }

        [TestMethod]
        public void ReplaceParameterTest_MethodCallExpression_Variable()
        {
            var value = "mucha";
            var expression = CreateExpression<string, bool>(x => x.Contains(value));
            var rdo =
                GetDemoQuery()
                    .Any(expression);

            Assert.AreEqual(true, rdo);
        }

        [TestMethod]
        [DataRow("mucha", true)]
        [DataRow("muchas", true)]
        [DataRow("pocas", false)]
        public void ReplaceParameterTest_MethodCallExpression_Parameter(string value, bool expected)
        {
            var expression = CreateExpression<string, bool>(x => x.Contains(value));
            var rdo =
                GetDemoQuery()
                    .Any(expression);

            Assert.AreEqual(expected, rdo);
        }

        [TestMethod]
        public void ReplaceParameterTest_BinaryExpression()
        {
            var expression = CreateExpression<string, bool>(x => x == "mucha");
            var rdo =
                GetDemoQuery()
                    .Any(expression);

            Assert.AreEqual(false, rdo);
        }

        [TestMethod]
        public void ReplaceParameterTest_MemberExpression()
        {
            var expression = CreateExpression<string, string>(x => x.ToLower());
            var expected = string.Join(",",
                GetDemoQuery()
                    .Select(x => x.ToLower()));
            var rdo = string.Join(",",
                GetDemoQuery()
                    .Select(expression));

            Assert.AreEqual(expected, rdo);
        }

        [TestMethod]
        public void ReplaceParameterTest_PropertyExpression()
        {
            var expression = CreateExpression<string, int>(x => x.Length);
            var expected =
                GetDemoQuery()
                    .Select(x => x.Length)
                    .Sum();
            var rdo =
                GetDemoQuery()
                    .Select(expression)
                    .Sum();

            Assert.AreEqual(expected, rdo);
        }

        [TestMethod]
        public void ReplaceParameterTest_Mix()
        {
            Expression<Func<string, int>> expression =
                x => string.Join("", x.AsEnumerable().Except("a")).Replace("e", "").Length;
            var expected =
                GetDemoQuery()
                    .Select(expression)
                    .Sum();
            var rdo =
                GetDemoQuery()
                    .Select(CreateExpression(expression))
                    .Sum();

            Assert.AreEqual(expected, rdo);
        }

        private static Expression<Func<T, TResult>> CreateExpression<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return (Expression<Func<T, TResult>>)expression.ReplaceParameter(
                Expression.Parameter(typeof(T), "item"));
        }

        private static IQueryable<string> GetDemoQuery()
        {
            var items = new List<string>()
            {
                "hola", "esto", "es", "una", "prueba", "de", "muchas"
            };

            return items.AsQueryable();
        }

    }
}
