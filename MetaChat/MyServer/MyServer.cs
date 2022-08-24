using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.Collections.Generic;

namespace MetaChat
{
  partial class MyServer
  {
    public string port;
    public App app;
    public string Prefix;
    private HttpListener _listener;
    private string _accessToken;

    public void Start()
    {
      Prefix = $"http://127.0.0.1:{port}/";
      _listener = new HttpListener();
      _listener.Prefixes.Add(Prefix);
      _listener.Start();
      _listener.BeginGetContext(OnContext, null);
    }

    private void OnContext(IAsyncResult ar)
    {
      var ctx = _listener.EndGetContext(ar);
      _listener.BeginGetContext(OnContext, null);
      var req = ctx.Request;

      //Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + $" {req.HttpMethod} {req.Url.AbsolutePath}");

      if (req.HttpMethod == "GET")
      {
        HandleIndex(ctx);
      }
      else if (req.HttpMethod == "POST")
      {
        if (req.Url.AbsolutePath == "/accessToken")
        {
          HandleAccessToken(ctx);
        }
        else if (req.Url.AbsolutePath == "/rtc")
        {
          HandleRTC(ctx);
        }
        else if (req.Url.AbsolutePath == "/chat")
        {
          HandleChat(ctx);
        }
        else if (req.Url.AbsolutePath == "/hub-info")
        {
          HandleHubInfo(ctx);
        }
        else if (req.Url.AbsolutePath == "/sync")
        {
          HandleSync(ctx);
        }
        else
        {
          Handle404(ctx);
        }
      }
    }
  }
}