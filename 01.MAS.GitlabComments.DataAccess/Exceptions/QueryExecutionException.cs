namespace MAS.GitlabComments.DataAccess.Exceptions
{
    using System;

    /// <summary>
    /// State of query execution (when)
    /// </summary>
    public enum QueryExecutionExceptionState
    {
        /// <summary>
        /// Default value
        /// <para>Must not be used</para>
        /// </summary>
        None,

        /// <summary>
        /// Before execution of query
        /// </summary>
        Before,

        /// <summary>
        /// During execution of query
        /// <para>
        ///     Mostly likely caused by database schema rule checks
        /// </para>
        /// </summary>
        During,

        /// <summary>
        /// After execution of query
        /// </summary>
        After
    }

    /// <summary>
    /// Exception caused during SQL query execution
    /// </summary>
    [Serializable]
    public class QueryExecutionException : Exception
    {
        /// <summary>
        /// State when exception raised
        /// </summary>
        public QueryExecutionExceptionState State { get; }

        /// <summary>
        /// Initialization of <see cref="QueryExecutionException"/>
        /// </summary>
        /// <param name="state">State of execution query</param>
        public QueryExecutionException(QueryExecutionExceptionState state)
            : base()
        {
            State = state;
        }

        /// <summary>
        /// Initialization of <see cref="QueryExecutionException"/>
        /// </summary>
        /// <param name="state">State of execution query</param>
        /// <param name="message">Explanation message</param>
        public QueryExecutionException(QueryExecutionExceptionState state, string message)
            : base(message)
        {
            State = state;
        }
    }
}
