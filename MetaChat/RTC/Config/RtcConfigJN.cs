using Microsoft.MixedReality.WebRTC;
using System.Collections.Generic;
using System.Linq;

namespace MetaChat
{
  public class RtcConfigJN
  {
    public string selfId { get; set; }
    public string host { get; set; }
    public string token { get; set; }
    public List<IceServerJN> IceServers { get; set; }

    public List<IceServer> GetIceServers()
    {
      return IceServers.Select(i => i.To()).ToList();
    }
  }
}
