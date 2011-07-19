using System.Collections.Generic;

namespace derpirc.Data.Settings
{
    public interface ISession
    {
        ICollection<SessionNetwork> Networks { get; set; }
        ICollection<SessionServer> Servers { get; set; }
    }
}