using sdORM.Session;

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
    }
}