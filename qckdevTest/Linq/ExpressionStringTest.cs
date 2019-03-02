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
        public void ExpressionStringTest0001_Arithmetic_Simple(string predicate, int expected)
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
        public void ExpressionStringTest0002_Arithmetic_Advanced(string predicate, int expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("(4^4)^4", 4294967296)]
        public void ExpressionStringTest0003_Arithmetic_Long(string predicate, long expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("(42)", 42)]
        [DataRow("(4+2)", 6)]
        [DataRow("((4+2))", 6)]
        [DataRow("3+(4*2)", 11)]
        [DataRow("2*3+3^1", 9)]
        [DataRow("2*(6+7)-8^2", -38)]
        // TODO: negatives.
        //[DataRow("(10+5^2)*((5*-2)+9-3^3)/2", -490)]
        public void ExpressionStringTest0004_Arithmetic_Special(string predicate, int expected)
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
        public void ExpressionStringTest0005_Comparison_Int_Simple(string predicate, bool expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("'42'", "42")]
        public void ExpressionStringTest0006_Comparison_String_Return(string predicate, string expected)
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
        public void ExpressionStringTest0007_Comparison_String_Simple(string predicate, bool expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        // TODO: implementar.
        [TestMethod]
        [DataRow("0 IN (1, 2, 3, 0)", true)]
        public void ExpressionStringTest0008_Comparison_Int_InClause(string predicate, bool expected)
        {
            Assert.Inconclusive("Not implemented.");
            //SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("[Value]*[Factor]", 20)]
        public void ExpressionStringTest0009_Arithmetic_WithItem(string predicate, int result)
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
        [DataRow(@"[Value]=='(Patata)'", "(Patata)", true)]
        public void ExpressionStringTest0010_Comparison_String_WithItem(string predicate, string value, bool result)
        {
            var item = new { Value = value };

            SimpleArithmeticTestCore(predicate, item, result);
        }


        [TestMethod, ExpectedException(typeof(FormatException))]
        [DataRow("2*(6+7-8^2")]
        [DataRow("2*(6+7))-8^2")]
        [DataRow("[Value] = 'Pata")]
        [DataRow("[Value = 'Patata'")]
        [DataRow("Value] = 'Patata'")]
        public void ExpressionStringTest0011_Comparison_FormatException(string predicate)
        {
            SimpleArithmeticTestCore(predicate, (string)null);
        }

        private void SimpleArithmeticTestCore<T, TResult>(string predicate, T parameter, TResult expected)
        {
            var expressionTree = qckdev.Linq.Expressions.ExpressionString.BuildTree(predicate);
            var expression = qckdev.Linq.Expressions.ExpressionBuilder<T, TResult>.Create(expressionTree);
            var lambda = expression.Compile();
            var actual = lambda.Invoke(parameter);

            Assert.AreEqual(expected, actual, $"\n{expression}");
        }

        private void SimpleArithmeticTestCore<TResult>(string predicate, TResult expected)
        {
            SimpleArithmeticTestCore<object, TResult>(predicate, null, expected);
        }

    }
}