using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MetaChat
{
  partial class MyServer
  {
    private async void HandleHubInfo(HttpListenerContext ctx)
    {
      var req = ctx.Request;
      string payload = GetRequestPostData(req);
      var jn = JsonSerializer.Deserialize<RequestHubInfoJN>(payload);
      var url = jn.url;
      var thumbnail = await RequestHubInfo(jn.url);
      ReturnJson(ctx, new HubInfoJN { url = url, thumbnail = thumbnail });
    }

    private async Task<string> RequestHubInfo(string url)
    {
      CookieContainer cookies = new CookieContainer();
      cookies.Add(new Cookie("vamhubconsent", "yes") { Domain = "hub.virtamate.com" });
      HttpClientHandler handler = new HttpClientHandler();
      handler.CookieContainer = cookies;

      using (HttpClient client = new HttpClient(handler))
      {
        var response = await client.GetAsync(url);
        var respString = await response.Content.ReadAsStringAsync();
        var matches = Regex.Matches(respString, @"<meta property=""og:image"" content=""(.*?)""");

        foreach (Match match in matches)
        {
          if (match.Success && match.Groups.Count > 0)
          {
            var text = match.Groups[1].Value;
            return text;
          }
        }
      }
      return null;
    }
  }
}
