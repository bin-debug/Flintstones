using FlintstonesBackOffice.Utils;
using FlintstonesEntities;

namespace FlintstonesBackOffice.Services
{
    public class LoginService
    {
        private TableStorageService _tableStorageService;

        public LoginService(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
            _tableStorageService.TableName = "BACKOFFICE";
        }

        public async Task<BOUserEntity> ValidateUserAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var hash = HashUtil.ComputeSha256Hash(password);

            _tableStorageService.TableName = "BACKOFFICE";
            var tableClient = await _tableStorageService.GetTableClient();
            string query = $"PartitionKey eq 'USERS' and RowKey eq '{username}' and Password eq '{hash}'";
            var result = tableClient.Query<BOUserEntity>(query).FirstOrDefault();

            return result;
        }
    }
}
