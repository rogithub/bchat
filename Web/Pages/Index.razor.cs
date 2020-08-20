using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace Web.Pages
{
    public class MessageModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Message is too long.")]
        public string Message { get; set; }
    }


    public partial class Index
    {
        public MessageModel Message { get; set; } = new MessageModel();
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
        }

        private async Task Send()
        {
            if (string.IsNullOrWhiteSpace(Message.Name) ||
                string.IsNullOrWhiteSpace(Message.Message))
                return;

            Message.Message = string.Empty;
            await Focus("txtMsg");

            await hubConnection.SendAsync("SendMessage", Message.Name, Message.Message);
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
