namespace MAS.GitlabComments.DataAccess.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Additional test assertions
    /// </summary>
    public static class CommonAssert
    {
        /// <summary>
        /// Assert collection items
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="expectedCollection">Expected items</param>
        /// <param name="actualCollection">Actual items</param>
        /// <param name="assertFunction">Assert function</param>
        public static void CollectionsWithSameType<T>(IEnumerable<T> expectedCollection, IEnumerable<T> actualCollection, Action<T, T> assertFunction)
        {
            if (expectedCollection == null)
            {
                throw new ArgumentNullException(nameof(expectedCollection));
            }
            if (actualCollection == null)
            {
                throw new ArgumentNullException(nameof(actualCollection));
            }
            if (assertFunction == null)
            {
                throw new ArgumentNullException(nameof(assertFunction));
            }

            var expectedCollectionAsArray = expectedCollection.ToArray();
            var actualCollectionAsArray = actualCollection.ToArray();

            if (expectedCollectionAsArray.Length != actualCollectionAsArray.Length)
            {
                throw new ArgumentException("Collections must be same size");
            }

            for (var i = 0; i < expectedCollectionAsArray.Length; i++)
            {
                assertFunction(expectedCollectionAsArray[i], actualCollectionAsArray[i]);
            }
        }

        /// <summary>
        /// Assert collection items
        /// </summary>
        /// <param name="expectedCollection">Expected items</param>
        /// <param name="actualCollection">Actual items</param>
        /// <param name="assertFunction">Assert function</param>
        public static void Collections<TExpected, TActiual>(IEnumerable<TExpected> expectedCollection, IEnumerable<TActiual> actualCollection, Action<TExpected, TActiual> assertFunction)
        {
            if (expectedCollection == null)
            {
                throw new ArgumentNullException(nameof(expectedCollection));
            }
            if (actualCollection == null)
            {
                throw new ArgumentNullException(nameof(actualCollection));
            }
            if (assertFunction == null)
            {
                throw new ArgumentNullException(nameof(assertFunction));
            }

            var expectedCollectionAsArray = expectedCollection.ToArray();
            var actualCollectionAsArray = actualCollection.ToArray();

            if (expectedCollectionAsArray.Length != actualCollectionAsArray.Length)
            {
                throw new ArgumentException("Collections must be same size");
            }

            for (var i = 0; i < expectedCollectionAsArray.Length; i++)
            {
                assertFunction(expectedCollectionAsArray[i], actualCollectionAsArray[i]);
            }
        }
    }
}
