using Microsoft.MixedReality.WebRTC;
using System.Collections.Generic;
using System.Linq;

namespace MetaChat
{
  public class ChatInfoJN
  {
    public string channel { get; set; }
    public string userK { get; set; }
    public string host { get; set; }
    public string token { get; set; }
    public IceServerJN iceServers { get; set; }
    public List<IceServer> GetIceServers()
    {
      return new List<IceServer> { iceServers.To() };
    }
  }
  public class ChatterJN
  {
    public string atom { get; set; }

  }
  public class ChatJN
  {
    public string id { get; set; }
    public ChatterJN me { get; set; }

  }
  public class CreateChatJN
  {
    public ChatJN chat { get; set; }
    public ChatInfoJN channel { get; set; }
    public ChatInfoJN audio { get; set; }
  }
}
