using BlazorMonaco;
using BlazorMonaco.Bridge;
using FlintstonesEntities;
using MudBlazor;
using System.Text;
using System.Text.Json;

namespace FlintstonesBackOffice.Pages.Reports
{
    public partial class TransactionReport
    {
        bool _processing = false;
        private bool _bladeProcessing = false;
        DateTime? FromDate = DateTime.Today;
        DateTime MinDate = DateTime.Today.AddMonths(-1);
        DateTime MaxDate = DateTime.Today;   
        DateTime? ToDate = DateTime.Today;
        public List<BetEntity> _bets = new List<BetEntity>();
        string[] headings = { "Date", "Market", "Selection", "Duration", "Status", "Stake", "Payout", "" };

        bool open;
        Anchor anchor;
        string bladeText = "";
        private MonacoEditor _editor { get; set; }
        private MonacoEditor _editorTwo { get; set; }

        public StandaloneEditorConstructionOptions EditorConstructionOptions(MonacoEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Theme = "vs-dark",
                Language = "json",
                Value = "",
                ReadOnly = true,
                Minimap = new MinimapOptions { Enabled = false }
            };
        }

        async Task Search()
        {
            _processing = true;
            //CreatedDate ge datetime'2022-07-26' and CreatedDate lt datetime'2022-07-27'
            string from = FromDate.Value.ToString("yyyy-MM-ddT00:00:00");
            string to = ToDate.Value.ToString("yyyy-MM-ddT23:59:59");
            string filter = $"CreatedDate ge datetime'{from}' and CreatedDate le datetime'{to}'";

            TableStorageService.TableName = "BETS";
            var tableClient = await TableStorageService.GetTableClient();
            var results = tableClient.Query<BetEntity>(filter.ToString()).OrderByDescending(r => r.CreatedDate).ToList();
            _bets = results;
            _processing = false;
        }

        async Task OpenDrawerAsync(BetEntity bet)
        {
            _bladeProcessing = true;
            bladeText = bet.RowKey;
            open = true;
            this.anchor = Anchor.End;

            TableStorageService.TableName = "RESULTS";
            var tableClient = await TableStorageService.GetTableClient();
            var result = tableClient.Query<ResultEntity>($"PartitionKey eq '{bet.PartitionKey}' and RowKey eq '{bet.RowKey}'").FirstOrDefault();

            var betData = JsonSerializer.Serialize(bet);
            var resultData = JsonSerializer.Serialize(result);

            await _editor.SetValue(BeautifyJson(betData));
            await _editorTwo.SetValue(BeautifyJson(resultData));

            var endDate = bet.CreatedDate.AddSeconds(bet.Duration);
            await GetPrices(bet.Market, bet.CreatedDate, endDate);

            _bladeProcessing = false;
        }

        public static string BeautifyJson(string json)
        {
            using JsonDocument document = JsonDocument.Parse(json);
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions() { Indented = true });
            document.WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        async Task GetPrices(string market, DateTime start, DateTime end)
        {
            try
            {
                TableStorageService.TableName = market.ToUpper();
                var tableClient = await TableStorageService.GetTableClient();
                string partitionKey = start.ToString("ddMMyyyy");
                //PartitionKey eq '25072022' and RowKey gt '2022-07-25T18:08:54' and RowKey lt '2022-07-25T18:09:00'
                string filter = $"PartitionKey eq '{partitionKey}' and RowKey ge '{start.ToString("yyyy-MM-ddTHH:mm:ss")}' and RowKey le '{end.ToString("yyyy-MM-ddTHH:mm:ss")}'";
                var results = tableClient.Query<FeedEntity>(filter).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}

