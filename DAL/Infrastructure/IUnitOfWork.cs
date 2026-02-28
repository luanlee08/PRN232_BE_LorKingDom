namespace DAL.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        /// <summary>
        /// Flush tracked changes to the database without committing the transaction.
        /// Allows BLL to persist without holding a reference to DbContext.
        /// </summary>
        Task SaveChangesAsync();
    }
}
