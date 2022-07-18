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

namespace FlintstonesBackOffice.Shared
{
    public partial class MainLayout
    {
        bool _drawerOpen = true;

        MudTheme _currentTheme = null;

        MudTheme _darkTheme = new MudTheme
        {
            PaletteDark = new Palette
            {
                Background = "#0F172A",
                AppbarBackground = "#0F172A",
                DrawerText = "#f4f7f9",
                TextPrimary = "#f4f7f9",
                AppbarText = "#f4f7f9",
                DrawerBackground = "#0F172A",
                //Dark = "#0F172A",
                Surface = "#111827",
                TextDisabled = "#7C7C89",
                
            }
        };

        protected override Task OnInitializedAsync()
        {
            _currentTheme = _darkTheme;
            return base.OnInitializedAsync();
        }

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }


    }
}