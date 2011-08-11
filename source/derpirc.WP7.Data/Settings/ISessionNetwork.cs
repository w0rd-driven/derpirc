using System.Collections.Generic;

namespace derpirc.Data.Settings
{
    public interface ISessionNetwork
    {
        int BasedOnId { get; set; }
        Network Network { get; set; }
        int SessionId { get; set; }
        Session Session { get; set; }
        ICollection<SessionServer> Servers { get; set; }
    }
}
