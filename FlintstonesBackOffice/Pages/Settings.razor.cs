using FlintstonesEntities;

namespace FlintstonesBackOffice.Pages
{
    public partial class Settings
    {
        bool _processing = false;
        string[] headings = { "", "Name", "Value", "" };
        public List<SettingEntity> _settings = new List<SettingEntity>();
        SettingEntity SelectedSettingToUpdate;
        public string UpdatedSettingValue;
        private bool visible;
        private void OpenDialog() => visible = true;
        void CloseDialog() => visible = false;
        bool open;

        protected override async Task OnInitializedAsync()
        {
           TableStorageService.TableName = "BACKOFFICE";
           await PopulateSettings();
        }

        async Task PopulateSettings()
        {
            _processing = true;
            var tableClient = await TableStorageService.GetTableClient();
            string query = $"PartitionKey eq 'SETTINGS'";
            var results = tableClient.Query<SettingEntity>(query).ToList();
            _settings = results;
            _processing = false;
        }

        Task OpenSettingsDialog(SettingEntity settingEntity)
        {
            SelectedSettingToUpdate = new SettingEntity();
            SelectedSettingToUpdate = settingEntity;
            visible = true;
            UpdatedSettingValue = settingEntity.Value;
            return Task.CompletedTask;
        }

        async Task SaveSetting()
        {
            _processing = true;

            if (string.IsNullOrEmpty(UpdatedSettingValue))
            {
                _processing = false;
                visible = false;
                return;
            }

            var tableClient = await TableStorageService.GetTableClient();
            SelectedSettingToUpdate.Value = UpdatedSettingValue;
            var response = await tableClient.UpsertEntityAsync(SelectedSettingToUpdate);
            if (response.Status == 204)
            {
                _processing = false;
                visible = false;
            }
        }
    }
}

// await tableClient.UpsertEntityAsync<SettingEntity>(new SettingEntity() { PartitionKey = "SETTINGS", ID = Guid.NewGuid().ToString(), RowKey = "DebitURL", Value = "niv" });
