namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IAffiliateRepository Affiliate { get; }
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }

        void CreateTransaction();
        void Commit();
        void Rollback();
    }
}
