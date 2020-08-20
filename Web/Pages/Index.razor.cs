using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace Web.Pages
{
    public class NombreModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }
    }


    public partial class Index
    {
        public NombreModel Model { get; set; } = new NombreModel();

        protected override async Task OnInitializedAsync()
        {
            await Focus("txtUserName");
        }

        private void Send()
        {
            if (string.IsNullOrWhiteSpace(Model.Name))
            {
                return;
            }

            NavigationManager.NavigateTo($"/chat/{HttpUtility.UrlEncode(Model.Name)}");
        }


        public async Task Focus(string elementId)
        {
            await js.InvokeVoidAsync("helpers.focus", elementId);
        }

    }
}
