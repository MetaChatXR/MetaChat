
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MixedReality.WebRTC;
using System.Text.Json;

namespace MetaChat
{

  class MyPeer : IDisposable
  {
    Transceiver audioTransceiver = null;
    LocalAudioTrack localAudioTrack = null;

    public string remoteId;
    public bool IsClient;
    public bool Started = false;
    public bool Cannelled = false;
    public bool Loop = false;
    private List<IceServer> IceServers;

    public PeerConnection pc;
    DataChannel dc;
    public event EventHandler<MsgJN> OnSend;
    public delegate void ReceivedHandler(MyPeer peer, DcJN inter);
    public event ReceivedHandler Received;

    public MyPeer(string remoteId, bool IsClient, List<IceServer> IceServers, DeviceAudioTrackSource audioTrackSource2)
    {
      this.remoteId = remoteId;
      this.IsClient = IsClient;
      this.IceServers = IceServers;
    }
    public async Task Init()
    {
      if (IsClient)
      {
        OnSend?.Invoke(this, MsgJN.Handshake("start"));
      }
      else
      {
        await Start();
        OnSend?.Invoke(this, MsgJN.Handshake("done"));
      }
    }

    public async Task Start()
    {
      if (Started) return;
      Started = true;
      try
      {
        pc = new PeerConnection();

        pc.DataChannelAdded += PeerConnection_DataChannelAdded;
        pc.DataChannelRemoved += PeerConnection_DataChannelRemoved;
        pc.LocalSdpReadytoSend += PeerConnection_LocalSdpReadytoSend;
        pc.IceCandidateReadytoSend += PeerConnection_IceCandidateReadytoSend;

        var config = new PeerConnectionConfiguration
        {
          IceServers = IceServers
        };

        await pc.InitializeAsync(config);

        var dataChannelLabel = $"data_channel";
        dc = await pc.AddDataChannelAsync(dataChannelLabel, true, true, CancellationToken.None);

        var audioTrackSource = await DeviceAudioTrackSource.CreateAsync();
        if (Cannelled) return;
        var trackSettings = new LocalAudioTrackInitConfig { trackName = "mic_track" };
        localAudioTrack = LocalAudioTrack.CreateFromSource(audioTrackSource, trackSettings);

        audioTransceiver = pc.AddTransceiver(MediaKind.Audio);
        audioTransceiver.DesiredDirection = Transceiver.Direction.SendReceive;
        audioTransceiver.LocalAudioTrack = localAudioTrack;

        pc.Connected += () =>
        {
          //Console.WriteLine("PeerConnection: connected.");
        };
        pc.IceStateChanged += (IceConnectionState newState) => { Console.WriteLine($"ICE state: {newState}"); };
        pc.AudioTrackAdded += (RemoteAudioTrack track) =>
        {
          //Console.WriteLine($"AudioTrackAdded33: {track.Enabled}");
          //track.AudioFrameReady += (AudioFrame frame) =>
          //{
          //    Console.WriteLine($"AudioFrameReady4: {frame.bitsPerSample}, {frame.sampleRate}, {frame.channelCount}, {frame.sampleCount}");
          //};
          //track.OutputToDevice(true);
        };
        if (IsClient)
        {
          //Console.WriteLine("Connecting to remote peer...");
          if (Cannelled) return;
          pc.CreateOffer();
        }
        else
        {
          //Console.WriteLine("Waiting for offer from remote peer...");
        }

      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }
    public void SendMessage(string message)
    {
      if (dc != null && dc.State == DataChannel.ChannelState.Open)
      {
        dc.SendMessage(Encoding.UTF8.GetBytes(message));
      }
    }

    private async void setupDataChannel(DataChannel channel)
    {
      //if (dc != null && dc.Label == channel.Label) return;
      //Console.WriteLine($"Event: DataChannel Added {channel.Label}");
      dc = channel;

      dc.MessageReceived += (byte[] b) =>
      {
        var text = Encoding.UTF8.GetString(b);
        var dcJn = JsonSerializer.Deserialize<DcJN>(text);
        dcJn.fromId = remoteId;
        Received?.Invoke(this, dcJn);
      };
      dc.StateChanged += () => {
          //Console.WriteLine($"DataChannel '{channel.Label}':  StateChanged '{channel.State}'");
      };
    }

    private void PeerConnection_IceCandidateReadytoSend(IceCandidate candidate)
    {
      //Console.WriteLine($"ice\n{candidate.SdpMid}\n{candidate.SdpMlineIndex}\n{candidate.Content}\n\n");
      OnSend?.Invoke(this, MsgJN.Ice(candidate));
    }

    private void PeerConnection_LocalSdpReadytoSend(SdpMessage message)
    {
      //Console.WriteLine($"sdp\n{typeStr}\n{message.Content}\n\n");
      OnSend?.Invoke(this, MsgJN.Sdp(message));
    }

    private void PeerConnection_DataChannelAdded(DataChannel channel)
    {
      //Console.WriteLine($"Event: DataChannel Added {channel.Label}");
      setupDataChannel(channel);

    }

    private void PeerConnection_DataChannelRemoved(DataChannel channel)
    {
      dc = null;
      //Console.WriteLine($"Event: DataChannel Removed {channel.Label}");
    }
    public async void MsgReceived(PackageJN pkt)
    {
      var type = pkt.msg.type;
      if (type == "sdp" || type == "ice")
      {
        SignalingReceived(pkt);
        return;
      }

      if (type == MsgJN.Type.Handshake)
      {
        if (pkt.msg.content == "done")
        {
          Start();
        }
      }
    }

    public async void SignalingReceived(PackageJN pkt)
    {
      int retries = 30;
      while (pc == null)
      {
        await Task.Delay(300);
        if (Cannelled) return;
        retries--;
        if (retries <= 0) return;
      }

      if (pkt.msg.type == "sdp")
      {
        SdpMessageReceived(pkt.msg.sdp?.toSdp());
      }
      else if (pkt.msg.type == "ice")
      {
        IceCandidateReceived(pkt.msg.ice?.ToIce());
      }
    }
    private async void SdpMessageReceived(SdpMessage message)
    {
      if (message == null) return;
      await pc.SetRemoteDescriptionAsync(message);
      if (message.Type == SdpMessageType.Offer)
      {
        pc.CreateAnswer();
      }
    }

    private void IceCandidateReceived(IceCandidate candidate)
    {
      if (candidate == null) return;
      pc.AddIceCandidate(candidate);
    }

    public void Dispose()
    {
      Cannelled = true;
      if (pc != null)
      {
        pc.LocalSdpReadytoSend -= PeerConnection_LocalSdpReadytoSend;
        pc.DataChannelAdded -= PeerConnection_DataChannelAdded;
        pc.DataChannelRemoved -= PeerConnection_DataChannelRemoved;
        pc.IceCandidateReadytoSend -= PeerConnection_IceCandidateReadytoSend;
        pc.Close();
        pc.Dispose();
      }
      localAudioTrack?.Dispose();
    }
  }
}
