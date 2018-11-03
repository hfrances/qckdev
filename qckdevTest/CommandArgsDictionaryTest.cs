using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using qckdev;

namespace qckdevTest
{

    [TestClass]
    public class CommandArgsDictionaryTest
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CommandArgsNullParameter()
        {
            qckdev.CommandArgsDictionary.Create(null);
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CommandArgsEmptyParameter()
        {
            var args = new string[] { };
            var dicArgs = qckdev.CommandArgsDictionary.Create(args);

            Assert.IsNotNull(dicArgs);
        }

        [TestMethod]
        public void CommandArgsParameter()
        {
            var args = new string[] { "Parametro1", "/Parametro2:valor2", "/Parametro3", "/Parametro4:\"tiene espacios\"", "\"" };
            var dicArgs = qckdev.CommandArgsDictionary.Create(false, args);
            var rdo = (dicArgs.Count > 0);
            var idx = 0;
            var etor = dicArgs.GetEnumerator();
            
            while (etor.MoveNext() && rdo )
            {
                var pair = etor.Current;

                switch (idx)
                {
                    case 0:
                        rdo = (pair.Key == "0" && pair.Value == "Parametro1");
                        break;
                    case 1:
                        rdo = (pair.Key == "Parametro2" && pair.Value == "valor2");
                        break;
                    case 2:
                        rdo = (pair.Key == "Parametro3" && pair.Value == string.Empty);
                        break;
                    case 3:
                        rdo = (pair.Key == "Parametro4" && pair.Value == "tiene espacios");
                        break;
                    case 4:
                        rdo = (pair.Key == "4" && pair.Value == "\"");
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
                idx++;
            }
            Assert.IsTrue(rdo);
        }

    }
}
