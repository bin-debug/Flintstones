using BlazorMonaco;
using BlazorMonaco.Bridge;
using FlintstonesEntities;
using MudBlazor;
using System.Text;
using System.Text.Json;

namespace FlintstonesBackOffice.Pages.Reports
{
    public partial class ClientReport
    {
        bool _processing = false;
        private bool _bladeProcessing = false;
        public string SelectedSearchType { get; set; }
        public string SelectedStatus { get; set; }
        string SearchTextValue;
        DateTime? FromDate = DateTime.Today;
        Dictionary<string, string> SearchData = new Dictionary<string, string>();
        public List<BetEntity> _bets = new List<BetEntity>();
        string[] headings = { "Date", "Market", "Selection", "Duration", "Status", "Stake", "Payout","" };
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
                ReadOnly = true
            };
        }

        protected override Task OnInitializedAsync()
        {
            SelectedSearchType = "ClientID";
            return base.OnInitializedAsync();
        }

        async Task OnSelectedValuesChanged(string val)
        {
            SelectedSearchType = val;
        }

        void AddFilter()
        {
            string type = SelectedSearchType;
            string value = "";

            if (type == "ClientID")
            {
                if (!string.IsNullOrEmpty(SearchTextValue))
                    value = SearchTextValue;
                else
                    return;
            }

            if (type == "BetID")
            {
                if (!string.IsNullOrEmpty(SearchTextValue))
                    value = SearchTextValue;
                else
                    return;
            }

            if (type == "Status")
            {
                if (!string.IsNullOrEmpty(SelectedStatus))
                    value = SelectedStatus;
                else
                    return;
            }

            if (type == "From Date")
                value = FromDate.Value.Date.ToString();

            if (SearchData.ContainsKey(type))
                return;

            SearchData.Add(type, value);
        }

        void Closed(MudChip chip)
        {
            SearchData.Remove(chip.Value.ToString());
        }

        async Task Search()
        {
            try
            {
                var filter = new StringBuilder();

                if (SearchData == null || SearchData.Count == 0)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopEnd;
                    Snackbar.Add("Please add to search criteria.", Severity.Error);
                    return;
                }

                if (!SearchData.ContainsKey("ClientID"))
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopEnd;
                    Snackbar.Add("Please enter clientID.", Severity.Error);
                    return;
                }

                _processing = true;
                TableStorageService.TableName = "BETS";
                var tableClient = await TableStorageService.GetTableClient();

                string clientID = SearchData["ClientID"];
                filter.AppendLine($"PartitionKey eq '{clientID}'");

                if (SearchData.ContainsKey("BetID"))
                {
                    if (!string.IsNullOrEmpty(SearchData["BetID"]))
                    {
                        string betID = SearchData["BetID"];
                        filter.AppendLine($" and RowKey eq '{betID}'");
                    }
                }

                if (SearchData.ContainsKey("Status"))
                {
                    if (!string.IsNullOrEmpty(SearchData["Status"]))
                    {
                        int statusID = GetStatusIDFromName(SearchData["Status"]);
                        filter.AppendLine($" and StatusID eq {statusID}");
                    }
                }

                if (SearchData.ContainsKey("From Date"))
                {
                    if (!string.IsNullOrEmpty(SearchData["From Date"]))
                    {
                        DateTime fromDate = Convert.ToDateTime(SearchData["From Date"]);
                        filter.AppendLine($" and CreatedDate ge datetime'{fromDate.ToString("yyyy-MM-dd")}'");
                    }
                }
                //PartitionKey eq '123' and CreatedDate ge datetime'2022-07-26T00:00:00.000Z'
                var f = filter.ToString();

                var results = tableClient.Query<BetEntity>(filter.ToString()).OrderByDescending(r => r.CreatedDate).ToList();
                _bets = results;
                _processing = false;
            }
            catch (Exception ex)
            {
                throw;
            }
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

        int GetStatusIDFromName(string name)
        {
            //1: Active, 2: Win, 3: Lose, 4: Refund
            int result = 1;

            switch (name)
            {
                case "Active":
                    result = 1;
                    break;
                case "Win":
                    result = 2;
                    break;
                case "Lose":
                    result = 3;
                    break;
                case "Refund":
                    result = 4;
                    break;
                default:
                    result = 1;
                    break;
            }
            return result;
        }
    }
}

