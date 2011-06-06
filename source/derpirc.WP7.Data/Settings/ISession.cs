using System.Collections.Generic;

namespace derpirc.Data.Settings
{
    public interface ISession
    {
        List<Network> Networks { get; set; }
        List<Server> Servers { get; set; }
    }
}