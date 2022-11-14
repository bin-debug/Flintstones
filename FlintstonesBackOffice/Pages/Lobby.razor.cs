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
using FlintstonesBackOffice.Models;
using BlazorMonaco;
using BlazorMonaco.Bridge;
using MudBlazor;
using FlintstonesBackOffice.Services;
using FlintstonesEntities;
using System.Runtime;

namespace FlintstonesBackOffice.Pages
{
    public partial class Lobby
    {
        bool _processing = false;
        string[] headings = {"Market", "Symbol", "Active", "" };
        private bool visible;
        private void OpenDialog() => visible = true;
        void CloseDialog() => visible = false;
        public List<LobbyEntity> _markets = new List<LobbyEntity>();
        LobbyEntity SelectedMarketToUpdate;

        bool open;
        string MarketName;
        string MarketSymbol;
        bool IsMarketActive;

        protected override async Task OnInitializedAsync()
        {
            await PopulateLobby();
            base.OnInitialized();
        }

        async Task PopulateLobby()
        {
            _processing = true;
            TableStorageService.TableName = "BACKOFFICE";
            var tableClient = await TableStorageService.GetTableClient();
            string query = $"PartitionKey eq 'LOBBY'";
            var results = tableClient.Query<LobbyEntity>(query).ToList();
            _markets = results;
            _processing = false;
        }

        Task OpenMarketDialog()
        {
            visible = true;

            return Task.CompletedTask;
        }

        async Task SaveMarket()
        {
            _processing = true;

            if (string.IsNullOrEmpty(MarketName))
            {
                _processing = false;
                visible = false;
                return;
            }

            SelectedMarketToUpdate = new LobbyEntity()
            {
                PartitionKey = "LOBBY",
                RowKey = MarketName,
                MarketName = MarketName,
                MarketSymbol = MarketSymbol,
                IsMarketActive = IsMarketActive,
                Timestamp = DateTime.Now,
            };

            TableStorageService.TableName = "BACKOFFICE";
            var tableClient = await TableStorageService.GetTableClient();
            var response = await tableClient.UpsertEntityAsync(SelectedMarketToUpdate);
            if (response.Status == 204)
            {
                await PopulateLobby();

                _processing = false;
                visible = false;
            }
        }

    }
}