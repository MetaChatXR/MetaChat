using System.Collections.Generic;

namespace MetaChat
{
  public class RtcJN
  {
    public List<SyncPeerJN> peers { get; set; }
    public RtcJN()
    {
      peers = new List<SyncPeerJN> { };
    }
  }
}
