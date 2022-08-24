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
    private void HandleAccessToken(HttpListenerContext ctx)
    {
      var req = ctx.Request;
      string payload = GetRequestPostData(req);
      var jn = JsonSerializer.Deserialize<AccessTokenJN>(payload);
      _accessToken = jn.accessToken;

      ReturnAny(ctx);
    }
  }
}