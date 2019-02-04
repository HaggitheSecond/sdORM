using System;

namespace sdORM.Session.Exceptions
{
    [Serializable]
    public class DatabaseSessionFactoryNotInitializedException : Exception
    {
        public DatabaseSessionFactoryNotInitializedException(Type DatabaseSessionFactoryType)
            : base($"Your {DatabaseSessionFactoryType} has not been initialized. Please call Initialize() or InitializeAsync() before creating sessions.")
        {
            
        }
    }
}