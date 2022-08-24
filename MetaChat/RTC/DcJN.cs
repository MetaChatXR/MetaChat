using System.Collections.Generic;

namespace MetaChat
{
  public class DcJN
  {
    public static class Type
    {
      public static string Sync { get { return "sync"; } }
    }
    public string type { get; set; }
    public string fromId { get; set; }
    public SyncJN sync { get; set; }

  }
}