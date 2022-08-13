using FlintstonesEntities;
using System.Timers;

namespace FlintstonesBackOffice.Services
{
    public class SummaryService : IDisposable
    {
        private readonly System.Timers.Timer timer;
        const int INTERVAL_MS = 3000;
        public event Action<BOSummaryEntity> OnSummaryChanged;
        public event Action<List<BetEntity>> OnBetSummaryChanged;
        private TableStorageService _tableStorageService;

        public SummaryService(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
            _tableStorageService.TableName = "BACKOFFICE";
            timer = new System.Timers.Timer(INTERVAL_MS);
            timer.Elapsed += TimerTick;
            timer.Enabled = true;
        }

        private async void TimerTick(object sender, ElapsedEventArgs e)
        {
            var tableClient = await _tableStorageService.GetTableClient();
            string query = $"PartitionKey eq '{DateTime.Now.ToString("ddMMyyyy")}' and RowKey eq '{DateTime.Now.ToString("ddMMyyyy")}'";
            var result = tableClient.Query<BOSummaryEntity>(query).FirstOrDefault();
            OnSummaryChanged?.Invoke(result);

            string from = DateTime.Now.ToString("yyyy-MM-ddT00:00:00");
            string to = DateTime.Now.ToString("yyyy-MM-ddT23:59:59");
            string filter = $"CreatedDate ge datetime'{from}' and CreatedDate le datetime'{to}'";
            _tableStorageService.TableName = "BETS";
            var betsTableClient = await _tableStorageService.GetTableClient();
            var results = betsTableClient.Query<BetEntity>(filter.ToString()).OrderByDescending(r => r.CreatedDate).Take(20).ToList();
            OnBetSummaryChanged?.Invoke(results);

        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
