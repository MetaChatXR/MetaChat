using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.MixedReality.WebRTC;
using SuperSimpleTcp;
using System.Text.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;

namespace MetaChat
{
    class App
    {
        public string serverPort;
        public string gamePort;
        public MyServer server;
        public MyRTC rtc;
        DeviceAudioTrackSource audioTrackSource = null;
        public void Start()
        {
            server = new MyServer
            {
                app = this,
                port = serverPort
            };
            server.Start();
            Console.WriteLine($"\n If no web page is opened automatically, please open {server.Prefix} by yourself.");
            OpenBrowser(server.Prefix);
        }
        public void StartWS(CreateChatJN config)
        {
            if (rtc != null)
            {
                rtc.Dispose();
            }
            //if (audioTrackSource == null)
            //    audioTrackSource = await DeviceAudioTrackSource.CreateAsync();
            rtc = new MyRTC
            {
                config = config,
                audioTrackSource = audioTrackSource
            };
            rtc.EnterChannel();
        }
        public void UpdateChat(CreateChatJN config)
        {
            if (config.chat == null)
            {
                rtc?.Dispose();
                rtc = null;
            }
            else
            {
                if (rtc == null) return;
                if (config.chat.id != rtc.config.chat.id)
                {
                    rtc?.Dispose();
                    rtc = null;
                }
            }
        }

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
