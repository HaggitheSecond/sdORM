using sdORM.Session.Interfaces;

namespace sdORM.Extensions
{
    public static class DatabaseSessionFactoryExtensions
    {
        public static IDatabaseSession WithTransaction(this IDatabaseSession session)
        {
            session.AddTransaction();
            return session;
        }

        public static IDatabaseSessionAsync WithTransaction(this IDatabaseSessionAsync session)
        {
            session.AddTransaction();
            return session;
        }

        public static IRawDatabaseSession WithTransaction(this IRawDatabaseSession session)
        {
            session.AddTransaction();
            return session;
        }

        public static IRawDatabaseSessionAsync WithTransaction(this IRawDatabaseSessionAsync session)
        {
            session.AddTransaction();
            return session;
        }
    }
}