namespace MetaChat
{
  public class MetaJN
  {
    public static class Type
    {
      public static string Peers { get { return "peers"; } }
      public static string PeerConnected { get { return "peer_connected"; } }
      public static string PeerRemoved { get { return "peer_removed"; } }
      public static string Message { get { return "message"; } }
    }

    public string type;
    public string from;
    public string fromId => from.Split("/").Length > 1 ? from.Split("/")[1] : from.Split("/")[0];
  }

}
