using FlintstonesEntities;
using MudBlazor;

namespace FlintstonesBackOffice.Pages.Reports
{
    public partial class ClientReport
    {
        bool _processing = false;
        public string SelectedSearchType { get; set; }
        public string SelectedStatus { get; set; }

        string SearchTextValue;
        DateTime? FromDate = DateTime.Today;
        Dictionary<string, string> SearchData = new Dictionary<string, string>();

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

        Task Search()
        { 

            return Task.CompletedTask;
        }
    }
}

