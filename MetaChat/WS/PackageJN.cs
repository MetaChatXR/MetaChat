using System.Linq;
using System.Text.Json;

namespace MetaChat
{
  public class PackageJN
  {
    public MetaJN meta;
    public PeersJN peers;
    public string user;
    public MsgJN msg;

    public static PackageJN JN(JsonDocument jd)
    {
      var root = jd.RootElement;
      var meta = root.GetProperty("m");
      var payload = root.GetProperty("p");
      var type = meta.GetProperty("o").GetString();
      var from = meta.GetProperty("f").GetString();

      var r = new PackageJN
      {
        meta = new MetaJN
        {
          type = type,
          from = from
        }
      };


      if (type == MetaJN.Type.Peers)
      {
        r.peers = new PeersJN
        {
          users = payload.GetProperty("users").EnumerateArray().Select(u => u.GetString()).ToList()
        };

      }
      else if (type == MetaJN.Type.PeerConnected)
      {
        r.user = payload.GetString();
      }
      else if (type == MetaJN.Type.PeerRemoved)
      {
        r.user = payload.GetString();
      }
      else if (type == MetaJN.Type.Message)
      {
        r.msg = JsonSerializer.Deserialize<MsgJN>(payload.GetString());
      }

      return r;
    }

  }
}
