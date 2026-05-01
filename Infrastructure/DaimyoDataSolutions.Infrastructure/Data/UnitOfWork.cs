using DaimyoDataSolutions.Application.Interfaces.Data;
using System.Threading.Tasks;

namespace DaimyoDataSolutions.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseSession _dbSession;
        private readonly IAffiliateRepository _affiliateRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoriesRepository _productCategoriesRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UnitOfWork
            (
                DatabaseSession dbSession,
                IAffiliateRepository affiliateRepository,
                IProductRepository productRepository,
                IProductCategoriesRepository productCategoriesRepository,
                ICategoryRepository categoryRepository
            )
        {
            _dbSession = dbSession;
            _affiliateRepository = affiliateRepository;
            _productRepository = productRepository;
            _productCategoriesRepository = productCategoriesRepository;
            _categoryRepository = categoryRepository;
        }

        public IAffiliateRepository Affiliate => _affiliateRepository;
        public IProductRepository Products => _productRepository;
        public IProductCategoriesRepository ProductCategories => _productCategoriesRepository;
        public ICategoryRepository Categories => _categoryRepository;

        public void CreateTransaction()
        {
            // Assuming DatabaseSession has a method to begin a transaction
            // on its internal IDbConnection
            _dbSession.Transaction = _dbSession.Connection.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _dbSession.Transaction?.Commit();
            }
            catch
            {
                Rollback();
                throw;
            }
            finally
            {
                DisposeTransaction();
            }
        }

        public void Rollback()
        {
            _dbSession.Transaction?.Rollback();
            DisposeTransaction();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await Task.FromResult(1);
        }

        public void ClearChangeTracker()
        {
        }

        private void DisposeTransaction()
        {
            _dbSession.Transaction?.Dispose();
            _dbSession.Transaction = null;
        }

        public void Dispose()
        {
            DisposeTransaction();
            _dbSession.Connection?.Dispose();
        }
    }
}