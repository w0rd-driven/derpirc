using System;

namespace derpirc.Data
{
    public enum DbState
    {
        Undefined = 0,
        Initialized = 1,
        WipePending = 2,
        Disconnected = 3,
    }
}
