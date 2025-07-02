using System;
using System.Runtime.Serialization;

namespace MarketAudit.Common.Exceptions
{
    [Serializable]
    public class DatabaseQueryExecutionException : Exception
    {
        public DatabaseQueryExecutionException()
        {
        }

        public DatabaseQueryExecutionException(string message) : base(message)
        {
        }

        public DatabaseQueryExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DatabaseQueryExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
