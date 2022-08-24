using System.Net;
using System.Text;
using System.Text.Json;

namespace MetaChat
{
  partial class MyServer
  {
    private void ReturnJson<TValue>(HttpListenerContext ctx, TValue value)
    {
      var resp = ctx.Response;

      var buf = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(value));
      resp.ContentType = "application/json";

      resp.OutputStream.Write(buf, 0, buf.Length);
      resp.OutputStream.Close();
    }
    private void ReturnAny(HttpListenerContext ctx)
    {
      var resp = ctx.Response;

      var buf = Encoding.ASCII.GetBytes("{\"kkk\": 444}");
      resp.ContentType = "application/json";

      resp.OutputStream.Write(buf, 0, buf.Length);
      resp.OutputStream.Close();
    }

    private void Handle404(HttpListenerContext ctx)
    {
      var req = ctx.Request;
      var resp = ctx.Response;

      var buf = Encoding.ASCII.GetBytes("404");
      resp.ContentType = "text/plain";

      resp.OutputStream.Write(buf, 0, buf.Length);
      resp.OutputStream.Close();
    }

    public static string GetRequestPostData(HttpListenerRequest request)
    {
      if (!request.HasEntityBody)
      {
        return null;
      }
      using (System.IO.Stream body = request.InputStream) // here we have data
      {
        using (var reader = new System.IO.StreamReader(body, request.ContentEncoding))
        {
          return reader.ReadToEnd();
        }
      }
    }

  }
}