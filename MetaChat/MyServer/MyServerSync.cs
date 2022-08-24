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
    private void HandleSync(HttpListenerContext ctx)
    {
      var req = ctx.Request;
      string payload = GetRequestPostData(req);
      var jn = JsonSerializer.Deserialize<SyncJN>(payload);
      app.rtc?.SendSync(jn);
      var back = app.rtc?.GetBack() ?? new RtcJN();

      ReturnJson(ctx, back);
    }
  }
}