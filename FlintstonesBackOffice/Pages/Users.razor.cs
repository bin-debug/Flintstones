using FlintstonesBackOffice.Utils;
using FlintstonesEntities;
using System.Net.Mail;

namespace FlintstonesBackOffice.Pages
{
    public partial class Users
    {
        bool _processing = false;
        bool _showDialogErr = false;
        string[] headings = { "Name", "Email" };
        public List<BOUserEntity> _users = new List<BOUserEntity>();
        public string dialogErrMsg;


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
            TableStorageService.TableName = "BACKOFFICE";
            var tableClient = await TableStorageService.GetTableClient();
            string query = $"PartitionKey eq 'USERS'";
            var results = tableClient.Query<BOUserEntity>(query).ToList();
            _users = results;
            _processing = false;
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

            if (IsEmailValid(NewEmail) == false)
            {
                _processing = false;
                _showDialogErr = true;
                dialogErrMsg = "Email address is invalid";
                return;
            }

            var hashy = HashUtil.ComputeSha256Hash(NewPassword);

            TableStorageService.TableName = "BACKOFFICE";
            var tableClient = await TableStorageService.GetTableClient();
            var user = new BOUserEntity()
            {
                CreatedDate = DateTime.UtcNow,
                Email = NewEmail,
                FullName = NewFullName,
                LastLogin = DateTime.UtcNow,
                Password = hashy,
                PartitionKey = "USERS",
                RowKey = NewEmail
            };

            var response = await tableClient.UpsertEntityAsync(user);
            if (response.Status == 204)
            {
                await PopulateUsers();
                NewFullName = String.Empty;
                NewEmail = String.Empty;
                NewPassword = String.Empty;
                _processing = false;
                _showDialogErr = false;
                dialogErrMsg = string.Empty;
                visible = false;
            }
        }

        async Task DeleteUser(BOUserEntity user)
        {
            _processing = true;
            var tableClient = await TableStorageService.GetTableClient("BACKOFFICE");
            var response = await tableClient.DeleteEntityAsync("USERS", user.RowKey);
            if (response.Status == 204)
            {
                await PopulateUsers();

                _processing = false;
                visible = false;
            }
        }

        private static bool IsEmailValid(string email)
        {
            var valid = true;
            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }
            return valid;
        }
    }
}