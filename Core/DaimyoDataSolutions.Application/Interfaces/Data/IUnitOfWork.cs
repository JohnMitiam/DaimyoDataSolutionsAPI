namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProductRepository Products { get; }

        void CreateTransaction();
        void Commit();
        void Rollback();
    }
}
