using System;
using WebSocket4Net;
using SuperSocket.ClientEngine;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.MixedReality.WebRTC;

namespace MetaChat
{
  public class WSClient
  {
    public event EventHandler<PackageJN> Received;

    public WebSocket ws;
    public WSClient(string host, string token)
    {
      ws = new WebSocket($"{host}/v2/{token}");
      //ws.Security.AllowNameMismatchCertificate = true;
      //ws.Security.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls;

      ws.Opened += Ws_Opened;
      ws.MessageReceived += Ws_MessageReceived;
      ws.Closed += Ws_Closed;
      ws.Error += Ws_Error;
    }
    public void Dispose()
    {
      ws.Opened -= Ws_Opened;
      ws.MessageReceived -= Ws_MessageReceived;
      ws.Closed -= Ws_Closed;
      ws.Error -= Ws_Error;
      ws.Close();
    }
    public void Open()
    {
      ws.Open();
    }

    private void Ws_Opened(object sender, EventArgs e)
    {
    }

    private void Ws_Closed(object sender, EventArgs e)
    {
    }

    private void Ws_Error(object sender, ErrorEventArgs e)
    {
    }

    private void Ws_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
      Received?.Invoke(this, PackageJN.JN(JsonDocument.Parse(e.Message)));
    }

  }
}
