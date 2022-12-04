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
using Azure.Core;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using FlintstonesUtils;

namespace FlintstonesWeb.Pages
{
    public partial class Lobby
    {
        [Parameter]
        public string client { get; set; }

        [Parameter]
        public string Key { get; set; }
        [Parameter]
        public string Token { get; set; }

        string[] headings = {"Name", "Symbol", ""};
        public List<LobbyEntity> LobbyEntity = new List<LobbyEntity>();
        public bool Authorize { get; set; } = false;
        public string cToken { get; set; }

        private bool visible;
        private void OpenDialog() => visible = true;
        void Submit() => visible = false;
        bool open;
        private DialogOptions dialogOptions = new() { };

        protected override async Task<Task> OnInitializedAsync()
        {
            Authorize = ValidateKey();

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
            NavigationManager.NavigateTo($"/game/{client}/{lobbyEntity.MarketSymbol}/{Key}/{Token}");
        }

        private bool ValidateKey()
        {
            try
            {
                var token = new JwtSecurityToken(jwtEncodedString: Key);
                string value = token.Claims.First(c => c.Type == "id").Value;
                if (value == null)
                    return false;

                if (value == "369")
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
    }
}