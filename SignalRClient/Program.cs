using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using static SignalRClient.LogAndReadFile;

namespace SignalRClient
{
  internal class Program
  {
    static MqttClient? client = null;
    public static string Urls = "http://10.14.7.15:9011/logErr";
    public static string userName = "agv";
    public static string broker = "10.14.7.15";
    public static string password = "thaco@1234";
    public static int port = 8883;
    public static string topic = "AGV/tydang";
    public static int countErrorWebSocket = 0;
    public static int countErrorMqtt = 0;
    public static int countpushMqtt = 0;
    public static int countConnectMqtt= 0;
    public static int countConnectWebsocket = 0;
    public static HubConnection connectionWebsocket = null;
    public static MqttClient clientMQTT = null;
    public static bool IsConnectWebsocket = false;
    public static bool IsConnectMqtt = false;

    static void ConnectWebsocket(string Urls)
    {
       connectionWebsocket = new HubConnectionBuilder()
        .WithUrl(Urls)
        .Build();
      try
      {
        connectionWebsocket.StartAsync().Wait();
      }
      catch (Exception)
      {
        IsConnectWebsocket = false;
      }
     
      if (connectionWebsocket.State == HubConnectionState.Connected)
        IsConnectWebsocket = true;
      else
      {
        countErrorWebSocket++;
        LogErrorToFile("logWebsocket.txt", $" {countErrorWebSocket}. - Websocket kết nối thất bại");
        IsConnectWebsocket = false;
      }  
        
    }
    static void ConnectMQTT(string broker, int port, string username, string password)
    {
      try
      {
        clientMQTT = new MqttClient(broker, port, false, MqttSslProtocols.None, null, null);
        clientMQTT.Connect(Guid.NewGuid().ToString(), username, password);
      }
      catch (Exception)
      {
        countErrorMqtt++;
        LogErrorToFile("logMQTT.txt", $" {countErrorMqtt}. - MQTT kết nối thất bại");

      }
    }
    static  void PublishWebsocket(HubConnection connectionWebsocket)
    {
      connectionWebsocket.On<string>("tuesday", (string data) =>
      {
        Console.WriteLine(data + ":");
      });

      try
      {
        if (IsConnectWebsocket)
        {
          countConnectWebsocket++;
          Console.Write(countConnectWebsocket + ". Connect socket!!!");
          connectionWebsocket.InvokeAsync("NewMessage", 1).GetAwaiter().GetResult();
        }
        else
        {
          Console.WriteLine("*********fail to connect Websocket");
          countErrorWebSocket++;
          LogErrorToFile("logWebsocket.txt", $" {countErrorWebSocket}. - Websocket kết nối thất bại");
          ConnectWebsocket(Urls);
        } 
      }
      catch (OperationCanceledException ex)
      {
       
      }
      catch (Exception ex)
      {
        //Console.WriteLine("&&&&&&&   ############fail to connect Websocket");
        //countErrorWebSocket++;
        //LogErrorToFile("logWebsocket.txt", $" {countErrorWebSocket}. - Websocket kết nối thất bại");
        ConnectWebsocket(Urls);
      }
    }
    static void PushlishMQTT(MqttClient client)
    {
      try
      {
         if(client.IsConnected)
         {
          countpushMqtt++;
          
          client.Publish(topic, Encoding.UTF8.GetBytes($" {countpushMqtt} - hi maintenance!!!"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

          //Thread.Sleep(1000);
          countConnectMqtt++;
          Console.Write(countConnectMqtt + ". Connected to MQTT Broker");
          }
         else
         {
          Console.WriteLine("****fail to MQTT Broker");
          countErrorMqtt++;
          LogErrorToFile("logMQTT.txt", $" {countErrorMqtt}. - MQTT kết nối thất bại");
          ConnectMQTT(broker, port, userName, password);
        }    
      }
      catch (Exception ex)
      {
        Console.WriteLine("fail to MQTT Broker");
        countErrorMqtt++;
        LogErrorToFile("logMQTT.txt", $" {countErrorMqtt}. - MQTT kết nối thất bại");
        ConnectMQTT(broker, port, userName, password);
      }
    }
    static async Task Main(string[] args)
    {
      
      Task task1 = Task.Run(() =>
      {
        ConnectWebsocket(Urls);
      });

      Task task2 = Task.Run(() =>
      {
        ConnectMQTT(broker, port, userName, password);

      });
      //Task.Delay(1000).Wait();
      Task.WaitAll(task1, task2);


      while (true)
      {

        Task task3 = Task.Run(() =>
        {
          PublishWebsocket(connectionWebsocket);
        });
        Task task4 = Task.Run(() =>
        {
          PushlishMQTT(clientMQTT);

        });
        //Task.Delay(1000).Wait();
        Task.WaitAll(task3, task4);
      }
    }
  }
}
