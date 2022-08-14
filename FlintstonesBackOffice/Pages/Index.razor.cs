using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using FlintstonesBackOffice;
using FlintstonesBackOffice.Shared;
using MudBlazor;
using FlintstonesBackOffice.Models;
using BlazorMonaco;
using BlazorMonaco.Bridge;
using FlintstonesEntities;

namespace FlintstonesBackOffice.Pages
{
    public partial class Index
    {
        private bool _processing = false;
        private int NumberOfBets = 0;
        private double TotalStake = 0;
        private double TotalPayout = 0;


        string[] headings = { "Date", "Market", "Selection", "Duration", "Status", "Stake", "Payout",};
        public List<BetEntity> _bets = new List<BetEntity>();

        protected override Task OnInitializedAsync()
        {
            SummaryService.OnSummaryChanged += SummaryService_OnSummaryChanged;
            SummaryService.OnBetSummaryChanged += SummaryService_OnBetSummaryChanged;

            return base.OnInitializedAsync();
        }

        private void SummaryService_OnBetSummaryChanged(List<BetEntity> obj)
        {

            if (obj == null)
                return;

            _processing = true;
            _bets = obj;
            _processing = false;

            InvokeAsync(() => StateHasChanged());
        }

        private void SummaryService_OnSummaryChanged(FlintstonesEntities.BOSummaryEntity obj)
        {

            if (obj == null)
                return;

            _processing = true;
            NumberOfBets = obj.NumberOfBets;
            TotalStake = obj.TotalStake;
            TotalPayout = obj.TotalPayout;
            _processing = false;

        }
    }
}