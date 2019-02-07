using System;

namespace sdORM.Session.Exceptions
{
    [Serializable]
    public class DatabaseSessionFactoryNotInitializedException : Exception
    {
        public DatabaseSessionFactoryNotInitializedException(Type databaseSessionFactoryType)
            : base($"Your {databaseSessionFactoryType} has not been initialized. Please call Initialize() or InitializeAsync() before creating sessions.")
        {
            
        }
    }
}