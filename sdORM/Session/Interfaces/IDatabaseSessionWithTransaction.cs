namespace sdORM.Session.Interfaces
{
    public interface IDatabaseSessionWithTransaction
    {
        /// <summary>
        /// Creates an transaction which will be applied to all non-reading operations and will be commited when the session is disposed.
        /// </summary>
        void AddTransaction();
    }
}