using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qckdev.Linq;

namespace qckdevTest.Linq
{

    [TestClass]
    public class ExpressionStringTest
    {

        [TestMethod]
        [DataRow("4", 4)]
        [DataRow("4+4", 8)]
        [DataRow("4 + 4", 8)]
        [DataRow("(4)", 4)]
        [DataRow("(4+4)", 8)]
        [DataRow("(4 + 4)", 8)]
        [DataRow("((4))", 4)]
        [DataRow("((4+4))", 8)]
        [DataRow("((4 + 4))", 8)]
        [DataRow("( (4 + 4) )", 8)]
        [DataRow("( ( 4 + 4 ) )", 8)]
        public void SimpleArithmeticTest1(string predicate, int expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("4 * 2", 8)]
        [DataRow("4 * 2 + 3", 11)]
        [DataRow("(4 * 2) + 3", 11)]
        [DataRow("4 * (2 + 3)", 20)]
        // TODO: no coge bien el orden de los operadores.
        //[DataRow("3 + 2 * 4", 11)]
        public void SimpleArithmeticTest2(string predicate, int expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("3 > 2", true)]
        [DataRow("0 < 2", true)]
        [DataRow("0 == 0", true)]
        [DataRow("0 != 1", true)]
        [DataRow("0 <> 1", true)]
        [DataRow("1 >= 1", true)]
        [DataRow("1 >= 2", false)]
        [DataRow("1 >= 0", true)]
        [DataRow("1 => 0", true)]
        [DataRow("1 <= 1", true)]
        [DataRow("1 <= 2", true)]
        [DataRow("1 <= 0", false)]
        [DataRow("1 =< 2", true)]
        [DataRow("3>2", true)]
        [DataRow("1>=2", false)]
        [DataRow("1=<2", true)]
        public void SimpleArithmeticTest3(string predicate, bool expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("'Texto' == 'Texto'", true)]
        [DataRow("'Texto' != 'Texta'", true)]
        [DataRow("'Texto' <> 'Texta'", true)]
        [DataRow("'Texto' == 'Tex*'", false)]
        // TODO: implementar.
        //[DataRow("'Texto' = 'Tex*'", true)]
        //[DataRow("'Texto' LIKE 'Tex*'", true)]
        public void SimpleArithmeticTest4(string predicate, bool expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        // TODO: implementar.
        //[TestMethod]
        //[DataRow("0 IN (1, 2, 3, 0)", true)]
        public void SimpleArithmeticTest5(string predicate, bool expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("[Value]*[Factor]", 20)]
        public void SimpleArithmeticTest6(string predicate, int result)
        {
            var item = new { Value = 5, Factor = 4 };

            SimpleArithmeticTestCore(predicate, item, result);
        }

        [TestMethod]
        [DataRow("[Value]=='Patata'", "Patata", true)]
        [DataRow("[Value]=='Patata(s)'", "Patata(s)", true)]
        [DataRow(@"[Value]=='Patata'''", "Patata'", true)]
        [DataRow(@"[Value]=='Patata\''", "Patata'", true)]
        [DataRow("[Value]=='Patata\"'", "Patata\"", true)]
        [DataRow(@"[Value]=='Patata""'", "Patata\"", true)]
        public void SimpleArithmeticTest7(string predicate, string value, bool result)
        {
            var item = new { Value = value };

            SimpleArithmeticTestCore(predicate, item, result);
        }


        private void SimpleArithmeticTestCore<TResult>(string predicate, TResult expected)
        {
            var expressionTree = qckdev.Linq.Expressions.ExpressionString.BuildTree(predicate);
            var expression = qckdev.Linq.Expressions.ExpressionBuilder<object, TResult>.Create(expressionTree);
            var lambda = expression.Compile();

            Assert.AreEqual(expected, lambda.Invoke(null));
        }

        private void SimpleArithmeticTestCore<T, TResult>(string predicate, T parameter, TResult expected)
        {
            var expressionTree = qckdev.Linq.Expressions.ExpressionString.BuildTree(predicate);
            var expression = qckdev.Linq.Expressions.ExpressionBuilder<T, TResult>.Create(expressionTree);
            var lambda = expression.Compile();

            Assert.AreEqual(expected, lambda.Invoke(parameter));
        }

    }
}