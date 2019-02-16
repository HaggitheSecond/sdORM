using System;
using System.Runtime.Serialization;
using sdORM.Exceptions;

namespace sdORM.Session.Exceptions
{
    [Serializable]
    public class DatabaseSessionFactoryNotInitializedException : sdOrmException
    {
        public DatabaseSessionFactoryNotInitializedException(Type databaseSessionFactoryType)
            : base($"Your {databaseSessionFactoryType} has not been initialized. Please call Initialize() or InitializeAsync() before creating sessions.")
        {

        }

        protected DatabaseSessionFactoryNotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}