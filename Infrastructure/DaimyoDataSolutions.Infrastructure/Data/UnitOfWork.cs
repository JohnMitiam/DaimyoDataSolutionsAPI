using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Infrastructure.Data.Repositories;

namespace DaimyoDataSolutions.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseSession _dbSession;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        public UnitOfWork
            (
                DatabaseSession dbSession,
                IUserRepository userRepository,
                IProductRepository productRepository
            )
        {
            _dbSession = dbSession;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        public IUserRepository Users => _userRepository;
        public IProductRepository Products => _productRepository;

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
