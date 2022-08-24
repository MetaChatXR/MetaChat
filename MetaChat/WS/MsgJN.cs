using Microsoft.MixedReality.WebRTC;

namespace MetaChat
{
  public class MsgJN
  {
    public static class Type
    {
      public static string Sdp { get { return "sdp"; } }
      public static string Ice { get { return "ice"; } }
      public static string Handshake { get { return "handshake"; } }
    }

    public string type { get; set; }
    public SdpMessageJN sdp { get; set; }
    public IceCandidateJN ice { get; set; }
    public string content { get; set; }

    public static MsgJN Sdp(SdpMessage message)
    {
      return new MsgJN
      {
        type = Type.Sdp,
        sdp = SdpMessageJN.Sdp(message)
      };
    }
    public static MsgJN Ice(IceCandidate candidate)
    {
      return new MsgJN
      {
        type = Type.Ice,
        ice = IceCandidateJN.Ice(candidate)
      };
    }
    public static MsgJN Handshake(string content)
    {
      return new MsgJN
      {
        type = Type.Handshake,
        content = content
      };
    }
  }
}
