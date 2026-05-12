namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IAffiliateRepository Affiliate { get; }
        IProductRepository Products { get; }
        IProductCategoriesRepository ProductCategories { get; }
        ICategoryRepository Categories { get; }

        void CreateTransaction();
        void Commit();
        void Rollback();
        Task<int> SaveChangesAsync();
    }
}
