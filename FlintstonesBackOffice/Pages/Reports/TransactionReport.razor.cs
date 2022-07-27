using FlintstonesEntities;

namespace FlintstonesBackOffice.Pages.Reports
{
    public partial class TransactionReport
    {
        bool _processing = false;
        DateTime? FromDate = DateTime.Today;
        DateTime? ToDate = DateTime.Today;
        public List<BetEntity> _bets = new List<BetEntity>();
        string[] headings = { "Date", "Market", "Selection", "Duration", "Status", "Stake", "Payout", "" };

        async Task Search()
        {
            _processing = true;
            //CreatedDate ge datetime'2022-07-26' and CreatedDate lt datetime'2022-07-27'
            string from = FromDate.Value.ToString("yyyy-MM-dd");
            string to = ToDate.Value.ToString("yyyy-MM-dd");
            string filter = $"CreatedDate ge datetime'{from}' and CreatedDate le datetime'{to}'";

            TableStorageService.TableName = "BETS";
            var tableClient = await TableStorageService.GetTableClient();
            var results = tableClient.Query<BetEntity>(filter.ToString()).OrderByDescending(r => r.CreatedDate).ToList();
            _bets = results;
            _processing = false;
        }

        async Task OpenDrawerAsync(BetEntity bet)
        {
            
        }
    }

}

