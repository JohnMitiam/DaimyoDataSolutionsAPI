using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Infrastructure.Data.Repositories;

namespace DaimyoDataSolutions.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseSession _dbSession;
        private readonly IAffiliateRepository _affiliateRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UnitOfWork
            (
                DatabaseSession dbSession,
                IAffiliateRepository affiliateRepository,
                IProductRepository productRepository,
                ICategoryRepository categoryRepository
            )
        {
            _dbSession = dbSession;
            _affiliateRepository = affiliateRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IAffiliateRepository Affiliate => _affiliateRepository;
        public IProductRepository Products => _productRepository;

        public ICategoryRepository Categories => _categoryRepository;

        public void Commit()
        {
            // Commit the transaction
            if (_dbSession.Transaction != null)
                _dbSession.Transaction.Commit();
            Dispose();
        }

        public void CreateTransaction()
        {
            // Create a new transaction
            if (_dbSession.Connection != null)
            {
                _dbSession.Transaction = _dbSession.Connection.BeginTransaction();
            }
            else
            {
                throw new Exception("Database Session is null");
            }
        }

        public void Dispose()
        {
            // Dispose of the unit of work
            _dbSession.Transaction?.Dispose();
        }

        public void Rollback()
        {
            // Rollback the transaction
            if (_dbSession.Transaction != null)
                _dbSession.Transaction.Rollback();
            Dispose();
        }
    }
}
