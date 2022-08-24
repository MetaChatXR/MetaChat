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
    private string _buildVer = "v0.1.0";
    private void HandleIndex(HttpListenerContext ctx)
    {
      var req = ctx.Request;
      var resp = ctx.Response;
      var env = req.QueryString?["env"] ?? "web";
      var isVAM = env.Equals("vam");
      var insert = $@"
    <script>
window.metachatEnv = '{env}';
window.gamePort = '{app.gamePort}';
" +
(isVAM ? $"localStorage.setItem('AS.AccessToken', '{_accessToken}');" : "")
+ @"
    </script>
";

      var buf = Encoding.ASCII.GetBytes($@"
<!DOCTYPE html>
<html lang='en'>
  <head>
    <meta charset='utf-8' />
    <link
      rel='icon'
      href='https://metachat-app.s3.us-west-1.amazonaws.com/favicon.ico'
    />
    <meta name='viewport' content='width=device-width,initial-scale=1' />
    <meta name='theme-color' content='#1976d2' />
    <meta
      name='description'
      content='Web site created using create-react-app'
    />
    <link
      rel='apple-touch-icon'
      href='https://metachat-app.s3.us-west-1.amazonaws.com/logo.png'
    />
    <link
      rel='stylesheet'
      href='https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap'
    />
    <link
      rel='stylesheet'
      href='https://fonts.googleapis.com/icon?family=Material+Icons'
    />
    <link
      rel='manifest'
      href='https://metachat-app.s3.us-west-1.amazonaws.com/manifest.json'
    />
    <title>MetaChat</title>
    <script
      defer='defer'
      src='https://metachat-app.s3.us-west-1.amazonaws.com/static-{_buildVer}/js/main.js'
    ></script>
    <link
      href='https://metachat-app.s3.us-west-1.amazonaws.com/static-{_buildVer}/css/main.css'
      rel='stylesheet'
    />
  </head>
  <body>
    <noscript>You need to enable JavaScript to run this app.</noscript>
" + insert + @"
    <div id='root'></div>
    <script
      async
      src='https://www.googletagmanager.com/gtag/js?id=G-CPFMJJ0SZZ'
    ></script>
    <script>
      function gtag() {
        dataLayer.push(arguments);
      }
      (window.dataLayer = window.dataLayer || []),
        gtag('js', new Date()),
        gtag('config', 'G-CPFMJJ0SZZ');
    </script>
  </body>
</html>


");
      resp.ContentType = "text/html";
      resp.AppendHeader("Access-Control-Allow-Origin", "*");

      resp.OutputStream.Write(buf, 0, buf.Length);
      resp.OutputStream.Close();
    }
  }
}
