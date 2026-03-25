using Dapper;
using DaimyoDataSolutions.Domain.Entities;
using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.ResourceParameters;
using System.Data;

namespace DaimyoDataSolutions.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseSession _dbSession;

        public UserRepository(DatabaseSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<User> CreateAsync(User user)
        {
            var query = $@"sp_CreateUser";

            var queryParams = new
            {
                user.UserName,
                user.Email,
                user.Status,
                user.IsActive,
                user.CreatedBy,
                user.DateCreated
            };

            user.Id = await _dbSession.Connection
                .ExecuteScalarAsync<int>(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return user;
        }

        public async Task<(IEnumerable<User> users, int recordCount)> GetAsync(UserResourceParameters resourceParameters)
        {
            var queryParamBuilder = new QueryParameters(
                resourceParameters.Search ?? string.Empty,
                resourceParameters.SearchFields ?? new List<string>(),
                resourceParameters.Page,
                resourceParameters.PageSize
            );

            var baseDataQuery = @"SELECT * FROM Users WHERE IsDeleted = 0 ";
            var baseCountQuery = @"SELECT COUNT(*) FROM Users WHERE IsDeleted = 0 ";

            var searchSQL = queryParamBuilder.GetSearchSQLQuery();
            var filterSQL = queryParamBuilder.GetFilterSQLQuery();
            var paginationSQL = queryParamBuilder.GetPaginationSQLQuery();

            var finalDataQuery = baseDataQuery + searchSQL + filterSQL + paginationSQL;
            var finalCountQuery = baseCountQuery + searchSQL + filterSQL;

            var result = await _dbSession.Connection.QueryAsync<User>(finalDataQuery, queryParamBuilder.Parameters);
            var totalCount = await _dbSession.Connection.ExecuteScalarAsync<int>(finalCountQuery, queryParamBuilder.Parameters);

            return (result, totalCount);
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            var query = @"sp_GetUserById";

            var queryParams = new
            {
                UserID = userId
            };

            var result = await _dbSession.Connection
                .QueryFirstOrDefaultAsync<User>(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            var query = $@"sp_DeleteUser";

            var queryParams = new
            {
                UserID = userId
            };

            await _dbSession.Connection.ExecuteAsync(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure);

            return true;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var query = $@"sp_UpdateUser";

            var queryParams = new
            {
                UserID = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Status = user.Status,
                IsActive = user.IsActive,
                CreatedBy = user.CreatedBy,
                DateUpdated =DateTime.Now
            };

            await _dbSession.Connection
                .ExecuteAsync(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return true;
        }
    }
}