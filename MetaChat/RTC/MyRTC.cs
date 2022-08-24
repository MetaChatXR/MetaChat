
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MixedReality.WebRTC;
using WebSocket4Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace MetaChat
{
  class MyRTC : IDisposable
  {
    public CreateChatJN config;
    public DeviceAudioTrackSource audioTrackSource;

    WSClient wsClient;
    List<MyPeer> peers = new List<MyPeer> { };
    public RtcJN back = new RtcJN();

    public void EnterChannel()
    {
      wsClient = new WSClient(config.channel.host, config.channel.token);
      wsClient.Received += Ws_Received;
      wsClient.Open();
    }

    public void SendSync(SyncJN jn)
    {
      var dcJn = new DcJN
      {
        type = DcJN.Type.Sync,
        sync = jn
      };
      var dcStr = JsonSerializer.Serialize(dcJn);
      foreach (var peer in peers)
      {
        peer.SendMessage(dcStr);
      }
    }
    public RtcJN GetBack()
    {
      RtcJN back = this.back;
      this.back = new RtcJN();
      return back;
    }

    private async void Ws_Received(object sender, PackageJN pkt)
    {
      var type = pkt.meta.type;
      if (type == "message")
      {
        var from = pkt.meta.fromId;
        if (from == config.channel.userK) return;
        HandleMessage(pkt);
        return;
      }

      if (type == "peers")
      {
        foreach (var user in pkt.peers.users)
        {
          addUser(user);
        }
      }
      else if (type == "peer_connected")
      {
        addUser(pkt.user);
      }
      else if (type == "peer_removed")
      {
        var peer = peers.Find(peer => peer.remoteId == pkt.user);
        peer.Received -= Peer_Received;
        peer.Dispose();
        peers.Remove(peer);
      }

    }
    private async void HandleMessage(PackageJN pkt)
    {
      var from = pkt.meta.fromId;
      var peer = peers.Find(peer => peer.remoteId == from);
      if (peer == null)
      {
        peer = initUser(from, false);
      }

      peer.MsgReceived(pkt);
    }
    private void addUser(string user)
    {
      if (user == config.channel.userK) return;
      if (peers.Where(peer => peer.remoteId == user).Any()) return;
      var IsClient = String.Compare(config.channel.userK, user) == 1;
      initUser(user, IsClient);

    }

    private MyPeer initUser(string user, bool IsClient)
    {
      var peer = new MyPeer(user, IsClient, config.channel.GetIceServers(), audioTrackSource);
      peer.OnSend += (object sender, MsgJN m) =>
      {
        string mJN = JsonSerializer.Serialize(m);
        var s = new SendJN
        {
          t = "u",
          m = new SendMetaJN
          {
            f = $"{config.channel.channel}/{config.channel.userK}",
            t = user,
            o = "message"
          },
          p = mJN
        };

        wsClient.ws.Send(JsonSerializer.Serialize(s));
      };
      peer.Received += Peer_Received;
      peers.Add(peer);
      peer.Init();
      return peer;

    }
    private void Peer_Received(MyPeer peer, DcJN dcJn)
    {
      if (dcJn.type == DcJN.Type.Sync)
      {
        var p = back.peers.Find(p => p.id == peer.remoteId);
        if (p != null) back.peers.Remove(p);
        back.peers.Add(new SyncPeerJN
        {
          id = peer.remoteId,
          info = dcJn.sync.info
        });
      }
    }

    public void Dispose()
    {
      if (wsClient != null)
      {
        wsClient.Received += Ws_Received;
        wsClient.Dispose();
      }
      foreach (var peer in peers)
      {
        peer.Dispose();
      }
    }
  }
}
