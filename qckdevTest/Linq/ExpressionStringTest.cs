using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace qckdevTest.Linq
{

    [SuppressMessage("Critical Code Smell", "S2699:Tests should include assertions", Justification = "Assertion is in sub-methods.")]
    [TestClass]
    public class ExpressionStringTest
    {

        [TestMethod]
        [DataRow("4", 4)]
        [DataRow("4+4", 8)]
        [DataRow("4 + 4", 8)]
        [DataRow("4 - 4", 0)]
        [DataRow("(4)", 4)]
        [DataRow("(4+4)", 8)]
        [DataRow("(4 + 4)", 8)]
        [DataRow("((4))", 4)]
        [DataRow("((4+4))", 8)]
        [DataRow("((4 + 4))", 8)]
        [DataRow("( (4 + 4) )", 8)]
        [DataRow("( ( 4 + 4 ) )", 8)]
        [DataRow("-1", -1)]
        [DataRow("5*-1", -5)]
        [DataRow("(5*1)-1", 4)]
        [DataRow("(-1*5)-1", -6)]
        [DataRow("(-1 * 5) - 1", -6)]
        [DataRow("(-1*-5)-1", 4)]
        [DataRow("(-1 * -5) - 1", 4)]
        [DataRow("(-1*-5)-1-1", 3)]
        [DataRow("(-1*-5) - 1 - 1", 3)]
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
        [DataRow("(10+5^2)*((5*-2)+9-3^3)/2", -490)]
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
        [DataRow("true==true", true)]
        [DataRow("false==false", true)]
        [DataRow("true!=false", true)]
        [DataRow("true==false", false)]
        public void ExpressionStringTest0005a_Comparison_Int_Simple(string predicate, bool expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("(1=<2) == true", true)]
        public void ExpressionStringTest0005b_Comparison_Int_Complex(string predicate, bool expected)
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
        [DataRow("'Texto' == 'texto'", true)]
        public void ExpressionStringTest0007a_Comparison_String_Simple(string predicate, bool expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("'Texto' = 'Tex*'", true)]
        [DataRow("'Texto' LIKE 'Tex*'", true)]
        [DataRow("'Texto' = 'texto'", true)]
        [DataRow("'Texto falso' = 'Texto'", false)]
        public void ExpressionStringTest0007b_Comparison_String_Like(string predicate, bool expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("[Value2] IN (1, 2, 3, 11, 0)", true)]
        [DataRow("[Value1] == 1 AND [Value2] IN (1 , 2 ,3 , 11, 0)", true)]
        [DataRow("[Value1]==1 AND [Value2] IN (1,2,3,11,0) OR [Value3]==false", true)]
        public void ExpressionStringTest0008_Comparison_Int_InClause(string predicate, bool expected)
        {
            var item = new { Value1 = 1, Value2 = 0, Value3 = true };

            SimpleArithmeticTestCore(predicate, item, expected);
        }

        [TestMethod]
        [DataRow("1 IN ([Value1], [Value2])", true)]
        [DataRow("[Value3] IN ([Value1], [Value2])", true)]
        public void ExpressionStringTest0008_Comparison_Int_InClause_Reverse(string predicate, bool expected)
        {
            var item = new { Value1 = 1, Value2 = 0, Value3 = 1 };

            SimpleArithmeticTestCore(predicate, item, expected);
        }

        [TestMethod]
        [DataRow("[Value]*[Factor]", 20)]
        public void ExpressionStringTest0009_Arithmetic_WithItem(string predicate, int expected)
        {
            var item = new { Value = 5, Factor = 4 };

            SimpleArithmeticTestCore(predicate, item, expected);
        }

        [TestMethod]
        [DataRow("[Value]=='Patata'", "Patata", true)]
        [DataRow("[Value]=='Patata(s)'", "Patata(s)", true)]
        [DataRow("Value=='Patata'", "Patata", true)] // Test without property brakets.
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
        [DataRow("[Value] IN (1, 2, 3")]
        [DataRow("[Value] IN (1, 2, 3 AND 1==1")]
        [DataRow("[Value] IN (1, 2, 3 && 1==1")]
        public void ExpressionStringTest0011_Comparison_FormatException(string predicate)
        {
            SimpleArithmeticTestCore(predicate, (string)null);
        }

        [TestMethod]
        [DataRow("1 == 1 AND 2 == 2", true)]
        [DataRow("1 == 1 && 2 == 2", true)]
        [DataRow("1 == 2 OR 2 == 2", true)]
        [DataRow("1 == 2 || 2 == 2", true)]
        [DataRow("1 == 1 OR 2 == 1", true)]
        [DataRow("1 == 1 || 2 == 1", true)]
        [DataRow("!false==false", false)]
        [DataRow("not false==false", false)]
        [DataRow("!(false==false)", false)]
        [DataRow("not (false==false)", false)]
        [DataRow("!true==false", true)]
        [DataRow("!(true==false)", true)]
        [DataRow("not (true==false)", true)]
        public void ExpressionStringTest0012a_Logical_Simple(string predicate, bool expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [SuppressMessage("Critical Code Smell", "S1764:Identical expressions should not be used on both sides of a binary operator")]
        [DataRow("1==1 || 1==0 && 1==1", 1 == 1 || 1 == 0 && 1 == 1)]
        [DataRow("1==0 || 1==1 && 1==1", 1 == 0 || 1 == 1 && 1 == 1)]
        [DataRow("1==1 && 1==1 || 1==0", 1 == 1 && 1 == 1 || 1 == 0)]
        [DataRow("1==0 && 1==1 || 1==0", 1 == 0 && 1 == 1 || 1 == 0)]
        [DataRow("1==0 && 1==1 || 1==1", 1 == 0 && 1 == 1 || 1 == 1)]
        [DataRow("1==0 && 1==1 || 1==1", 1 == 0 && 1 == 1 || 1 == 1)]
        public void ExpressionStringTest0012b_Logical_Priorized(string predicate, bool expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        [TestMethod]
        [DataRow("(-1*-5)-1 == 4 AND -1==-1", true)]
        public void ExpressionStringTest0012c_Logical_Advanced(string predicate, bool expected)
        {
            SimpleArithmeticTestCore(predicate, expected);
        }

        /// <remarks>
        /// <seealso href="https://stackoverflow.com/questions/1241142/sql-logic-operator-precedence-and-and-or"/>
        /// </remarks>
        [TestMethod]
        [DataRow("[x]==1 OR [y]==1 And [z]==1", true)]
        [DataRow("[x]==1 OR ([y]==1 And [z]==1)", true)]
        [DataRow("([x]==1 OR [y]==1) And [z]==1", false)]
        [DataRow("([x]==1 OR [y]==1) And ![z]==1", true)]
        [DataRow("([x]==1 OR [y]==1) And !([z]==1)", true)]
        [DataRow("([x]==1 OR [y]==1) And (![z]==1)", true)]
        public void ExpressionStringTest0013_Logical_WithItem(string predicate, bool expected)
        {
            var item = new { x = 1, y = 0, z = 0 };

            SimpleArithmeticTestCore(predicate, item, expected);
        }

        [TestMethod]
        [DataRow("[date1]>=#2020/09/25#", 3)]
        [DataRow("[date1]>=#09/25/2020#", 3)]
        [DataRow("[date1]<=#2020/09/25#", 1)]
        [DataRow("[date1]<=#09/25/2020#", 1)]
        [DataRow("[date1]==#2020/09/25#", 1)]
        [DataRow("[date1]==#09/25/2020#", 1)]
        //TODO: [DataRow("[date1]='26/09/2020'", 1)]
        //TODO: [DataRow("[date1]='26/09/2020*'", 2)]
        public void ExpressionStringTest0014_DateTime(string predicate, int expected)
        {
            var collection = new[]
            {
                new { date1 = new DateTime(2020, 09, 25), date2 = DateTime.Now },
                new { date1 = new DateTime(2020, 09, 26), date2 = DateTime.Now },
                new { date1 = new DateTime(2020, 09, 26, 12, 45, 01), date2 = DateTime.Now },
            };

            CollectionArithmeticTestCore(predicate, collection, expected);
        }

        #region common

        private void SimpleArithmeticTestCore<T, TResult>(string predicate, T parameter, TResult expected)
        {
            var expression = qckdev.Linq.Expressions.ExpressionBuilder<T, TResult>.Create(predicate);
            var lambda = expression.Compile();
            var actual = lambda.Invoke(parameter);

            Assert.AreEqual(expected, actual, $"\n{expression}");
        }

        private void SimpleArithmeticTestCore<TResult>(string predicate, TResult expected)
        {
            SimpleArithmeticTestCore<object, TResult>(predicate, null, expected);
        }

        private void CollectionArithmeticTestCore<T>(string predicate, IEnumerable<T> collection, int expected)
        {
            var expression = qckdev.Linq.Expressions.ExpressionBuilder<T, bool>.Create(predicate);
            var lambda = expression.Compile();

            Assert.AreEqual(expected, collection.Count(lambda), $"\n{expression}");
        }

        #endregion

    }
}