namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }

        void CreateTransaction();
        void Commit();
        void Rollback();
    }
}
