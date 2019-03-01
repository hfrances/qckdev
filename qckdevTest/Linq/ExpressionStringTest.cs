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
        // TODO: negatives.
        //[DataRow("-1", -1)]
        //[DataRow("5*-1", -5)]
        public void SimpleArithmeticTest1(string predicate, int expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        /// <remarks>
        /// https://www.calculatorsoup.com/calculators/math/math-equation-solver.php
        /// </remarks>
        [TestMethod]
        [DataRow("4 * 2", 8)]
        [DataRow("4 * 2 + 3", 11)]
        [DataRow("(4 * 2) + 3", 11)]
        [DataRow("4 * (2 + 3)", 20)]
        [DataRow("4^1*(2+3)", 20)]
        [DataRow("(4)^1*(2+3)", 20)]
        [DataRow("(2+2)^1*(2+3)", 20)]
        [DataRow("6/2", 3)]
        [DataRow("(3+3)/2", 3)]
        [DataRow("3 + 2 * 4", 11)]
        [DataRow("3 + 2 * 4 * 2", 19)]
        [DataRow("3 + 2 * 4 * 2 + 3", 22)]
        [DataRow("2^16", 65536)]
        [DataRow("2^16 + 3", 65539)]
        [DataRow("3^2*5", 45)]
        [DataRow("3 + 2^16 + 3", 65542)]
        [DataRow("3 + 2 * 4 * 2^16 + 3", 524294)]
        [DataRow("3 + 2 * 4 * 2^16 + 3^2", 524300)]
        [DataRow("3 + 2 * 4 * 2^16 + 3^2*5", 524336)]
        [DataRow("3 + (2 * 4 * 2^16) + ((3^2)*5)", 524336)]
        [DataRow("(4^4)+4", 260)]
        [DataRow("(3+2)^2*(5^3)", 3125)]
        [DataRow("(3+2)^2*(5^3)^2", 390625)]
        [DataRow("(3+2)^2*(5^3)^2*(5^0)*(5^4)", 244140625)]
        [DataRow("(10+5^2)*((5*2)+9+3^3)/2", 805)]
        [DataRow("(10+5^2)*((5*2)+9-3^3)/2", -140)]
        public void SimpleArithmeticTest2a(string predicate, int expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("(4^4)^4", 4294967296)]
        public void SimpleArithmeticTest2b(string predicate, long expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("2*(6+7)-8^2", -38)] // Fails.
        // TODO: negatives.
        //[DataRow("(10+5^2)*((5*-2)+9-3^3)/2", -490)]
        public void SimpleArithmeticTest2c(string predicate, int expected)
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


        private void SimpleArithmeticTestCore<T, TResult>(string predicate, T parameter, TResult expected)
        {
            var expressionTree = qckdev.Linq.Expressions.ExpressionString.BuildTree(predicate);
            var expression = qckdev.Linq.Expressions.ExpressionBuilder<T, TResult>.Create(expressionTree);
            var lambda = expression.Compile();

            Assert.AreEqual(expected, lambda.Invoke(parameter));
        }

        private void SimpleArithmeticTestCore<TResult>(string predicate, TResult expected)
        {
            SimpleArithmeticTestCore<object, TResult>(predicate, null, expected);
        }

    }
}