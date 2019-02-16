using System;
using System.Runtime.Serialization;

namespace sdORM.Exceptions
{
    [Serializable]
    public class sdOrmException : Exception
    {
        public sdOrmException()
        {
        }

        protected sdOrmException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public sdOrmException(string message) : base(message)
        {
        }

        public sdOrmException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}