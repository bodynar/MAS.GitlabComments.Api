namespace MAS.GitlabComments.WebApi.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Model representing service call result
    /// </summary>
    [DataContract]
    public class BaseServiceResult
    {
        /// <summary>
        /// Flag determines that operation performed successful
        /// </summary>
        [DataMember(Name = "success")]
        public bool IsSuccess { get; protected set; }

        /// <summary>
        /// Operation perform error message
        /// </summary>
        [DataMember(Name = "error")]
        public string ErrorMessage { get; protected set; } = string.Empty;

        protected BaseServiceResult() { }

        /// <summary>
        /// Create model that represents successful operation
        /// </summary>
        /// <returns>Model representing service call result</returns>
        public static BaseServiceResult Success()
            => new() { IsSuccess = true };

        /// <summary>
        /// Create model that represents not successful operation by passing catch exception
        /// </summary>
        /// <param name="exception">Exception catch during operation perform</param>
        /// <returns>Model representing service call result</returns>
        public static BaseServiceResult Error(Exception exception)
            => new() { IsSuccess = false, ErrorMessage = exception.Message };

        /// <summary>
        /// Create model that represents not successful operation by passing error message
        /// </summary>
        /// <param name="error">Error message specifying reasons for stop operation perform</param>
        /// <returns>Model representing service call result</returns>
        public static BaseServiceResult Error(string error)
            => new() { IsSuccess = false, ErrorMessage = error };
    }

    /// <summary>
    /// Model representing service call result
    /// </summary>
    /// <typeparam name="TResult">Type of result model</typeparam>
    [DataContract]
    public sealed class BaseServiceResult<TResult>: BaseServiceResult
    {
        /// <summary>
        /// Result of service operation
        /// </summary>
        [DataMember(Name = "result")]
        public TResult Result { get; private set; }

        private BaseServiceResult() { }

        /// <summary>
        /// Create model that represents successful operation with result
        /// </summary>
        /// <param name="result">Result of performed operation</param>
        /// <returns>Model representing service call result</returns>
        public static BaseServiceResult<TResult> Success(TResult result)
            => new() { IsSuccess = true, Result = result };

        /// <summary>
        /// Create model that represents not successful operation by passing catch exception
        /// </summary>
        /// <param name="exception">Exception catch during operation perform</param>
        /// <returns>Model representing service call result</returns>
        public static new BaseServiceResult<TResult> Error(Exception exception)
            => new() { IsSuccess = false, ErrorMessage = exception.Message };

        /// <summary>
        /// Create model that represents not successful operation by passing error message
        /// </summary>
        /// <param name="error">Error message specifying reasons for stop operation perform</param>
        /// <returns>Model representing service call result</returns>
        public static new BaseServiceResult<TResult> Error(string error)
            => new() { IsSuccess = false, ErrorMessage = error };
    }
}
