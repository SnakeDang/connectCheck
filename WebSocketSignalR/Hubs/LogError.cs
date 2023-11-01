using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace WebSocketSignalR.Hubs
{
  public class LogError : Hub
  {
    public static int count { get; set; } = 9;
    public override Task OnConnectedAsync()
    {
      // var data = dashboardController.GetContact();
      //var data = uow.GetRepository<Contact>().Count(x => !x.IsDeleted);

      //Clients.All.SendAsync("tuesday", $"Total {count} Contact \n TimeCurrent {DateTime.Now.ToLongTimeString()}").GetAwaiter().GetResult();
      return base.OnConnectedAsync();
    }

    public async Task NewMessage(int increase)
    {
      count += increase;
      await Clients.All.SendAsync("tuesday", $"Total {count} Contact \n TimeCurrent {DateTime.Now.ToLongTimeString()}");
    }
  }
}
