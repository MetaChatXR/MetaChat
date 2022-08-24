using Microsoft.MixedReality.WebRTC;
using System.Collections.Generic;

namespace MetaChat
{
  public class IceServerJN
  {
    public List<string> urls { get; set; }
    public string username { get; set; }
    public string credential { get; set; }
    public IceServer To()
    {
      var i = new IceServer { Urls = urls };
      if (username != null) i.TurnUserName = username;
      if (credential != null) i.TurnPassword = credential;
      return i;
    }
  }
}
