using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

class Script
{
    private const string ConnectionUrl = "http://localhost:64596/";

  [STAThread]
  static public void Main(string[] args)
    {
        var connection = new HubConnection(ConnectionUrl);
        var hubProxy = connection.CreateHubProxy("Chat");

        //Start connection
        Console.WriteLine("Starting connection...");
        connection.Start().Wait();

        if (connection.State == ConnectionState.Connected)
        {
            Console.WriteLine("Invoking hub method...");
            hubProxy.Invoke("SendAll", "lol").Wait();
            Console.WriteLine("Closing connection...");
            connection.Disconnect();
        }
        else
        {
            Console.WriteLine("Not connected...");
        }

        SignalRProxyConnection.SendMessage();
  }

  static class SignalRProxyConnection
  {
      private static IHubProxy _proxy;
      private static HubConnection _hubConnection;
      private const string ConnectionUrl = "http://localhost:64596/";
      private const string HubName = "Chat";
      private const string HubMethodName = "SendAll";

      public static async void SendMessage()
      {
          Console.WriteLine("Trying to send message to hub");
          if (!IsConnected)
          {
              _hubConnection = new HubConnection(ConnectionUrl);
              _proxy = _hubConnection.CreateHubProxy(HubName);
              try
              {
                  _hubConnection.Start().Wait();
                  Console.WriteLine("Success! Connected with client connection id {0}", _hubConnection.ConnectionId);
              }
              catch (Exception ex)
              {
                  Console.WriteLine(
                      "An error occured when trying to connection to {0} for Hub Proxy {1} with this exception: {2}",
                      ConnectionUrl, HubName, ex.GetBaseException());
                  throw;
              }
          }
          else
          {
              Console.WriteLine("Already connected with client connection id {0}", _hubConnection.ConnectionId);
          }
          try
          {
              await _proxy.Invoke(HubMethodName, "lol");
              Console.WriteLine("Success! Invoked SendAll()");
          }
          catch (Exception ex)
          {
              Console.WriteLine("An error occurred trying to invoke the Hub method {0} with this exception: {1}",
                                HubMethodName, ex.GetBaseException());
              throw;
          }
      }

      private static bool IsConnected
      {
          get { return _hubConnection != null && _hubConnection.State != ConnectionState.Connected; }
      }
  }
}

