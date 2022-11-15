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
using FlintstonesWeb;
using FlintstonesWeb.Shared;
using MudBlazor;
using FlintstonesEntities;
using Newtonsoft.Json;

namespace FlintstonesWeb.Pages
{
    public partial class Lobby
    {
        string[] headings = {"Name", "Symbol", ""};
        public List<LobbyEntity> LobbyEntity = new List<LobbyEntity>();

        protected override async Task<Task> OnInitializedAsync()
        {
            await PopulateLobby();

            return base.OnInitializedAsync();
        }

        public async Task PopulateLobby()
        {
            var client = ClientFactory.CreateClient();
            var response = await client.GetStringAsync("https://markets-api.azurewebsites.net/api/LobbyFunction?code=y_XaVEJCPWdd3twatFpodDTlRRG3UMPrBLx8lPkJ410cAzFunAW_tg==");
            if (response != null)
            {
                LobbyEntity = JsonConvert.DeserializeObject<List<LobbyEntity>>(response);
            }
        }

        public void Navigate(LobbyEntity lobbyEntity)
        {
            NavigationManager.NavigateTo($"/game/{lobbyEntity.MarketSymbol}");
        }
    }
}