using Microsoft.MixedReality.WebRTC;

namespace MetaChat
{
  public class SdpMessageJN
  {
    public string Type { get; set; }
    public string Content { get; set; }

    public static SdpMessageJN Sdp(SdpMessage message)
    {
      return new SdpMessageJN
      {
        Type = SdpMessage.TypeToString(message.Type),
        Content = message.Content
      };
    }
    public SdpMessage toSdp()
    {
      if (Type == null) return null;
      return new SdpMessage
      {
        Type = SdpMessage.StringToType(Type),
        Content = Content
      };
    }
  }
}