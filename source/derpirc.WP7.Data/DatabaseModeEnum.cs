﻿
namespace derpirc.Data
{
    public enum DatabaseMode
    {
        Default = 0,
        ReadWrite = 1,
        ReadOnly = 2,
        Exclusive = 3,
        SharedRead = 4,
    }
}
