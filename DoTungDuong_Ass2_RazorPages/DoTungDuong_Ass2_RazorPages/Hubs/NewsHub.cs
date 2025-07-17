using Microsoft.AspNetCore.SignalR;

namespace DoTungDuong_Ass2_RazorPages.Hubs
{
    public class NewsHub : Hub
    {
        public async Task SendNewsUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveNewsUpdate", message);
        }
    }
}
