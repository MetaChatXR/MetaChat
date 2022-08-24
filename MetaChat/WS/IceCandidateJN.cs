using Microsoft.MixedReality.WebRTC;

namespace MetaChat
{
  public class IceCandidateJN
  {
    public string SdpMid { get; set; }
    public int SdpMlineIndex { get; set; }
    public string Content { get; set; }
    public static IceCandidateJN Ice(IceCandidate candidate)
    {
      return new IceCandidateJN
      {
        SdpMid = candidate.SdpMid,
        SdpMlineIndex = candidate.SdpMlineIndex,
        Content = candidate.Content
      };
    }
    public IceCandidate ToIce()
    {
      return new IceCandidate
      {
        SdpMid = SdpMid,
        SdpMlineIndex = SdpMlineIndex,
        Content = Content
      };
    }
  }
}
