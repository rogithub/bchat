using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace Web.Pages
{
    public class ChatModel
    {
        [Required]
        [StringLength(255, ErrorMessage = "Message is too long.")]
        public string Message { get; set; }
    }


    public partial class Chat
    {
        [Parameter]
        public string User { get; set; }
        public ChatModel Message { get; set; } = new ChatModel();
        string Url { get; set; } = "https://dejatupuntito.com/communicator";
        private HubConnection hubConnection;
        private List<string> Messages { get; set; } = new List<string>();

        public bool IsConnected =>
        hubConnection.State == HubConnectionState.Connected;


        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Url)
                .Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                Messages.Add(encodedMsg);
                StateHasChanged();
            });

            await hubConnection.StartAsync();
            await Focus("txtMsg");
        }

        private async Task Send()
        {
            await Focus("txtMsg");

            if (string.IsNullOrWhiteSpace(User))
            {
                NavigationManager.NavigateTo($"/");
                return;
            }
            if (!IsConnected)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Message.Message))
            {
                return;
            }

            await hubConnection.SendAsync("SendMessage", User, Message.Message);
            Message.Message = string.Empty;
        }

        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }

        public async Task Focus(string elementId)
        {
            await js.InvokeVoidAsync("helpers.focus", elementId);
        }

        private string ConnectionId { get; set; }
    }
}
