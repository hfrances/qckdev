using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using qckdev;

namespace qckdevTest
{
    [TestClass]
    public class KeyTest
    {

        const int DICTIONARYCOUNT = 100000;

        /// <summary>
        /// Verificar que dos claves con los mismos valores se consideran idénticas.
        /// </summary>
        [TestMethod]
        public void TestMethod001()
        {
            var key1 = new Key("hola", 10, true);
            var key2 = new Key("hola", 10, true);

            Assert.AreEqual(key1, key2);
        }

        /// <summary>
        /// Verificar que dos claves con los mismos valores se consideran idénticas incluso si las cadenas difieren en mayúsculas / minúsuclas.
        /// </summary>
        [TestMethod]
        public void TestMethod002()
        {
            var key1 = new Key("hola", 10, true);
            var key2 = new Key("Hola", 10, true);

            Assert.AreEqual(key1, key2);
        }

        /// <summary>
        /// Verificar que dos claves con los mismos valores se consideran idénticas incluso si las cadenas difieren en mayúsculas / minúsuclas.
        /// Especificando parámetro <see cref="Key.IgnoreCase"/>.
        /// </summary>
        [TestMethod]
        public void TestMethod003()
        {
            var key1 = new Key(new object[] { "hola", 10, true }, ignoreCase: true);
            var key2 = new Key(new object[] { "Hola", 10, true }, ignoreCase: true);

            Assert.AreEqual(key1, key2);
        }

        /// <summary>
        /// Verificar que dos claves con los mismos valores se consideran idénticas diferenciando entre mayúsculas y minúsuclas.
        /// </summary>
        [TestMethod]
        public void TestMethod004()
        {
            var key1 = new Key(new object[] { "hola", 10, true }, ignoreCase: false);
            var key2 = new Key(new object[] { "Hola", 10, true }, ignoreCase: false);

            Assert.AreNotEqual(key1, key2);
        }

        /// <summary>
        /// Pruebas en diccionarios. Calcular el tiempo que cuesta rellenar y buscar en un diccionario <see cref="String"/>.
        /// Cantidad: 100.000 elementos.
        /// Valor de referencia para las pruebas con <see cref="Key"/>.
        /// </summary>
        [TestMethod]
        public void TestMethod005a()
        {
            var dictionary = new Dictionary<string, int>();
            var rdo = true;

            for (int i = 0; i < DICTIONARYCOUNT; i++)
                dictionary.Add(GetStringKey(i), i);

            for (int i = 0; i < DICTIONARYCOUNT && rdo; i += 5)
                rdo = dictionary.ContainsKey(GetStringKey(i));
        }

        /// <summary>
        /// Pruebas en diccionarios. Calcular el tiempo que cuesta rellenar y buscar en un diccionario <see cref="Key{String}"/>.
        /// <see cref="Key.IgnoreCase"/> = false.
        /// Cantidad: 100.000 elementos.
        /// </summary>
        [TestMethod]
        public void TestMethod005b()
        {
            var dictionary = new Dictionary<Key, int>();
            var rdo = true;

            for (int i = 0; i < DICTIONARYCOUNT; i++)
                dictionary.Add(new Key(GetStringKey(i)), i);

            for (int i = 0; i < DICTIONARYCOUNT && rdo; i += 5)
                rdo = dictionary.ContainsKey(new Key(new object[] { GetStringKey(i) }, ignoreCase: false));
        }

        /// <summary>
        /// Pruebas en diccionarios. Calcular el tiempo que cuesta rellenar y buscar en un diccionario <see cref="Key{String, Int32}"/>.
        /// <see cref="Key.IgnoreCase"/> = false.
        /// Cantidad: 100.000 elementos.
        /// </summary>
        [TestMethod]
        public void TestMethod005c()
        {
            var dictionary = new Dictionary<Key, int>();
            var rdo = true;

            for (int i = 0; i < DICTIONARYCOUNT; i++)
                dictionary.Add(new Key(GetStringKey(i), i), i);

            for (int i = 0; i < DICTIONARYCOUNT && rdo; i += 5)
                rdo = dictionary.ContainsKey(new Key(new object[] { GetStringKey(i), i }, ignoreCase: false));
        }

        /// <summary>
        /// Pruebas en diccionarios. Calcular el tiempo que cuesta rellenar y buscar en un diccionario <see cref="Key{String, Int32}"/>.
        /// <see cref="Key.IgnoreCase"/> = true.
        /// Cantidad: 100.000 elementos.
        /// </summary>
        [TestMethod]
        public void TestMethod005d()
        {
            var dictionary = new Dictionary<Key, int>();
            var rdo = true;

            for (int i = 0; i < DICTIONARYCOUNT; i++)
                dictionary.Add(new Key(GetStringKey(i), i), i);

            for (int i = 0; i < DICTIONARYCOUNT && rdo; i += 5)
                rdo = dictionary.ContainsKey(new Key(new object[] { GetStringKey(i), i }, ignoreCase: true));
        }


        #region helpers

        private static string GetStringKey(int i)
        {
            return string.Format("Prueba{0:0000}", i);
        }

        #endregion  

    }
}
