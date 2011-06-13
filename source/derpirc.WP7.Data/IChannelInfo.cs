
namespace derpirc.Data
{
    /// <summary>
    /// Used by internal IRC subsystem to rejoin password-protected channels
    /// </summary>
    interface IChannelInfo
    {
        /// <summary>
        /// 1:1 Id to ChannelView for Name lookup
        /// </summary>
        int ListId { get; set; }
        /// <summary>
        /// IRC channel password
        /// </summary>
        string Key { get; set; }
    }
}
