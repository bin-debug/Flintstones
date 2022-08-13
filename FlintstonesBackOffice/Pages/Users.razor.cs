using FlintstonesBackOffice.Utils;
using FlintstonesEntities;

namespace FlintstonesBackOffice.Pages
{
    public partial class Users
    {
        bool _processing = false;
        string[] headings = { "Name", "Email" };
        public List<BOUserEntity> _users = new List<BOUserEntity>();

        public string NewFullName { get; set; }
        public string NewEmail { get; set; }
        public string NewPassword { get; set; }

        private bool visible;
        private void OpenDialog() => visible = true;
        void CloseDialog() => visible = false;
        bool open;

        protected override async Task OnInitializedAsync()
        {
            TableStorageService.TableName = "BACKOFFICE";
            await PopulateUsers();
        }

        async Task PopulateUsers()
        {
            _processing = true;
            var tableClient = await TableStorageService.GetTableClient();
            string query = $"PartitionKey eq 'USERS'";
            var results = tableClient.Query<BOUserEntity>(query).ToList();
            _users = results;
            _processing = false;
        }

        Task OpenUserDialog(SettingEntity settingEntity)
        {

            visible = true;
            return Task.CompletedTask;
        }

        async Task SaveUser()
        {
            _processing = true;

            if (string.IsNullOrEmpty(NewFullName) || string.IsNullOrEmpty(NewEmail) || string.IsNullOrEmpty(NewPassword))
            {
                _processing = false;
                visible = false;
                return;
            }

            NewPassword = HashUtil.ComputeSha256Hash(NewPassword);

            var tableClient = await TableStorageService.GetTableClient();
            var user = new BOUserEntity()
            {
                CreatedDate = DateTime.UtcNow,
                Email = NewEmail,
                FullName = NewFullName,
                LastLogin = DateTime.UtcNow,
                Password = NewPassword,
                PartitionKey = "USERS",
                RowKey = NewEmail
            };

            var response = await tableClient.UpsertEntityAsync(user);
            if (response.Status == 204)
            {
                await PopulateUsers();
                _processing = false;
                visible = false;
            }
        }
    }
}