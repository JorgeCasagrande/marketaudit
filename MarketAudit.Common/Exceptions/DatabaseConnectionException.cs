using System;
using System.Runtime.Serialization;

namespace MarketAudit.Common.Exceptions
{
    [Serializable]
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException()
        {
        }

        public DatabaseConnectionException(string message) : base(message)
        {
        }

        public DatabaseConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DatabaseConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
